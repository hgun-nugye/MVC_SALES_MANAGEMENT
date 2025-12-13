USE DB_QLBH;
GO
-- INSERT
CREATE OR ALTER PROC Tinh_Insert
(
    @TenTinh NVARCHAR(90)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng tên tỉnh
    IF EXISTS (SELECT 1 FROM Tinh WHERE TenTinh = @TenTinh)
    BEGIN
        RAISERROR(N'Tên tỉnh đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaTinh SMALLINT;
    SELECT @MaTinh = ISNULL(MAX(MaTinh), 0) + 1 FROM Tinh;

    INSERT INTO Tinh (MaTinh, TenTinh)
    VALUES (@MaTinh, @TenTinh);
END;
GO

-- UPDATE
CREATE OR ALTER PROC Tinh_Update
(@MaTinh SMALLINT, @TenTinh NVARCHAR(90))
AS
BEGIN
	 SET NOCOUNT ON;

    -- Kiểm tra trùng tên tỉnh 
    IF EXISTS (SELECT 1 FROM Tinh WHERE TenTinh = @TenTinh AND MaTinh <> @MaTinh)
    BEGIN
        RAISERROR(N'Tên tỉnh đã tồn tại.', 16, 1);
        RETURN;
    END;

    UPDATE Tinh SET TenTinh = @TenTinh WHERE MaTinh = @MaTinh;
END
GO

-- GET ALL
CREATE OR ALTER PROC Tinh_GetAll
AS
BEGIN
    SELECT * FROM Tinh;
END
GO

-- GET BY ID
CREATE OR ALTER PROC Tinh_GetByID
(@MaTinh SMALLINT)
AS
BEGIN
    SELECT * FROM Tinh WHERE MaTinh = @MaTinh;
END
GO

-- DELETE
CREATE OR ALTER PROC Tinh_Delete
(@MaTinh SMALLINT)
AS
BEGIN
    DELETE FROM Tinh WHERE MaTinh = @MaTinh;
END
GO

-- search
CREATE OR ALTER PROCEDURE Tinh_Search
    @Search      NVARCHAR(100) = NULL   
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
        FROM Tinh
    WHERE 
        (
            @Search IS NULL OR @Search = '' OR
            Tinh.MaTinh   LIKE '%' + @Search + '%' OR
            Tinh.TenTinh   LIKE '%' + @Search + '%'
        )
END;
GO