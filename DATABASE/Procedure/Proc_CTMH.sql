USE DB_QLBH;
GO

CREATE OR ALTER PROC CTMH_Insert
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng sản phẩm trong cùng đơn
    IF EXISTS (SELECT 1 FROM CTMH WHERE MaDMH = @MaDMH AND MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại trong đơn mua này.', 16, 1);
        RETURN;
    END;

    -- Thêm chi tiết đơn mua
    INSERT INTO CTMH (MaDMH, MaSP, SLM, DGM)
    VALUES (@MaDMH, @MaSP, @SLM, @DGM);	
END;
GO

CREATE OR ALTER PROC CTMH_Update
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; 

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @OldSLM INT;

        SELECT @OldSLM = SLM 
        FROM CTMH 
        WHERE MaDMH = @MaDMH AND MaSP = @MaSP;

        IF @OldSLM IS NULL
        BEGIN
            RAISERROR(N'Chi tiết không tồn tại.', 16, 1);
        END

        -- Cập nhật chi tiết mua hàng
        UPDATE CTMH
        SET SLM = @SLM,
            DGM = @DGM
        WHERE MaDMH = @MaDMH AND MaSP = @MaSP;      
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;

        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi cập nhật CTMH: %s', 16, 1, @Err);
    END CATCH;
END;
GO

CREATE OR ALTER PROC CTMH_Delete
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @SLM INT;
        SELECT @SLM = SLM FROM CTMH WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
        IF @SLM IS NULL
        BEGIN
            RAISERROR(N'Chi tiết đơn mua không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END;

        -- Xóa
        DELETE FROM CTMH WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(N'Lỗi khi xóa CTMH: %s', 16, 1, @Err);
    END CATCH;
END;
GO

CREATE OR ALTER PROC CTMH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
		SP.GiaBan,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC;
END;
GO

CREATE OR ALTER PROC CTMH_GetByID
(
    @MaDMH CHAR(11),
	@MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE CT.MaDMH = @MaDMH AND CT.MaSP = @MaSP;
END;
GO

