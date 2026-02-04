namespace SupplierHubAPI.Models
{
    // Modelo de datos para la respuesta de cada una de las fuentes
    public class ScrapXResponse<T>
    {
        public int Code {get; set; }
        public string Message {get; set; }
        public int NumHits {get; set; }
        public List<T> Results {get; set; }
    }
}