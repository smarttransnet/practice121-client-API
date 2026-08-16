using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SupportMultipleSpecificDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "specific_date",
                schema: "public",
                table: "session_groups");

            migrationBuilder.AddColumn<string>(
                name: "specific_dates_raw",
                schema: "public",
                table: "session_groups",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "specific_dates_raw",
                schema: "public",
                table: "session_groups");

            migrationBuilder.AddColumn<DateOnly>(
                name: "specific_date",
                schema: "public",
                table: "session_groups",
                type: "date",
                nullable: true);
        }
    }
}
