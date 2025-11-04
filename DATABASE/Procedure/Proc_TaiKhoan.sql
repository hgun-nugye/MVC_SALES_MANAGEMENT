-- Insert
CREATE OR ALTER PROC sp_TaiKhoan_Insert
(
    @TenDN VARCHAR(50),
    @MatKhau NVARCHAR(255),
    @VaiTro NVARCHAR(20),
    @MaKH VARCHAR(10) = NULL,
    @MaNV VARCHAR(10) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDN = @TenDN)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại!', 16, 1);
        RETURN;
    END;

    INSERT INTO TaiKhoan (TenDN, MatKhau, VaiTro, MaKH, MaNV, TrangThai)
    VALUES (@TenDN, @MatKhau, @VaiTro, @MaKH, @MaNV, 1);
END;
GO

-- Update (đổi mật khẩu hoặc vai trò)
CREATE OR ALTER PROC sp_TaiKhoan_Update
(
    @TenDN VARCHAR(50),
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
    WHERE TenDN = @TenDN;
END;
GO

-- Delete
CREATE OR ALTER PROC sp_TaiKhoan_Delete
(
    @TenDN VARCHAR(50)
)
AS
BEGIN
    DELETE FROM TaiKhoan WHERE TenDN = @TenDN;
END;
GO

-- GetAll
CREATE OR ALTER PROC sp_TaiKhoan_GetAll
AS
BEGIN
    SELECT * FROM TaiKhoan;
END;
GO

-- Get by username
CREATE OR ALTER PROC sp_TaiKhoan_GetByID
(
    @TenDN VARCHAR(50)
)
AS
BEGIN
    SELECT * FROM TaiKhoan WHERE TenDN = @TenDN;
END;
GO

-- Login check
CREATE OR ALTER PROC sp_TaiKhoan_Login
(
    @TenDN VARCHAR(50),
    @MatKhau NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TenDN, VaiTro, TrangThai
    FROM TaiKhoan
    WHERE TenDN = @TenDN AND MatKhau = @MatKhau AND TrangThai = 1;
END;
GO
