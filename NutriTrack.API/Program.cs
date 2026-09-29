using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NutriTrack.API.GeneracionReportesPdf;
using NutriTrack.API.Services;
using NutriTrack.Infraestructure.Data;
using NutriTrack.Infraestructure.Repositories;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RegistroPesoRepository>();
builder.Services.AddScoped<RodeoRepository>();
builder.Services.AddScoped<AnimalRepository>();
builder.Services.AddScoped<PlanAlimenticioRepository>();
builder.Services.AddScoped<AltaAnimalRepository>();
builder.Services.AddScoped<ConsultaFichaIndividualAnimalRepository>();
builder.Services.AddScoped<IngredienteRepository>();
builder.Services.AddScoped<DesactivacionReactivacionAnimalRepository>();
builder.Services.AddScoped<PlanRodeoAsignacionRepository>();
builder.Services.AddScoped<EdicionFichaAnimalRepository>();
builder.Services.AddScoped<MedicamentoRepository>();
builder.Services.AddScoped<TransferenciaAnimalesRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<EliminarRodeoRepository>();
builder.Services.AddScoped<EventoSanitarioRepository>();
builder.Services.AddScoped<ReporteInventarioAnimalesRepository>();
builder.Services.AddScoped<IReportePdfService, ReportePdfService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();