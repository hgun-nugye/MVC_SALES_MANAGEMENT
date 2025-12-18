USE DB_QLBH;
GO

CREATE OR ALTER PROC TrangThaiBH_Insert
(
    @MaTTBH CHAR(3),
    @TenTTBH NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra trùng mã
        IF EXISTS (SELECT 1 FROM TrangThaiBH WHERE MaTTBH = @MaTTBH)
        BEGIN
            RAISERROR(N'Mã trạng thái này đã tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        INSERT INTO TrangThaiBH (MaTTBH, TenTTBH)
        VALUES (@MaTTBH, @TenTTBH);

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiBH_Update
(
    @MaTTBH CHAR(3),
    @TenTTBH NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM TrangThaiBH WHERE MaTTBH = @MaTTBH)
        BEGIN
            RAISERROR(N'Mã trạng thái không tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE TrangThaiBH
        SET TenTTBH = @TenTTBH
        WHERE MaTTBH = @MaTTBH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiBH_Delete
(
    @MaTTBH CHAR(3)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM TrangThaiBH WHERE MaTTBH = @MaTTBH)
        BEGIN
            RAISERROR(N'Mã trạng thái không tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- Kiểm tra xem trạng thái này có đang được dùng trong đơn hàng nào không
        IF EXISTS (SELECT 1 FROM DonBanHang WHERE MaTTBH = @MaTTBH)
        BEGIN
            RAISERROR(N'Không thể xóa trạng thái này vì đang có đơn hàng sử dụng!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        DELETE FROM TrangThaiBH WHERE MaTTBH = @MaTTBH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiBH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTBH, TenTTBH
    FROM TrangThaiBH
    ORDER BY TenTTBH; 
END;
GO

CREATE OR ALTER PROC TrangThaiBH_GetByID
(
    @MaTTBH CHAR(3)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTBH, TenTTBH
    FROM TrangThaiBH
    WHERE MaTTBH = @MaTTBH;
END;
GO

CREATE OR ALTER PROC TrangThaiBH_Search
(
    @Search NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTBH, TenTTBH
    FROM TrangThaiBH
    WHERE (@Search IS NULL OR @Search = '' 
           OR MaTTBH LIKE '%' + @Search + '%' 
           OR TenTTBH LIKE N'%' + @Search + '%')
END;
GO