-- Insert
CREATE PROC sp_KhuyenMai_Insert
(
    @TenKM NVARCHAR(100),
    @MoTa TEXT,
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255),
    @TrangThai BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaKM VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM KhuyenMai;
    SET @MaKM = 'KM' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO KhuyenMai(MaKM, TenKM, MoTa, GiaTriKM, NgayBatDau, NgayKetThuc, DieuKienApDung, TrangThai)
    VALUES (@MaKM, @TenKM, @MoTa, @GiaTriKM, @NgayBatDau, @NgayKetThuc, @DieuKienApDung, @TrangThai);
END;
GO

-- Update
CREATE PROC sp_KhuyenMai_Update
(
    @MaKM VARCHAR(10),
    @TenKM NVARCHAR(100),
    @MoTa NVARCHAR(500),
    @GiaTriKM DECIMAL(10,2),
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME,
    @DieuKienApDung NVARCHAR(255),
    @TrangThai BIT
)
AS
BEGIN
    UPDATE KhuyenMai
    SET TenKM = @TenKM,
        MoTa = @MoTa,
        GiaTriKM = @GiaTriKM,
        NgayBatDau = @NgayBatDau,
        NgayKetThuc = @NgayKetThuc,
        DieuKienApDung = @DieuKienApDung,
        TrangThai = @TrangThai
    WHERE MaKM = @MaKM;
END;
GO

-- Delete
CREATE PROC sp_KhuyenMai_Delete
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    DELETE FROM KhuyenMai WHERE MaKM = @MaKM;
END;
GO

-- GetAll
CREATE PROC sp_KhuyenMai_GetAll
AS
BEGIN
    SELECT * FROM KhuyenMai;
END;
GO

-- Get by ID
CREATE PROC sp_KhuyenMai_GetByID
(
    @MaKM VARCHAR(10)
)
AS
BEGIN
    SELECT * FROM KhuyenMai WHERE MaKM = @MaKM;
END;
GO
