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
                name: "NhanViens",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaPhongBan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaChucVu = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanViens_ChucVus_MaChucVu",
                        column: x => x.MaChucVu,
                        principalTable: "ChucVus",
                        principalColumn: "MaChucVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanViens_PhongBans_MaPhongBan",
                        column: x => x.MaPhongBan,
                        principalTable: "PhongBans",
                        principalColumn: "MaPhongBan",
                        onDelete: ReferentialAction.Cascade);
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
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonXinNghis", x => x.MaDon);
                    table.ForeignKey(
                        name: "FK_DonXinNghis_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichPhanCongs",
                columns: table => new
                {
                    MaLich = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayLam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichPhanCongs", x => x.MaLich);
                    table.ForeignKey(
                        name: "FK_LichPhanCongs_LoaiCaLams_MaLoaiCa",
                        column: x => x.MaLoaiCa,
                        principalTable: "LoaiCaLams",
                        principalColumn: "MaLoaiCa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichPhanCongs_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaVaiTro = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_TaiKhoans_VaiTros_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTros",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "ChamCongs",
                columns: table => new
                {
                    MaChamCong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThoiGianChekin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianCheckout = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLich = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCongs", x => x.MaChamCong);
                    table.ForeignKey(
                        name: "FK_ChamCongs_LichPhanCongs_MaLich",
                        column: x => x.MaLich,
                        principalTable: "LichPhanCongs",
                        principalColumn: "MaLich",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauDoiCas",
                columns: table => new
                {
                    MaYeuCau = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaLichCu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaLichMoi = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauDoiCas", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiCas_LichPhanCongs_MaLichCu",
                        column: x => x.MaLichCu,
                        principalTable: "LichPhanCongs",
                        principalColumn: "MaLich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiCas_LichPhanCongs_MaLichMoi",
                        column: x => x.MaLichMoi,
                        principalTable: "LichPhanCongs",
                        principalColumn: "MaLich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiCas_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyThaoTacs",
                columns: table => new
                {
                    MaLog = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyThaoTacs", x => x.MaLog);
                    table.ForeignKey(
                        name: "FK_NhatKyThaoTacs_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_MaLich",
                table: "ChamCongs",
                column: "MaLich");

            migrationBuilder.CreateIndex(
                name: "IX_DonXinNghis_MaNhanVien",
                table: "DonXinNghis",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhanCongs_MaLoaiCa",
                table: "LichPhanCongs",
                column: "MaLoaiCa");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhanCongs_MaNhanVien",
                table: "LichPhanCongs",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_MaChucVu",
                table: "NhanViens",
                column: "MaChucVu");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_MaPhongBan",
                table: "NhanViens",
                column: "MaPhongBan");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyThaoTacs_MaTaiKhoan",
                table: "NhatKyThaoTacs",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_MaNhanVien",
                table: "TaiKhoans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_MaVaiTro",
                table: "TaiKhoans",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiCas_MaLichCu",
                table: "YeuCauDoiCas",
                column: "MaLichCu");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiCas_MaLichMoi",
                table: "YeuCauDoiCas",
                column: "MaLichMoi");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiCas_MaNhanVien",
                table: "YeuCauDoiCas",
                column: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCongs");

            migrationBuilder.DropTable(
                name: "DonXinNghis");

            migrationBuilder.DropTable(
                name: "NhatKyThaoTacs");

            migrationBuilder.DropTable(
                name: "YeuCauDoiCas");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "LichPhanCongs");

            migrationBuilder.DropTable(
                name: "VaiTros");

            migrationBuilder.DropTable(
                name: "LoaiCaLams");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "ChucVus");

            migrationBuilder.DropTable(
                name: "PhongBans");
        }
    }
}
