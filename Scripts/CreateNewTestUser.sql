USE WF_HealthTracker;
GO

-- ============================================
-- TẠO TÀI KHOẢN MỚI VỚI ĐẦY ĐỦ DỮ LIỆU MẪU
-- Username: testuser
-- Password: test123 (cần generate hash)
-- ============================================

-- ============================================
-- BƯỚC 1: GENERATE PASSWORD HASH
-- ============================================
-- Chạy code C# sau trong ứng dụng để generate hash:
--
-- using HealthApp.Common.Helpers;
-- string hash = PasswordHelper.HashPassword("test123");
-- System.Diagnostics.Debug.WriteLine($"Hash: {hash}");
--
-- Copy hash và thay thế <PASSWORD_HASH> bên dưới

-- ============================================
-- BƯỚC 2: TẠO USER MỚI
-- ============================================
DECLARE @NewUserID VARCHAR(20) = 'user_test1';
DECLARE @Username NVARCHAR(50) = 'testuser';
DECLARE @PasswordHash NVARCHAR(256) = '<PASSWORD_HASH>'; -- Thay thế bằng hash từ code C#
DECLARE @Email NVARCHAR(100) = 'testuser@example.com';
DECLARE @HoTen NVARCHAR(100) = N'Nguyễn Văn Test';
DECLARE @SDT NVARCHAR(20) = '0909000999';

-- Xóa user cũ nếu có
IF EXISTS (SELECT 1 FROM Users WHERE UserID = @NewUserID)
BEGIN
    -- Xóa các bảng liên quan trước
    DELETE FROM BaiTapChiTiet WHERE BuoiTapID IN (SELECT BuoiTapID FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = @NewUserID));
    DELETE FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = @NewUserID);
    DELETE FROM KeHoachLuyenTap WHERE UserID = @NewUserID;
    DELETE FROM BuaAnChiTiet WHERE KeHoachAnID IN (SELECT KeHoachAnID FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = @NewUserID));
    DELETE FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = @NewUserID);
    DELETE FROM MucTieu WHERE UserID = @NewUserID;
    DELETE FROM TinhTrangTongQuan WHERE UserID = @NewUserID;
    DELETE FROM DanhGiaPT WHERE KhachHangID = @NewUserID;
    DELETE FROM GiaoDich WHERE KhachHangID = @NewUserID;
    DELETE FROM DatLichPT WHERE KhachHangID = @NewUserID;
    DELETE FROM GoiThanhVien WHERE UserID = @NewUserID;
    DELETE FROM TapTin WHERE UserID = @NewUserID;
    DELETE FROM BanBe WHERE UserID = @NewUserID OR NguoiNhanID = @NewUserID;
    DELETE FROM ChiaSeThanhTuu WHERE NguoiChiaSe = @NewUserID;
    DELETE FROM LuotThichChiaSeThanhTuu WHERE UserID = @NewUserID;
    DELETE FROM ThongBao WHERE UserID = @NewUserID;
    DELETE FROM ThanhTuu WHERE UserID = @NewUserID;
    DELETE FROM Users WHERE UserID = @NewUserID;
END
GO

-- Tạo user mới (CHỈ CHẠY SAU KHI ĐÃ GENERATE PASSWORD HASH)
-- INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu, CreatedDate)
-- VALUES (@NewUserID, @Username, @PasswordHash, 'Client', @Email, @SDT, @HoTen, '1995-01-01', 'Nam', 'Light', 'vi', GETDATE());

PRINT '============================================';
PRINT 'HƯỚNG DẪN TẠO USER MỚI:';
PRINT '============================================';
PRINT '1. Chạy code C# để generate password hash (xem comment trên)';
PRINT '2. Copy hash và thay thế <PASSWORD_HASH> trong script';
PRINT '3. Uncomment INSERT statement và chạy';
PRINT '4. Sau đó chạy phần tạo dữ liệu mẫu bên dưới';
PRINT '============================================';
GO

-- ============================================
-- BƯỚC 3: TẠO DỮ LIỆU MẪU (CHẠY SAU KHI TẠO USER)
-- ============================================
-- Uncomment các phần dưới sau khi đã tạo user thành công

DECLARE @NewUserID VARCHAR(20) = 'user_test1';

-- TÌNH TRẠNG TỔNG QUAN (5 records)
/*
INSERT INTO TinhTrangTongQuan
(BanGhiID, UserID, NgayGhiNhan, CanNang, ChieuCao, SoDoVong1, SoDoVong2, SoDoVong3, SoDoBapTay, SoDoBapChan, TheTrang, BenhID, TrinhDoCaNhan, GhiChu)
VALUES
('rec_test1', @NewUserID, DATEADD(day, -20, GETDATE()), 70, 175, 90, 80, 95, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test2', @NewUserID, DATEADD(day, -15, GETDATE()), 69.5, 175, 89, 79, 94, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Giảm nhẹ'),
('rec_test3', @NewUserID, DATEADD(day, -10, GETDATE()), 69, 175, 89, 79, 93, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test4', @NewUserID, DATEADD(day, -5, GETDATE()), 68.5, 175, 88, 78, 93, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tiến bộ'),
('rec_test5', @NewUserID, GETDATE(), 68, 175, 88, 78, 92, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tốt');
*/

