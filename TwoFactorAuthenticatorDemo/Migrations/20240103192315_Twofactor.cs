using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwoFactorAuthenticatorDemo.Migrations
{
    /// <inheritdoc />
    public partial class Twofactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorSetupInfo",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TwoFactorSecretKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorLoginVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorLoginVerified",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TwoFactorSecretKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSetupInfo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
