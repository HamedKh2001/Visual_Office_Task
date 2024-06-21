using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Services;
using VO.Application.Contracts.Persistence;
using VO.Infrastructure.Persistence;
using VO.Infrastructure.Repositories.Common;

namespace VO.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            
            // services.AddDbContextPool<ApplicationWriteDbContext>(options =>
            // {
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            //             o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            //         .EnableSensitiveDataLogging();
            // });
            //
            // services.AddDbContextPool<ApplicationReadDbContext>(options =>
            // {
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            //             o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            //         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            //         .EnableSensitiveDataLogging();
            // });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            ConfigureAuthentication(services, configuration);
            ConfigureSwaggerGen(services);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            services.AddTransient<IDistributedCacheWrapper, DistributedCacheWrapper>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowAll", options => options
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowAnyOrigin());
            });

            return services;
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["BearerTokens:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["BearerTokens:Key"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(configuration.GetValue<int>("BearerTokens:ClockSkew"))
                };
            });
        }
        private static void ConfigureSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HCM-Swagger", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme,Id = "Bearer"}},
                        new string[] {}
                    }
                });
            });
        }
    }
}