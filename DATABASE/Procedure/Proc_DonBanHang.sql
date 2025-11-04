USE DB_QLBH;
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonBanHang_Insert
(
    @NgayBH DATE,
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDBH CHAR(11);
    DECLARE @Count INT;
    DECLARE @Prefix VARCHAR(8);

    -- Prefix: B + YYMMDD (ví dụ: B251028)
    SET @Prefix = 'B' +
                  RIGHT(CAST(YEAR(@NgayBH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

    -- Đếm số đơn trong cùng ngày để tạo số thứ tự
    SELECT @Count = COUNT(*) + 1
    FROM DonBanHang
    WHERE CONVERT(DATE, NgayBH) = @NgayBH;

    -- Sinh mã mới: BYYMMDD#### (vd: B2510280001)
    SET @MaDBH = @Prefix + RIGHT('0000' + CAST(@Count AS VARCHAR(4)), 4);

    -- Kiểm tra khách hàng có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Mã khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Thêm đơn bán hàng
    INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH)
    VALUES (@MaDBH, @NgayBH, @MaKH);

    -- Xuất mã mới tạo
    SELECT @MaDBH AS MaDonBanHangMoi;
END;
GO

---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra đơn hàng tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra khách hàng tồn tại
    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Mã khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    UPDATE DonBanHang
    SET NgayBH = @NgayBH,
        MaKH = @MaKH
    WHERE MaDBH = @MaDBH;
END;
GO

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonBanHang_Delete
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM DonBanHang WHERE MaDBH = @MaDBH;
END;
GO

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonBanHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DBH.*, KH.TenKH
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    ORDER BY DBH.NgayBH DESC;
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC sp_DonBanHang_GetByID
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DBH.*, KH.TenKH
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE DBH.MaDBH = @MaDBH;
END;
GO
