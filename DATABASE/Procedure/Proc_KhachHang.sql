USE DB_QLBH;
GO

-- ============================
-- INSERT
-- ============================
CREATE OR ALTER PROC KhachHang_Insert
(
    @TenKH NVARCHAR(50),
    @DienThoaiKH VARCHAR(10),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255)
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
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM KhachHang;
    SET @MaKH = 'KH' + RIGHT('00000000' + CAST(@Count AS VARCHAR(8)), 8);

    BEGIN TRY
        INSERT INTO KhachHang(MaKH, TenKH, DienThoaiKH, EmailKH, DiaChiKH)
        VALUES (@MaKH, @TenKH, @DienThoaiKH, @EmailKH, @DiaChiKH);

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
    @DienThoaiKH VARCHAR(10),
    @EmailKH NVARCHAR(255),
    @DiaChiKH NVARCHAR(255)
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
            DienThoaiKH = @DienThoaiKH,
            EmailKH = @EmailKH,
            DiaChiKH = @DiaChiKH
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
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Không tìm thấy khách hàng với mã này.', 16, 1);
        RETURN;
    END;

    SELECT * FROM KhachHang WHERE MaKH = @MaKH;
END;
GO

-- ============================
-- SEARCH
-- ============================
CREATE OR ALTER PROC KhachHang_Search
(
    @Keyword NVARCHAR(200) = NULL       -- Từ khóa tìm kiếm (Tên, Email, SĐT, Địa chỉ)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MaKH,
        TenKH,
        DienThoaiKH,
        EmailKH,
        DiaChiKH
    FROM KhachHang
    WHERE
        @Keyword IS NULL
        OR TenKH LIKE '%' + @Keyword + '%'
        OR EmailKH LIKE '%' + @Keyword + '%'
        OR DienThoaiKH LIKE '%' + @Keyword + '%'
        OR DiaChiKH LIKE '%' + @Keyword + '%'
    ORDER BY TenKH ASC;
END;
GO

