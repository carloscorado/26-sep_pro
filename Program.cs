using Microsoft.EntityFrameworkCore;
using ApiEmpresa.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Obtener la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("ConexionMySQL");

// Configurar DbContext usando Pomelo MySQL
builder.Services.AddDbContext<Conexiones>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();