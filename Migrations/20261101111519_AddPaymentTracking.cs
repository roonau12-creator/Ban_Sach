using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    MaTT = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaDH = table.Column<int>(type: "integer", nullable: false),
                    SoTien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PhuongThuc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TrangThai = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MaGiaoDich = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.MaTT);
                    table.ForeignKey(
                        name: "FK_ThanhToans_DonHangs_MaDH",
                        column: x => x.MaDH,
                        principalTable: "DonHangs",
                        principalColumn: "MaDH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaDH",
                table: "ThanhToans",
                column: "MaDH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaGiaoDich",
                table: "ThanhToans",
                column: "MaGiaoDich",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO "ThanhToans" ("MaDH", "SoTien", "PhuongThuc", "TrangThai", "NgayTao", "NgayCapNhat", "NgayThanhToan")
                SELECT "MaDH", "TongTien", "PhuongThucThanhToan", "TrangThaiThanhToan", "NgayDat", "NgayDat",
                    CASE WHEN "TrangThaiThanhToan" = 'DaThanhToan' THEN "NgayDat" ELSE NULL END
                FROM "DonHangs";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThanhToans");
        }
    }
}
