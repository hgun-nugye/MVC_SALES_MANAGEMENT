USE DB_QLBH;
GO

CREATE OR ALTER PROC SanPham_Insert
(
    @TenSP NVARCHAR(50),
    @GiaBan DECIMAL(18,2),
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),

    @ThanhPhan NVARCHAR(MAX),
    @CongDung NVARCHAR(MAX),
    @HDSD NVARCHAR(MAX),
    @HDBaoQuan NVARCHAR(MAX),
    @TrongLuong DECIMAL(5,2),

    @MaTT CHAR(3),
    @MaLoai VARCHAR(10),
    @MaHang CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaSP, 3, 8) AS INT)), 0)
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    INSERT INTO SanPham
    (
        MaSP, TenSP, GiaBan, MoTaSP, AnhMH,
        ThanhPhan, CongDung, HDSD, HDBaoQuan, TrongLuong,
        MaTT, MaLoai, MaHang
    )
    VALUES
    (
        @MaSP, @TenSP, @GiaBan, @MoTaSP, @AnhMH,
        @ThanhPhan, @CongDung, @HDSD, @HDBaoQuan, @TrongLuong,
        @MaTT, @MaLoai, @MaHang
    );

    PRINT N'Thêm sản phẩm thành công!';
END;
GO

CREATE OR ALTER PROC SanPham_Update
(
    @MaSP VARCHAR(10),
    @TenSP NVARCHAR(50),
    @GiaBan DECIMAL(18,2),
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),

    @ThanhPhan NVARCHAR(MAX),
    @CongDung NVARCHAR(MAX),
    @HDSD NVARCHAR(MAX),
    @HDBaoQuan NVARCHAR(MAX),
    @TrongLuong DECIMAL(5,2),

    @MaTT CHAR(3),
    @MaLoai VARCHAR(10),
    @MaHang CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

	IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP and MaSP<>@MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại.', 16, 1);
        RETURN;
    END;

    UPDATE SanPham
    SET
        TenSP = @TenSP,
        GiaBan = @GiaBan,
        MoTaSP = @MoTaSP,
        AnhMH = @AnhMH,

        ThanhPhan = @ThanhPhan,
        CongDung = @CongDung,
        HDSD = @HDSD,
        HDBaoQuan = @HDBaoQuan,
        TrongLuong = @TrongLuong,

        MaTT = @MaTT,
        MaLoai = @MaLoai,
        MaHang = @MaHang
    WHERE MaSP = @MaSP;

    PRINT N'Cập nhật sản phẩm thành công!';
END;
GO

CREATE OR ALTER PROC SanPham_Delete
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM SanPham WHERE MaSP = @MaSP;
    PRINT N'Xóa sản phẩm thành công!';
END;
GO

CREATE OR ALTER PROC SanPham_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        S.MaSP,
        S.TenSP,
        S.GiaBan,
        S.MoTaSP,
        S.AnhMH,
        S.ThanhPhan,
        S.CongDung,
        S.HDSD,
        S.HDBaoQuan,
        S.TrongLuong,

        L.TenLoai,
        H.TenHang,
        TT.TenTT,

        -- TỒN KHO
        ISNULL(SUM(CMH.SLM), 0) - ISNULL(SUM(CBH.SLB), 0) AS SoLuongTon

    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
    JOIN Hang H ON H.MaHang = S.MaHang
    JOIN TrangThai TT ON TT.MaTT = S.MaTT

    LEFT JOIN CTMH CMH ON CMH.MaSP = S.MaSP
    LEFT JOIN CTBH CBH ON CBH.MaSP = S.MaSP

    GROUP BY 
        S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH,
        S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
        L.TenLoai, H.TenHang, TT.TenTT;
END;
GO

CREATE OR ALTER PROC SanPham_GetByID
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        S.MaSP,
        S.TenSP,
        S.GiaBan,
        S.MoTaSP,
        S.AnhMH,
        S.ThanhPhan,
        S.CongDung,
        S.HDSD,
        S.HDBaoQuan,
        S.TrongLuong,

        L.TenLoai,
        H.TenHang,
        TT.TenTT,

        ISNULL(SUM(CMH.SLM), 0) - ISNULL(SUM(CBH.SLB), 0) AS SoLuongTon

    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
    JOIN Hang H ON H.MaHang = S.MaHang
    JOIN TrangThai TT ON TT.MaTT = S.MaTT

    LEFT JOIN CTMH CMH ON CMH.MaSP = S.MaSP
    LEFT JOIN CTBH CBH ON CBH.MaSP = S.MaSP

    WHERE S.MaSP = @MaSP

    GROUP BY 
        S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH,
        S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
        L.TenLoai, H.TenHang, TT.TenTT;
END;
GO

CREATE OR ALTER PROC SanPham_Search
(
    @Search NVARCHAR(100) = NULL,
    @MaTT   CHAR(3) = NULL,
    @MaLoai VARCHAR(10) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        S.MaSP,
        S.TenSP,
        S.GiaBan,
        S.MoTaSP,
        S.AnhMH,
        S.ThanhPhan,
        S.CongDung,
        S.HDSD,
        S.HDBaoQuan,
        S.TrongLuong,

        L.TenLoai,
        H.TenHang,
        TT.TenTT,

        ISNULL(SUM(CMH.SLM), 0) - ISNULL(SUM(CBH.SLB), 0) AS SoLuongTon

    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
    JOIN Hang H ON H.MaHang = S.MaHang
    JOIN TrangThai TT ON TT.MaTT = S.MaTT

    LEFT JOIN CTMH CMH ON CMH.MaSP = S.MaSP
    LEFT JOIN CTBH CBH ON CBH.MaSP = S.MaSP

    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            S.TenSP LIKE '%' + @Search + '%' OR
            H.TenHang LIKE '%' + @Search + '%' OR
            L.TenLoai LIKE '%' + @Search + '%'
        )
        AND (@MaTT IS NULL OR S.MaTT = @MaTT)
        AND (@MaLoai IS NULL OR S.MaLoai = @MaLoai)

    GROUP BY 
        S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH,
        S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
        L.TenLoai, H.TenHang, TT.TenTT;
END;
GO
