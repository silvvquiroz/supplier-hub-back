using Microsoft.EntityFrameworkCore;
using SupplierHubAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde las variables de entorno de Azure
var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Configurar el DbContext para usar SQL Server con la cadena de conexión desde las variables de entorno
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

// Configurar la API externa para las consultas a las listas de riesgo
builder.Services.AddControllers();
builder.Services.AddHttpClient("ExternalApi", client =>{
    client.BaseAddress = new Uri("https://scrapx-app.thankfulocean-d0a214e1.eastus.azurecontainerapps.io/api/");
});

var app = builder.Build();

// Configurar el pipeline de la app
app.UseAuthorization();

app.MapControllers();

app.Run();
