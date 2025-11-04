USE DB_QLBH;
GO

-- =============================
-- NHÀ CUNG CẤP
-- =============================
INSERT INTO NhaCC VALUES
('NCC0000001', N'Công ty TNHH Mỹ Phẩm Senka', '0901234567', 'contact@senka.vn', N'123 Lê Lợi, Q.1, TP.HCM'),
('NCC0000002', N'Công ty TNHH L''Oréal Việt Nam', '0902345678', 'info@loreal.vn', N'45 Nguyễn Huệ, Q.1, TP.HCM'),
('NCC0000003', N'Công ty TNHH Unilever Việt Nam', '0903456789', 'sales@unilever.com', N'12 Tôn Đức Thắng, Q.1, TP.HCM');
GO

-- =============================
-- KHÁCH HÀNG
-- =============================
INSERT INTO KhachHang VALUES
('KH00000001', N'Nguyễn Văn An', '0911111111', 'an.nguyen@gmail.com', N'12 Lý Thường Kiệt, Hà Nội'),
('KH00000002', N'Trần Thị Bình', '0922222222', 'binh.tran@gmail.com', N'45 CMT8, TP.HCM'),
('KH00000003', N'Lê Minh Châu', '0933333333', 'chau.le@gmail.com', N'78 Nguyễn Trãi, Đà Nẵng');
GO

-- =============================
-- NHÓM SẢN PHẨM
-- =============================
INSERT INTO NhomSP VALUES
('NSP000000000001', N'Mỹ phẩm chăm sóc da'),
('NSP000000000002', N'Sản phẩm trang điểm');
GO

-- =============================
-- LOẠI SẢN PHẨM
-- =============================
INSERT INTO LoaiSP VALUES
('LSP00000001', N'Sữa rửa mặt', 'NSP000000000001'),
('LSP00000002', N'Kem chống nắng', 'NSP000000000001'),
('LSP00000003', N'Son môi', 'NSP000000000002');
GO

-- =============================
-- SẢN PHẨM
-- =============================
INSERT INTO SanPham VALUES
('SP00000001', N'Sữa rửa mặt Senka Perfect Whip', 65000, 89000, N'Làm sạch sâu, tạo bọt mịn', 'senka_whip.jpg', N'Còn Hàng', 120, 'LSP00000001', 'NCC0000001'),
('SP00000002', N'Kem chống nắng L''Oréal UV Defender', 180000, 250000, N'Chống nắng SPF50+, dưỡng da', 'loreal_uv.jpg', N'Còn Hàng', 90, 'LSP00000002', 'NCC0000002'),
('SP00000003', N'Son dưỡng môi Vaseline Lip Therapy', 35000, 55000, N'Dưỡng ẩm, chống nứt nẻ môi', 'vaseline_lip.jpg', N'Sắp Hết', 25, 'LSP00000003', 'NCC0000003');
GO

-- =============================
-- ĐƠN MUA HÀNG (nhập từ NCC)
-- =============================
INSERT INTO DonMuaHang VALUES
('M2510010001', '2025-10-01', 'NCC0000001'),
('M2510020001', '2025-10-02', 'NCC0000002');
GO

-- =============================
-- CHI TIẾT MUA HÀNG
-- =============================
INSERT INTO CTMH VALUES
('M2510010001', 'SP00000001', 200, 65000),
('M2510020001', 'SP00000002', 100, 180000);
GO

-- =============================
-- ĐƠN BÁN HÀNG
-- =============================
INSERT INTO DonBanHang VALUES
('B2510200001', '2025-10-20', 'KH00000001'),
('B2510210001', '2025-10-21', 'KH00000002');
GO

-- =============================
-- CHI TIẾT BÁN HÀNG
-- =============================
INSERT INTO CTBH VALUES
('B2510200001', 'SP00000001', 2, 89000),
('B2510200001', 'SP00000003', 1, 55000),
('B2510210001', 'SP00000002', 1, 250000);
GO

-- =============================
-- KHUYẾN MÃI
-- =============================
INSERT INTO KhuyenMai VALUES
('KM00000001', N'Giảm giá mùa lễ hội', N'Giảm 10% cho đơn hàng trên 300.000đ', 10, '2025-10-01', '2025-12-31', N'Tổng tiền > 300000', 1),
('KM00000002', N'Mua 2 tặng 1', N'Áp dụng cho sản phẩm Senka', 33, '2025-09-15', '2025-11-15', N'Sản phẩm Senka', 1);
GO

-- =============================
-- ĐÁNH GIÁ SẢN PHẨM
-- =============================
INSERT INTO DanhGia (MaSP, MaKH, SoSao, NoiDung) VALUES
('SP00000001', 'KH00000001', 5, N'Sản phẩm tạo bọt mịn, rất dễ chịu'),
('SP00000002', 'KH00000002', 4, N'Dưỡng da tốt nhưng giá hơi cao'),
('SP00000003', 'KH00000003', 5, N'Son thơm, mềm môi, rất ưng ý');
GO

-- =============================
-- TÀI KHOẢN NGƯỜI DÙNG
-- =============================
INSERT INTO TaiKhoan VALUES
('admin', N'admin123', N'Admin', 1, GETDATE(), NULL),
('kh_an', N'123456', N'KhachHang', 1, GETDATE(), 'KH00000001'),
('kh_binh', N'123456', N'KhachHang', 1, GETDATE(), 'KH00000002'),
('kh_chau', N'123456', N'KhachHang', 1, GETDATE(), 'KH00000003');
GO
