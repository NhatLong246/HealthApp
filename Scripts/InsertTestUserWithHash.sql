USE WF_HealthTracker;
GO

-- ============================================
-- TẠO USER MỚI VỚI HASH ĐÃ CÓ
-- Username: testuser
-- Password: test123
-- Hash: mkDzYH1QyASFHJA2HAxK0N/nRaFi42DP98g2KpT3MP0S435MlLsmTVmOK7C31uvj
-- ============================================

DECLARE @NewUserID VARCHAR(20) = 'user_test1';
DECLARE @Username NVARCHAR(50) = 'testuser';
DECLARE @PasswordHash NVARCHAR(256) = 'mkDzYH1QyASFHJA2HAxK0N/nRaFi42DP98g2KpT3MP0S435MlLsmTVmOK7C31uvj';
DECLARE @Email NVARCHAR(100) = 'testuser@example.com';
DECLARE @HoTen NVARCHAR(100) = N'Nguyễn Văn Test';
DECLARE @SDT NVARCHAR(20) = '0909000999';

-- Xóa user cũ nếu có
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
    DELETE FROM Users WHERE UserID = @NewUserID;
    
    PRINT 'Đã xóa xong!';
END
GO

-- Tạo user mới
INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu, CreatedDate)
VALUES ('user_test1', 'testuser', 'mkDzYH1QyASFHJA2HAxK0N/nRaFi42DP98g2KpT3MP0S435MlLsmTVmOK7C31uvj', 'Client', 'testuser@example.com', '0909000999', N'Nguyễn Văn Test', '1995-01-01', 'Nam', 'Light', 'vi', GETDATE());

PRINT 'Đã tạo user thành công!';
PRINT 'Username: testuser';
PRINT 'Password: test123';
GO

-- Đảm bảo có benh_0005
IF NOT EXISTS (SELECT 1 FROM HoSoBenhLi WHERE BenhID = 'benh_0005')
BEGIN
    INSERT INTO HoSoBenhLi (BenhID, TenBenh, LoaiBenh)
    VALUES ('benh_0005', N'Không có bệnh', N'Khác');
END
GO

-- TÌNH TRẠNG TỔNG QUAN (5 records)
INSERT INTO TinhTrangTongQuan
(BanGhiID, UserID, NgayGhiNhan, CanNang, ChieuCao, SoDoVong1, SoDoVong2, SoDoVong3, SoDoBapTay, SoDoBapChan, TheTrang, BenhID, TrinhDoCaNhan, GhiChu)
VALUES
('rec_test1', 'user_test1', DATEADD(day, -20, GETDATE()), 70, 175, 90, 80, 95, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test2', 'user_test1', DATEADD(day, -15, GETDATE()), 69.5, 175, 89, 79, 94, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Giảm nhẹ'),
('rec_test3', 'user_test1', DATEADD(day, -10, GETDATE()), 69, 175, 89, 79, 93, 30, 40, N'Cân đối', 'benh_0005', N'Vừa phải', N'Ổn định'),
('rec_test4', 'user_test1', DATEADD(day, -5, GETDATE()), 68.5, 175, 88, 78, 93, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tiến bộ'),
('rec_test5', 'user_test1', GETDATE(), 68, 175, 88, 78, 92, 29, 39, N'Cân đối', 'benh_0005', N'Vừa phải', N'Tốt');
GO

-- MỤC TIÊU (1 mục tiêu - Giảm cân)
INSERT INTO MucTieu
(MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, TrangThai, PTID, GhiChu)
VALUES
('goal_test1', 'user_test1', N'Giảm cân', N'Giảm 3kg trong 2 tháng', 3, DATEADD(month, -1, GETDATE()), DATEADD(month, 1, GETDATE()), N'Đang thực hiện', NULL, N'Cố gắng mỗi ngày');
GO

PRINT '';
PRINT '============================================';
PRINT 'ĐÃ TẠO XONG USER VÀ DỮ LIỆU MẪU!';
PRINT '============================================';
PRINT 'Thông tin đăng nhập:';
PRINT 'Username: testuser';
PRINT 'Password: test123';
PRINT 'Email: testuser@example.com';
PRINT '============================================';
GO

