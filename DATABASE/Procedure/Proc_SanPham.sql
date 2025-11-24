USE DB_QLBH;
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC SanPham_Insert
(
    @TenSP NVARCHAR(50),
    @DonGia MONEY,
    @GiaBan MONEY,
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),
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
    DECLARE @Next INT;

    -- Lấy mã lớn nhất hiện có, rồi cộng thêm 1
    SELECT @Next = ISNULL(MAX(CAST(SUBSTRING(MaSP, 8, LEN(MaSP)) AS INT)), 0) + 1
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@Next AS VARCHAR(8)), 8);

    -- Kiểm tra trùng tên sản phẩm
    IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP)
    BEGIN
        RAISERROR(N'Tên sản phẩm đã tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã loại hợp lệ
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã nhà cung cấp hợp lệ
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Thực hiện thêm mới
    INSERT INTO SanPham(MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH, TrangThai, SoLuongTon, MaLoai, MaNCC, MaGH)
    VALUES (@MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH);
END;
GO

---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC SanPham_Update
(
    @MaSP VARCHAR(10),
    @TenSP NVARCHAR(50),
    @DonGia MONEY,
    @GiaBan MONEY,
    @MoTaSP NVARCHAR(MAX),
    @AnhMH NVARCHAR(255),
    @TrangThai NVARCHAR(50),
    @SoLuongTon INT,
    @MaLoai VARCHAR(10),
    @MaNCC VARCHAR(10),
	@MaGH VARCHAR(10)

)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra mã sản phẩm
    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã loại hợp lệ
    IF NOT EXISTS (SELECT 1 FROM LoaiSP WHERE MaLoai = @MaLoai)
    BEGIN
        RAISERROR(N'Mã loại không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra mã nhà cung cấp hợp lệ
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    UPDATE SanPham
    SET TenSP = @TenSP,
        DonGia = @DonGia,
        GiaBan = @GiaBan,
        MoTaSP = @MoTaSP,
        AnhMH = @AnhMH,
        TrangThai = @TrangThai,
        SoLuongTon = @SoLuongTon,
        MaLoai = @MaLoai,
        MaNCC = @MaNCC,
		MaGH = @MaGH
    WHERE MaSP = @MaSP;
END;
GO

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
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

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC SanPham_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.*, L.TenLoai, G.TenGH, N.TenNCC
    FROM SanPham S
	JOIN LoaiSP L ON L.MaLoai=S.MaLoai
	JOIN GianHang G ON G.MaGH=S.MaGH
	JOIN NhaCC N ON N.MaNCC=S.MaNCC
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC SanPham_GetByID
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.*, L.TenLoai, G.TenGH, N.TenNCC
    FROM SanPham S
	JOIN LoaiSP L ON L.MaLoai=S.MaLoai
	JOIN GianHang G ON G.MaGH=S.MaGH
	JOIN NhaCC N ON N.MaNCC=S.MaNCC
    WHERE S.MaSP = @MaSP;
END;
GO

---------------------------------------------------------
-- ========== SEARCH (tùy chọn, cho MVC lọc dữ liệu) ==========
---------------------------------------------------------
CREATE OR ALTER PROC SanPham_Search
(
    @Keyword NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.*, L.TenLoai, G.TenGH, N.TenNCC
    FROM SanPham S
	JOIN LoaiSP L ON L.MaLoai=S.MaLoai
	JOIN GianHang G ON G.MaGH=S.MaGH
	JOIN NhaCC N ON N.MaNCC=S.MaNCC
    WHERE S.TenSP LIKE N'%' + @Keyword + '%'
       OR L.TenLoai LIKE N'%' + @Keyword + '%'
       OR N.TenNCC LIKE N'%' + @Keyword + '%';
END;
GO

-- Kiểm tra tồn tại
CREATE OR ALTER PROC SanPham_Exists
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS ExistsCount
    FROM SanPham
    WHERE MaSP = @MaSP;
END;
GO

-- Search & Filter
CREATE OR ALTER PROCEDURE SanPham_SearchFilter
    @Search NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @TenLoai VARCHAR(10) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartRow INT = (@PageNumber - 1) * @PageSize;

    SELECT S.*, L.TenLoai, G.TenGH, N.TenNCC
    FROM SanPham S
	JOIN LoaiSP L ON L.MaLoai=S.MaLoai
	JOIN GianHang G ON G.MaGH=S.MaGH
	JOIN NhaCC N ON N.MaNCC=S.MaNCC
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR G.TenGH LIKE '%' + @Search + '%'
            OR S.TenSP LIKE '%' + @Search + '%'
            OR S.MaSP LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE '%' + @Search + '%'
            OR L.TenLoai LIKE '%' + @Search + '%')
        AND (@TrangThai IS NULL OR S.TrangThai=@TrangThai)
        AND (@TenLoai IS NULL OR L.MaLoai = @TenLoai)
    ORDER BY S.MaSP ASC 
    OFFSET @StartRow ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- count
CREATE OR ALTER PROC SanPham_Count
     @Search NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @TenLoai VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS TotalRecords
    FROM SanPham S
	JOIN LoaiSP L ON L.MaLoai=S.MaLoai
	JOIN GianHang G ON G.MaGH=S.MaGH
	JOIN NhaCC N ON N.MaNCC=S.MaNCC
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR G.TenGH LIKE '%' + @Search + '%'
            OR S.TenSP LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE '%' + @Search + '%'
            OR L.TenLoai LIKE '%' + @Search + '%')
        AND (@TrangThai IS NULL OR S.TrangThai=@TrangThai)
        AND (@TenLoai IS NULL OR L.MaLoai = @TenLoai);
END;
GO