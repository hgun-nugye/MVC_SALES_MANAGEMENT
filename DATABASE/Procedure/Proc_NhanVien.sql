USE DB_QLBH;
GO

-- =========================
-- Thêm nhân viên
-- =========================
CREATE OR ALTER PROC NhanVien_Insert
(
    @CCCD VARCHAR(12),
    @TenNV NVARCHAR(100),
    @GioiTinh BIT,
    @NgaySinh DATE,
    @SDT VARCHAR(10),
    @Email VARCHAR(50),
    @DiaChiNV NVARCHAR(255),
    @MaXa CHAR(5),
    @AnhNV NVARCHAR(255),
    @TenDNNV VARCHAR(50),
    @MatKhauNV VARCHAR(255),
    @MaVT CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên đăng nhập
    IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDNNV = @TenDNNV)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại!', 16, 1);
        RETURN;
    END;

    -- kiểm tra trùng CCCD
    IF EXISTS (SELECT 1 FROM NhanVien WHERE CCCD = @CCCD)
    BEGIN
        RAISERROR(N'Nhân viên đã tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @MaNV VARCHAR(10);
    DECLARE @MaxID INT;

    -- sinh mã nhân viên
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaNV, 3, 8) AS INT)), 0)
    FROM NhanVien;

    SET @MaNV = 'NV' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    BEGIN TRAN;

    INSERT INTO NhanVien
    (
        MaNV, CCCD, TenNV, GioiTinh, NgaySinh,
        SDT, Email, DiaChiNV, MaXa,
        TenDNNV, MatKhauNV, AnhNV
    )
    VALUES
    (
        @MaNV, @CCCD, @TenNV, @GioiTinh, @NgaySinh,
        @SDT, @Email, @DiaChiNV, @MaXa,
        @TenDNNV, @MatKhauNV, @AnhNV
    );

    INSERT INTO PhanQuyen(MaVT, MaNV)
    VALUES (@MaVT, @MaNV);

    COMMIT;
END;
GO

-- =========================
-- Cập nhật nhân viên
-- =========================
CREATE OR ALTER PROC NhanVien_Update
(
    @MaNV VARCHAR(10),
    @CCCD VARCHAR(12),
    @TenNV NVARCHAR(100),
    @GioiTinh BIT,
    @NgaySinh DATE,
    @SDT VARCHAR(10),
    @Email VARCHAR(50),
    @DiaChiNV NVARCHAR(255),
    @MaXa CHAR(5),
    @AnhNV NVARCHAR(255),
    @TenDNNV VARCHAR(50),
    @MatKhauNV VARCHAR(255),
    @MaVT CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- kiểm tra trùng tên đăng nhập
    IF EXISTS (
        SELECT 1 FROM NhanVien
        WHERE TenDNNV = @TenDNNV AND MaNV <> @MaNV
    )
    BEGIN
        RAISERROR(N'Tên đăng nhập đã được sử dụng!', 16, 1);
        RETURN;
    END;

    -- kiểm tra trùng CCCD
    IF EXISTS (
        SELECT 1 FROM NhanVien
        WHERE CCCD = @CCCD AND MaNV <> @MaNV
    )
    BEGIN
        RAISERROR(N'Nhân viên đã tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRAN;

    UPDATE NhanVien
    SET
        CCCD = @CCCD,
        TenNV = @TenNV,
        GioiTinh = @GioiTinh,
        NgaySinh = @NgaySinh,
        SDT = @SDT,
        Email = @Email,
        DiaChiNV = @DiaChiNV,
        MaXa = @MaXa,
        TenDNNV = @TenDNNV,
        MatKhauNV = @MatKhauNV,
        AnhNV = @AnhNV
    WHERE MaNV = @MaNV;

    DELETE FROM PhanQuyen WHERE MaNV = @MaNV;

    INSERT INTO PhanQuyen(MaVT, MaNV)
    VALUES (@MaVT, @MaNV);

    COMMIT;
END;
GO

-- =========================
-- Xóa nhân viên
-- =========================
CREATE OR ALTER PROC NhanVien_Delete
(
    @MaNV VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
    BEGIN
        RAISERROR(N'Không tìm thấy nhân viên!', 16, 1);
        RETURN;
    END;

    BEGIN TRAN;
    DELETE FROM PhanQuyen WHERE MaNV = @MaNV;
    DELETE FROM NhanVien WHERE MaNV = @MaNV;
    COMMIT;
END;
GO

-- =========================
-- Lấy tất cả nhân viên
-- =========================
CREATE OR ALTER PROC NhanVien_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        nv.*,
        vt.TenVT,
        x.TenXa,
        t.TenTinh
    FROM NhanVien nv
    JOIN PhanQuyen pq ON pq.MaNV = nv.MaNV
    JOIN VaiTro vt ON vt.MaVT = pq.MaVT
    JOIN Xa x ON x.MaXa = nv.MaXa
    JOIN Tinh t ON t.MaTinh = x.MaTinh
    ORDER BY nv.MaNV;
END;
GO

-- =========================
-- Lấy nhân viên theo mã
-- =========================
CREATE OR ALTER PROC NhanVien_GetByID
(
    @MaNV VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        nv.*,
        vt.MaVT,
        vt.TenVT,
        x.TenXa,
        t.TenTinh
    FROM NhanVien nv
    JOIN PhanQuyen pq ON pq.MaNV = nv.MaNV
    JOIN VaiTro vt ON vt.MaVT = pq.MaVT
    JOIN Xa x ON x.MaXa = nv.MaXa
    JOIN Tinh t ON t.MaTinh = x.MaTinh
    WHERE nv.MaNV = @MaNV;
END;
GO

-- =========================
-- Tìm kiếm nhân viên
-- =========================
CREATE OR ALTER PROC NhanVien_Search
(
    @Search NVARCHAR(200) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        nv.*,
        vt.TenVT,
        x.TenXa,
        t.TenTinh
    FROM NhanVien nv
    JOIN PhanQuyen pq ON pq.MaNV = nv.MaNV
    JOIN VaiTro vt ON vt.MaVT = pq.MaVT
    JOIN Xa x ON x.MaXa = nv.MaXa
    JOIN Tinh t ON t.MaTinh = x.MaTinh
    WHERE
        @Search IS NULL OR @Search = '' OR
        nv.TenNV LIKE N'%' + @Search + N'%' OR
        nv.TenDNNV LIKE '%' + @Search + '%' OR
        nv.SDT LIKE '%' + @Search + '%' OR
        vt.TenVT LIKE N'%' + @Search + N'%';
END;
GO
