USE DB_QLBH;
GO
-- =========================================
-- ============ INSERT =====================
-- =========================================
CREATE OR ALTER PROC sp_DanhGia_Insert
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
CREATE OR ALTER PROC sp_DanhGia_Update
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
CREATE OR ALTER PROC sp_DanhGia_Delete
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
CREATE OR ALTER PROC sp_DanhGia_GetAll
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
CREATE OR ALTER PROC sp_DanhGia_GetByID
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
CREATE OR ALTER PROC sp_DanhGia_GetBySP
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
