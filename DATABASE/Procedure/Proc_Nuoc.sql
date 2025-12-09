USE DB_QLBH;
GO

-- Thêm quốc gia
CREATE OR ALTER PROC Nuoc_Insert
(
    @TenNuoc NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Nuoc WHERE TenNuoc = @TenNuoc)
    BEGIN
        RAISERROR(N'Nước đã tồn tại.', 16, 1);
        RETURN;
    END

    DECLARE @MaNuoc CHAR(5);
    SELECT @MaNuoc = 'QG' + RIGHT('000' + CAST(ISNULL(MAX(CAST(SUBSTRING(MaNuoc,3,3) AS INT)),0) + 1 AS VARCHAR), 3)
    FROM Nuoc;

    INSERT INTO Nuoc(MaNuoc, TenNuoc)
    VALUES(@MaNuoc, @TenNuoc);

    PRINT N'Thêm nước thành công! Mã nước: ' + @MaNuoc;
END;
GO

-- Lấy tất cả nước
CREATE OR ALTER PROC Nuoc_GetAll AS
SELECT * FROM Nuoc;
GO

-- Lấy nước theo ID
CREATE OR ALTER PROC Nuoc_GetByID(@MaNuoc CHAR(5)) AS
SELECT * FROM Nuoc WHERE MaNuoc=@MaNuoc;
GO

-- Xóa nước
CREATE OR ALTER PROC Nuoc_Delete(@MaNuoc CHAR(5)) AS
DELETE FROM Nuoc WHERE MaNuoc=@MaNuoc;
GO

-- Cập nhật nước
CREATE OR ALTER PROC Nuoc_Update(@MaNuoc CHAR(5), @TenNuoc NVARCHAR(100))
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Nuoc WHERE MaNuoc=@MaNuoc)
    BEGIN
        RAISERROR(N'Không tìm thấy nước để cập nhật.',16,1);
        RETURN;
    END

    IF EXISTS(SELECT 1 FROM Nuoc WHERE TenNuoc=@TenNuoc AND MaNuoc<>@MaNuoc)
    BEGIN
        RAISERROR(N'Tên nước đã tồn tại.',16,1);
        RETURN;
    END

    UPDATE Nuoc SET TenNuoc=@TenNuoc WHERE MaNuoc=@MaNuoc;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE Nuoc_Search
    @Search NVARCHAR(200) = NULL
AS
BEGIN    
    SELECT * FROM Nuoc N       
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            N.TenNuoc LIKE '%' + @Search + '%' 
        )
END;
GO