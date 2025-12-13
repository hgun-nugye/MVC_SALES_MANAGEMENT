USE DB_QLBH;
GO

CREATE OR ALTER PROC NhomSP_Insert
(
    @MaNhom VARCHAR(10),
    @TenNhom NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng 
    IF EXISTS (SELECT 1 FROM NhomSP WHERE TenNhom = @TenNhom OR MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Nhóm sản phẩm đã tồn tại.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO NhomSP (MaNhom, TenNhom)
        VALUES (@MaNhom, @TenNhom);

        PRINT N'Thêm nhóm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi thêm nhóm sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO

CREATE OR ALTER PROC NhomSP_Update
(
    @MaNhom VARCHAR(10),
    @TenNhom NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra nhóm tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhomSP WHERE MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Không tìm thấy nhóm sản phẩm cần cập nhật.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra trùng tên với nhóm khác
    IF EXISTS (SELECT 1 FROM NhomSP WHERE TenNhom = @TenNhom AND MaNhom <> @MaNhom)
    BEGIN
        RAISERROR(N'Tên nhóm sản phẩm đã tồn tại ở nhóm khác.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE NhomSP
        SET TenNhom = @TenNhom
        WHERE MaNhom = @MaNhom;

        PRINT N'Cập nhật nhóm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi cập nhật nhóm sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO

CREATE OR ALTER PROC NhomSP_Delete
(
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhomSP WHERE MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Không tìm thấy nhóm sản phẩm cần xóa.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        DELETE FROM NhomSP WHERE MaNhom = @MaNhom;
        PRINT N'Xóa nhóm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi xóa nhóm sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO

CREATE OR ALTER PROC NhomSP_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM NhomSP ORDER BY MaNhom;
END;
GO

CREATE OR ALTER PROC NhomSP_GetByID
(
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM NhomSP WHERE MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Không tìm thấy nhóm sản phẩm với mã này.', 16, 1);
        RETURN;
    END;

    SELECT * FROM NhomSP WHERE MaNhom = @MaNhom;
END;
GO

CREATE OR ALTER PROC NhomSP_Search
(
     @Search NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM NhomSP
    WHERE 
       (@Search IS NULL OR @Search = '' 
            OR MaNhom LIKE '%' + @Search + '%'
            OR TenNhom LIKE '%' + @Search + '%')
    ORDER BY MaNhom;
END;
GO

