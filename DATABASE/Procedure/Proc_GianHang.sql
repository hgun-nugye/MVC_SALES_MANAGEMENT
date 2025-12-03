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
    @DiaChiGH NVARCHAR(200),
	@MaXa SMALLINT

)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra trùng tên hoặc số điện thoại
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

    DECLARE @MaGH VARCHAR(10);
    DECLARE @MaxID INT;

    -- Lấy số lớn nhất hiện có
    SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaGH, 3, LEN(MaGH)-2) AS INT)), 0)
    FROM GianHang;

    -- Tăng lên 1
    SET @MaGH = 'GH' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

    BEGIN TRY
        INSERT INTO GianHang (MaGH, TenGH, MoTaGH, NgayTao, DienThoaiGH, EmailGH, DiaChiGH, MaXa)
        VALUES (@MaGH, @TenGH, @MoTaGH, GETDATE(), @DienThoaiGH, @EmailGH, @DiaChiGH, @MaXa);

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
    @DiaChiGH NVARCHAR(200),
	@MaXa SMALLINT

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
            DiaChiGH = @DiaChiGH,
			MaXa = @MaXa
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
    SELECT G.*, X.TenXa, T.TenTinh FROM GianHang G
	JOIN Xa X ON X.MaXa = G.MaXa
	JOIN Tinh T ON T.MaTinh = X.MaTinh
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
     SELECT G.*, X.TenXa, T.TenTinh FROM GianHang G
	JOIN Xa X ON X.MaXa = G.MaXa
	JOIN Tinh T ON T.MaTinh = X.MaTinh
	WHERE MaGH = @MaGH;
END;
GO

-- Search & Filter
CREATE OR ALTER PROCEDURE GianHang_Search
    @Search NVARCHAR(100) = NULL,
	@MaTinh SMALLINT
AS
BEGIN
    SELECT G.*, X.TenXa, T.TenTinh FROM GianHang G
	JOIN Xa X ON X.MaXa = G.MaXa
	JOIN Tinh T ON T.MaTinh = X.MaTinh
    WHERE
        (@Search IS NULL OR @Search = '' 
            OR G.MaGH LIKE '%' + @Search + '%'
            OR G.TenGH LIKE '%' + @Search + '%'
            OR G.EmailGH LIKE '%' + @Search + '%')
         AND (
            @MaTinh IS NULL 
            OR T.MaTinh LIKE '%' + @MaTinh + '%'
        )
 
END;
GO

