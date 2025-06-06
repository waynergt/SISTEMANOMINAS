using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Services;
using QuestPDF.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;

namespace ProyectoNominas.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);

            // BD SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Servicios
            builder.Services.AddScoped<ReporteNominaService>();
            builder.Services.AddScoped<InformacionAcademicaService>();
            builder.Services.AddScoped<ExpedienteService>();
            builder.Services.AddScoped<NominaService>();

            // JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            // Sirve archivos estáticos
            app.UseStaticFiles(); // wwwroot

            // Archivos del expediente
            var documentosPath = Path.Combine(app.Environment.WebRootPath, "documentos");
            if (!Directory.Exists(documentosPath))
                Directory.CreateDirectory(documentosPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(documentosPath),
                RequestPath = "/documentos"
            });

            app.MapControllers();
            app.Run();
        }
    }
}
