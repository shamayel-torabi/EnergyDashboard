using MeterFailService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterFailService.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateNewAppDb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "meter_energys",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                serialnumber = table.Column<string>(name: "serial_number", type: "character varying(50)", maxLength: 50, nullable: false),
                recorddate = table.Column<DateTime>(name: "record_date", type: "timestamp without time zone", nullable: false),
                hourenergys = table.Column<List<HourEnergy>>(name: "hour_energys", type: "jsonb", nullable: false),
                anomaly = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meter_energys", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "meter_energys");
    }
}
