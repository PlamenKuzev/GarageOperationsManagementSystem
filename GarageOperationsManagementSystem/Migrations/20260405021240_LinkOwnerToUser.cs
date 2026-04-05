using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageOperationsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class LinkOwnerToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Owners",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Owners_ApplicationUserId",
                table: "Owners",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_AspNetUsers_ApplicationUserId",
                table: "Owners",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_AspNetUsers_ApplicationUserId",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Owners_ApplicationUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Owners");
        }
    }
}
