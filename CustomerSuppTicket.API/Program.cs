using AutoMapper;
using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.Intefaces;
using CustomerSuppTicket.Common.Mappings;
using CustomerSuppTicket.Common.ViewModels;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Repository.Data;
using CustomerSuppTicket.Repository.Repositories;
using CustomerSuppTicket.Services.Options;
using CustomerSuppTicket.Services.Services;
using CustomerSuppTicket.Services.Services.AiServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("jwtAuthKey")); // מחליפים למפתח מאובטח

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(TicketProfile));

// Register JSON-backed repository instead of EF Core DbContext
var jsonDbPath = builder.Configuration.GetValue<string>("JsonDbPath") ?? "Data/tickets.json";
builder.Services.AddSingleton<ITicketRepository>(sp => new JsonTicketRepository(jsonDbPath));


var sendGridApiKey = builder.Configuration.GetValue<string>("SendGrid:ApiKey");
var sendGridFrom = builder.Configuration.GetValue<string>("SendGrid:FromEmail");


builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.Configure<MistralOptions>(builder.Configuration.GetSection("Mistral"));

// Register email sender (MailKit implementation)
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();

builder.Services.AddScoped<ISummarizerService, MistralSummarizerClient>();
// Register service
builder.Services.AddScoped<ITicketService, TicketService>();

// Swagger (only enabled in Development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors("AllowAll");

// Enable Swagger UI in Development environment only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapPost("/api/register", (RegisterViewModel login) =>
{

    var tokenHandler = new JwtSecurityTokenHandler();

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, login.Username)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString });
});
app.MapPost("/api/login", (LoginViewModel login) =>
{
    if (login.Username != "a" || login.Password != "a")
        return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, login.Username)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString });
});


//minimal api`s
app.MapPost("/api/tickets", async (TicketViewModel t, ITicketService service, IMapper mapper) =>
{
    // Map TicketViewModel to TicketDto using AutoMapper
    var dto = mapper.Map<TicketDto>(t);

    var created = await service.CreateAsync(dto);
    return Results.Created($"/api/tickets/{created.Id}", created);
});

app.MapGet("/api/tickets", async (ITicketService service) => await service.GetAllAsync())
    .RequireAuthorization();

app.MapGet("/api/tickets/{id:guid}", async (Guid id, ITicketService service) =>
 await service.GetByIdAsync(id) is TicketDto t ? Results.Ok(t) : Results.NotFound());

app.MapPut("/api/tickets", async ( TicketUpdateStatusViewModel updated, ITicketService service) =>
{
    var ticket = await service.UpdateStatusAsync(updated);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
}).RequireAuthorization();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
