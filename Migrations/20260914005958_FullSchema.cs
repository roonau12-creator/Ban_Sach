using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class FullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhaXuatBans",
                columns: table => new
                {
                    MaNXB = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNXB = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DienThoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaXuatBans", x => x.MaNXB);
                });

            migrationBuilder.CreateTable(
                name: "NhomSachs",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNhom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false),
                    ThuTu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomSachs", x => x.MaNhom);
                });

            migrationBuilder.CreateTable(
                name: "TacGias",
                columns: table => new
                {
                    MaTG = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenTacGia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TieuSu = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacGias", x => x.MaTG);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTK = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenDangNhap = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VaiTro = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "boolean", nullable: false),
                    ResetToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ResetTokenHetHan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTK);
                });

            migrationBuilder.CreateTable(
                name: "Sachs",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenSach = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    MaCD = table.Column<int>(type: "integer", nullable: false),
                    MaNhom = table.Column<int>(type: "integer", nullable: false),
                    MaNXB = table.Column<int>(type: "integer", nullable: false),
                    GiaBan = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "integer", nullable: false),
                    AnhBia = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    NamXB = table.Column<int>(type: "integer", nullable: true),
                    SachMoi = table.Column<bool>(type: "boolean", nullable: false),
                    NoiBat = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sachs", x => x.MaSach);
                    table.ForeignKey(
                        name: "FK_Sachs_ChuDes_MaCD",
                        column: x => x.MaCD,
                        principalTable: "ChuDes",
                        principalColumn: "MaCD",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sachs_NhaXuatBans_MaNXB",
                        column: x => x.MaNXB,
                        principalTable: "NhaXuatBans",
                        principalColumn: "MaNXB",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sachs_NhomSachs_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NhomSachs",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaTK = table.Column<int>(type: "integer", nullable: false),
                    HoTen = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.MaKH);
                    table.ForeignKey(
                        name: "FK_KhachHangs_TaiKhoans_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SachTacGias",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "integer", nullable: false),
                    MaTG = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SachTacGias", x => new { x.MaSach, x.MaTG });
                    table.ForeignKey(
                        name: "FK_SachTacGias_Sachs_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Sachs",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SachTacGias_TacGias_MaTG",
                        column: x => x.MaTG,
                        principalTable: "TacGias",
                        principalColumn: "MaTG",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiaChis",
                columns: table => new
                {
                    MaDC = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaKH = table.Column<int>(type: "integer", nullable: false),
                    DiaChiChiTiet = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    MacDinh = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaChis", x => x.MaDC);
                    table.ForeignKey(
                        name: "FK_DiaChis_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoDienThoais",
                columns: table => new
                {
                    MaSDT = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaKH = table.Column<int>(type: "integer", nullable: false),
                    SoDT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MacDinh = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoDienThoais", x => x.MaSDT);
                    table.ForeignKey(
                        name: "FK_SoDienThoais_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    MaDH = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaKH = table.Column<int>(type: "integer", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThai = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TongTien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MaDC = table.Column<int>(type: "integer", nullable: true),
                    MaSDT = table.Column<int>(type: "integer", nullable: true),
                    DiaChiGiao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SoDienThoaiGiao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.MaDH);
                    table.ForeignKey(
                        name: "FK_DonHangs_DiaChis_MaDC",
                        column: x => x.MaDC,
                        principalTable: "DiaChis",
                        principalColumn: "MaDC",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DonHangs_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_SoDienThoais_MaSDT",
                        column: x => x.MaSDT,
                        principalTable: "SoDienThoais",
                        principalColumn: "MaSDT",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHangs",
                columns: table => new
                {
                    MaCT = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaDH = table.Column<int>(type: "integer", nullable: false),
                    MaSach = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHangs", x => x.MaCT);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_DonHangs_MaDH",
                        column: x => x.MaDH,
                        principalTable: "DonHangs",
                        principalColumn: "MaDH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_Sachs_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Sachs",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiaoHangs",
                columns: table => new
                {
                    MaGH = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaDH = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NgayGiao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GhiChu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoHangs", x => x.MaGH);
                    table.ForeignKey(
                        name: "FK_GiaoHangs_DonHangs_MaDH",
                        column: x => x.MaDH,
                        principalTable: "DonHangs",
                        principalColumn: "MaDH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_MaDH",
                table: "ChiTietDonHangs",
                column: "MaDH");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_MaSach",
                table: "ChiTietDonHangs",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "IX_DiaChis_MaKH",
                table: "DiaChis",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaDC",
                table: "DonHangs",
                column: "MaDC");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaKH",
                table: "DonHangs",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaSDT",
                table: "DonHangs",
                column: "MaSDT");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoHangs_MaDH",
                table: "GiaoHangs",
                column: "MaDH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_MaTK",
                table: "KhachHangs",
                column: "MaTK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sachs_MaCD",
                table: "Sachs",
                column: "MaCD");

            migrationBuilder.CreateIndex(
                name: "IX_Sachs_MaNhom",
                table: "Sachs",
                column: "MaNhom");

            migrationBuilder.CreateIndex(
                name: "IX_Sachs_MaNXB",
                table: "Sachs",
                column: "MaNXB");

            migrationBuilder.CreateIndex(
                name: "IX_SachTacGias_MaTG",
                table: "SachTacGias",
                column: "MaTG");

            migrationBuilder.CreateIndex(
                name: "IX_SoDienThoais_MaKH",
                table: "SoDienThoais",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_Email",
                table: "TaiKhoans",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHangs");

            migrationBuilder.DropTable(
                name: "GiaoHangs");

            migrationBuilder.DropTable(
                name: "SachTacGias");

            migrationBuilder.DropTable(
                name: "DonHangs");

            migrationBuilder.DropTable(
                name: "Sachs");

            migrationBuilder.DropTable(
                name: "TacGias");

            migrationBuilder.DropTable(
                name: "DiaChis");

            migrationBuilder.DropTable(
                name: "SoDienThoais");

            migrationBuilder.DropTable(
                name: "NhaXuatBans");

            migrationBuilder.DropTable(
                name: "NhomSachs");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");
        }
    }
}
