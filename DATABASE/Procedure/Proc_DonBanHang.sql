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

    -- Prefix BYYMMDD
    SET @Prefix = 'B' +
                  RIGHT(CAST(YEAR(@NgayBH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

    SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDBH, 4) AS INT)), 0)
    FROM DonBanHang
    WHERE CONVERT(DATE, NgayBH) = @NgayBH;

    SET @MaDBH = @Prefix + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);

    -- Thêm đơn
    INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH)
    VALUES (@MaDBH, @NgayBH, @MaKH);

    -- Thêm chi tiết
    INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
    SELECT @MaDBH, MaSP, SLB, DGB
    FROM @ChiTiet;

    UPDATE SP
    SET SP.SoLuongTon = SP.SoLuongTon - CT.SLB
    FROM SanPham SP
    JOIN @ChiTiet CT ON CT.MaSP = SP.MaSP;

END;
GO


---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @ChiTiet CTBH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
    BEGIN
        RAISERROR(N'Mã khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRAN;

        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon + C.SLB
        FROM SanPham SP
        JOIN CTBH C ON C.MaSP = SP.MaSP
        WHERE C.MaDBH = @MaDBH;

        DELETE FROM CTBH WHERE MaDBH = @MaDBH;

             INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
        SELECT @MaDBH, MaSP, SLB, DGB
        FROM @ChiTiet;

        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon - CT.SLB
        FROM SanPham SP
        JOIN @ChiTiet CT ON CT.MaSP = SP.MaSP;

        UPDATE DonBanHang
        SET NgayBH = @NgayBH,
            MaKH = @MaKH
        WHERE MaDBH = @MaDBH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(2000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH;
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

    IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
    BEGIN
        RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRAN;
        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon + C.SLB
        FROM SanPham SP
        JOIN CTBH C ON C.MaSP = SP.MaSP
        WHERE C.MaDBH = @MaDBH;

        DELETE FROM CTBH WHERE MaDBH = @MaDBH;

        DELETE FROM DonBanHang WHERE MaDBH = @MaDBH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(2000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH;
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
