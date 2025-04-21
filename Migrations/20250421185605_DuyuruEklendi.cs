using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskidaKitap.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class DuyuruEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kitaplar_KitapKategoriler_KitapKategoriId",
                table: "Kitaplar");

            migrationBuilder.CreateTable(
                name: "Duyurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Icerik = table.Column<string>(type: "TEXT", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duyurular", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitapDegisim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KitapId = table.Column<int>(type: "INTEGER", nullable: false),
                    OgrenciId = table.Column<string>(type: "TEXT", nullable: false),
                    AlisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitapDegisim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitapDegisim_AspNetUsers_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KitapDegisim_Kitaplar_KitapId",
                        column: x => x.KitapId,
                        principalTable: "Kitaplar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KitapDegisim_KitapId",
                table: "KitapDegisim",
                column: "KitapId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapDegisim_OgrenciId",
                table: "KitapDegisim",
                column: "OgrenciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kitaplar_KitapKategoriler_KitapKategoriId",
                table: "Kitaplar",
                column: "KitapKategoriId",
                principalTable: "KitapKategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kitaplar_KitapKategoriler_KitapKategoriId",
                table: "Kitaplar");

            migrationBuilder.DropTable(
                name: "Duyurular");

            migrationBuilder.DropTable(
                name: "KitapDegisim");

            migrationBuilder.AddForeignKey(
                name: "FK_Kitaplar_KitapKategoriler_KitapKategoriId",
                table: "Kitaplar",
                column: "KitapKategoriId",
                principalTable: "KitapKategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
