USE DB_QLBH;
GO

-- ===========================
-- INSERT
-- ===========================
CREATE OR ALTER PROC GianHang_Insert
(
    @TenGH NVARCHAR(100),
    @MoTaGH NVARCHAR(255),
    @DienThoaiGH VARCHAR(15),
    @EmailGH NVARCHAR(100),
    @DiaChiGH NVARCHAR(200)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM GianHang WHERE TenGH = @TenGH)
    BEGIN
        RAISERROR(N'Tên gian hàng đã tồn tại!', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM GianHang WHERE DienThoaiGH = @DienThoaiGH)
    BEGIN
        RAISERROR(N'Số điện thoại gian hàng đã tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @Count INT = (SELECT COUNT(*) + 1 FROM GianHang);
    DECLARE @MaGH VARCHAR(10) = 'GH' + RIGHT('00000000' + CAST(@Count AS VARCHAR(8)), 8);

    BEGIN TRY
        INSERT INTO GianHang (MaGH, TenGH, MoTaGH, NgayTao, DienThoaiGH, EmailGH, DiaChiGH)
        VALUES (@MaGH, @TenGH, @MoTaGH, GETDATE(), @DienThoaiGH, @EmailGH, @DiaChiGH);
        PRINT N'Thêm gian hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- ===========================
-- UPDATE
-- ===========================
CREATE OR ALTER PROC GianHang_Update
(
    @MaGH VARCHAR(10),
    @TenGH NVARCHAR(100),
    @MoTaGH NVARCHAR(255),
    @DienThoaiGH VARCHAR(15),
    @EmailGH NVARCHAR(100),
    @DiaChiGH NVARCHAR(200)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM GianHang WHERE MaGH = @MaGH)
    BEGIN
        RAISERROR(N'Mã gian hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM GianHang WHERE TenGH = @TenGH AND MaGH <> @MaGH)
    BEGIN
        RAISERROR(N'Tên gian hàng đã tồn tại!', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM GianHang WHERE DienThoaiGH = @DienThoaiGH AND MaGH <> @MaGH)
    BEGIN
        RAISERROR(N'Số điện thoại gian hàng đã tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE GianHang
        SET 
            TenGH = @TenGH,
            MoTaGH = @MoTaGH,
            DienThoaiGH = @DienThoaiGH,
            EmailGH = @EmailGH,
            DiaChiGH = @DiaChiGH
        WHERE MaGH = @MaGH;

        PRINT N'Cập nhật gian hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- ===========================
-- DELETE
-- ===========================
CREATE OR ALTER PROC GianHang_Delete
(
    @MaGH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM GianHang WHERE MaGH = @MaGH)
    BEGIN
        RAISERROR(N'Mã gian hàng không tồn tại!', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        DELETE FROM GianHang WHERE MaGH = @MaGH;
        PRINT N'Xóa gian hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- ===========================
-- GET ALL
-- ===========================
CREATE OR ALTER PROC GianHang_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM GianHang
    ORDER BY TenGH ASC;
END;
GO

-- ===========================
-- GET BY ID
-- ===========================
CREATE OR ALTER PROC GianHang_GetByID
(
    @MaGH VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM GianHang WHERE MaGH = @MaGH;
END;
GO

-- ===========================
-- SEARCH
-- ===========================
CREATE OR ALTER PROC GianHang_Search
(
    @Keyword NVARCHAR(200) = NULL    -- Tìm theo Tên, Mô tả, Email, SĐT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM GianHang
    WHERE
        @Keyword IS NULL
        OR TenGH LIKE '%' + @Keyword + '%'
        OR MoTaGH LIKE '%' + @Keyword + '%'
        OR EmailGH LIKE '%' + @Keyword + '%'
        OR DienThoaiGH LIKE '%' + @Keyword + '%'
    ORDER BY TenGH ASC;
END;
GO
