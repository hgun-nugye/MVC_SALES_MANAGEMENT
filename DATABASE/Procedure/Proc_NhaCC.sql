USE DB_QLBH;
GO

-- ==========================================
-- INSERT: Thêm nhà cung cấp
-- ==========================================
CREATE OR ALTER PROC NhaCC_Insert
(
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @DiaChiNCC NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Kiểm tra trùng tên hoặc email hoặc số điện thoại
        IF EXISTS (SELECT 1 FROM NhaCC WHERE TenNCC = @TenNCC)
        BEGIN
            RAISERROR(N'Tên nhà cung cấp đã tồn tại.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhaCC WHERE DienThoaiNCC = @DienThoaiNCC)
        BEGIN
            RAISERROR(N'Số điện thoại đã được sử dụng.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhaCC WHERE EmailNCC = @EmailNCC)
        BEGIN
            RAISERROR(N'Email đã tồn tại.', 16, 1);
            RETURN;
        END

        -- Sinh mã NCC tự động
        DECLARE @MaNCC VARCHAR(10);
        SELECT @MaNCC = 'NCC' + RIGHT('0000000' + CAST(ISNULL(COUNT(*) + 1, 1) AS VARCHAR(7)),7)
        FROM NhaCC;

        -- Thêm dữ liệu
        INSERT INTO NhaCC(MaNCC, TenNCC, DienThoaiNCC, EmailNCC, DiaChiNCC)
        VALUES (@MaNCC, @TenNCC, @DienThoaiNCC, @EmailNCC, @DiaChiNCC);

        PRINT N'Thêm nhà cung cấp thành công.';
    END TRY
    BEGIN CATCH
        RAISERROR(N'Lỗi khi thêm nhà cung cấp: %s', 16, 1);
    END CATCH
END;
GO

-- ==========================================
-- UPDATE: Cập nhật nhà cung cấp
-- ==========================================
CREATE OR ALTER PROC NhaCC_Update
(
    @MaNCC VARCHAR(10),
    @TenNCC NVARCHAR(100),
    @DienThoaiNCC VARCHAR(15),
    @EmailNCC VARCHAR(100),
    @DiaChiNCC NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Kiểm tra mã NCC có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
        BEGIN
            RAISERROR(N'Mã nhà cung cấp không tồn tại.', 16, 1);
            RETURN;
        END

        -- Kiểm tra trùng tên / email / SDT với bản ghi khác
        IF EXISTS (SELECT 1 FROM NhaCC WHERE TenNCC = @TenNCC AND MaNCC <> @MaNCC)
        BEGIN
            RAISERROR(N'Tên nhà cung cấp đã tồn tại.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhaCC WHERE DienThoaiNCC = @DienThoaiNCC AND MaNCC <> @MaNCC)
        BEGIN
            RAISERROR(N'Số điện thoại đã được sử dụng.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhaCC WHERE EmailNCC = @EmailNCC AND MaNCC <> @MaNCC)
        BEGIN
            RAISERROR(N'Email đã tồn tại.', 16, 1);
            RETURN;
        END

        -- Cập nhật dữ liệu
        UPDATE NhaCC
        SET TenNCC = @TenNCC,
            DienThoaiNCC = @DienThoaiNCC,
            EmailNCC = @EmailNCC,
            DiaChiNCC = @DiaChiNCC
        WHERE MaNCC = @MaNCC;

        PRINT N'Cập nhật nhà cung cấp thành công.';
    END TRY
    BEGIN CATCH
        RAISERROR(N'Lỗi khi cập nhật nhà cung cấp: %s', 16, 1);
    END CATCH
END;
GO

-- ==========================================
-- DELETE: Xóa nhà cung cấp
-- ==========================================
CREATE OR ALTER PROC NhaCC_Delete
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
        BEGIN
            RAISERROR(N'Mã nhà cung cấp không tồn tại.', 16, 1);
            RETURN;
        END

        DELETE FROM NhaCC WHERE MaNCC = @MaNCC;
        PRINT N'Xóa nhà cung cấp thành công.';
    END TRY
    BEGIN CATCH
        RAISERROR(N'Lỗi khi xóa nhà cung cấp: %s', 16, 1);
    END CATCH
END;
GO

-- ==========================================
-- GET ALL: Lấy danh sách nhà cung cấp
-- ==========================================
CREATE OR ALTER PROC NhaCC_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNCC, TenNCC, DienThoaiNCC, EmailNCC, DiaChiNCC
    FROM NhaCC
    ORDER BY TenNCC;
END;
GO

-- ==========================================
-- GET BY ID: Lấy nhà cung cấp theo mã
-- ==========================================
CREATE OR ALTER PROC NhaCC_GetByID
(
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNCC, TenNCC, DienThoaiNCC, EmailNCC, DiaChiNCC
    FROM NhaCC
    WHERE MaNCC = @MaNCC;
END;
GO

-- Search & Filter
CREATE OR ALTER PROCEDURE NhaCC_SearchFilter
    @Search NVARCHAR(100) = NULL,   
	@TinhFilter NVARCHAR(100) = NULL,    -- filter theo Tỉnh
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartRow INT = (@PageNumber - 1) * @PageSize;

    SELECT *
    FROM NhaCC N
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR N.MaNCC LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE '%' + @Search + '%'
            OR N.EmailNCC LIKE '%' + @Search + '%')
        AND (
            @TinhFilter IS NULL 
            OR N.DiaChiNCC LIKE '%' + @TinhFilter + '%'
        )
END;
GO

-- count
CREATE OR ALTER PROC NhaCC_Count
    @Search NVARCHAR(100) = NULL,   
	@TinhFilter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM NhaCC N
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR N.MaNCC LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE '%' + @Search + '%'
            OR N.EmailNCC LIKE '%' + @Search + '%')
        AND (
            @TinhFilter IS NULL 
            OR N.DiaChiNCC LIKE '%' + @TinhFilter + '%'
        )
END;
GO