USE DB_QLBH;
GO

-- =========================
-- Thêm tỉnh
-- =========================
CREATE OR ALTER PROC Tinh_Insert
(
    @TenTinh NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên tỉnh
    IF EXISTS (SELECT 1 FROM Tinh WHERE TenTinh = @TenTinh)
    BEGIN
        RAISERROR (N'Tên tỉnh đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaTinh CHAR(2);
    DECLARE @So INT;

    -- lấy số lớn nhất
    SELECT @So = MAX(CAST(MaTinh AS INT)) FROM Tinh;

    IF @So IS NULL
        SET @So = 1;
    ELSE
        SET @So = @So + 1;

    -- tạo mã tỉnh 2 ký tự
    SET @MaTinh = RIGHT('00' + CAST(@So AS VARCHAR(2)), 2);

    INSERT INTO Tinh(MaTinh, TenTinh)
    VALUES (@MaTinh, @TenTinh);
END;
GO

-- =========================
-- Cập nhật tỉnh
-- =========================
CREATE OR ALTER PROC Tinh_Update
(
    @MaTinh CHAR(2),
    @TenTinh NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên tỉnh
    IF EXISTS (
        SELECT 1 FROM Tinh 
        WHERE TenTinh = @TenTinh AND MaTinh <> @MaTinh
    )
    BEGIN
        RAISERROR (N'Tên tỉnh đã tồn tại.', 16, 1);
        RETURN;
    END;

    UPDATE Tinh
    SET TenTinh = @TenTinh
    WHERE MaTinh = @MaTinh;
END;
GO

-- =========================
-- Lấy tất cả tỉnh
-- =========================
CREATE OR ALTER PROC Tinh_GetAll
AS
BEGIN
    SELECT * FROM Tinh;
END;
GO

-- =========================
-- Lấy tỉnh theo mã
-- =========================
CREATE OR ALTER PROC Tinh_GetByID
(
    @MaTinh CHAR(2)
)
AS
BEGIN
    SELECT * 
    FROM Tinh 
    WHERE MaTinh = @MaTinh;
END;
GO

-- =========================
-- Xóa tỉnh
-- =========================
CREATE OR ALTER PROC Tinh_Delete
(
    @MaTinh CHAR(2)
)
AS
BEGIN
    DELETE FROM Tinh 
    WHERE MaTinh = @MaTinh;
END;
GO

-- =========================
-- Tìm kiếm tỉnh
-- =========================
CREATE OR ALTER PROC Tinh_Search
(
    @Search NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Tinh
    WHERE
        @Search IS NULL OR @Search = ''
        OR MaTinh LIKE '%' + @Search + '%'
        OR TenTinh LIKE N'%' + @Search + N'%';
END;
GO
