using Api.Middlewares;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la Base de Datos (SQL Server LocalDB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configurar CORS (Vital para que Blazor pueda conectarse)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin() // En producción se usa .WithOrigins("url")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 3. Inyección de Dependencias (Capas Application e Infrastructure)
builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware de Manejo de Excepciones
app.UseMiddleware<ExceptionMiddleware>();

// 4. Ejecutar el Seeder automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 5. Aplicar la política de CORS
app.UseCors("AllowBlazor");

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }