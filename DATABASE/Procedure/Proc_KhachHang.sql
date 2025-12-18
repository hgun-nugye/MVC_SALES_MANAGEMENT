USE DB_QLBH;
GO

CREATE OR ALTER PROC KhachHang_Insert
(
    @TenKH NVARCHAR(100),
    @AnhKH NVARCHAR(255),
    @GioiTinh BIT,
    @DienThoaiKH VARCHAR(15),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255),
    @MaXa SMALLINT,
    @TenDNKH VARCHAR(50),
    @MatKhauKH VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE DienThoaiKH = @DienThoaiKH)
    BEGIN
        RAISERROR(N'Số điện thoại đã tồn tại.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE EmailKH = @EmailKH)
    BEGIN
        RAISERROR(N'Email đã tồn tại.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE TenDNKH = @TenDNKH)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaKH VARCHAR(10);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, 8) AS INT)), 0)
    FROM KhachHang;

    SET @MaKH = 'KH' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    INSERT INTO KhachHang
    (
        MaKH, TenKH, AnhKH, GioiTinh,
        DienThoaiKH, EmailKH, DiaChiKH,
        MaXa, TenDNKH, MatKhauKH
    )
    VALUES
    (
        @MaKH, @TenKH, @AnhKH, @GioiTinh,
        @DienThoaiKH, @EmailKH, @DiaChiKH,
        @MaXa, @TenDNKH, @MatKhauKH
    );

    PRINT N'Thêm khách hàng thành công!';
END;
GO

CREATE OR ALTER PROC KhachHang_Update
(
    @MaKH VARCHAR(10),
    @TenKH NVARCHAR(100),
    @AnhKH NVARCHAR(255),
    @GioiTinh BIT,
    @DienThoaiKH VARCHAR(15),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255),
    @MaXa SMALLINT,
    @TenDNKH VARCHAR(50),
    @MatKhauKH VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Không tìm thấy khách hàng.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE DienThoaiKH = @DienThoaiKH AND MaKH <> @MaKH)
    BEGIN
        RAISERROR(N'Số điện thoại đã được sử dụng.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE EmailKH = @EmailKH AND MaKH <> @MaKH)
    BEGIN
        RAISERROR(N'Email đã được sử dụng.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE TenDNKH = @TenDNKH AND MaKH <> @MaKH)
    BEGIN
        RAISERROR(N'Tên đăng nhập đã được sử dụng.', 16, 1);
        RETURN;
    END;

    UPDATE KhachHang
    SET
        TenKH = @TenKH,
        AnhKH = @AnhKH,
        GioiTinh = @GioiTinh,
        DienThoaiKH = @DienThoaiKH,
        EmailKH = @EmailKH,
        DiaChiKH = @DiaChiKH,
        MaXa = @MaXa,
        TenDNKH = @TenDNKH,
        MatKhauKH = @MatKhauKH
    WHERE MaKH = @MaKH;

    PRINT N'Cập nhật khách hàng thành công!';
END;
GO

CREATE OR ALTER PROC KhachHang_Delete
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Không tìm thấy khách hàng cần xóa.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        DELETE FROM KhachHang WHERE MaKH = @MaKH;
        PRINT N'Xóa khách hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Error NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi xóa khách hàng: %s', 16, 1, @Error);
    END CATCH
END;
GO

CREATE OR ALTER PROC KhachHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM KhachHang ORDER BY MaKH;
END;
GO

CREATE OR ALTER PROC KhachHang_GetByID
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SELECT * FROM KhachHang WHERE MaKH = @MaKH;
END;
GO

CREATE OR ALTER PROC KhachHang_GetAllWithXa
AS
BEGIN
    SELECT 
        KH.MaKH,
        KH.TenKH,
        KH.GioiTinh,
        KH.DienThoaiKH,
        KH.EmailKH,
        KH.DiaChiKH,
		KH.AnhKH,
        X.TenXa,
        T.TenTinh
    FROM KhachHang KH
    LEFT JOIN Xa X ON KH.MaXa = X.MaXa
    LEFT JOIN Tinh T ON X.MaTinh = T.MaTinh
    ORDER BY KH.MaKH;
END;
GO

CREATE OR ALTER PROC KhachHang_GetByIDWithXa
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SELECT 
        KH.*,
        X.TenXa,
        T.TenTinh
    FROM KhachHang KH
    LEFT JOIN Xa X ON KH.MaXa = X.MaXa
    LEFT JOIN Tinh T ON X.MaTinh = T.MaTinh
    WHERE KH.MaKH = @MaKH;
END;
GO

-- Search
CREATE OR ALTER PROC KhachHang_Search
(
    @Search NVARCHAR(200) = NULL,
    @MaTinh SMALLINT = NULL,
    @GioiTinh BIT = NULL
)
AS
BEGIN
    SELECT 
        KH.*,
        X.TenXa,
        T.TenTinh
    FROM KhachHang KH
    JOIN Xa X ON KH.MaXa = X.MaXa
    JOIN Tinh T ON X.MaTinh = T.MaTinh
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            KH.TenKH LIKE '%' + @Search + '%' OR
            KH.EmailKH LIKE '%' + @Search + '%' OR
            KH.DienThoaiKH LIKE '%' + @Search + '%'
        )
        AND (@MaTinh IS NULL OR T.MaTinh = @MaTinh)
        AND (@GioiTinh IS NULL OR KH.GioiTinh = @GioiTinh);
END;
GO

