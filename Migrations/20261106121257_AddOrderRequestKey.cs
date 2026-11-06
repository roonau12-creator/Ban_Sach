using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanSach.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderRequestKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaYeuCau",
                table: "DonHangs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaYeuCau",
                table: "DonHangs",
                column: "MaYeuCau",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DonHangs_MaYeuCau",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "MaYeuCau",
                table: "DonHangs");
        }
    }
}
