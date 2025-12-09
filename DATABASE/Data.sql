USE DB_QLBH;
GO

-- Tỉnh
INSERT INTO Tinh VALUES (1, N'Hà Nội');
INSERT INTO Tinh VALUES (2, N'Hồ Chí Minh');
INSERT INTO Tinh VALUES (3, N'Đà Nẵng');
INSERT INTO Tinh VALUES (4, N'Hải Phòng');
INSERT INTO Tinh VALUES (5, N'Bắc Ninh');
INSERT INTO Tinh VALUES (6, N'Nam Định');
INSERT INTO Tinh VALUES (7, N'Bình Dương');
INSERT INTO Tinh VALUES (8, N'Khánh Hòa');
INSERT INTO Tinh VALUES (9, N'An Giang');
INSERT INTO Tinh VALUES (10, N'Thừa Thiên Huế');

-- Xã
INSERT INTO Xa (MaXa, TenXa, MaTinh) VALUES
(101, N'Phường Hoàn Kiếm', 1),
(201, N'Phường Bến Nghé', 2),
(301, N'Phường Hải Châu 1', 3),
(401, N'Phường Lê Chân', 4),
(501, N'Phường Ninh Kiều', 5),
(601, N'Xã Mỹ Hòa Hưng', 6),
(701, N'Phường 9', 7),
(801, N'Phường Thủ Dầu Một', 8),
(901, N'Xã Long Thành', 9),
(1001, N'Phường Thuận Hòa', 10);

-- Nước 
INSERT INTO Nuoc(MaNuoc, TenNuoc) VALUES
('QG001', N'Hàn Quốc'),
('QG002', N'Nhật Bản'),
('QG003', N'Mỹ'),
('QG004', N'Việt Nam'),
('QG005', N'Thái Lan');

