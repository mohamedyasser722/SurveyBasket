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

        app.UseAuthorization();

        //app.MapIdentityApi<ApplicationUser>();

        app.MapControllers();

        app.Run();
    }
}

