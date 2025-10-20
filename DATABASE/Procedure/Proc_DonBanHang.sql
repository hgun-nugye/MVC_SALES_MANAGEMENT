-- Insert
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

    -- Prefix = B + YYMMDD
    SET @Prefix = 'B' +
                  RIGHT(CAST(YEAR(@NgayBH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

    -- Đếm số đơn trong cùng ngày
    SELECT @Count = COUNT(*) + 1
    FROM DonBanHang
    WHERE CONVERT(DATE, NgayBH) = @NgayBH;

    -- Tạo mã mới: BYYMMDD#### (vd: B2510190001)
    SET @MaDBH = @Prefix + RIGHT('0000' + CAST(@Count AS VARCHAR(4)), 4);

    -- Thêm vào bảng
    INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH)
    VALUES (@MaDBH, @NgayBH, @MaKH);

    -- Xuất mã mới tạo
    SELECT @MaDBH AS MaDonBanHangMoi;
END;
GO


-- Update
CREATE PROC sp_DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10)
)
AS
BEGIN
    UPDATE DonBanHang
    SET NgayBH = @NgayBH,
        MaKH = @MaKH
    WHERE MaDBH = @MaDBH;
END;
GO

-- Delete
CREATE PROC sp_DonBanHang_Delete
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    DELETE FROM DonBanHang WHERE MaDBH = @MaDBH;
END;
GO

-- GetAll
CREATE PROC sp_DonBanHang_GetAll
AS
BEGIN
    SELECT DBH.*, KH.TenKH
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH;
END;
GO

--Get by ID
CREATE PROC sp_DonBanHang_GetByID
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SELECT DBH.*, KH.TenKH
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE DBH.MaDBH = @MaDBH;
END;
GO


