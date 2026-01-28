using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;

namespace HealthApp.Views.Admin
{
    public partial class ucBaiTap : UserControl
    {
        private ThuVienBaiTap _exercise;
        
        // Events
        public event EventHandler OnEdit;
        public event EventHandler OnDelete;
        
        public ucBaiTap()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }
        
        public ucBaiTap(ThuVienBaiTap exercise) : this()
        {
            LoadExerciseData(exercise);
        }
        
        /// <summary>
        /// Khởi tạo event handlers
        /// </summary>
        private void InitializeEventHandlers()
        {
            if (btnSua != null)
                btnSua.Click += (s, e) => OnEdit?.Invoke(this, EventArgs.Empty);
            
            if (btnXoa != null)
                btnXoa.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Load dữ liệu bài tập vào UserControl
        /// </summary>
        public void LoadExerciseData(ThuVienBaiTap exercise)
        {
            if (exercise == null) return;
            
            _exercise = exercise;
            
            // Populate dữ liệu
            if (lblTenBT != null)
                lblTenBT.Text = exercise.TenBaiTap ?? "N/A";
            
            if (lbNhomCo != null)
                lbNhomCo.Text = $"Nhóm Cơ: {exercise.NhomCoChinhNhat ?? "N/A"}";
            
            if (lbDungcutap != null)
                lbDungcutap.Text = $"Dụng cụ: {exercise.DungCu ?? "Không có"}";
            
            if (lbDoKho != null)
                lbDoKho.Text = $"Độ khó: {GetDifficultyText(exercise.CapDo)}";
            
            if (lbLuongCaloTieuHao != null)
            {
                string caloText = exercise.CaloriesMoiRep.HasValue
                    ? $"Calo tiêu hao: {exercise.CaloriesMoiRep.Value:F1} cal/rep"
                    : "Calo tiêu hao: N/A";
                lbLuongCaloTieuHao.Text = caloText;
            }
            
            // Load ảnh
            LoadExerciseImage(exercise);
        }
        
        /// <summary>
        /// Lấy text độ khó
        /// </summary>
        private string GetDifficultyText(string capDo)
        {
            if (string.IsNullOrWhiteSpace(capDo)) return "N/A";
            
            // Nếu capDo là số, format thành "x/5"
            if (int.TryParse(capDo, out int level))
            {
                return $"{level}/5";
            }
            
            // Nếu không phải số, trả về nguyên giá trị
            return capDo;
        }
        
        /// <summary>
        /// Load ảnh bài tập
        /// </summary>
        private void LoadExerciseImage(ThuVienBaiTap exercise)
        {
            if (picAnhBT == null || exercise == null) return;
            
            try
            {
                if (!string.IsNullOrWhiteSpace(exercise.AnhMinhHoa))
                {
                    string imagePath = System.IO.Path.Combine(
                        Application.StartupPath,
                        "Resources",
                        "Images",
                        "Exercises",
                        exercise.AnhMinhHoa
                    );
                    
                    if (System.IO.File.Exists(imagePath))
                    {
                        picAnhBT.Image = Image.FromFile(imagePath);
                        picAnhBT.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        picAnhBT.Image = null;
                    }
                }
                else
                {
                    picAnhBT.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucBaiTap] Error loading image: {ex.Message}");
                picAnhBT.Image = null;
            }
        }
        
        /// <summary>
        /// Lấy bài tập hiện tại
        /// </summary>
        public ThuVienBaiTap GetExercise()
        {
            return _exercise;
        }
    }
}
