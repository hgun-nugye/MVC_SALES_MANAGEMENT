USE DB_QLBH;
GO

-- 1. BaoCao_GetStats
CREATE OR ALTER PROC BaoCao_GetStats
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalProducts INT = (SELECT COUNT(*) FROM SanPham);
    DECLARE @TotalCustomers INT = (SELECT COUNT(*) FROM KhachHang);
    DECLARE @TotalEmployees INT = (SELECT COUNT(*) FROM NhanVien);
    DECLARE @TotalOrders INT = (SELECT COUNT(*) FROM DonBanHang);
    DECLARE @TotalPurchaseOrders INT = (SELECT COUNT(*) FROM DonMuaHang);
    
    DECLARE @CurrentYear INT = YEAR(GETDATE());
    DECLARE @CurrentMonth INT = MONTH(GETDATE());
    
    DECLARE @MonthlyRevenue DECIMAL(18,2) = (
        SELECT SUM(SLB * DGB) 
        FROM CTBH CT
        JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
        WHERE YEAR(DBH.NgayBH) = @CurrentYear 
          AND MONTH(DBH.NgayBH) = @CurrentMonth
          AND DBH.MaTTBH = 'HTH'
    );
    
    DECLARE @YearlyRevenue DECIMAL(18,2) = (
        SELECT SUM(SLB * DGB) 
        FROM CTBH CT
        JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
        WHERE YEAR(DBH.NgayBH) = @CurrentYear
          AND DBH.MaTTBH = 'HTH'
    );
    
    DECLARE @MonthlyOrders INT = (
        SELECT COUNT(*) 
        FROM DonBanHang 
        WHERE YEAR(NgayBH) = @CurrentYear AND MONTH(NgayBH) = @CurrentMonth
    );

    -- Thêm TotalRevenue và TotalCost
    DECLARE @TotalRevenue DECIMAL(18,2) = (
        SELECT ISNULL(SUM(SLB * DGB), 0) 
        FROM CTBH CT
        JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
        WHERE DBH.MaTTBH = 'HTH'
    );
    DECLARE @TotalCost DECIMAL(18,2) = (
        SELECT ISNULL(SUM(SLM * DGM), 0) 
        FROM CTMH CT
        JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
        WHERE DMH.MaTTMH = 'HTH'
    );

    SELECT 
        ISNULL(@TotalProducts, 0) AS TotalProducts,
        ISNULL(@TotalCustomers, 0) AS TotalCustomers,
        ISNULL(@TotalEmployees, 0) AS TotalEmployees,
        ISNULL(@TotalOrders, 0) AS TotalOrders,
        ISNULL(@TotalPurchaseOrders, 0) AS TotalPurchaseOrders,
        ISNULL(@MonthlyRevenue, 0) AS MonthlyRevenue,
        ISNULL(@YearlyRevenue, 0) AS YearlyRevenue,
        ISNULL(@MonthlyOrders, 0) AS MonthlyOrders,
        ISNULL(@TotalRevenue, 0) AS TotalRevenue,
        ISNULL(@TotalCost, 0) AS TotalCost;
END;
GO

-- 2. BaoCao_GetMonthlyRevenue
CREATE OR ALTER PROC BaoCao_GetMonthlyRevenue
(
    @Year INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.Month,
        DATENAME(MONTH, DATEFROMPARTS(@Year, m.Month, 1)) AS MonthName,
        ISNULL(SUM(ct.SLB * ct.DGB), 0) AS Revenue,
        (SELECT COUNT(*) FROM DonBanHang WHERE YEAR(NgayBH) = @Year AND MONTH(NgayBH) = m.Month AND MaTTBH = 'HTH') AS OrderCount
    FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) AS m(Month)
    LEFT JOIN DonBanHang dbh ON YEAR(dbh.NgayBH) = @Year AND MONTH(dbh.NgayBH) = m.Month AND dbh.MaTTBH = 'HTH'
    LEFT JOIN CTBH ct ON dbh.MaDBH = ct.MaDBH
    GROUP BY m.Month
    ORDER BY m.Month;
END;
GO

-- 3. BaoCao_GetTopSellingProducts
CREATE OR ALTER PROC BaoCao_GetTopSellingProducts
(
    @Limit INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Limit)
        SP.MaSP,
        SP.TenSP,
        SUM(CT.SLB) AS TotalQuantitySold,
        SUM(CT.SLB * CT.DGB) AS TotalRevenue
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    WHERE DBH.MaTTBH = 'HTH' OR DBH.MaTTBH = 'DXN'
    GROUP BY SP.MaSP, SP.TenSP
    ORDER BY TotalRevenue DESC;
