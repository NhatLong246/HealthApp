USE WF_HealthTracker;
GO

-- ============================================
-- DỮ LIỆU MẪU CHO USER_0001 (nguyencac)
-- Mỗi bảng 5 records (trừ Users)
-- ============================================

-- Xóa dữ liệu cũ của user_0001 (nếu có)
DELETE FROM BaiTapChiTiet WHERE BuoiTapID IN (SELECT BuoiTapID FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = 'user_0001'));
DELETE FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = 'user_0001');
DELETE FROM KeHoachLuyenTap WHERE UserID = 'user_0001';
DELETE FROM BuaAnChiTiet WHERE KeHoachAnID IN (SELECT KeHoachAnID FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = 'user_0001'));
DELETE FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = 'user_0001');
DELETE FROM MucTieu WHERE UserID = 'user_0001';
DELETE FROM TinhTrangTongQuan WHERE UserID = 'user_0001';
DELETE FROM DanhGiaPT WHERE KhachHangID = 'user_0001';
DELETE FROM GiaoDich WHERE KhachHangID = 'user_0001';
DELETE FROM DatLichPT WHERE KhachHangID = 'user_0001';
DELETE FROM GoiThanhVien WHERE UserID = 'user_0001';
DELETE FROM TapTin WHERE UserID = 'user_0001';
DELETE FROM BanBe WHERE UserID = 'user_0001' OR NguoiNhanID = 'user_0001';
DELETE FROM ChiaSeThanhTuu WHERE NguoiChiaSe = 'user_0001';
DELETE FROM LuotThichChiaSeThanhTuu WHERE UserID = 'user_0001';
DELETE FROM ThongBao WHERE UserID = 'user_0001';
DELETE FROM ThanhTuu WHERE UserID = 'user_0001';
GO

-- ============================================
-- HỒ SƠ BỆNH LÝ (5 bệnh)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM HoSoBenhLi WHERE BenhID = 'benh_0001')
BEGIN
    INSERT INTO HoSoBenhLi (BenhID, TenBenh, LoaiBenh) VALUES
    ('benh_0001', N'Tiểu đường', N'Mãn tính'),
    ('benh_0002', N'Huyết áp cao', N'Mãn tính'),
    ('benh_0003', N'Viêm dạ dày', N'Tiêu hóa'),
    ('benh_0004', N'Đau lưng', N'Cơ xương'),
    ('benh_0005', N'Không có bệnh', N'Khác');
END
GO

-- ============================================
-- TÌNH TRẠNG TỔNG QUAN (5 records cho user_0001)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TinhTrangTongQuan WHERE BanGhiID = 'rec_0001')
BEGIN
    INSERT INTO TinhTrangTongQuan
    (BanGhiID, UserID, NgayGhiNhan, CanNang, ChieuCao, SoDoVong1, SoDoVong2, SoDoVong3, SoDoBapTay, SoDoBapChan, TheTrang, BenhID, TrinhDoCaNhan, GhiChu)
    VALUES
    ('rec_0001', 'user_0001', '2025-01-15', 70, 175, 90, 80, 95, 30, 40, N'Cân đối', 'benh_0005', N'Cơ bản', N'Ổn định'),
    ('rec_0002', 'user_0001', '2025-01-20', 69.5, 175, 89, 79, 94, 30, 40, N'Cân đối', 'benh_0005', N'Cơ bản', N'Giảm nhẹ'),
    ('rec_0003', 'user_0001', '2025-01-25', 69, 175, 89, 79, 93, 30, 40, N'Cân đối', 'benh_0005', N'Cơ bản', N'Ổn định'),
    ('rec_0004', 'user_0001', '2025-01-30', 68.5, 175, 88, 78, 93, 29, 39, N'Cân đối', 'benh_0005', N'Cơ bản', N'Tiến bộ'),
    ('rec_0005', 'user_0001', '2025-02-05', 68, 175, 88, 78, 92, 29, 39, N'Cân đối', 'benh_0005', N'Cơ bản', N'Tốt');
END
GO

