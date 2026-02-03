using Microsoft.EntityFrameworkCore;

namespace SupplierHubAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Proveedor> Proveedores { get; set; } // Definimos la tabla 'Proveedores'

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Especificamos la precisión y escala para el campo FacturacionAnual
            modelBuilder.Entity<Proveedor>()
                .Property(p => p.FacturacionAnual)
                .HasColumnType("decimal(18,2)"); // 18 dígitos en total, 2 después del punto decimal
        }
    }
}
