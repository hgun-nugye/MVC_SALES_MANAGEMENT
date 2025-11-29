CREATE DATABASE DB_QLBH;
GO
USE DB_QLBH;
GO

/*====================================================
=                   BẢNG NHÀ CUNG CẤP               =
====================================================*/
CREATE TABLE NhaCC (
    MaNCC VARCHAR(10) PRIMARY KEY,
    TenNCC NVARCHAR(100) NOT NULL,
    DienThoaiNCC VARCHAR(15) NOT NULL,
    EmailNCC VARCHAR(100) NULL,
    DiaChiNCC NVARCHAR(255) NOT NULL
);
GO

/*====================================================
=                   BẢNG KHÁCH HÀNG                  =
====================================================*/
CREATE TABLE KhachHang(
    MaKH VARCHAR(10) PRIMARY KEY,
    TenKH NVARCHAR(50) NOT NULL,
    DienThoaiKH VARCHAR(15) NOT NULL,
    EmailKH NVARCHAR(255) NOT NULL,
    DiaChiKH NVARCHAR(255) NOT NULL
);
GO

/*====================================================
=                     GIAN HÀNG                      =
====================================================*/
CREATE TABLE GianHang(
    MaGH VARCHAR(10) PRIMARY KEY,
    TenGH NVARCHAR(100) NOT NULL,
    MoTaGH NVARCHAR(255) NULL,
    NgayTao DATE DEFAULT GETDATE(),
    DienThoaiGH VARCHAR(15) NOT NULL,
    EmailGH VARCHAR(100) NULL,
    DiaChiGH NVARCHAR(200) NOT NULL
);
GO

/*====================================================
=                 NHÓM SẢN PHẨM                      =
====================================================*/
CREATE TABLE NhomSP (
    MaNhom VARCHAR(10) PRIMARY KEY,
    TenNhom NVARCHAR(100) NOT NULL
);
GO

/*====================================================
=                 LOẠI SẢN PHẨM                      =
====================================================*/
CREATE TABLE LoaiSP(
    MaLoai VARCHAR(10) PRIMARY KEY,
    TenLoai NVARCHAR(50) NOT NULL,
    MaNhom VARCHAR(10) NOT NULL,
    CONSTRAINT FK_LoaiSP_NhomSP FOREIGN KEY (MaNhom)
        REFERENCES NhomSP(MaNhom) ON DELETE CASCADE
);
GO

/*====================================================
=                     SẢN PHẨM                       =
=  Thêm khóa ngoại GianHang → SanPham (MaGH)        =
====================================================*/
CREATE TABLE SanPham(
    MaSP VARCHAR(10) PRIMARY KEY,
    TenSP NVARCHAR(50) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    GiaBan DECIMAL(18,2) NOT NULL,
    MoTaSP NVARCHAR(MAX) NOT NULL,
    AnhMH NVARCHAR(255) NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL 
        CHECK (TrangThai IN (N'Còn Hàng', N'Hết Hàng', N'Cháy Hàng', N'Sắp Hết')),
    SoLuongTon INT NOT NULL,
    MaLoai VARCHAR(10) NOT NULL,
    MaNCC VARCHAR(10) NOT NULL,
    MaGH VARCHAR(10) NOT NULL,
    CONSTRAINT FK_SanPham_LoaiSP FOREIGN KEY (MaLoai)
        REFERENCES LoaiSP(MaLoai) ON DELETE CASCADE,
    CONSTRAINT FK_SanPham_NhaCC FOREIGN KEY (MaNCC)
        REFERENCES NhaCC(MaNCC) ON DELETE CASCADE,
    CONSTRAINT FK_SanPham_GianHang FOREIGN KEY (MaGH)
        REFERENCES GianHang(MaGH) ON DELETE CASCADE
);
GO

/*====================================================
=                ĐƠN MUA HÀNG (Nhập hàng)           =
====================================================*/
CREATE TABLE DonMuaHang(
    MaDMH CHAR(11) PRIMARY KEY,
    NgayMH DATE NOT NULL,
    MaNCC VARCHAR(10) NOT NULL,
    CONSTRAINT FK_DonMuaHang_NhaCC FOREIGN KEY (MaNCC)
        REFERENCES NhaCC(MaNCC) ON DELETE CASCADE
);
GO

/*====================================================
=              ĐƠN BÁN HÀNG (Xuất hàng)              =
====================================================*/
CREATE TABLE DonBanHang(
    MaDBH CHAR(11) PRIMARY KEY,
    NgayBH DATE NOT NULL,
    MaKH VARCHAR(10) NOT NULL,
    CONSTRAINT FK_DonBanHang_KhachHang FOREIGN KEY (MaKH)
        REFERENCES KhachHang(MaKH) ON DELETE CASCADE
);
GO

/*====================================================
=                 CHI TIẾT MUA HÀNG                  =
====================================================*/
CREATE TABLE CTMH(
    MaDMH CHAR(11) NOT NULL,
    MaSP VARCHAR(10) NOT NULL,
    SLM INT NOT NULL,
    DGM DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_CTMH PRIMARY KEY(MaDMH, MaSP),
    CONSTRAINT FK_CTMH_DonMuaHang FOREIGN KEY (MaDMH)
        REFERENCES DonMuaHang(MaDMH) ON DELETE CASCADE,
    CONSTRAINT FK_CTMH_SanPham FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP)
);
GO

/*====================================================
=                CHI TIẾT BÁN HÀNG                   =
====================================================*/
CREATE TABLE CTBH(
    MaDBH CHAR(11) NOT NULL,
    MaSP VARCHAR(10) NOT NULL,
    SLB INT NOT NULL,
    DGB DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_CTBH PRIMARY KEY(MaDBH, MaSP),
    CONSTRAINT FK_CTBH_DonBanHang FOREIGN KEY (MaDBH)
        REFERENCES DonBanHang(MaDBH) ON DELETE CASCADE,
    CONSTRAINT FK_CTBH_SanPham FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP) ON DELETE CASCADE
);
GO


-- Table cho Login
CREATE TABLE TaiKhoan (
    TenUser VARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(255) NOT NULL
);
GO
