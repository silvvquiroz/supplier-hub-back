using Microsoft.EntityFrameworkCore;
using SupplierHubAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde las variables de entorno de Azure
var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Configurar el DbContext para usar SQL Server con la cadena de conexión desde las variables de entorno
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

// Agregar servicios CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        // Permitir solicitudes desde tu dominio en Vercel
        policy.WithOrigins("https://supplier-hub-front.vercel.app")  // Cambia esto por tu dominio de Vercel
              .AllowAnyMethod()  // Permitir todos los métodos HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permitir todos los encabezados
    });

    // Política para el entorno local
    options.AddPolicy("AllowLocal", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // Cambia esto por tu URL local
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar la API externa para las consultas a las listas de riesgo
builder.Services.AddControllers();
builder.Services.AddHttpClient("ExternalApi", client =>{
    client.BaseAddress = new Uri("https://scrapx-app.thankfulocean-d0a214e1.eastus.azurecontainerapps.io/api/");
});

var app = builder.Build();

// Configurar el pipeline de la app
app.UseAuthorization();

// Usar la política CORS correctamente
app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:5173", "https://supplier-hub-front.vercel.app") // Orígenes específicos permitidos
          .AllowAnyMethod()  // Permitir todos los métodos HTTP (GET, POST, etc.)
          .AllowAnyHeader(); // Permitir todos los encabezados
});


app.MapControllers();

app.Run();
