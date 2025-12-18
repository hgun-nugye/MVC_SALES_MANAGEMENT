USE DB_QLBH;
GO

-- Tỉnh
INSERT INTO Tinh VALUES
(1, N'Hà Nội'),
(2, N'Hồ Chí Minh'),
(3, N'Đà Nẵng');
GO

-- Xã
INSERT INTO Xa VALUES
-- Hà Nội
(101, N'Phường Hoàn Kiếm', 1),
(102, N'Phường Ba Đình', 1),
(103, N'Phường Cầu Giấy', 1),

-- Hồ Chí Minh
(201, N'Phường Bến Nghé', 2),
(202, N'Phường Tân Định', 2),
(203, N'Phường Thảo Điền', 2),

-- Đà Nẵng
(301, N'Phường Hải Châu', 3),
(302, N'Phường Thanh Khê', 3),
(303, N'Phường Sơn Trà', 3);
GO

-- Nước 
INSERT INTO Nuoc VALUES
('QG001', N'Hàn Quốc'),
('QG002', N'Nhật Bản'),
('QG003', N'Mỹ'),
('QG004', N'Việt Nam');
GO

-- Hãng
INSERT INTO HangSX VALUES
('H0001', N'Innisfree', 'QG001'),
('H0002', N'Some By Mi', 'QG001'),
('H0003', N'La Roche-Posay', 'QG003'),
('H0004', N'Cocoon', 'QG004');
GO

-- Nhà cung cấp 
INSERT INTO NhaCC VALUES
('NCC0000001', N'Guardian Vietnam', '02873007300', 'cskh@guardian.com.vn', N'20 Tràng Tiền', 101),
('NCC0000002', N'Hasaki', '18006324', 'support@hasaki.vn', N'25 Nguyễn Huệ', 201);
GO

-- Vai trò 
INSERT INTO VaiTro VALUES
('QT', N'Quản trị'),
('NV', N'Nhân viên'),
('QL', N'Quản lý');
GO

-- Nhân viên
INSERT INTO NhanVien
(MaNV, CCCD, TenNV, GioiTinh, NgaySinh, SDT, Email, NgayLam, AnhNV, DiaChiNV, MaXa, TenDNNV, MatKhauNV)
VALUES
-- Admin
('NV25121001', '001201000001', N'Nguyễn Thị Hương', 0, '1995-03-15',
 '0901234567', 'admin@mypham.vn', '2022-01-10',
 NULL, N'10 Tràng Tiền', 101,
 'admin', '123456'),

-- Nhân viên tư vấn mỹ phẩm
('NV25121002', '001201000002', N'Trần Mỹ Linh', 0, '1998-07-22',
 '0902345678', 'linh.tran@mypham.vn', '2023-03-05',
 NULL, N'25 Nguyễn Huệ', 201,
 'tuvan01', '123456'),

-- Quản lý cửa hàng
('NV25121101', '001201000003', N'Lê Hoàng Nam', 1, '1992-11-02',
 '0903456789', 'nam.le@mypham.vn', '2021-06-20',
 NULL, N'88 Hải Châu', 301,
 'quanly01', '123456');
GO

-- Phân quyền 
INSERT INTO PhanQuyen VALUES
('QT', 'NV25121001'),
('NV', 'NV25121002'),
('QL', 'NV25121101');
GO

-- Khách hàng
INSERT INTO KhachHang VALUES
('KH00000001', N'Nguyễn Thị Lan', NULL, 0, 'lan@gmail.com', '0901111222', N'10 Tràng Tiền', 'lannt', '123456', 101),
('KH00000002', N'Trần Mỹ Anh', NULL, 0, 'anh@gmail.com', '0902222333', N'15 Nguyễn Huệ', 'anhtran', '123456', 202),
('KH00000003', N'Lê Hoàng', NULL, 1, 'hoang@gmail.com', '0903333444', N'88 Hải Châu', 'lehoang', '123456', 301);
GO

-- Nhóm SP 
INSERT INTO NhomSP(MaNhom, TenNhom) VALUES
('CSDA', N'Mỹ phẩm chăm sóc da'),
('CSTOC', N'Sản phẩm chăm sóc tóc');
GO 