-- ============================================
-- MỤC TIÊU (5 mục tiêu cho user_0001)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM MucTieu WHERE MucTieuID = 'goal_0001')
BEGIN
    INSERT INTO MucTieu
    (MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, TrangThai, PTID, GhiChu)
    VALUES
    ('goal_0001', 'user_0001', N'Giảm cân', N'Giảm 3kg trong 2 tháng', 3, '2025-01-01', '2025-03-01', N'Đang thực hiện', NULL, N'Cố gắng mỗi ngày'),
    ('goal_0002', 'user_0001', N'Tăng cơ', N'Tăng 2kg cơ', 2, '2025-01-15', '2025-04-15', N'Đang thực hiện', NULL, N'Tăng protein'),
    ('goal_0003', 'user_0001', N'Giảm mỡ', N'Giảm 2% body fat', 2, '2025-01-20', '2025-03-20', N'Hoàn thành', NULL, N'Tập cardio'),
    ('goal_0004', 'user_0001', N'Tăng sức bền', N'Chạy 5km mỗi ngày', 0, '2025-02-01', '2025-04-01', N'Đang thực hiện', NULL, N'Cường độ vừa'),
    ('goal_0005', 'user_0001', N'Duy trì', N'Duy trì cân nặng hiện tại', 0, '2025-02-10', '2025-05-10', N'Đang thực hiện', NULL, N'Duy trì tốt');
END
GO

-- ============================================
-- KẾ HOẠCH ĂN UỐNG (5 kế hoạch)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM KeHoachAnUong WHERE KeHoachAnID = 'meal_0001')
BEGIN
    INSERT INTO KeHoachAnUong
    (KeHoachAnID, MucTieuID, TongCalories, TongProtein, TongCarbs, TongFat, Fiber, MoTa, TrangThai)
    VALUES
    ('meal_0001', 'goal_0001', 1800, 120, 150, 60, 25, N'Giảm cân - ngày 1', N'Đang hoạt động'),
    ('meal_0002', 'goal_0001', 1700, 110, 140, 55, 20, N'Giảm cân - ngày 2', N'Đang hoạt động'),
    ('meal_0003', 'goal_0002', 2200, 160, 180, 70, 30, N'Tăng cơ - ngày 1', N'Đang hoạt động'),
    ('meal_0004', 'goal_0002', 2300, 170, 190, 75, 32, N'Tăng cơ - ngày 2', N'Đang hoạt động'),
    ('meal_0005', 'goal_0003', 2000, 140, 160, 65, 28, N'Duy trì - ngày 1', N'Đang hoạt động');
END
GO

-- ============================================
-- THƯ VIỆN MÓN ĂN (5 món - nếu chưa có)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM ThuVienMonAn WHERE MonAnID = 'food_0001')
BEGIN
    INSERT INTO ThuVienMonAn
    (MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber)
    VALUES
    ('food_0001', '', N'Ức gà', N'Thịt', 'g', 100, 165, 31, 0, 3.6, 0),
    ('food_0002', '', N'Bông cải xanh', N'Rau củ', 'g', 100, 34, 3, 7, 0.4, 2.6),
    ('food_0003', '', N'Yến mạch', N'Ngũ cốc', 'g', 100, 389, 17, 66, 7, 10),
    ('food_0004', '', N'Cá hồi', N'Hải sản', 'g', 100, 208, 20, 0, 13, 0),
    ('food_0005', '', N'Táo', N'Trái cây', 'g', 100, 52, 0.3, 14, 0.2, 2.4);
END
GO

-- ============================================
-- BỮA ĂN CHI TIẾT (5 dòng)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM BuaAnChiTiet WHERE BuaAnID = 'meal_item_0001')
BEGIN
    INSERT INTO BuaAnChiTiet
    (BuaAnID, KeHoachAnID, MonAnID, LoaiBuaAn, NgayAn, TenMonAn, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber)
    VALUES
    ('meal_item_0001', 'meal_0001', 'food_0001', N'Sáng', '2025-01-15', N'Ức gà', 'g', 100, 165, 31, 0, 3.6, 0),
    ('meal_item_0002', 'meal_0001', 'food_0002', N'Trưa', '2025-01-15', N'Bông cải', 'g', 100, 34, 3, 7, 0.4, 2.6),
    ('meal_item_0003', 'meal_0003', 'food_0003', N'Tối', '2025-01-20', N'Yến mạch', 'g', 100, 389, 17, 66, 7, 10),
    ('meal_item_0004', 'meal_0004', 'food_0004', N'Sáng', '2025-01-25', N'Cá hồi', 'g', 100, 208, 20, 0, 13, 0),
    ('meal_item_0005', 'meal_0005', 'food_0005', N'Phụ', '2025-02-01', N'Táo', 'g', 100, 52, 0.3, 14, 0.2, 2.4);
