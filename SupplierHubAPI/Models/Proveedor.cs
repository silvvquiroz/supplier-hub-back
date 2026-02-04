using System.ComponentModel.DataAnnotations;

namespace SupplierHubAPI.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El nombre comercial es obligatorio.")]
        public string NombreComercial { get; set; }

        [Required(ErrorMessage = "La identificación tributaria es obligatoria.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "La identificación tributaria debe tener 11 dígitos.")]
        public string IdentificacionTributaria { get; set; }

        [Phone(ErrorMessage = "El número telefónico no tiene un formato válido.")]
        public string? NumeroTelefonico { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string? CorreoElectronico { get; set; }
        
        [Url(ErrorMessage = "El sitio web no tiene un formato válido.")]
        public string? SitioWeb { get; set; }

        public string? DireccionFisica { get; set; }

        [Required(ErrorMessage = "El país es obligatorio.")]
        public string Pais { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "La facturación anual debe ser mayor a cero.")]
        public decimal FacturacionAnual { get; set; }

        public DateTime FechaUltimaEdicion { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado de activo es obligatorio.")]
        public bool Activo {get; set; } = true;
    }
}
