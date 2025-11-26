USE WF_HealthTracker;
GO

-- ============================================
-- KIỂM TRA VÀ THÊM COLUMN SDT NẾU CHƯA CÓ
-- ============================================

-- Kiểm tra xem column SDT đã tồn tại chưa
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Users' 
    AND COLUMN_NAME = 'SDT'
)
BEGIN
    PRINT 'Column SDT chưa tồn tại. Đang thêm column...';
    
    -- Thêm column SDT
    ALTER TABLE Users
    ADD SDT NVARCHAR(20) NULL;
    
    -- Thêm unique constraint nếu cần (tùy chọn)
    -- ALTER TABLE Users
    -- ADD CONSTRAINT UQ_Users_SDT UNIQUE (SDT);
    
    PRINT 'Đã thêm column SDT thành công!';
END
ELSE
BEGIN
    PRINT 'Column SDT đã tồn tại.';
END
GO

-- Kiểm tra lại
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
AND COLUMN_NAME = 'SDT';
GO

