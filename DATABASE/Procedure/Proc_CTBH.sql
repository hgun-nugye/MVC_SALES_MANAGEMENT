USE DB_QLBH;
GO

-- =========================================
-- INSERT
-- =========================================
CREATE OR ALTER PROC sp_CTBH_Insert
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng sản phẩm trong cùng đơn
    IF EXISTS (SELECT 1 FROM CTBH WHERE MaDBH = @MaDBH AND MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại trong đơn hàng.', 16, 1);
        RETURN;
    END;

    -- Thêm chi tiết đơn hàng
    INSERT INTO CTBH (MaDBH, MaSP, SLM, DGM)
    VALUES (@MaDBH, @MaSP, @SLM, @DGM);
END;
GO


-- =========================================
-- UPDATE
-- =========================================
CREATE OR ALTER PROC sp_CTBH_Update
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CTBH
    SET SLM = @SLM,
        DGM = @DGM
    WHERE MaDBH = @MaDBH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- DELETE
-- =========================================
CREATE OR ALTER PROC sp_CTBH_Delete
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CTBH
    WHERE MaDBH = @MaDBH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- GET ALL
-- =========================================
CREATE OR ALTER PROC sp_CTBH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH;
END;
GO


-- =========================================
-- GET BY ID
-- =========================================
CREATE OR ALTER PROC sp_CTBH_GetByID
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE CT.MaDBH = @MaDBH;
END;
GO
