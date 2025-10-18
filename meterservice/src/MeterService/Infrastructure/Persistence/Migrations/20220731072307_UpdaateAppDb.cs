using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterService.Infrastructure.Persistence.Migrations;

public partial class UpdaateAppDb : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "meters",
            columns: table => new
            {
                meter_id = table.Column<int>(type: "integer", nullable: false),
                serial_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                station_id = table.Column<int>(type: "integer", nullable: true),
                station_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                dismount = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                start_operation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                end_operation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meters", x => x.meter_id);
            });

        migrationBuilder.CreateTable(
            name: "meter_energys",
            columns: table => new
            {
                meter_energy_id = table.Column<Guid>(type: "uuid", nullable: false),
                meter_id = table.Column<int>(type: "integer", nullable: false),
                record_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                tool_type_id = table.Column<int>(type: "integer", nullable: true),
                energy_active_export = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                energy_active_import = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                energy_reactive_export = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                energy_reactive_import = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meter_energys", x => x.meter_energy_id);
                table.ForeignKey(
                    name: "fk_meter_energys_meters_meter_id",
                    column: x => x.meter_id,
                    principalTable: "meters",
                    principalColumn: "meter_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_meter_energys_meter_id",
            table: "meter_energys",
            column: "meter_id");

        migrationBuilder.CreateIndex(
            name: "ix_meter_energys_record_date",
            table: "meter_energys",
            column: "record_date");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "meter_energys");

        migrationBuilder.DropTable(
            name: "meters");
    }
}
