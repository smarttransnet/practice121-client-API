using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokenToDoctorAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_token_expiry",
                schema: "public",
                table: "doctor_accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_reset_token_hash",
                schema: "public",
                table: "doctor_accounts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_token_expiry",
                schema: "public",
                table: "doctor_accounts");

            migrationBuilder.DropColumn(
                name: "password_reset_token_hash",
                schema: "public",
                table: "doctor_accounts");
        }
    }
}
