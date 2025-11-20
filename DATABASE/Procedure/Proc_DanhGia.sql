USE DB_QLBH;
GO
-- =========================================
-- ============ INSERT =====================
-- =========================================
CREATE OR ALTER PROC DanhGia_Insert
(
    @MaSP VARCHAR(10),
    @MaKH VARCHAR(10),
    @SoSao TINYINT,
    @NoiDung NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DanhGia (MaSP, MaKH, SoSao, NoiDung, NgayDG)
    VALUES (@MaSP, @MaKH, @SoSao, @NoiDung, GETDATE());
END;
GO

-- =========================================
-- ============ UPDATE =====================
-- =========================================
CREATE OR ALTER PROC DanhGia_Update
(
    @MaDG INT,
    @SoSao TINYINT,
    @NoiDung NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE DanhGia
    SET SoSao = @SoSao,
        NoiDung = @NoiDung,
        NgayDG = GETDATE()
    WHERE MaDG = @MaDG;
END;
GO

-- =========================================
-- ============ DELETE =====================
-- =========================================
CREATE OR ALTER PROC DanhGia_Delete
(
    @MaDG INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM DanhGia
    WHERE MaDG = @MaDG;
END;
GO

-- =========================================
-- ============ GET ALL ====================
-- =========================================
CREATE OR ALTER PROC DanhGia_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DG.*, SP.TenSP, KH.TenKH
    FROM DanhGia DG
    JOIN SanPham SP ON DG.MaSP = SP.MaSP
    JOIN KhachHang KH ON DG.MaKH = KH.MaKH
    ORDER BY DG.NgayDG DESC;
END;
GO

-- =========================================
-- ============ GET BY ID ==================
-- =========================================
CREATE OR ALTER PROC DanhGia_GetByID
(
    @MaDG INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DG.*, SP.TenSP, KH.TenKH
    FROM DanhGia DG
    JOIN SanPham SP ON DG.MaSP = SP.MaSP
    JOIN KhachHang KH ON DG.MaKH = KH.MaKH
    WHERE DG.MaDG = @MaDG;
END;
GO

-- =========================================
-- ============ GET BY PRODUCT =============
-- =========================================
CREATE OR ALTER PROC DanhGia_GetBySP
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DG.*, KH.TenKH
    FROM DanhGia DG
    JOIN KhachHang KH ON DG.MaKH = KH.MaKH
    WHERE DG.MaSP = @MaSP
    ORDER BY DG.NgayDG DESC;
END;
GO

-- =========================================
-- SEARCH
-- =========================================
CREATE OR ALTER PROC DanhGia_Search
(
    @MaSP VARCHAR(10) = NULL,
    @MaKH VARCHAR(10) = NULL,
    @TenKH NVARCHAR(100) = NULL,
    @SoSao TINYINT = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        DG.MaDG,
        DG.MaSP,
        SP.TenSP,
        DG.MaKH,
        KH.TenKH,
        DG.SoSao,
        DG.NoiDung,
        DG.NgayDG
    FROM DanhGia DG
    JOIN SanPham SP ON DG.MaSP = SP.MaSP
    JOIN KhachHang KH ON DG.MaKH = KH.MaKH
    WHERE
        (@MaSP IS NULL OR DG.MaSP = @MaSP)
        AND (@MaKH IS NULL OR DG.MaKH = @MaKH)
        AND (@TenKH IS NULL OR KH.TenKH LIKE '%' + @TenKH + '%')
        AND (@SoSao IS NULL OR DG.SoSao = @SoSao)
        AND (@TuNgay IS NULL OR DG.NgayDG >= @TuNgay)
        AND (@DenNgay IS NULL OR DG.NgayDG <= @DenNgay)
    ORDER BY DG.NgayDG DESC;
END;
GO
