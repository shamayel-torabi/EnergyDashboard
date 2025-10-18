using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterService.Infrastructure.Persistence.Migrations;

public partial class UpdateNewAppDb : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "start_operation_date",
            table: "meters",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "end_operation_date",
            table: "meters",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "start_operation_date",
            table: "meters",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "end_operation_date",
            table: "meters",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "timestamp with time zone");
    }
}
