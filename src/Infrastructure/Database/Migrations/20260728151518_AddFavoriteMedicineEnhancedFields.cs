using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteMedicineEnhancedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "verified_name",
                schema: "public",
                table: "favorite_medicines",
                newName: "generic_name");

            migrationBuilder.AddColumn<string>(
                name: "brand_name",
                schema: "public",
                table: "favorite_medicines",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "doctor_specialty",
                schema: "public",
                table: "favorite_medicines",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dose",
                schema: "public",
                table: "favorite_medicines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "duration",
                schema: "public",
                table: "favorite_medicines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "frequency",
                schema: "public",
                table: "favorite_medicines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_favorite_medicines_doctor_specialty",
                schema: "public",
                table: "favorite_medicines",
                column: "doctor_specialty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_favorite_medicines_doctor_specialty",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.DropColumn(
                name: "brand_name",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.DropColumn(
                name: "doctor_specialty",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.DropColumn(
                name: "dose",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.DropColumn(
                name: "duration",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.DropColumn(
                name: "frequency",
                schema: "public",
                table: "favorite_medicines");

            migrationBuilder.RenameColumn(
                name: "generic_name",
                schema: "public",
                table: "favorite_medicines",
                newName: "verified_name");
        }
    }
}
