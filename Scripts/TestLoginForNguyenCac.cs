using System;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;

namespace HealthApp.Scripts
{
    /// <summary>
    /// Script để test và fix password cho user_0001 (nguyencac)
    /// </summary>
    public static class TestLoginForNguyenCac
    {
        /// <summary>
        /// Generate password hash mới cho nguyencac
        /// </summary>
        public static void GenerateNewPasswordHash()
        {
            string username = "nguyencac";
            string password = "nguyencac"; // Thay đổi password nếu cần
            
            string hash = PasswordHelper.HashPassword(password);
            
            Console.WriteLine("============================================");
            Console.WriteLine("Generate Password Hash for nguyencac");
            Console.WriteLine("============================================");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"New Hash: {hash}");
            Console.WriteLine();
            Console.WriteLine("SQL UPDATE statement:");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"UPDATE Users");
            Console.WriteLine($"SET PasswordHash = '{hash}'");
            Console.WriteLine($"WHERE UserID = 'user_0001' AND Username = '{username}';");
            Console.WriteLine("--------------------------------------------");
            
            // Copy vào clipboard
            try
            {
                Clipboard.SetText(hash);
                Console.WriteLine("Hash đã được copy vào clipboard!");
            }
            catch
            {
                // Ignore
            }
        }
        
        /// <summary>
        /// Test verify password với hash hiện tại trong database
        /// </summary>
        public static void TestCurrentPasswordHash()
        {
            string storedHash = "DYgp7y023mGyfTxXqZWcJpKQ0KeYLyHdF+1wq+f0VnBHjJ35ap9eCDfyqSwXX/bF";
            string[] testPasswords = { "nguyencac", "123456", "password", "admin" };
            
            Console.WriteLine("============================================");
            Console.WriteLine("Test Current Password Hash");
            Console.WriteLine("============================================");
            Console.WriteLine($"Stored Hash: {storedHash}");
            Console.WriteLine();
            
            foreach (string password in testPasswords)
            {
                try
                {
                    bool isValid = PasswordHelper.VerifyPassword(password, storedHash);
                    Console.WriteLine($"Password '{password}': {(isValid ? "VALID ✓" : "INVALID ✗")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Password '{password}': ERROR - {ex.Message}");
                }
            }
            Console.WriteLine("============================================");
        }
        
        /// <summary>
        /// Test login với các password phổ biến
        /// </summary>
        public static async void TestLogin()
        {
            string[] testPasswords = { "nguyencac", "123456", "password", "admin", "nguyencac123" };
            
            Console.WriteLine("============================================");
            Console.WriteLine("Test Login for nguyencac");
            Console.WriteLine("============================================");
            
            try
            {
                using (var authController = new AuthController())
                {
                    foreach (string password in testPasswords)
                    {
                        try
                        {
                            var result = await authController.LoginAsync("nguyencac", password);
                            Console.WriteLine($"Password '{password}': {(result.Success ? "SUCCESS ✓" : "FAILED ✗")} - {result.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Password '{password}': ERROR - {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Console.WriteLine("============================================");
        }
        
        /// <summary>
        /// Update password hash trong database (cần connection string)
        /// </summary>
        public static async void UpdatePasswordHashInDatabase(string newPassword)
        {
            try
            {
                using (var db = new Models.WF_HealthTracker())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserID == "user_0001" && u.Username == "nguyencac");
                    if (user != null)
                    {
                        string newHash = PasswordHelper.HashPassword(newPassword);
                        user.PasswordHash = newHash;
                        db.SaveChanges();
                        
                        Console.WriteLine("============================================");
                        Console.WriteLine("Password Updated Successfully!");
                        Console.WriteLine("============================================");
                        Console.WriteLine($"Username: {user.Username}");
                        Console.WriteLine($"New Hash: {newHash}");
                        Console.WriteLine("============================================");
                    }
                    else
                    {
                        Console.WriteLine("User not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
            }
        }
    }
}

