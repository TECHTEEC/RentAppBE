using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAppBE.Migrations
{
    /// <inheritdoc />
    public partial class MakeIBANUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IBAN",
                table: "UserProfiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_IBAN",
                table: "UserProfiles",
                column: "IBAN",
                unique: true,
                filter: "[IBAN] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_IBAN",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "IBAN",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
