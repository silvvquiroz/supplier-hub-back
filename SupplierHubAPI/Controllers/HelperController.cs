using Microsoft.AspNetCore.Mvc;
using SupplierHubAPI.Models;
using Newtonsoft.Json;

namespace SupplierHubAPI.Controllers
{
    // Controller para definir los endpoints de helpers
    [Route("api/[controller]")]
    [ApiController]
    public class HelperController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public HelperController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("paises")]
        public async Task<ActionResult<IEnumerable<string>>> GetPaises()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync("https://restcountries.com/v3.1/all?fields=name");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al obtener los países.");
                }

                // Leer la respuesta y obtener solo los nombres comunes de los países en español
                var jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Deserializamos solo los nombres de los países
                var paises = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse)
                                .Select(p => p.name.common.ToString()) // Solo obtenemos el nombre común
                                .ToList();

                return Ok(paises);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al obtener los países: {ex.Message}");
            }
        }

    }
}