USE DB_QLBH;
GO

CREATE OR ALTER PROC PhanQuyen_Insert
(
    @MaVT CHAR(5),
    @MaNV VARCHAR(10)
)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM VaiTro WHERE MaVT = @MaVT)
    BEGIN
        RAISERROR(N'Vai trò không tồn tại.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
    BEGIN
        RAISERROR(N'Nhân viên không tồn tại.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM PhanQuyen WHERE MaVT = @MaVT AND MaNV = @MaNV)
    BEGIN
        RAISERROR(N'Phân quyền đã tồn tại.', 16, 1);
        RETURN;
    END;

    INSERT INTO PhanQuyen(MaVT, MaNV)
    VALUES (@MaVT, @MaNV);

    PRINT N'Phân quyền thành công!';
END;
GO

CREATE OR ALTER PROC PhanQuyen_Update
(
    @MaNV VARCHAR(10),    
    @OldMaVT CHAR(5),      
    @NewMaVT CHAR(5)       
)
AS
BEGIN
    SET NOCOUNT ON;
	
	IF EXISTS (SELECT 1 FROM PhanQuyen WHERE MaVT = @NewMaVT AND MaNV = @MaNV)
    BEGIN
        RAISERROR(N'Phân quyền đã tồn tại.', 16, 1);
        RETURN;
    END;
    UPDATE PhanQuyen
    SET MaVT = @NewMaVT
    WHERE MaNV = @MaNV AND MaVT = @OldMaVT;

    PRINT N'Cập nhật phân quyền thành công!';
END;
GO

CREATE OR ALTER PROC PhanQuyen_Delete
(
    @MaVT CHAR(5),
    @MaNV VARCHAR(10)
)
AS
BEGIN
    DELETE FROM PhanQuyen
    WHERE MaVT = @MaVT AND MaNV = @MaNV;

    PRINT N'Hủy phân quyền thành công!';
END;
GO

CREATE OR ALTER PROC PhanQuyen_GetByNhanVien
(
    @MaNV VARCHAR(10)
)
AS
BEGIN
    SELECT pq.MaNV, vt.MaVT, vt.TenVT
    FROM PhanQuyen pq
    JOIN VaiTro vt ON pq.MaVT = vt.MaVT
    WHERE pq.MaNV = @MaNV;
END;
GO