END
GO

-- ============================================
-- THƯ VIỆN BÀI TẬP (5 bài)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM ThuVienBaiTap WHERE BaiTapID = 'ex_0001')
BEGIN
    INSERT INTO ThuVienBaiTap
    (BaiTapID, TenBaiTap, LoaiMucTieu, NhomCoChinhNhat, NhomCoPhu, CapDo, DungCu, MoTa, HuongDan, LuuY, NguoiTao)
    VALUES
    ('ex_0001', N'Chống đẩy', N'Tăng cơ', N'Ngực', N'Tay sau', 'Beginner', N'Không dụng cụ', N'Mô tả chống đẩy', N'Hướng dẫn từng bước', N'Lưu ý tư thế', NULL),
    ('ex_0002', N'Kéo xà', N'Tăng cơ', N'Lưng', N'Tay trước', 'Intermediate', N'Xà đơn', N'Mô tả kéo xà', N'Hướng dẫn từng bước', N'Lưu ý an toàn', NULL),
    ('ex_0003', N'Squat', N'Tăng cơ', N'Chân', N'Mông', 'All Levels', N'Không', N'Mô tả squat', N'Hướng dẫn từng bước', N'Lưu ý đầu gối', NULL),
    ('ex_0004', N'Plank', N'Giảm mỡ', N'Core', N'Bụng', 'Beginner', N'Thảm tập', N'Mô tả plank', N'Hướng dẫn từng bước', N'Lưu ý lưng thẳng', NULL),
    ('ex_0005', N'Burpee', N'Giảm mỡ', N'Full body', NULL, 'Intermediate', N'Không', N'Mô tả burpee', N'Hướng dẫn từng bước', N'Lưu ý nhịp thở', NULL);
END
GO

-- ============================================
-- KẾ HOẠCH LUYỆN TẬP (5 kế hoạch cho user_0001)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM KeHoachLuyenTap WHERE KeHoachTapID = 'workout_0001')
BEGIN
    INSERT INTO KeHoachLuyenTap
    (KeHoachTapID, UserID, MucTieuID, TongCalories, CapDo, TrangThai, MoTa)
    VALUES
    ('workout_0001', 'user_0001', 'goal_0001', 0, 'Beginner', N'Đang hoạt động', N'Giảm cân'),
    ('workout_0002', 'user_0001', 'goal_0002', 0, 'Intermediate', N'Đang hoạt động', N'Tăng cơ'),
    ('workout_0003', 'user_0001', 'goal_0003', 0, 'Beginner', N'Đang hoạt động', N'Giảm mỡ'),
    ('workout_0004', 'user_0001', 'goal_0004', 0, 'Intermediate', N'Đang hoạt động', N'Sức bền'),
    ('workout_0005', 'user_0001', 'goal_0005', 0, 'Beginner', N'Đang hoạt động', N'Duy trì');
END
GO