-- Hãng
INSERT INTO Hang(MaHang, TenHang, MaNuoc) VALUES
('H0001', N'Innisfree', 'QG001'),
('H0002', N'Shubi', 'QG002'),
('H0003', N'Nature\''s Bounty', 'QG003'),
('H0004', N'Sunhouse', 'QG004'),
('H0005', N'Thailand Beauty', 'QG005');

-- Nhà cung cấp 
INSERT INTO NhaCC VALUES ('NCC0000001', N'AEON Vietnam', '02836208000', 'contact@aeon.com.vn', N'Số 10 Tràng Tiền', 401);
INSERT INTO NhaCC VALUES ('NCC0000002', N'Co.opXtra', '02839999555', 'support@coopmart.vn', N'Xóm Chài 2', 501);
INSERT INTO NhaCC VALUES ('NCC0000003', N'Pharmacity', '18006821', 'support@pharmacity.vn', N'15 Đặng Huy Trứ', 601);
INSERT INTO NhaCC VALUES ('NCC0000004', N'Boshop Vietnam', '02873007300', 'cskh@boshop.vn', N'18 Trần Phú', 701);

-- Khách hàng
INSERT INTO KhachHang (MaKH, TenKH, AnhKH, DienThoaiKH, EmailKH, DiaChiKH, MaXa) VALUES
('KH00000001', N'Nguyễn Văn A', '/images/customers/user1.jpg', '0901111222', 'a.nguyen@example.com', N'Số 10 Tràng Tiền', 101),
('KH00000002', N'Trần Thị B', '/images/customers/user2.jpg', '0902222333', 'b.tran@example.com', N'25 Lê Lợi', 201),
('KH00000003', N'Lê Văn C', '/images/customers/user3.jpg', '0903333444', 'c.le@example.com', N'123 Nguyễn Văn Linh', 301),
('KH00000004', N'Phạm Thị D', '/images/customers/user4.jpg', '0904444555', 'd.pham@example.com', N'18 Trần Phú', 401),
('KH00000005', N'Hoàng Minh E', '/images/customers/user5.jpg', '0905555666', 'e.hoang@example.com', N'7 Nguyễn Trãi', 501),
('KH00000006', N'Võ Thị F', '/images/customers/user6.jpg', '0906666777', 'f.vo@example.com', N'Xóm Chài 2', 601),
('KH00000007', N'Đỗ Văn G', '/images/customers/user7.jpg', '0907777888', 'g.do@example.com', N'20 Đinh Tiên Hoàng', 701),
('KH00000008', N'Bùi Thị H', '/images/customers/user8.jpg', '0908888999', 'h.bui@example.com', N'Khu 3, Thủ Dầu Một', 801),
('KH00000009', N'Tống Văn I', '/images/customers/user9.jpg', '0931234567', 'i.tong@example.com', N'Ấp 4 Long Thành', 901),
('KH00000010', N'Huỳnh Thị K', '/images/customers/user10.jpg', '0912345678', 'k.huynh@example.com', N'15 Đặng Huy Trứ', 1001);

-- Nhóm SP 
INSERT INTO NhomSP(MaNhom, TenNhom) VALUES
('N000000001', N'Mỹ phẩm'),
('N000000002', N'Dược phẩm'),
('N000000003', N'Hàng gia dụng');

-- Loại SP 
INSERT INTO LoaiSP(MaLoai, TenLoai, MaNhom) VALUES
('L000000001', N'Sữa rửa mặt', 'N000000001'),
('L000000002', N'Kem dưỡng da', 'N000000001'),
('L000000003', N'Thực phẩm chức năng', 'N000000002'),
('L000000004', N'Thuốc cảm cúm', 'N000000002'),
('L000000005', N'Nồi cơm điện', 'N000000003'),
('L000000006', N'Quạt', 'N000000003');

-- Sản phẩm
INSERT INTO SanPham(MaSP, TenSP, DonGia, GiaBan, MoTaSP, AnhMH, ThanhPhan, CongDung, HDSD, XuatXu, BaoQuan, TrangThai, SoLuongTon, MaLoai, MaHang)
VALUES
('SP00000001', N'Sữa rửa mặt Innisfree', 60000, 80000, N'Làm sạch da mụn', '/img/sp6.jpg', N'Thảo mộc tự nhiên', N'Làm sạch, ngừa mụn', N'Dùng sáng/tối', N'Hàn Quốc', N'Nơi khô ráo', N'Còn Hàng', 100, 'L000000001', 'H0001'),
('SP00000002', N'Kem dưỡng Shubi', 90000, 120000, N'Dưỡng ẩm cho da nhạy cảm', '/img/sp7.jpg', N'Lô hội, Vitamin E', N'Dưỡng ẩm, làm mềm da', N'Dùng buổi tối', N'Nhật Bản', N'Nơi khô ráo', N'Còn Hàng', 50, 'L000000002', 'H0002'),
('SP00000003', N'Viên uống Vitamin tổng hợp', 70000, 95000, N'Tăng cường sức khỏe', '/img/sp8.jpg', N'Vitamin C, D, E', N'Tăng sức đề kháng', N'Uống sau ăn', N'Mỹ', N'Tránh ẩm', N'Còn Hàng', 200, 'L000000003',  'H0003'),
('SP00000004', N'Nồi cơm điện Sunhouse 1.5L', 450000, 600000, N'Nồi cơm điện 1.5L', '/img/sp9.jpg', N'Inox', N'Nấu cơm', N'Cắm điện', N'Việt Nam', N'Nơi khô mát', N'Còn Hàng', 30, 'L000000005', 'H0004');

-- DMH 
INSERT INTO DonMuaHang(MaDMH, NgayMH, MaNCC) VALUES
('M2411010001', '2024-11-01', 'NCC0000001'),
('M2411050001', '2024-11-05', 'NCC0000003');

-- CTMH
INSERT INTO CTMH(MaDMH, MaSP, SLM, DGM) VALUES
('M2411010001', 'SP00000001', 100, 48000),
('M2411010001', 'SP00000002', 60, 76000),
('M2411050001', 'SP00000003', 80, 58000),
('M2411050001', 'SP00000004', 50, 25000);

-- DBH
INSERT INTO DonBanHang(MaDBH, NgayBH, MaKH) VALUES
('B2412010001', '2024-12-01', 'KH00000001'),
('B2412030001', '2024-12-03', 'KH00000003');

-- CTMH
INSERT INTO CTBH(MaDBH, MaSP, SLB, DGB) VALUES
('B2412010001', 'SP00000001', 3, 70000),
('B2412010001', 'SP00000002', 2, 110000),
('B2412030001', 'SP00000003', 5, 85000),
('B2412030001', 'SP00000004', 2, 45000);
