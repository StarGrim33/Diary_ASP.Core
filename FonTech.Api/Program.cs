using FonTech.Application.DependencyInjection;
using FonTech.DAL.DependencyInjection;
using Serilog;

namespace FonTech.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSwagger();

            builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));
            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddApplication();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", name: "FonTech Swagger v1.0");
                    s.SwaggerEndpoint("/swagger/v2/swagger.json", name: "FonTech Swagger v2.0");
                    s.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSerilogRequestLogging();
            app.Run();
        }
    }
}