-- ============================================
-- BUỔI TẬP (5 buổi)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM BuoiTap WHERE BuoiTapID = 'session_0001')
BEGIN
    INSERT INTO BuoiTap
    (BuoiTapID, KeHoachTapID, ThuNgay, ThoiGianBatDau, ThoiGianKetThuc, TrangThai, Calories, NgayThucHien)
    VALUES
    ('session_0001', 'workout_0001', N'Thứ 2', DATEADD(day, -6, GETDATE()), DATEADD(day, -6, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 350, DATEADD(day, -6, GETDATE())),
    ('session_0002', 'workout_0002', N'Thứ 3', DATEADD(day, -5, GETDATE()), DATEADD(day, -5, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 420, DATEADD(day, -5, GETDATE())),
    ('session_0003', 'workout_0001', N'Thứ 4', DATEADD(day, -4, GETDATE()), DATEADD(day, -4, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 380, DATEADD(day, -4, GETDATE())),
    ('session_0004', 'workout_0003', N'Thứ 5', DATEADD(day, -3, GETDATE()), DATEADD(day, -3, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 450, DATEADD(day, -3, GETDATE())),
    ('session_0005', 'workout_0002', N'Thứ 6', DATEADD(day, -2, GETDATE()), DATEADD(day, -2, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 400, DATEADD(day, -2, GETDATE()));
END
GO

-- ============================================
-- BÀI TẬP CHI TIẾT (5 dòng)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM BaiTapChiTiet WHERE BaiTapChiTietID = 'detail_0001')
BEGIN
    INSERT INTO BaiTapChiTiet
    (BaiTapChiTietID, BuoiTapID, BaiTapID, SoSet, SoRep, ThoiLuongDeNghi, ThoiGianNghi, TrongLuong, Calories, ThuTuThucHien, TrangThai)
    VALUES
    ('detail_0001', 'session_0001', 'ex_0001', 3, 12, 30, 60, 0, 120, 1, N'Hoàn thành'),
    ('detail_0002', 'session_0001', 'ex_0002', 3, 10, 40, 60, 0, 230, 2, N'Hoàn thành'),
    ('detail_0003', 'session_0002', 'ex_0002', 4, 10, 40, 60, 0, 280, 1, N'Hoàn thành'),
    ('detail_0004', 'session_0002', 'ex_0003', 3, 15, 50, 90, 0, 140, 2, N'Hoàn thành'),
    ('detail_0005', 'session_0003', 'ex_0001', 4, 12, 30, 60, 0, 150, 1, N'Hoàn thành');
END
GO

-- ============================================
-- HUẤN LUYỆN VIÊN (Cần tạo user PT trước)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = 'user_0002')
BEGIN
    INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu)
    VALUES ('user_0002', 'pt_long', 'DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF', 'PT', 'ptlong@example.com', '0909000002', N'Nguyễn Nhật Long', '1990-05-15', 'Nam', 'Light', 'vi');
END
GO

IF NOT EXISTS (SELECT 1 FROM HuanLuyenVien WHERE PTID = 'ptr_0001')
BEGIN
    INSERT INTO HuanLuyenVien
    (PTID, UserID, ChungChi, ChuyenMon, SoNamKinhNghiem, ThanhPho, GiaTheoGio, TieuSu, DaXacMinh, NhanKhach)
    VALUES
    ('ptr_0001', 'user_0002', N'NASM, ACE', N'Giảm cân, Tăng cơ, Cardio', 5, N'Hà Nội', 300000, N'PT chuyên nghiệp với 5 năm kinh nghiệm', 1, 1);
END
GO

-- ============================================
-- ĐẶT LỊCH PT (5 lịch)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM DatLichPT WHERE DatLichID = 'bkg_0001')
BEGIN
    INSERT INTO DatLichPT
    (DatLichID, KhachHangID, PTID, NgayGioDat, ThoiLuong, LoaiBuoiTap, TrangThai, ChoXemSucKhoe)
    VALUES
    ('bkg_0001', 'user_0001', 'ptr_0001', DATEADD(day, 1, GETDATE()), 60, 'Online', 'Pending', 0),
    ('bkg_0002', 'user_0001', 'ptr_0001', DATEADD(day, 3, GETDATE()), 60, 'In-person', 'Confirmed', 1),
    ('bkg_0003', 'user_0001', 'ptr_0001', DATEADD(day, 5, GETDATE()), 90, 'Online', 'Pending', 0),
    ('bkg_0004', 'user_0001', 'ptr_0001', DATEADD(day, 7, GETDATE()), 60, 'In-person', 'Confirmed', 1),
    ('bkg_0005', 'user_0001', 'ptr_0001', DATEADD(day, 10, GETDATE()), 60, 'Online', 'Pending', 0);
END
GO

-- ============================================
-- ĐÁNH GIÁ PT (5 đánh giá)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM DanhGiaPT WHERE DanhGiaID = 'rev_0001')
BEGIN
    INSERT INTO DanhGiaPT
    (DanhGiaID, DatLichID, KhachHangID, PTID, Diem, BinhLuan, NgayDanhGia)
    VALUES
    ('rev_0001', 'bkg_0001', 'user_0001', 'ptr_0001', 5, N'PT rất nhiệt tình và chuyên nghiệp', DATEADD(day, -5, GETDATE())),
    ('rev_0002', 'bkg_0002', 'user_0001', 'ptr_0001', 5, N'Rất hài lòng với buổi tập', DATEADD(day, -3, GETDATE())),
    ('rev_0003', 'bkg_0003', 'user_0001', 'ptr_0001', 4, N'Tốt nhưng cần cải thiện thêm', DATEADD(day, -2, GETDATE())),
    ('rev_0004', 'bkg_0004', 'user_0001', 'ptr_0001', 5, N'Xuất sắc!', DATEADD(day, -1, GETDATE())),
    ('rev_0005', 'bkg_0005', 'user_0001', 'ptr_0001', 5, N'Rất recommend', GETDATE());
