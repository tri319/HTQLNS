-- =============================================================================
-- SCRIPT CƠ SỞ DỮ LIỆU: HỆ THỐNG QUẢN LÝ NHÂN SỰ STARZONE HRM (StarZoneHRM)
-- Hệ quản trị CSDL: Microsoft SQL Server
-- Tài khoản mặc định:
--   1. Admin:     admin / 123456
--   2. Nhân viên: nhanvien / 123456
-- =============================================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StarZoneHRM')
BEGIN
    CREATE DATABASE [StarZoneHRM];
END
GO

USE [StarZoneHRM];
GO

-- =============================================================================
-- 1. XÓA BẢNG CŨ NẾU ĐÃ TỒN TẠI (ĐỂ CHẠY LẠI SCRIPT SẠCH SẼ)
-- =============================================================================
IF OBJECT_ID(N'dbo.NhatKyThaoTacs', N'U') IS NOT NULL DROP TABLE dbo.NhatKyThaoTacs;
IF OBJECT_ID(N'dbo.YeuCauDoiCas', N'U') IS NOT NULL DROP TABLE dbo.YeuCauDoiCas;
IF OBJECT_ID(N'dbo.ChamCongs', N'U') IS NOT NULL DROP TABLE dbo.ChamCongs;
IF OBJECT_ID(N'dbo.DonXinNghis', N'U') IS NOT NULL DROP TABLE dbo.DonXinNghis;
IF OBJECT_ID(N'dbo.LichPhanCongs', N'U') IS NOT NULL DROP TABLE dbo.LichPhanCongs;
IF OBJECT_ID(N'dbo.TaiKhoans', N'U') IS NOT NULL DROP TABLE dbo.TaiKhoans;
IF OBJECT_ID(N'dbo.NhanViens', N'U') IS NOT NULL DROP TABLE dbo.NhanViens;
IF OBJECT_ID(N'dbo.LoaiCaLams', N'U') IS NOT NULL DROP TABLE dbo.LoaiCaLams;
IF OBJECT_ID(N'dbo.VaiTros', N'U') IS NOT NULL DROP TABLE dbo.VaiTros;
IF OBJECT_ID(N'dbo.ChucVus', N'U') IS NOT NULL DROP TABLE dbo.ChucVus;
IF OBJECT_ID(N'dbo.PhongBans', N'U') IS NOT NULL DROP TABLE dbo.PhongBans;
GO

-- =============================================================================
-- 2. TẠO CẤU TRÚC CÁC BẢNG (TABLES & CONSTRAINTS)
-- =============================================================================

-- Bảng Phòng Ban
CREATE TABLE [dbo].[PhongBans] (
    [MaPhongBan]  NVARCHAR(450) NOT NULL,
    [TenPhongBan] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_PhongBans] PRIMARY KEY ([MaPhongBan])
);

-- Bảng Chức Vụ
CREATE TABLE [dbo].[ChucVus] (
    [MaChucVu]  NVARCHAR(450) NOT NULL,
    [TenChucVu] NVARCHAR(100) NOT NULL,
    [MoTa]      NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_ChucVus] PRIMARY KEY ([MaChucVu])
);

-- Bảng Vai Trò (Phân quyền)
CREATE TABLE [dbo].[VaiTros] (
    [MaVaiTro]  NVARCHAR(450) NOT NULL,
    [TenVaiTro] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_VaiTros] PRIMARY KEY ([MaVaiTro])
);

-- Bảng Loại Ca Làm
CREATE TABLE [dbo].[LoaiCaLams] (
    [MaLoaiCa]   NVARCHAR(450) NOT NULL,
    [TenCa]      NVARCHAR(100) NOT NULL,
    [GioBatDau]  TIME          NOT NULL,
    [GioKetThuc] TIME          NOT NULL,
    CONSTRAINT [PK_LoaiCaLams] PRIMARY KEY ([MaLoaiCa])
);