END;
GO

-- 4. BaoCao_GetSlowMovingProducts
CREATE OR ALTER PROC BaoCao_GetSlowMovingProducts
(
    @Limit INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH TopSelling AS (
        SELECT TOP (@Limit) CT.MaSP
        FROM CTBH CT
        JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
        WHERE DBH.MaTTBH = 'HTH' OR DBH.MaTTBH = 'DXN'
        GROUP BY CT.MaSP
        ORDER BY SUM(CT.SLB) DESC
    )
    SELECT TOP (@Limit)
        SP.MaSP,
        SP.TenSP,
        ISNULL(SUM(CT.SLB), 0) AS TotalQuantitySold,
        ISNULL(SUM(CT.SLB * CT.DGB), 0) AS TotalRevenue
    FROM SanPham SP
    LEFT JOIN CTBH CT ON SP.MaSP = CT.MaSP
    LEFT JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH AND DBH.MaTTBH = 'HTH'
    WHERE SP.MaSP NOT IN (SELECT MaSP FROM TopSelling)
    GROUP BY SP.MaSP, SP.TenSP
    ORDER BY TotalRevenue ASC;
END;
GO

-- 5. BaoCao_GetOrderDetailsReport
CREATE OR ALTER PROC BaoCao_GetOrderDetailsReport
(
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @MaTTBH CHAR(3) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        DBH.MaDBH,
        DBH.NgayBH,
        KH.TenKH,
        DBH.DiaChiDBH,
        TT.TenTTBH AS TrangThai,
        ISNULL(SUM(CT.SLB * CT.DGB), 0) AS TongTien,
        COUNT(CT.MaSP) AS SoLuongSP
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    JOIN TrangThaiBH TT ON DBH.MaTTBH = TT.MaTTBH
    LEFT JOIN CTBH CT ON DBH.MaDBH = CT.MaDBH
    WHERE (@FromDate IS NULL OR DBH.NgayBH >= @FromDate)
      AND (@ToDate IS NULL OR DBH.NgayBH <= @ToDate)
      AND (@MaTTBH IS NULL OR DBH.MaTTBH = @MaTTBH)
    GROUP BY DBH.MaDBH, DBH.NgayBH, KH.TenKH, DBH.DiaChiDBH, TT.TenTTBH
	ORDER BY DBH.NgayBH ASC
END;
GO

CREATE OR ALTER PROC BaoCao_GetImportOrderDetailsReport
(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @MaTTMH CHAR(3) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MH.MaDMH,
        MH.NgayMH,
        NCC.TenNCC,
        TT.TenTTMH AS TrangThai,
        SUM(CT.SLM) AS SoLuongSP,
        SUM(CT.SLM * CT.DGM) AS TongTien
    FROM DonMuaHang MH
    JOIN CTMH CT ON MH.MaDMH = CT.MaDMH
    JOIN NhaCC NCC ON MH.MaNCC = NCC.MaNCC
    JOIN TrangThaiMH TT ON MH.MaTTMH = TT.MaTTMH
    WHERE (@FromDate IS NULL OR MH.NgayMH >= @FromDate)
      AND (@ToDate IS NULL OR MH.NgayMH <= @ToDate)
      AND (@MaTTMH IS NULL OR MH.MaTTMH = @MaTTMH)
    GROUP BY
        MH.MaDMH, MH.NgayMH, NCC.TenNCC, TT.TenTTMH
    ORDER BY MH.NgayMH ASC;
END;
GO

-- 6. BaoCao_GetProductRevenueReport
CREATE OR ALTER PROC BaoCao_GetProductRevenueReport_Today
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Khai báo biến ngày để SQL tối ưu hóa việc truy vấn (SARGable)
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

    SELECT
        SP.MaSP,
        SP.TenSP,
        SP.GiaBan,
        ISNULL(SUM(CT.SLB), 0) AS SoLuongBan,
        ISNULL(SUM(CT.SLB * CT.DGB), 0) AS DoanhThu,
        COUNT(DISTINCT CT.MaDBH) AS SoDonHang
    FROM SanPham SP
    INNER JOIN CTBH CT ON SP.MaSP = CT.MaSP
    INNER JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH 
    WHERE 
        DBH.MaTTBH = 'HTH' OR DBH.MaTTBH = 'DXN'
        -- Lấy từ 00:00:00 hôm nay đến trước 00:00:00 ngày mai
        AND DBH.NgayBH >= @Today 
        AND DBH.NgayBH < DATEADD(DAY, 1, @Today)
    GROUP BY SP.MaSP, SP.TenSP, SP.GiaBan
    ORDER BY DoanhThu DESC;
END;
GO