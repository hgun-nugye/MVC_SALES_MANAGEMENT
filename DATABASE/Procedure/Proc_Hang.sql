USE DB_QLBH;
GO

-- Thêm hãng
CREATE OR ALTER PROC Hang_Insert
(
    @TenHang NVARCHAR(100),
    @MaNuoc CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Hang WHERE TenHang = @TenHang AND MaNuoc=@MaNuoc)
    BEGIN
        RAISERROR(N'Hãng đã tồn tại cho nước này.',16,1);
        RETURN;
    END

    DECLARE @MaHang CHAR(5);
    SELECT @MaHang = 'H' + RIGHT('0000' + CAST(ISNULL(MAX(CAST(SUBSTRING(MaHang,3,4) AS INT)),0)+1 AS VARCHAR),4)
    FROM Hang;

    INSERT INTO Hang(MaHang, TenHang, MaNuoc)
    VALUES(@MaHang, @TenHang, @MaNuoc);

    PRINT N'Thêm hãng thành công! Mã hãng: ' + @MaHang;
END;
GO

-- Lấy tất cả hãng
CREATE OR ALTER PROC  Hang_GetAll AS
SELECT H.*, N.TenNuoc FROM Hang H LEFT JOIN Nuoc N ON H.MaNuoc=N.MaNuoc;
GO

-- Lấy hãng theo ID
CREATE OR ALTER PROC Hang_GetByID(@MaHang CHAR(5)) AS
SELECT H.*, N.TenNuoc FROM Hang H JOIN Nuoc N ON H.MaNuoc=N.MaNuoc WHERE H.MaHang=@MaHang;
GO

-- Xóa hãng
CREATE OR ALTER PROC Hang_Delete(@MaHang CHAR(5)) AS
DELETE FROM Hang WHERE MaHang=@MaHang;
GO

-- Cập nhật hãng
CREATE OR ALTER PROC Hang_Update
(
    @MaHang CHAR(5),
    @TenHang NVARCHAR(100),
    @MaNuoc CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM Hang WHERE TenHang=@TenHang AND MaNuoc=@MaNuoc AND MaHang<>@MaHang)
    BEGIN
        RAISERROR(N'Tên hãng đã tồn tại trong nước này.',16,1);
        RETURN;
    END

    UPDATE Hang SET TenHang=@TenHang, MaNuoc=@MaNuoc WHERE MaHang=@MaHang;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE Hang_Search
    @Search NVARCHAR(200) = NULL,        
    @MaNuoc CHAR(5) = NULL
AS
BEGIN    
    SELECT H.*, N.TenNuoc
	FROM Hang H
	JOIN Nuoc N ON H.MaNuoc = N.MaNuoc
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            H.TenHang LIKE '%' + @Search + '%' 
        )
        AND (
            @MaNuoc IS NULL 
			OR H.MaNuoc = @MaNuoc)
END;
GO