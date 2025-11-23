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
        /// Tìm món ăn trong thư viện
        /// </summary>
        public List<ThuVienMonAn> SearchFood(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return new List<ThuVienMonAn>();
                }

                keyword = keyword.Trim().ToLower();

                return _dbContext.ThuVienMonAn
                    .Where(m => m.TenMonAn.ToLower().Contains(keyword))
                    .OrderBy(m => m.TenMonAn)
                    .Take(50) // Giới hạn 50 kết quả
                    .ToList();
            }
            catch (Exception ex)
            {
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
        /// Tạo BuaAnID tự động (format: meal_0001, meal_0002, ...)
        /// </summary>
        private string GenerateMealId()
        {
            var lastMeal = _dbContext.BuaAnChiTiet
                .OrderByDescending(m => m.BuaAnID)
                .FirstOrDefault();

            if (lastMeal == null || !lastMeal.BuaAnID.StartsWith("meal_"))
            {
                return "meal_0001";
            }

            string numberPart = lastMeal.BuaAnID.Substring(5);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                return $"meal_{newNumber:D4}";
            }

            int mealCount = _dbContext.BuaAnChiTiet.Count();
            return $"meal_{(mealCount + 1):D4}";
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
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

