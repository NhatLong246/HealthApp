using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Common.Helpers;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Dinh Dưỡng (Nutrition)
    /// </summary>
    public class NutritionController : IDisposable
    {
        private readonly WF_HealthTracker _dbContext;

        public NutritionController()
        {
            _dbContext = new WF_HealthTracker();
        }

        /// <summary>
        /// Lấy danh sách bữa ăn theo ngày
        /// </summary>
        /// <param name="ngayAn">Ngày cần lấy</param>
        /// <returns>Danh sách bữa ăn</returns>
        public List<BuaAnChiTiet> GetMealsByDate(DateTime ngayAn)
        {
            try
            {
                var ngayBatDau = ngayAn.Date;
                var ngayKetThuc = ngayAn.Date.AddDays(1).AddTicks(-1);

                return _dbContext.BuaAnChiTiet
                    .Where(b => b.NgayAn >= ngayBatDau && b.NgayAn < ngayKetThuc)
                    .OrderBy(b => b.NgayAn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách bữa ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách bữa ăn theo loại bữa ăn
        /// </summary>
        public List<BuaAnChiTiet> GetMealsByType(DateTime ngayAn, string loaiBuaAn)
        {
            try
            {
                var ngayBatDau = ngayAn.Date;
                var ngayKetThuc = ngayAn.Date.AddDays(1).AddTicks(-1);

                return _dbContext.BuaAnChiTiet
                    .Where(b => b.NgayAn >= ngayBatDau && 
                           b.NgayAn < ngayKetThuc &&
                           b.LoaiBuaAn == loaiBuaAn)
                    .OrderBy(b => b.NgayAn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách bữa ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính tổng dinh dưỡng trong ngày
        /// </summary>
        public NutritionSummary CalculateDailyNutrition(DateTime ngayAn)
        {
            try
            {
                var meals = GetMealsByDate(ngayAn);

                var summary = new NutritionSummary
                {
                    Date = ngayAn,
                    TotalCalories = meals.Sum(m => m.Calories ?? 0),
                    TotalProtein = meals.Sum(m => m.Protein ?? 0),
                    TotalCarbs = meals.Sum(m => m.Carbs ?? 0),
                    TotalFat = meals.Sum(m => m.Fat ?? 0),
                    TotalFiber = meals.Sum(m => m.Fiber ?? 0),
                    MealCount = meals.Count
                };

                return summary;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính tổng dinh dưỡng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách món ăn chỉ với các cột cần thiết (MonAnID, TenMonAn, Loai, Donvi)
        /// </summary>
        public List<FoodListItem> GetAllFoodsList()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== GetAllFoodsList ===");
                
                // Load về memory trước để tránh lỗi EF translation
                var allFoods = _dbContext.ThuVienMonAn
                    .OrderBy(m => m.TenMonAn)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allFoods.Count} foods from database");

                // Select các cột cần thiết trong memory
                var foods = allFoods
                    .Select(m => new FoodListItem
                    {
                        MonAnID = m.MonAnID,
                        TenMonAn = m.TenMonAn,
                        Loai = m.Loai,
                        Donvi = m.Donvi
                    })
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Processed {foods.Count} foods (basic info only)");
                return foods;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllFoodsList error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                throw new Exception($"Lỗi khi tải danh sách món ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm món ăn trong thư viện (chỉ lấy các cột cần thiết)
        /// </summary>
        public List<FoodListItem> SearchFoodList(string keyword)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== SearchFoodList - Keyword: '{keyword}' ===");
                
                // Load về memory trước để tránh lỗi EF translation
                var allFoods = _dbContext.ThuVienMonAn
                    .OrderBy(m => m.TenMonAn)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allFoods.Count} foods from database");

                // Select các cột cần thiết trong memory
                var foodList = allFoods
                    .Select(m => new FoodListItem
                    {
                        MonAnID = m.MonAnID,
                        TenMonAn = m.TenMonAn,
                        Loai = m.Loai,
                        Donvi = m.Donvi
                    })
                    .ToList();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Nếu keyword rỗng, trả về 100 món đầu tiên
                    var foodResults = foodList.Take(100).ToList();
                    System.Diagnostics.Debug.WriteLine($"Returned {foodResults.Count} foods (no keyword)");
                    return foodResults;
                }

                keyword = keyword.Trim().ToLower();
                
                // Filter trong memory
                var filteredResults = foodList
                    .Where(m => 
                        (m.TenMonAn != null && m.TenMonAn.ToLower().Contains(keyword)) || 
                        (m.Loai != null && m.Loai.ToLower().Contains(keyword)))
                    .OrderBy(m => m.TenMonAn)
                    .Take(100)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"Found {filteredResults.Count} matching foods");
                return filteredResults;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchFoodList error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                throw new Exception($"Lỗi khi tìm món ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm món ăn trong thư viện (full object - dùng khi cần chi tiết)
        /// </summary>
        public List<ThuVienMonAn> SearchFood(string keyword)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== SearchFood - Keyword: '{keyword}' ===");
                
                // Load tất cả dữ liệu về memory trước (giới hạn 200 để tránh quá tải)
                var allFoods = _dbContext.ThuVienMonAn
                    .OrderBy(m => m.TenMonAn)
                    .Take(200) // Giới hạn 200 kết quả để tránh quá tải memory
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allFoods.Count} foods from database");

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Nếu keyword rỗng, trả về 100 món ăn đầu tiên
                    return allFoods.Take(100).ToList();
                }

                keyword = keyword.Trim().ToLower();

                // Filter trong memory (không còn vấn đề với EF translation)
                var results = allFoods
                    .Where(m => 
                        (m.TenMonAn != null && m.TenMonAn.ToLower().Contains(keyword)) || 
                        (m.Loai != null && m.Loai.ToLower().Contains(keyword)))
                    .OrderBy(m => m.TenMonAn)
                    .Take(50) // Giới hạn 50 kết quả
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {results.Count} matching foods");
                return results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchFood error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                throw new Exception($"Lỗi khi tìm món ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy món ăn theo ID
        /// </summary>
        public ThuVienMonAn GetFoodById(string monAnId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(monAnId))
                {
                    return null;
                }

                return _dbContext.ThuVienMonAn.FirstOrDefault(m => m.MonAnID == monAnId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy món ăn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm bữa ăn mới
        /// </summary>
        public async Task<NutritionResult> AddMealAsync(
            string keHoachAnId,
            string monAnId,
            string loaiBuaAn,
            DateTime ngayAn,
            string tenMonAn,
            double? khoiLuongChuan = null,
            string donVi = null,
            string ghiChu = null)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(keHoachAnId))
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "KeHoachAnID không được để trống!"
                    };
                }

                if (string.IsNullOrWhiteSpace(monAnId))
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "MonAnID không được để trống!"
                    };
                }

                if (string.IsNullOrWhiteSpace(loaiBuaAn))
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "Loại bữa ăn không được để trống!"
                    };
                }

                // Lấy thông tin món ăn từ thư viện
                var monAn = GetFoodById(monAnId);
                if (monAn == null)
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "Không tìm thấy món ăn trong thư viện!"
                    };
                }

                // Tính toán dinh dưỡng dựa trên khối lượng
                double heSo = 1.0;
                if (khoiLuongChuan.HasValue && monAn.KhoiLuongChuan.HasValue && monAn.KhoiLuongChuan.Value > 0)
                {
                    heSo = khoiLuongChuan.Value / monAn.KhoiLuongChuan.Value;
                }

                // Tạo BuaAnID tự động
                string buaAnId = GenerateMealId();

                // Tạo bữa ăn mới
                var newMeal = new BuaAnChiTiet
                {
                    BuaAnID = buaAnId,
                    KeHoachAnID = keHoachAnId.Trim(),
                    MonAnID = monAnId.Trim(),
                    LoaiBuaAn = loaiBuaAn.Trim(),
                    NgayAn = ngayAn,
                    TenMonAn = string.IsNullOrWhiteSpace(tenMonAn) ? monAn.TenMonAn : tenMonAn.Trim(),
                    Donvi = donVi ?? monAn.Donvi,
                    KhoiLuongChuan = khoiLuongChuan ?? monAn.KhoiLuongChuan,
                    Calories = (monAn.Calories ?? 0) * heSo,
                    Protein = (monAn.Protein ?? 0) * heSo,
                    Carbs = (monAn.Carbs ?? 0) * heSo,
                    Fat = (monAn.Fat ?? 0) * heSo,
                    Fiber = (monAn.Fiber ?? 0) * heSo,
                    GhiChu = ghiChu?.Trim(),
                    NgayCapNhat = DateTime.Now
                };

                _dbContext.BuaAnChiTiet.Add(newMeal);
                await Task.Run(() => _dbContext.SaveChanges());

                return new NutritionResult
                {
                    Success = true,
                    Message = "Thêm bữa ăn thành công!",
                    Meal = newMeal
                };
            }
            catch (Exception ex)
            {
                return new NutritionResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xóa bữa ăn
        /// </summary>
        public async Task<NutritionResult> DeleteMealAsync(string buaAnId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(buaAnId))
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "BuaAnID không được để trống!"
                    };
                }

                var meal = _dbContext.BuaAnChiTiet.FirstOrDefault(b => b.BuaAnID == buaAnId);
                if (meal == null)
                {
                    return new NutritionResult
                    {
                        Success = false,
                        Message = "Không tìm thấy bữa ăn!"
                    };
                }

                _dbContext.BuaAnChiTiet.Remove(meal);
                await Task.Run(() => _dbContext.SaveChanges());

                return new NutritionResult
                {
                    Success = true,
                    Message = "Xóa bữa ăn thành công!"
                };
            }
            catch (Exception ex)
            {
                return new NutritionResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tạo BuaAnID tự động (format: meal_item_0001, meal_item_0002, ...)
        /// </summary>
        private string GenerateMealId()
        {
            var lastMeal = _dbContext.BuaAnChiTiet
                .OrderByDescending(m => m.BuaAnID)
                .FirstOrDefault();

            if (lastMeal == null || !lastMeal.BuaAnID.StartsWith("meal_item_"))
            {
                return "meal_item_0001";
            }

            string numberPart = lastMeal.BuaAnID.Substring(10);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                return $"meal_item_{newNumber:D4}";
            }

            int mealCount = _dbContext.BuaAnChiTiet.Count();
            return $"meal_item_{(mealCount + 1):D4}";
        }

        /// <summary>
        /// Tạo kế hoạch ăn uống mới
        /// </summary>
        public async Task<string> CreateMealPlanAsync(
            string mucTieuId,
            double? tongCalories = null,
            double? tongProtein = null,
            double? tongCarbs = null,
            double? tongFat = null,
            double? tongFiber = null,
            string moTa = null)
        {
            try
            {
                // Tạo KeHoachAnID
                string keHoachAnId = GenerateKeHoachAnId();

                var keHoachAn = new KeHoachAnUong
                {
                    KeHoachAnID = keHoachAnId,
                    MucTieuID = mucTieuId,
                    TongCalories = tongCalories,
                    TongProtein = tongProtein,
                    TongCarbs = tongCarbs,
                    TongFat = tongFat,
                    Fiber = tongFiber,
                    MoTa = moTa ?? "Kế hoạch ăn uống",
                    TrangThai = "Đang hoạt động"
                };

                _dbContext.KeHoachAnUong.Add(keHoachAn);
                _dbContext.SaveChanges();

                return await Task.FromResult(keHoachAnId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateMealPlanAsync error: {ex.Message}");
                throw new Exception($"Lỗi khi tạo kế hoạch ăn uống: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm món ăn vào kế hoạch ăn uống
        /// </summary>
        public async Task<bool> AddMealToPlanAsync(
            string keHoachAnId,
            string monAnId,
            string loaiBuaAn,
            DateTime ngayAn,
            string tenMonAn,
            double? khoiLuongChuan = null,
            string donVi = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keHoachAnId) || string.IsNullOrWhiteSpace(monAnId))
                {
                    return false;
                }

                // Lấy thông tin món ăn gốc
                var monAnGoc = GetFoodById(monAnId);
                if (monAnGoc == null)
                {
                    return false;
                }

                // Tính toán dinh dưỡng dựa trên khối lượng
                double tiLe = 1.0;
                if (khoiLuongChuan.HasValue && monAnGoc.KhoiLuongChuan.HasValue && monAnGoc.KhoiLuongChuan.Value > 0)
                {
                    tiLe = khoiLuongChuan.Value / monAnGoc.KhoiLuongChuan.Value;
                }

                // Tạo BuaAnID
                string buaAnId = GenerateMealId();

                var buaAn = new BuaAnChiTiet
                {
                    BuaAnID = buaAnId,
                    KeHoachAnID = keHoachAnId,
                    MonAnID = monAnId,
                    LoaiBuaAn = loaiBuaAn ?? "Trưa",
                    NgayAn = ngayAn,
                    TenMonAn = tenMonAn ?? monAnGoc.TenMonAn,
                    Donvi = donVi ?? monAnGoc.Donvi ?? "g",
                    KhoiLuongChuan = khoiLuongChuan ?? monAnGoc.KhoiLuongChuan,
                    Calories = (monAnGoc.Calories ?? 0) * tiLe,
                    Protein = (monAnGoc.Protein ?? 0) * tiLe,
                    Carbs = (monAnGoc.Carbs ?? 0) * tiLe,
                    Fat = (monAnGoc.Fat ?? 0) * tiLe,
                    Fiber = (monAnGoc.Fiber ?? 0) * tiLe
                };

                _dbContext.BuaAnChiTiet.Add(buaAn);
                _dbContext.SaveChanges();

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddMealToPlanAsync error: {ex.Message}");
                throw new Exception($"Lỗi khi thêm món ăn vào kế hoạch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo KeHoachAnID tự động
        /// </summary>
        private string GenerateKeHoachAnId()
        {
            var last = _dbContext.KeHoachAnUong
                .OrderByDescending(k => k.KeHoachAnID)
                .FirstOrDefault();

            if (last == null || !last.KeHoachAnID.StartsWith("meal_"))
            {
                return "meal_0001";
            }

            string numberPart = last.KeHoachAnID.Substring(5);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                return $"meal_{newNumber:D4}";
            }

            int count = _dbContext.KeHoachAnUong.Count();
            return $"meal_{(count + 1):D4}";
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    /// <summary>
    /// DTO cho danh sách món ăn (chỉ các cột cần thiết)
    /// </summary>
    public class FoodListItem
    {
        public string MonAnID { get; set; }
        public string TenMonAn { get; set; }
        public string Loai { get; set; }
        public string Donvi { get; set; }
    }

    /// <summary>
    /// Tổng hợp dinh dưỡng trong ngày
    /// </summary>
    public class NutritionSummary
    {
        public DateTime Date { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalFat { get; set; }
        public double TotalFiber { get; set; }
        public int MealCount { get; set; }
    }

    /// <summary>
    /// Kết quả thao tác với dinh dưỡng
    /// </summary>
    public class NutritionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public BuaAnChiTiet Meal { get; set; }
    }
}

