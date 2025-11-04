USE DB_QLBH;
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonMuaHang_Insert
(
    @NgayMH DATE,
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDMH CHAR(11);
    DECLARE @Count INT;
    DECLARE @Prefix VARCHAR(8);

    -- Prefix: M + YYMMDD (ví dụ: M251028)
    SET @Prefix = 'M' +
                  RIGHT(CAST(YEAR(@NgayMH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

    -- Đếm số đơn trong ngày đó
    SELECT @Count = COUNT(*) + 1
    FROM DonMuaHang
    WHERE CONVERT(DATE, NgayMH) = @NgayMH;

    -- Gộp thành mã: MYYMMDD#### → M2510280001
    SET @MaDMH = @Prefix + RIGHT('0000' + CAST(@Count AS VARCHAR(4)), 4);

    -- Kiểm tra NCC có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Thêm mới
    INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC)
    VALUES (@MaDMH, @NgayMH, @MaNCC);

    -- Xuất ra mã mới để kiểm tra / log
    SELECT @MaDMH AS MaDonMuaHangMoi;
END;
GO

---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonMuaHang_Update
(
    @MaDMH CHAR(11),
    @NgayMH DATE,
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra mã đơn tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
    BEGIN
        RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra NCC tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    UPDATE DonMuaHang
    SET NgayMH = @NgayMH,
        MaNCC = @MaNCC
    WHERE MaDMH = @MaDMH;
END;
GO

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonMuaHang_Delete
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
    BEGIN
        RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM DonMuaHang WHERE MaDMH = @MaDMH;
END;
GO

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonMuaHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    ORDER BY DMH.NgayMH DESC;
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonMuaHang_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE DMH.MaDMH = @MaDMH;
END;
GO
