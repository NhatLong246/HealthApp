-- Script để thêm trường LichTrinhID vào bảng DatLichPT
-- Mục đích: Nhóm các buổi tập cùng một lịch trình lại với nhau

USE WF_HealthTracker;
GO

-- Thêm cột LichTrinhID để nhóm các buổi tập
ALTER TABLE DatLichPT
ADD LichTrinhID VARCHAR(20) NULL;
GO

-- Tạo index để tăng hiệu suất truy vấn theo LichTrinhID
CREATE NONCLUSTERED INDEX IX_DatLichPT_LichTrinhID
ON DatLichPT(LichTrinhID)
WHERE LichTrinhID IS NOT NULL;
GO

-- Thêm comment cho cột
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID của lịch trình (package) để nhóm các buổi tập lại với nhau. NULL nếu là buổi tập đơn lẻ.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'DatLichPT', 
    @level2type = N'COLUMN', @level2name = N'LichTrinhID';
GO

-- Sửa lỗi: Xóa trường MucTieuLuyenTap bị trùng (dòng 357)
-- Lưu ý: Cần kiểm tra xem có dữ liệu nào đang sử dụng trường thứ 2 không
-- Nếu không có dữ liệu, có thể chạy lệnh sau:
-- ALTER TABLE DatLichPT DROP COLUMN [MucTieuLuyenTap_duplicate]; -- Nếu có tên cột trùng
