USE DB_QLBH;
GO

CREATE TYPE dbo.CTBH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLB INT,
    DGB MONEY
);
GO

CREATE OR ALTER PROC DonBanHang_Insert
(
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @DiaChiDBH NVARCHAR(255),
    @MaXa SMALLINT,
    @ChiTiet CTBH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @MaDBH CHAR(11);
        DECLARE @MaxNum INT;
        DECLARE @Prefix VARCHAR(8);

        -- Tạo mã đơn: BYYMMDDxxxx
        SET @Prefix = 'B' + RIGHT(CAST(YEAR(@NgayBH) AS VARCHAR(4)), 2)
                         + RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2)
                         + RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

        SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDBH,4) AS INT)),0)
        FROM DonBanHang
        WHERE NgayBH = @NgayBH;

        SET @MaDBH = @Prefix + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);

        -- Thêm đơn bán hàng
        INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH, DiaChiDBH, MaXa)
        VALUES (@MaDBH, @NgayBH, @MaKH, @DiaChiDBH, @MaXa);

        -- Thêm chi tiết
        INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
        SELECT @MaDBH, MaSP, SLB, DGB
        FROM @ChiTiet;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @DiaChiDBH NVARCHAR(255),
    @MaXa SMALLINT,
    @ChiTiet CTBH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS(SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
        BEGIN
            RAISERROR(N'Mã đơn bán hàng không tồn tại!',16,1);
            RETURN;
        END

        -- Cập nhật thông tin đơn
        UPDATE DonBanHang
        SET NgayBH = @NgayBH,
            MaKH = @MaKH,
            DiaChiDBH = @DiaChiDBH,
            MaXa = @MaXa
        WHERE MaDBH = @MaDBH;

        -- Xóa chi tiết cũ
        DELETE FROM CTBH WHERE MaDBH = @MaDBH;

        -- Thêm chi tiết mới
        INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
        SELECT @MaDBH, MaSP, SLB, DGB FROM @ChiTiet;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC DonBanHang_Delete
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS(SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
        BEGIN
            RAISERROR(N'Mã đơn bán hàng không tồn tại!',16,1);
            RETURN;
        END

        DELETE FROM CTBH WHERE MaDBH = @MaDBH;
        DELETE FROM DonBanHang WHERE MaDBH = @MaDBH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC DonBanHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        DBH.MaDBH,
        DBH.NgayBH,
        DBH.MaKH,
        KH.TenKH,
        DBH.DiaChiDBH,
        DBH.MaXa,
        CT.MaSP,
        SP.TenSP,
        CT.SLB,
        CT.DGB
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    JOIN CTBH CT ON CT.MaDBH = DBH.MaDBH
    JOIN SanPham SP ON SP.MaSP = CT.MaSP
    ORDER BY DBH.NgayBH DESC, DBH.MaDBH DESC;
END;
GO

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
        DBH.DiaChiDBH,
        DBH.MaXa,
        CT.MaSP,
        SP.TenSP,
        CT.SLB,
        CT.DGB
    FROM DonBanHang DBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    JOIN CTBH CT ON CT.MaDBH = DBH.MaDBH
    JOIN SanPham SP ON SP.MaSP = CT.MaSP
    WHERE DBH.MaDBH = @MaDBH;
END;
GO

CREATE OR ALTER PROC DonBanHang_Search
(
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT D.*, K.TenKH, C.SLB, C.DGB, C.MaSP, S.TenSP
    FROM DonBanHang D
    LEFT JOIN KhachHang K ON K.MaKH = D.MaKH
    LEFT JOIN CTBH C ON C.MaDBH = D.MaDBH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR D.MaDBH LIKE '%' + @Search + '%'
            OR D.MaKH LIKE '%' + @Search + '%'
            OR K.TenKH LIKE N'%' + @Search + '%'
        )
        AND (@Month IS NULL OR MONTH(D.NgayBH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayBH) = @Year)
    ORDER BY D.NgayBH DESC;
END;
GO