-- Bảng Nhân Viên
CREATE TABLE [dbo].[NhanViens] (
    [MaNhanVien] NVARCHAR(450) NOT NULL,
    [HoTen]      NVARCHAR(100) NOT NULL,
    [NgaySinh]   DATETIME2     NULL,
    [SDT]        NVARCHAR(20)  NULL,
    [Email]      NVARCHAR(100) NULL,
    [MaPhongBan] NVARCHAR(450) NOT NULL,
    [MaChucVu]   NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_NhanViens] PRIMARY KEY ([MaNhanVien]),
    CONSTRAINT [FK_NhanViens_PhongBans_MaPhongBan] FOREIGN KEY ([MaPhongBan]) REFERENCES [dbo].[PhongBans] ([MaPhongBan]) ON DELETE CASCADE,
    CONSTRAINT [FK_NhanViens_ChucVus_MaChucVu]     FOREIGN KEY ([MaChucVu])   REFERENCES [dbo].[ChucVus] ([MaChucVu])     ON DELETE CASCADE
);

-- Bảng Tài Khoản
CREATE TABLE [dbo].[TaiKhoans] (
    [MaTaiKhoan]  NVARCHAR(450) NOT NULL,
    [TenDangNhap] NVARCHAR(50)  NOT NULL,
    [MatKhau]     NVARCHAR(255) NULL,
    [TrangThai]   NVARCHAR(MAX) NULL,
    [HoTen]       NVARCHAR(100) NULL,
    [NgaySinh]    DATETIME2     NULL,
    [MaNhanVien]  NVARCHAR(450) NULL,
    [MaVaiTro]    NVARCHAR(450) NULL,
    CONSTRAINT [PK_TaiKhoans] PRIMARY KEY ([MaTaiKhoan]),
    CONSTRAINT [FK_TaiKhoans_NhanViens_MaNhanVien] FOREIGN KEY ([MaNhanVien]) REFERENCES [dbo].[NhanViens] ([MaNhanVien]),
    CONSTRAINT [FK_TaiKhoans_VaiTros_MaVaiTro]     FOREIGN KEY ([MaVaiTro])   REFERENCES [dbo].[VaiTros] ([MaVaiTro])
);

-- Bảng Lịch Phân Công
CREATE TABLE [dbo].[LichPhanCongs] (
    [MaLich]     NVARCHAR(450) NOT NULL,
    [NgayLam]    DATETIME2     NOT NULL,
    [TrangThai]  NVARCHAR(50)  NULL,
    [MaNhanVien] NVARCHAR(450) NOT NULL,
    [MaLoaiCa]   NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_LichPhanCongs] PRIMARY KEY ([MaLich]),
    CONSTRAINT [FK_LichPhanCongs_NhanViens_MaNhanVien] FOREIGN KEY ([MaNhanVien]) REFERENCES [dbo].[NhanViens] ([MaNhanVien]) ON DELETE CASCADE,
    CONSTRAINT [FK_LichPhanCongs_LoaiCaLams_MaLoaiCa]  FOREIGN KEY ([MaLoaiCa])   REFERENCES [dbo].[LoaiCaLams] ([MaLoaiCa])  ON DELETE CASCADE
);

-- Bảng Chấm Công
CREATE TABLE [dbo].[ChamCongs] (
    [MaChamCong]       NVARCHAR(450) NOT NULL,
    [ThoiGianChekin]   DATETIME2     NULL,
    [ThoiGianCheckout] DATETIME2     NULL,
    [TrangThai]        NVARCHAR(50)  NULL,
    [MaLich]           NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_ChamCongs] PRIMARY KEY ([MaChamCong]),
    CONSTRAINT [FK_ChamCongs_LichPhanCongs_MaLich] FOREIGN KEY ([MaLich]) REFERENCES [dbo].[LichPhanCongs] ([MaLich]) ON DELETE CASCADE
);

-- Bảng Đơn Xin Nghỉ
CREATE TABLE [dbo].[DonXinNghis] (
    [MaDon]      NVARCHAR(450) NOT NULL,
    [TuNgay]     DATETIME2     NOT NULL,
    [DenNgay]    DATETIME2     NOT NULL,
    [LyDo]       NVARCHAR(MAX) NULL,
    [TrangThai]  NVARCHAR(50)  NULL,
    [MaNhanVien] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_DonXinNghis] PRIMARY KEY ([MaDon]),
    CONSTRAINT [FK_DonXinNghis_NhanViens_MaNhanVien] FOREIGN KEY ([MaNhanVien]) REFERENCES [dbo].[NhanViens] ([MaNhanVien]) ON DELETE CASCADE
);

