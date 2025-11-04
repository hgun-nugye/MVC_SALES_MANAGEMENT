USE DB_QLBH;
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_Insert
(
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
    SET NOCOUNT ON;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @Next INT;

    -- Lấy mã lớn nhất hiện có, rồi cộng thêm 1
    SELECT @Next = ISNULL(MAX(CAST(SUBSTRING(MaSP, 8, LEN(MaSP)) AS INT)), 0) + 1
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@Next AS VARCHAR(8)), 8);

    -- Kiểm tra trùng tên sản phẩm
    IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP)
    BEGIN
        RAISERROR(N'Tên sản phẩm đã tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã loại hợp lệ
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã nhà cung cấp hợp lệ
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Thực hiện thêm mới
    INSERT INTO SanPham(MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH, TrangThai, SoLuongTon, MaLoai, MaNCC)
    VALUES (@MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC);
END;
GO

---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_Update
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
    SET NOCOUNT ON;

    -- Kiểm tra mã sản phẩm
    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã loại hợp lệ
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã nhà cung cấp hợp lệ
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

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

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_Delete
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM SanPham WHERE MaSP = @MaSP;
END;
GO

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SP.*, 
           L.TenLSP, 
           N.TenNCC
    FROM SanPham SP
    JOIN LoaiSP L ON SP.MaLoai = L.MaLoai
    JOIN NhaCC N ON SP.MaNCC = N.MaNCC;
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_GetByID
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SP.*, 
           L.TenLSP, 
           N.TenNCC
    FROM SanPham SP
    JOIN LoaiSP L ON SP.MaLoai = L.MaLoai
    JOIN NhaCC N ON SP.MaNCC = N.MaNCC
    WHERE SP.MaSP = @MaSP;
END;
GO

---------------------------------------------------------
-- ========== SEARCH (tùy chọn, cho MVC lọc dữ liệu) ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_SanPham_Search
(
    @Keyword NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SP.*, 
           L.TenLSP, 
           N.TenNCC
    FROM SanPham SP
    JOIN LoaiSP L ON SP.MaLoai = L.MaLoai
    JOIN NhaCC N ON SP.MaNCC = N.MaNCC
    WHERE SP.TenSP LIKE N'%' + @Keyword + '%'
       OR L.TenLSP LIKE N'%' + @Keyword + '%'
       OR N.TenNCC LIKE N'%' + @Keyword + '%';
END;
GO
