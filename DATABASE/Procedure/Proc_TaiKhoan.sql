USE DB_QLBH;
GO
-- Insert
CREATE OR ALTER PROC TaiKhoan_Insert
(
    @TenUser VARCHAR(50),
    @MatKhau NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TaiKhoan WHERE TenUser = @TenUser)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại!', 16, 1);
        RETURN;
    END;

    INSERT INTO TaiKhoan (TenUser, MatKhau)
    VALUES (@TenUser, @MatKhau);
END;
GO

-- Update (đổi mật khẩu hoặc vai trò)
CREATE OR ALTER PROC TaiKhoan_Update
(
    @TenUser VARCHAR(50),
    @MatKhau NVARCHAR(255)
)
AS
BEGIN
    UPDATE TaiKhoan
    SET MatKhau = @MatKhau
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

    SELECT TenUser
    FROM TaiKhoan
    WHERE TenUser = @TenUser AND MatKhau = @MatKhau
END;
GO
