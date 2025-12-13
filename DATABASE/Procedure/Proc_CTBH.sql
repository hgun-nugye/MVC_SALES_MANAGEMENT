USE DB_QLBH;
GO

CREATE OR ALTER PROC CTBH_Insert
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLB INT,
    @DGB MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM CTBH WHERE MaDBH=@MaDBH AND MaSP=@MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã tồn tại trong đơn hàng.',16,1);
        RETURN;
    END

    -- Thêm chi tiết đơn
    INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB)
    VALUES(@MaDBH, @MaSP, @SLB, @DGB);
END;
GO

CREATE OR ALTER PROC CTBH_Update
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10),
    @SLB INT,
    @DGB MONEY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldSLB INT;

    -- Lấy số lượng cũ
    SELECT @OldSLB = SLB FROM CTBH
    WHERE MaDBH=@MaDBH AND MaSP=@MaSP;

    IF @OldSLB IS NULL
    BEGIN
        RAISERROR(N'Dòng chi tiết không tồn tại.',16,1);
        RETURN;
    END

    -- Update chi tiết
    UPDATE CTBH
    SET SLB=@SLB, DGB=@DGB
    WHERE MaDBH=@MaDBH AND MaSP=@MaSP;
END;
GO

CREATE OR ALTER PROC CTBH_Delete
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldSLB INT;

    SELECT @OldSLB = SLB
    FROM CTBH
    WHERE MaDBH=@MaDBH AND MaSP=@MaSP;

    IF @OldSLB IS NULL
    BEGIN
        RAISERROR(N'Dòng chi tiết không tồn tại.',16,1);
        RETURN;
    END

    -- Xóa chi tiết
    DELETE FROM CTBH WHERE MaDBH=@MaDBH AND MaSP=@MaSP;
END;
GO

CREATE OR ALTER PROC CTBH_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH;
END;
GO

CREATE OR ALTER PROC CTBH_GetByIDDBH
(
    @MaDBH CHAR(11)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE CT.MaDBH = @MaDBH;
END;
GO

CREATE OR ALTER PROC CTBH_GetByID
(
    @MaDBH CHAR(11),
    @MaSP VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CT.MaDBH,
        CT.MaSP,
        SP.TenSP,
        SP.GiaBan,
        CT.SLB,
        CT.DGB,
        (CT.SLB * CT.DGB) AS ThanhTien,
        DBH.NgayBH,
        KH.TenKH
    FROM CTBH CT
    JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN DonBanHang DBH ON CT.MaDBH = DBH.MaDBH
    JOIN KhachHang KH ON DBH.MaKH = KH.MaKH
    WHERE CT.MaDBH = @MaDBH AND CT.MaSP= @MaSP;
END;
GO
