using GestorFinanzasAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace GestorFinanzasAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var conexion = builder.Configuration
    .GetConnectionString("DefaultConnection");

            Console.WriteLine("¿Se encontró la conexión?: "
                + (conexion != null));

            Console.WriteLine("¿Se está usando el secreto correcto?: "
                + (conexion?.Contains("CAMBIAR_LOCALMENTE") == false
                   && conexion?.Contains("TU_CONTRASEÑA_REAL") == false));

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            // Conexión con PostgreSQL
            builder.Services.AddDbContext<FinanzasDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
    }}

