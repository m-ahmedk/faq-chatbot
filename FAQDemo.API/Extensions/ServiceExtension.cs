using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FAQDemo.API.Converters;
using FAQDemo.API.DTOs.Auth;
using FAQDemo.API.Helpers;
using FAQDemo.API.Repositories;
using FAQDemo.API.Repositories.Interfaces;
using FAQDemo.API.Services;
using FAQDemo.API.Services.Interfaces;
using FAQDemo.API.Validators.Auth;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FAQDemo.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddCustomBinders(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new CleanStringModelBinderProvider());
            });

            return services;
        }

        public static IServiceCollection AddCustomJsonConverters(this IServiceCollection services)
        {
            services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new CleanStringJsonConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                        )
                    };
                });

            services.AddAuthorization(); //  DI registration

            return services;
        }

        public static IServiceCollection AddSwaggerDocument(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });

                // Add JWT Auth option
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like: Bearer {your token}"
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
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddProjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmbeddingRepository, EmbeddingRepository>();

            return services;
        }

        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<IPromptService, PromptService>();

            return services;
        }

        public static IServiceCollection AddProjectValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>(); // registers all validators
            //services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
            //services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();
            //services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            //services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();


            return services;
        }

        public static IServiceCollection AddProjectMappings(this IServiceCollection services)
        {
            // Build a LoggerFactory from DI
            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

            var config = new MapperConfiguration(cfg =>
            {
                // globally ignore EF base entity properties, managed by system (AppDbContext)
                cfg.AddGlobalIgnore("CreatedAt");
                cfg.AddGlobalIgnore("LastModifiedAt");
                cfg.AddGlobalIgnore("IsDeleted");
                cfg.AddGlobalIgnore("DeletedAt");

                // scan assembly for Profiles
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            }, loggerFactory);

            // validate mappings at startup (fail fast if misconfigured)
            config.AssertConfigurationIsValid();

            // register IMapper so it can be injected
            services.AddSingleton(config.CreateMapper());

            return services;
        }
    }
}