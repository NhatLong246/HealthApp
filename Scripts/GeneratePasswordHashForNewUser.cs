using System;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Models;
using System.Linq;

namespace HealthApp.Scripts
{
    /// <summary>
    /// Script để generate password hash và tạo user mới với đầy đủ dữ liệu
    /// Chạy trong Immediate Window hoặc tạo form test
    /// </summary>
    public static class CreateNewTestUser
    {
        /// <summary>
        /// Generate password hash cho user mới
        /// </summary>
        public static void GeneratePasswordHash(string username = "testuser", string password = "test123")
        {
            string hash = PasswordHelper.HashPassword(password);
            
            Console.WriteLine("============================================");
            Console.WriteLine("Generate Password Hash for New User");
            Console.WriteLine("============================================");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash: {hash}");
            Console.WriteLine();
            Console.WriteLine("SQL INSERT statement:");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"INSERT INTO Users (UserID, Username, PasswordHash, Role, Email, SDT, HoTen, NgaySinh, GioiTinh, Theme, NgonNgu, CreatedDate)");
            Console.WriteLine($"VALUES ('user_test1', '{username}', '{hash}', 'Client', '{username}@example.com', '0909000999', N'Nguyễn Văn Test', '1995-01-01', 'Nam', 'Light', 'vi', GETDATE());");
            Console.WriteLine("--------------------------------------------");
            
            // Copy vào clipboard
            try
            {
                Clipboard.SetText(hash);
                Console.WriteLine("Hash đã được copy vào clipboard!");
            }
            catch { }
        }
        
