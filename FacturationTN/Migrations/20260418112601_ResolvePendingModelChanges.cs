using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturationTN.Migrations
{
    /// <inheritdoc />
    public partial class ResolvePendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Clients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Clients",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}
