-- Insert
CREATE PROC sp_NhaCC_Insert
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

-- Update
CREATE PROC sp_NhaCC_Update
(
    @MaNCC VARCHAR(10),
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @MaXa CHAR(6),
    @MaQG CHAR(2)
)
AS
BEGIN
    UPDATE NhaCC
    SET TenNCC = @TenNCC,
        DienThoaiNCC = @DienThoaiNCC,
        EmailNCC = @EmailNCC,
        MaXa = @MaXa,
        MaQG = @MaQG
    WHERE MaNCC = @MaNCC;
END;
GO

-- Delete
CREATE PROC sp_NhaCC_Delete
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    DELETE FROM NhaCC WHERE MaNCC = @MaNCC;
END;
GO

-- GetAll
CREATE PROC sp_NhaCC_GetAll
AS
BEGIN
    SELECT NCC.*, Xa.TenXa, Tinh.TenTinh, QG.TenQG
    FROM NhaCC NCC
    LEFT JOIN Xa ON NCC.MaXa = Xa.MaXa
    LEFT JOIN Tinh ON Xa.MaTinh = Tinh.MaTinh
    LEFT JOIN QuocGia QG ON NCC.MaQG = QG.MaQG;
END;
GO

-- Get by ID
CREATE PROC sp_NhaCC_GetByID
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SELECT NCC.*, Xa.TenXa, Tinh.TenTinh, QG.TenQG
    FROM NhaCC NCC
    LEFT JOIN Xa ON NCC.MaXa = Xa.MaXa
    LEFT JOIN Tinh ON Xa.MaTinh = Tinh.MaTinh
    LEFT JOIN QuocGia QG ON NCC.MaQG = QG.MaQG
    WHERE NCC.MaNCC = @MaNCC;
END;
GO

