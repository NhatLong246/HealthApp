-- Script để sửa cột ThuNgay từ VARCHAR sang NVARCHAR và cập nhật dữ liệu bị lỗi
-- Chạy script này trên database WF_HealthTracker

USE WF_HealthTracker;
GO

-- Bước 1: Tạo cột tạm để lưu dữ liệu đã sửa
ALTER TABLE BuoiTap
ADD ThuNgay_Temp NVARCHAR(50);
GO

-- Bước 2: Chuyển đổi và sửa dữ liệu từ cột cũ sang cột tạm
-- Sửa các ký tự bị lỗi encoding (Th? -> Thứ)
UPDATE BuoiTap
SET ThuNgay_Temp = CASE 
    WHEN ThuNgay LIKE '%Th? 2%' OR ThuNgay LIKE '%Th?2%' OR ThuNgay = 'Th? 2' THEN N'Thứ 2'
    WHEN ThuNgay LIKE '%Th? 3%' OR ThuNgay LIKE '%Th?3%' OR ThuNgay = 'Th? 3' THEN N'Thứ 3'
    WHEN ThuNgay LIKE '%Th? 4%' OR ThuNgay LIKE '%Th?4%' OR ThuNgay = 'Th? 4' THEN N'Thứ 4'
    WHEN ThuNgay LIKE '%Th? 5%' OR ThuNgay LIKE '%Th?5%' OR ThuNgay = 'Th? 5' THEN N'Thứ 5'
    WHEN ThuNgay LIKE '%Th? 6%' OR ThuNgay LIKE '%Th?6%' OR ThuNgay = 'Th? 6' THEN N'Thứ 6'
    WHEN ThuNgay LIKE '%Th? 7%' OR ThuNgay LIKE '%Th?7%' OR ThuNgay = 'Th? 7' THEN N'Thứ 7'
    WHEN ThuNgay LIKE '%Ch?%' OR ThuNgay LIKE '%Ch? nhật%' THEN N'Chủ nhật'
    WHEN ThuNgay IS NOT NULL THEN CAST(ThuNgay AS NVARCHAR(50)) -- Chuyển đổi sang Unicode
    ELSE NULL
END;
GO

-- Bước 3: Xóa cột cũ và đổi tên cột tạm thành tên cũ
ALTER TABLE BuoiTap
DROP COLUMN ThuNgay;
GO

EXEC sp_rename 'BuoiTap.ThuNgay_Temp', 'ThuNgay', 'COLUMN';
GO

-- Bước 4: Kiểm tra kết quả
SELECT BuoiTapID, ThuNgay, ThoiGianBatDau, ThoiGianKetThuc, TrangThai
FROM BuoiTap
ORDER BY ThoiGianBatDau;
GO

PRINT N'Đã hoàn thành việc sửa cột ThuNgay từ VARCHAR sang NVARCHAR và cập nhật dữ liệu!';
GO

