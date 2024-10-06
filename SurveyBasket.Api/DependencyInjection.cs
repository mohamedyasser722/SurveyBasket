

using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Health;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection Services,IConfiguration Configuration)
    {
        // Add services to the container.

        Services.AddControllers();


        // add caching
        Services.AddDistributedMemoryCache();

        // allow CORS

        Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string[]>());
                });
        });

        Services.AddSwagger();

        #region MyServices

        Services.AddScoped<IPollService, PollService>();
        Services.AddScoped<IJwtProvider, JwtProvider>();
        Services.AddScoped<IQuestionService, QuestionService>();
        Services.AddScoped<IVoteService, VoteService>();
        Services.AddScoped<IResultService, ResultService>();
        Services.AddScoped<ICacheService,CacheService>();
        Services.AddScoped<IEmailSender, EmailService>();
        Services.AddScoped<INotificationService, NotificationService>();
        Services.AddScoped<IUserService, UserService>();
        Services.AddScoped<IRoleService, RoleService>();
        Services.AddHangFire(Configuration);
        Services.AddHttpContextAccessor();
        Services.RegisterMapsterConfiguration();
        Services.AddFluentValidation();
        Services.AddDataBase(Configuration);
        Services.AddAuthConfig(Configuration);

        Services.AddExceptionHandler<GlobalExceptionHandler>();
        Services.AddProblemDetails();

        //Services.Configure<MailSettings>(Configuration.GetSection(nameof(MailSettings)));
        Services.AddOptions<MailSettings>()
            .BindConfiguration(nameof(MailSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        Services.AddHealthChecks()
            .AddSqlServer(name: "database", connectionString: Configuration.GetConnectionString("DefaultConnection")!)
            .AddHangfire(options => {options.MinimumAvailableServers = 1;})
            .AddCheck<MailProviderHealthCheck>(name: "mail service");

        Services.AddRateLimiter();

        Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });


        #endregion

        return Services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection Services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen();

        Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Survey Basket", Version = "v1" });

            // Define the BearerAuth scheme that's in use
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
                });
        });


        return Services;
    }
    public static IServiceCollection AddFluentValidation(this IServiceCollection Services)
    {
        Services.AddValidatorsFromAssemblyContaining<PollRequest>();
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
    public static IServiceCollection AddAuthConfig(this IServiceCollection Services,IConfiguration configuration)
    {

        Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        Services.AddScoped<IAuthService, AuthService>();

        //Services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        Services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var Jwtsettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        

        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwtsettings?.Key!)),
                ValidIssuer = Jwtsettings?.Issuer,
                ValidAudience = Jwtsettings?.Audience
            };
        });

        Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return Services;
    }

    // register HangFire 
    public static IServiceCollection AddHangFire(this IServiceCollection Services, IConfiguration Configuration)
    {
        Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        Services.AddHangfireServer();

        return Services;
    }

    // Add Rate Limitter
    public static IServiceCollection AddRateLimiter(this IServiceCollection Services)
    {
        Services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(RateLimiterPolicyNames.ipLimit, httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(

                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }

                )


            );

            rateLimiterOptions.AddPolicy(RateLimiterPolicyNames.userLimit, httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(

                        partitionKey: httpContext.User.GetUserId(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }

                )


            );

            rateLimiterOptions.AddConcurrencyLimiter(RateLimiterPolicyNames.concurrency, configureOptions =>
            {
                configureOptions.PermitLimit = 1000;
                configureOptions.QueueLimit = 100;
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            //rateLimiterOptions.AddTokenBucketLimiter(RateLimiterPolicyNames.tokenBucket, configureOptions =>
            // {
            //     configureOptions.TokenLimit = 2;
            //     configureOptions.QueueLimit = 1;
            //     configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //     configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //     configureOptions.TokensPerPeriod = 2;
            //     configureOptions.AutoReplenishment = true;
            // });

            //rateLimiterOptions.AddFixedWindowLimiter(RateLimiterPolicyNames.fixedWindow, configureOptions =>
            //{
            //    configureOptions.PermitLimit = 2;
            //    configureOptions.QueueLimit = 1;
            //    configureOptions.Window = TimeSpan.FromSeconds(20);
            //    configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            //rateLimiterOptions.AddSlidingWindowLimiter(RateLimiterPolicyNames.slidingWindow, configureOptions =>
            //{
            //    configureOptions.PermitLimit = 2;
            //    configureOptions.QueueLimit = 1;
            //    configureOptions.SegmentsPerWindow = 2;
            //    configureOptions.Window = TimeSpan.FromSeconds(20);
            //    configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});
        });

        return Services;
    }
}
