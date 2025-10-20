-- Insert
CREATE PROC sp_KhachHang_Insert
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

-- Update
CREATE PROC sp_KhachHang_Update
(
    @MaKH VARCHAR(10),
    @TenKH NVARCHAR(50),
    @DienThoaiKH VARCHAR(10),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255)
)
AS
BEGIN
    UPDATE KhachHang
    SET TenKH = @TenKH,
        DienThoaiKH = @DienThoaiKH,
        EmailKH = @EmailKH,
        DiaChiKH = @DiaChiKH
    WHERE MaKH = @MaKH;
END;
GO

-- Delete
CREATE PROC sp_KhachHang_Delete
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    DELETE FROM KhachHang WHERE MaKH = @MaKH;
END;
GO

-- GetAll
CREATE PROC sp_KhachHang_GetAll
AS
BEGIN
    SELECT * FROM KhachHang;
END;
GO

-- Get by ID
CREATE PROC sp_KhachHang_GetByID
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SELECT * FROM KhachHang WHERE MaKH = @MaKH;
END;
GO
