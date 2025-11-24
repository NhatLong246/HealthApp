USE WF_HealthTracker;
GO

-- ============================================
-- CẬP NHẬT PASSWORD HASH CHO USER_0001 (nguyencac)
-- ============================================

-- Kiểm tra user có tồn tại không
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = 'user_0001' AND Username = 'nguyencac')
BEGIN
    PRINT 'ERROR: User_0001 với username "nguyencac" không tồn tại!';
    RETURN;
END

PRINT 'Đã tìm thấy user_0001 (nguyencac)';
PRINT '';

-- ============================================
-- HƯỚNG DẪN:
-- ============================================
-- Password hash hiện tại có thể không đúng format PBKDF2
-- Cần generate password hash mới bằng code C#
--
-- Bước 1: Chạy code C# sau trong ứng dụng (debug console hoặc form test):
--
-- using HealthApp.Common.Helpers;
-- string password = "nguyencac"; // hoặc password bạn muốn đặt
-- string hash = PasswordHelper.HashPassword(password);
-- System.Diagnostics.Debug.WriteLine($"Password: {password}");
-- System.Diagnostics.Debug.WriteLine($"Hash: {hash}");
--
-- Bước 2: Copy hash được in ra và thay thế <NEW_PASSWORD_HASH> bên dưới
-- Bước 3: Uncomment và chạy UPDATE statement

-- ============================================
-- UPDATE PASSWORD HASH
-- ============================================
-- Thay thế <NEW_PASSWORD_HASH> bằng hash mới được generate từ code C#
-- UPDATE Users 
-- SET PasswordHash = '<NEW_PASSWORD_HASH>'
-- WHERE UserID = 'user_0001' AND Username = 'nguyencac';
--
-- PRINT 'Đã cập nhật password hash thành công!';

-- ============================================
-- HOẶC: Sử dụng form đăng ký/đổi mật khẩu
-- ============================================
-- 1. Chạy ứng dụng
-- 2. Sử dụng chức năng "Quên mật khẩu" hoặc "Đổi mật khẩu" nếu có
-- 3. Hoặc đăng ký user mới với username "nguyencac" và password mong muốn
-- 4. Sau đó update UserID thành 'user_0001' nếu cần:
--    UPDATE Users SET UserID = 'user_0001' WHERE Username = 'nguyencac';

PRINT 'Vui lòng generate password hash mới bằng code C# (xem hướng dẫn trên)';
PRINT 'Sau đó thay thế <NEW_PASSWORD_HASH> và uncomment UPDATE statement.';
GO

