-- ============================================================
-- Script cập nhật Database WF_HealthTracker để phù hợp với Models
-- Ngày tạo: 2025
-- Mô tả: Cập nhật các thay đổi từ Models vào Database
-- ============================================================

USE WF_HealthTracker;
GO

-- ============================================================
-- 1. BẢNG DatLichPT: Thêm cột ThoiGianBatDau và ThoiGianKetThuc
-- ============================================================

-- Kiểm tra và thêm cột ThoiGianBatDau
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('DatLichPT') 
               AND name = 'ThoiGianBatDau')
BEGIN
    ALTER TABLE DatLichPT ADD ThoiGianBatDau DATETIME NULL;
    PRINT 'Da them cot ThoiGianBatDau';
END
ELSE 
BEGIN
    PRINT 'Cot ThoiGianBatDau da ton tai';
END
GO

-- Cập nhật dữ liệu cũ: Nếu ThoiGianBatDau NULL, lấy từ NgayGioDat
UPDATE DatLichPT
SET ThoiGianBatDau = NgayGioDat
WHERE ThoiGianBatDau IS NULL;
GO

-- Đặt NOT NULL cho ThoiGianBatDau
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID('DatLichPT') 
           AND name = 'ThoiGianBatDau'
           AND is_nullable = 1)
BEGIN
    ALTER TABLE DatLichPT
    ALTER COLUMN ThoiGianBatDau DATETIME NOT NULL;
    PRINT 'Da cap nhat ThoiGianBatDau thanh NOT NULL';
END
GO

-- Kiểm tra và thêm cột ThoiGianKetThuc
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('DatLichPT') 
               AND name = 'ThoiGianKetThuc')
BEGIN
    ALTER TABLE DatLichPT ADD ThoiGianKetThuc DATETIME NULL;
    PRINT 'Da them cot ThoiGianKetThuc';
END
ELSE 
BEGIN
    PRINT 'Cot ThoiGianKetThuc da ton tai';
END
GO

-- Cập nhật dữ liệu cũ: Tính từ NgayGioDat + ThoiLuong (mặc định 60 phút nếu NULL)
UPDATE DatLichPT
SET ThoiGianKetThuc = DATEADD(MINUTE, ISNULL(ThoiLuong, 60), NgayGioDat)
WHERE ThoiGianKetThuc IS NULL;
GO

-- Đặt NOT NULL cho ThoiGianKetThuc
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID('DatLichPT') 
           AND name = 'ThoiGianKetThuc'
           AND is_nullable = 1)
BEGIN
    ALTER TABLE DatLichPT
    ALTER COLUMN ThoiGianKetThuc DATETIME NOT NULL;
    PRINT 'Da cap nhat ThoiGianKetThuc thanh NOT NULL';
END
GO

-- Thêm ràng buộc CHECK cho ThoiGianKetThuc > ThoiGianBatDau
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DatLichPT_ThoiGian')
BEGIN
    ALTER TABLE DatLichPT
    ADD CONSTRAINT CK_DatLichPT_ThoiGian
        CHECK (ThoiGianKetThuc > ThoiGianBatDau);
    PRINT 'Da them rang buoc CK_DatLichPT_ThoiGian';
END
ELSE
BEGIN
    PRINT 'Rang buoc CK_DatLichPT_ThoiGian da ton tai';
END
GO

-- Cập nhật ràng buộc CK_DatLichPT_NgayGio để kiểm tra ThoiGianBatDau thay vì NgayGioDat
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DatLichPT_NgayGio')
BEGIN
    ALTER TABLE DatLichPT
    DROP CONSTRAINT CK_DatLichPT_NgayGio;
    PRINT 'Da xoa rang buoc CK_DatLichPT_NgayGio cu';
END
GO

-- Đặt thời gian đồng bộ vào tương lai nhẹ để tránh xung đột với GETDATE tại thời điểm thêm constraint
DECLARE @ThoiGianHienTai DATETIME = DATEADD(MINUTE, 1, GETDATE());

