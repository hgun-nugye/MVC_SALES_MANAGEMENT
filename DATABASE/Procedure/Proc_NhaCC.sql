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
    @DiaChiNCC NVARCHAR(255),
    @MaXa SMALLINT
	
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

        -- Sinh mã NCC tự động dựa trên số lớn nhất
        DECLARE @MaNCC VARCHAR(10);
        DECLARE @MaxID INT;

        SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaNCC, 4, LEN(MaNCC)-3) AS INT)), 0)
        FROM NhaCC;

        SET @MaNCC = 'NCC' + RIGHT('0000000' + CAST(@MaxID + 1 AS VARCHAR(7)), 7);

        -- Thêm dữ liệu
        INSERT INTO NhaCC(MaNCC, TenNCC, DienThoaiNCC, EmailNCC, DiaChiNCC, MaXa)
        VALUES (@MaNCC, @TenNCC, @DienThoaiNCC, @EmailNCC, @DiaChiNCC, @MaXa);

        PRINT N'Thêm nhà cung cấp thành công.';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi thêm nhà cung cấp: %s', 16, 1, @Err);
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
    @DiaChiNCC NVARCHAR(255),
    @MaXa SMALLINT

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
            DiaChiNCC = @DiaChiNCC,
			MaXa = @MaXa
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
    SELECT N.*,X.TenXa, T.TenTinh
    FROM NhaCC N
	JOIN Xa X
	ON X.MaXa= N.MaXa
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
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
   SELECT N.*,X.TenXa, T.TenTinh
    FROM NhaCC N
	JOIN Xa X
	ON X.MaXa= N.MaXa
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
    WHERE MaNCC = @MaNCC;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE NhaCC_Search
    @Search NVARCHAR(100) = NULL,   
	@MaTinh SMALLINT = NULL
AS
BEGIN
    
    SELECT N.*,X.TenXa, T.TenTinh
    FROM NhaCC N
	JOIN Xa X
	ON X.MaXa= N.MaXa
	JOIN Tinh T
	ON T.MaTinh = X.MaTinh
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR N.MaNCC LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE '%' + @Search + '%'
            OR N.EmailNCC LIKE '%' + @Search + '%')
        AND (
            @MaTinh IS NULL 
            OR T.MaTinh = @MaTinh
        )
END;
GO
