using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierHubAPI.Models;
using Newtonsoft.Json;

namespace SupplierHubAPI.Controllers
{

    // Controller para definir los endpoints de proveedores
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProveedorController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Proveedor/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            var proveedoresActivos = await _context.Proveedores.Where(p => p.Activo).ToListAsync();

            return proveedoresActivos;
        }

        // GET: api/Proveedor?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores(int page = 1, int pageSize = 10)
        {
            // Verificar que los parámetros son válidos
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // Calculamos el número de proveedores a omitir (paginación)
            var skip = (page - 1) * pageSize;

            // Obtener proveedores activos de manera paginada
            var proveedoresPaginados = await _context.Proveedores
                .Where(p => p.Activo)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Obtener el total de proveedores activos (para calcular el número total de páginas)
            var totalProveedores = await _context.Proveedores
                .Where(p => p.Activo)
                .CountAsync();

            // Calcular el número total de páginas
            var totalPages = (int)Math.Ceiling(totalProveedores / (double)pageSize);

            // Crear un objeto para devolver los resultados junto con la información de paginado
            var result = new
            {
                TotalProveedores = totalProveedores,
                TotalPages = totalPages,
                CurrentPage = page,
                Proveedores = proveedoresPaginados
            };

            return Ok(result);
        }


        // GET: api/Proveedor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }
            if (!proveedor.Activo) {
                return NotFound("Proveedor inactivo");
            }

            return proveedor;
        }

        // POST: api/Proveedor
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(Proveedor proveedor)
        {

            // Establecer valores predeterminados
            proveedor.Activo = true; 
            proveedor.FechaUltimaEdicion = DateTime.UtcNow;  


            // Verificar las validaciones del Model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProveedor", new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/Proveedor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest();
            }

            // Verificar las validaciones del Model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Desvincular la instancia actual del proveedor en el contexto
            // y validar si e proveedor realmente existe
            var existingProveedor = await _context.Proveedores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (existingProveedor == null) {
                return NotFound("Proveedor no encontrado.");
            }

            // Actualizar la fecha de última modificación
            proveedor.FechaUltimaEdicion = DateTime.UtcNow;

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Proveedores.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Proveedor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            // Cambiar el estado de Activo a false (eliminado lógico)
            proveedor.Activo = false;

            // Actualizar la fecha de última modificación
            proveedor.FechaUltimaEdicion = DateTime.UtcNow;

            // Actualizar el proveedor en la BD
            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("external/offshore/{entityName}")]
        public async Task<ActionResult> GetOffShoreData(string entityName) {
            var client = _httpClientFactory.CreateClient("ExternalApi");

            try
            {
                // Usar la BaseAddress configurada en Program.cs
                var offshoreUrl = $"offshore?entity={entityName}";
                var response = await client.GetAsync(offshoreUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al obtener los datos de offshore.");
                }

                // Leer la respuesta de la API externa
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta en la clase ScrapXResponse con el tipo específico (OffshoreResult)
                var apiResponse = JsonConvert.DeserializeObject<ScrapXResponse<OffShoreResult>>(jsonResponse);

                return Ok(apiResponse);
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"Error de conexión a la API externa: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error inesperado: {ex.Message}");
            }
        }

        [HttpGet("external/ofac/{entityName}")]
        public async Task<ActionResult> GetOfacData(string entityName)
        {
            var client = _httpClientFactory.CreateClient("ExternalApi");

            try
            {
                // Usar la BaseAddress configurada en Program.cs
                var ofacUrl = $"ofac?entity={entityName}&score=100";
                var response = await client.GetAsync(ofacUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al obtener los datos de OFAC.");
                }

                // Leer la respuesta de la API de ScrapX
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta en la clase ScrapXResponse con el tipo específico (OfacResult)
                var apiResponse = JsonConvert.DeserializeObject<ScrapXResponse<OfacResult>>(jsonResponse);

                return Ok(apiResponse);
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"Error de conexión a la API externa: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error inesperado: {ex.Message}");
            }
        }


    }
}
