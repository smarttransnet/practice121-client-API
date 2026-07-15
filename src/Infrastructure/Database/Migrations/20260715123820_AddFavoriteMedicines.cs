using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteMedicines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorite_medicines",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    verified_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorite_medicines", x => x.id);
                    table.ForeignKey(
                        name: "fk_favorite_medicines_doctor_accounts_doctor_id",
                        column: x => x.doctor_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_favorite_medicines_doctor_id",
                schema: "public",
                table: "favorite_medicines",
                column: "doctor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_medicines",
                schema: "public");
        }
    }
}
