using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeCentres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "districts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_districts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "moh_areas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    district_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moh_areas", x => x.id);
                    table.ForeignKey(
                        name: "fk_moh_areas_districts_district_id",
                        column: x => x.district_id,
                        principalSchema: "public",
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "places",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    moh_area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_places", x => x.id);
                    table.ForeignKey(
                        name: "fk_places_moh_areas_moh_area_id",
                        column: x => x.moh_area_id,
                        principalSchema: "public",
                        principalTable: "moh_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "practice_centres",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    place_id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinic_name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    max_patients = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_practice_centres", x => x.id);
                    table.ForeignKey(
                        name: "fk_practice_centres_doctor_accounts_doctor_id",
                        column: x => x.doctor_id,
                        principalSchema: "public",
                        principalTable: "doctor_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_practice_centres_places_place_id",
                        column: x => x.place_id,
                        principalSchema: "public",
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nurses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practice_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nurses", x => x.id);
                    table.ForeignKey(
                        name: "fk_nurses_practice_centres_practice_centre_id",
                        column: x => x.practice_centre_id,
                        principalSchema: "public",
                        principalTable: "practice_centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_groups",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practice_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    days_of_week_raw = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_groups_practice_centres_practice_centre_id",
                        column: x => x.practice_centre_id,
                        principalSchema: "public",
                        principalTable: "practice_centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "time_blocks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_time_blocks", x => x.id);
                    table.ForeignKey(
                        name: "fk_time_blocks_session_groups_session_group_id",
                        column: x => x.session_group_id,
                        principalSchema: "public",
                        principalTable: "session_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_moh_areas_district_id",
                schema: "public",
                table: "moh_areas",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "ix_nurses_practice_centre_id",
                schema: "public",
                table: "nurses",
                column: "practice_centre_id");

            migrationBuilder.CreateIndex(
                name: "ix_places_moh_area_id_name",
                schema: "public",
                table: "places",
                columns: new[] { "moh_area_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_practice_centres_doctor_id",
                schema: "public",
                table: "practice_centres",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "ix_practice_centres_place_id",
                schema: "public",
                table: "practice_centres",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_groups_practice_centre_id",
                schema: "public",
                table: "session_groups",
                column: "practice_centre_id");

            migrationBuilder.CreateIndex(
                name: "ix_time_blocks_session_group_id",
                schema: "public",
                table: "time_blocks",
                column: "session_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nurses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "time_blocks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "session_groups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "practice_centres",
                schema: "public");

            migrationBuilder.DropTable(
                name: "places",
                schema: "public");

            migrationBuilder.DropTable(
                name: "moh_areas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "districts",
                schema: "public");
        }
    }
}