-- Bảng Yêu Cầu Đổi Ca
CREATE TABLE [dbo].[YeuCauDoiCas] (
    [MaYeuCau]   NVARCHAR(450) NOT NULL,
    [NgayGui]    DATETIME2     NOT NULL,
    [LyDo]       NVARCHAR(MAX) NULL,
    [TrangThai]  NVARCHAR(50)  NULL,
    [MaNhanVien] NVARCHAR(450) NOT NULL,
    [MaLichCu]   NVARCHAR(450) NOT NULL,
    [MaLichMoi]  NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_YeuCauDoiCas] PRIMARY KEY ([MaYeuCau]),
    CONSTRAINT [FK_YeuCauDoiCas_NhanViens_MaNhanVien]     FOREIGN KEY ([MaNhanVien]) REFERENCES [dbo].[NhanViens] ([MaNhanVien])     ON DELETE CASCADE,
    CONSTRAINT [FK_YeuCauDoiCas_LichPhanCongs_MaLichCu]   FOREIGN KEY ([MaLichCu])   REFERENCES [dbo].[LichPhanCongs] ([MaLich])     ON DELETE NO ACTION,
    CONSTRAINT [FK_YeuCauDoiCas_LichPhanCongs_MaLichMoi]  FOREIGN KEY ([MaLichMoi])  REFERENCES [dbo].[LichPhanCongs] ([MaLich])     ON DELETE NO ACTION
);

-- Bảng Nhật Ký Thao Tác
CREATE TABLE [dbo].[NhatKyThaoTacs] (
    [MaLog]      NVARCHAR(450) NOT NULL,
    [ThoiGian]   DATETIME2     NOT NULL,
    [HanhDong]   NVARCHAR(100) NULL,
    [NoiDung]    NVARCHAR(MAX) NULL,
    [MaTaiKhoan] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_NhatKyThaoTacs] PRIMARY KEY ([MaLog]),
    CONSTRAINT [FK_NhatKyThaoTacs_TaiKhoans_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [dbo].[TaiKhoans] ([MaTaiKhoan]) ON DELETE CASCADE
);
GO

-- Tạo Indexes
CREATE INDEX [IX_NhanViens_MaPhongBan]    ON [dbo].[NhanViens] ([MaPhongBan]);
CREATE INDEX [IX_NhanViens_MaChucVu]      ON [dbo].[NhanViens] ([MaChucVu]);
CREATE INDEX [IX_TaiKhoans_MaNhanVien]    ON [dbo].[TaiKhoans] ([MaNhanVien]);
CREATE INDEX [IX_TaiKhoans_MaVaiTro]      ON [dbo].[TaiKhoans] ([MaVaiTro]);
CREATE INDEX [IX_LichPhanCongs_MaNhanVien] ON [dbo].[LichPhanCongs] ([MaNhanVien]);
CREATE INDEX [IX_LichPhanCongs_MaLoaiCa]  ON [dbo].[LichPhanCongs] ([MaLoaiCa]);
CREATE INDEX [IX_ChamCongs_MaLich]        ON [dbo].[ChamCongs] ([MaLich]);
CREATE INDEX [IX_DonXinNghis_MaNhanVien]  ON [dbo].[DonXinNghis] ([MaNhanVien]);
CREATE INDEX [IX_YeuCauDoiCas_MaNhanVien] ON [dbo].[YeuCauDoiCas] ([MaNhanVien]);
CREATE INDEX [IX_YeuCauDoiCas_MaLichCu]   ON [dbo].[YeuCauDoiCas] ([MaLichCu]);
CREATE INDEX [IX_YeuCauDoiCas_MaLichMoi]  ON [dbo].[YeuCauDoiCas] ([MaLichMoi]);
CREATE INDEX [IX_NhatKyThaoTacs_MaTaiKhoan] ON [dbo].[NhatKyThaoTacs] ([MaTaiKhoan]);
GO

-- =============================================================================
-- 3. THÊM DỮ LIỆU MẪU (SEED DATA)
-- =============================================================================

