using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveDaysOffToSessionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "practice_centre_days_off",
                schema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "days_of_week_raw",
                schema: "public",
                table: "session_groups",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "session_group_days_off",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_group_days_off", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_group_days_off_session_groups_session_group_id",
                        column: x => x.session_group_id,
                        principalSchema: "public",
                        principalTable: "session_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_session_group_days_off_session_group_id",
                schema: "public",
                table: "session_group_days_off",
                column: "session_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "session_group_days_off",
                schema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "days_of_week_raw",
                schema: "public",
                table: "session_groups",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
    }
}
