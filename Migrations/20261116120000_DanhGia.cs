using BanSach.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261116120000_DanhGia")]
    public partial class DanhGia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhGias",
                columns: table => new
                {
                    MaDG = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaTK = table.Column<int>(type: "integer", nullable: false),
                    MaSach = table.Column<int>(type: "integer", nullable: false),
                    SoSao = table.Column<int>(type: "integer", nullable: false),
                    NoiDung = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    An = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGias", x => x.MaDG);
                    table.CheckConstraint("CK_DanhGias_SoSao", "\"SoSao\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_DanhGias_Sachs_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Sachs",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGias_TaiKhoans_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_MaSach_An",
                table: "DanhGias",
                columns: new[] { "MaSach", "An" });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_MaTK_MaSach",
                table: "DanhGias",
                columns: new[] { "MaTK", "MaSach" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DanhGias");
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            ApplicationDbContextModelSnapshot.BuildTargetModel(modelBuilder);
        }
    }
}
