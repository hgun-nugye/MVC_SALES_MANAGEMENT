USE DB_QLBH;
GO

CREATE OR ALTER PROC NhanVien_Insert
(
    @CCCD VARCHAR(12),
    @TenNV NVARCHAR(100),
    @GioiTinh BIT,
    @NgaySinh DATE,
    @SDT VARCHAR(10),
    @Email VARCHAR(50),
    @DiaChiNV NVARCHAR(255),
    @MaXa SMALLINT,

    @TenDNNV VARCHAR(50),
    @MatKhauNV VARCHAR(255),

    @MaVT CHAR(5)   -- VAI TRÒ
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check username
    IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDNNV = @TenDNNV)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại!', 16, 1);
        RETURN;
    END;

	 IF EXISTS (SELECT 1 FROM NhanVien WHERE CCCD = @CCCD)
    BEGIN
        RAISERROR(N'Nhân viên đã tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @MaNV VARCHAR(10);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaNV, 3, 8) AS INT)), 0)
    FROM NhanVien;

    SET @MaNV = 'NV' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    BEGIN TRAN;

    INSERT INTO NhanVien
    (
        MaNV, CCCD, TenNV, GioiTinh, NgaySinh,
        SDT, Email, DiaChiNV, MaXa,
        TenDNNV, MatKhauNV
    )
    VALUES
    (
        @MaNV, @CCCD, @TenNV, @GioiTinh, @NgaySinh,
        @SDT, @Email, @DiaChiNV, @MaXa,
        @TenDNNV, @MatKhauNV
    );

    INSERT INTO PhanQuyen(MaVT, MaNV)
    VALUES (@MaVT, @MaNV);

    COMMIT;

    PRINT N'Thêm nhân viên thành công!';
END;
GO

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
    @MaXa SMALLINT,

    @TenDNNV VARCHAR(50),
    @MatKhauNV VARCHAR(255),

    @MaVT CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;
	   
    IF EXISTS (
        SELECT 1 FROM NhanVien
        WHERE TenDNNV = @TenDNNV AND MaNV <> @MaNV			
    )
    BEGIN
        RAISERROR(N'Tên đăng nhập đã được sử dụng!', 16, 1);
        RETURN;
    END;

	IF EXISTS (
        SELECT 1 FROM NhanVien
        WHERE CCCD = @CCCD AND MaNV <> @MaNV
    )
    BEGIN
        RAISERROR(N'Nhân viên đã được sử dụng!', 16, 1);
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
        MatKhauNV = @MatKhauNV
    WHERE MaNV = @MaNV;

    -- Update vai trò
    DELETE FROM PhanQuyen WHERE MaNV = @MaNV;

    INSERT INTO PhanQuyen(MaVT, MaNV)
    VALUES (@MaVT, @MaNV);

    COMMIT;

    PRINT N'Cập nhật nhân viên thành công!';
END;
GO

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

    PRINT N'Xóa nhân viên thành công!';
END;
GO

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


-- Search
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
		X.TenXa, 
		T.TenTinh
    FROM NhanVien nv
    JOIN PhanQuyen pq ON pq.MaNV = nv.MaNV
    JOIN VaiTro vt ON vt.MaVT = pq.MaVT
	JOIN Xa X ON X.MaXa = nv.MaXa 
	JOIN Tinh T ON T.MaTinh = X. MaTinh
    WHERE
        @Search IS NULL OR @Search = '' OR
        nv.TenNV LIKE '%' + @Search + '%' OR
        nv.TenDNNV LIKE '%' + @Search + '%' OR
        nv.SDT LIKE '%' + @Search + '%' OR
        vt.TenVT LIKE '%' + @Search + '%';
END;
GO
