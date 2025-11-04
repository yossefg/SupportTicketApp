using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class CorsExtensions
{
    public static void AddAppCors(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AppCors", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                );
            });
        }
        else
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AppCors", builder =>
                    builder.WithOrigins(
                        "https://ticketProdDomain.com"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
        }
    }

    public static void UseAppCors(this WebApplication app)
    {
        app.UseCors("AppCors");
    }
}
