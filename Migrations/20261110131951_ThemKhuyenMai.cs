using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class ThemKhuyenMai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaKM",
                table: "DonHangs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaKhuyenMai",
                table: "DonHangs",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienGiam",
                table: "DonHangs",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "KhuyenMais",
                columns: table => new
                {
                    MaKM = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    TenKhuyenMai = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LoaiGiamGia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GiamToiDa = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DonHangToiThieu = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DangHoatDong = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMais", x => x.MaKM);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaKM",
                table: "DonHangs",
                column: "MaKM");

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMais_MaCode",
                table: "KhuyenMais",
                column: "MaCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_KhuyenMais_MaKM",
                table: "DonHangs",
                column: "MaKM",
                principalTable: "KhuyenMais",
                principalColumn: "MaKM",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_KhuyenMais_MaKM",
                table: "DonHangs");

            migrationBuilder.DropTable(
                name: "KhuyenMais");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_MaKM",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "MaKM",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "MaKhuyenMai",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "SoTienGiam",
                table: "DonHangs");
        }
    }
}
