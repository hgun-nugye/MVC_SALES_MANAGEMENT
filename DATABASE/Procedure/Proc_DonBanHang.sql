USE DB_QLBH;
GO

-- =========================
-- TYPE: danh sách chi tiết bán hàng
-- =========================
CREATE TYPE dbo.CTBH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLB INT,
    DGB DECIMAL(18,2)
);
GO

-- =========================
-- Thêm đơn bán hàng
-- =========================
CREATE OR ALTER PROC DonBanHang_Insert
(
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @DiaChiDBH NVARCHAR(255),
    @MaXa CHAR(5),
    @MaTTBH CHAR(3),
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
        DECLARE @Prefix CHAR(7);

        -- Tạo prefix: BYYMMDD
        -- Prefix: BYYMMDD
		SET @Prefix = 'B'
			+ RIGHT(CAST(YEAR(@NgayBH) AS CHAR(4)), 2)
			+ RIGHT('0' + CAST(MONTH(@NgayBH) AS VARCHAR(2)), 2)
			+ RIGHT('0' + CAST(DAY(@NgayBH) AS VARCHAR(2)), 2);

		SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDBH,4) AS INT)), 0)
		FROM DonBanHang
		WHERE MaDBH LIKE @Prefix + '%';

		SET @MaDBH = @Prefix 
           + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);


        -- Thêm đơn bán hàng
        INSERT INTO DonBanHang
        (MaDBH, NgayBH, MaKH, DiaChiDBH, MaXa, MaTTBH)
        VALUES
        (@MaDBH, @NgayBH, @MaKH, @DiaChiDBH, @MaXa, @MaTTBH);

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

