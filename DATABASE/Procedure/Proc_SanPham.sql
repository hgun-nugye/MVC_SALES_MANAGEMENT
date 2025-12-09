USE DB_QLBH;
GO

CREATE OR ALTER PROC SanPham_Insert
(
    @TenSP NVARCHAR(50),
    @DonGia DECIMAL(18,2),
    @GiaBan DECIMAL(18,2),
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),

    @ThanhPhan NVARCHAR(MAX),
    @CongDung NVARCHAR(MAX),
    @HDSD NVARCHAR(MAX),
    @XuatXu NVARCHAR(MAX),
    @BaoQuan NVARCHAR(MAX),

    @TrangThai NVARCHAR(50),
    @SoLuongTon INT,

    @MaLoai VARCHAR(10),
    @MaHang CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaSP, 3, LEN(MaSP)-2) AS INT)), 0)
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);
    
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO SanPham
        (
            MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH,
            ThanhPhan, CongDung, HDSD, XuatXu, BaoQuan,
            TrangThai, SoLuongTon, MaLoai, MaHang
        )
        VALUES
        (
            @MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH,
            @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan,
            @TrangThai, @SoLuongTon, @MaLoai, @MaHang
        );

        PRINT N'Thêm sản phẩm thành công!';
    END TRY
    BEGIN CATCH
        RAISERROR(N'Lỗi khi thêm sản phẩm: %s', 16, 1);
    END CATCH
END;
GO

CREATE OR ALTER PROC SanPham_Update
(
    @MaSP VARCHAR(10),
    @TenSP NVARCHAR(50),
    @DonGia DECIMAL(18,2),
    @GiaBan DECIMAL(18,2),
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),

    @ThanhPhan NVARCHAR(MAX),
    @CongDung NVARCHAR(MAX),
    @HDSD NVARCHAR(MAX),
    @XuatXu NVARCHAR(MAX),
    @BaoQuan NVARCHAR(MAX),

    @TrangThai NVARCHAR(50),
    @SoLuongTon INT,

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

    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    UPDATE SanPham
    SET
        TenSP = @TenSP,
        DonGia = @DonGia,
        GiaBan = @GiaBan,
        MoTaSP = @MoTaSP,
        AnhMH = @AnhMH,

        ThanhPhan = @ThanhPhan,
        CongDung = @CongDung,
        HDSD = @HDSD,
        XuatXu = @XuatXu,
        BaoQuan = @BaoQuan,

        TrangThai = @TrangThai,
        SoLuongTon = @SoLuongTon,

        MaLoai = @MaLoai,
		MaHang = @MaHang
    WHERE MaSP = @MaSP;
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
END;
GO

CREATE OR ALTER PROC SanPham_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        S.*,
        L.TenLoai,
		H.TenHang
    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
	JOIN Hang H ON H.MaHang = S.MaHang
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
        S.*,
        L.TenLoai,
		H.TenHang
    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
	JOIN Hang H ON H.MaHang = S.MaHang
    WHERE S.MaSP = @MaSP;
END;
GO

CREATE OR ALTER PROCEDURE SanPham_Search
    @Search      NVARCHAR(100) = NULL,
    @TrangThai   NVARCHAR(50)  = NULL,
    @TenLoai     VARCHAR(10)   = NULL   
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        S.*,
        L.TenLoai,
		H.TenHang
    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
	JOIN Hang H ON H.MaHang = S.MaHang
    WHERE 
        (
            @Search IS NULL OR @Search = '' OR
            H.TenHang   LIKE '%' + @Search + '%' OR
            S.TenSP   LIKE '%' + @Search + '%' OR
            S.MaSP    LIKE '%' + @Search + '%' OR
            L.TenLoai LIKE '%' + @Search + '%'
        )
        AND (@TrangThai IS NULL OR S.TrangThai = @TrangThai)
        AND (@TenLoai IS NULL OR L.MaLoai = @TenLoai)
    
END;
GO
