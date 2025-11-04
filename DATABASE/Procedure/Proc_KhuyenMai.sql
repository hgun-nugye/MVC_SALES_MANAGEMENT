USE DB_QLBH;
GO

-- =============================================
-- ================ INSERT =====================
-- =============================================
CREATE OR ALTER PROC sp_KhuyenMai_Insert
(
    @TenKM NVARCHAR(100),
    @MoTa NVARCHAR(MAX) = NULL,
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255) = NULL,
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra trùng tên khuyến mãi
        IF EXISTS (SELECT 1 FROM KhuyenMai WHERE TenKM = @TenKM)
        BEGIN
            RAISERROR(N'Tên khuyến mãi đã tồn tại!', 16, 1);
            RETURN;
        END

        DECLARE @MaKM VARCHAR(10);
        DECLARE @Count INT;

        SELECT @Count = COUNT(*) + 1 FROM KhuyenMai;
        SET @MaKM = 'KM' + RIGHT('00000000' + CAST(@Count AS VARCHAR(8)), 8);

        INSERT INTO KhuyenMai(MaKM, TenKM, MoTa, GiaTriKM, NgayBatDau, NgayKetThuc, DieuKienApDung, TrangThai)
        VALUES (@MaKM, @TenKM, @MoTa, @GiaTriKM, @NgayBatDau, @NgayKetThuc, @DieuKienApDung, @TrangThai);

        PRINT N'Thêm khuyến mãi thành công!';
        SELECT @MaKM AS MaKhuyenMaiMoi;
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO


-- =============================================
-- ================ UPDATE =====================
-- =============================================
CREATE OR ALTER PROC sp_KhuyenMai_Update
(
    @MaKM VARCHAR(10),
    @TenKM NVARCHAR(100),
    @MoTa NVARCHAR(MAX),
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255),
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhuyenMai WHERE MaKM = @MaKM)
        BEGIN
            RAISERROR(N'Mã khuyến mãi không tồn tại!', 16, 1);
            RETURN;
        END

        UPDATE KhuyenMai
        SET TenKM = @TenKM,
            MoTa = @MoTa,
            GiaTriKM = @GiaTriKM,
            NgayBatDau = @NgayBatDau,
            NgayKetThuc = @NgayKetThuc,
            DieuKienApDung = @DieuKienApDung,
            TrangThai = @TrangThai
        WHERE MaKM = @MaKM;

        PRINT N'Cập nhật khuyến mãi thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO


-- =============================================
-- ================ DELETE =====================
-- =============================================
CREATE OR ALTER PROC sp_KhuyenMai_Delete
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhuyenMai WHERE MaKM = @MaKM)
        BEGIN
            RAISERROR(N'Mã khuyến mãi không tồn tại!', 16, 1);
            RETURN;
        END

        DELETE FROM KhuyenMai WHERE MaKM = @MaKM;

        PRINT N'Xóa khuyến mãi thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO


-- =============================================
-- ================ GET ALL ====================
-- =============================================
CREATE OR ALTER PROC sp_KhuyenMai_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaKM, TenKM, MoTa, GiaTriKM, NgayBatDau, NgayKetThuc, 
           DieuKienApDung, TrangThai
    FROM KhuyenMai
    ORDER BY NgayBatDau DESC;
END;
GO


-- =============================================
-- ================ GET BY ID ==================
-- =============================================
CREATE OR ALTER PROC sp_KhuyenMai_GetByID
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaKM, TenKM, MoTa, GiaTriKM, NgayBatDau, NgayKetThuc, 
           DieuKienApDung, TrangThai
    FROM KhuyenMai
    WHERE MaKM = @MaKM;
END;
GO
