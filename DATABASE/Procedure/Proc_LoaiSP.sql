USE DB_QLBH;
GO

CREATE OR ALTER PROC LoaiSP_Insert
(
    @MaLoai VARCHAR(10),
    @TenLoai NVARCHAR(50),
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng
    IF EXISTS (SELECT 1 FROM LoaiSP WHERE TenLoai = @TenLoai AND MaNhom = @MaNhom OR MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Loại sản phẩm đã tồn tại!', 16, 1);
        RETURN;
    END;
	   
        INSERT INTO LoaiSP(MaLoai, TenLoai, MaNhom)
        VALUES (@MaLoai, @TenLoai, @MaNhom);

        PRINT N'Thêm loại sản phẩm thành công!';    
END;
GO

CREATE OR ALTER PROC LoaiSP_Update
(
    @MaLoai VARCHAR(10),
    @TenLoai NVARCHAR(50),
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra loại tồn tại
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Không tìm thấy loại sản phẩm cần cập nhật.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra trùng tên trong cùng nhóm
    IF EXISTS (SELECT 1 FROM LoaiSP WHERE TenLoai = @TenLoai AND MaNhom = @MaNhom AND MaLoai <> @MaLoai)
    BEGIN
        RAISERROR(N'Tên loại sản phẩm đã tồn tại trong nhóm này.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE LoaiSP
        SET TenLoai = @TenLoai,
            MaNhom = @MaNhom
        WHERE MaLoai = @MaLoai;

        PRINT N'Cập nhật loại sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi cập nhật loại sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO


-- ============================
-- DELETE
-- ============================
CREATE OR ALTER PROC LoaiSP_Delete
(
    @MaLoai VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Không tìm thấy loại sản phẩm cần xóa.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        DELETE FROM LoaiSP WHERE MaLoai = @MaLoai;
        PRINT N'Xóa loại sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi xóa loại sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO


-- ============================
-- GET ALL
-- ============================
CREATE OR ALTER PROC LoaiSP_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT L.*, N.TenNhom
    FROM LoaiSP L
    LEFT JOIN NhomSP N ON L.MaNhom = N.MaNhom
    ORDER BY L.MaLoai;
END;
GO


-- ============================
-- GET BY ID
-- ============================
CREATE OR ALTER PROC LoaiSP_GetByID
(
    @MaLoai VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Không tìm thấy loại sản phẩm với mã này.', 16, 1);
        RETURN;
    END;

    SELECT L.*, N.TenNhom
    FROM LoaiSP L
    LEFT JOIN NhomSP N ON L.MaNhom = N.MaNhom
    WHERE L.MaLoai = @MaLoai;
END;
GO

-- ============================
-- SEARCH
-- ============================
CREATE OR ALTER PROC LoaiSP_Search
(
     @Search NVARCHAR(100) = NULL,
    @MaNhom VARCHAR(10) = NULL
	)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        L.MaLoai,
        L.TenLoai,
        L.MaNhom,
        N.TenNhom
    FROM LoaiSP L
    JOIN NhomSP N ON L.MaNhom = N.MaNhom
    
      WHERE
        (@Search IS NULL OR @Search = '' 
            OR L.MaLoai LIKE '%' + @Search + '%'
            OR L.MaNhom LIKE '%' + @Search + '%'
            OR N.TenNhom LIKE '%' + @Search + '%'
            OR L.TenLoai LIKE '%' + @Search + '%')
		AND (@MaNhom IS NULL OR N.MaNhom=@MaNhom)
    ORDER BY L.MaLoai;
END;
GO

