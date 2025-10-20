--Proc Quốc gia 
-- Thêm Quốc Gia
--Proc Nhà cung cấp
CREATE PROC NhaCC_Insert
(
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @MaXa CHAR(6),
    @MaQG CHAR(2)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaNCC VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM NhaCC;
    SET @MaNCC = 'NCC' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO NhaCC(MaNCC, TenNCC, DienThoaiNCC, EmailNCC, MaXa, MaQG)
    VALUES (@MaNCC, @TenNCC, @DienThoaiNCC, @EmailNCC, @MaXa, @MaQG);
END;
GO


--Proc Khách hàng
CREATE PROC KhachHang_Insert
(
    @TenKH NVARCHAR(50),
    @DienThoaiKH VARCHAR(10),
    @EmailKH VARCHAR(255),
    @DiaChiKH NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaKH VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM KhachHang;
    SET @MaKH = 'KH' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO KhachHang(MaKH, TenKH, DienThoaiKH, EmailKH, DiaChiKH)
    VALUES (@MaKH, @TenKH, @DienThoaiKH, @EmailKH, @DiaChiKH);
END;
GO

-- Proc Loại SP
CREATE PROC LoaiSP_Insert
(
    @TenLSP NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaLoai VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM LoaiSP;
    SET @MaLoai = 'LSP' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO LoaiSP(MaLoai, TenLSP)
    VALUES (@MaLoai, @TenLSP);
END;
GO

-- Proc Sản phẩm
CREATE PROC SanPham_Insert
(
    @TenSP NVARCHAR(50),
    @DonGia MONEY,
    @GiaBan MONEY,
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(50),
    @TrangThai NVARCHAR(50),
    @SoLuongTon INT,
    @MaLoai VARCHAR(10),
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM SanPham;
    SET @MaSP = 'SP' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO SanPham(MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH, TrangThai, SoLuongTon, MaLoai, MaNCC)
    VALUES (@MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC);
END;
GO

-- Proc Đơn MH
CREATE PROC DonMuaHang_Insert
(
    @NgayMH DATE,
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDMH CHAR(11);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM DonMuaHang;
    SET @MaDMH = 'DMH' + RIGHT('0000000' + CAST(@Count AS VARCHAR(7)), 7);

    INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC)
    VALUES (@MaDMH, @NgayMH, @MaNCC);
END;
GO

-- Proc Đơn BH
CREATE PROC DonBanHang_Insert
(
    @NgayBH DATE,
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDBH CHAR(11);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM DonBanHang;
    SET @MaDBH = 'DBH' + RIGHT('0000000' + CAST(@Count AS VARCHAR(7)), 7);

    INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH)
    VALUES (@MaDBH, @NgayBH, @MaKH);
END;
GO

-- Proc Khuyến mãi
CREATE PROC KhuyenMai_Insert
(
    @TenKM NVARCHAR(100),
    @MoTa TEXT,
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255),
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaKM VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM KhuyenMai;
    SET @MaKM = 'KM' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO KhuyenMai(MaKM, TenKM, MoTa, GiaTriKM, NgayBatDau, NgayKetThuc, DieuKienApDung, TrangThai)
    VALUES (@MaKM, @TenKM, @MoTa, @GiaTriKM, @NgayBatDau, @NgayKetThuc, @DieuKienApDung, @TrangThai);
END;
GO
