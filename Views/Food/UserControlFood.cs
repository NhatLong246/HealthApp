using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;

namespace HealthApp.Views.Food
{
    public partial class UserControlFood : UserControl
    {
        public ThuVienMonAn MonAn { get; private set; }

        public UserControlFood()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gán dữ liệu món ăn vào UserControl
        /// </summary>
        public void SetData(ThuVienMonAn monAn)
        {
            if (monAn == null) return;

            MonAn = monAn;

            // Gán tên món ăn
            lblMonAn1.Text = monAn.TenMonAn ?? "";

            // Gán loại món ăn
            lblLoaiMonAn1.Text = monAn.Loai ?? "";

            // Gán đơn vị khối lượng chuẩn
            string donVi = monAn.Donvi ?? "g";
            double khoiLuong = monAn.KhoiLuongChuan ?? 100;
            lblDonViKhoiLuongChuan.Text = $"{khoiLuong}{donVi}";

            // Gán calories
            lblCalories.Text = monAn.Calories.HasValue ? $"{monAn.Calories.Value:F0} kcal" : "0 kcal";

            // Gán protein
            lblProtein.Text = monAn.Protein.HasValue ? $"{monAn.Protein.Value:F0}g protein" : "0g protein";

            // Gán carbs
            lblCarbs.Text = monAn.Carbs.HasValue ? $"{monAn.Carbs.Value:F0}g carbs" : "0g carbs";

            // Gán fat
            lblFat.Text = monAn.Fat.HasValue ? $"{monAn.Fat.Value:F0}g fat" : "0g fat";

            // Gán chất xơ (fiber)
            lblChatSo.Text = monAn.Fiber.HasValue ? $"{monAn.Fiber.Value:F0}g chất sơ" : "0g chất sơ";

            // Load hình ảnh
            LoadImage(monAn.imageURL);
        }

        /// <summary>
        /// Load hình ảnh từ URL hoặc đường dẫn file
        /// </summary>
        private void LoadImage(string imageURL)
        {
            if (string.IsNullOrEmpty(imageURL))
            {
                ptrHinhAnh.Image = null;
                return;
            }

            try
            {
                // Nếu là URL (http/https)
                if (imageURL.StartsWith("http://") || imageURL.StartsWith("https://"))
                {
                    // Load từ URL (cần async nhưng tạm thời dùng sync)
                    using (WebClient client = new WebClient())
                    {
                        byte[] imageData = client.DownloadData(imageURL);
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            ptrHinhAnh.Image = Image.FromStream(ms);
                        }
                    }
                }
                // Nếu là đường dẫn file tương đối hoặc tuyệt đối
                else
                {
                    string imagePath = imageURL;
                    
                    // Nếu là đường dẫn tương đối, thử tìm trong thư mục Resources/Images/Food
                    if (!Path.IsPathRooted(imagePath))
                    {
                        string basePath = Path.Combine(Application.StartupPath, "Resources", "Images", "Food");
                        string fullPath = Path.Combine(basePath, imagePath);
                        if (File.Exists(fullPath))
                        {
                            imagePath = fullPath;
                        }
                        else if (File.Exists(imagePath))
                        {
                            // Giữ nguyên nếu file tồn tại ở vị trí hiện tại
                        }
                        else
                        {
                            // Không tìm thấy file
                            ptrHinhAnh.Image = null;
                            return;
                        }
                    }

                    // Load từ file
                    if (File.Exists(imagePath))
                    {
                        ptrHinhAnh.Image = Image.FromFile(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Nếu lỗi, để null hoặc hiển thị ảnh mặc định
                System.Diagnostics.Debug.WriteLine($"Lỗi load hình ảnh: {ex.Message}");
                ptrHinhAnh.Image = null;
            }
        }
    }
}