-- 3.1. Phòng Ban
INSERT INTO [dbo].[PhongBans] ([MaPhongBan], [TenPhongBan]) VALUES
(N'PB001', N'Bar & Lounge'),
(N'PB002', N'Sân khấu & Biểu diễn'),
(N'PB003', N'Lễ tân & CSKH'),
(N'PB004', N'Âm thanh ánh sáng'),
(N'PB005', N'An ninh & Nội vụ');

-- 3.2. Chức Vụ
INSERT INTO [dbo].[ChucVus] ([MaChucVu], [TenChucVu], [MoTa]) VALUES
(N'CV001', N'Pha chế (Bartender)',   N'Pha chế đồ uống tại quầy Bar'),
(N'CV002', N'Lễ tân viên',           N'Đón tiếp khách và đặt bàn'),
(N'CV003', N'Quản lý',               N'Quản lý điều hành bộ phận'),
(N'CV004', N'Kỹ thuật viên Âm thanh', N'Vận hành hệ thống âm thanh, ánh sáng'),
(N'CV005', N'DJ & Biểu diễn',        N'Biểu diễn âm nhạc sân khấu'),
(N'CV006', N'Phục vụ bàn',           N'Phục vụ khách khu vực Lounge'),
(N'CV007', N'Giám sát An ninh',      N'Đảm bảo trật tự và an toàn');

-- 3.3. Vai Trò
INSERT INTO [dbo].[VaiTros] ([MaVaiTro], [TenVaiTro]) VALUES
(N'VT001', N'Admin'),
(N'VT002', N'Nhân viên');

-- 3.4. Nhân Viên
INSERT INTO [dbo].[NhanViens] ([MaNhanVien], [HoTen], [NgaySinh], [SDT], [Email], [MaPhongBan], [MaChucVu]) VALUES
(N'NV001', N'Trần Minh Quân (Admin)', '1995-05-15', N'0901234567', N'trin99832@gmail.com',   N'PB001', N'CV003'),
(N'NV002', N'Nguyễn Văn An',          '2001-08-20', N'0987654321', N'nhanvien@starzone.vn',  N'PB001', N'CV001'),
(N'NV003', N'Lê Thị Bảo Ngọc',        '2000-03-12', N'0912345678', N'baongoc@starzone.vn',   N'PB003', N'CV003'),
(N'NV004', N'Phạm Hoàng Nam',         '1999-11-05', N'0934567890', N'hoangnam@starzone.vn',  N'PB002', N'CV005'),
(N'NV005', N'Võ Thị Thanh Tâm',       '2002-07-25', N'0978123456', N'thanhtam@starzone.vn',  N'PB003', N'CV002'),
(N'NV006', N'Đặng Quốc Khánh',        '1997-01-30', N'0965432109', N'quockhanh@starzone.vn', N'PB004', N'CV003'),
(N'NV007', N'Bùi Tuấn Kiệt',          '2001-04-18', N'0943216789', N'tuankiet@starzone.vn',  N'PB004', N'CV004'),
(N'NV008', N'Hoàng Mai Phương',       '2003-09-10', N'0923456781', N'maiphuong@starzone.vn', N'PB001', N'CV006'),
(N'NV009', N'Ngô Gia Huy',            '1998-12-02', N'0956781234', N'giahuy@starzone.vn',    N'PB002', N'CV003'),
(N'NV010', N'Đỗ Thùy Linh',           '2002-02-14', N'0918273645', N'thuylinh@starzone.vn',  N'PB003', N'CV002'),
(N'NV011', N'Trương Văn Hùng',        '1994-06-08', N'0909876543', N'vanhung@starzone.vn',   N'PB005', N'CV003'),
(N'NV012', N'Phan Thanh Tùng',        '2000-10-22', N'0938475610', N'thanhtung@starzone.vn', N'PB001', N'CV001');

