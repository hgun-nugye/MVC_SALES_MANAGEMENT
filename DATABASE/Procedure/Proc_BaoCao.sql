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
        WHERE YEAR(DBH.NgayBH) = @CurrentYear AND MONTH(DBH.NgayBH) = @CurrentMonth
    );
    
    DECLARE @YearlyRevenue DECIMAL(18,2) = (
        SELECT SUM(SLB * DGB) 
        FROM CTBH CT
        JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
        WHERE YEAR(DBH.NgayBH) = @CurrentYear
    );
    
    DECLARE @MonthlyOrders INT = (
        SELECT COUNT(*) 
        FROM DonBanHang 
        WHERE YEAR(NgayBH) = @CurrentYear AND MONTH(NgayBH) = @CurrentMonth
    );

    -- Thêm TotalRevenue và TotalCost
    DECLARE @TotalRevenue DECIMAL(18,2) = (SELECT ISNULL(SUM(SLB * DGB), 0) FROM CTBH);
    DECLARE @TotalCost DECIMAL(18,2) = (SELECT ISNULL(SUM(SLM * DGM), 0) FROM CTMH);

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
        (SELECT COUNT(*) FROM DonBanHang WHERE YEAR(NgayBH) = @Year AND MONTH(NgayBH) = m.Month) AS OrderCount
    FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) AS m(Month)
    LEFT JOIN DonBanHang dbh ON YEAR(dbh.NgayBH) = @Year AND MONTH(dbh.NgayBH) = m.Month
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
    GROUP BY SP.MaSP, SP.TenSP
    ORDER BY TotalQuantitySold DESC;
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
        SELECT TOP (@Limit) MaSP
        FROM CTBH
        GROUP BY MaSP
        ORDER BY SUM(SLB) DESC
    )
    SELECT TOP (@Limit)
        SP.MaSP,
        SP.TenSP,
        ISNULL(SUM(CT.SLB), 0) AS TotalQuantitySold,
        ISNULL(SUM(CT.SLB * CT.DGB), 0) AS TotalRevenue
    FROM SanPham SP
    LEFT JOIN CTBH CT ON SP.MaSP = CT.MaSP
    WHERE SP.MaSP NOT IN (SELECT MaSP FROM TopSelling)
    GROUP BY SP.MaSP, SP.TenSP
    ORDER BY TotalQuantitySold ASC;
END;
GO

-- 5. BaoCao_GetOrderDetailsReport
CREATE OR ALTER PROC BaoCao_GetOrderDetailsReport
(
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
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
    GROUP BY DBH.MaDBH, DBH.NgayBH, KH.TenKH, DBH.DiaChiDBH, TT.TenTTBH
	ORDER BY DBH.NgayBH ASC
END;
GO

CREATE OR ALTER PROC BaoCao_GetImportOrderDetailsReport
(
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MH.MaDMH,
        MH.NgayMH,
        NCC.TenNCC,
        SUM(CT.SLM) AS SoLuongSP,
        SUM(CT.SLM * CT.DGM) AS TongTien
    FROM DonMuaHang MH
    JOIN CTMH CT ON MH.MaDMH = CT.MaDMH
    JOIN NhaCC NCC ON MH.MaNCC = NCC.MaNCC
    WHERE
        (@FromDate IS NULL OR MH.NgayMH >= @FromDate)
        AND (@ToDate IS NULL OR MH.NgayMH <= @ToDate)
    GROUP BY
        MH.MaDMH, MH.NgayMH, NCC.TenNCC
    ORDER BY MH.NgayMH ASC;
END;
GO

-- 6. BaoCao_GetProductRevenueReport
CREATE OR ALTER PROC BaoCao_GetProductRevenueReport
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SP.MaSP,
        SP.TenSP,
        SP.GiaBan,
        ISNULL(SUM(CT.SLB), 0) AS SoLuongBan,
        ISNULL(SUM(CT.SLB * CT.DGB), 0) AS DoanhThu,
        COUNT(DISTINCT CT.MaDBH) AS SoDonHang
    FROM SanPham SP
    LEFT JOIN CTBH CT ON SP.MaSP = CT.MaSP
    GROUP BY SP.MaSP, SP.TenSP, SP.GiaBan
    ORDER BY DoanhThu DESC;
END;
GO
