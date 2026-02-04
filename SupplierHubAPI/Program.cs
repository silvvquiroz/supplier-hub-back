using Microsoft.EntityFrameworkCore;
using SupplierHubAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar el DbContext para usar SQL server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
