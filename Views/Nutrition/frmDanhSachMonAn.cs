using HealthApp.Models;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class frmDanhSachMonAn : Form
    {
        private List<ThuVienMonAn> _allFoods;
        private List<ThuVienMonAn> _filteredFoods;

        public ThuVienMonAn SelectedFood { get; private set; }

        public frmDanhSachMonAn()
        {
            InitializeComponent();
            LoadAllFoods();
        }

        private void LoadAllFoods()
        {
            try
            {
                using (var dbContext = new WF_HealthTracker())
                {
                    _allFoods = dbContext.ThuVienMonAn
                        .OrderBy(m => m.TenMonAn)
                        .ToList();

                    _filteredFoods = new List<ThuVienMonAn>(_allFoods);
                    DisplayFoods();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách món ăn:\n\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayFoods()
        {
            try
            {
                flowLayoutPanel.Controls.Clear();

                if (_filteredFoods == null || _filteredFoods.Count == 0)
                {
                    var lblEmpty = new Label
                    {
                        Text = "Không tìm thấy món ăn nào.",
                        AutoSize = false,
                        Width = flowLayoutPanel.Width - 40,
                        Font = new System.Drawing.Font("Segoe UI", 11F),
                        ForeColor = System.Drawing.Color.Gray,
                        Height = 50,
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                        Padding = new Padding(20)
                    };
                    flowLayoutPanel.Controls.Add(lblEmpty);
                    return;
                }

                flowLayoutPanel.SuspendLayout();

                foreach (var monAn in _filteredFoods)
                {
                    try
                    {
                        var item = new ucMonAnItem(monAn);
                        item.Width = flowLayoutPanel.Width - 40;
                        item.Margin = new Padding(0, 5, 0, 5);
                        
                        // Tắt tự động mở frmChinhSuaMonAn, chỉ raise event để mở frmThemMonAn
                        item.AutoOpenEditForm = false;

                        // Gắn event click để chọn món ăn
                        item.MonAnClicked += (s, food) =>
                        {
                            SelectedFood = food;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        };

                        flowLayoutPanel.Controls.Add(item);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo item món ăn {monAn?.TenMonAn}: {ex.Message}");
                    }
                }

                flowLayoutPanel.ResumeLayout(true);
                flowLayoutPanel.PerformLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách món ăn:\n\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtTimKiem.Text?.Trim().ToLower() ?? "";

                if (string.IsNullOrEmpty(searchText))
                {
                    _filteredFoods = new List<ThuVienMonAn>(_allFoods);
                }
                else
                {
                    _filteredFoods = _allFoods
                        .Where(m => !string.IsNullOrEmpty(m.TenMonAn) && 
                               m.TenMonAn.ToLower().Contains(searchText))
                        .ToList();
                }

                DisplayFoods();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
