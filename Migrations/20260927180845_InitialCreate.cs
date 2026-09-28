using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HTQLNS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChamCongs",
                columns: table => new
                {
                    MaChamCong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThoiGianChekin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianCheckout = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLich = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCongs", x => x.MaChamCong);
                });

            migrationBuilder.CreateTable(
                name: "ChucVus",
                columns: table => new
                {
                    MaChucVu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucVus", x => x.MaChucVu);
                });

            migrationBuilder.CreateTable(
                name: "DonXinNghis",
                columns: table => new
                {
                    MaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonXinNghis", x => x.MaDon);
                });

            migrationBuilder.CreateTable(
                name: "LichPhanCongs",
                columns: table => new
                {
                    MaLich = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayLam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichPhanCongs", x => x.MaLich);
                });

            migrationBuilder.CreateTable(
                name: "LoaiCaLams",
                columns: table => new
                {
                    MaLoaiCa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenCa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiCaLams", x => x.MaLoaiCa);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaPhongBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChucVu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyThaoTacs",
                columns: table => new
                {
                    MaLog = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyThaoTacs", x => x.MaLog);
                });

            migrationBuilder.CreateTable(
                name: "PhongBans",
                columns: table => new
                {
                    MaPhongBan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenPhongBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBans", x => x.MaPhongBan);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    MaVaiTro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenVaiTro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauDoiCas",
                columns: table => new
                {
                    MaYeuCau = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLichCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLichMoi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauDoiCas", x => x.MaYeuCau);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCongs");

            migrationBuilder.DropTable(
                name: "ChucVus");

            migrationBuilder.DropTable(
                name: "DonXinNghis");

            migrationBuilder.DropTable(
                name: "LichPhanCongs");

            migrationBuilder.DropTable(
                name: "LoaiCaLams");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "NhatKyThaoTacs");

            migrationBuilder.DropTable(
                name: "PhongBans");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "VaiTros");

            migrationBuilder.DropTable(
                name: "YeuCauDoiCas");
        }
    }
}
