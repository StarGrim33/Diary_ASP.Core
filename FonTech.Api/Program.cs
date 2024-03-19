using Consumer.DependencyInjection;
using FonTech.Api.Middlewares;
using FonTech.Application.DependencyInjection;
using FonTech.DAL.DependencyInjection;
using FonTech.Domain.Settings;
using FonTech.Producer.DependencyInjection;
using Serilog;

namespace FonTech.Api
{
    /// <summary>
    /// Main program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings))); // Заполнение класса RabbitMqSettings из AppSettings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection)); // Заполнение класса JwtSettings из AppSettings
            builder.Services.AddAuthenticationAndAuthorization(builder);
            builder.Services.AddControllers();
            builder.Services.AddSwagger();
            builder.Services.AddMemoryCache();
            builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddProducer();
            builder.Services.AddConsumer();
            
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", name: "FonTech Swagger v1.0");
                    s.SwaggerEndpoint("/swagger/v2/swagger.json", name: "FonTech Swagger v2.0");
                });
            }

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSerilogRequestLogging();
            app.Run();
        }
    }
}
