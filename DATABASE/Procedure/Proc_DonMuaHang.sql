USE DB_QLBH;
GO

CREATE TYPE CTMH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLM INT,
    DGM DECIMAL(18,2)
);
GO

CREATE OR ALTER PROC DonMuaHang_Insert
(
    @NgayMH DATE,
    @MaNCC VARCHAR(10),
    @MaNV VARCHAR(10),
	@MaTTMH CHAR(3),
    @ChiTiet CTMH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @MaDMH CHAR(11);
        DECLARE @MaxNum INT;
        DECLARE @Prefix VARCHAR(8);

        -- Tạo mã đơn: MYYMMDDxxxx
		SET @Prefix = 'B'
			+ RIGHT(CAST(YEAR(@NgayMH) AS CHAR(4)), 2)
			+ RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2)
			+ RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

		SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDMH,4) AS INT)), 0)
		FROM DonMuaHang
		WHERE MaDMH LIKE @Prefix + '%';

		SET @MaDMH = @Prefix 
           + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);


        -- Thêm đơn mua hàng
        INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC, MaNV, MaTTMH)
        VALUES (@MaDMH, @NgayMH, @MaNCC, @MaNV, @MaTTMH);

        -- Thêm chi tiết
        INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM)
        SELECT @MaDMH, MaSP, SLM, DGM
        FROM @ChiTiet;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO


CREATE OR ALTER PROC DonMuaHang_Update
(
    @MaDMH CHAR(11),
    @NgayMH DATE,
    @MaNCC VARCHAR(10),
    @MaNV VARCHAR(10),
	@MaTTMH CHAR(3),
    @ChiTiet CTMH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
        BEGIN
            RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
            RETURN;
        END;

        UPDATE DonMuaHang
        SET NgayMH = @NgayMH,
            MaNCC = @MaNCC,
            MaNV = @MaNV,
			MaTTMH = @MaTTMH
        WHERE MaDMH = @MaDMH;

        -- Xóa chi tiết cũ
        DELETE FROM CTMH WHERE MaDMH = @MaDMH;

        -- Thêm chi tiết mới
        INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM)
        SELECT @MaDMH, MaSP, SLM, DGM FROM @ChiTiet;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC DonMuaHang_Delete
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM DonMuaHang WHERE MaDMH = @MaDMH)
        BEGIN
            RAISERROR(N'Mã đơn mua hàng không tồn tại!', 16, 1);
            RETURN;
        END;

        DELETE FROM CTMH WHERE MaDMH = @MaDMH;
        DELETE FROM DonMuaHang WHERE MaDMH = @MaDMH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROC DonMuaHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        D.MaDMH,
        D.NgayMH,
        D.MaNCC,
		D.MaTTMH,
        N.TenNCC,
        D.MaNV,
        NV.TenNV,
		TT.TenTTMH,
        -- Gộp danh sách sản phẩm thành chuỗi
        STRING_AGG(C.MaSP, ', ') AS MaSP, 
        STRING_AGG(S.TenSP, N', ') AS TenSP,

        -- Thủ thuật map với Class C# (ThanhTien = SL * DG)
        1 AS SLM, 
        ISNULL(SUM(C.SLM * C.DGM), 0) AS DGM -- Tổng tiền đơn hàng

    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC = D.MaNCC
    JOIN NhanVien NV ON NV.MaNV = D.MaNV
	JOIN TrangThaiMH TT ON TT.MaTTMH = D.MaTTMH
    LEFT JOIN CTMH C ON C.MaDMH = D.MaDMH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP

    GROUP BY D.MaDMH, D.NgayMH, D.MaNCC, N.TenNCC, D.MaNV, NV.TenNV, D.MaTTMH, TT. TenTTMH
    ORDER BY D.NgayMH DESC, D.MaDMH DESC;
END;
GO

CREATE OR ALTER PROC DonMuaHang_GetByID
(
    @MaDMH CHAR(11)
)
AS
BEGIN
    SELECT 
        D.MaDMH,
        D.NgayMH,
        D.MaNCC,
		D.MaTTMH,
		TT.TenTTMH,
        N.TenNCC,
        D.MaNV,
        NV.TenNV,
        S.MaSP,
        S.TenSP,
        C.DGM,
        C.SLM
    FROM DonMuaHang D
    LEFT JOIN NhaCC N ON N.MaNCC = D.MaNCC
    LEFT JOIN NhanVien NV ON NV.MaNV = D.MaNV
    LEFT JOIN CTMH C ON C.MaDMH = D.MaDMH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP
	LEFT JOIN TrangThaiMH TT ON TT.MaTTMH = D.MaTTMH
    WHERE D.MaDMH = @MaDMH;
END;
GO

CREATE OR ALTER PROC DonMuaHang_Search
(
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL,
    @MaTTMH CHAR(3) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        D.MaDMH,
        D.NgayMH,
        D.MaNCC,
        N.TenNCC,
        D.MaNV,
		D.MaTTMH, 
		TT.TenTTMH,
        NV.TenNV,
        STRING_AGG(C.MaSP, ', ') AS MaSP, 
        STRING_AGG(S.TenSP, N', ') AS TenSP,
        ISNULL(SUM(C.SLM * C.DGM), 0) AS TongTien
    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC = D.MaNCC
    JOIN NhanVien NV ON NV.MaNV = D.MaNV
	JOIN TrangThaiMH TT ON TT.MaTTMH = D.MaTTMH
    LEFT JOIN CTMH C ON C.MaDMH = D.MaDMH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP
    WHERE
        (
            @Search IS NULL OR @Search = '' OR
            D.MaDMH LIKE '%' + @Search + '%' OR
            N.TenNCC LIKE N'%' + @Search + '%' OR
            D.MaNCC LIKE '%' + @Search + '%' OR
            NV.TenNV LIKE N'%' + @Search + '%'
        )
        AND (@Month IS NULL OR MONTH(D.NgayMH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayMH) = @Year)
		AND (@MaTTMH IS NULL OR D.MaTTMH = @MaTTMH)
    GROUP BY D.MaDMH, D.NgayMH, D.MaDMH, D.NgayMH, D.MaNCC, N.TenNCC, D.MaNV, NV.TenNV, D.MaTTMH, TT.TenTTMH
	ORDER BY D.NgayMH ASC
END;
GO