END
GO

-- ============================================
-- GIAO DỊCH (5 giao dịch)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM GiaoDich WHERE GiaoDichID = 'txn_0001')
BEGIN
    INSERT INTO GiaoDich
    (GiaoDichID, DatLichID, KhachHangID, PTID, SoTien, HoaHongApp, SoTienHoaHong, SoTienPTNhan, TrangThaiThanhToan, PhuongThucThanhToan, MaGiaoDich, NgayGiaoDich)
    VALUES
    ('txn_0001', 'bkg_0001', 'user_0001', 'ptr_0001', 300000, 15, 45000, 255000, 'Completed', 'Credit Card', 'TXN001', DATEADD(day, -5, GETDATE())),
    ('txn_0002', 'bkg_0002', 'user_0001', 'ptr_0001', 300000, 15, 45000, 255000, 'Completed', 'Bank Transfer', 'TXN002', DATEADD(day, -3, GETDATE())),
    ('txn_0003', 'bkg_0003', 'user_0001', 'ptr_0001', 450000, 15, 67500, 382500, 'Pending', 'E-Wallet', 'TXN003', DATEADD(day, -1, GETDATE())),
    ('txn_0004', 'bkg_0004', 'user_0001', 'ptr_0001', 300000, 15, 45000, 255000, 'Completed', 'Credit Card', 'TXN004', GETDATE()),
    ('txn_0005', 'bkg_0005', 'user_0001', 'ptr_0001', 300000, 15, 45000, 255000, 'Pending', 'E-Wallet', 'TXN005', DATEADD(day, 1, GETDATE()));
END
GO

-- ============================================
-- GÓI THÀNH VIÊN (5 gói)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM GoiThanhVien WHERE GoiThanhVienID = 'sub_0001')
BEGIN
    INSERT INTO GoiThanhVien
    (GoiThanhVienID, UserID, LoaiGoi, NgayBatDau, NgayKetThuc, TrangThai, SoTien, ChuKyThanhToan, PhuongThucThanhToan, TuDongGiaHan, NgayDangKy)
    VALUES
    ('sub_0001', 'user_0001', 'Free', '2025-01-01', NULL, 'Active', NULL, 'Lifetime', NULL, 0, '2025-01-01'),
    ('sub_0002', 'user_0001', 'Basic', '2025-01-15', '2025-02-15', 'Expired', 99000, 'Monthly', 'Credit Card', 0, '2025-01-15'),
    ('sub_0003', 'user_0001', 'Premium', '2025-02-01', '2025-03-01', 'Active', 199000, 'Monthly', 'Bank Transfer', 1, '2025-02-01'),
    ('sub_0004', 'user_0001', 'Premium', '2025-01-20', '2025-07-20', 'Active', 999000, 'Yearly', 'Credit Card', 1, '2025-01-20'),
    ('sub_0005', 'user_0001', 'Basic', '2024-12-01', '2025-01-01', 'Cancelled', 99000, 'Monthly', 'E-Wallet', 0, '2024-12-01');
END
GO

