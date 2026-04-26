using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturationTN.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageDataUrl",
                table: "UserAccounts",
                type: "TEXT",
                maxLength: 4000000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageDataUrl",
                table: "UserAccounts");
        }
    }
}
