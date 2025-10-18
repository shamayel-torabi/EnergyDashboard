using System;
using EnergyDashboard.Domain.Diagram;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnergyDashboard.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_tariffs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    startdate = table.Column<DateTime>(name: "start_date", type: "timestamp with time zone", nullable: false),
                    enddate = table.Column<DateTime>(name: "end_date", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_energy_tariffs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_operation_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    equipmenttype = table.Column<int>(name: "equipment_type", type: "integer", nullable: false),
                    operationname = table.Column<string>(name: "operation_name", type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_operation_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "load_feeder_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_load_feeder_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "networks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    diagram = table.Column<DiagramModel>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_networks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "powerplant_operators",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_powerplant_operators", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "powerplant_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_powerplant_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "energy_tariff_rate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    energytariffid = table.Column<Guid>(name: "energy_tariff_id", type: "uuid", nullable: false),
                    starttime = table.Column<TimeSpan>(name: "start_time", type: "interval", nullable: false),
                    endtime = table.Column<TimeSpan>(name: "end_time", type: "interval", nullable: false),
                    rate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_energy_tariff_rate", x => new { x.id, x.energytariffid });
                    table.ForeignKey(
                        name: "fk_energy_tariff_rate_energy_tariffs_energy_tariff_id",
                        column: x => x.energytariffid,
                        principalTable: "energy_tariffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    diagram = table.Column<DiagramModel>(type: "jsonb", nullable: false),
                    networkid = table.Column<Guid>(name: "network_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_areas", x => x.id);
                    table.ForeignKey(
                        name: "fk_areas_networks_network_id",
                        column: x => x.networkid,
                        principalTable: "networks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "zones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    diagram = table.Column<DiagramModel>(type: "jsonb", nullable: false),
                    areaid = table.Column<Guid>(name: "area_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_zones", x => x.id);
                    table.ForeignKey(
                        name: "fk_zones_areas_area_id",
                        column: x => x.areaid,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "substations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    diagram = table.Column<DiagramModel>(type: "jsonb", nullable: false),
                    igmcstationid = table.Column<int>(name: "igmc_station_id", type: "integer", nullable: false, defaultValue: 0),
                    igmccode = table.Column<string>(name: "igmc_code", type: "character varying(50)", maxLength: 50, nullable: true),
                    dispatchingcode = table.Column<string>(name: "dispatching_code", type: "character varying(50)", maxLength: 50, nullable: true),
                    mountdate = table.Column<DateTimeOffset>(name: "mount_date", type: "timestamp with time zone", nullable: true),
                    dismountdate = table.Column<DateTimeOffset>(name: "dismount_date", type: "timestamp with time zone", nullable: true),
                    zoneid = table.Column<Guid>(name: "zone_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_substations", x => x.id);
                    table.ForeignKey(
                        name: "fk_substations_zones_zone_id",
                        column: x => x.zoneid,
                        principalTable: "zones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CTRatioPrimary = table.Column<int>(name: "CTRatio.Primary", type: "integer", nullable: true),
                    CTRatioSecondary = table.Column<int>(name: "CTRatio.Secondary", type: "integer", nullable: true),
                    PTRatioPrimary = table.Column<int>(name: "PTRatio.Primary", type: "integer", nullable: true),
                    PTRatioSecondary = table.Column<int>(name: "PTRatio.Secondary", type: "integer", nullable: true),
                    voltage = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    reverse = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    @virtual = table.Column<bool>(name: "virtual", type: "boolean", nullable: false, defaultValue: false),
                    ctid = table.Column<Guid>(name: "ct_id", type: "uuid", nullable: false),
                    busbarid = table.Column<Guid>(name: "busbar_id", type: "uuid", nullable: true),
                    busbarname = table.Column<string>(name: "busbar_name", type: "character varying(50)", maxLength: 50, nullable: true),
                    igmctoolid = table.Column<int>(name: "igmc_tool_id", type: "integer", nullable: true),
                    igmccode = table.Column<string>(name: "igmc_code", type: "text", nullable: true),
                    dispatchingcode = table.Column<string>(name: "dispatching_code", type: "text", nullable: true),
                    areaid = table.Column<Guid>(name: "area_id", type: "uuid", nullable: false),
                    zoneid = table.Column<Guid>(name: "zone_id", type: "uuid", nullable: false),
                    substationid = table.Column<Guid>(name: "substation_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipment_substations_substation_id",
                        column: x => x.substationid,
                        principalTable: "substations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_energys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorddate = table.Column<DateTime>(name: "record_date", type: "timestamp with time zone", nullable: false),
                    importwatt = table.Column<decimal>(name: "import_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    importvar = table.Column<decimal>(name: "import_var", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    exportwatt = table.Column<decimal>(name: "export_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    exportvar = table.Column<decimal>(name: "export_var", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    equipmentid = table.Column<Guid>(name: "equipment_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_energys", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_energys_equipments_equipment_id",
                        column: x => x.equipmentid,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "energy_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorddate = table.Column<DateTimeOffset>(name: "record_date", type: "timestamp with time zone", nullable: false),
                    importwatt = table.Column<decimal>(name: "import_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    importvar = table.Column<decimal>(name: "import_var", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    exportwatt = table.Column<decimal>(name: "export_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    exportvar = table.Column<decimal>(name: "export_var", type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    importtotalwatt = table.Column<decimal>(name: "import_total_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: true),
                    exporttotalwatt = table.Column<decimal>(name: "export_total_watt", type: "numeric(18,0)", precision: 18, scale: 0, nullable: true),
                    equipmentid = table.Column<Guid>(name: "equipment_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_energy_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_energy_profiles_equipments_equipment_id",
                        column: x => x.equipmentid,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment_meter",
                columns: table => new
                {
                    equipmentid = table.Column<Guid>(name: "equipment_id", type: "uuid", nullable: false),
                    serialnumber = table.Column<string>(name: "serial_number", type: "character varying(50)", maxLength: 50, nullable: false),
                    mountdate = table.Column<DateTimeOffset>(name: "mount_date", type: "timestamp with time zone", nullable: false),
                    dismountdate = table.Column<DateTimeOffset>(name: "dismount_date", type: "timestamp with time zone", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_meter", x => new { x.serialnumber, x.equipmentid });
                    table.ForeignKey(
                        name: "fk_equipment_meter_equipments_equipment_id",
                        column: x => x.equipmentid,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment_operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dateofopereation = table.Column<DateTime>(name: "date_of_opereation", type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    equipmentoperationtypeid = table.Column<int>(name: "equipment_operation_type_id", type: "integer", nullable: false),
                    equipmentid = table.Column<Guid>(name: "equipment_id", type: "uuid", nullable: false),
                    substationid = table.Column<Guid>(name: "substation_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_operations", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipment_operations_equipments_equipment_id",
                        column: x => x.equipmentid,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "line_feeders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transmissionlineid = table.Column<Guid>(name: "transmission_line_id", type: "uuid", nullable: false),
                    linefeedertypeid = table.Column<int>(name: "line_feeder_type_id", type: "integer", nullable: false),
                    transfercapacity = table.Column<double>(name: "transfer_capacity", type: "double precision", nullable: true),
                    sourcebusid = table.Column<Guid>(name: "source_bus_id", type: "uuid", nullable: true),
                    destinationbusid = table.Column<Guid>(name: "destination_bus_id", type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_line_feeders", x => x.id);
                    table.ForeignKey(
                        name: "fk_line_feeders_equipment_id",
                        column: x => x.id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "load_feeders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    loadfeedertypeid = table.Column<int>(name: "load_feeder_type_id", type: "integer", nullable: false),
                    maxdemand = table.Column<double>(name: "max_demand", type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_load_feeders", x => x.id);
                    table.ForeignKey(
                        name: "fk_load_feeders_equipment_id",
                        column: x => x.id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_load_feeders_load_feeder_types_load_feeder_type_id",
                        column: x => x.loadfeedertypeid,
                        principalTable: "load_feeder_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meter_reading_failures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorddate = table.Column<DateTime>(name: "record_date", type: "timestamp with time zone", nullable: false),
                    equipmentid = table.Column<Guid>(name: "equipment_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meter_reading_failures", x => x.id);
                    table.ForeignKey(
                        name: "fk_meter_reading_failures_equipments_equipment_id",
                        column: x => x.equipmentid,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "powerplant_feeders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    powerplanttypeid = table.Column<int>(name: "powerplant_type_id", type: "integer", nullable: false),
                    powerplantoperatorid = table.Column<int>(name: "powerplant_operator_id", type: "integer", nullable: false),
                    powerplantscale = table.Column<int>(name: "powerplant_scale", type: "integer", nullable: false),
                    capacity = table.Column<double>(type: "double precision", nullable: true),
                    nominalcapacity = table.Column<double>(name: "nominal_capacity", type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_powerplant_feeders", x => x.id);
                    table.ForeignKey(
                        name: "fk_powerplant_feeders_equipment_id",
                        column: x => x.id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_powerplant_feeders_powerplant_operators_powerplant_operator",
                        column: x => x.powerplantoperatorid,
                        principalTable: "powerplant_operators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_powerplant_feeders_powerplant_types_powerplant_type_id",
                        column: x => x.powerplanttypeid,
                        principalTable: "powerplant_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transformer_feeders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transofmertype = table.Column<int>(name: "transofmer_type", type: "integer", nullable: false),
                    mva = table.Column<double>(type: "double precision", nullable: true),
                    primaryvoltage = table.Column<double>(name: "primary_voltage", type: "double precision", nullable: true),
                    secondaryvoltage = table.Column<double>(name: "secondary_voltage", type: "double precision", nullable: true),
                    sourcebusid = table.Column<Guid>(name: "source_bus_id", type: "uuid", nullable: true),
                    destinationbusid = table.Column<Guid>(name: "destination_bus_id", type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transformer_feeders", x => x.id);
                    table.ForeignKey(
                        name: "fk_transformer_feeders_equipment_id",
                        column: x => x.id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_areas_network_id",
                table: "areas",
                column: "network_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_energys_equipment_id",
                table: "daily_energys",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_energy_profiles_equipment_id",
                table: "energy_profiles",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_energy_tariff_rate_energy_tariff_id",
                table: "energy_tariff_rate",
                column: "energy_tariff_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_substation_id",
                table: "equipment",
                column: "substation_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_meter_equipment_id",
                table: "equipment_meter",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_operations_equipment_id",
                table: "equipment_operations",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_load_feeders_load_feeder_type_id",
                table: "load_feeders",
                column: "load_feeder_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_reading_failures_equipment_id",
                table: "meter_reading_failures",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_powerplant_feeders_powerplant_operator_id",
                table: "powerplant_feeders",
                column: "powerplant_operator_id");

            migrationBuilder.CreateIndex(
                name: "ix_powerplant_feeders_powerplant_type_id",
                table: "powerplant_feeders",
                column: "powerplant_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_substations_zone_id",
                table: "substations",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_zones_area_id",
                table: "zones",
                column: "area_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_energys");

            migrationBuilder.DropTable(
                name: "energy_profiles");

            migrationBuilder.DropTable(
                name: "energy_tariff_rate");

            migrationBuilder.DropTable(
                name: "equipment_meter");

            migrationBuilder.DropTable(
                name: "equipment_operation_types");

            migrationBuilder.DropTable(
                name: "equipment_operations");

            migrationBuilder.DropTable(
                name: "line_feeders");

            migrationBuilder.DropTable(
                name: "load_feeders");

            migrationBuilder.DropTable(
                name: "meter_reading_failures");

            migrationBuilder.DropTable(
                name: "powerplant_feeders");

            migrationBuilder.DropTable(
                name: "transformer_feeders");

            migrationBuilder.DropTable(
                name: "energy_tariffs");

            migrationBuilder.DropTable(
                name: "load_feeder_types");

            migrationBuilder.DropTable(
                name: "powerplant_operators");

            migrationBuilder.DropTable(
                name: "powerplant_types");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "substations");

            migrationBuilder.DropTable(
                name: "zones");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "networks");
        }
    }
}
