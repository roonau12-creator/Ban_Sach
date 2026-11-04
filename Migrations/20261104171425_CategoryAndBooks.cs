using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class CategoryAndBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "ChuDes",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenChuDeNormalized",
                table: "ChuDes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sachs",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenSach = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MaCD = table.Column<int>(type: "integer", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChuDes_TenChuDeNormalized",
                table: "ChuDes",
                column: "TenChuDeNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sachs_MaCD",
                table: "Sachs",
                column: "MaCD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sachs");

            migrationBuilder.DropIndex(
                name: "IX_ChuDes_TenChuDeNormalized",
                table: "ChuDes");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "ChuDes");

            migrationBuilder.DropColumn(
                name: "TenChuDeNormalized",
                table: "ChuDes");
        }
    }
}
