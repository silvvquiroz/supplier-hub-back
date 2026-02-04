namespace SupplierHubAPI.Models
{
    // Modelo de resultado de la consulta a OffShore
    public class OffShoreResult
    {
        public string Entity { get; set; }
        public string Jurisdiction { get; set; }
        public string LinkedTo { get; set; }
        public string DataFrom { get; set; }
    }
}