using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChildPatientSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_patient_accounts_nic_number",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "patient_id",
                schema: "public",
                table: "PatientQueueTicket",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nic_number",
                schema: "public",
                table: "patient_accounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "public",
                table: "patient_accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_patient_queue_ticket_patient_id",
                schema: "public",
                table: "PatientQueueTicket",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_accounts_nic_number",
                schema: "public",
                table: "patient_accounts",
                column: "nic_number",
                unique: true,
                filter: "nic_number IS NOT NULL AND nic_number <> ''");

            migrationBuilder.CreateIndex(
                name: "ix_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "fk_patient_accounts_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts",
                column: "parent_id",
                principalSchema: "public",
                principalTable: "patient_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_patient_queue_ticket_patient_accounts_patient_id",
                schema: "public",
                table: "PatientQueueTicket",
                column: "patient_id",
                principalSchema: "public",
                principalTable: "patient_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patient_accounts_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_patient_queue_ticket_patient_accounts_patient_id",
                schema: "public",
                table: "PatientQueueTicket");

            migrationBuilder.DropIndex(
                name: "ix_patient_queue_ticket_patient_id",
                schema: "public",
                table: "PatientQueueTicket");

            migrationBuilder.DropIndex(
                name: "ix_patient_accounts_nic_number",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.DropIndex(
                name: "ix_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.DropColumn(
                name: "patient_id",
                schema: "public",
                table: "PatientQueueTicket");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.AlterColumn<string>(
                name: "nic_number",
                schema: "public",
                table: "patient_accounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_patient_accounts_nic_number",
                schema: "public",
                table: "patient_accounts",
                column: "nic_number",
                unique: true);
        }
    }
}
