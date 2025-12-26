USE DB_QLBH;
GO

-- =========================
-- Thêm nhà cung cấp
-- =========================
CREATE OR ALTER PROC NhaCC_Insert
(
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @DiaChiNCC NVARCHAR(255),
    @MaXa CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên
    IF EXISTS (SELECT 1 FROM NhaCC WHERE TenNCC = @TenNCC)
    BEGIN
        RAISERROR(N'Tên nhà cung cấp đã tồn tại.', 16, 1);
        RETURN;
    END

    -- kiểm tra trùng số điện thoại
    IF EXISTS (SELECT 1 FROM NhaCC WHERE DienThoaiNCC = @DienThoaiNCC)
    BEGIN
        RAISERROR(N'Số điện thoại đã được sử dụng.', 16, 1);
        RETURN;
    END

    -- kiểm tra trùng email
    IF EXISTS (SELECT 1 FROM NhaCC WHERE EmailNCC = @EmailNCC)
    BEGIN
        RAISERROR(N'Email đã tồn tại.', 16, 1);
        RETURN;
    END

    DECLARE @MaNCC VARCHAR(10);
    DECLARE @MaxID INT;

    -- sinh mã NCC
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaNCC, 4, 7) AS INT)), 0)
    FROM NhaCC;

    SET @MaNCC = 'NCC' + RIGHT('0000000' + CAST(@MaxID + 1 AS VARCHAR(7)), 7);

    INSERT INTO NhaCC
    (
        MaNCC, TenNCC, DienThoaiNCC,
        EmailNCC, DiaChiNCC, MaXa
    )
    VALUES
    (
        @MaNCC, @TenNCC, @DienThoaiNCC,
        @EmailNCC, @DiaChiNCC, @MaXa
    );
END;
GO

-- =========================
-- Cập nhật nhà cung cấp
-- =========================
CREATE OR ALTER PROC NhaCC_Update
(
    @MaNCC VARCHAR(10),
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @DiaChiNCC NVARCHAR(255),
    @MaXa CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại.', 16, 1);
        RETURN;
    END

    -- kiểm tra trùng tên
    IF EXISTS (SELECT 1 FROM NhaCC WHERE TenNCC = @TenNCC AND MaNCC <> @MaNCC)
    BEGIN
        RAISERROR(N'Tên nhà cung cấp đã tồn tại.', 16, 1);
        RETURN;
    END

    -- kiểm tra trùng SDT
    IF EXISTS (SELECT 1 FROM NhaCC WHERE DienThoaiNCC = @DienThoaiNCC AND MaNCC <> @MaNCC)
    BEGIN
        RAISERROR(N'Số điện thoại đã được sử dụng.', 16, 1);
        RETURN;
    END

    -- kiểm tra trùng email
    IF EXISTS (SELECT 1 FROM NhaCC WHERE EmailNCC = @EmailNCC AND MaNCC <> @MaNCC)
    BEGIN
        RAISERROR(N'Email đã tồn tại.', 16, 1);
        RETURN;
    END

    UPDATE NhaCC
    SET TenNCC = @TenNCC,
        DienThoaiNCC = @DienThoaiNCC,
        EmailNCC = @EmailNCC,
        DiaChiNCC = @DiaChiNCC,
        MaXa = @MaXa
    WHERE MaNCC = @MaNCC;
END;
GO

-- =========================
-- Xóa nhà cung cấp
-- =========================
CREATE OR ALTER PROC NhaCC_Delete
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại.', 16, 1);
        RETURN;
    END

    DELETE FROM NhaCC WHERE MaNCC = @MaNCC;
END;
GO

-- =========================
-- Lấy tất cả nhà cung cấp
-- =========================
CREATE OR ALTER PROC NhaCC_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT N.*, X.TenXa, T.TenTinh
    FROM NhaCC N
    JOIN Xa X ON X.MaXa = N.MaXa
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    ORDER BY N.TenNCC;
END;
GO

-- =========================
-- Lấy nhà cung cấp theo mã
-- =========================
CREATE OR ALTER PROC NhaCC_GetByID
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT N.*, X.TenXa, T.TenTinh
    FROM NhaCC N
    JOIN Xa X ON X.MaXa = N.MaXa
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE N.MaNCC = @MaNCC;
END;
GO

-- =========================
-- Tìm kiếm nhà cung cấp
-- =========================
CREATE OR ALTER PROC NhaCC_Search
(
    @Search NVARCHAR(100) = NULL,
    @MaTinh CHAR(2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT N.*, X.TenXa, T.MaTinh, T.TenTinh
    FROM NhaCC N
    JOIN Xa X ON X.MaXa = N.MaXa
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (
            @Search IS NULL OR @Search = ''
            OR N.MaNCC LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE N'%' + @Search + N'%'
            OR N.EmailNCC LIKE '%' + @Search + '%'
        )
        AND (
            @MaTinh IS NULL
            OR T.MaTinh = @MaTinh
        );
END;
GO
