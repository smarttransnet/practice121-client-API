using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientQueueTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientQueueTicket",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    queue_number = table.Column<int>(type: "integer", nullable: false),
                    queue_order = table.Column<int>(type: "integer", nullable: false),
                    patient_mobile = table.Column<string>(type: "text", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practice_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_date = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    called_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_queue_ticket", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patient_queue_ticket_practice_centre_id_doctor_id_visit_date_",
                schema: "public",
                table: "PatientQueueTicket",
                columns: new[] { "practice_centre_id", "doctor_id", "visit_date", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientQueueTicket",
                schema: "public");
        }
    }
}
