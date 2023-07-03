using System.Net.Mime;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Interfaces;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Domain.Options;
using PismoWebInput.Core.Infrastructure.Extensions;
using PismoWebInput.Core.Persistence.Contexts;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core;

public static class DependencyInjection
{
    public static void ConfigureContainer(this ContainerBuilder builder)
    {
        builder.RegisterModule(new AppModule());
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions(configuration);
        services.ConfigureAutoMapper(Assembly.GetExecutingAssembly());
        services.AddPersistence(configuration);
        services.AddIdentityServices(configuration);
        services.AddEmailServices(configuration);
        return services;
    }

    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ForwardedHeadersOptions>(x =>
            x.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
        services.Configure<AppOptions>(configuration.GetSection(AppOptions.SectionKey));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionKey));
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<EfContext>(x =>
            {
                x.UseSqlServer(configuration.GetConnectionString(nameof(EfContext)), b => b
                    .MigrationsAssembly(typeof(EfContext).Assembly.FullName)
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            })
            .AddScoped<IUnitOfWork<EfContext>, UnitOfWork>()
            .AddScoped<IEfUnitOfWork, EfUnitOfWork>();

        services
            .AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<EfContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static void AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        var mailOptions = configuration.GetSection(MailOptions.SectionKey).Get<MailOptions>();
        services
            .AddFluentEmail(mailOptions.DefaultFrom)
#if DEBUG
            .AddSmtpSender(mailOptions.Host, mailOptions.Port)
#else
            .AddSmtpSender(mailOptions.Host, mailOptions.Port, mailOptions.ApiKey, mailOptions.ApiSecret)
#endif
            ;
    }

    private static void AddIdentityServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionKey).Get<JwtOptions>();
        //services
        //    .AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddCookie(options =>
        //    {
        //        options.LoginPath = "/Account/Login";
        //    })
        //    .AddJwtBearer(x =>
        //    {
        //        x.ClaimsIssuer = jwtOptions.Issuer;
        //        x.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            IssuerSigningKey = jwtOptions.GetSecurityKey(),
        //            ValidateIssuerSigningKey = true,
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.Zero,
        //            ValidIssuer = jwtOptions.Issuer,
        //            ValidAudience = jwtOptions.Audience
        //        };

        //        x.Events = new JwtBearerEvents
        //        {
        //            OnAuthenticationFailed = context =>
        //            {
        //                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
        //                    context.Response.Headers.Add("Token-Expired", "true");

        //                return Task.CompletedTask;
        //            },
        //            OnChallenge = context =>
        //            {
        //                context.HandleResponse();
        //                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //                context.Response.ContentType = MediaTypeNames.Application.Json;

        //                var problem = ToProblem(context.HttpContext, StatusCodes.Status401Unauthorized,
        //                    "Unauthorized", "You are not authorized");
        //                return context.Response.WriteAsync(problem.ToJson());
        //            },
        //            OnForbidden = context =>
        //            {
        //                context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //                context.Response.ContentType = MediaTypeNames.Application.Json;

        //                var problem = ToProblem(context.HttpContext, StatusCodes.Status403Forbidden, "Forbidden",
        //                    "You are not authorized to access this resource");
        //                return context.Response.WriteAsync(problem.ToJson());
        //            },
        //            OnMessageReceived = context =>
        //            {
        //                var accessToken = context.Request.Query["access_token"];
        //                var path = context.HttpContext.Request.Path;
        //                if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hub"))
        //                    context.Token = accessToken;

        //                return Task.CompletedTask;
        //            }
        //        };
        //    });
    }

    private static ProblemDetails ToProblem(HttpContext context, int statusCode, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Detail = detail
        };
    }
}