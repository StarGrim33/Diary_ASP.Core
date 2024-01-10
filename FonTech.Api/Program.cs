using FonTech.DAL.DependencyInjection;
using FonTech.Application.DependencyInjection;
using Serilog;

namespace FonTech.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddApplication();
            builder.Host.UseSerilog((context, configuration)
                => configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
