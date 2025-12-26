USE DB_QLBH;
GO

-- ======================
-- TỈNH
-- ======================
INSERT INTO Tinh (MaTinh, TenTinh) VALUES 
('01', N'Hà Nội'),
('02', N'Hồ Chí Minh'),
('03', N'Đà Nẵng'),
('04', N'Hải Phòng'),
('05', N'Bắc Ninh'),
('06', N'Nam Định'),
('07', N'Bình Dương'),
('08', N'Khánh Hòa'),
('09', N'An Giang'),
('10', N'Thừa Thiên Huế');
GO

-- ======================
-- XÃ
-- ======================
INSERT INTO Xa (MaXa, TenXa, MaTinh) VALUES
('01001', N'Phường Hoàn Kiếm', '01'),
('01002', N'Phường Dịch Vọng', '01'),
('01003', N'Phường Mỹ Đình 1', '01'),

('02001', N'Phường Bến Nghé', '02'),
('02002', N'Phường Tân Định', '02'),
('02003', N'Phường Thảo Điền', '02'),

('03001', N'Phường Hải Châu 1', '03'),
('03002', N'Phường Thạch Thang', '03');
GO

-- ======================
-- NƯỚC
-- ======================
INSERT INTO Nuoc VALUES
('QG001', N'Hàn Quốc'),
('QG002', N'Nhật Bản'),
('QG003', N'Mỹ'),
('QG004', N'Việt Nam');
GO

-- ======================
-- HÃNG SẢN XUẤT
-- ======================
INSERT INTO HangSX VALUES
('H0001', N'Innisfree', 'QG001'),
('H0002', N'Some By Mi', 'QG001'),
('H0003', N'La Roche-Posay', 'QG003'),
('H0004', N'Cocoon', 'QG004');
GO

-- ======================
-- NHÀ CUNG CẤP
-- ======================
INSERT INTO NhaCC VALUES
('NCC0000001', N'Guardian Vietnam', '02873007300', 'cskh@guardian.com.vn', N'20 Tràng Tiền', '01001'),
('NCC0000002', N'Hasaki', '18006324', 'support@hasaki.vn', N'25 Nguyễn Huệ', '02001');
GO

-- ======================
-- VAI TRÒ
-- ======================
INSERT INTO VaiTro VALUES
('QT', N'Quản trị'),
('NV', N'Nhân viên'),
('QL', N'Quản lý');
GO

-- ======================
-- NHÂN VIÊN
-- ======================
INSERT INTO NhanVien
(MaNV, CCCD, TenNV, GioiTinh, NgaySinh, SDT, Email, NgayLam, AnhNV, DiaChiNV, MaXa, TenDNNV, MatKhauNV)
VALUES
('NV25121001', '001201000001', N'Nguyễn Thị Hương', 0, '1995-03-15',
 '0901234567', 'admin@mypham.vn', '2022-01-10',
 NULL, N'10 Tràng Tiền', '01001',
 'admin', '123456'),

('NV25121002', '001201000002', N'Trần Mỹ Linh', 0, '1998-07-22',
 '0902345678', 'linh.tran@mypham.vn', '2023-03-05',
 NULL, N'25 Nguyễn Huệ', '02001',
 'tuvan01', '123456'),

('NV25121101', '001201000003', N'Lê Hoàng Nam', 1, '1992-11-02',
 '0903456789', 'nam.le@mypham.vn', '2021-06-20',
 NULL, N'88 Hải Châu', '03001',
 'quanly01', '123456');
GO

-- ======================
-- PHÂN QUYỀN
-- ======================
INSERT INTO PhanQuyen VALUES
('QT', 'NV25121001'),
('NV', 'NV25121002'),
('QL', 'NV25121101');
GO

-- ======================
-- KHÁCH HÀNG
-- ======================
INSERT INTO KhachHang VALUES
('KH00000001', N'Nguyễn Thị Lan', NULL, 0, 'lan@gmail.com', '0901111222', N'10 Tràng Tiền', 'lannt', '123456', '01001'),
('KH00000002', N'Trần Mỹ Anh', NULL, 0, 'anh@gmail.com', '0902222333', N'15 Nguyễn Huệ', 'anhtran', '123456', '02001'),
('KH00000003', N'Lê Hoàng', NULL, 1, 'hoang@gmail.com', '0903333444', N'88 Hải Châu', 'lehoang', '123456', '03001');
GO

-- ======================
-- NHÓM SP
-- ======================
INSERT INTO NhomSP VALUES
('CSDA', N'Mỹ phẩm chăm sóc da'),
('CSTOC', N'Sản phẩm chăm sóc tóc');
GO

-- ======================
-- LOẠI SP
-- ======================
INSERT INTO LoaiSP VALUES
('SRM', N'Sữa rửa mặt', 'CSDA'),
('TONER', N'Toner', 'CSDA'),
('SERUM', N'Serum', 'CSDA'),
('KDUONG', N'Kem dưỡng', 'CSDA'),
('KCN', N'Kem chống nắng', 'CSDA');
GO

-- ======================
-- TRẠNG THÁI SP
-- ======================
INSERT INTO TrangThai VALUES
('TT1', N'Còn hàng'),
('TT2', N'Cháy hàng'),
('TT3', N'Hết hàng'),
('TT4', N'Ngưng bán');
GO

-- ======================
-- SẢN PHẨM
-- ======================
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
 'TT3', 'SERUM', 'H0003');
GO

-- ======================
-- TRẠNG THÁI BÁN / MUA
-- ======================
INSERT INTO TrangThaiBH VALUES
('CHO', N'Chờ xác nhận'),
('HTH', N'Hoàn thành'),
('HUY', N'Đã hủy');

INSERT INTO TrangThaiMH VALUES
('CHO', N'Chờ nhập'),
('HTH', N'Hoàn thành');
GO

-- ======================
-- ĐƠN MUA
-- ======================
INSERT INTO DonMuaHang VALUES
('M2511010001', '2025-11-01', 'NCC0000001', 'NV25121002', 'HTH');

INSERT INTO CTMH VALUES
('M2511010001', 'SP00000001', 100, 90000),
('M2511010001', 'SP00000002', 80, 140000);
GO

-- ======================
-- ĐƠN BÁN
-- ======================
INSERT INTO DonBanHang VALUES
('B2512010001', '2025-12-01', 'KH00000001', N'10 Tràng Tiền', '01001', 'CHO');

INSERT INTO CTBH VALUES
('B2512010001', 'SP00000001', 2, 120000),
('B2512010001', 'SP00000003', 1, 850000);
GO
