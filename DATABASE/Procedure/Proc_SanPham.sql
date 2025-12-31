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
	@DoiTuongSuDung NVARCHAR(MAX),
    @MaTT CHAR(3),
    @MaLoai VARCHAR(10),
    @MaHangSX CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng tên sản phẩm
    IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP)
    BEGIN
        RAISERROR(N'Tên sản phẩm đã tồn tại.', 16, 1);
        RETURN;
    END;

    DECLARE @MaSP VARCHAR(10);
    DECLARE @MaxID INT;

    -- Tạo mã tự động: SP00000001
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaSP, 3, 8) AS INT)), 0)
    FROM SanPham;

    SET @MaSP = 'SP' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    INSERT INTO SanPham
    (
        MaSP, TenSP, GiaBan, MoTaSP, AnhMH,
        ThanhPhan, CongDung, HDSD, HDBaoQuan, TrongLuong, DoiTuongSuDung,
        MaTT, MaLoai, MaHangSX
    )
    VALUES
    (
        @MaSP, @TenSP, @GiaBan, @MoTaSP, @AnhMH,
        @ThanhPhan, @CongDung, @HDSD, @HDBaoQuan, @TrongLuong, @DoiTuongSuDung,
        @MaTT, @MaLoai, @MaHangSX
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
	@DoiTuongSuDung NVARCHAR(MAX),

    @MaTT CHAR(3),
    @MaLoai VARCHAR(10),
    @MaHangSX CHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra trùng tên với sản phẩm khác (ngoại trừ chính nó)
    IF EXISTS (SELECT 1 FROM SanPham WHERE TenSP = @TenSP AND MaSP <> @MaSP)
    BEGIN
        RAISERROR(N'Tên sản phẩm đã tồn tại ở một mã khác.', 16, 1);
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
		DoiTuongSuDung = @DoiTuongSuDung,

        MaTT = @MaTT,
        MaLoai = @MaLoai,
        MaHangSX = @MaHangSX
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

    -- Kiểm tra xem sản phẩm đã có giao dịch chưa
    IF EXISTS (SELECT 1 FROM CTMH WHERE MaSP = @MaSP) OR EXISTS (SELECT 1 FROM CTBH WHERE MaSP = @MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã có phát sinh giao dịch (Mua/Bán), không thể xóa! Hãy chuyển trạng thái sang Ngừng kinh doanh.', 16, 1);
        RETURN;
    END

    DELETE FROM SanPham WHERE MaSP = @MaSP;
    PRINT N'Xóa sản phẩm thành công!';
END;
GO

CREATE OR ALTER PROC SanPham_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    WITH Inventory AS (
        SELECT 
            S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH,
            S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
			S.DoiTuongSuDung,
            S.MaTT AS OriginalMaTT, S.MaLoai, S.MaHangSX, 
            L.TenLoai,
            H.TenHangSX,
            (
                ISNULL((SELECT SUM(SLM) FROM CTMH WHERE MaSP = S.MaSP), 0) 
                - 
                ISNULL((SELECT SUM(SLB) FROM CTBH WHERE MaSP = S.MaSP), 0)
            ) AS SoLuongTon
        FROM SanPham S
        JOIN LoaiSP L ON L.MaLoai = S.MaLoai
        JOIN HangSX H ON H.MaHangSX = S.MaHangSX
    )
    SELECT 
        I.MaSP, I.TenSP, I.GiaBan, I.MoTaSP, I.AnhMH,
        I.ThanhPhan, I.DoiTuongSuDung, I.CongDung, I.HDSD, I.HDBaoQuan, I.TrongLuong,
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END AS MaTT,
        I.MaLoai, I.MaHangSX, 
        I.TenLoai,
        I.TenHangSX,
        TT.TenTT,
        I.SoLuongTon
    FROM Inventory I
    JOIN TrangThai TT ON TT.MaTT = (
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END
    )
    ORDER BY I.TenSP;
END;
GO

CREATE OR ALTER PROC SanPham_GetByID
(
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    WITH Inventory AS (
        SELECT 
            S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH, S.DoiTuongSuDung,
            S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
            S.MaTT AS OriginalMaTT, S.MaLoai, S.MaHangSX, 
            L.TenLoai,
            H.TenHangSX,
            (
                ISNULL((SELECT SUM(SLM) FROM CTMH WHERE MaSP = S.MaSP), 0) 
                - 
                ISNULL((SELECT SUM(SLB) FROM CTBH WHERE MaSP = S.MaSP), 0)
            ) AS SoLuongTon
        FROM SanPham S
        JOIN LoaiSP L ON L.MaLoai = S.MaLoai
        JOIN HangSX H ON H.MaHangSX = S.MaHangSX
        WHERE S.MaSP = @MaSP
    )
    SELECT 
        I.MaSP, I.TenSP, I.GiaBan, I.MoTaSP, I.AnhMH,I.DoiTuongSuDung,
        I.ThanhPhan, I.CongDung, I.HDSD, I.HDBaoQuan, I.TrongLuong,
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END AS MaTT,
        I.MaLoai, I.MaHangSX, 
        I.TenLoai,
        I.TenHangSX,
        TT.TenTT,
        I.SoLuongTon
    FROM Inventory I
    JOIN TrangThai TT ON TT.MaTT = (
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END
    )
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

    WITH Inventory AS (
        SELECT 
            S.MaSP, S.TenSP, S.GiaBan, S.MoTaSP, S.AnhMH, S.DoiTuongSuDung,
            S.ThanhPhan, S.CongDung, S.HDSD, S.HDBaoQuan, S.TrongLuong,
            S.MaTT AS OriginalMaTT, S.MaLoai, S.MaHangSX, 
            L.TenLoai,
            H.TenHangSX,
            (
                ISNULL((SELECT SUM(SLM) FROM CTMH WHERE MaSP = S.MaSP), 0) 
                - 
                ISNULL((SELECT SUM(SLB) FROM CTBH WHERE MaSP = S.MaSP), 0)
            ) AS SoLuongTon
        FROM SanPham S
        JOIN LoaiSP L ON L.MaLoai = S.MaLoai
        JOIN HangSX H ON H.MaHangSX = S.MaHangSX
        WHERE
            (
                @Search IS NULL OR @Search = '' OR
                S.TenSP LIKE '%' + @Search + '%' OR
                H.TenHangSX LIKE '%' + @Search + '%' OR
                L.TenLoai LIKE '%' + @Search + '%' OR
				S.DoiTuongSuDung LIKE '%' + @Search + '%'
            )
            AND (@MaLoai IS NULL OR S.MaLoai = @MaLoai)
    )
    SELECT 
        I.MaSP, I.TenSP, I.GiaBan, I.MoTaSP, I.AnhMH, I.DoiTuongSuDung,
        I.ThanhPhan, I.CongDung, I.HDSD, I.HDBaoQuan, I.TrongLuong,
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END AS MaTT,
        I.MaLoai, I.MaHangSX, 
        I.TenLoai,
        I.TenHangSX,
        TT.TenTT,
        I.SoLuongTon
    FROM Inventory I
    JOIN TrangThai TT ON TT.MaTT = (
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END
    )
    WHERE (@MaTT IS NULL OR (
        CASE 
            WHEN I.OriginalMaTT = 'TT4' THEN 'TT4'
            WHEN I.SoLuongTon = 0 THEN 'TT3'
            WHEN I.SoLuongTon < 10 THEN 'TT2'
            ELSE 'TT1'
        END
    ) = @MaTT)
END;
GO