UPDATE DatLichPT
SET ThoiGianBatDau = @ThoiGianHienTai,
    ThoiGianKetThuc = DATEADD(MINUTE, ISNULL(ThoiLuong, 60), @ThoiGianHienTai)
WHERE ThoiGianBatDau < @ThoiGianHienTai;
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DatLichPT_ThoiGianBatDau')
BEGIN
    ALTER TABLE DatLichPT
    ADD CONSTRAINT CK_DatLichPT_ThoiGianBatDau
        CHECK (ThoiGianBatDau >= GETDATE());
    PRINT 'Da them rang buoc CK_DatLichPT_ThoiGianBatDau';
END
GO

-- ============================================================
-- 2. BẢNG Users: Đảm bảo cột SDT tồn tại và có thể sử dụng
-- ============================================================

-- Kiểm tra xem cột SDT đã tồn tại chưa
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Users') 
               AND name = 'SDT')
BEGIN
    ALTER TABLE Users ADD SDT NVARCHAR(20) NULL;
    PRINT 'Da them cot SDT vao bang Users';
END
ELSE
BEGIN
    PRINT 'Cot SDT da ton tai trong bang Users';
END
GO

-- Thêm UNIQUE constraint cho SDT nếu chưa có (theo SQL gốc)
IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE object_id = OBJECT_ID('Users') 
               AND name = 'UQ_Users_SDT')
BEGIN
    -- Kiểm tra xem có dữ liệu trùng lặp không
    IF NOT EXISTS (SELECT SDT, COUNT(*) as cnt 
                   FROM Users 
                   WHERE SDT IS NOT NULL 
                   GROUP BY SDT 
                   HAVING COUNT(*) > 1)
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX UQ_Users_SDT
        ON Users(SDT)
        WHERE SDT IS NOT NULL; -- Filtered index để cho phép nhiều NULL
        PRINT 'Da them UNIQUE constraint cho SDT (filtered index)';
    END
    ELSE
    BEGIN
        PRINT 'Canh bao: Co du lieu trung lap trong SDT, khong the them UNIQUE constraint';
    END
END
ELSE
BEGIN
    PRINT 'UNIQUE constraint cho SDT da ton tai';
END
GO

-- ============================================================
-- 3. BẢNG BaiTapChiTiet: Đảm bảo BaiTapChiTietID là PRIMARY KEY
-- ============================================================

-- Kiểm tra xem BaiTapChiTietID đã là PRIMARY KEY chưa
IF NOT EXISTS (SELECT * FROM sys.key_constraints 
               WHERE parent_object_id = OBJECT_ID('BaiTapChiTiet') 
               AND type = 'PK')
BEGIN
    -- Nếu chưa có PRIMARY KEY, thêm vào
    ALTER TABLE BaiTapChiTiet
    ADD CONSTRAINT PK_BaiTapChiTiet PRIMARY KEY (BaiTapChiTietID);
    PRINT 'Da them PRIMARY KEY cho BaiTapChiTiet';
END
ELSE
BEGIN
    PRINT 'BaiTapChiTiet da co PRIMARY KEY';
END
GO

-- ============================================================
-- 4. BẢNG BuaAnChiTiet: Đảm bảo BuaAnID là PRIMARY KEY
-- ============================================================

-- Kiểm tra xem BuaAnID đã là PRIMARY KEY chưa
IF NOT EXISTS (SELECT * FROM sys.key_constraints 
               WHERE parent_object_id = OBJECT_ID('BuaAnChiTiet') 
               AND type = 'PK')
BEGIN
    ALTER TABLE BuaAnChiTiet
    ADD CONSTRAINT PK_BuaAnChiTiet PRIMARY KEY (BuaAnID);
    PRINT 'Da them PRIMARY KEY cho BuaAnChiTiet';
END
ELSE
BEGIN
    PRINT 'BuaAnChiTiet da co PRIMARY KEY';
END
GO

-- ============================================================
-- 5. KIỂM TRA VÀ CẬP NHẬT CÁC RÀNG BUỘC CHECK CHO TrangThai
-- ============================================================

