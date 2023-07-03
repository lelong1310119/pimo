using System.Net.Mime;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PismoWebInput.Core.Infrastructure.Common.Exceptions;
using PismoWebInput.Core.Infrastructure.Helpers;
using PismoWebInput.Core.Persistence.Contexts;
using Serilog;
using Serilog.Events;

namespace PismoWebInput.Core.Infrastructure.Extensions
{
    public static class StartupExtensions
    {
        private static readonly string[] ExposedHeaders =
            { "Token-Expired", "WWW-Authenticate", "X-Pagination" };

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.File(
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    path: Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Logs\log.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}")
#endif
                .CreateLogger();
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddControllersWithViews()
                ;
            builder.Services.AddSession();

            builder.Services
                .AddInfrastructure(builder.Configuration)
                .AddSwagger()
                .AddCorsPolicies()
                .AddHealthChecks(builder.Configuration)
                .AddApiBehaviorOptions()
                .AddProblemDetails(builder.Environment)
                .AddMemoryCache()
                .AddHttpClient()
                .AddHttpContextAccessor()
                ;
        }

        private static IServiceCollection AddCorsPolicies(this IServiceCollection services)
        {
            services.AddCors(x => x.AddDefaultPolicy(policy => policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .WithExposedHeaders(ExposedHeaders)
                .AllowCredentials()));
            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", _ => HealthCheckResult.Healthy())
                .AddSqlServer(configuration.GetConnectionString(nameof(EfContext)));
            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LBPB API", Version = "v1" });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API"
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                    });
                });

            return services;
        }

        private static IServiceCollection AddProblemDetails(this IServiceCollection services, IWebHostEnvironment env)
        {
            services
                .AddProblemDetails(options =>
                {
                    foreach (var header in ExposedHeaders) options.AllowedHeaderNames.Add(header);

                    // This is the default behavior; only include exception details in a development environment.
                    options.IncludeExceptionDetails = (_, _) => env.IsDevelopment();

                    options.Map<HttpRequestException>(ProblemMapper.StatusCode(StatusCodes.Status503ServiceUnavailable));
                    options.Map<SecurityTokenException>(ProblemMapper.StatusCode(StatusCodes.Status401Unauthorized));
                    options.Map<UnauthorizedException>(ProblemMapper.StatusCode(StatusCodes.Status401Unauthorized));
                    options.Map<ForbiddenException>(ProblemMapper.StatusCode(StatusCodes.Status403Forbidden));
                    options.Map<HttpStatusCodeException>(ProblemMapper.DynamicStatusCode);
                    options.Map(ProblemMapper.StatusCode(StatusCodes.Status500InternalServerError));
                });

            return services;
        }

        private static IServiceCollection AddApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problem = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}"
                    };

                    var result = new BadRequestObjectResult(problem);
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                    return result;
                };
            });
            return services;
        }
    }
}
