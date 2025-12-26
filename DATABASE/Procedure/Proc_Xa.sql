USE DB_QLBH;
GO

-- =========================
-- Thêm xã
-- =========================
CREATE OR ALTER PROC Xa_Insert
(
    @TenXa  NVARCHAR(50),
    @MaTinh CHAR(2)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên xã trong cùng tỉnh
    IF EXISTS (
        SELECT 1 
        FROM Xa 
        WHERE TenXa = @TenXa AND MaTinh = @MaTinh
    )
    BEGIN
        RAISERROR (N'Xã đã tồn tại trong tỉnh này.', 16, 1);
        RETURN;
    END;

    DECLARE @MaXa CHAR(5);
    DECLARE @So INT;

    -- lấy số lớn nhất của xã trong tỉnh
    SELECT @So = MAX(CAST(RIGHT(MaXa, 3) AS INT))
    FROM Xa
    WHERE MaTinh = @MaTinh;

    IF @So IS NULL
        SET @So = 1;
    ELSE
        SET @So = @So + 1;

    -- tạo mã xã: MaTinh + 3 số
    SET @MaXa = @MaTinh + RIGHT('000' + CAST(@So AS VARCHAR(3)), 3);

    INSERT INTO Xa(MaXa, TenXa, MaTinh)
    VALUES (@MaXa, @TenXa, @MaTinh);
END;
GO

-- =========================
-- Lấy tất cả xã
-- =========================
CREATE OR ALTER PROC Xa_GetAll
AS
BEGIN
    SELECT * FROM Xa;
END;
GO

-- =========================
-- Lấy xã theo mã
-- =========================
CREATE OR ALTER PROC Xa_GetByID
(
    @MaXa CHAR(5)
)
AS
BEGIN
    SELECT * 
    FROM Xa 
    WHERE MaXa = @MaXa;
END;
GO

-- =========================
-- Xóa xã
-- =========================
CREATE OR ALTER PROC Xa_Delete
(
    @MaXa CHAR(5)
)
AS
BEGIN
    DELETE FROM Xa 
    WHERE MaXa = @MaXa;
END;
GO

-- =========================
-- Cập nhật xã
-- =========================
CREATE OR ALTER PROC Xa_Update
(
    @MaXa  CHAR(5),
    @TenXa NVARCHAR(50),
    @MaTinh CHAR(2)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên xã
    IF EXISTS (
        SELECT 1
        FROM Xa
        WHERE TenXa = @TenXa
          AND MaTinh = @MaTinh
          AND MaXa <> @MaXa
    )
    BEGIN
        RAISERROR (N'Tên xã đã tồn tại trong tỉnh này.', 16, 1);
        RETURN;
    END;

    UPDATE Xa
    SET TenXa = @TenXa,
        MaTinh = @MaTinh
    WHERE MaXa = @MaXa;
END;
GO

-- =========================
-- Lấy xã theo tỉnh
-- =========================
CREATE OR ALTER PROC Xa_GetByIDTinh
(
    @MaTinh CHAR(2)
)
AS
BEGIN
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE X.MaTinh = @MaTinh
    ORDER BY X.MaXa;
END;
GO

-- =========================
-- Lấy xã + tỉnh theo mã xã
-- =========================
CREATE OR ALTER PROC Xa_GetByIDWithTinh
(
    @MaXa CHAR(5)
)
AS
BEGIN
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE X.MaXa = @MaXa;
END;
GO

-- =========================
-- Lấy tất cả xã + tỉnh
-- =========================
CREATE OR ALTER PROC Xa_GetAllWithTinh
AS
BEGIN
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    ORDER BY X.MaXa;
END;
GO

-- =========================
-- Tìm kiếm xã
-- =========================
CREATE OR ALTER PROC Xa_Search
(
    @Search NVARCHAR(200) = NULL,
    @MaTinh CHAR(2) = NULL
)
AS
BEGIN
    SELECT X.MaXa, X.TenXa, X.MaTinh, T.TenTinh
    FROM Xa X
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (
            @Search IS NULL OR @Search = ''
            OR X.TenXa LIKE N'%' + @Search + N'%'
        )
        AND (
            @MaTinh IS NULL
            OR X.MaTinh = @MaTinh
        );
END;
GO
