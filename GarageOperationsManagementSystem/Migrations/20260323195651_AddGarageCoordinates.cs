using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageOperationsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGarageCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Garages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Garages",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Garages");
        }
    }
}
