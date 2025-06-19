using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAppBE.Migrations
{
    /// <inheritdoc />
    public partial class editotpentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OtpRecords");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "OtpRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PhoneOrEmail",
                table: "OtpRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ResendCount",
                table: "OtpRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArabicMsg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglisMsg = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OtpRecords");

            migrationBuilder.DropColumn(
                name: "PhoneOrEmail",
                table: "OtpRecords");

            migrationBuilder.DropColumn(
                name: "ResendCount",
                table: "OtpRecords");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OtpRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "OtpRecords",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
