using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Data.Migrations;

/// <inheritdoc />
public partial class InitialAppDb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "text", nullable: false),
                date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                message_type = table.Column<byte>(type: "smallint", nullable: false),
                message_scope = table.Column<byte>(type: "smallint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_messages", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_messages_date",
            table: "messages",
            column: "date");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "messages");
    }
}