-- ============================================
-- TẬP TIN (5 file)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TapTin WHERE TapTinID = 'file_0001')
BEGIN
    INSERT INTO TapTin
    (TapTinID, UserID, TenTapTin, TenLuuTrenServer, DuongDan, KichThuoc, MimeType, LoaiFile, MucDich, NgayUpload)
    VALUES
    ('file_0001', 'user_0001', 'anh_dai_dien.jpg', 'uuid_001.jpg', '/uploads/users/user_0001/', 245760, 'image/jpeg', 'Image', 'AnhDaiDien', '2025-01-10'),
    ('file_0002', 'user_0001', 'bao_cao_thang_1.pdf', 'uuid_002.pdf', '/uploads/users/user_0001/reports/', 1024000, 'application/pdf', 'PDF', 'BaoCao', '2025-01-25'),
    ('file_0003', 'user_0001', 'anh_bua_an_1.jpg', 'uuid_003.jpg', '/uploads/users/user_0001/meals/', 512000, 'image/jpeg', 'Image', 'AnhBuaAn', '2025-02-01'),
    ('file_0004', 'user_0001', 'video_tap_luyen.mp4', 'uuid_004.mp4', '/uploads/users/user_0001/videos/', 15728640, 'video/mp4', 'Video', 'VideoTap', '2025-02-05'),
    ('file_0005', 'user_0001', 'ket_qua_xet_nghiem.pdf', 'uuid_005.pdf', '/uploads/users/user_0001/health/', 2048000, 'application/pdf', 'PDF', 'Document', '2025-02-10');
END
GO

-- ============================================
-- BẠN BÈ (Cần tạo thêm users)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = 'user_0003')
BEGIN
    INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu)
    VALUES 
    ('user_0003', 'friend1', 'DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF', 'Client', 'friend1@example.com', '0909000003', N'Nguyễn Văn A', '1995-03-10', 'Nam', 'Light', 'vi'),
    ('user_0004', 'friend2', 'DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF', 'Client', 'friend2@example.com', '0909000004', N'Trần Thị B', '1992-07-20', 'Nữ', 'Dark', 'vi'),
    ('user_0005', 'friend3', 'DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF', 'Client', 'friend3@example.com', '0909000005', N'Lê Văn C', '1998-11-15', 'Nam', 'Light', 'vi'),
    ('user_0006', 'friend4', 'DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF', 'Client', 'friend4@example.com', '0909000006', N'Phạm Thị D', '1990-05-25', 'Nữ', 'Light', 'vi');
END
GO

IF NOT EXISTS (SELECT 1 FROM BanBe WHERE BanBeID = 'friend_0001')
BEGIN
    INSERT INTO BanBe
    (BanBeID, UserID, NguoiNhanID, TrangThai, NgayGui, NgayChapNhan)
    VALUES
    ('friend_0001', 'user_0001', 'user_0002', 'Accepted', '2025-01-05', '2025-01-06'),
    ('friend_0002', 'user_0001', 'user_0003', 'Accepted', '2025-01-10', '2025-01-11'),
    ('friend_0003', 'user_0001', 'user_0004', 'Pending', '2025-01-15', NULL),
    ('friend_0004', 'user_0005', 'user_0001', 'Accepted', '2025-01-20', '2025-01-21'),
    ('friend_0005', 'user_0006', 'user_0001', 'Pending', '2025-02-01', NULL);
END
GO

-- ============================================
-- THÀNH TỰU (5 thành tựu)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM ThanhTuu WHERE ThanhTuuID = 'achv_0001')
BEGIN
    INSERT INTO ThanhTuu
    (ThanhTuuID, UserID, LoaiThanhTuu, TenThanhTuu, Diem, NgayDatDuoc, MoTa, BieuTuong, CapDo)
    VALUES
    ('achv_0001', 'user_0001', N'Badge', N'Người mới bắt đầu', 10, '2025-01-15', N'Hoàn thành buổi tập đầu tiên', N'🏅', 1),
    ('achv_0002', 'user_0001', N'Streak', N'7 ngày liên tiếp', 50, '2025-01-22', N'Tập luyện 7 ngày liên tiếp', N'🔥', 1),
    ('achv_0003', 'user_0001', N'Milestone', N'Giảm 2kg', 100, '2025-02-01', N'Đạt mục tiêu giảm 2kg', N'⭐', 1),
    ('achv_0004', 'user_0001', N'Badge', N'Cardio Master', 30, '2025-02-05', N'Hoàn thành 10 buổi cardio', N'💪', 1),
    ('achv_0005', 'user_0001', N'Challenge', N'Thử thách 30 ngày', 200, '2025-02-10', N'Hoàn thành thử thách 30 ngày', N'🎯', 1);
END
GO

