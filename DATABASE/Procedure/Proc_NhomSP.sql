-- Insert
CREATE PROC sp_NhomSP_Insert
(
    @TenNhom NVARCHAR(100),
    @MoTa NVARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaNhom VARCHAR(10);
    DECLARE @Count INT;

    SELECT @Count = COUNT(*) + 1 FROM NhomSP;
    SET @MaNhom = 'NSP' + RIGHT('000' + CAST(@Count AS VARCHAR(3)), 3);

    INSERT INTO NhomSP (MaNhom, TenNhom, MoTa)
    VALUES (@MaNhom, @TenNhom, @MoTa);
END;
GO

-- Update
CREATE PROC sp_NhomSP_Update
(
    @MaNhom VARCHAR(10),
    @TenNhom NVARCHAR(100),
    @MoTa NVARCHAR(255)
)
AS
BEGIN
    UPDATE NhomSP
    SET TenNhom = @TenNhom,
        MoTa = @MoTa
    WHERE MaNhom = @MaNhom;
END;
GO

--Delete
CREATE PROC sp_NhomSP_Delete
(
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    DELETE FROM NhomSP WHERE MaNhom = @MaNhom;
END;
GO

-- GetAll
CREATE PROC sp_NhomSP_GetAll
AS
BEGIN
    SELECT * FROM NhomSP;
END;
GO

-- Get by ID
CREATE PROC sp_NhomSP_GetByID
(
    @MaNhom VARCHAR(10)
)
AS
BEGIN
    SELECT * FROM NhomSP WHERE MaNhom = @MaNhom;
END;
GO
