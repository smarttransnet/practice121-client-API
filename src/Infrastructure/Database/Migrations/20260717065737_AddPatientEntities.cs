using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patient_accounts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nic_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    mobile_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    verified = table.Column<bool>(type: "boolean", nullable: false),
                    completion_status = table.Column<int>(type: "integer", nullable: false),
                    created_by_doctor_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_accounts_doctor_accounts_created_by_doctor_id",
                        column: x => x.created_by_doctor_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "patient_documents",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    file_data = table.Column<byte[]>(type: "bytea", nullable: true),
                    content_type = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    archived_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_documents_patient_accounts_patient_account_id",
                        column: x => x.patient_account_id,
                        principalSchema: "public",
                        principalTable: "patient_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patient_accounts_created_by_doctor_id",
                schema: "public",
                table: "patient_accounts",
                column: "created_by_doctor_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_accounts_nic_number",
                schema: "public",
                table: "patient_accounts",
                column: "nic_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_patient_documents_patient_account_id",
                schema: "public",
                table: "patient_documents",
                column: "patient_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_documents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "patient_accounts",
                schema: "public");
        }
    }
}
