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
    DECLARE @MaxID INT;

    -- Lấy số lớn nhất hiện có
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, LEN(MaKH)-2) AS INT)), 0)
    FROM KhachHang;

    -- Tăng lên 1
    SET @MaKH = 'KH' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

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
    @Search NVARCHAR(200) = NULL       -- Từ khóa tìm kiếm (Tên, Email, SĐT, Địa chỉ)
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
        @Search IS NULL
        OR TenKH LIKE '%' + @Search + '%'
        OR EmailKH LIKE '%' + @Search + '%'
        OR DienThoaiKH LIKE '%' + @Search + '%'
        OR DiaChiKH LIKE '%' + @Search + '%'
    ORDER BY TenKH ASC;
END;
GO

-- Kiểm tra tồn tại
CREATE OR ALTER PROC KhachHang_Exists
(
    @MaKH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS ExistsCount
    FROM KhachHang
    WHERE MaKH = @MaKH;
END;
GO

-- Search & Filter
CREATE OR ALTER PROCEDURE KhachHang_SearchFilter
    @Search NVARCHAR(200) = NULL,        
    @TinhFilter NVARCHAR(100) = NULL,    -- filter theo Tỉnh
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartRow INT = (@PageNumber - 1) * @PageSize;

    SELECT 
        KH.*
    FROM KhachHang KH
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            KH.TenKH LIKE '%' + @Search + '%' OR
            KH.EmailKH LIKE '%' + @Search + '%' OR
            KH.DienThoaiKH LIKE '%' + @Search + '%'
        )
        AND (
            @TinhFilter IS NULL 
            OR KH.DiaChiKH LIKE '%' + @TinhFilter + '%'
        )
       
    ORDER BY KH.MaKH ASC
    OFFSET @StartRow ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO


-- count
CREATE OR ALTER PROC KhachHang_Count
    @Search NVARCHAR(200) = NULL,
    @TinhFilter NVARCHAR(100) = NULL    -- filter theo Tỉnh
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM KhachHang KH
     WHERE
        (
            @Search IS NULL OR @Search = '' OR
            KH.TenKH LIKE '%' + @Search + '%' OR
            KH.EmailKH LIKE '%' + @Search + '%' OR
            KH.DienThoaiKH LIKE '%' + @Search + '%'
        )
        AND (
            @TinhFilter IS NULL 
            OR KH.DiaChiKH LIKE '%' + @TinhFilter + '%'
        )
END;
GO
