using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace CustomerSuppTicket.API.MiddlewareExtensions
{
    public static class ExceptionHandlerExtensions
    {
        public static void UseGlobalExceptionHandler(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                return;
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    app.Use(async (context, next) =>
                    {
                        try
                        {
                            await next();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Unhandled exception occurred");

                            context.Response.ContentType = "application/json";

                            // קביעת סטטוס בהתאם לסוג השגיאה
                            int statusCode;
                            if (ex is UnauthorizedAccessException)
                                statusCode = StatusCodes.Status401Unauthorized;
                            else if (ex is KeyNotFoundException)
                                statusCode = StatusCodes.Status404NotFound;
                            else
                                statusCode = StatusCodes.Status500InternalServerError; 

                            context.Response.StatusCode = statusCode;

                            // החזרת הודעה רק אם השגיאה היא 500
                            if (statusCode == StatusCodes.Status500InternalServerError)
                            {
                                await context.Response.WriteAsJsonAsync(new
                                {
                                    Message = "An internal server error occurred. Please try again later."
                                });
                            }
                        }
                    });

                });
            });
        }
    }
}
