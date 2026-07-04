using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDbFileStorageAndQualifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "archived_at",
                schema: "public",
                table: "e_signatures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "public",
                table: "e_signatures",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "signature_content_type",
                schema: "public",
                table: "e_signatures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "signature_data",
                schema: "public",
                table: "e_signatures",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "archived_at",
                schema: "public",
                table: "documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "content_type",
                schema: "public",
                table: "documents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "file_data",
                schema: "public",
                table: "documents",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "public",
                table: "documents",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture_content_type",
                schema: "public",
                table: "doctor_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "profile_picture_data",
                schema: "public",
                table: "doctor_profiles",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "qualifications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    certificate_data = table.Column<byte[]>(type: "bytea", nullable: true),
                    certificate_content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    archived_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_qualifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_qualifications_doctor_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "public",
                        principalTable: "doctor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_qualifications_profile_id",
                schema: "public",
                table: "qualifications",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qualifications",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "archived_at",
                schema: "public",
                table: "e_signatures");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "public",
                table: "e_signatures");

            migrationBuilder.DropColumn(
                name: "signature_content_type",
                schema: "public",
                table: "e_signatures");

            migrationBuilder.DropColumn(
                name: "signature_data",
                schema: "public",
                table: "e_signatures");

            migrationBuilder.DropColumn(
                name: "archived_at",
                schema: "public",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "content_type",
                schema: "public",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "file_data",
                schema: "public",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "public",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "profile_picture_content_type",
                schema: "public",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "profile_picture_data",
                schema: "public",
                table: "doctor_profiles");
        }
    }
}
