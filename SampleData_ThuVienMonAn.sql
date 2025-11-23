-- Script thêm dữ liệu mẫu vào bảng ThuVienMonAn
USE WF_HealthTracker;
GO

-- Xóa dữ liệu cũ nếu có (tùy chọn)
-- DELETE FROM ThuVienMonAn;
-- GO

-- Thêm dữ liệu mẫu
INSERT INTO ThuVienMonAn (MonAnID, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, imageURL)
VALUES
('food_0001', N'Thịt gà luộc', N'Thịt', N'g', 100, 165, 31, 0, 3.6, 0, 'galuoc.jpg'),
('food_0002', N'Thịt bò nướng', N'Thịt', N'g', 100, 250, 26, 0, 17, 0, NULL),
('food_0003', N'Thịt heo luộc', N'Thịt', N'g', 100, 242, 27, 0, 14, 0, NULL),
('food_0004', N'Cá hồi nướng', N'Hải sản', N'g', 100, 208, 20, 0, 12, 0, NULL),
('food_0005', N'Tôm luộc', N'Hải sản', N'g', 100, 99, 24, 0, 0.3, 0, NULL),
('food_0006', N'Cá ngừ nướng', N'Hải sản', N'g', 100, 184, 30, 0, 6, 0, NULL),
('food_0007', N'Bông cải xanh luộc', N'Rau củ', N'g', 100, 35, 2.8, 7, 0.4, 2.6, NULL),
('food_0008', N'Cà rốt luộc', N'Rau củ', N'g', 100, 41, 0.9, 10, 0.2, 2.8, NULL),
('food_0009', N'Rau muống xào', N'Rau củ', N'g', 100, 23, 2.6, 3.1, 0.2, 2.1, NULL),
('food_0010', N'Cà chua', N'Rau củ', N'g', 100, 18, 0.9, 3.9, 0.2, 1.2, NULL),
('food_0011', N'Táo', N'Trái cây', N'g', 100, 52, 0.3, 14, 0.2, 2.4, NULL),
('food_0012', N'Chuối', N'Trái cây', N'g', 100, 89, 1.1, 23, 0.3, 2.6, NULL),
('food_0013', N'Cam', N'Trái cây', N'g', 100, 47, 0.9, 12, 0.1, 2.4, NULL),
('food_0014', N'Nho', N'Trái cây', N'g', 100, 69, 0.7, 18, 0.2, 0.9, NULL),
('food_0015', N'Ức gà nướng', N'Thịt', N'g', 100, 165, 31, 0, 3.6, 0, NULL),
('food_0016', N'Cá basa nướng', N'Hải sản', N'g', 100, 162, 18, 0, 9, 0, NULL);
GO

-- Kiểm tra dữ liệu đã thêm
SELECT COUNT(*) AS 'Tổng số món ăn' FROM ThuVienMonAn;
SELECT * FROM ThuVienMonAn ORDER BY Loai, TenMonAn;
GO