-- Loại SP 
INSERT INTO LoaiSP VALUES
('SRM', N'Sữa rửa mặt', 'CSDA'),
('TONER', N'Toner', 'CSDA'),
('SERUM', N'Serum', 'CSDA'),
('KDUONG', N'Kem dưỡng', 'CSDA'),
('KCN', N'Kem chống nắng', 'CSDA');
GO

-- Trạng thái
INSERT INTO TrangThai VALUES
('TT1', N'Còn hàng'),
('TT2', N'Cháy hàng'),
('TT3', N'Hết hàng'),
('TT4', N'Ngưng bán');
GO

-- Sản phẩm
INSERT INTO SanPham VALUES
('SP00000001', N'Sữa rửa mặt Innisfree Green Tea', 120000,
 N'Làm sạch da dịu nhẹ', NULL,
 N'Trà xanh Jeju', N'Làm sạch & cấp ẩm',
 N'Dùng sáng và tối', N'Bảo quản nơi khô ráo', 150,
 'TT1', 'SRM', 'H0001'),

('SP00000002', N'Toner Some By Mi AHA BHA PHA', 180000,
 N'Làm sạch sâu, giảm mụn', NULL,
 N'AHA, BHA, PHA', N'Cân bằng da, giảm mụn',
 N'Dùng sau rửa mặt', N'Tránh ánh nắng trực tiếp', 120,
 'TT2', 'TONER', 'H0002'),

('SP00000003', N'Serum La Roche-Posay Hyalu B5', 850000,
 N'Cấp ẩm và phục hồi da', NULL,
 N'Vitamin B5, HA', N'Phục hồi da',
 N'Dùng buổi tối', N'Nơi khô mát', 80,
 'TT3', 'SERUM', 'H0003'),

('SP00000004', N'Kem dưỡng Cocoon Bưởi', 220000,
 N'Dưỡng ẩm, phục hồi da', NULL,
 N'Tinh dầu bưởi', N'Dưỡng ẩm, sáng da',
 N'Dùng sáng và tối', N'Nơi khô ráo', 100,
 'TT1', 'KDUONG', 'H0004'),

('SP00000005', N'Kem chống nắng La Roche-Posay SPF50+', 450000,
 N'Chống nắng phổ rộng', NULL,
 N'Mexoryl XL', N'Bảo vệ da khỏi tia UV',
 N'Dùng ban ngày', N'Tránh nhiệt độ cao', 60,
 'TT4', 'KCN', 'H0003');
GO

-- TTBH
INSERT INTO TrangThaiBH (MaTTBH, TenTTBH) VALUES 
('CHO', N'Chờ xác nhận'),
('XLY', N'Đang xử lý'),
('GIA', N'Đang giao hàng'),
('HTH', N'Hoàn thành'),
('TRH', N'Trả hàng'),
('HUY', N'Đã hủy');
GO

-- TTMH
INSERT INTO TrangThaiMH (MaTTMH, TenTTMH) VALUES 
('CHO', N'Chờ nhập'),
('XLY', N'Đang xử lý'),
('DNH', N'Đang nhập'),
('HTH', N'Hoàn thành'),
('HUY', N'Đã hủy');
GO

-- DMH 
INSERT INTO DonMuaHang VALUES
('M2511010001', '2025-11-01', 'NCC0000001', 'NV25121002', 'HTH');

INSERT INTO CTMH VALUES
('M2511010001', 'SP00000001', 100, 90000),
('M2511010001', 'SP00000002', 80, 140000),
('M2511010001', 'SP00000004', 70, 170000);
GO

-- DBH
INSERT INTO DonBanHang VALUES
('B2512010001', '2025-12-01', 'KH00000001', N'10 Tràng Tiền', 101, 'CHO');

INSERT INTO CTBH VALUES
('B2512010001', 'SP00000001', 2, 120000),
('B2512010001', 'SP00000003', 1, 850000),
('B2512010001', 'SP00000005', 1, 450000);
GO
