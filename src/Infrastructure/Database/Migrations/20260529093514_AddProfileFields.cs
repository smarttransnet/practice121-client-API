using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bio",
                schema: "public",
                table: "doctor_profiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "public",
                table: "doctor_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                schema: "public",
                table: "doctor_profiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "public",
                table: "doctor_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bio",
                schema: "public",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "public",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "gender",
                schema: "public",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "public",
                table: "doctor_profiles");
        }
    }
}
