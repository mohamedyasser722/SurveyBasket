namespace SurveyBasket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddDependencies(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.UseExceptionHandler();  // new way in from .net 8 and above

        //app.MapIdentityApi<ApplicationUser>();    // old way in .net 7 and below

        app.Run();
    }
}

