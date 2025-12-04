USE DB_QLBH;
GO

CREATE OR ALTER PROC Xa_Insert
(
    @TenXa NVARCHAR(90),
    @MaTinh SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Xa WHERE TenXa = @TenXa AND MaTinh = @MaTinh)
    BEGIN
        RAISERROR(N'Xã đã tồn tại trong tỉnh này.', 16, 1);
        RETURN;
    END;

    DECLARE @MaXa SMALLINT;
    DECLARE @Base INT = @MaTinh * 100;  -- Dải mã xã của tỉnh

    SELECT @MaXa = MAX(MaXa)
    FROM Xa
    WHERE MaXa BETWEEN @Base + 1 AND @Base + 99;

    IF (@MaXa IS NULL)
        SET @MaXa = @Base + 1;
    ELSE
        SET @MaXa = @MaXa + 1;

    INSERT INTO Xa (MaXa, TenXa, MaTinh)
    VALUES (@MaXa, @TenXa, @MaTinh);

    PRINT N'Thêm xã thành công! Mã xã: ' + CAST(@MaXa AS NVARCHAR(10));
END;
GO

CREATE OR ALTER PROC Xa_GetAll AS SELECT * FROM Xa; 
GO
CREATE OR ALTER PROC Xa_GetByID(@MaXa SMALLINT) AS SELECT * FROM Xa WHERE MaXa=@MaXa; 
GO
CREATE OR ALTER PROC Xa_Delete(@MaXa SMALLINT) AS DELETE FROM Xa WHERE MaXa=@MaXa; 
GO

CREATE OR ALTER PROC Xa_Update
(
    @MaXa SMALLINT,
    @TenXa NVARCHAR(90),
    @MaTinh SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra mã xã có tồn tại chưa
    IF NOT EXISTS (SELECT 1 FROM Xa WHERE MaXa = @MaXa)
    BEGIN
        RAISERROR(N'Không tìm thấy mã xã để cập nhật.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra trùng tên xã trong cùng tỉnh, trừ chính nó
    IF EXISTS (
        SELECT 1 
        FROM Xa 
        WHERE TenXa = @TenXa 
              AND MaTinh = @MaTinh
              AND MaXa <> @MaXa
    )
    BEGIN
        RAISERROR(N'Tên xã đã tồn tại trong tỉnh này.', 16, 1);
        RETURN;
    END;

    -- Cập nhật dữ liệu
    UPDATE Xa
    SET TenXa = @TenXa,
        MaTinh = @MaTinh
    WHERE MaXa = @MaXa;
END;
GO

-- Lấy danh sách xã theo tỉnh
CREATE OR ALTER PROC Xa_GetByIDTinh
(
    @MaTinh SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
    WHERE T.MaTinh = @MaTinh
    ORDER BY MaXa;
END;
GO

CREATE OR ALTER PROC Xa_GetByIDWithTinh
(
    @MaXa SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
    WHERE X.MaXa = @MaXa
    ORDER BY MaXa;
END;
GO

CREATE OR ALTER PROC Xa_GetAllWithTinh
AS
BEGIN
     SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
    ORDER BY MaXa;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE Xa_Search
    @Search NVARCHAR(200) = NULL,        
    @MaTinh SMALLINT = NULL
AS
BEGIN    
    SELECT 
        X.*, T.TenTinh
    FROM Xa X
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            X.TenXa LIKE '%' + @Search + '%' 
        )
        AND (
            @MaTinh IS NULL 
            OR T.MaTinh = @MaTinh
        )
END;
GO




