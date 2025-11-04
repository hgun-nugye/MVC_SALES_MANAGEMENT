CREATE DATABASE DB_QLBH;
GO
USE DB_QLBH;
GO

-- =============================
-- BẢNG NHÀ CUNG CẤP
-- =============================
CREATE TABLE NhaCC (
    MaNCC VARCHAR(10) PRIMARY KEY NOT NULL,
    TenNCC NVARCHAR(100) NOT NULL,
    DienThoaiNCC VARCHAR(15) NOT NULL,
    EmailNCC VARCHAR(100) NULL,
	DiaChiNCC NVARCHAR(255) NOT NULL
);
GO


-- =============================
-- KHÁCH HÀNG
-- =============================
CREATE TABLE KhachHang(
	MaKH VARCHAR(10) NOT NULL PRIMARY KEY,
	TenKH NVARCHAR(50) NOT NULL,
	DienThoaiKH VARCHAR(10) NOT NULL,
	EmailKH NVARCHAR(255) NOT NULL,
	DiaChiKH NVARCHAR(255) NOT NULL
);
GO


-- =============================
-- BẢNG NHÓM SẢN PHẨM
-- =============================
CREATE TABLE NhomSP (
    MaNhom VARCHAR(10) PRIMARY KEY NOT NULL,
    TenNhom NVARCHAR(100) NOT NULL
);
GO

-- =============================
-- LOẠI SẢN PHẨM
-- =============================
CREATE TABLE LoaiSP(
	MaLoai VARCHAR(10) NOT NULL PRIMARY KEY,
	TenLoai NVARCHAR(50) NOT NULL,
	MaNhom VARCHAR(10) NOT NULL,
    CONSTRAINT FK_LoaiSP_NhomSP FOREIGN KEY (MaNhom)
        REFERENCES NhomSP(MaNhom)
);
GO

-- =============================
-- SẢN PHẨM
-- =============================
CREATE TABLE SanPham(
	MaSP VARCHAR(10) NOT NULL PRIMARY KEY,
	TenSP NVARCHAR(50) NOT NULL,
	DonGia DECIMAL(18,2) NOT NULL,
	GiaBan DECIMAL(18,2) NOT NULL,
	MoTaSP NVARCHAR(MAX) NOT NULL,
	AnhMH NVARCHAR(255) NOT NULL,            -- Ảnh minh họa
	TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN (N'Còn Hàng', N'Hết Hàng',N'Cháy Hàng', N'Sắp Hết')),
	SoLuongTon INT NOT NULL,
	MaLoai VARCHAR(10) NOT NULL,
	MaNCC VARCHAR(10) NOT NULL,
	CONSTRAINT FK_SanPham_LoaiSP FOREIGN KEY (MaLoai)
		REFERENCES LoaiSP(MaLoai) ON DELETE CASCADE,
	CONSTRAINT FK_SanPham_NhaCC FOREIGN KEY (MaNCC)
		REFERENCES NhaCC(MaNCC) ON DELETE CASCADE
);
GO

-- =============================
-- ĐƠN MUA HÀNG (NHẬP HÀNG)
-- =============================
CREATE TABLE DonMuaHang(
	MaDMH CHAR(11) NOT NULL PRIMARY KEY,
	NgayMH DATE NOT NULL,
	MaNCC VARCHAR(10) NOT NULL,
	CONSTRAINT FK_DonMuaHang_NhaCC FOREIGN KEY (MaNCC)
		REFERENCES NhaCC(MaNCC) ON DELETE CASCADE
);
GO

-- =============================
-- ĐƠN BÁN HÀNG (XUẤT HÀNG)
-- =============================
CREATE TABLE DonBanHang(
	MaDBH CHAR(11) NOT NULL PRIMARY KEY,
	NgayBH DATE NOT NULL,
	MaKH VARCHAR(10) NOT NULL,
	CONSTRAINT FK_DonBanHang_KhachHang FOREIGN KEY (MaKH)
		REFERENCES KhachHang(MaKH) ON DELETE CASCADE
);
GO

-- =============================
-- CHI TIẾT MUA HÀNG 
-- =============================
CREATE TABLE CTMH(
	MaDMH CHAR(11) NOT NULL,
	MaSP VARCHAR(10) NOT NULL, 
	SLM INT NOT NULL,
	DGM DECIMAL(18,2) NOT NULL,
	CONSTRAINT PK_CTMH PRIMARY KEY(MaDMH, MaSP),
	CONSTRAINT FK_CTMH_DonMuaHang FOREIGN KEY (MaDMH)
		REFERENCES DonMuaHang(MaDMH) ON DELETE  NO ACTION,
	CONSTRAINT FK_CTMH_SanPham FOREIGN KEY (MaSP)
		REFERENCES SanPham(MaSP) ON DELETE NO ACTION
);
GO

-- =============================
-- CHI TIẾT BÁN HÀNG
-- =============================
CREATE TABLE CTBH(
	MaDBH CHAR(11) NOT NULL,
	MaSP VARCHAR(10) NOT NULL, 
	SLB INT NOT NULL,
	DGB DECIMAL(18,2) NOT NULL,
	CONSTRAINT PK_CTBH PRIMARY KEY(MaDBH, MaSP),
	CONSTRAINT FK_CTBH_DonBanHang FOREIGN KEY (MaDBH)
		REFERENCES DonBanHang(MaDBH) ON DELETE  NO ACTION,
	CONSTRAINT FK_CTBH_SanPham FOREIGN KEY (MaSP)
		REFERENCES SanPham(MaSP) ON DELETE  NO ACTION
);
GO

-- =============================
-- KHUYẾN MÃI
-- =============================
CREATE TABLE KhuyenMai (
    MaKM VARCHAR(10) PRIMARY KEY NOT NULL,
    TenKM NVARCHAR(100) NOT NULL,          
    MoTaKM NVARCHAR(MAX) NULL,                
    GiaTriKM DECIMAL(10,2) NOT NULL,       
    NgayBatDau DATETIME NOT NULL,
    NgayKetThuc DATETIME NOT NULL,
    DieuKienApDung NVARCHAR(255) NULL,     
    TrangThai BIT DEFAULT 1
);

-- =============================
-- ĐÁNH GIÁ SẢN PHẨM
-- =============================
CREATE TABLE DanhGia (
    MaDG INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    MaSP VARCHAR(10) NOT NULL,
    MaKH VARCHAR(10) NOT NULL,
    SoSao TINYINT CHECK (SoSao BETWEEN 1 AND 5),
    NoiDung NVARCHAR(MAX) NULL,
    NgayDG DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DanhGia_SanPham FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP) ON DELETE CASCADE,
    CONSTRAINT FK_DanhGia_KhachHang FOREIGN KEY (MaKH)
        REFERENCES KhachHang(MaKH) ON DELETE CASCADE
);
GO

-- =============================
-- TÀI KHOẢN NGƯỜI DÙNG
-- =============================
CREATE TABLE TaiKhoan (
    TenUser VARCHAR(50) PRIMARY KEY NOT NULL,   -- Tên đăng nhập (Username)
    MatKhau NVARCHAR(255) NOT NULL,           -- Mật khẩu (đã mã hóa)
    VaiTro NVARCHAR(20) NOT NULL CHECK (VaiTro IN (N'Admin', N'KhachHang')),
    TrangThai BIT DEFAULT 1,                  -- 1: Hoạt động, 0: Bị khóa
    NgayTao DATETIME DEFAULT GETDATE(),       -- Thời gian tạo tài khoản
    MaKH VARCHAR(10) NULL,                    -- Nếu là khách hàng
    CONSTRAINT FK_TaiKhoan_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO
