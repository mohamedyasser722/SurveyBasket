namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection Services,IConfiguration Configuration)
    {
        // Add services to the container.

        Services.AddControllers();
        
        Services.AddSwagger();

        #region MyServices

        Services.AddScoped<IPollService, PollService>();
        Services.RegisterMapsterConfiguration();
        Services.AddFluentValidation();
        Services.AddDataBase(Configuration);

        #endregion

        return Services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection Services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen();


        return Services;
    }
    public static IServiceCollection AddFluentValidation(this IServiceCollection Services)
    {
        Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();
        Services.AddFluentValidationAutoValidation();
        return Services;
    }

    public static IServiceCollection AddDataBase(this IServiceCollection Services, IConfiguration Configuration)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection") ??
           throw new InvalidOperationException("Connection String 'DefaultConnection' not found");
        Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        return Services;
    }
}
