USE WF_HealthTracker;
GO

-- ============================================
-- KIỂM TRA EMAIL TRONG DATABASE
-- ============================================

PRINT '============================================';
PRINT 'KIỂM TRA EMAIL TRONG DATABASE';
PRINT '============================================';
PRINT '';

-- Kiểm tra user_0001
SELECT 
    UserID,
    Username,
    Email,
    LEN(Email) AS EmailLength,
    LEN(LTRIM(RTRIM(Email))) AS TrimmedEmailLength,
    LTRIM(RTRIM(Email)) AS TrimmedEmail,
    LOWER(LTRIM(RTRIM(Email))) AS LowercaseEmail
FROM Users
WHERE UserID = 'user_0001';
GO

PRINT '';
PRINT 'Kiểm tra email cụ thể:';
PRINT '';

-- Kiểm tra email có tồn tại không (case-insensitive)
DECLARE @SearchEmail NVARCHAR(100) = 'nloc123@gmail.com';
DECLARE @SearchEmailLower NVARCHAR(100) = LOWER(LTRIM(RTRIM(@SearchEmail)));

SELECT 
    UserID,
    Username,
    Email,
    CASE 
        WHEN LOWER(LTRIM(RTRIM(Email))) = @SearchEmailLower THEN 'MATCH ✓'
        ELSE 'NO MATCH ✗'
    END AS MatchStatus
FROM Users
WHERE LOWER(LTRIM(RTRIM(Email))) = @SearchEmailLower;
GO

PRINT '';
PRINT 'Tất cả users có email:';
PRINT '';

SELECT 
    UserID,
    Username,
    Email,
    LEN(Email) AS EmailLength
FROM Users
WHERE Email IS NOT NULL AND Email != '';
GO

