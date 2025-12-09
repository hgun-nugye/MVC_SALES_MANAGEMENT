USE DB_QLBH;
GO

CREATE TYPE dbo.CTMH_List AS TABLE
(
    MaSP VARCHAR(10),
    SLM INT,
    DGM MONEY
);
GO

CREATE OR ALTER PROC DonMuaHang_Insert
(
    @NgayMH DATE,
    @MaNCC VARCHAR(10),
    @MaNV VARCHAR(10),
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

        SET @Prefix = 'M' +
                      RIGHT(CAST(YEAR(@NgayMH) AS VARCHAR(4)), 2) +
                      RIGHT('0' + CAST(MONTH(@NgayMH) AS VARCHAR(2)), 2) +
                      RIGHT('0' + CAST(DAY(@NgayMH) AS VARCHAR(2)), 2);

        SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(MaDMH,4) AS INT)),0)
        FROM DonMuaHang
        WHERE NgayMH = @NgayMH;

        SET @MaDMH = @Prefix + RIGHT('0000' + CAST(@MaxNum + 1 AS VARCHAR(4)), 4);

        INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC, MaNV)
        VALUES (@MaDMH, @NgayMH, @MaNCC, @MaNV);

        INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM)
        SELECT @MaDMH, MaSP, SLM, DGM FROM @ChiTiet;

        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon + CT.SLM
        FROM SanPham SP
        JOIN @ChiTiet CT ON SP.MaSP = CT.MaSP;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO


CREATE OR ALTER PROC DonMuaHang_Update
(
    @MaDMH CHAR(11),
    @NgayMH DATE,
    @MaNCC VARCHAR(10),
    @MaNV VARCHAR(10),
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
            MaNV = @MaNV
        WHERE MaDMH = @MaDMH;

        -- Trừ tồn kho cũ
        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon - C.SLM
        FROM SanPham SP
        JOIN CTMH C ON SP.MaSP = C.MaSP
        WHERE C.MaDMH = @MaDMH;

        DELETE FROM CTMH WHERE MaDMH = @MaDMH;

        -- Thêm lại chi tiết mới
        INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM)
        SELECT @MaDMH, MaSP, SLM, DGM FROM @ChiTiet;

        -- Cộng tồn kho mới
        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon + CT.SLM
        FROM SanPham SP
        JOIN @ChiTiet CT ON SP.MaSP = CT.MaSP;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
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

        -- Trừ tồn kho
        UPDATE SP
        SET SP.SoLuongTon = SP.SoLuongTon - C.SLM
        FROM SanPham SP
        JOIN CTMH C ON C.MaSP = SP.MaSP
        WHERE C.MaDMH = @MaDMH;

        DELETE FROM CTMH WHERE MaDMH = @MaDMH;
        DELETE FROM DonMuaHang WHERE MaDMH = @MaDMH;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROC DonMuaHang_GetAll
AS
BEGIN
    SELECT 
        D.MaDMH,
        D.NgayMH,
        D.MaNCC,
        N.TenNCC,
        D.MaNV,
        NV.TenNV,
        (
            SELECT 
                C.MaSP, S.TenSP, C.SLM, C.DGM
            FROM CTMH C
            JOIN SanPham S ON S.MaSP = C.MaSP
            WHERE C.MaDMH = D.MaDMH
            FOR JSON PATH
        ) AS ChiTietJSON
    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC = D.MaNCC
    JOIN NhanVien NV ON NV.MaNV = D.MaNV
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
        N.TenNCC,
        D.MaNV,
        NV.TenNV,
        (
            SELECT 
                C.MaSP, S.TenSP, C.SLM, C.DGM
            FROM CTMH C
            JOIN SanPham S ON S.MaSP = C.MaSP
            WHERE C.MaDMH = D.MaDMH
            FOR JSON PATH
        ) AS ChiTietJSON
    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC = D.MaNCC
    JOIN NhanVien NV ON NV.MaNV = D.MaNV
    WHERE D.MaDMH = @MaDMH;
END;
GO

CREATE OR ALTER PROC DonMuaHang_Search
(
    @Search NVARCHAR(100) = NULL,
    @Month INT = NULL,
    @Year INT = NULL
)
AS
BEGIN
    SELECT 
        D.MaDMH,
        D.NgayMH,
        D.MaNCC,
        N.TenNCC,
        D.MaNV,
        NV.TenNV
    FROM DonMuaHang D
    JOIN NhaCC N ON N.MaNCC = D.MaNCC
    JOIN NhanVien NV ON NV.MaNV = D.MaNV
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
    ORDER BY D.NgayMH DESC;
END;
GO
