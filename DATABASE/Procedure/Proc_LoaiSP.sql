USE DB_QLBH;
GO

-- ============================
-- INSERT
-- ============================
CREATE OR ALTER PROC sp_LoaiSP_Insert
(
    @TenLSP NVARCHAR(50),
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng tên loại trong cùng nhóm
    IF EXISTS (SELECT 1 FROM LoaiSP WHERE TenLoai = @TenLSP AND MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Tên loại sản phẩm đã tồn tại trong nhóm này.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra nhóm tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhomSP WHERE MaNhom = @MaNhom)
    BEGIN
        RAISERROR(N'Mã nhóm sản phẩm không tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaLoai VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM LoaiSP;
    SET @MaLoai = 'LSP' + RIGHT('0000000' + CAST(@Count AS VARCHAR(7)), 7);

    BEGIN TRY
        INSERT INTO LoaiSP(MaLoai, TenLoai, MaNhom)
        VALUES (@MaLoai, @TenLSP, @MaNhom);

        PRINT N'Thêm loại sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi thêm loại sản phẩm: %s', 16, 1, @Err);
    END CATCH
END;
GO


-- ============================
-- UPDATE
-- ============================
CREATE OR ALTER PROC sp_LoaiSP_Update
(
    @MaLoai VARCHAR(10),
    @TenLSP NVARCHAR(50),
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
    IF EXISTS (SELECT 1 FROM LoaiSP WHERE TenLoai = @TenLSP AND MaNhom = @MaNhom AND MaLoai <> @MaLoai)
    BEGIN
        RAISERROR(N'Tên loại sản phẩm đã tồn tại trong nhóm này.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE LoaiSP
        SET TenLoai = @TenLSP,
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
CREATE OR ALTER PROC sp_LoaiSP_Delete
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
CREATE OR ALTER PROC sp_LoaiSP_GetAll
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
CREATE OR ALTER PROC sp_LoaiSP_GetByID
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