        /// <summary>
        /// Tạo user mới và tất cả dữ liệu mẫu trong một lần
        /// </summary>
        public static void CreateUserWithAllData(string username = "testuser", string password = "test123")
        {
            try
            {
                string userId = "user_test1";
                
                using (var db = new WF_HealthTracker())
                {
                    // Kiểm tra user đã tồn tại chưa
                    var existingUser = db.Users.FirstOrDefault(u => u.UserID == userId || u.Username == username);
                    if (existingUser != null)
                    {
                        Console.WriteLine($"User đã tồn tại: {existingUser.Username}");
                        Console.WriteLine("Xóa user cũ? (Chạy DeleteTestUser() trước)");
                        return;
                    }
                    
                    Console.WriteLine("============================================");
                    Console.WriteLine("Đang tạo user mới...");
                    Console.WriteLine("============================================");
                    
                    // Tạo user mới
                    string hash = PasswordHelper.HashPassword(password);
                    var newUser = new Users
                    {
                        UserID = userId,
                        Username = username,
                        PasswordHash = hash,
                        Role = "Client",
                        Email = $"{username}@example.com",
                        SDT = "0909000999",
                        HoTen = "Nguyễn Văn Test",
                        NgaySinh = new DateTime(1995, 1, 1),
                        GioiTinh = "Nam",
                        Theme = "Light",
                        NgonNgu = "vi",
                        CreatedDate = DateTime.Now
                    };
                    
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    
                    Console.WriteLine($"✓ User created: {username} / {password}");
                    Console.WriteLine();
                    Console.WriteLine("Đang tạo dữ liệu mẫu...");
                    
                    // Tạo TinhTrangTongQuan
                    CreateSampleHealthRecords(db, userId);
                    Console.WriteLine("✓ TinhTrangTongQuan created");
                    
                    // Tạo MucTieu
                    string mucTieuId = CreateSampleGoal(db, userId);
                    Console.WriteLine($"✓ MucTieu created: {mucTieuId}");
                    
                    // Tạo KeHoachAnUong và BuaAnChiTiet
                    CreateSampleMealPlans(db, mucTieuId);
                    Console.WriteLine("✓ KeHoachAnUong & BuaAnChiTiet created");
                    
                    // Tạo KeHoachLuyenTap và BuoiTap
                    CreateSampleWorkoutPlans(db, userId, mucTieuId);
                    Console.WriteLine("✓ KeHoachLuyenTap & BuoiTap created");
                    
                    db.SaveChanges();
                    
                    Console.WriteLine();
                    Console.WriteLine("============================================");
                    Console.WriteLine("ĐÃ TẠO XONG TẤT CẢ DỮ LIỆU MẪU!");
                    Console.WriteLine("============================================");
                    Console.WriteLine($"UserID: {userId}");
                    Console.WriteLine($"Username: {username}");
                    Console.WriteLine($"Password: {password}");
                    Console.WriteLine($"Email: {newUser.Email}");
                    Console.WriteLine();
                    Console.WriteLine("Bây giờ bạn có thể đăng nhập với:");
                    Console.WriteLine($"Username: {username}");
                    Console.WriteLine($"Password: {password}");
                    Console.WriteLine("============================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
            }
        }
        
        private static void CreateSampleHealthRecords(WF_HealthTracker db, string userId)
        {
            // Đảm bảo có benh_0005 (Không có bệnh) - sử dụng raw SQL
            try
            {
                db.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM HoSoBenhLi WHERE BenhID = 'benh_0005')
                    BEGIN
                        INSERT INTO HoSoBenhLi (BenhID, TenBenh, LoaiBenh)
                        VALUES ('benh_0005', N'Không có bệnh', N'Khác');
                    END
                ");
            }
            catch { }
            
            for (int i = 1; i <= 5; i++)
            {
                var record = new TinhTrangTongQuan
                {
                    BanGhiID = $"rec_test{i}",
                    UserID = userId,
                    NgayGhiNhan = DateTime.Now.AddDays(-(6 - i) * 5),
                    CanNang = 70 - (i - 1) * 0.5,
                    ChieuCao = 175,
                    SoDoVong1 = 90 - (i - 1),
                    SoDoVong2 = 80 - (i - 1),
                    SoDoVong3 = 95 - (i - 1),
                    SoDoBapTay = 30,
                    SoDoBapChan = 40,
                    TheTrang = "Cân đối",
                    BenhID = "benh_0005",
                    TrinhDoCaNhan = "Vừa phải",
                    GhiChu = i == 5 ? "Tốt" : "Ổn định"
                };
                db.TinhTrangTongQuan.Add(record);
            }
        }
        
        private static string CreateSampleGoal(WF_HealthTracker db, string userId)
        {
            var goal = new MucTieu
            {
                MucTieuID = "goal_test1",
                UserID = userId,
                LoaiMucTieu = "Giảm cân",
                TenMucTieu = "Giảm 3kg trong 2 tháng",
                GiaTriMucTieu = 3,
                NgayBatDau = DateTime.Now.AddMonths(-1),
                NgayKetThucDuKien = DateTime.Now.AddMonths(1),
                TrangThai = "Đang thực hiện",
                GhiChu = "Cố gắng mỗi ngày"
            };
            db.MucTieu.Add(goal);
            return goal.MucTieuID;
        }
        
        private static void CreateSampleMealPlans(WF_HealthTracker db, string mucTieuId)
        {
            // Đảm bảo có món ăn trong database
            try
            {
                db.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM ThuVienMonAn WHERE MonAnID = 'food_0001')
                    BEGIN
                        INSERT INTO ThuVienMonAn (MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber)
                        VALUES 
                        ('food_0001', '', N'Ức gà', N'Thịt', 'g', 100, 165, 31, 0, 3.6, 0),
                        ('food_0002', '', N'Bông cải xanh', N'Rau củ', 'g', 100, 34, 3, 7, 0.4, 2.6),
                        ('food_0004', '', N'Cá hồi', N'Hải sản', 'g', 100, 208, 20, 0, 13, 0);
                    END
                ");
            }
            catch { }
            
            // Tạo kế hoạch ăn uống
            var mealPlan1 = new KeHoachAnUong
            {
                KeHoachAnID = "meal_test1",
                MucTieuID = mucTieuId,
                TongCalories = 1800,
                TongProtein = 120,
                TongCarbs = 150,
                TongFat = 60,
                Fiber = 25,
                MoTa = "Giảm cân - ngày 1",
                TrangThai = "Đang hoạt động"
            };
            db.KeHoachAnUong.Add(mealPlan1);
            
            // Tạo 3 bữa ăn chi tiết
            var meals = new[]
            {
                new { MealID = "meal_item_test1", MonAnID = "food_0001", LoaiBuaAn = "Sáng", TenMonAn = "Ức gà", Calories = 165.0, Protein = 31.0, Carbs = 0.0, Fat = 3.6, Fiber = 0.0 },
                new { MealID = "meal_item_test2", MonAnID = "food_0002", LoaiBuaAn = "Trưa", TenMonAn = "Bông cải xanh", Calories = 34.0, Protein = 3.0, Carbs = 7.0, Fat = 0.4, Fiber = 2.6 },
                new { MealID = "meal_item_test3", MonAnID = "food_0004", LoaiBuaAn = "Tối", TenMonAn = "Cá hồi", Calories = 208.0, Protein = 20.0, Carbs = 0.0, Fat = 13.0, Fiber = 0.0 }
            };
            
            foreach (var meal in meals)
            {
                var mealItem = new BuaAnChiTiet
                {
                    BuaAnID = meal.MealID,
                    KeHoachAnID = mealPlan1.KeHoachAnID,
                    MonAnID = meal.MonAnID,
                    LoaiBuaAn = meal.LoaiBuaAn,
                    NgayAn = DateTime.Now.Date,
                    TenMonAn = meal.TenMonAn,
                    Donvi = "g",
                    KhoiLuongChuan = 100,
                    Calories = meal.Calories,
                    Protein = meal.Protein,
                    Carbs = meal.Carbs,
                    Fat = meal.Fat,
                    Fiber = meal.Fiber
                };
                db.BuaAnChiTiet.Add(mealItem);
            }
        }
        