-- 3.5. Tài Khoản (Bao gồm tài khoản Admin: admin/123456 và Nhân viên: nhanvien/123456)
INSERT INTO [dbo].[TaiKhoans] ([MaTaiKhoan], [TenDangNhap], [MatKhau], [TrangThai], [HoTen], [NgaySinh], [MaNhanVien], [MaVaiTro]) VALUES
(N'TK001',    N'trin99832@gmail.com',    N'123456', N'Hoạt động', N'Trần Minh Quân (Admin)',    '1995-05-15', N'NV001', N'VT001'),
(N'TK_ADMIN', N'admin',                  N'123456', N'Hoạt động', N'Quản trị viên (Admin)',     '1995-05-15', N'NV001', N'VT001'),
(N'TK_STAFF', N'nhanvien',               N'123456', N'Hoạt động', N'Nguyễn Văn An (Nhân viên)', '2001-08-20', N'NV002', N'VT002'),
(N'TK003',    N'baongoc@starzone.vn',    N'123456', N'Hoạt động', N'Lê Thị Bảo Ngọc',           '2000-03-12', N'NV003', N'VT002'),
(N'TK004',    N'hoangnam@starzone.vn',   N'123456', N'Hoạt động', N'Phạm Hoàng Nam',            '1999-11-05', N'NV004', N'VT002'),
(N'TK005',    N'thanhtam@starzone.vn',   N'123456', N'Hoạt động', N'Võ Thị Thanh Tâm',          '2002-07-25', N'NV005', N'VT002'),
(N'TK006',    N'quockhanh@starzone.vn',  N'123456', N'Bị khóa',   N'Đặng Quốc Khánh',           '1997-01-30', N'NV006', N'VT002'),
(N'TK_PEND1', N'minhkhoi.work@gmail.com', N'123456', N'Chờ duyệt', N'Lý Minh Khôi',             '2003-04-11', NULL,     N'VT002'),
(N'TK_PEND2', N'khanhvy.le@gmail.com',    N'123456', N'Chờ duyệt', N'Lê Khánh Vy',              '2004-09-28', NULL,     N'VT002');

-- 3.6. Loại Ca Làm
INSERT INTO [dbo].[LoaiCaLams] ([MaLoaiCa], [TenCa], [GioBatDau], [GioKetThuc]) VALUES
(N'CA001', N'Ca Sáng',  '08:00:00', '14:00:00'),
(N'CA002', N'Ca Chiều', '14:00:00', '20:00:00'),
(N'CA003', N'Ca Tối',   '18:00:00', '23:59:00');

-- 3.7. Lịch Phân Công (Tự động tính theo tuần hiện tại khi chạy script)
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
DECLARE @Monday DATE = DATEADD(DAY, -((DATEPART(WEEKDAY, @Today) + @@DATEFIRST - 2) % 7), @Today);