-- Kiểm tra ràng buộc CHECK cho DatLichPT.TrangThai
IF NOT EXISTS (SELECT * FROM sys.check_constraints 
               WHERE name = 'CK_DatLichPT_TrangThai')
BEGIN
    ALTER TABLE DatLichPT
    ADD CONSTRAINT CK_DatLichPT_TrangThai
        CHECK (TrangThai IN ('Pending', 'Confirmed', 'Completed', 'Cancelled'));
    PRINT 'Da them rang buoc CHECK cho DatLichPT.TrangThai';
END
GO

-- Kiểm tra ràng buộc CHECK cho DatLichPT.LoaiBuoiTap
IF NOT EXISTS (SELECT * FROM sys.check_constraints 
               WHERE name = 'CK_DatLichPT_LoaiBuoiTap')
BEGIN
    ALTER TABLE DatLichPT
    ADD CONSTRAINT CK_DatLichPT_LoaiBuoiTap
        CHECK (LoaiBuoiTap IN ('Online', 'In-person') OR LoaiBuoiTap IS NULL);
    PRINT 'Da them rang buoc CHECK cho DatLichPT.LoaiBuoiTap';
END
GO

-- ============================================================
-- 6. CẬP NHẬT CÁC CỘT NULLABLE/NOT NULL ĐỂ PHÙ HỢP VỚI MODELS
-- ============================================================

-- DatLichPT: NgayGioDat vẫn giữ NOT NULL (model có Required)
-- DatLichPT: ThoiLuong có thể NULL (model có int?)
-- DatLichPT: ThoiGianBatDau và ThoiGianKetThuc đã được xử lý ở trên

-- Users: SDT có thể NULL (model không có [Required])

-- ============================================================
-- 7. KIỂM TRA FOREIGN KEY CONSTRAINTS
-- ============================================================

-- Kiểm tra FK từ DatLichPT.NguoiHuy đến Users.UserID (nếu cần)
-- Lưu ý: Trong SQL gốc có comment là "RÀNG BUỘC BẰNG C#", 
-- nhưng nếu muốn thêm vào DB thì uncomment dòng dưới:
/*
IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
               WHERE name = 'FK_DatLichPT_NguoiHuy')
BEGIN
    ALTER TABLE DatLichPT
    ADD CONSTRAINT FK_DatLichPT_NguoiHuy 
        FOREIGN KEY (NguoiHuy) REFERENCES Users(UserID)
        ON DELETE NO ACTION;
    PRINT 'Da them FOREIGN KEY cho DatLichPT.NguoiHuy';
END
GO
*/

-- ============================================================
-- 8. CẬP NHẬT CÁC CỘT CÓ THỂ THIẾU
-- ============================================================

-- Kiểm tra cột NgayCapNhat trong các bảng
-- (Đã có trong SQL gốc, nhưng kiểm tra để chắc chắn)

-- DatLichPT.NgayCapNhat
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('DatLichPT') 
               AND name = 'NgayCapNhat')
BEGIN
    ALTER TABLE DatLichPT ADD NgayCapNhat DATETIME DEFAULT GETDATE();
    PRINT 'Da them cot NgayCapNhat vao DatLichPT';
END
GO

-- ============================================================
-- 9. TỔNG KẾT VÀ HƯỚNG DẪN TIẾP THEO
-- ============================================================

PRINT '========================================';
PRINT 'Hoan thanh cap nhat database!';
PRINT 'Cac thay doi chinh:';
PRINT '1. Them cot ThoiGianBatDau va ThoiGianKetThuc vao DatLichPT';
PRINT '2. Kiem tra va cap nhat cot SDT trong Users';
PRINT '3. Kiem tra PRIMARY KEY cho BaiTapChiTiet va BuaAnChiTiet';
PRINT '4. Cap nhat cac rang buoc CHECK';
PRINT '========================================';
PRINT '';
PRINT 'LUU Y: Sau khi chay script nay, can cap nhat code:';
PRINT '1. Trong file Models/WF_HealthTracker.cs, bo dong .Ignore(e => e.SDT)';
PRINT '2. Uncomment phan mapping cho SDT (dong 323-327)';
PRINT '========================================';
GO

