USE WF_HealthTracker;
GO

-- ============================================
-- TẠO TÀI KHOẢN MỚI VỚI ĐẦY ĐỦ DỮ LIỆU MẪU
-- Username: testuser
-- Password: test123
-- ============================================

DECLARE @NewUserID VARCHAR(20) = 'user_test1';
DECLARE @Username NVARCHAR(50) = 'testuser';
DECLARE @PasswordHash NVARCHAR(256) = '<PASSWORD_HASH>'; -- Thay thế bằng hash từ code C#
DECLARE @Email NVARCHAR(100) = 'testuser@example.com';
DECLARE @HoTen NVARCHAR(100) = N'Nguyễn Văn Test';
DECLARE @SDT NVARCHAR(20) = '0909000999';

-- ============================================
-- BƯỚC 1: GENERATE PASSWORD HASH
-- ============================================
-- Chạy code C# sau trong ứng dụng (Immediate Window):
--
-- using HealthApp.Common.Helpers;
-- string hash = PasswordHelper.HashPassword("test123");
-- System.Diagnostics.Debug.WriteLine($"Hash: {hash}");
--
-- Copy hash và thay thế <PASSWORD_HASH> bên dưới

-- ============================================
-- BƯỚC 2: XÓA USER CŨ NẾU CÓ
-- ============================================
IF EXISTS (SELECT 1 FROM Users WHERE UserID = @NewUserID)
BEGIN
    PRINT 'Đang xóa user cũ và dữ liệu liên quan...';
    
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
    
    PRINT 'Đã xóa xong!';
END
GO

-- ============================================
-- BƯỚC 3: TẠO USER MỚI
-- ============================================
-- Uncomment và thay thế <PASSWORD_HASH> sau khi generate hash
/*
INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu, CreatedDate)
VALUES (@NewUserID, @Username, @PasswordHash, 'Client', @Email, @SDT, @HoTen, '1995-01-01', 'Nam', 'Light', 'vi', GETDATE());
*/

PRINT '';
PRINT '============================================';
PRINT 'SAU KHI TẠO USER, CHẠY PHẦN DỮ LIỆU MẪU BÊN DƯỚI';
PRINT '============================================';
GO

-- ============================================
-- BƯỚC 4: TẠO DỮ LIỆU MẪU
-- ============================================
-- Uncomment các phần dưới sau khi đã tạo user thành công

DECLARE @NewUserID VARCHAR(20) = 'user_test1';

-- Đảm bảo có benh_0005
IF NOT EXISTS (SELECT 1 FROM HoSoBenhLi WHERE BenhID = 'benh_0005')
BEGIN
    INSERT INTO HoSoBenhLi (BenhID, TenBenh, LoaiBenh)
    VALUES ('benh_0005', N'Không có bệnh', N'Khác');
END
GO

-- TÌNH TRẠNG TỔNG QUAN (5 records)
/*
INSERT INTO TinhTrangTongQuan
(BanGhiID, UserID, NgayGhiNhan, CanNang, ChieuCao, SoDoVong1, SoDoVong2, SoDoVong3, SoDoBapTay, SoDoBapChan, TheTrang, BenhID, TrinhDoCaNhan, GhiChu)
VALUES
('rec_test1', 'user_test1', DATEADD(day, -20, GETDATE()), 70, 175, 90, 80, 95, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test2', 'user_test1', DATEADD(day, -15, GETDATE()), 69.5, 175, 89, 79, 94, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Giảm nhẹ'),
('rec_test3', 'user_test1', DATEADD(day, -10, GETDATE()), 69, 175, 89, 79, 93, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test4', 'user_test1', DATEADD(day, -5, GETDATE()), 68.5, 175, 88, 78, 93, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tiến bộ'),
('rec_test5', 'user_test1', GETDATE(), 68, 175, 88, 78, 92, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tốt');
*/

-- MỤC TIÊU (1 mục tiêu - Giảm cân)
/*
INSERT INTO MucTieu
(MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, TrangThai, PTID, GhiChu)
VALUES
('goal_test1', 'user_test1', N'Giảm cân', N'Giảm 3kg trong 2 tháng', 3, DATEADD(month, -1, GETDATE()), DATEADD(month, 1, GETDATE()), N'Đang thực hiện', NULL, N'Cố gắng mỗi ngày');
*/

-- KẾ HOẠCH ĂN UỐNG (1 kế hoạch)
/*
INSERT INTO KeHoachAnUong
(KeHoachAnID, MucTieuID, TongCalories, TongProtein, TongCarbs, TongFat, Fiber, MoTa, TrangThai)
VALUES
('meal_test1', 'goal_test1', 1800, 120, 150, 60, 25, N'Giảm cân - ngày 1', N'Đang hoạt động');
*/

-- BỮA ĂN CHI TIẾT (3 dòng - sử dụng raw SQL vì mapping phức tạp)
/*
-- Lưu ý: BuaAnChiTiet map vào table NhatKyDinhDuong với column mapping đặc biệt
INSERT INTO NhatKyDinhDuong (DinhDuongID, UserID, MonAnID, NgayGhiLog, LuongThucAn, GhiChu)
VALUES
('meal_item_test1', 'meal_test1', 'food_0001', GETDATE(), 100, N'Ức gà - Sáng'),
('meal_item_test2', 'meal_test1', 'food_0002', GETDATE(), 100, N'Bông cải xanh - Trưa'),
('meal_item_test3', 'meal_test1', 'food_0004', GETDATE(), 100, N'Cá hồi - Tối');
*/

-- KẾ HOẠCH LUYỆN TẬP (1 kế hoạch)
/*
INSERT INTO KeHoachLuyenTap
(KeHoachTapID, UserID, MucTieuID, TongCalories, CapDo, TrangThai, MoTa)
VALUES
('workout_test1', 'user_test1', 'goal_test1', 0, 'Beginner', N'Đang hoạt động', N'Giảm cân');
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
('detail_test3', 'session_test2', 'ex_0001', 4, 12, 30, 60, 0, 150, 1, N'Hoàn thành');
*/

PRINT '';
PRINT '============================================';
PRINT 'HƯỚNG DẪN:';
PRINT '1. Generate password hash bằng code C#';
PRINT '2. Thay thế <PASSWORD_HASH> và uncomment INSERT Users';
PRINT '3. Chạy phần tạo dữ liệu mẫu (uncomment các phần trên)';
PRINT '============================================';
GO

