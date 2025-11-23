using System;
using System.Threading.Tasks;
using HealthApp.Common.Helpers;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Dashboard - Tính toán BMI, TDEE, BMR
    /// </summary>
    public class DashboardController
    {
        /// <summary>
        /// Tính BMI (Body Mass Index)
        /// </summary>
        /// <param name="chieuCao">Chiều cao (cm)</param>
        /// <param name="canNang">Cân nặng (kg)</param>
        /// <returns>Kết quả tính BMI</returns>
        public BMIResult CalculateBMI(double chieuCao, double canNang)
        {
            try
            {
                // Validation
                if (chieuCao <= 0)
                {
                    return new BMIResult
                    {
                        Success = false,
                        Message = "Chiều cao phải lớn hơn 0!"
                    };
                }

                if (canNang <= 0)
                {
                    return new BMIResult
                    {
                        Success = false,
                        Message = "Cân nặng phải lớn hơn 0!"
                    };
                }

                // Tính BMI: BMI = weight (kg) / (height (m))^2
                double chieuCaoM = chieuCao / 100.0; // Chuyển từ cm sang m
                double bmi = canNang / (chieuCaoM * chieuCaoM);

                // Phân loại BMI
                string category = GetBMICategory(bmi);

                return new BMIResult
                {
                    Success = true,
                    BMI = bmi,
                    Category = category,
                    Message = $"BMI: {bmi:F1} - {category}"
                };
            }
            catch (Exception ex)
            {
                return new BMIResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tính BMR (Basal Metabolic Rate) - Tỷ lệ trao đổi chất cơ bản
        /// </summary>
        /// <param name="canNang">Cân nặng (kg)</param>
        /// <param name="chieuCao">Chiều cao (cm)</param>
        /// <param name="tuoi">Tuổi</param>
        /// <param name="gioiTinh">Giới tính ("Nam" hoặc "Nữ")</param>
        /// <returns>Kết quả tính BMR</returns>
        public BMRResult CalculateBMR(double canNang, double chieuCao, int tuoi, string gioiTinh)
        {
            try
            {
                // Validation
                if (canNang <= 0)
                {
                    return new BMRResult
                    {
                        Success = false,
                        Message = "Cân nặng phải lớn hơn 0!"
                    };
                }

                if (chieuCao <= 0)
                {
                    return new BMRResult
                    {
                        Success = false,
                        Message = "Chiều cao phải lớn hơn 0!"
                    };
                }

                if (tuoi <= 0 || tuoi > 150)
                {
                    return new BMRResult
                    {
                        Success = false,
                        Message = "Tuổi không hợp lệ!"
                    };
                }

                if (string.IsNullOrWhiteSpace(gioiTinh))
                {
                    return new BMRResult
                    {
                        Success = false,
                        Message = "Vui lòng chọn giới tính!"
                    };
                }

                // Tính BMR theo công thức Mifflin-St Jeor
                double bmr;
                string gioiTinhLower = gioiTinh.ToLower();

                if (gioiTinhLower.Contains("nam") || gioiTinhLower.Contains("male"))
                {
                    // BMR cho nam: 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) + 5
                    bmr = (10 * canNang) + (6.25 * chieuCao) - (5 * tuoi) + 5;
                }
                else
                {
                    // BMR cho nữ: 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) - 161
                    bmr = (10 * canNang) + (6.25 * chieuCao) - (5 * tuoi) - 161;
                }

                return new BMRResult
                {
                    Success = true,
                    BMR = bmr,
                    Message = $"BMR: {bmr:F0} kcal/ngày"
                };
            }
            catch (Exception ex)
            {
                return new BMRResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tính TDEE (Total Daily Energy Expenditure) - Tổng năng lượng tiêu hao hàng ngày
        /// </summary>
        /// <param name="canNang">Cân nặng (kg)</param>
        /// <param name="chieuCao">Chiều cao (cm)</param>
        /// <param name="tuoi">Tuổi</param>
        /// <param name="gioiTinh">Giới tính</param>
        /// <param name="mucDoHoatDong">Mức độ hoạt động</param>
        /// <returns>Kết quả tính TDEE</returns>
        public TDEEResult CalculateTDEE(double canNang, double chieuCao, int tuoi, string gioiTinh, string mucDoHoatDong)
        {
            try
            {
                // Tính BMR trước
                var bmrResult = CalculateBMR(canNang, chieuCao, tuoi, gioiTinh);
                if (!bmrResult.Success)
                {
                    return new TDEEResult
                    {
                        Success = false,
                        Message = bmrResult.Message
                    };
                }

                // Lấy Activity Factor
                double activityFactor = GetActivityFactor(mucDoHoatDong);
                if (activityFactor <= 0)
                {
                    return new TDEEResult
                    {
                        Success = false,
                        Message = "Mức độ hoạt động không hợp lệ!"
                    };
                }

                // Tính TDEE: TDEE = BMR × Activity Factor
                double tdee = bmrResult.BMR * activityFactor;

                return new TDEEResult
                {
                    Success = true,
                    BMR = bmrResult.BMR,
                    TDEE = tdee,
                    ActivityFactor = activityFactor,
                    Message = $"TDEE: {tdee:F0} kcal/ngày"
                };
            }
            catch (Exception ex)
            {
                return new TDEEResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy Activity Factor dựa trên mức độ hoạt động
        /// </summary>
        private double GetActivityFactor(string mucDoHoatDong)
        {
            if (string.IsNullOrWhiteSpace(mucDoHoatDong))
                return 0;

            string hoatDongLower = mucDoHoatDong.ToLower();

            if (hoatDongLower.Contains("ít") || hoatDongLower.Contains("sedentary") || hoatDongLower.Contains("không"))
            {
                return 1.2; // Ít vận động
            }
            else if (hoatDongLower.Contains("nhẹ") || hoatDongLower.Contains("light"))
            {
                return 1.375; // Nhẹ (1-3 buổi/tuần)
            }
            else if (hoatDongLower.Contains("vừa") || hoatDongLower.Contains("trung bình") || hoatDongLower.Contains("moderate"))
            {
                return 1.55; // Vừa/Trung bình (3-5 buổi/tuần)
            }
            else if (hoatDongLower.Contains("nhiều") || hoatDongLower.Contains("năng động") || hoatDongLower.Contains("active"))
            {
                return 1.725; // Nhiều/Năng động (6-7 buổi/tuần)
            }
            else if (hoatDongLower.Contains("rất") || hoatDongLower.Contains("very") || hoatDongLower.Contains("lao động"))
            {
                return 1.9; // Rất nhiều/Rất năng động
            }
            else
            {
                return 1.375; // Mặc định là nhẹ
            }
        }

        /// <summary>
        /// Phân loại BMI
        /// </summary>
        private string GetBMICategory(double bmi)
        {
            if (bmi < 18.5)
                return "Thiếu cân";
            else if (bmi < 23)
                return "Bình thường";
            else if (bmi < 25)
                return "Thừa cân";
            else if (bmi < 30)
                return "Béo phì độ I";
            else
                return "Béo phì độ II";
        }
    }

    /// <summary>
    /// Kết quả tính BMI
    /// </summary>
    public class BMIResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public double BMI { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// Kết quả tính BMR
    /// </summary>
    public class BMRResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public double BMR { get; set; }
    }

    /// <summary>
    /// Kết quả tính TDEE
    /// </summary>
    public class TDEEResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double ActivityFactor { get; set; }
    }
}

