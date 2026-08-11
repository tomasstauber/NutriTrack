using Microsoft.EntityFrameworkCore;
using NutriTrack.Infraestructure.Data;
using NutriTrack.Infraestructure.Repositories;
using NutriTrack.Infraestructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RegistroPesoRepository>();
builder.Services.AddScoped<RodeoRepository>();
builder.Services.AddScoped<AnimalRepository>();
builder.Services.AddScoped<PlanAlimenticioRepository>();
builder.Services.AddScoped<AltaAnimalRepository>();
builder.Services.AddScoped<ConsultaFichaIndividualAnimalRepository>();
builder.Services.AddScoped<IngredienteRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<PlanRodeoAsignacionRepository>();
builder.Services.AddScoped<EdicionFichaAnimalRepository>();

builder.Services.AddScoped<AlertaPesoService>();
builder.Services.AddScoped<EventoSanitarioRepository>();
builder.Services.AddScoped<AlertaSanitariaService>();
builder.Services.AddHostedService<AlertaSanitariaWorker>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Permite trabajar con fechas locales con Npgsql.
AppContext.SetSwitch(
    "Npgsql.EnableLegacyTimestampBehavior",
    true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
