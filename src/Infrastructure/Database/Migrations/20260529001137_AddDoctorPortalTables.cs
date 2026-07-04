using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorPortalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_locations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientPortalUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_attendance_logs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_auth_credentials",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_commission_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_documents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_location_updates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_notifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_trip_assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "expense_report_line_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "fuel_cost_allocations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inspection_photos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inspection_results",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InvoiceLineItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InvoicePayments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InvoiceReminderLogs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InvoiceTripLinks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OutstandingInvoiceSnapshots",
                schema: "public");

            migrationBuilder.DropTable(
                name: "QuotationLineItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ReportFormatColumns",
                schema: "public");

            migrationBuilder.DropTable(
                name: "salary_expense_lines",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trailers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_custom_fields",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_halts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_pod_uploads",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_status_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vehicle_fuel_summaries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "woqood_card_mappings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "work_order_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "work_order_status_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_gps_logs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "monthly_expense_reports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "woqood_fuel_transactions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "checklist_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OutstandingInvoiceReports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_expenses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "driver_salary_records",
                schema: "public");

            migrationBuilder.DropTable(
                name: "custom_field_definitions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_vouchers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trip_stops",
                schema: "public");

            migrationBuilder.DropTable(
                name: "work_orders",
                schema: "public");

            migrationBuilder.DropTable(
                name: "woqood_import_batches",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InvoiceReportFormats",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Quotations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "drivers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "trips",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vehicle_inspections",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "import_batches",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inspection_checklists",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vehicle_categories",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "doctor_accounts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    auth_provider = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    google_sub_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_doctor_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_profiles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    slmc_reg_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nic_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mobile_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    qualifications = table.Column<string[]>(type: "text[]", nullable: false),
                    specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    profile_picture_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    completion_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_doctor_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_doctor_profiles_doctor_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "otp_sessions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    otp_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    verified = table.Column<bool>(type: "boolean", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_otp_sessions_doctor_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "practice_identities",
                schema: "public",
                columns: table => new
                {
                    practice_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    barcode_data = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_practice_identities", x => x.practice_id);
                    table.ForeignKey(
                        name: "fk_practice_identities_doctor_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    file_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_documents_doctor_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "public",
                        principalTable: "doctor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "e_signatures",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signature_data_url = table.Column<string>(type: "text", nullable: false),
                    signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_e_signatures", x => x.id);
                    table.ForeignKey(
                        name: "fk_e_signatures_doctor_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "public",
                        principalTable: "doctor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verification_records",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slmc_verified = table.Column<bool>(type: "boolean", nullable: false),
                    qualifications_verified = table.Column<bool>(type: "boolean", nullable: false),
                    documents_verified = table.Column<bool>(type: "boolean", nullable: false),
                    badge_awarded = table.Column<bool>(type: "boolean", nullable: false),
                    audited_by = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_verification_records_doctor_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "public",
                        principalTable: "doctor_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_doctor_accounts_email",
                schema: "public",
                table: "doctor_accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_doctor_accounts_google_sub_id",
                schema: "public",
                table: "doctor_accounts",
                column: "google_sub_id",
                unique: true,
                filter: "google_sub_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_profiles_account_id",
                schema: "public",
                table: "doctor_profiles",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_doctor_profiles_nic_number",
                schema: "public",
                table: "doctor_profiles",
                column: "nic_number",
                unique: true,
                filter: "nic_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_doctor_profiles_slmc_reg_number",
                schema: "public",
                table: "doctor_profiles",
                column: "slmc_reg_number",
                unique: true,
                filter: "slmc_reg_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_documents_profile_id",
                schema: "public",
                table: "documents",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_e_signatures_profile_id",
                schema: "public",
                table: "e_signatures",
                column: "profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_otp_sessions_account_id",
                schema: "public",
                table: "otp_sessions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_practice_identities_account_id",
                schema: "public",
                table: "practice_identities",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_verification_records_profile_id",
                schema: "public",
                table: "verification_records",
                column: "profile_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "e_signatures",
                schema: "public");

            migrationBuilder.DropTable(
                name: "otp_sessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "practice_identities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "verification_records",
                schema: "public");

            migrationBuilder.DropTable(
                name: "doctor_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "doctor_accounts",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "asset_locations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_type = table.Column<int>(type: "integer", nullable: false),
                    is_assigned = table.Column<bool>(type: "boolean", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    location_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    client_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_person_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    payment_terms_days = table.Column<int>(type: "integer", nullable: false),
                    tax_registration_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "custom_field_definitions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    applies_to = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    default_value = table.Column<string>(type: "text", nullable: true),
                    field_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    field_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    field_type = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    validation_regex = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_custom_field_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "drivers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    employee_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    licence_expiry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    licence_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nationality_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sponsor_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drivers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "import_batches",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    error_summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    failure_count = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    success_count = table.Column<int>(type: "integer", nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_import_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inspection_checklists",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    applicable_vehicle_types = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspection_type = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_checklists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceReportFormats",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    column_configuration = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    footer_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    header_logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    show_shipping_address = table.Column<bool>(type: "boolean", nullable: false),
                    show_tax_breakdown = table.Column<bool>(type: "boolean", nullable: false),
                    show_trip_details = table.Column<bool>(type: "boolean", nullable: false),
                    template_file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_report_formats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "monthly_expense_reports",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exported_file_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    generated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    report_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_driver_expenses_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_fuel_cost_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_operational_cost_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_salary_cost_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_monthly_expense_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_categories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "woqood_import_batches",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    error_summary = table.Column<string>(type: "text", nullable: true),
                    failure_count = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    success_count = table.Column<int>(type: "integer", nullable: false),
                    total_amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_litres = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_woqood_import_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ClientPortalUsers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_portal_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_client_portal_users_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutstandingInvoiceReports",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_status = table.Column<int>(type: "integer", nullable: false),
                    exported_file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invoice_count = table.Column<int>(type: "integer", nullable: false),
                    oldest_invoice_date = table.Column<DateOnly>(type: "date", nullable: true),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_to_email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    total_outstanding_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outstanding_invoice_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_outstanding_invoice_reports_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    converted_to_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issued_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    quotation_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    sub_total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tax_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    terms_and_conditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quotations", x => x.id);
                    table.ForeignKey(
                        name: "fk_quotations_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "driver_attendance_logs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attendance_date = table.Column<DateOnly>(type: "date", nullable: false),
                    check_in_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_in_latitude = table.Column<double>(type: "double precision", nullable: true),
                    check_in_longitude = table.Column<double>(type: "double precision", nullable: true),
                    check_out_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_out_latitude = table.Column<double>(type: "double precision", nullable: true),
                    check_out_longitude = table.Column<double>(type: "double precision", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source = table.Column<int>(type: "integer", nullable: false),
                    total_hours_worked = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_attendance_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_attendance_logs_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_auth_credentials",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_locked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    platform = table.Column<int>(type: "integer", nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    refresh_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_auth_credentials", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_auth_credentials_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_documents",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    file_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    submitted_from_app = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_documents_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_expenses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    expense_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expense_type = table.Column<int>(type: "integer", nullable: false),
                    fuel_litres = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    fuel_station = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    odometer_reading = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    receipt_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_expenses", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_expenses_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_gps_logs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_speed_kmh = table.Column<float>(type: "real", nullable: true),
                    point_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    raw_track_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    session_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    session_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_distance_km = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_gps_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_gps_logs_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_notifications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notification_type = table.Column<int>(type: "integer", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_notifications_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_salary_records",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allowances_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    base_salary_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    commission_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deductions_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    net_payable_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    overtime_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    sponsor_approval_status = table.Column<int>(type: "integer", nullable: false),
                    sponsor_approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sponsor_approved_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_salary_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_salary_records_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "driver_trip_assignments",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accepted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assigned_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_status = table.Column<int>(type: "integer", nullable: false),
                    displayed_in_app_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_trip_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_trip_assignments_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    driver_confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    import_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_imported = table.Column<bool>(type: "boolean", nullable: false),
                    office_approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    office_approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    scheduled_start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_distance_km = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    trailer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    trip_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trips", x => x.id);
                    table.ForeignKey(
                        name: "fk_trips_import_batches_import_batch_id",
                        column: x => x.import_batch_id,
                        principalSchema: "public",
                        principalTable: "import_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "checklist_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_checklist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_checklist_items_inspection_checklists_inspection_checklist_",
                        column: x => x.inspection_checklist_id,
                        principalSchema: "public",
                        principalTable: "inspection_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportFormatColumns",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_report_format_id = table.Column<Guid>(type: "uuid", nullable: false),
                    column_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    format_pattern = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    width_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_format_columns", x => x.id);
                    table.ForeignKey(
                        name: "fk_report_format_columns_invoice_report_formats_invoice_report_for",
                        column: x => x.invoice_report_format_id,
                        principalSchema: "public",
                        principalTable: "InvoiceReportFormats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "expense_report_line_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    monthly_expense_report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    sub_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_report_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_expense_report_line_items_monthly_expense_reports_monthly_e",
                        column: x => x.monthly_expense_report_id,
                        principalSchema: "public",
                        principalTable: "monthly_expense_reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    current_driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    current_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    make = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    odometer_reading = table.Column<decimal>(type: "numeric", nullable: false),
                    plate_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    registration_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vehicle_type = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicles_vehicle_categories_vehicle_category_id",
                        column: x => x.vehicle_category_id,
                        principalSchema: "public",
                        principalTable: "vehicle_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "woqood_fuel_transactions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    woqood_import_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fuel_type = table.Column<int>(type: "integer", nullable: false),
                    is_allocated = table.Column<bool>(type: "boolean", nullable: false),
                    odometer = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    quantity_litres = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    raw_row_data = table.Column<string>(type: "text", nullable: true),
                    station_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    total_amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_price_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    woqood_card_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_woqood_fuel_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_woqood_fuel_transactions_woqood_import_batches_woqood_impor",
                        column: x => x.woqood_import_batch_id,
                        principalSchema: "public",
                        principalTable: "woqood_import_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    report_format_id = table.Column<Guid>(type: "uuid", nullable: true),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issued_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    outstanding_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    paid_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    sub_total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tax_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoices_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_invoice_report_formats_report_format_id",
                        column: x => x.report_format_id,
                        principalSchema: "public",
                        principalTable: "InvoiceReportFormats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "public",
                        principalTable: "Quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuotationLineItems",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    line_total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    service_type = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    tax_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    unit_price_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quotation_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_quotation_line_items_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "public",
                        principalTable: "Quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_location_updates",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accuracy = table.Column<float>(type: "real", nullable: true),
                    bearing = table.Column<float>(type: "real", nullable: true),
                    driver_gps_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    speed_kmh = table.Column<float>(type: "real", nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_location_updates", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_location_updates_driver_gps_logs_driver_gps_log_id",
                        column: x => x.driver_gps_log_id,
                        principalSchema: "public",
                        principalTable: "driver_gps_logs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_driver_location_updates_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "salary_expense_lines",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_expense_id = table.Column<Guid>(type: "uuid", nullable: true),
                    driver_salary_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_deduction = table.Column<bool>(type: "boolean", nullable: false),
                    line_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_salary_expense_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_salary_expense_lines_driver_expenses_driver_expense_id",
                        column: x => x.driver_expense_id,
                        principalSchema: "public",
                        principalTable: "driver_expenses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_salary_expense_lines_driver_salary_records_driver_salary_re",
                        column: x => x.driver_salary_record_id,
                        principalSchema: "public",
                        principalTable: "driver_salary_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_commission_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_salary_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    applied_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    calculation_basis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    commission_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_commission_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_commission_items_driver_salary_records_driver_salary",
                        column: x => x.driver_salary_record_id,
                        principalSchema: "public",
                        principalTable: "driver_salary_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_driver_commission_items_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "trip_halts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    halt_type = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    location_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    recorded_by_driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_halts", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_halts_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_status_histories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    previous_status = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_status_histories_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_stops",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_arrival_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_departure_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    location_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    poc_email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    poc_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    poc_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    scheduled_arrival_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    stop_order = table.Column<int>(type: "integer", nullable: false),
                    stop_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_stops", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_stops_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_vouchers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    voucher_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    voucher_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_vouchers", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_vouchers_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trailers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attached_vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    capacity = table.Column<decimal>(type: "numeric", nullable: false),
                    capacity_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    current_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_expenses_qar = table.Column<decimal>(type: "numeric", nullable: false),
                    total_revenue_qar = table.Column<decimal>(type: "numeric", nullable: false),
                    trailer_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    trailer_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trailers", x => x.id);
                    table.ForeignKey(
                        name: "fk_trailers_vehicles_attached_vehicle_id",
                        column: x => x.attached_vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_fuel_summaries",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_cost_per_litre_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    average_fuel_efficiency_km_per_l = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    driver_entry_count = table.Column<int>(type: "integer", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    total_cost_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_litres = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    woqood_transaction_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_fuel_summaries", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_fuel_summaries_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_inspections",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_checklist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_signature = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    driver_signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    inspected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspection_type = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    odometer_reading = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_inspections", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_inspections_inspection_checklists_inspection_checkl",
                        column: x => x.inspection_checklist_id,
                        principalSchema: "public",
                        principalTable: "inspection_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vehicle_inspections_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "woqood_card_mappings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    card_holder_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    mapped_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mapped_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    woqood_card_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_woqood_card_mappings", x => x.id);
                    table.ForeignKey(
                        name: "fk_woqood_card_mappings_drivers_driver_id",
                        column: x => x.driver_id,
                        principalSchema: "public",
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_woqood_card_mappings_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "fuel_cost_allocations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_expense_id = table.Column<Guid>(type: "uuid", nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    woqood_fuel_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allocated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allocation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    allocation_source = table.Column<int>(type: "integer", nullable: false),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    quantity_litres = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fuel_cost_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_fuel_cost_allocations_driver_expenses_driver_expense_id",
                        column: x => x.driver_expense_id,
                        principalSchema: "public",
                        principalTable: "driver_expenses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fuel_cost_allocations_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fuel_cost_allocations_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_fuel_cost_allocations_woqood_fuel_transactions_woqood_fuel_",
                        column: x => x.woqood_fuel_transaction_id,
                        principalSchema: "public",
                        principalTable: "woqood_fuel_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItems",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    line_total_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    service_type = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    tax_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    unit_price_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "public",
                        principalTable: "Invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    payment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    payment_method = table.Column<int>(type: "integer", nullable: false),
                    payment_reference = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    recorded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_payments_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "public",
                        principalTable: "Invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceReminderLogs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_error = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    delivery_status = table.Column<int>(type: "integer", nullable: false),
                    is_automated = table.Column<bool>(type: "boolean", nullable: false),
                    reminder_type = table.Column<int>(type: "integer", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sent_to_email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    triggered_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_reminder_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_reminder_logs_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "public",
                        principalTable: "Invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceTripLinks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    linked_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_completion_verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_trip_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_trip_links_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "public",
                        principalTable: "Invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invoice_trip_links_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutstandingInvoiceSnapshots",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    outstanding_invoice_report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aging_bucket = table.Column<int>(type: "integer", nullable: false),
                    aging_days = table.Column<int>(type: "integer", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    original_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    outstanding_amount_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outstanding_invoice_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "fk_outstanding_invoice_snapshots_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "public",
                        principalTable: "Invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_outstanding_invoice_snapshots_outstanding_invoice_reports_outst",
                        column: x => x.outstanding_invoice_report_id,
                        principalSchema: "public",
                        principalTable: "OutstandingInvoiceReports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_pod_uploads",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_stop_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uploaded_by_driver_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_pod_uploads", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_pod_uploads_trip_stops_trip_stop_id",
                        column: x => x.trip_stop_id,
                        principalSchema: "public",
                        principalTable: "trip_stops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trip_pod_uploads_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_custom_fields",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_voucher_id = table.Column<Guid>(type: "uuid", nullable: true),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_custom_fields", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_custom_fields_custom_field_definitions_field_definitio",
                        column: x => x.field_definition_id,
                        principalSchema: "public",
                        principalTable: "custom_field_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trip_custom_fields_trip_vouchers_trip_voucher_id",
                        column: x => x.trip_voucher_id,
                        principalSchema: "public",
                        principalTable: "trip_vouchers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trip_custom_fields_trips_trip_id",
                        column: x => x.trip_id,
                        principalSchema: "public",
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspection_photos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    photo_type = table.Column<int>(type: "integer", nullable: false),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uploaded_by_driver_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspection_photos_vehicle_inspections_vehicle_inspection_id",
                        column: x => x.vehicle_inspection_id,
                        principalSchema: "public",
                        principalTable: "vehicle_inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspection_results",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    checklist_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_passed = table.Column<bool>(type: "boolean", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspection_results_checklist_items_checklist_item_id",
                        column: x => x.checklist_item_id,
                        principalSchema: "public",
                        principalTable: "checklist_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inspection_results_vehicle_inspections_vehicle_inspection_id",
                        column: x => x.vehicle_inspection_id,
                        principalSchema: "public",
                        principalTable: "vehicle_inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_orders",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_inspection_id = table.Column<Guid>(type: "uuid", nullable: true),
                    actual_cost_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    assigned_technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    estimated_cost_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    work_order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_orders_vehicle_inspections_vehicle_inspection_id",
                        column: x => x.vehicle_inspection_id,
                        principalSchema: "public",
                        principalTable: "vehicle_inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_work_orders_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "public",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    item_type = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_cost_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    unit_cost_qar = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_items_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "public",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_order_status_histories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    previous_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_status_histories_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "public",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_items_inspection_checklist_id",
                schema: "public",
                table: "checklist_items",
                column: "inspection_checklist_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_portal_users_client_id",
                schema: "public",
                table: "ClientPortalUsers",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_portal_users_email",
                schema: "public",
                table: "ClientPortalUsers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_clients_client_code",
                schema: "public",
                table: "Clients",
                column: "client_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_clients_company_name",
                schema: "public",
                table: "Clients",
                column: "company_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_attendance_logs_driver_id",
                schema: "public",
                table: "driver_attendance_logs",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_attendance_logs_driver_id_attendance_date",
                schema: "public",
                table: "driver_attendance_logs",
                columns: new[] { "driver_id", "attendance_date" });

            migrationBuilder.CreateIndex(
                name: "ix_driver_auth_credentials_driver_id",
                schema: "public",
                table: "driver_auth_credentials",
                column: "driver_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_commission_items_driver_salary_record_id",
                schema: "public",
                table: "driver_commission_items",
                column: "driver_salary_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_commission_items_trip_id",
                schema: "public",
                table: "driver_commission_items",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_documents_driver_id",
                schema: "public",
                table: "driver_documents",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_expenses_driver_id",
                schema: "public",
                table: "driver_expenses",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_expenses_trip_id",
                schema: "public",
                table: "driver_expenses",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_gps_logs_driver_id",
                schema: "public",
                table: "driver_gps_logs",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_location_updates_driver_gps_log_id",
                schema: "public",
                table: "driver_location_updates",
                column: "driver_gps_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_location_updates_driver_id",
                schema: "public",
                table: "driver_location_updates",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_location_updates_driver_id_recorded_at",
                schema: "public",
                table: "driver_location_updates",
                columns: new[] { "driver_id", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "ix_driver_notifications_driver_id",
                schema: "public",
                table: "driver_notifications",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_salary_records_driver_id_period_year_period_month",
                schema: "public",
                table: "driver_salary_records",
                columns: new[] { "driver_id", "period_year", "period_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_trip_assignments_driver_id",
                schema: "public",
                table: "driver_trip_assignments",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_trip_assignments_trip_id",
                schema: "public",
                table: "driver_trip_assignments",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_drivers_email",
                schema: "public",
                table: "drivers",
                column: "email",
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_drivers_employee_number",
                schema: "public",
                table: "drivers",
                column: "employee_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_drivers_phone_number",
                schema: "public",
                table: "drivers",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_expense_report_line_items_monthly_expense_report_id",
                schema: "public",
                table: "expense_report_line_items",
                column: "monthly_expense_report_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuel_cost_allocations_driver_expense_id",
                schema: "public",
                table: "fuel_cost_allocations",
                column: "driver_expense_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuel_cost_allocations_trip_id",
                schema: "public",
                table: "fuel_cost_allocations",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuel_cost_allocations_vehicle_id",
                schema: "public",
                table: "fuel_cost_allocations",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuel_cost_allocations_woqood_fuel_transaction_id",
                schema: "public",
                table: "fuel_cost_allocations",
                column: "woqood_fuel_transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inspection_photos_vehicle_inspection_id",
                schema: "public",
                table: "inspection_photos",
                column: "vehicle_inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_results_checklist_item_id",
                schema: "public",
                table: "inspection_results",
                column: "checklist_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_results_vehicle_inspection_id",
                schema: "public",
                table: "inspection_results",
                column: "vehicle_inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_invoice_id",
                schema: "public",
                table: "InvoiceLineItems",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_trip_id",
                schema: "public",
                table: "InvoiceLineItems",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_payments_invoice_id",
                schema: "public",
                table: "InvoicePayments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_payments_payment_date",
                schema: "public",
                table: "InvoicePayments",
                column: "payment_date");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_reminder_logs_invoice_id",
                schema: "public",
                table: "InvoiceReminderLogs",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_reminder_logs_sent_at",
                schema: "public",
                table: "InvoiceReminderLogs",
                column: "sent_at");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_report_formats_name",
                schema: "public",
                table: "InvoiceReportFormats",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_client_id",
                schema: "public",
                table: "Invoices",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_invoice_number",
                schema: "public",
                table: "Invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_quotation_id",
                schema: "public",
                table: "Invoices",
                column: "quotation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_report_format_id",
                schema: "public",
                table: "Invoices",
                column: "report_format_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_trip_links_invoice_id",
                schema: "public",
                table: "InvoiceTripLinks",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_trip_links_invoice_id_trip_id",
                schema: "public",
                table: "InvoiceTripLinks",
                columns: new[] { "invoice_id", "trip_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoice_trip_links_trip_id",
                schema: "public",
                table: "InvoiceTripLinks",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_monthly_expense_reports_period_year_period_month_report_type",
                schema: "public",
                table: "monthly_expense_reports",
                columns: new[] { "period_year", "period_month", "report_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outstanding_invoice_reports_client_id",
                schema: "public",
                table: "OutstandingInvoiceReports",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_outstanding_invoice_reports_period_year_period_month",
                schema: "public",
                table: "OutstandingInvoiceReports",
                columns: new[] { "period_year", "period_month" });

            migrationBuilder.CreateIndex(
                name: "ix_outstanding_invoice_snapshots_invoice_id",
                schema: "public",
                table: "OutstandingInvoiceSnapshots",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_outstanding_invoice_snapshots_outstanding_invoice_report_id",
                schema: "public",
                table: "OutstandingInvoiceSnapshots",
                column: "outstanding_invoice_report_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotation_line_items_quotation_id",
                schema: "public",
                table: "QuotationLineItems",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotations_client_id",
                schema: "public",
                table: "Quotations",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotations_quotation_number",
                schema: "public",
                table: "Quotations",
                column: "quotation_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_report_format_columns_invoice_report_format_id",
                schema: "public",
                table: "ReportFormatColumns",
                column: "invoice_report_format_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_format_columns_invoice_report_format_id_column_key",
                schema: "public",
                table: "ReportFormatColumns",
                columns: new[] { "invoice_report_format_id", "column_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_salary_expense_lines_driver_expense_id",
                schema: "public",
                table: "salary_expense_lines",
                column: "driver_expense_id");

            migrationBuilder.CreateIndex(
                name: "ix_salary_expense_lines_driver_salary_record_id",
                schema: "public",
                table: "salary_expense_lines",
                column: "driver_salary_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_trailers_attached_vehicle_id",
                schema: "public",
                table: "trailers",
                column: "attached_vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_custom_fields_field_definition_id",
                schema: "public",
                table: "trip_custom_fields",
                column: "field_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_custom_fields_trip_id",
                schema: "public",
                table: "trip_custom_fields",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_custom_fields_trip_voucher_id",
                schema: "public",
                table: "trip_custom_fields",
                column: "trip_voucher_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_halts_trip_id",
                schema: "public",
                table: "trip_halts",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_pod_uploads_trip_id",
                schema: "public",
                table: "trip_pod_uploads",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_pod_uploads_trip_stop_id",
                schema: "public",
                table: "trip_pod_uploads",
                column: "trip_stop_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_status_histories_trip_id",
                schema: "public",
                table: "trip_status_histories",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_stops_trip_id",
                schema: "public",
                table: "trip_stops",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_vouchers_trip_id",
                schema: "public",
                table: "trip_vouchers",
                column: "trip_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trips_import_batch_id",
                schema: "public",
                table: "trips",
                column: "import_batch_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_fuel_summaries_vehicle_id_period_year_period_month",
                schema: "public",
                table: "vehicle_fuel_summaries",
                columns: new[] { "vehicle_id", "period_year", "period_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_inspections_inspection_checklist_id",
                schema: "public",
                table: "vehicle_inspections",
                column: "inspection_checklist_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_inspections_vehicle_id",
                schema: "public",
                table: "vehicle_inspections",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_vehicle_category_id",
                schema: "public",
                table: "vehicles",
                column: "vehicle_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_woqood_card_mappings_driver_id",
                schema: "public",
                table: "woqood_card_mappings",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_woqood_card_mappings_vehicle_id",
                schema: "public",
                table: "woqood_card_mappings",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_woqood_card_mappings_woqood_card_number",
                schema: "public",
                table: "woqood_card_mappings",
                column: "woqood_card_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_woqood_fuel_transactions_woqood_import_batch_id",
                schema: "public",
                table: "woqood_fuel_transactions",
                column: "woqood_import_batch_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_items_work_order_id",
                schema: "public",
                table: "work_order_items",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_status_histories_work_order_id",
                schema: "public",
                table: "work_order_status_histories",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_vehicle_id",
                schema: "public",
                table: "work_orders",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_vehicle_inspection_id",
                schema: "public",
                table: "work_orders",
                column: "vehicle_inspection_id");
        }
    }
}
