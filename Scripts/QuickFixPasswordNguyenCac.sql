USE WF_HealthTracker;
GO

-- ============================================
-- QUICK FIX: CẬP NHẬT PASSWORD CHO nguyencac
-- ============================================

PRINT '============================================';
PRINT 'CẬP NHẬT PASSWORD HASH CHO nguyencac';
PRINT '============================================';
PRINT '';

-- Kiểm tra user
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = 'user_0001' AND Username = 'nguyencac')
BEGIN
    PRINT 'ERROR: User không tồn tại!';
    RETURN;
END

PRINT 'Đã tìm thấy user_0001 (nguyencac)';
PRINT '';

-- ============================================
-- CÁCH 1: Generate hash bằng code C# (KHUYẾN NGHỊ)
-- ============================================
PRINT 'CÁCH 1: Generate hash bằng code C#';
PRINT '-----------------------------------';
PRINT '1. Mở ứng dụng trong Visual Studio';
PRINT '2. Trong Immediate Window hoặc Debug Console, chạy:';
PRINT '';
PRINT '   using HealthApp.Common.Helpers;';
PRINT '   string hash = PasswordHelper.HashPassword("nguyencac");';
PRINT '   System.Diagnostics.Debug.WriteLine(hash);';
PRINT '';
PRINT '3. Copy hash được in ra';
PRINT '4. Thay thế <HASH> trong UPDATE statement bên dưới';
PRINT '5. Uncomment và chạy UPDATE statement';
PRINT '';

-- UPDATE Users 
-- SET PasswordHash = '<HASH>'
-- WHERE UserID = 'user_0001' AND Username = 'nguyencac';

-- ============================================
-- CÁCH 2: Sử dụng chức năng "Quên mật khẩu" trong ứng dụng
-- ============================================
PRINT 'CÁCH 2: Sử dụng form "Quên mật khẩu"';
PRINT '-----------------------------------';
PRINT '1. Chạy ứng dụng';
PRINT '2. Click "Quên mật khẩu"';
PRINT '3. Nhập email: nloc123@gmail.com';
PRINT '4. Làm theo hướng dẫn để reset password';
PRINT '';

-- ============================================
-- CÁCH 3: Đăng ký user mới và update UserID
-- ============================================
PRINT 'CÁCH 3: Đăng ký user mới';
PRINT '-----------------------------------';
PRINT '1. Chạy ứng dụng';
PRINT '2. Đăng ký với username "nguyencac" và password mong muốn';
PRINT '3. Sau đó chạy SQL để update UserID:';
PRINT '';
PRINT '   UPDATE Users SET UserID = ''user_0001'' WHERE Username = ''nguyencac'' AND UserID != ''user_0001'';';
PRINT '   -- Xóa user cũ nếu có';
PRINT '   DELETE FROM Users WHERE UserID = ''user_0001'' AND Username != ''nguyencac'';';
PRINT '';

PRINT '============================================';
PRINT 'Sau khi cập nhật, thử đăng nhập với:';
PRINT 'Username: nguyencac';
PRINT 'Password: (password bạn đã set)';
PRINT '============================================';
GO

