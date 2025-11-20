USE DB_QLBH;
GO
-- Insert
CREATE OR ALTER PROC TaiKhoan_Insert
(
    @TenUser VARCHAR(50),
    @MatKhau NVARCHAR(255),
    @VaiTro NVARCHAR(20),
    @MaKH VARCHAR(10) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TaiKhoan WHERE TenUser = @TenUser)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại!', 16, 1);
        RETURN;
    END;

    INSERT INTO TaiKhoan (TenUser, MatKhau, VaiTro, MaKH, TrangThai)
    VALUES (@TenUser, @MatKhau, @VaiTro, @MaKH, 1);
END;
GO

-- Update (đổi mật khẩu hoặc vai trò)
CREATE OR ALTER PROC TaiKhoan_Update
(
    @TenUser VARCHAR(50),
    @MatKhau NVARCHAR(255),
    @VaiTro NVARCHAR(20),
    @TrangThai BIT
)
AS
BEGIN
    UPDATE TaiKhoan
    SET MatKhau = @MatKhau,
        VaiTro = @VaiTro,
        TrangThai = @TrangThai
    WHERE TenUser = @TenUser;
END;
GO

-- Delete
CREATE OR ALTER PROC TaiKhoan_Delete
(
    @TenUser VARCHAR(50)
)
AS
BEGIN
    DELETE FROM TaiKhoan WHERE TenUser = @TenUser;
END;
GO

-- GetAll
CREATE OR ALTER PROC TaiKhoan_GetAll
AS
BEGIN
    SELECT * FROM TaiKhoan;
END;
GO

-- Get by username
CREATE OR ALTER PROC TaiKhoan_GetByID
(
    @TenUser VARCHAR(50)
)
AS
BEGIN
    SELECT * FROM TaiKhoan WHERE TenUser = @TenUser;
END;
GO

-- Login check
CREATE OR ALTER PROC TaiKhoan_Login
(
    @TenUser VARCHAR(50),
    @MatKhau NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TenUser, VaiTro, TrangThai
    FROM TaiKhoan
    WHERE TenUser = @TenUser AND MatKhau = @MatKhau AND TrangThai = 1;
END;
GO

-- Search
CREATE OR ALTER PROC TaiKhoan_Search
(
    @TenUser VARCHAR(50) = NULL,
    @VaiTro NVARCHAR(20) = NULL,
    @MaKH VARCHAR(10) = NULL,
    @TrangThai BIT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM TaiKhoan
    WHERE 
        (@TenUser IS NULL OR TenUser LIKE '%' + @TenUser + '%') AND
        (@VaiTro IS NULL OR VaiTro LIKE '%' + @VaiTro + '%') AND
        (@MaKH IS NULL OR MaKH LIKE '%' + @MaKH + '%') AND
        (@TrangThai IS NULL OR TrangThai = @TrangThai);
END;
GO