-- ============================================
-- CHIA SẺ THÀNH TỰU (5 chia sẻ)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM ChiaSeThanhTuu WHERE ChiaSeID = 'share_0001')
BEGIN
    INSERT INTO ChiaSeThanhTuu
    (ChiaSeID, ThanhTuuID, NguoiChiaSe, NgayChiaSe, DoiTuongXem, ChuThich, SoLuongThich)
    VALUES
    ('share_0001', 'achv_0001', 'user_0001', '2025-01-15', 'Friends', N'Vui quá!', 5),
    ('share_0002', 'achv_0002', 'user_0001', '2025-01-22', 'Public', N'7 ngày liên tiếp!', 12),
    ('share_0003', 'achv_0003', 'user_0001', '2025-02-01', 'Friends', N'Đạt mục tiêu giảm cân!', 8),
    ('share_0004', 'achv_0004', 'user_0001', '2025-02-05', 'Public', N'Cardio Master!', 15),
    ('share_0005', 'achv_0005', 'user_0001', '2025-02-10', 'Friends', N'30 ngày challenge!', 20);
END
GO

-- ============================================
-- LƯỢT THÍCH CHIA SẺ THÀNH TỰU (5 lượt thích)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM LuotThichChiaSeThanhTuu WHERE ThichID = 'like_0001')
BEGIN
    INSERT INTO LuotThichChiaSeThanhTuu
    (ThichID, ChiaSeID, UserID, NgayThich)
    VALUES
    ('like_0001', 'share_0001', 'user_0002', '2025-01-15'),
    ('like_0002', 'share_0001', 'user_0003', '2025-01-16'),
    ('like_0003', 'share_0002', 'user_0002', '2025-01-22'),
    ('like_0004', 'share_0003', 'user_0004', '2025-02-01'),
    ('like_0005', 'share_0004', 'user_0003', '2025-02-05');
END
GO

-- ============================================
-- THÔNG BÁO (5 thông báo)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM ThongBao WHERE ThongBaoID = 'notif_0001')
BEGIN
    INSERT INTO ThongBao
    (ThongBaoID, UserID, NoiDung, TieuDe, Loai, MaLienQuan, DaDoc, NgayTao)
    VALUES
    ('notif_0001', 'user_0001', N'Chúc mừng! Bạn đã hoàn thành mục tiêu giảm mỡ', N'Đạt mục tiêu', 'Achievement', 'goal_0003', 0, '2025-01-20'),
    ('notif_0002', 'user_0001', N'Bạn có lịch tập với PT vào ngày mai', N'Nhắc nhở lịch tập', 'Reminder', 'bkg_0002', 0, DATEADD(day, -1, GETDATE())),
    ('notif_0003', 'user_0001', N'Bạn đã nhận được lượt thích từ bạn bè', N'Thông báo tương tác', 'Social', 'share_0001', 1, '2025-01-15'),
    ('notif_0004', 'user_0001', N'Gói Premium của bạn sắp hết hạn vào 01/03/2025', N'Gia hạn gói', 'Subscription', 'sub_0003', 0, '2025-02-25'),
    ('notif_0005', 'user_0001', N'Bạn đã đạt thành tích mới: 7 ngày liên tiếp', N'Thành tích mới', 'Achievement', 'achv_0002', 1, '2025-01-22');
END
GO

-- ============================================
-- TÍNH NĂNG GÓI (5 tính năng - không liên quan user)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TinhNangGoi WHERE TinhNangID = 'feat_0001')
BEGIN
    INSERT INTO TinhNangGoi
    (TinhNangID, TenTinhNang, GoiToiThieu, MoTa, ConHoatDong, NgayTao)
    VALUES
    ('feat_0001', 'AI_Suggestions', 'Premium', N'Đề xuất món ăn và bài tập bằng AI', 1, '2025-01-01'),
    ('feat_0002', 'Advanced_Reports', 'Premium', N'Báo cáo chi tiết và phân tích nâng cao', 1, '2025-01-01'),
    ('feat_0003', 'PT_Booking', 'Basic', N'Đặt lịch với huấn luyện viên', 1, '2025-01-01'),
    ('feat_0004', 'Social_Features', 'Basic', N'Kết bạn và chia sẻ thành tích', 1, '2025-01-01'),
    ('feat_0005', 'Basic_Tracking', 'Free', N'Theo dõi cơ bản dinh dưỡng và tập luyện', 1, '2025-01-01');
END
GO

PRINT 'Đã tạo xong dữ liệu mẫu cho user_0001!';
GO

