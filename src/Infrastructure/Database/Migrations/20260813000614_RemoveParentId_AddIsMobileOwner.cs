using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveParentId_AddIsMobileOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patient_accounts_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.DropIndex(
                name: "ix_patient_accounts_parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.AddColumn<bool>(
                name: "is_mobile_owner",
                schema: "public",
                table: "patient_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_mobile_owner",
                schema: "public",
                table: "patient_accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "public",
                table: "patient_accounts",
                type: "uuid",
                nullable: true);

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
        }
    }
}
