using AutoMapper;
using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.Intefaces.Repositories;
using CustomerSuppTicket.Common.Intefaces.Repositoy;
using CustomerSuppTicket.Common.Intefaces.Services;
using CustomerSuppTicket.Common.Mappings;
using CustomerSuppTicket.Common.ViewModels;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Repository.Data;
using CustomerSuppTicket.Repository.Repositories;
using CustomerSuppTicket.Services.Options;
using CustomerSuppTicket.Services.Services;
using CustomerSuppTicket.Services.Services.AiServices;
using CustomerSuppTicketEntity.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("jwtAuthKey")); // מחליפים למפתח מאובטח

builder.Services.AddAuthentication(options =>
{
    // The default scheme used to authenticate requests
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // The default scheme used to challenge unauthorized requests
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    // If true, requires HTTPS for the metadata endpoint (set false for development)
    options.RequireHttpsMetadata = false;

    // If true, saves the token in AuthenticationProperties after a successful authorization
    options.SaveToken = true;

    // Parameters used to validate the JWT token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the token issuer (who created the token)
        ValidateIssuer = false, // false: issuer not checked, true: validate issuer

        // Validate the token audience (who the token is intended for)
        ValidateAudience = false, // false: audience not checked, true: validate audience

        // Validate the signing key (ensures token integrity)
        ValidateIssuerSigningKey = true,

        // The key used to sign the token (symmetric key in this example)
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(TicketProfile));

// Register JSON-backed repository instead of EF Core DbContext
var jsonDbPath = builder.Configuration.GetValue<string>("JsonDbPath");
builder.Services.AddSingleton<ITicketRepository>(sp => new JsonTicketRepository(jsonDbPath));

var jsonUserDbPath = builder.Configuration.GetValue<string>("JsonUserDbPath");
builder.Services.AddSingleton<IUserRepository>(sp => new JsonUserRepository(jsonUserDbPath));


var sendGridApiKey = builder.Configuration.GetValue<string>("SendGrid:ApiKey");
var sendGridFrom = builder.Configuration.GetValue<string>("SendGrid:FromEmail");


builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.Configure<MistralOptions>(builder.Configuration.GetSection("Mistral"));

// Register email sender (MailKit implementation)
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();

builder.Services.AddScoped<ISummarizerService, MistralSummarizerClient>();
// Register service
builder.Services.AddScoped<ITicketService, TicketService>();
// Register service
builder.Services.AddScoped<IUserService, UserService>();

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

app.MapPost("/api/register", async (UserDto login, IUserService service) =>
{
    UserDto user = await service.CreateAsync(login);

    var tokenHandler = new JwtSecurityTokenHandler();

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Username),
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString });
});
app.MapPost("/api/login", async (LoginViewModel login, IUserService service) =>
{
    bool isLogIn =await service.LoginAsync(new UserDto { Username = login.Username, Password = login.Password });

    if(!isLogIn)
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

app.MapPut("/api/tickets", async (TicketUpdateStatusViewModel updated, ITicketService service) =>
{
    var ticket = await service.UpdateStatusAsync(updated);
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
}).RequireAuthorization();

app.MapPut("/api/tickets/bulk-update", async (
    List<TicketUpdateStatusViewModel> updatedTickets,
    ITicketService service) =>
{
    if (updatedTickets == null || updatedTickets.Count == 0)
        return Results.BadRequest("No tickets received.");

    var results = await service.UpdateStatusBulkAsync(updatedTickets);

    return Results.Ok(results);
}).RequireAuthorization();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
