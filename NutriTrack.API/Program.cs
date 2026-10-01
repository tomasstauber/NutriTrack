using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

//Leer configuraci�n jwt
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta Jwt:Key en la configuraci�n.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Falta Jwt:Issuer en la configuraci�n.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Falta Jwt:Audience en la configuraci�n.");
builder.Services.AddScoped <ReporteFechasImportantesRepository>();
builder.Services.AddScoped<ReporteEvolucionPesoRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

//Fallback policy funciona con principio de lista negra
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();