using AutoMapper;
using CustomerSuppTicket.API.Controllers;
using CustomerSuppTicket.Common.Intefaces.Repositories;
using CustomerSuppTicket.Common.Intefaces.Repositoy;
using CustomerSuppTicket.Common.Intefaces.Services;
using CustomerSuppTicket.Common.Mappings;
using CustomerSuppTicket.Repository.Repositories;
using CustomerSuppTicket.Services.Options;
using CustomerSuppTicket.Services.Services;
using CustomerSuppTicket.Services.Services.AiServices;
using CustomerSuppTicketEntity.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);


// AutoMapper
builder.Services.AddAutoMapper(typeof(TicketProfile));

// Repositories
var jsonDbPath = builder.Configuration.GetValue<string>("JsonDbPath");
builder.Services.AddSingleton<ITicketRepository>(new JsonTicketRepository(jsonDbPath));

var jsonUserDbPath = builder.Configuration.GetValue<string>("JsonUserDbPath");
builder.Services.AddSingleton<IUserRepository>(new JsonUserRepository(jsonUserDbPath));

// Options
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.Configure<MistralOptions>(builder.Configuration.GetSection("Mistral"));

// Services
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<ISummarizerService, MistralSummarizerClient>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddAppCors(builder.Environment);


var app = builder.Build();


 app.UseAppCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapTicketEndpoints();

app.Run();
