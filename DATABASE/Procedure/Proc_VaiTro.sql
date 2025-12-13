USE DB_QLBH;
GO

CREATE OR ALTER PROC VaiTro_Insert
(
	 @MaVT CHAR(5),
    @TenVT VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    
	-- Kiểm tra trùng
    IF EXISTS (SELECT 1 FROM VaiTro WHERE MaVT = @MaVT)
    BEGIN
        RAISERROR(N'Vai trò đã tồn tại.', 16, 1);
        RETURN;
    END;

    INSERT INTO VaiTro(MaVT, TenVT)
    VALUES (@MaVT, @TenVT);

    PRINT N'Thêm vai trò thành công!';
END;
GO

CREATE OR ALTER PROC VaiTro_Update
(
    @MaVT CHAR(5),
    @TenVT VARCHAR(50)
)
AS
BEGIN
	
	-- Kiểm tra trùng
    IF EXISTS (SELECT 1 FROM VaiTro WHERE MaVT <> @MaVT AND TenVT = @TenVT)
    BEGIN
        RAISERROR(N'Vai trò đã tồn tại.', 16, 1);
        RETURN;
    END;

	UPDATE VaiTro
    SET TenVT = @TenVT
    WHERE MaVT = @MaVT;

    PRINT N'Cập nhật vai trò thành công!';
END;
GO

CREATE OR ALTER PROC VaiTro_Delete
(
    @MaVT CHAR(5)
)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM PhanQuyen WHERE MaVT = @MaVT)
    BEGIN
        RAISERROR(N'Vai trò đang được sử dụng.', 16, 1);
        RETURN;
    END;

    DELETE FROM VaiTro WHERE MaVT = @MaVT;
END;
GO

CREATE OR ALTER PROC VaiTro_GetAll
AS
BEGIN
    SELECT * FROM VaiTro ORDER BY MaVT;
END;
GO

CREATE OR ALTER PROC VaiTro_GetByID
(
    @MaVT CHAR(5)
)
AS
BEGIN
    SELECT * FROM VaiTro 
	WHERE MaVT = @MaVT
	ORDER BY MaVT;
END;
GO

