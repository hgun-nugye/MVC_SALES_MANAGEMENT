USE DB_QLBH;
GO

CREATE TYPE dbo.CTMH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLM INT,
    DGM MONEY
);
GO

---------------------------------------------------------
-- ========== INSERT ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_Insert
(
    @NgayMH DATE,
    @MaNCC VARCHAR(10),
    @ChiTiet CTMH_List READONLY 
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaDMH CHAR(11);
    DECLARE @MaxNum INT;
    DECLARE @Prefix VARCHAR(8);

    -- Tạo prefix MYYMMDD (vd: M251119)
    SET @Prefix = 'M' +
                  RIGHT(CAST(YEAR(@NgayMH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

    -- Lấy số lớn nhất trong ngày và tăng lên 1
    SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDMH,4) AS INT)),0)
    FROM DonMuaHang
    WHERE CONVERT(DATE, NgayMH) = @NgayMH;

    SET @MaDMH = @Prefix + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);

    -- Thêm đơn mua hàng
    INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC)
    VALUES (@MaDMH, @NgayMH, @MaNCC);

    -- Thêm chi tiết mua hàng từ danh sách truyền vào
    INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM)
    SELECT @MaDMH, MaSP, SLM, DGM
    FROM @ChiTiet;

END;
GO


---------------------------------------------------------
-- ========== UPDATE ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_Update
(
    @MaDMH CHAR(11),
    @NgayMH DATE,
    @MaNCC VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra mã đơn tồn tại
    IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
    BEGIN
        RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra NCC tồn tại
    IF NOT EXISTS (SELECT 1 FROM NhaCC WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Mã nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;

    UPDATE DonMuaHang
    SET NgayMH = @NgayMH,
        MaNCC = @MaNCC
    WHERE MaDMH = @MaDMH;
END;
GO

---------------------------------------------------------
-- ========== DELETE ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_Delete
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
    BEGIN
        RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    DELETE FROM DonMuaHang WHERE MaDMH = @MaDMH;
END;
GO

---------------------------------------------------------
-- ========== GET ALL ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    ORDER BY DMH.NgayMH DESC;
END;
GO

---------------------------------------------------------
-- ========== GET BY ID ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE DMH.MaDMH = @MaDMH;
END;
GO

-- get By ID Detail
CREATE OR ALTER PROC DonMuaHang_GetById_Detail 
	@MaDMH CHAR(11)
AS SELECT D.MaDMH, D.NgayMH, D.MaNCC, N.TenNCC AS TenNCC, C.SLM as SLM, C.DGM as DGM,S.MaSP, S.TenSP FROM DonMuaHang D 
		join NhaCC N ON N.MaNCC=D.MaNCC 
		join CTMH C on C.MaDMH=D.MaDMH
		join SanPham S on S.MaSP=C.MaSP
		WHERE D.MaDMH=@MaDMH;
GO

-- Search Filter
CREATE OR ALTER PROC DonMuaHang_SearchFilter
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT D.*, N.TenNCC
    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC= D.MaNCC
    WHERE
        -- SEARCH
        (
            @Search IS NULL OR @Search = '' 
            OR D.MaDMH LIKE '%' + @Search + '%'
            OR D.MaNCC LIKE '%' + @Search + '%'
            OR N.TenNCC LIKE N'%' + @Search + '%'
            OR CONVERT(VARCHAR(10), D.NgayMH, 103) LIKE '%' + @Search + '%'
        )
        -- FILTER MONTH
        AND (@Month IS NULL OR MONTH(D.NgayMH) = @Month)
        -- FILTER YEAR
        AND (@Year IS NULL OR YEAR(D.NgayMH) = @Year)
    ORDER BY D.NgayMH DESC;
END;
GO

-- Search & Filter
CREATE OR ALTER PROCEDURE DonMuaHang_SearchFilter
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartRow INT = (@PageNumber - 1) * @PageSize;

    SELECT D.*, N.TenNCC
    FROM DonMuaHang D
	JOIN NhaCC N ON N.MaNCC=D.MaNCC
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR D.MaDMH LIKE '%' + @Search + '%'
            OR N.MaNCC LIKE '%' + @Search + '%'
			OR N.TenNCC LIKE N'%' + @Search + '%'
        AND (@Month IS NULL OR MONTH(D.NgayMH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayMH) = @Year))
    ORDER BY D.NgayMH DESC 
    OFFSET @StartRow ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- count
CREATE OR ALTER PROC DonMuaHang_Count
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM DonMuaHang D
	JOIN NhaCC N ON N.MaNCC=D.MaNCC
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR D.MaDMH LIKE '%' + @Search + '%'
            OR N.MaNCC LIKE '%' + @Search + '%'
			OR N.TenNCC LIKE N'%' + @Search + '%'
        AND (@Month IS NULL OR MONTH(D.NgayMH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayMH) = @Year));
END;
GO