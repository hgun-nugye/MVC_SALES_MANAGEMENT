USE DB_QLBH;
GO
CREATE TYPE dbo.CTBH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLB INT,
    DGB MONEY
);
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_Insert
(
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @ChiTiet CTBH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDBH CHAR(11);
    DECLARE @MaxNum INT;
    DECLARE @Prefix VARCHAR(8);

    -- Tạo prefix BYYMMDD (vd: B251119)
    SET @Prefix = 'B' +
                  RIGHT(CAST(YEAR(@NgayBH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

    -- Lấy số lớn nhất trong ngày và tăng lên 1
    SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDBH,4) AS INT)),0)
    FROM DonBanHang
    WHERE CONVERT(DATE, NgayBH) = @NgayBH;

    SET @MaDBH = @Prefix + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);

    -- Thêm đơn bán hàng
    INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH)
    VALUES (@MaDBH, @NgayBH, @MaKH);

    -- Thêm chi tiết bán hàng
    INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
    SELECT @MaDBH, MaSP, SLB, DGB
    FROM @ChiTiet;

END;
GO

---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10)
   
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra đơn hàng tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra khách hàng tồn tại
    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Mã khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Cập nhật thông tin đơn
    UPDATE DonBanHang
    SET NgayBH = @NgayBH,
        MaKH = @MaKH
    WHERE MaDBH = @MaDBH;
	
END;
GO

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_Delete
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM DonBanHang WHERE MaDBH = @MaDBH;
END;
GO

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

   SELECT 
        DBH.MaDBH,
        DBH.NgayBH,
        DBH.MaKH,
        KH.TenKH,
        CT.MaSP,
        SP.TenSP,
        CT.SLB,
        CT.DGB
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    JOIN CTBH CT ON CT.MaDBH = DBH.MaDBH
    JOIN SanPham SP ON SP.MaSP = CT.MaSP
    ORDER BY DBH.NgayBH DESC;
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_GetByID
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        DBH.MaDBH,
        DBH.NgayBH,
        DBH.MaKH,
        KH.TenKH,
		
        CT.MaSP,
        SP.TenSP,
        CT.SLB,
        CT.DGB
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    JOIN CTBH CT ON CT.MaDBH = DBH.MaDBH
    JOIN SanPham SP ON SP.MaSP = CT.MaSP
	JOIN Xa X ON X.MaXa = KH.MaXa
	JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE DBH.MaDBH = @MaDBH;
END;
GO

-- Search
CREATE OR ALTER PROCEDURE DonBanHang_Search
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
	SELECT D.*, K.TenKH
    FROM DonBanHang D
	JOIN KhachHang K ON K.MaKH = D.MaKH
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR D.MaDBH LIKE '%' + @Search + '%'
            OR D.MaKH LIKE '%' + @Search + '%'
			OR K.TenKH LIKE N'%' + @Search + '%'
			)
        AND (@Month IS NULL OR MONTH(D.NgayBH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayBH) = @Year)
END;
GO
