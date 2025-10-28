
using Microsoft.Extensions.Options;
using My_Store.API.Middlewares;
using My_Store.Infrastructure.Extensions;
using Serilog;
using My_Store.API.Cloudinary;
using CloudinaryDotNet;      
using CloudinaryDotNet.Actions;

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
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddInfrastructure(builder.Configuration);

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
                          .AllowAnyHeader();
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
