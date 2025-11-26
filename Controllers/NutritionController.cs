using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using HealthApp.Services;
using HealthApp.Controllers;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Dinh Dưỡng (Nutrition)
    /// </summary>
    public class NutritionController : IDisposable
    {
        private readonly WF_HealthTracker _dbContext;
        private readonly ChatGPTService _chatGPTService;
        private readonly GoalController _goalController;

        public NutritionController()
        {
            _dbContext = new WF_HealthTracker();
            _chatGPTService = new ChatGPTService();
            _goalController = new GoalController();
        }

        /// <summary>
        /// Lấy danh sách bữa ăn theo ngày (dùng raw SQL để tránh Entity Framework mapping issues)
        /// </summary>
        /// <param name="ngayAn">Ngày cần lấy</param>
        /// <returns>Danh sách bữa ăn</returns>
        public List<BuaAnChiTiet> GetMealsByDate(DateTime ngayAn)
        {
            try
            {
                var ngayBatDau = ngayAn.Date;
                var ngayKetThuc = ngayAn.Date.AddDays(1).AddTicks(-1);

                // Dùng raw SQL để tránh Entity Framework mapping issues
                string sqlQuery = @"SELECT BuaAnID, UserID, MonAnID, NgayAn, LoaiBuaAn, KhoiLuongChuan, 
                                           Calories, Protein, Carbs, Fat, Fiber, GhiChu, KeHoachAnID 
                                    FROM BuaAnChiTiet 
                                    WHERE NgayAn >= @p0 AND NgayAn < @p1 
                                    ORDER BY NgayAn";

                var meals = _dbContext.Database.SqlQuery<BuaAnChiTiet>(sqlQuery, ngayBatDau, ngayKetThuc).ToList();
                
                System.Diagnostics.Debug.WriteLine($"GetMealsByDate: Lấy được {meals?.Count ?? 0} bữa ăn cho ngày {ngayAn:yyyy-MM-dd}");
                
                return meals ?? new List<BuaAnChiTiet>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetMealsByDate: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                // Trả về danh sách rỗng thay vì throw exception để không crash ứng dụng
                return new List<BuaAnChiTiet>();
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
        /// Lấy toàn bộ món ăn với đầy đủ thông tin (dùng cho UI cache)
        /// </summary>
        public List<ThuVienMonAn> GetAllFoods()
        {
            try
            {
                return _dbContext.ThuVienMonAn
                    .OrderBy(m => m.TenMonAn)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllFoods error: {ex.Message}");
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
        /// Lấy danh sách món ăn đề xuất từ thư viện (dùng AI nếu có mục tiêu)
        /// </summary>
        /// <param name="loaiBuaAn">Loại bữa ăn (Sáng, Trưa, Tối)</param>
        /// <param name="soLuong">Số lượng món ăn cần lấy</param>
        /// <returns>Danh sách món ăn đề xuất</returns>
        public async Task<List<ThuVienMonAn>> GetSuggestedFoodsAsync(string loaiBuaAn = null, int soLuong = 5, DateTime? ngayDeXuat = null, List<string> monAnDaDeXuat = null)
        {
            // Fallback method không async để tương thích
            return await GetSuggestedFoodsInternalAsync(loaiBuaAn, soLuong, ngayDeXuat, monAnDaDeXuat);
        }

        /// <summary>
        /// Lấy danh sách món ăn đề xuất từ thư viện (dùng AI nếu có mục tiêu)
        /// </summary>
        /// <param name="loaiBuaAn">Loại bữa ăn (Sáng, Trưa, Tối)</param>
        /// <param name="soLuong">Số lượng món ăn cần lấy</param>
        /// <returns>Danh sách món ăn đề xuất</returns>
        public List<ThuVienMonAn> GetSuggestedFoods(string loaiBuaAn = null, int soLuong = 5)
        {
            // Synchronous wrapper - sẽ gọi async method
            try
            {
                return GetSuggestedFoodsInternalAsync(loaiBuaAn, soLuong).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetSuggestedFoods: {ex.Message}");
                return GetSuggestedFoodsFallback(loaiBuaAn, soLuong);
            }
        }

        /// <summary>
        /// Internal method để lấy món ăn đề xuất (dùng AI)
        /// </summary>
        private async Task<List<ThuVienMonAn>> GetSuggestedFoodsInternalAsync(string loaiBuaAn = null, int soLuong = 5, DateTime? ngayDeXuat = null, List<string> monAnDaDeXuat = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetSuggestedFoodsInternalAsync START ===");
                System.Diagnostics.Debug.WriteLine($"LoaiBuaAn: {loaiBuaAn}, SoLuong: {soLuong}");

                // Kiểm tra kết nối database
                if (!_dbContext.Database.Exists())
                {
                    throw new Exception("Database không tồn tại hoặc không thể kết nối!");
                }

                // Lấy tất cả món ăn từ database
                var allFoods = _dbContext.Database.SqlQuery<ThuVienMonAn>(
                    "SELECT MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, NgayTao FROM ThuVienMonAn"
                ).ToList();

                if (allFoods == null || allFoods.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Không có món ăn nào trong database!");
                    return new List<ThuVienMonAn>();
                }

                // Lấy mục tiêu của user hiện tại
                string userId = CurrentUser.UserID;
                string mucTieu = null;

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    System.Diagnostics.Debug.WriteLine($"Đang lấy mục tiêu cho user: {userId}");
                    var goals = _goalController.GetGoalsByUser(userId, "Đang thực hiện");
                    
                    if (goals != null && goals.Count > 0)
                    {
                        // Lấy mục tiêu đầu tiên đang thực hiện
                        var activeGoal = goals.FirstOrDefault();
                        if (activeGoal != null)
                        {
                            mucTieu = $"{activeGoal.LoaiMucTieu}: {activeGoal.TenMucTieu}";
                            System.Diagnostics.Debug.WriteLine($"Mục tiêu: {mucTieu}");
                        }
                    }
                }

                // Nếu có mục tiêu, dùng AI để đề xuất
                if (!string.IsNullOrWhiteSpace(mucTieu))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Đang gọi ChatGPT để đề xuất món ăn...");
                        var danhSachTenMonAn = allFoods.Select(f => f.TenMonAn).ToList();
                        
                        // Loại bỏ món đã đề xuất ở các bữa trước khỏi danh sách
                        if (monAnDaDeXuat != null && monAnDaDeXuat.Count > 0)
                        {
                            var danhSachTenMonAnFiltered = danhSachTenMonAn
                                .Where(m => !monAnDaDeXuat.Contains(m, StringComparer.OrdinalIgnoreCase))
                                .ToList();
                            
                            System.Diagnostics.Debug.WriteLine($"Đã loại bỏ {danhSachTenMonAn.Count - danhSachTenMonAnFiltered.Count} món đã đề xuất. Còn lại {danhSachTenMonAnFiltered.Count} món để chọn.");
                            
                            // Nếu còn ít món, vẫn dùng danh sách gốc (tránh không có món nào)
                            if (danhSachTenMonAnFiltered.Count >= 3)
                            {
                                danhSachTenMonAn = danhSachTenMonAnFiltered;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Còn quá ít món sau khi loại bỏ, dùng lại danh sách gốc");
                            }
                        }
                        
                        // Truyền ngày đề xuất và món đã đề xuất để AI tránh trùng lặp
                        DateTime ngayDeXuatValue = ngayDeXuat ?? DateTime.Today;
                        var suggestedFoodNames = await _chatGPTService.SuggestFoodsAsync(loaiBuaAn, mucTieu, danhSachTenMonAn, ngayDeXuatValue, monAnDaDeXuat);

                        if (suggestedFoodNames != null && suggestedFoodNames.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"ChatGPT đề xuất {suggestedFoodNames.Count} món ăn: {string.Join(", ", suggestedFoodNames)}");
                            
                            // Tìm các món ăn từ database theo tên được đề xuất (theo thứ tự AI đề xuất)
                            // CHỈ lấy món ăn có trong database
                            var result = new List<ThuVienMonAn>();
                            foreach (var tenMonAn in suggestedFoodNames)
                            {
                                // Ưu tiên exact match trước
                                var monAn = allFoods.FirstOrDefault(f => 
                                    f.TenMonAn.Equals(tenMonAn.Trim(), StringComparison.OrdinalIgnoreCase));
                                
                                // Nếu không tìm thấy exact match, thử fuzzy match
                                if (monAn == null)
                                {
                                    monAn = allFoods.FirstOrDefault(f => 
                                        f.TenMonAn.IndexOf(tenMonAn.Trim(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        tenMonAn.Trim().IndexOf(f.TenMonAn, StringComparison.OrdinalIgnoreCase) >= 0);
                                }
                                
                                if (monAn != null && !result.Contains(monAn))
                                {
                                    result.Add(monAn);
                                    System.Diagnostics.Debug.WriteLine($"  ✓ Tìm thấy trong database: {monAn.TenMonAn} (match với '{tenMonAn}')");
                                }
                                else if (monAn == null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"  ✗ KHÔNG tìm thấy trong database: '{tenMonAn}' - Bỏ qua món này");
                                }
                                
                                // Lấy đúng số lượng AI đề xuất (không giới hạn bởi soLuong)
                                if (result.Count >= suggestedFoodNames.Count)
                                    break;
                            }
                            
                            // Nếu không tìm thấy món nào từ AI, fallback về logic cũ
                            if (result.Count == 0)
                            {
                                System.Diagnostics.Debug.WriteLine("Không tìm thấy món nào từ AI đề xuất trong database, fallback về logic cũ");
                            }

                            // Giữ nguyên số lượng AI đề xuất (không giới hạn bởi soLuong)
                            // AI tự quyết định số lượng món ăn phù hợp
                            System.Diagnostics.Debug.WriteLine($"=== GetSuggestedFoodsInternalAsync END: Trả về {result.Count} món ăn từ AI (theo đề xuất AI, không giới hạn) ===");
                            return result;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ChatGPT không trả về món ăn nào, fallback về logic cũ");
                        }
                    }
                    catch (Exception aiEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi gọi ChatGPT, fallback về logic cũ: {aiEx.Message}");
                    }
                }

                // Fallback: Dùng logic cũ nếu không có mục tiêu hoặc AI lỗi
                return GetSuggestedFoodsFallback(loaiBuaAn, soLuong, allFoods);
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                string errorMsg = $"Lỗi SQL Server:\n{sqlEx.Message}";
                errorMsg += $"\n\nMã lỗi: {sqlEx.Number}";
                if (sqlEx.InnerException != null)
                {
                    errorMsg += $"\n\nInner Exception:\n{sqlEx.InnerException.Message}";
                }
                System.Diagnostics.Debug.WriteLine($"=== SQL ERROR ===");
                System.Diagnostics.Debug.WriteLine(errorMsg);
                throw new Exception($"Lỗi khi lấy món ăn đề xuất: {errorMsg}", sqlEx);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Lỗi: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                System.Diagnostics.Debug.WriteLine($"=== GENERAL ERROR ===");
                System.Diagnostics.Debug.WriteLine(errorMsg);
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Lỗi khi lấy món ăn đề xuất: {errorMsg}", ex);
            }
        }

        /// <summary>
        /// Fallback method khi không có AI hoặc AI lỗi
        /// </summary>
        private List<ThuVienMonAn> GetSuggestedFoodsFallback(string loaiBuaAn = null, int soLuong = 5, List<ThuVienMonAn> allFoods = null)
        {
            try
            {
                if (allFoods == null)
                {
                    allFoods = _dbContext.Database.SqlQuery<ThuVienMonAn>(
                        "SELECT MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, NgayTao FROM ThuVienMonAn"
                    ).ToList();
                }

                if (allFoods == null || allFoods.Count == 0)
                {
                    return new List<ThuVienMonAn>();
                }

                var result = new List<ThuVienMonAn>();
                var random = new Random();

                if (!string.IsNullOrWhiteSpace(loaiBuaAn))
                {
                    List<string> preferredTypes = new List<string>();
                    switch (loaiBuaAn.ToLower())
                    {
                        case "sáng":
                            preferredTypes = new List<string> { "Ngũ cốc", "Trái cây" };
                            break;
                        case "trưa":
                            preferredTypes = new List<string> { "Thịt", "Hải sản" };
                            break;
                        case "tối":
                            preferredTypes = new List<string> { "Rau củ", "Hải sản" };
                            break;
                    }

                    var preferredFoods = allFoods
                        .Where(m => m.Loai != null && preferredTypes.Contains(m.Loai))
                        .ToList();

                    if (preferredFoods.Count > 0)
                    {
                        preferredFoods = preferredFoods.OrderBy(x => random.Next()).Take(soLuong).ToList();
                        result.AddRange(preferredFoods);
                    }

                    if (result.Count < soLuong)
                    {
                        var remainingFoods = allFoods
                            .Where(m => m.Loai == null || !preferredTypes.Contains(m.Loai))
                            .ToList();

                        if (remainingFoods.Count > 0)
                        {
                            var needed = soLuong - result.Count;
                            remainingFoods = remainingFoods.OrderBy(x => random.Next()).Take(needed).ToList();
                            result.AddRange(remainingFoods);
                        }
                    }
                }
                else
                {
                    result = allFoods.OrderBy(x => random.Next()).Take(soLuong).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetSuggestedFoodsFallback: {ex.Message}");
                return new List<ThuVienMonAn>();
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
            _chatGPTService?.Dispose();
            _goalController?.Dispose();
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

