--Insert
CREATE PROC sp_LoaiSP_Insert
(
    @TenLSP NVARCHAR(50),
	@MaNhom NVARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaLoai VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM LoaiSP;
    SET @MaLoai = 'LSP' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO LoaiSP(MaLoai, TenLSP, MaNhom)
    VALUES (@MaLoai, @TenLSP, @MaNhom);
END;
GO

-- Update
CREATE PROC sp_LoaiSP_Update
(
    @MaLoai VARCHAR(10),
    @TenLSP NVARCHAR(50),
	@MaNhom VARCHAR(10)
)
AS
BEGIN
    UPDATE LoaiSP SET TenLSP = @TenLSP WHERE MaLoai = @MaLoai;
END;
GO

-- Delete
CREATE PROC sp_LoaiSP_Delete
(
    @MaLoai VARCHAR(10)
)
AS
BEGIN
    DELETE FROM LoaiSP WHERE MaLoai = @MaLoai;
END;
GO

-- GetAll
CREATE PROC sp_LoaiSP_GetAll
AS
BEGIN
    SELECT * FROM LoaiSP;
END;
GO

--Get by ID
CREATE PROC LoaiSP_GetByID
(
    @MaLoai VARCHAR(10)
)
AS
BEGIN
    SELECT * FROM LoaiSP WHERE MaLoai = @MaLoai;
END;
GO