-- =========================
-- Cập nhật đơn bán hàng
-- =========================
CREATE OR ALTER PROC DonBanHang_Update
(
    @MaDBH CHAR(11),
    @NgayBH DATE,
    @MaKH VARCHAR(10),
    @DiaChiDBH NVARCHAR(255),
    @MaXa CHAR(5),
    @MaTTBH CHAR(3),
    @ChiTiet CTBH_List READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
        BEGIN
            RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
            RETURN;
        END;

        -- Cập nhật đơn
        UPDATE DonBanHang
        SET NgayBH = @NgayBH,
            MaKH = @MaKH,
            DiaChiDBH = @DiaChiDBH,
            MaXa = @MaXa,
            MaTTBH = @MaTTBH
        WHERE MaDBH = @MaDBH;

        -- Xóa chi tiết cũ
        DELETE FROM CTBH WHERE MaDBH = @MaDBH;

        -- Thêm chi tiết mới
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

-- =========================
-- Xóa đơn bán hàng
-- =========================
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

        IF NOT EXISTS (SELECT 1 FROM DonBanHang WHERE MaDBH = @MaDBH)
        BEGIN
            RAISERROR(N'Mã đơn bán hàng không tồn tại!', 16, 1);
            RETURN;
        END;

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

-- =========================
-- Lấy danh sách đơn bán hàng
-- =========================
CREATE OR ALTER PROC DonBanHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        D.MaDBH,
        D.NgayBH,
        D.MaKH,
        K.TenKH,
        D.DiaChiDBH,
        X.TenXa,
        T.TenTinh,
        D.MaTTBH,
        TT.TenTTBH,

        STRING_AGG(C.MaSP, ', ') AS MaSP,
        STRING_AGG(S.TenSP, N', ') AS TenSP,

        ISNULL(SUM(C.SLB * C.DGB), 0) AS TongTien
    FROM DonBanHang D
    JOIN KhachHang K ON K.MaKH = D.MaKH
    JOIN TrangThaiBH TT ON TT.MaTTBH = D.MaTTBH
    JOIN Xa X ON X.MaXa = D.MaXa
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    LEFT JOIN CTBH C ON C.MaDBH = D.MaDBH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP
    GROUP BY 
        D.MaDBH, D.NgayBH, D.MaKH, K.TenKH,
        D.DiaChiDBH, X.TenXa, T.TenTinh,
        D.MaTTBH, TT.TenTTBH
    ORDER BY D.NgayBH DESC, D.MaDBH DESC;
END;
GO

-- =========================
-- Lấy đơn bán hàng theo mã
-- =========================
CREATE OR ALTER PROC DonBanHang_GetByID
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        DBH.*,
        KH.TenKH,
        TT.TenTTBH,
        CT.MaSP,
        SP.TenSP,
        CT.SLB,
        CT.DGB,
        X.TenXa,
        T.TenTinh,
        (CT.SLB * CT.DGB) AS ThanhTien
    FROM DonBanHang DBH
    JOIN KhachHang KH ON KH.MaKH = DBH.MaKH
    JOIN TrangThaiBH TT ON TT.MaTTBH = DBH.MaTTBH
    JOIN CTBH CT ON CT.MaDBH = DBH.MaDBH
    JOIN SanPham SP ON SP.MaSP = CT.MaSP
    JOIN Xa X ON X.MaXa = DBH.MaXa
    JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE DBH.MaDBH = @MaDBH;
END;
GO

-- =========================
-- Tìm kiếm đơn bán hàng
-- =========================
CREATE OR ALTER PROC DonBanHang_Search
(
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL,
    @MaTTBH CHAR(3) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        D.MaDBH,
        D.NgayBH,
        D.MaKH,
        K.TenKH,
        D.DiaChiDBH,
        D.MaTTBH,
        TT.TenTTBH,
        X.MaXa,
        X.TenXa,
        T.MaTinh,
        T.TenTinh,
        STRING_AGG(C.MaSP, ', ') AS MaSP,
        STRING_AGG(S.TenSP, N', ') AS TenSP,
        ISNULL(SUM(C.SLB * C.DGB), 0) AS TongTien
    FROM DonBanHang D
    JOIN KhachHang K ON K.MaKH = D.MaKH
    JOIN TrangThaiBH TT ON TT.MaTTBH = D.MaTTBH
    LEFT JOIN CTBH C ON C.MaDBH = D.MaDBH
    LEFT JOIN SanPham S ON S.MaSP = C.MaSP
    LEFT JOIN Xa X ON X.MaXa = D.MaXa
    LEFT JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (@Search IS NULL OR @Search = ''
            OR D.MaDBH LIKE '%' + @Search + '%'
            OR K.TenKH LIKE N'%' + @Search + '%'
        )
        AND (@Month IS NULL OR MONTH(D.NgayBH) = @Month)
        AND (@Year IS NULL OR YEAR(D.NgayBH) = @Year)
        AND (@MaTTBH IS NULL OR D.MaTTBH = @MaTTBH)
    GROUP BY
        D.MaDBH, D.NgayBH, D.MaKH, K.TenKH,
        D.DiaChiDBH, D.MaTTBH, TT.TenTTBH,
        X.MaXa, T.MaTinh, T.TenTinh, X.TenXa
    ORDER BY D.NgayBH ASC
END;
GO


CREATE OR ALTER PROCEDURE DonBanHang_Confirm
    @MaDBH CHAR(11)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE DonBanHang
    SET MaTTBH = 'DXN'
    WHERE RTRIM(MaDBH) = RTRIM(@MaDBH)
      AND RTRIM(MaTTBH) = 'CHO';

    IF @@ROWCOUNT = 0
    BEGIN
        THROW 50001, N'Đơn hàng không ở trạng thái chờ xác nhận', 1;
    END
END
GO

CREATE OR ALTER PROCEDURE DonBanHang_Cancel
    @MaDBH CHAR(11)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE DonBanHang
    SET MaTTBH = 'HUY'
    WHERE MaDBH = @MaDBH
      AND MaTTBH IN ('CHO', 'DXN'); -- cho phép hủy đơn mới / đã xác nhận
END
GO
