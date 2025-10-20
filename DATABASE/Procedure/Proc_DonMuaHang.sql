-- Insert
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

    -- Prefix: M + YYMMDD (rút gọn từ @NgayMH)
    SET @Prefix = 'M' + 
                  RIGHT(CAST(YEAR(@NgayMH) AS VARCHAR(4)), 2) + 
                  RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2) + 
                  RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

    -- Đếm số lượng đơn trong cùng ngày để tạo thứ tự 4 chữ số
    SELECT @Count = COUNT(*) + 1 
    FROM DonMuaHang 
    WHERE CONVERT(DATE, NgayMH) = @NgayMH;

    -- Gộp thành mã cuối: MYYMMDD#### (vd: M2510190001)
    SET @MaDMH = @Prefix + RIGHT('0000' + CAST(@Count AS VARCHAR(4)), 4);

    -- Thực hiện thêm
    INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC)
    VALUES (@MaDMH, @NgayMH, @MaNCC);

    -- Xuất mã vừa tạo (để dễ test hoặc log)
    SELECT @MaDMH AS MaDonMuaHangMoi;
END;

GO

-- Update
CREATE PROC sp_DonMuaHang_Update
(
    @MaDMH CHAR(11),
    @NgayMH DATE,
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    UPDATE DonMuaHang
    SET NgayMH = @NgayMH,
        MaNCC = @MaNCC
    WHERE MaDMH = @MaDMH;
END;
GO

-- Delete
CREATE PROC sp_DonMuaHang_Delete
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    DELETE FROM DonMuaHang WHERE MaDMH = @MaDMH;
END;
GO

-- GetAll
CREATE PROC sp_DonMuaHang_GetAll
AS
BEGIN
    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC;
END;
GO

--Get by ID
CREATE PROC sp_DonMuaHang_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE DMH.MaDMH = @MaDMH;
END;
GO
