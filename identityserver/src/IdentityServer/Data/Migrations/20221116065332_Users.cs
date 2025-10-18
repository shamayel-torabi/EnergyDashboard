using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IdentityServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(name: "normalized_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(name: "first_name", type: "text", nullable: true),
                    lastname = table.Column<string>(name: "last_name", type: "text", nullable: true),
                    picture = table.Column<byte[]>(type: "bytea", nullable: true),
                    username = table.Column<string>(name: "user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(name: "normalized_user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(name: "normalized_email", type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(name: "email_confirmed", type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(name: "password_hash", type: "text", nullable: true),
                    securitystamp = table.Column<string>(name: "security_stamp", type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true),
                    phonenumber = table.Column<string>(name: "phone_number", type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(name: "phone_number_confirmed", type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(name: "two_factor_enabled", type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(name: "lockout_end", type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(name: "lockout_enabled", type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(name: "access_failed_count", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendlyname = table.Column<string>(name: "friendly_name", type: "text", nullable: true),
                    xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                columns: table => new
                {
                    usercode = table.Column<string>(name: "user_code", type: "character varying(200)", maxLength: 200, nullable: false),
                    devicecode = table.Column<string>(name: "device_code", type: "character varying(200)", maxLength: 200, nullable: false),
                    subjectid = table.Column<string>(name: "subject_id", type: "character varying(200)", maxLength: 200, nullable: true),
                    sessionid = table.Column<string>(name: "session_id", type: "character varying(100)", maxLength: 100, nullable: true),
                    clientid = table.Column<string>(name: "client_id", type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    creationtime = table.Column<DateTime>(name: "creation_time", type: "timestamp without time zone", nullable: false),
                    expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_codes", x => x.usercode);
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    use = table.Column<string>(type: "text", nullable: true),
                    algorithm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isx509certificate = table.Column<bool>(name: "is_x509certificate", type: "boolean", nullable: false),
                    dataprotected = table.Column<bool>(name: "data_protected", type: "boolean", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    subjectid = table.Column<string>(name: "subject_id", type: "character varying(200)", maxLength: 200, nullable: true),
                    sessionid = table.Column<string>(name: "session_id", type: "character varying(100)", maxLength: 100, nullable: true),
                    clientid = table.Column<string>(name: "client_id", type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    creationtime = table.Column<DateTime>(name: "creation_time", type: "timestamp without time zone", nullable: false),
                    expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    consumedtime = table.Column<DateTime>(name: "consumed_time", type: "timestamp without time zone", nullable: true),
                    data = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persisted_grants", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(name: "role_id", type: "text", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    providerkey = table.Column<string>(name: "provider_key", type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(name: "provider_display_name", type: "text", nullable: true),
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    roleid = table.Column<string>(name: "role_id", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    userid = table.Column<string>(name: "user_id", type: "text", nullable: false),
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_codes_device_code",
                table: "DeviceCodes",
                column: "device_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_codes_expiration",
                table: "DeviceCodes",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "ix_keys_use",
                table: "Keys",
                column: "use");

            migrationBuilder.CreateIndex(
                name: "ix_persisted_grants_consumed_time",
                table: "PersistedGrants",
                column: "consumed_time");

            migrationBuilder.CreateIndex(
                name: "ix_persisted_grants_expiration",
                table: "PersistedGrants",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "ix_persisted_grants_subject_id_client_id_type",
                table: "PersistedGrants",
                columns: new[] { "subject_id", "client_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_persisted_grants_subject_id_session_id_type",
                table: "PersistedGrants",
                columns: new[] { "subject_id", "session_id", "type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
