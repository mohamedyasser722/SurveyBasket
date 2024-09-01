using Serilog;

namespace SurveyBasket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddDependencies(builder.Configuration);

        // adding serilog to the application
        builder.Host.UseSerilog((context, configuration) =>
        {
            //configuration
            //.MinimumLevel.Information()
            //.WriteTo.Console();

            // i will make it read from the appsettings.json file
            configuration.ReadFrom
            .Configuration(context.Configuration);

        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();     // makes serilog log the request show the request in the console

        app.UseHttpsRedirection();

        app.UseCors();         

        app.MapControllers();

        app.UseExceptionHandler();  // new way in from .net 8 and above

        //app.MapIdentityApi<ApplicationUser>();    // old way in .net 7 and below

        app.Run();
    }
}

