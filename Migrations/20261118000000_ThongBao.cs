using BanSach.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261118000000_ThongBao")]
    public partial class ThongBao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaTB = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaTK = table.Column<int>(type: "integer", nullable: false),
                    MaDH = table.Column<int>(type: "integer", nullable: true),
                    MaSuKien = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TieuDe = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NoiDung = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaDoc = table.Column<bool>(type: "boolean", nullable: false),
                    An = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaTB);
                    table.ForeignKey(
                        name: "FK_ThongBaos_DonHangs_MaDH",
                        column: x => x.MaDH,
                        principalTable: "DonHangs",
                        principalColumn: "MaDH",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ThongBaos_TaiKhoans_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaDH",
                table: "ThongBaos",
                column: "MaDH");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaTK_An_DaDoc",
                table: "ThongBaos",
                columns: new[] { "MaTK", "An", "DaDoc" });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaTK_MaSuKien",
                table: "ThongBaos",
                columns: new[] { "MaTK", "MaSuKien" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ThongBaos");
        }
    }
}