-- MỤC TIÊU (1 mục tiêu - Giảm cân)
/*
INSERT INTO MucTieu
(MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, TrangThai, PTID, GhiChu)
VALUES
('goal_test1', @NewUserID, N'Giảm cân', N'Giảm 3kg trong 2 tháng', 3, DATEADD(month, -1, GETDATE()), DATEADD(month, 1, GETDATE()), N'Đang thực hiện', NULL, N'Cố gắng mỗi ngày');
*/

-- KẾ HOẠCH ĂN UỐNG (2 kế hoạch)
/*
INSERT INTO KeHoachAnUong
(KeHoachAnID, MucTieuID, TongCalories, TongProtein, TongCarbs, TongFat, Fiber, MoTa, TrangThai)
VALUES
('meal_test1', 'goal_test1', 1800, 120, 150, 60, 25, N'Giảm cân - ngày 1', N'Đang hoạt động'),
('meal_test2', 'goal_test1', 1700, 110, 140, 55, 20, N'Giảm cân - ngày 2', N'Đang hoạt động');
*/

-- BỮA ĂN CHI TIẾT (3 dòng)
/*
INSERT INTO BuaAnChiTiet
(BuaAnID, KeHoachAnID, MonAnID, LoaiBuaAn, NgayAn, TenMonAn, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber)
VALUES
('meal_item_test1', 'meal_test1', 'food_0001', N'Sáng', GETDATE(), N'Ức gà', 'g', 100, 165, 31, 0, 3.6, 0),
('meal_item_test2', 'meal_test1', 'food_0002', N'Trưa', GETDATE(), N'Bông cải xanh', 'g', 100, 34, 3, 7, 0.4, 2.6),
('meal_item_test3', 'meal_test1', 'food_0004', N'Tối', GETDATE(), N'Cá hồi', 'g', 100, 208, 20, 0, 13, 0);
*/

-- KẾ HOẠCH LUYỆN TẬP (1 kế hoạch)
/*
INSERT INTO KeHoachLuyenTap
(KeHoachTapID, UserID, MucTieuID, TongCalories, CapDo, TrangThai, MoTa)
VALUES
('workout_test1', @NewUserID, 'goal_test1', 0, 'Beginner', N'Đang hoạt động', N'Giảm cân');
*/

-- BUỔI TẬP (3 buổi)
/*
INSERT INTO BuoiTap
(BuoiTapID, KeHoachTapID, ThuNgay, ThoiGianBatDau, ThoiGianKetThuc, TrangThai, Calories, NgayThucHien)
VALUES
('session_test1', 'workout_test1', N'Thứ 2', DATEADD(day, -6, GETDATE()), DATEADD(day, -6, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 350, DATEADD(day, -6, GETDATE())),
('session_test2', 'workout_test1', N'Thứ 4', DATEADD(day, -4, GETDATE()), DATEADD(day, -4, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 380, DATEADD(day, -4, GETDATE())),
('session_test3', 'workout_test1', N'Thứ 6', DATEADD(day, -2, GETDATE()), DATEADD(day, -2, DATEADD(hour, 1, GETDATE())), N'Hoàn thành', 400, DATEADD(day, -2, GETDATE()));
*/

-- BÀI TẬP CHI TIẾT (3 dòng)
/*
INSERT INTO BaiTapChiTiet
(BaiTapChiTietID, BuoiTapID, BaiTapID, SoSet, SoRep, ThoiLuongDeNghi, ThoiGianNghi, TrongLuong, Calories, ThuTuThucHien, TrangThai)
VALUES
('detail_test1', 'session_test1', 'ex_0001', 3, 12, 30, 60, 0, 120, 1, N'Hoàn thành'),
('detail_test2', 'session_test1', 'ex_0002', 3, 10, 40, 60, 0, 230, 2, N'Hoàn thành'),
('detail_test3', 'session_test2', 'ex_0003', 3, 15, 50, 90, 0, 140, 1, N'Hoàn thành');
*/

PRINT '';
PRINT '============================================';
PRINT 'SAU KHI TẠO USER, UNCOMMENT CÁC PHẦN TRÊN';
PRINT 'VÀ CHẠY LẠI ĐỂ TẠO DỮ LIỆU MẪU';
PRINT '============================================';
GO

