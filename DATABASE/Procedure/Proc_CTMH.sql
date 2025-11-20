USE DB_QLBH;
GO

-- =========================================
-- INSERT
-- =========================================
CREATE OR ALTER PROC CTMH_Insert
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng sản phẩm trong cùng đơn
    IF EXISTS (SELECT 1 FROM CTMH WHERE MaDMH = @MaDMH AND MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại trong đơn mua này.', 16, 1);
        RETURN;
    END;

    -- Thêm chi tiết đơn mua
    INSERT INTO CTMH (MaDMH, MaSP, SLM, DGM)
    VALUES (@MaDMH, @MaSP, @SLM, @DGM);
END;
GO


-- =========================================
-- UPDATE
-- =========================================
CREATE OR ALTER PROC CTMH_Update
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10),
    @SLM INT,
    @DGM MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CTMH
    SET SLM = @SLM,
        DGM = @DGM
    WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- DELETE
-- =========================================
CREATE OR ALTER PROC CTMH_Delete
(
    @MaDMH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CTMH
    WHERE MaDMH = @MaDMH AND MaSP = @MaSP;
END;
GO


-- =========================================
-- GET ALL
-- =========================================
CREATE OR ALTER PROC CTMH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.DonGia,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC;
END;
GO

-- get all Detail
CREATE OR ALTER PROC CTMH_GetAll_Detail AS SELECT C.MaDMH, C.MaSP, S.TenSP, C.DGM, C.SLM FROM CTMH C JOIN SanPham S ON S.MaSP=C.MaSP; 
GO

-- Get All by MaDMH
CREATE OR ALTER PROC CTMH_GetAllByDMH
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.*, sp.TenSP
    FROM CTMH c
    LEFT JOIN SanPham sp ON c.MaSP = sp.MaSP
    WHERE c.MaDMH = @MaDMH;
END;
GO


-- =========================================
-- GET BY ID
-- =========================================
CREATE OR ALTER PROC CTMH_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.DonGia,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE CT.MaDMH = @MaDMH;
END;
GO

-- Get by ID Detail
CREATE OR ALTER PROC CTMH_GetById_Detail 
	@MaDMH CHAR(11), @MaSP VARCHAR(10)
	AS SELECT C.MaDMH, C.MaSP, S.TenSP, C.DGM, C.SLM FROM CTMH C JOIN SanPham S ON S.MaSP=C.MaSP
	WHERE C.MaDMH = @MaDMH AND C.MaSP = @MaSP;
GO

-- =========================================
-- SEARCH
-- =========================================
CREATE OR ALTER PROC CTMH_Search
(
    @MaDMH CHAR(11) = NULL,
    @MaSP VARCHAR(10) = NULL,
    @TenSP NVARCHAR(100) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDMH,
        CT.MaSP,
        SP.TenSP,
        SP.DonGia,
        CT.SLM,
        CT.DGM,
        (CT.SLM * CT.DGM) AS ThanhTien,
        DMH.NgayMH,
        NCC.TenNCC
    FROM CTMH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonMuaHang DMH ON CT.MaDMH = DMH.MaDMH
    JOIN NhaCC NCC ON DMH.MaNCC = NCC.MaNCC
    WHERE
        (@MaDMH IS NULL OR CT.MaDMH = @MaDMH)
        AND (@MaSP IS NULL OR CT.MaSP = @MaSP)
        AND (@TenSP IS NULL OR SP.TenSP LIKE '%' + @TenSP + '%')
        AND (@TuNgay IS NULL OR DMH.NgayMH >= @TuNgay)
        AND (@DenNgay IS NULL OR DMH.NgayMH <= @DenNgay)
    ORDER BY DMH.NgayMH DESC;
END;
GO
