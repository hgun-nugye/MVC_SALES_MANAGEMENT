USE DB_QLBH;
GO

-- =========================================
-- INSERT
-- =========================================
CREATE OR ALTER PROC sp_CTMH_Insert
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


-- =========================================
-- UPDATE
-- =========================================
CREATE OR ALTER PROC sp_CTMH_Update
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CTMH
    SET SLM = @SLM,
        DGM = @DGM
    WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- DELETE
-- =========================================
CREATE OR ALTER PROC sp_CTMH_Delete
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CTMH
    WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- GET ALL
-- =========================================
CREATE OR ALTER PROC sp_CTMH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.DonGia,
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


-- =========================================
-- GET BY ID
-- =========================================
CREATE OR ALTER PROC sp_CTMH_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.DonGia,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE CT.MaDMH = @MaDMH;
END;
GO
