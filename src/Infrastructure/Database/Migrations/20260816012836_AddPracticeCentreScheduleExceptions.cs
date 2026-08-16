using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeCentreScheduleExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "specific_date",
                schema: "public",
                table: "session_groups",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "practice_centre_days_off",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practice_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_practice_centre_days_off", x => x.id);
                    table.ForeignKey(
                        name: "fk_practice_centre_days_off_practice_centres_practice_centre_id",
                        column: x => x.practice_centre_id,
                        principalSchema: "public",
                        principalTable: "practice_centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_practice_centre_days_off_practice_centre_id",
                schema: "public",
                table: "practice_centre_days_off",
                column: "practice_centre_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "practice_centre_days_off",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "specific_date",
                schema: "public",
                table: "session_groups");
        }
    }
}
