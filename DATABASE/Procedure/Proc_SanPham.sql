-- Insert
CREATE PROC sp_SanPham_Insert
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

-- Update
CREATE PROC sp_SanPham_Update
(
    @MaSP VARCHAR(10),
    @TenSP NVARCHAR(50),
    @DonGia MONEY,
    @GiaBan MONEY,
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),
    @TrangThai NVARCHAR(50),
    @SoLuongTon INT,
    @MaLoai VARCHAR(10),
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    UPDATE SanPham
    SET TenSP = @TenSP,
        DonGia = @DonGia,
        GiaBan = @GiaBan,
        MoTaSP = @MoTaSP,
        AnhMH = @AnhMH,
        TrangThai = @TrangThai,
        SoLuongTon = @SoLuongTon,
        MaLoai = @MaLoai,
        MaNCC = @MaNCC
    WHERE MaSP = @MaSP;
END;
GO

-- Delete
CREATE PROC sp_SanPham_Delete
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    DELETE FROM SanPham WHERE MaSP = @MaSP;
END;
GO

-- GetAll
CREATE PROC sp_SanPham_GetAll
AS
BEGIN
    SELECT SP.*, L.TenLSP, NCC.TenNCC
    FROM SanPham SP
    JOIN LoaiSP L ON SP.MaLoai = L.MaLoai
    JOIN NhaCC NCC ON SP.MaNCC = NCC.MaNCC;
END;
GO

-- Get by ID
CREATE PROC sp_SanPham_GetByID
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SELECT SP.*, L.TenLSP, NCC.TenNCC
    FROM SanPham SP
    JOIN LoaiSP L ON SP.MaLoai = L.MaLoai
    JOIN NhaCC NCC ON SP.MaNCC = NCC.MaNCC
    WHERE SP.MaSP = @MaSP;
END;
GO
