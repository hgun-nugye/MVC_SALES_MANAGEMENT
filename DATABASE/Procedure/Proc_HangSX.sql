USE DB_QLBH;
GO

-- Thêm hãng
CREATE OR ALTER PROC HangSX_Insert
(
    @TenHangSX NVARCHAR(100),
    @MaNuoc CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM HangSX WHERE TenHangSX = @TenHangSX AND MaNuoc=@MaNuoc)
    BEGIN
        RAISERROR(N'Hãng đã tồn tại cho nước này.',16,1);
        RETURN;
    END

    DECLARE @MaHangSX CHAR(5);
    SELECT @MaHangSX = 'H' + RIGHT('0000' + CAST(ISNULL(MAX(CAST(SUBSTRING(MaHangSX,3,4) AS INT)),0)+1 AS VARCHAR),4)
    FROM HangSX;

    INSERT INTO HangSX(MaHangSX, TenHangSX, MaNuoc)
    VALUES(@MaHangSX, @TenHangSX, @MaNuoc);

    PRINT N'Thêm hãng thành công! Mã hãng: ' + @MaHangSX;
END;
GO

-- Lấy tất cả hãng
CREATE OR ALTER PROC  HangSX_GetAll AS
SELECT H.*, N.TenNuoc FROM HangSX H LEFT JOIN Nuoc N ON H.MaNuoc=N.MaNuoc;
GO

-- Lấy hãng theo ID
CREATE OR ALTER PROC HangSX_GetByID(@MaHangSX CHAR(5)) AS
SELECT H.*, N.TenNuoc FROM HangSX H JOIN Nuoc N ON H.MaNuoc=N.MaNuoc WHERE H.MaHangSX=@MaHangSX;
GO

-- Xóa hãng
CREATE OR ALTER PROC HangSX_Delete(@MaHangSX CHAR(5)) AS
DELETE FROM HangSX WHERE MaHangSX=@MaHangSX;
GO

-- Cập nhật hãng
CREATE OR ALTER PROC HangSX_Update
(
    @MaHangSX CHAR(5),
    @TenHangSX NVARCHAR(100),
    @MaNuoc CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM HangSX WHERE TenHangSX=@TenHangSX AND MaNuoc=@MaNuoc AND MaHangSX<>@MaHangSX)
    BEGIN
        RAISERROR(N'Tên hãng đã tồn tại trong nước này.',16,1);
        RETURN;
    END

    UPDATE HangSX SET TenHangSX=@TenHangSX, MaNuoc=@MaNuoc WHERE MaHangSX=@MaHangSX;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE HangSX_Search
    @Search NVARCHAR(200) = NULL,        
    @MaNuoc CHAR(5) = NULL
AS
BEGIN    
    SELECT H.*, N.TenNuoc
	FROM HangSX H
	JOIN Nuoc N ON H.MaNuoc = N.MaNuoc
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            H.TenHangSX LIKE '%' + @Search + '%' 
        )
        AND (
            @MaNuoc IS NULL 
			OR H.MaNuoc = @MaNuoc)
END;
GO