-- Quốc gia
INSERT INTO QuocGia VALUES
('VN', N'Việt Nam'),
('US', N'Hoa Kỳ'),
('KR', N'Hàn Quốc'),
('JP', N'Nhật Bản'),
('FR', N'Pháp');

-- Tỉnh
INSERT INTO Tinh VALUES
('VN-HN', N'Hà Nội', 'VN'),
('VN-HCM', N'Hồ Chí Minh', 'VN'),
('KR-11', N'Seoul', 'KR'),
('FR-75', N'Paris', 'FR'),
('US-CA', N'California', 'US');

-- Xã
INSERT INTO Xa VALUES
('010101', N'Phúc Xá', 'VN-HN'),
('010102', N'Trúc Bạch', 'VN-HN'),
('020101', N'Bến Nghé', 'VN-HCM'),
('020102', N'Đa Kao', 'VN-HCM');

-- Nhóm & Loại sản phẩm
EXEC sp_NhomSP_Insert N'Mỹ phẩm dưỡng da', N'Sản phẩm chăm sóc, bảo vệ và làm đẹp da';
EXEC sp_NhomSP_Insert N'Trang điểm', N'Sản phẩm trang điểm cho môi, mắt, da mặt';
EXEC sp_LoaiSP_Insert N'Kem dưỡng da', 'NSP001';
EXEC sp_LoaiSP_Insert N'Serum', 'NSP001';
EXEC sp_LoaiSP_Insert N'Sữa rửa mặt', 'NSP001';
EXEC sp_LoaiSP_Insert N'Son môi', 'NSP002';
EXEC sp_LoaiSP_Insert N'Nước tẩy trang', 'NSP001';

-- Nhà cung cấp
EXEC sp_NhaCC_Insert N'Watsons Vietnam', '0909000999', 'contact@watsons.vn', '020101', 'VN';
EXEC sp_NhaCC_Insert N'Innisfree Co., Ltd', '0082-100-2222', 'support@innisfree.kr', NULL, 'KR';
EXEC sp_NhaCC_Insert N'L’Oréal Paris', '0033-145-006600', 'contact@loreal.fr', NULL, 'FR';
EXEC sp_NhaCC_Insert N'Estée Lauder Inc', '001-800-315-8393', 'info@esteelauder.com', NULL, 'US';

-- Sản phẩm
EXEC sp_SanPham_Insert N'La Roche-Posay Cicaplast Baume B5', 300000, 300000, 
    N'Kem phục hồi da, giảm kích ứng, phù hợp mọi loại da', 
    N'cicaplast_baume.jpg', N'Bán chạy', 100, 'LSP001', 'NCC003';

EXEC sp_SanPham_Insert N'Estée Lauder Advanced Night Repair', 2200000, 2500000, 
    N'Serum phục hồi da ban đêm, chống lão hóa nổi tiếng', 
    N'anr_serum.jpg', N'Cao cấp', 50, 'LSP002', 'NCC004';

EXEC sp_SanPham_Insert N'Cetaphil Gentle Skin Cleanser 125ml', 250000, 260000, 
    N'Sữa rửa mặt dịu nhẹ, dùng cho da nhạy cảm', 
    N'cetaphil_cleanser.jpg', N'Ổn định', 80, 'LSP003', 'NCC001';

EXEC sp_SanPham_Insert N'Maybelline SuperStay Matte Ink', 230000, 230000, 
    N'Son lì lâu trôi, giữ màu cả ngày', 
    N'maybelline_matteink.jpg', N'Mới', 150, 'LSP004', 'NCC001';

EXEC sp_SanPham_Insert N'L’Oréal Micellar Water 3-in-1', 180000, 180000, 
    N'Nước tẩy trang micellar làm sạch sâu', 
    N'loreal_micellar.jpg', N'Bán chạy', 120, 'LSP005', 'NCC003';

-- Khuyến mãi
EXEC sp_KhuyenMai_Insert N'Khuyến mãi hè 2025', N'Giảm 15% cho toàn bộ mỹ phẩm dưỡng da', 
15, '2025-06-01', '2025-06-15', N'Không áp dụng sản phẩm đã giảm giá', 1;

EXEC sp_KhuyenMai_Insert N'MUA 2 TẶNG 1 Serum', N'Mua 2 serum bất kỳ tặng 1 mini size', 
0, '2025-07-01', '2025-07-10', N'Chỉ áp dụng nhóm Serum', 1;

-- Khách hàng & đánh giá
EXEC sp_KhachHang_Insert N'Nguyễn Thị Hoa', '0901234567', 'hoa.nguyen@example.com', N'Hà Nội';
EXEC sp_KhachHang_Insert N'Lê Minh Tuấn', '0912345678', 'tuan.le@example.com', N'Hồ Chí Minh';
EXEC sp_KhachHang_Insert N'Phạm Mai Anh', '0934567890', 'anh.pham@example.com', N'Hà Nội';

INSERT INTO DanhGia (MaSP, MaKH, SoSao, NoiDung)
VALUES
('SP001', 'KH001', 5, N'Kem dưỡng tuyệt vời, da mềm và mịn hơn.'),
('SP002', 'KH002', 4, N'Serum thấm nhanh, da sáng hơn sau 1 tuần.'),
('SP003', 'KH003', 5, N'Sữa rửa mặt dịu nhẹ, dùng hằng ngày rất ổn.'),
('SP005', 'KH001', 4, N'Nước tẩy trang làm sạch tốt, không cay mắt.');
GO

-- Giả sử đã có các đơn mua hàng
EXEC sp_DonMuaHang_Insert '2025-10-19', 'NCC001';
EXEC sp_DonMuaHang_Insert '2025-10-19', 'NCC002';
EXEC sp_DonMuaHang_Insert '2025-10-20', 'NCC001';
GO

-- Chi tiết đơn mua hàng
EXEC sp_CTMH_Insert 'M2510190001', 'SP001', 50, 250000;   -- La Roche-Posay
EXEC sp_CTMH_Insert 'M2510190002', 'SP004', 100, 180000;  -- Son Maybelline

EXEC sp_CTMH_Insert 'M2510200001', 'SP002', 30, 1800000;  -- Estee Lauder Serum
GO

-- Giả sử đã có các đơn bán hàng
EXEC sp_DonBanHang_Insert '2025-10-19', 'KH001';
EXEC sp_DonBanHang_Insert '2025-10-19', 'KH002';
EXEC sp_DonBanHang_Insert '2025-10-20', 'KH001';
GO

-- Chi tiết đơn bán hàng
EXEC sp_CTBH_Insert 'B2510190001', 'SP001', 3, 300000;  -- Kem dưỡng La Roche-Posay
EXEC sp_CTBH_Insert 'B2510190002', 'SP005', 2, 180000;  -- Nước tẩy trang L’Oréal
EXEC sp_CTBH_Insert 'B2510200001', 'SP002', 1, 2000000; -- Serum Estee Lauder
GO
