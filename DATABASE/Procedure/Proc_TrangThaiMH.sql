USE DB_QLBH;
GO

CREATE OR ALTER PROC TrangThaiMH_Insert
(
    @MaTTMH CHAR(3),
    @TenTTMH NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra trùng mã
        IF EXISTS (SELECT 1 FROM TrangThaiMH WHERE MaTTMH = @MaTTMH)
        BEGIN
            RAISERROR(N'Mã trạng thái này đã tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        INSERT INTO TrangThaiMH (MaTTMH, TenTTMH)
        VALUES (@MaTTMH, @TenTTMH);

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiMH_Update
(
    @MaTTMH CHAR(3),
    @TenTTMH NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM TrangThaiMH WHERE MaTTMH = @MaTTMH)
        BEGIN
            RAISERROR(N'Mã trạng thái không tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE TrangThaiMH
        SET TenTTMH = @TenTTMH
        WHERE MaTTMH = @MaTTMH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiMH_Delete
(
    @MaTTMH CHAR(3)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM TrangThaiMH WHERE MaTTMH = @MaTTMH)
        BEGIN
            RAISERROR(N'Mã trạng thái không tồn tại!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- Kiểm tra xem trạng thái này có đang được dùng trong đơn hàng nào không
        IF EXISTS (SELECT 1 FROM DonMuaHang WHERE MaTTMH = @MaTTMH)
        BEGIN
            RAISERROR(N'Không thể xóa trạng thái này vì đang có đơn hàng sử dụng!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        DELETE FROM TrangThaiMH WHERE MaTTMH = @MaTTMH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC TrangThaiMH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTMH, TenTTMH
    FROM TrangThaiMH
    ORDER BY TenTTMH; 
END;
GO

CREATE OR ALTER PROC TrangThaiMH_GetByID
(
    @MaTTMH CHAR(3)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTMH, TenTTMH
    FROM TrangThaiMH
    WHERE MaTTMH = @MaTTMH;
END;
GO

CREATE OR ALTER PROC TrangThaiMH_Search
(
    @Search NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaTTMH, TenTTMH
    FROM TrangThaiMH
    WHERE (@Search IS NULL OR @Search = '' 
           OR MaTTMH LIKE '%' + @Search + '%' 
           OR TenTTMH LIKE N'%' + @Search + '%')
END;
GO