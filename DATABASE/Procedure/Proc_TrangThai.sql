USE DB_QLBH;
GO

CREATE OR ALTER PROC TrangThai_Insert
(
    @TenTT VARCHAR(50)
)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TrangThai WHERE TenTT = @TenTT)
    BEGIN
        RAISERROR(N'Trạng thái đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaTT CHAR(3);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaTT, 3, 1) AS INT)), 0)
    FROM TrangThai;

    SET @MaTT = 'TT' + CAST(@MaxID + 1 AS CHAR(1));

    INSERT INTO TrangThai(MaTT, TenTT)
    VALUES (@MaTT, @TenTT);

    PRINT N'Thêm trạng thái thành công!';
END;
GO

CREATE OR ALTER PROC TrangThai_Update
(
    @MaTT CHAR(3),
    @TenTT VARCHAR(50)
)
AS
BEGIN

	IF EXISTS (SELECT 1 FROM TrangThai WHERE TenTT = @TenTT AND MaTT<>@MaTT)
    BEGIN
        RAISERROR(N'Trạng thái đã tồn tại.', 16, 1);
        RETURN;
    END;
    
    UPDATE TrangThai
    SET TenTT = @TenTT
    WHERE MaTT = @MaTT;
END;
GO

CREATE OR ALTER PROC TrangThai_Delete
(
    @MaTT CHAR(3)
)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM SanPham WHERE MaTT = @MaTT)
    BEGIN
        RAISERROR(N'Trạng thái đang được sử dụng.', 16, 1);
        RETURN;
    END;

    DELETE FROM TrangThai WHERE MaTT = @MaTT;
END;
GO

CREATE OR ALTER PROC TrangThai_GetAll
AS
BEGIN
    SELECT * FROM TrangThai ORDER BY MaTT;
END;
GO

CREATE OR ALTER PROC TrangThai_GetByID
(
    @MaTT CHAR(3)
)
AS
BEGIN
    SELECT * FROM TrangThai ORDER BY MaTT;
END;
GO


CREATE OR ALTER PROC TrangThai_Search
(
    @Search NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTT, TenTT
    FROM TrangThai
    WHERE (@Search IS NULL OR @Search = '' 
           OR MaTT LIKE '%' + @Search + '%' 
           OR TenTT LIKE N'%' + @Search + '%')
END;
GO