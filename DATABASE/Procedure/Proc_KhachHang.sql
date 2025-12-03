USE DB_QLBH;
GO

-- ============================
-- INSERT
-- ============================
CREATE OR ALTER PROC KhachHang_Insert
(
    @TenKH NVARCHAR(50),
	@AnhKH NVARCHAR(255),
    @DienThoaiKH VARCHAR(10),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255),
    @MaXa SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng dữ liệu
    IF EXISTS (SELECT 1 FROM KhachHang WHERE DienThoaiKH = @DienThoaiKH)
    BEGIN
        RAISERROR(N'Số điện thoại khách hàng đã tồn tại.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE EmailKH = @EmailKH)
    BEGIN
        RAISERROR(N'Email khách hàng đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaKH VARCHAR(10);
    DECLARE @MaxID INT;

    -- Lấy số lớn nhất hiện có
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, LEN(MaKH)-2) AS INT)), 0)
    FROM KhachHang;

    -- Tăng lên 1
    SET @MaKH = 'KH' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    BEGIN TRY
        INSERT INTO KhachHang(MaKH, TenKH, DienThoaiKH, EmailKH, DiaChiKH,MaXa, AnhKH)
        VALUES (@MaKH, @TenKH, @DienThoaiKH, @EmailKH, @DiaChiKH,@MaXa, @AnhKH);

        PRINT N'Thêm khách hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Error NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi thêm khách hàng: %s', 16, 1, @Error);
    END CATCH
END;
GO

-- ============================
-- UPDATE
-- ============================
CREATE OR ALTER PROC KhachHang_Update
(
    @MaKH VARCHAR(10),
    @TenKH NVARCHAR(50),
	@AnhKH NVARCHAR(255),
    @DienThoaiKH VARCHAR(10),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255),
    @MaXa SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Không tìm thấy khách hàng cần cập nhật.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra trùng số điện thoại và email với khách hàng khác
    IF EXISTS (SELECT 1 FROM KhachHang WHERE DienThoaiKH = @DienThoaiKH AND MaKH <> @MaKH)
    BEGIN
        RAISERROR(N'Số điện thoại đã được sử dụng bởi khách hàng khác.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM KhachHang WHERE EmailKH = @EmailKH AND MaKH <> @MaKH)
    BEGIN
        RAISERROR(N'Email đã được sử dụng bởi khách hàng khác.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE KhachHang
        SET TenKH = @TenKH,
			AnhKH = @AnhKH,
            DienThoaiKH = @DienThoaiKH,
            EmailKH = @EmailKH,
            DiaChiKH = @DiaChiKH,
			MaXa = @MaXa
        WHERE MaKH = @MaKH;

        PRINT N'Cập nhật thông tin khách hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Error NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi cập nhật khách hàng: %s', 16, 1, @Error);
    END CATCH
END;
GO

-- ============================
-- DELETE
-- ============================
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


-- ============================
-- GET ALL
-- ============================
CREATE OR ALTER PROC KhachHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM KhachHang ORDER BY MaKH;
END;
GO


-- ============================
-- GET BY ID
-- ============================
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
    SELECT KH.MaKH, KH.TenKH, KH.DienThoaiKH, KH.EmailKH, KH.DiaChiKH, KH.AnhKH,
           KH.MaXa,
           X.TenXa,
           T.TenTinh
    FROM KhachHang KH
    LEFT JOIN Xa X ON KH.MaXa = X.MaXa
    LEFT JOIN Tinh T ON X.MaTinh = T.MaTinh
    ORDER BY KH.MaKH
END
GO

CREATE OR ALTER PROC KhachHang_GetbyIDWithXa
	    @MaKH VARCHAR(10)
AS
BEGIN
    SELECT KH.MaKH, KH.TenKH, KH.DienThoaiKH, KH.EmailKH, KH.DiaChiKH, KH.AnhKH,
           KH.MaXa,
           X.TenXa,
           T.TenTinh
    FROM KhachHang KH
    LEFT JOIN Xa X ON KH.MaXa = X.MaXa
    LEFT JOIN Tinh T ON X.MaTinh = T.MaTinh
	WHERE KH.MaKH = @MaKH
END
GO

-- Search
CREATE OR ALTER PROCEDURE KhachHang_Search
    @Search NVARCHAR(200) = NULL,        
    @MaTinh SMALLINT = NULL
AS
BEGIN    
    SELECT 
        K.*, X.TenXa, T.TenTinh
    FROM KhachHang K
	JOIN Xa X ON X.MaXa = K.MaXa
	JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            K.TenKH LIKE '%' + @Search + '%' OR
            K.EmailKH LIKE '%' + @Search + '%' OR
            K.DienThoaiKH LIKE '%' + @Search + '%'
        )
        AND (
            @MaTinh IS NULL 
            OR T.MaTinh LIKE '%' + @MaTinh + '%'
        )
END;
GO