        private static void CreateSampleWorkoutPlans(WF_HealthTracker db, string userId, string mucTieuId)
        {
            // Đảm bảo có bài tập trong database
            try
            {
                db.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM ThuVienBaiTap WHERE BaiTapID = 'ex_0001')
                    BEGIN
                        INSERT INTO ThuVienBaiTap (BaiTapID, TenBaiTap, LoaiMucTieu, NhomCoChinhNhat, NhomCoPhu, CapDo, DungCu, MoTa, HuongDan, LuuY, NguoiTao)
                        VALUES 
                        ('ex_0001', N'Chống đẩy', N'Tăng cơ', N'Ngực', N'Tay sau', 'Beginner', N'Không dụng cụ', N'Mô tả chống đẩy', N'Hướng dẫn từng bước', N'Lưu ý tư thế', NULL),
                        ('ex_0002', N'Kéo xà', N'Tăng cơ', N'Lưng', N'Tay trước', 'Intermediate', N'Xà đơn', N'Mô tả kéo xà', N'Hướng dẫn từng bước', N'Lưu ý an toàn', NULL);
                    END
                ");
            }
            catch { }
            
            // Tạo kế hoạch luyện tập
            var workoutPlan = new KeHoachLuyenTap
            {
                KeHoachTapID = "workout_test1",
                UserID = userId,
                MucTieuID = mucTieuId,
                TongCalories = 0,
                CapDo = "Beginner",
                TrangThai = "Đang hoạt động",
                MoTa = "Giảm cân"
            };
            db.KeHoachLuyenTap.Add(workoutPlan);
            
            // Tạo 3 buổi tập
            var sessions = new[]
            {
                new { SessionID = "session_test1", DaysAgo = 6, Calories = 350 },
                new { SessionID = "session_test2", DaysAgo = 4, Calories = 380 },
                new { SessionID = "session_test3", DaysAgo = 2, Calories = 400 }
            };
            
            foreach (var session in sessions)
            {
                var date = DateTime.Now.AddDays(-session.DaysAgo);
                var buoiTap = new BuoiTap
                {
                    BuoiTapID = session.SessionID,
                    KeHoachTapID = workoutPlan.KeHoachTapID,
                    ThuNgay = "Thứ 2",
                    ThoiGianBatDau = date,
                    ThoiGianKetThuc = date.AddHours(1),
                    TrangThai = "Hoàn thành",
                    Calories = session.Calories,
                    NgayThucHien = date
                };
                db.BuoiTap.Add(buoiTap);
                
                // Tạo bài tập chi tiết
                if (session.SessionID == "session_test1")
                {
                    var exercise1 = new BaiTapChiTiet
                    {
                        BaiTapChiTietID = "detail_test11",
                        BuoiTapID = session.SessionID,
                        BaiTapID = "ex_0001",
                        SoSet = 3,
                        SoRep = 12,
                        ThoiLuongDeNghi = 30,
                        ThoiGianNghi = 60,
                        TrongLuong = 0,
                        Calories = 120,
                        ThuTuThucHien = 1,
                        TrangThai = "Hoàn thành"
                    };
                    db.BaiTapChiTiet.Add(exercise1);
                }
            }
        }
        
        /// <summary>
        /// Xóa user test và tất cả dữ liệu liên quan
        /// </summary>
        public static void DeleteTestUser()
        {
            try
            {
                string userId = "user_test1";
                
                using (var db = new WF_HealthTracker())
                {
                    // Xóa theo thứ tự để tránh foreign key constraint
                    db.Database.ExecuteSqlCommand(@"
                        DELETE FROM BaiTapChiTiet WHERE BuoiTapID IN (SELECT BuoiTapID FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = {0}));
                        DELETE FROM BuoiTap WHERE KeHoachTapID IN (SELECT KeHoachTapID FROM KeHoachLuyenTap WHERE UserID = {0});
                        DELETE FROM KeHoachLuyenTap WHERE UserID = {0};
                        DELETE FROM BuaAnChiTiet WHERE KeHoachAnID IN (SELECT KeHoachAnID FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = {0}));
                        DELETE FROM KeHoachAnUong WHERE MucTieuID IN (SELECT MucTieuID FROM MucTieu WHERE UserID = {0});
                        DELETE FROM MucTieu WHERE UserID = {0};
                        DELETE FROM TinhTrangTongQuan WHERE UserID = {0};
                        DELETE FROM Users WHERE UserID = {0};
                    ", userId);
                    
                    Console.WriteLine($"Đã xóa user {userId} và tất cả dữ liệu liên quan!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

