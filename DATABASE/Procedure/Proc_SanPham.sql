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
    @MaNCC VARCHAR(10),
    @MaGH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @MaxID INT;

    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaSP, 3, LEN(MaSP)-2) AS INT)), 0)
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP AND MaGH = @MaGH)
    BEGIN
        RAISERROR(N'Tên sản phẩm đã tồn tại trong gian hàng này!', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO SanPham
        (
            MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH,
            ThanhPhan, CongDung, HDSD, XuatXu, BaoQuan,
            TrangThai, SoLuongTon, MaLoai, MaNCC, MaGH
        )
        VALUES
        (
            @MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH,
            @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan,
            @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH
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
    @MaNCC VARCHAR(10),
    @MaGH VARCHAR(10)
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

    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
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
        MaNCC = @MaNCC,
        MaGH = @MaGH
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
        G.TenGH,
        N.TenNCC
    FROM SanPham S
    JOIN LoaiSP L ON L.MaLoai = S.MaLoai
    JOIN GianHang G ON G.MaGH = S.MaGH
    JOIN NhaCC N ON N.MaNCC = S.MaNCC;
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
        sp.*,
        l.TenLoai,
        n.TenNCC,
        g.TenGH
    FROM SanPham sp
    LEFT JOIN LoaiSP l ON sp.MaLoai = l.MaLoai
    LEFT JOIN NhaCC n ON sp.MaNCC = n.MaNCC
    LEFT JOIN GianHang g ON sp.MaGH = g.MaGH
    WHERE sp.MaSP = @MaSP;
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
        G.TenGH, 
        N.TenNCC
    FROM SanPham S
        JOIN LoaiSP L   ON L.MaLoai = S.MaLoai
        JOIN GianHang G ON G.MaGH   = S.MaGH
        JOIN NhaCC N    ON N.MaNCC  = S.MaNCC
    WHERE 
        (
            @Search IS NULL OR @Search = '' OR
            G.TenGH   LIKE '%' + @Search + '%' OR
            S.TenSP   LIKE '%' + @Search + '%' OR
            S.MaSP    LIKE '%' + @Search + '%' OR
            N.TenNCC  LIKE '%' + @Search + '%' OR
            L.TenLoai LIKE '%' + @Search + '%'
        )
        AND (@TrangThai IS NULL OR S.TrangThai = @TrangThai)
        AND (@TenLoai IS NULL OR L.MaLoai = @TenLoai)
    
END;
GO