INSERT INTO [dbo].[LichPhanCongs] ([MaLich], [MaNhanVien], [MaLoaiCa], [NgayLam], [TrangThai]) VALUES
-- Thứ 2
(N'LC_W01', N'NV012', N'CA001', @Monday, N'Hoàn thành'),
(N'LC_W02', N'NV005', N'CA001', @Monday, N'Hoàn thành'),
(N'LC_W03', N'NV003', N'CA002', @Monday, N'Hoàn thành'),
(N'LC_W04', N'NV008', N'CA002', @Monday, N'Hoàn thành'),
(N'LC_W05', N'NV004', N'CA003', @Monday, N'Hoàn thành'),
(N'LC_W06', N'NV007', N'CA003', @Monday, N'Hoàn thành'),
(N'LC_W07', N'NV009', N'CA003', @Monday, N'Hoàn thành'),
-- Thứ 3
(N'LC_W08', N'NV005', N'CA001', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
(N'LC_W09', N'NV010', N'CA001', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
(N'LC_W10', N'NV008', N'CA002', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
(N'LC_W11', N'NV006', N'CA002', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
(N'LC_W12', N'NV004', N'CA003', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
(N'LC_W13', N'NV009', N'CA003', DATEADD(DAY, 1, @Monday), N'Hoàn thành'),
-- Thứ 4
(N'LC_W14', N'NV012', N'CA001', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
(N'LC_W15', N'NV003', N'CA001', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
(N'LC_W16', N'NV008', N'CA002', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
(N'LC_W17', N'NV010', N'CA002', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
(N'LC_W18', N'NV007', N'CA003', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
(N'LC_W19', N'NV011', N'CA003', DATEADD(DAY, 2, @Monday), N'Hoàn thành'),
-- Thứ 5
(N'LC_W20', N'NV005', N'CA001', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
(N'LC_W21', N'NV012', N'CA001', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
(N'LC_W22', N'NV006', N'CA002', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
(N'LC_W23', N'NV003', N'CA002', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
(N'LC_W24', N'NV004', N'CA003', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
(N'LC_W25', N'NV009', N'CA003', DATEADD(DAY, 3, @Monday), N'Hoàn thành'),
-- Thứ 6
(N'LC_W26', N'NV003', N'CA001', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W27', N'NV010', N'CA001', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W28', N'NV005', N'CA002', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W29', N'NV008', N'CA002', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W30', N'NV004', N'CA003', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W31', N'NV007', N'CA003', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_W32', N'NV012', N'CA003', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
-- Thứ 7
(N'LC_W33', N'NV003', N'CA001', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W34', N'NV005', N'CA002', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W35', N'NV008', N'CA002', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W36', N'NV012', N'CA003', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W37', N'NV004', N'CA003', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W38', N'NV009', N'CA003', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_W39', N'NV011', N'CA003', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
-- Chủ Nhật
(N'LC_W40', N'NV010', N'CA001', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
(N'LC_W41', N'NV012', N'CA002', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
(N'LC_W42', N'NV004', N'CA003', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
(N'LC_W43', N'NV007', N'CA003', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
(N'LC_W44', N'NV008', N'CA003', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
-- Lịch của NV002 (Nguyễn Văn An - tài khoản nhanvien) cả tuần và quá khứ
(N'LC_NV2_D0', N'NV002', N'CA001', @Monday,                  N'Sắp tới'),
(N'LC_NV2_D1', N'NV002', N'CA002', DATEADD(DAY, 1, @Monday), N'Sắp tới'),
(N'LC_NV2_D2', N'NV002', N'CA001', DATEADD(DAY, 2, @Monday), N'Sắp tới'),
(N'LC_NV2_D3', N'NV002', N'CA002', DATEADD(DAY, 3, @Monday), N'Sắp tới'),
(N'LC_NV2_D4', N'NV002', N'CA001', DATEADD(DAY, 4, @Monday), N'Sắp tới'),
(N'LC_NV2_D5', N'NV002', N'CA003', DATEADD(DAY, 5, @Monday), N'Sắp tới'),
(N'LC_NV2_D6', N'NV002', N'CA002', DATEADD(DAY, 6, @Monday), N'Sắp tới'),
(N'LC_NV2_P1', N'NV002', N'CA001', DATEADD(DAY, -3, @Today), N'Hoàn thành'),
(N'LC_NV2_P2', N'NV002', N'CA002', DATEADD(DAY, -2, @Today), N'Hoàn thành'),
(N'LC_NV2_P3', N'NV002', N'CA001', DATEADD(DAY, -1, @Today), N'Hoàn thành');

-- 3.8. Chấm Công
INSERT INTO [dbo].[ChamCongs] ([MaChamCong], [MaLich], [ThoiGianChekin], [ThoiGianCheckout], [TrangThai]) VALUES
(N'CC001',    N'LC_W01',    DATEADD(MINUTE, 475, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 845, CAST(@Today AS DATETIME2)),  N'Đúng giờ'),
(N'CC002',    N'LC_W02',    DATEADD(MINUTE, 478, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 842, CAST(@Today AS DATETIME2)),  N'Đúng giờ'),
(N'CC003',    N'LC_W03',    DATEADD(MINUTE, 830, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 1210, CAST(@Today AS DATETIME2)), N'Đúng giờ'),
(N'CC004',    N'LC_W04',    DATEADD(MINUTE, 858, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 1205, CAST(@Today AS DATETIME2)), N'Trễ giờ'),
(N'CC005',    N'LC_W05',    DATEADD(MINUTE, 1075, CAST(@Today AS DATETIME2)), DATEADD(MINUTE, 1438, CAST(@Today AS DATETIME2)), N'Đúng giờ'),
(N'CC006',    N'LC_W06',    DATEADD(MINUTE, 1070, CAST(@Today AS DATETIME2)), DATEADD(MINUTE, 1439, CAST(@Today AS DATETIME2)), N'Đúng giờ'),
(N'CC007',    N'LC_W07',    DATEADD(MINUTE, 1105, CAST(@Today AS DATETIME2)), DATEADD(MINUTE, 1439, CAST(@Today AS DATETIME2)), N'Trễ giờ'),
(N'CC008',    N'LC_W08',    DATEADD(MINUTE, 472, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 840, CAST(@Today AS DATETIME2)),  N'Đúng giờ'),
(N'CC009',    N'LC_W09',    NULL,                                             NULL,                                             N'Vắng mặt'),
(N'CC010',    N'LC_W10',    DATEADD(MINUTE, 835, CAST(@Today AS DATETIME2)),  DATEADD(MINUTE, 1215, CAST(@Today AS DATETIME2)), N'Đúng giờ'),
(N'CC_NV2_1', N'LC_NV2_P1', DATEADD(MINUTE, 474, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2)), DATEADD(MINUTE, 843, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2)),  N'Đúng giờ'),
(N'CC_NV2_2', N'LC_NV2_P2', DATEADD(MINUTE, 852, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2)), DATEADD(MINUTE, 1205, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2)), N'Trễ giờ'),
(N'CC_NV2_3', N'LC_NV2_P3', DATEADD(MINUTE, 470, CAST(DATEADD(DAY, -1, @Today) AS DATETIME2)), DATEADD(MINUTE, 850, CAST(DATEADD(DAY, -1, @Today) AS DATETIME2)),  N'Đúng giờ');

-- 3.9. Đơn Xin Nghỉ
INSERT INTO [dbo].[DonXinNghis] ([MaDon], [MaNhanVien], [TuNgay], [DenNgay], [LyDo], [TrangThai]) VALUES
(N'DN001', N'NV002', DATEADD(DAY, 2, @Today),  DATEADD(DAY, 3, @Today),  N'Giải quyết việc gia đình ở quê',      N'Chờ duyệt'),
(N'DN002', N'NV005', DATEADD(DAY, 1, @Today),  DATEADD(DAY, 1, @Today),  N'Khám sức khỏe định kỳ tại bệnh viện', N'Chờ duyệt'),
(N'DN003', N'NV008', DATEADD(DAY, 4, @Today),  DATEADD(DAY, 5, @Today),  N'Tham dự lễ tốt nghiệp đại học',       N'Chờ duyệt'),
(N'DN004', N'NV004', DATEADD(DAY, -5, @Today), DATEADD(DAY, -4, @Today), N'Nghỉ phép năm theo chế độ',           N'Đã duyệt'),
(N'DN005', N'NV007', DATEADD(DAY, -3, @Today), DATEADD(DAY, -3, @Today), N'Bị cảm sốt, xin nghỉ 1 ca tối',       N'Đã duyệt'),
(N'DN006', N'NV010', DATEADD(DAY, -7, @Today), DATEADD(DAY, -6, @Today), N'Đi đám cưới anh ruột tại Đà Nẵng',    N'Đã duyệt');

-- 3.10. Yêu Cầu Đổi Ca
INSERT INTO [dbo].[YeuCauDoiCas] ([MaYeuCau], [MaNhanVien], [MaLichCu], [MaLichMoi], [NgayGui], [LyDo], [TrangThai]) VALUES
(N'DC001', N'NV002', N'LC_W26', N'LC_W30', @Today,                  N'Bận lịch thi học kỳ buổi sáng, xin đổi sang ca tối', N'Chờ duyệt'),
(N'DC002', N'NV005', N'LC_W28', N'LC_W33', DATEADD(DAY, -1, @Today), N'Đổi ca trực quầy lễ tân với Bảo Ngọc',              N'Chờ duyệt'),
(N'DC003', N'NV012', N'LC_W21', N'LC_W32', DATEADD(DAY, -2, @Today), N'Có lịch hẹn nha khoa buổi sáng',                    N'Đã duyệt');
GO

PRINT N'=== ĐÃ KHỞI TẠO XONG CSDL StarZoneHRM VÀ DỮ LIỆU MẪU THÀNH CÔNG ===';
GO

