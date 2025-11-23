USE WF_HealthTracker;
GO

-----------------------------------------------------
-- 1) THÊM CỘT ThoiGianBatDau
-----------------------------------------------------
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
GO     -- BẮT BUỘC

-- Cập nhật dữ liệu cũ
UPDATE DatLichPT
SET ThoiGianBatDau = NgayGioDat
WHERE ThoiGianBatDau IS NULL;
GO

-- Đặt NOT NULL
ALTER TABLE DatLichPT
ALTER COLUMN ThoiGianBatDau DATETIME NOT NULL;
GO


-----------------------------------------------------
-- 2) THÊM CỘT ThoiGianKetThuc
-----------------------------------------------------
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
GO    -- BẮT BUỘC

-- Cập nhật dữ liệu cũ
UPDATE DatLichPT
SET ThoiGianKetThuc = DATEADD(MINUTE, ISNULL(ThoiLuong, 60), NgayGioDat)
WHERE ThoiGianKetThuc IS NULL;
GO

-- Đặt NOT NULL
ALTER TABLE DatLichPT
ALTER COLUMN ThoiGianKetThuc DATETIME NOT NULL;
GO


-----------------------------------------------------
-- 3) THÊM RÀNG BUỘC CHECK
-----------------------------------------------------
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

PRINT 'Hoan tat cap nhat bang DatLichPT!';
GO
