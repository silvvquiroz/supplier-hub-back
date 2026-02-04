namespace SupplierHubAPI.Models
{
    // Modelo de resultado de la consulta a Ofac
    public class OfacResult 
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Program { get; set; }
        public string List { get; set; }
        public string Score { get; set; }
    }
}
