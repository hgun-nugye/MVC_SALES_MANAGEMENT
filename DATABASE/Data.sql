USE DB_QLBH;
GO

---------------------------------------------------------
-- 1 Nhà cung cấp
---------------------------------------------------------
EXEC NhaCC_Insert N'Watsons Vietnam', '0909000999', 'contact@watsons.vn', 'VN';
EXEC NhaCC_Insert N'Innisfree Co., Ltd', '0082-100-2222', 'support@innisfree.kr', 'KR';
EXEC NhaCC_Insert N'L’Oréal Paris', '0033-145-006600', 'contact@loreal.fr', 'FR';
EXEC NhaCC_Insert N'Estée Lauder Inc', '001-800-315-8393', 'info@esteelauder.com', 'US';
GO

---------------------------------------------------------
-- 2 Khách hàng
---------------------------------------------------------
EXEC KhachHang_Insert N'Nguyễn Thị Hoa', '0901234567', 'hoa.nguyen@example.com', N'Hà Nội';
EXEC KhachHang_Insert N'Lê Minh Tuấn', '0912345678', 'tuan.le@example.com', N'Hồ Chí Minh';
EXEC KhachHang_Insert N'Phạm Mai Anh', '0934567890', 'anh.pham@example.com', N'Hà Nội';
GO

---------------------------------------------------------
-- 3 Gian hàng
---------------------------------------------------------
EXEC GianHang_Insert 
    N'L''Oréal Official Store', 
    N'Gian hàng chính hãng L''Oréal tại Việt Nam', 
    '0901234567', 
    'loreal@store.vn', 
    N'123 Đường Nguyễn Trãi, Quận 5, TP. Hồ Chí Minh';

EXEC GianHang_Insert 
    N'Unilever Official Store', 
    N'Gian hàng chính hãng Unilever', 
    '0909988776', 
    'unilever@store.vn', 
    N'45 Đường Cộng Hòa, Quận Tân Bình, TP. Hồ Chí Minh';

EXEC GianHang_Insert 
    N'Senka Japan Store', 
    N'Sản phẩm chăm sóc da từ Nhật Bản', 
    '0911222333', 
    'senka@store.jp', 
    N'Tokyo, Nhật Bản';
GO

---------------------------------------------------------
-- 4 Nhóm sản phẩm
---------------------------------------------------------
EXEC Nhom_Insert N'Mỹ phẩm dưỡng da';
EXEC Nhom_Insert N'Trang điểm';
GO

---------------------------------------------------------
-- 5 Loại sản phẩm
---------------------------------------------------------
-- Giả sử nhóm 1 = Mỹ phẩm dưỡng da, nhóm 2 = Trang điểm
EXEC Loai_Insert N'Kem dưỡng da', 'NSP0000001';
EXEC Loai_Insert N'Serum', 'NSP0000001';
EXEC Loai_Insert N'Sữa rửa mặt', 'NSP0000001';
EXEC Loai_Insert N'Son môi', 'NSP0000002';
EXEC Loai_Insert N'Nước tẩy trang', 'NSP0000001';
GO

---------------------------------------------------------
-- 6 Sản phẩm
---------------------------------------------------------
EXEC SanPham_Insert N'La Roche-Posay Cicaplast Baume B5', 300000, 300000, 
    N'Kem phục hồi da, giảm kích ứng, phù hợp mọi loại da', 
    N'cicaplast_baume.jpg', N'Còn Hàng', 100, 'LSP0000001', 'NCC0000001', 'GH00000001';

EXEC SanPham_Insert N'Estée Lauder Advanced Night Repair', 2200000, 2500000, 
    N'Serum phục hồi da ban đêm, chống lão hóa nổi tiếng', 
    N'anr_serum.jpg', N'Hết Hàng', 50, 'LSP0000002', 'NCC0000004', 'GH00000002';

EXEC SanPham_Insert N'Cetaphil Gentle Skin Cleanser 125ml', 250000, 260000, 
    N'Sữa rửa mặt dịu nhẹ, dùng cho da nhạy cảm', 
    N'cetaphil_cleanser.jpg', N'Cháy Hàng', 80, 'LSP0000003', 'NCC0000001', 'GH00000001';

EXEC SanPham_Insert N'Maybelline SuperStay Matte Ink', 230000, 230000, 
    N'Son lì lâu trôi, giữ màu cả ngày', 
    N'maybelline_matteink.jpg', N'Cháy Hàng', 150, 'LSP0000004', 'NCC0000001', 'GH00000003';

EXEC SanPham_Insert N'L’Oréal Micellar Water 3-in-1', 180000, 180000, 
    N'Nước tẩy trang micellar làm sạch sâu', 
    N'loreal_micellar.jpg', N'Sắp Hết', 120, 'LSP0000005', 'NCC0000003', 'GH00000002';
GO

---------------------------------------------------------
-- 7 Đơn mua hàng
---------------------------------------------------------
EXEC DonMuaHang_Insert '2025-10-19', 'NCC0000001';
EXEC DonMuaHang_Insert '2025-10-19', 'NCC0000002';
EXEC DonMuaHang_Insert '2025-10-20', 'NCC0000001';
GO

---------------------------------------------------------
-- 8 Đơn bán hàng
---------------------------------------------------------
EXEC DonBanHang_Insert '2025-10-19', 'KH00000001';
EXEC DonBanHang_Insert '2025-10-19', 'KH00000002';
EXEC DonBanHang_Insert '2025-10-20', 'KH00000001';
GO
---------------------------------------------------------
-- 9 Chi tiết đơn mua hàng
---------------------------------------------------------
EXEC CTMH_Insert 'M2510190001', 'SP00000001', 50, 250000;
EXEC CTMH_Insert 'M2510190002', 'SP00000004', 100, 180000;
EXEC CTMH_Insert 'M2510200001', 'SP00000002', 30, 1800000;
GO

---------------------------------------------------------
-- 10 Chi tiết đơn bán hàng
---------------------------------------------------------
EXEC CTBH_Insert 'B2510190001', 'SP00000001', 3, 300000;
EXEC CTBH_Insert 'B2510190002', 'SP00000005', 2, 180000;
EXEC CTBH_Insert 'B2510200001', 'SP00000002', 1, 2000000;
GO

-- TaiKhoan cho Login

EXEC TaiKhoan_Insert 'admin', '123456';
