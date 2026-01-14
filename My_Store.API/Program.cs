
using System.Security.Claims;
using System.Text;
using CloudinaryDotNet;      
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using My_Store.API.Cloudinary;
using My_Store.API.Middlewares;
using My_Store.Application.Common.Settings;
using My_Store.Infrastructure.Extensions;
using Serilog;

namespace My_Store.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


                    Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();
                    builder.Host.UseSerilog();
                    // Add services to the container.

            try
            {

                builder.Services.AddControllers();
                builder.Services.AddHttpContextAccessor();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddInfrastructure(builder.Configuration);

                // JWT

                builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    var jwt = builder.Configuration.GetSection("JwtSettings");
                    var secretKey = jwt["SecretKey"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey!)
                        ),
                        ValidateIssuer = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwt["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;

                            // 🔥 IMPORTANT: skip token validation for refresh
                            if (path.StartsWithSegments("/api/Auth/refresh"))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("JWT AUTH FAILED:");
                            Console.WriteLine(context.Exception.Message);
                            return Task.CompletedTask;
                        }
                    };
                });



                // Razorpay Service

                builder.Services.Configure<RazorpaySettings>(builder.Configuration.GetSection("Razorpay"));

                // Cloudinary Part

                builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
                builder.Services.AddSingleton(sp =>
                {
                    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                    return new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(config.CloudName, config.ApiKey, config.ApiSecret));
                });

                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                    });
                });

                var app = builder.Build();

                app.UseSerilogRequestLogging();

                // Global exception handling middleware - must be added early in pipeline

                app.UseMiddleware<GlobalExceptionMiddleware>();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseCors();
                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();

            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
              

            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
