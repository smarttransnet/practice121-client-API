using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressAndRegistrationNumberToPlace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "public",
                table: "places",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "registration_number",
                schema: "public",
                table: "places",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "session_id",
                schema: "public",
                table: "PatientQueueTicket",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                schema: "public",
                table: "places");

            migrationBuilder.DropColumn(
                name: "registration_number",
                schema: "public",
                table: "places");

            migrationBuilder.DropColumn(
                name: "session_id",
                schema: "public",
                table: "PatientQueueTicket");
        }
    }
}
