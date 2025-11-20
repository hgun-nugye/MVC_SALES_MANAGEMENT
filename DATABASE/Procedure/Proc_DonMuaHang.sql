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
    DECLARE @Count INT;
    DECLARE @Prefix VARCHAR(8);

    -- Tạo mã MYYMMDD#### (vd: M2510190001)
    SET @Prefix = 'M' +
                  RIGHT(CAST(YEAR(@NgayMH) AS VARCHAR(4)), 2) +
                  RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2) +
                  RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

    SELECT @Count = COUNT(*) + 1
    FROM DonMuaHang
    WHERE CONVERT(DATE, NgayMH) = @NgayMH;

    SET @MaDMH = @Prefix + RIGHT('0000' + CAST(@Count AS VARCHAR(4)), 4);

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
---------------------------------------------------------
-- ========== SEARCH ==========
---------------------------------------------------------
CREATE OR ALTER PROC DonMuaHang_Search
(
    @MaDMH CHAR(11) = NULL,
    @MaNCC VARCHAR(10) = NULL,
    @TenNCC NVARCHAR(100) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DMH.*, NCC.TenNCC
    FROM DonMuaHang DMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE
        (@MaDMH IS NULL OR DMH.MaDMH = @MaDMH)
        AND (@MaNCC IS NULL OR DMH.MaNCC = @MaNCC)
        AND (@TenNCC IS NULL OR NCC.TenNCC LIKE '%' + @TenNCC + '%')
        AND (@TuNgay IS NULL OR DMH.NgayMH >= @TuNgay)
        AND (@DenNgay IS NULL OR DMH.NgayMH <= @DenNgay)
    ORDER BY DMH.NgayMH DESC;
END;
GO
