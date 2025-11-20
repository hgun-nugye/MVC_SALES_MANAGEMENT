USE DB_QLBH;
GO

-- =========================================
-- INSERT
-- =========================================
CREATE OR ALTER PROC CTBH_Insert
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLB INT,
    @DGB MONEY
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
    INSERT INTO CTBH (MaDBH, MaSP, SLB, DGB)
    VALUES (@MaDBH, @MaSP, @SLB, @DGB);
END;
GO


-- =========================================
-- UPDATE
-- =========================================
CREATE OR ALTER PROC CTBH_Update
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLB INT,
    @DGB MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CTBH
    SET SLB = @SLB,
        DGB = @DGB
    WHERE MaDBH = @MaDBH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- DELETE
-- =========================================
CREATE OR ALTER PROC CTBH_Delete
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
CREATE OR ALTER PROC CTBH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH;
END;
GO

-- Get All Detail
CREATE OR ALTER PROC CTBH_GetAll_Detail AS SELECT C.*, S.TenSP FROM CTBH C JOIN SanPham S ON S.MaSP=C.MaSP;
GO

-- =========================================
-- GET BY ID
-- =========================================
CREATE OR ALTER PROC CTBH_GetByID
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
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE CT.MaDBH = @MaDBH;
END;
GO

-- Get By ID Detail
CREATE OR ALTER PROC CTBH_GetById_Detail @MaDBH CHAR(11), @MaSP VARCHAR(10)
AS SELECT C.*, S.TenSP FROM CTBH C JOIN SanPham S ON S.MaSP=C.MaSP WHERE C.MaDBH = @MaDBH AND C.MaSP = @MaSP;
GO

-- =========================================
-- SEARCH
-- =========================================
CREATE OR ALTER PROC CTBH_Search
(
    @MaDBH CHAR(11) = NULL,
    @MaSP VARCHAR(10) = NULL,
    @TenSP NVARCHAR(100) = NULL,
    @TenKH NVARCHAR(100) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE
        (@MaDBH IS NULL OR CT.MaDBH = @MaDBH)
        AND (@MaSP IS NULL OR CT.MaSP = @MaSP)
        AND (@TenSP IS NULL OR SP.TenSP LIKE '%' + @TenSP + '%')
        AND (@TenKH IS NULL OR KH.TenKH LIKE '%' + @TenKH + '%')
        AND (@TuNgay IS NULL OR DBH.NgayBH >= @TuNgay)
        AND (@DenNgay IS NULL OR DBH.NgayBH <= @DenNgay)
    ORDER BY DBH.NgayBH DESC;
END;
GO
