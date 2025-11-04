USE DB_QLBH;
GO

-- ============================
-- INSERT
-- ============================
CREATE OR ALTER PROC sp_NhomSP_Insert
(
    @TenNhom NVARCHAR(100),
    @MoTa NVARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng tên nhóm
    IF EXISTS (SELECT 1 FROM NhomSP WHERE TenNhom = @TenNhom)
    BEGIN
        RAISERROR(N'Tên nhóm sản phẩm đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaNhom VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM NhomSP;
    SET @MaNhom = 'NSP' + RIGHT('0000000' + CAST(@Count AS VARCHAR(7)), 7);

    BEGIN TRY
        INSERT INTO NhomSP (MaNhom, TenNhom, MoTa)
        VALUES (@MaNhom, @TenNhom, @MoTa);

        PRINT N'Thêm nhóm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi thêm nhóm sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO


-- ============================
-- UPDATE
-- ============================
CREATE OR ALTER PROC sp_NhomSP_Update
(
    @MaNhom VARCHAR(10),
    @TenNhom NVARCHAR(100),
    @MoTa NVARCHAR(255)
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
        SET TenNhom = @TenNhom,
            MoTa = @MoTa
        WHERE MaNhom = @MaNhom;

        PRINT N'Cập nhật nhóm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi cập nhật nhóm sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO


-- ============================
-- DELETE
-- ============================
CREATE OR ALTER PROC sp_NhomSP_Delete
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


-- ============================
-- GET ALL
-- ============================
CREATE OR ALTER PROC sp_NhomSP_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM NhomSP ORDER BY MaNhom;
END;
GO


-- ============================
-- GET BY ID
-- ============================
CREATE OR ALTER PROC sp_NhomSP_GetByID
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
