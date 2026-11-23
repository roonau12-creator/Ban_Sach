using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrangThaiVersion",
                table: "DonHangs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmailOutbox",
                columns: table => new
                {
                    MaEmail = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Recipient = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Subject = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessingAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutbox", x => x.MaEmail);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutbox_EventKey",
                table: "EmailOutbox",
                column: "EventKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutbox_Status_NextAttemptAt",
                table: "EmailOutbox",
                columns: new[] { "Status", "NextAttemptAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOutbox");

            migrationBuilder.DropColumn(
                name: "TrangThaiVersion",
                table: "DonHangs");
        }
    }
}
