USE DB_QLBH;
GO
CREATE OR ALTER PROC NhanVien_Insert
(
	@TenNV NVARCHAR(100),
	@VaiTro NVARCHAR(50),
	@TenDNNV VARCHAR(50),
	@MatKhauNV VARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDNNV = @TenDNNV)
	BEGIN
		RAISERROR(N'Tên đăng nhập đã tồn tại.', 16, 1);
		RETURN;
	END;

	DECLARE @MaNV VARCHAR(10);
	DECLARE @MaxID INT;

	SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaNV, 3, LEN(MaNV)-2) AS INT)), 0)
	FROM NhanVien;

	SET @MaNV = 'NV' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

	INSERT INTO NhanVien(MaNV, TenNV, VaiTro, TenDNNV, MatKhauNV)
	VALUES (@MaNV, @TenNV, @VaiTro, @TenDNNV, @MatKhauNV);

	PRINT N'Thêm nhân viên thành công!';
END;
GO

CREATE OR ALTER PROC NhanVien_Update
(
	@MaNV VARCHAR(10),
	@TenNV NVARCHAR(100),
	@VaiTro NVARCHAR(50),
	@TenDNNV VARCHAR(50),
	@MatKhauNV VARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
	BEGIN
		RAISERROR(N'Không tìm thấy nhân viên.', 16, 1);
		RETURN;
	END;

	IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDNNV = @TenDNNV AND MaNV <> @MaNV)
	BEGIN
		RAISERROR(N'Tên đăng nhập đã được sử dụng.', 16, 1);
		RETURN;
	END;

	UPDATE NhanVien
	SET TenNV = @TenNV,
		VaiTro = @VaiTro,
		TenDNNV = @TenDNNV,
		MatKhauNV = @MatKhauNV
	WHERE MaNV = @MaNV;

	PRINT N'Cập nhật nhân viên thành công!';
END;
GO

CREATE OR ALTER PROC NhanVien_Delete
(
	@MaNV VARCHAR(10)
)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
	BEGIN
		RAISERROR(N'Không tìm thấy nhân viên.', 16, 1);
		RETURN;
	END;

	DELETE FROM NhanVien WHERE MaNV = @MaNV;

	PRINT N'Xóa nhân viên thành công!';
END;
GO

CREATE OR ALTER PROC NhanVien_GetAll
AS
BEGIN
	SELECT * FROM NhanVien ORDER BY MaNV;
END;
GO

CREATE OR ALTER PROC NhanVien_GetByID
(
	@MaNV VARCHAR(10)
)
AS
BEGIN
	SELECT * FROM NhanVien WHERE MaNV = @MaNV;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE NhanVien_Search
    @Search NVARCHAR(200) = NULL
AS
BEGIN    
    SELECT n.*
	FROM NhanVien n
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            n.TenNV LIKE '%' + @Search + '%' OR
            n.TenDNNV LIKE '%' + @Search + '%' OR
            n.VaiTro LIKE '%' + @Search + '%' 
        )
END;
GO