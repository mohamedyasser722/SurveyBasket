
using Serilog;
using SurveyBasket.Api.Hangfire;

namespace SurveyBasket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register necessary services
        builder.Services.AddDependencies(builder.Configuration);

        // Configure Serilog
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        // Configure Hangfire Dashboard with custom basic authentication
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter(
                    app.Configuration.GetValue<string>("HangfireSettings:Username")!,
                    app.Configuration.GetValue<string>("HangfireSettings:Password")!)
            },
            DashboardTitle = "Survey Basket Dashboard",
            //IsReadOnlyFunc = context => true          // Uncomment this line to make the hangfire dashboard read-only
        });

        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var notoficationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        RecurringJob.AddOrUpdate("SendNewPollsNotification", () =>  notoficationService.SendNewPollsNotification(null), Cron.Daily);

        // Use CORS if needed
        app.UseCors();

        // Add custom exception handling (new in .NET 8 and above)
        app.UseExceptionHandler();

        // Map controllers
        app.MapControllers();

        // Start the application
        app.Run();
    }
}
