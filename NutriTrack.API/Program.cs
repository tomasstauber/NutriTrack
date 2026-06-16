using Microsoft.EntityFrameworkCore;
using NutriTrack.Infraestructure.Data;
using NutriTrack.Infraestructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RegistroPesoRepository>();
builder.Services.AddScoped<CrearRodeoRepository>();
builder.Services.AddScoped<AnimalRepository>();
builder.Services.AddScoped<PlanAlimenticioRepository>();
builder.Services.AddScoped<AltaAnimalRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Esto le dice a Npgsql que acepte fechas locales como antes
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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