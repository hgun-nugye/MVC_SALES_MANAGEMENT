USE DB_QLBH;
GO

-- =============================================
-- ================ INSERT =====================
-- =============================================
CREATE OR ALTER PROC KhuyenMai_Insert
(
    @TenKM NVARCHAR(100),
    @MoTaKM NVARCHAR(MAX) = NULL,
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255) = NULL,
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra trùng tên khuyến mãi
        IF EXISTS (SELECT 1 FROM KhuyenMai WHERE TenKM = @TenKM)
        BEGIN
            RAISERROR(N'Tên khuyến mãi đã tồn tại!', 16, 1);
            RETURN;
        END

        DECLARE @MaKM VARCHAR(10);
        DECLARE @MaxID INT;

        -- Lấy số lớn nhất hiện có
        SELECT @MaxID = ISNULL(MAX(CAST(SUBSTRING(MaKM, 3, LEN(MaKM)-2) AS INT)), 0)
        FROM KhuyenMai;

        -- Tăng lên 1
        SET @MaKM = 'KM' + RIGHT('00000000' + CAST(@MaxID + 1 AS VARCHAR(8)), 8);

        INSERT INTO KhuyenMai(MaKM, TenKM, MoTaKM, GiaTriKM, NgayBatDau, NgayKetThuc, DieuKienApDung, TrangThai)
        VALUES (@MaKM, @TenKM, @MoTaKM, @GiaTriKM, @NgayBatDau, @NgayKetThuc, @DieuKienApDung, @TrangThai);

        PRINT N'Thêm khuyến mãi thành công!';
        SELECT @MaKM AS MaKhuyenMaiMoi;
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- =============================================
-- ================ UPDATE =====================
-- =============================================
CREATE OR ALTER PROC KhuyenMai_Update
(
    @MaKM VARCHAR(10),
    @TenKM NVARCHAR(100),
    @MoTaKM NVARCHAR(MAX),
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255),
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhuyenMai WHERE MaKM = @MaKM)
        BEGIN
            RAISERROR(N'Mã khuyến mãi không tồn tại!', 16, 1);
            RETURN;
        END

        UPDATE KhuyenMai
        SET TenKM = @TenKM,
            MoTaKM = @MoTaKM,
            GiaTriKM = @GiaTriKM,
            NgayBatDau = @NgayBatDau,
            NgayKetThuc = @NgayKetThuc,
            DieuKienApDung = @DieuKienApDung,
            TrangThai = @TrangThai
        WHERE MaKM = @MaKM;

        PRINT N'Cập nhật khuyến mãi thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO


-- =============================================
-- ================ DELETE =====================
-- =============================================
CREATE OR ALTER PROC KhuyenMai_Delete
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhuyenMai WHERE MaKM = @MaKM)
        BEGIN
            RAISERROR(N'Mã khuyến mãi không tồn tại!', 16, 1);
            RETURN;
        END

        DELETE FROM KhuyenMai WHERE MaKM = @MaKM;

        PRINT N'Xóa khuyến mãi thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO


-- =============================================
-- ================ GET ALL ====================
-- =============================================
CREATE OR ALTER PROC KhuyenMai_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaKM, TenKM, MoTaKM, GiaTriKM, NgayBatDau, NgayKetThuc, 
           DieuKienApDung, TrangThai
    FROM KhuyenMai
    ORDER BY NgayBatDau DESC;
END;
GO


-- =============================================
-- ================ GET BY ID ==================
-- =============================================
CREATE OR ALTER PROC KhuyenMai_GetByID
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaKM, TenKM, MoTaKM, GiaTriKM, NgayBatDau, NgayKetThuc, 
           DieuKienApDung, TrangThai
    FROM KhuyenMai
    WHERE MaKM = @MaKM;
END;
GO

-- ============================
-- SEARCH
-- ============================
CREATE OR ALTER PROC KhuyenMai_Search
(
    @Keyword NVARCHAR(200) = NULL,        -- Từ khoá tìm trong Tên, Mô tả, Điều kiện
    @TrangThai BIT = NULL,                -- Lọc trạng thái (NULL = không lọc)
    @TuNgay DATETIME = NULL,              -- Lọc từ ngày bắt đầu
    @DenNgay DATETIME = NULL              -- Lọc đến ngày kết thúc
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MaKM,
        TenKM,
        MoTaKM,
        GiaTriKM,
        NgayBatDau,
        NgayKetThuc,
        DieuKienApDung,
        TrangThai
    FROM KhuyenMai
    WHERE
        -- Tìm kiếm theo keyword
        (
            @Keyword IS NULL OR
            TenKM LIKE '%' + @Keyword + '%' OR
            MoTaKM LIKE '%' + @Keyword + '%' OR
            DieuKienApDung LIKE '%' + @Keyword + '%'
        )
        AND
        -- Lọc trạng thái
        (@TrangThai IS NULL OR TrangThai = @TrangThai)
        AND
        -- Lọc theo ngày
        (@TuNgay IS NULL OR NgayBatDau >= @TuNgay)
        AND
        (@DenNgay IS NULL OR NgayKetThuc <= @DenNgay)
    ORDER BY NgayBatDau DESC;
END;
GO
