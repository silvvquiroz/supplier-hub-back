using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddActivoToProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Proveedores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Proveedores");
        }
    }
}
