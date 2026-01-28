using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Common.Helpers;
using HealthApp.Models;
using System.Data.Entity;

namespace HealthApp.Views.PT
{
    public partial class frmDanhGiaPT : Form
    {
        private readonly DatLichPT _booking;
        private readonly List<GiaoBaiTapChoUser> _assignments;
        private int _selectedRating = 0; // Điểm đánh giá (1-5)
        private string _selectedQuickReview = null; // Đánh giá nhanh đã chọn
        private Image _starWhiteImage; // Ảnh ngôi sao trắng (từ ptrstar1)
        private Image _starYellowImage; // Ảnh ngôi sao vàng (từ ptrstar2)

        // Mảng các picture box ngôi sao (từ trái sang phải)
        private Guna2PictureBox[] _starPictureBoxes;

        public frmDanhGiaPT()
        {
            InitializeComponent();
            InitializeStars();
            btnDanhGia.Click += BtnDanhGia_Click;
        }

        /// <summary>
        /// Constructor với thông tin PT và bài tập
        /// </summary>
        public frmDanhGiaPT(DatLichPT booking, List<GiaoBaiTapChoUser> assignments) : this()
        {
            _booking = booking ?? throw new ArgumentNullException(nameof(booking));
            _assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
            
            Load += FrmDanhGiaPT_Load;
        }

        private void FrmDanhGiaPT_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPTInfo();
                LoadExerciseList();
                InitializeQuickReviewPanels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Khởi tạo mảng ngôi sao và lưu ảnh mẫu
        /// </summary>
        private void InitializeStars()
        {
            // Lưu ảnh mẫu: trắng từ ptrstar1, vàng từ ptrstar2
            _starWhiteImage = ptrstar1.Image;
            _starYellowImage = ptrstar2.Image;

            // Mảng các picture box ngôi sao (từ trái sang phải theo vị trí X)
            // ptrstar1: X=331, ptrstar2: X=377, ptrstar3: X=423, 
            // ptrstar4: X=469, ptrstar5: X=515
            _starPictureBoxes = new[]
            {
                ptrstar1, // ptrstar1 (trái nhất)
                ptrstar2, // ptrstar2
                ptrstar3, // ptrstar3
                ptrstar4, // ptrstar4
                ptrstar5  // ptrstar5 (phải nhất)
            };

            // Đặt tất cả ngôi sao về màu trắng ban đầu
            ResetStars();

            // Gắn event click cho từng ngôi sao
            for (int i = 0; i < _starPictureBoxes.Length; i++)
            {
                int index = i; // Capture index
                _starPictureBoxes[i].Click += (s, e) => Star_Click(index);
                _starPictureBoxes[i].Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// Reset tất cả ngôi sao về màu trắng
        /// </summary>
        private void ResetStars()
        {
            foreach (var star in _starPictureBoxes)
            {
                star.Image = _starWhiteImage;
            }
            _selectedRating = 0;
        }

        /// <summary>
        /// Xử lý khi click vào ngôi sao
        /// </summary>
        private void Star_Click(int clickedIndex)
        {
            // Đặt điểm = số ngôi sao được chọn (1-5)
            _selectedRating = clickedIndex + 1;

            // Đổi màu: từ trái sang phải, các ngôi sao từ 0 đến clickedIndex thành vàng
            for (int i = 0; i <= clickedIndex; i++)
            {
                _starPictureBoxes[i].Image = _starYellowImage;
            }

            // Các ngôi sao bên phải vẫn trắng
            for (int i = clickedIndex + 1; i < _starPictureBoxes.Length; i++)
            {
                _starPictureBoxes[i].Image = _starWhiteImage;
            }
        }

        /// <summary>
        /// Load thông tin PT (ảnh, tên)
        /// </summary>
        private void LoadPTInfo()
        {
            HuanLuyenVien pt = null;
            Users ptUser = null;

            System.Diagnostics.Debug.WriteLine("=== LoadPTInfo BẮT ĐẦU ===");
            System.Diagnostics.Debug.WriteLine($"Booking: {(_booking != null ? "NOT NULL" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"Booking.PTID: {_booking?.PTID ?? "NULL"}");
            System.Diagnostics.Debug.WriteLine($"Booking.HuanLuyenVien: {(_booking?.HuanLuyenVien != null ? "NOT NULL" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"Assignments count: {(_assignments?.Count ?? 0)}");

            // Ưu tiên 1: Lấy từ assignments (đã được eager load với HuanLuyenVien.Users)
            if (_assignments != null && _assignments.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Đang tìm PT từ assignments...");
                
                // Debug: In ra thông tin từng assignment
                foreach (var a in _assignments)
                {
                    System.Diagnostics.Debug.WriteLine($"  Assignment PTID: {a.PTID}, HuanLuyenVien: {a.HuanLuyenVien != null}, DatLichPT: {a.DatLichPT != null}");
                    if (a.DatLichPT != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"    DatLichPT.PTID: {a.DatLichPT.PTID}, HuanLuyenVien: {a.DatLichPT.HuanLuyenVien != null}");
                    }
                }
                
                // Tìm assignment có HuanLuyenVien đã được load và có Users
                var assignmentWithPT = _assignments.FirstOrDefault(a => 
                    !string.IsNullOrWhiteSpace(a.PTID) && 
                    a.HuanLuyenVien != null &&
                    a.HuanLuyenVien.Users != null);
                
                if (assignmentWithPT != null && assignmentWithPT.HuanLuyenVien != null)
                {
                    pt = assignmentWithPT.HuanLuyenVien;
                    ptUser = pt.Users;
                    System.Diagnostics.Debug.WriteLine($"✓ Tìm thấy PT từ assignment.HuanLuyenVien: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                }
                // Nếu không có assignment với PT đã load, thử lấy từ DatLichPT trong assignment
                else
                {
                    System.Diagnostics.Debug.WriteLine("Không tìm thấy từ assignment.HuanLuyenVien, thử từ DatLichPT...");
                    var assignmentWithBooking = _assignments.FirstOrDefault(a => 
                        a.DatLichPT != null && 
                        a.DatLichPT.HuanLuyenVien != null &&
                        a.DatLichPT.HuanLuyenVien.Users != null);
                    
                    if (assignmentWithBooking?.DatLichPT?.HuanLuyenVien != null)
                    {
                        pt = assignmentWithBooking.DatLichPT.HuanLuyenVien;
                        ptUser = pt.Users;
                        System.Diagnostics.Debug.WriteLine($"✓ Tìm thấy PT từ assignment.DatLichPT.HuanLuyenVien: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                    }
                    // Nếu vẫn không có, lấy PTID từ assignment và load từ DB
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Không tìm thấy từ DatLichPT, thử load từ DB...");
                        var firstAssignment = _assignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PTID));
                        if (firstAssignment != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Lấy PTID từ assignment: {firstAssignment.PTID}");
                            pt = LoadPTFromDatabase(firstAssignment.PTID);
                            if (pt != null)
                            {
                                ptUser = pt.Users;
                                System.Diagnostics.Debug.WriteLine($"✓ Load PT từ DB thành công: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("✗ Load PT từ DB thất bại!");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("✗ Không tìm thấy assignment nào có PTID!");
                        }
                    }
                }
            }

            // Ưu tiên 2: Lấy từ booking nếu chưa có
            if (pt == null && _booking?.HuanLuyenVien != null)
            {
                pt = _booking.HuanLuyenVien;
                ptUser = pt.Users;
                
                // Nếu PT không có Users, thử load lại từ DB
                if (ptUser == null && !string.IsNullOrWhiteSpace(pt.PTID))
                {
                    System.Diagnostics.Debug.WriteLine($"PT từ booking không có Users, load lại từ DB: {pt.PTID}");
                    pt = LoadPTFromDatabase(pt.PTID);
                    if (pt != null)
                    {
                        ptUser = pt.Users;
                        System.Diagnostics.Debug.WriteLine($"✓ Load lại PT từ DB thành công: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Tìm thấy PT từ booking.HuanLuyenVien: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                }
            }
            // Ưu tiên 3: Lấy từ PTID trong booking nếu chưa có
            else if (pt == null && !string.IsNullOrWhiteSpace(_booking?.PTID))
            {
                System.Diagnostics.Debug.WriteLine($"Load PT từ booking.PTID: {_booking.PTID}");
                pt = LoadPTFromDatabase(_booking.PTID);
                if (pt != null)
                {
                    ptUser = pt.Users;
                    System.Diagnostics.Debug.WriteLine($"✓ Load PT từ booking.PTID thành công: {pt.PTID}, User: {ptUser?.HoTen ?? "NULL"}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("✗ Load PT từ booking.PTID thất bại!");
                }
            }

            // Hiển thị tên PT
            if (ptUser != null)
            {
                string tenPT = ptUser.HoTen ?? ptUser.Username ?? pt?.PTID ?? "PT";
                lblTenPT.Text = tenPT;
                System.Diagnostics.Debug.WriteLine($"Hiển thị tên PT: {tenPT}");
            }
            else if (pt != null)
            {
                lblTenPT.Text = pt.PTID ?? "PT";
                System.Diagnostics.Debug.WriteLine($"Hiển thị PTID: {pt.PTID}");
            }
            else
            {
                lblTenPT.Text = "PT";
                System.Diagnostics.Debug.WriteLine("Không tìm thấy PT, hiển thị mặc định: PT");
            }

            // Load ảnh đại diện (ưu tiên từ HuanLuyenVien, sau đó từ Users)
            string anhDaiDien = pt?.AnhDaiDien ?? ptUser?.AnhDaiDien;
            System.Diagnostics.Debug.WriteLine($"Đường dẫn ảnh: {anhDaiDien ?? "NULL"}");
            LoadPTAvatar(anhDaiDien);
            System.Diagnostics.Debug.WriteLine("=== LoadPTInfo KẾT THÚC ===");
        }

        /// <summary>
        /// Load PT từ database theo PTID
        /// </summary>
        private HuanLuyenVien LoadPTFromDatabase(string ptId)
        {
            try
            {
                using (var context = new WF_HealthTracker())
                {
                    return context.HuanLuyenVien
                        .Include("Users")
                        .FirstOrDefault(h => h.PTID == ptId);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Load ảnh đại diện của PT
        /// </summary>
        private void LoadPTAvatar(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    ptrAnhPT.Image = null;
                    return;
                }

                string fullPath = imagePath;

                // Nếu là đường dẫn relative
                if (!Path.IsPathRooted(imagePath))
                {
                    var appDirectory = Application.StartupPath;
                    var possiblePaths = new[]
                    {
                        Path.Combine(appDirectory, "Resources", imagePath),
                        Path.Combine(appDirectory, imagePath),
                        Path.Combine(appDirectory, "Resources", "PTDocuments", imagePath)
                    };

                    foreach (var path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            fullPath = path;
                            break;
                        }
                    }
                }

                if (File.Exists(fullPath))
                {
                    ptrAnhPT.Image = Image.FromFile(fullPath);
                }
                else if (File.Exists(imagePath))
                {
                    ptrAnhPT.Image = Image.FromFile(imagePath);
                }
                else
                {
                    ptrAnhPT.Image = null;
                }
            }
            catch
            {
                ptrAnhPT.Image = null;
            }
        }

        /// <summary>
        /// Load danh sách tên bài tập (cùng 1 lịch trình)
        /// </summary>
        private void LoadExerciseList()
        {
            if (_assignments == null || _assignments.Count == 0)
            {
                lblDanhSachTenBaiTap.Text = "Danh sách: Không có bài tập";
                return;
            }

            // Lấy danh sách tên bài tập
            var exerciseNames = _assignments
                .Where(a => !string.IsNullOrWhiteSpace(a.TieuDe))
                .Select(a => a.TieuDe)
                .ToList();

            if (exerciseNames.Count == 0)
            {
                lblDanhSachTenBaiTap.Text = "Danh sách: Không có bài tập";
                return;
            }

            // Format: "Danh sách: bài tập 1, bài tập 2, ..."
            string exerciseList = string.Join(", ", exerciseNames);
            lblDanhSachTenBaiTap.Text = $"Danh sách: {exerciseList}";
        }

        /// <summary>
        /// Khởi tạo các panel đánh giá nhanh
        /// </summary>
        private void InitializeQuickReviewPanels()
        {
            // Gắn event click cho các panel đánh giá nhanh
            pnlDanhGiaNhanh1.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh1, lblDanhGiaNhanh1.Text);
            pnlDanhGiaNhanh2.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh2, lblDanhGiaNhanh2.Text);
            pnlDanhGiaNhanh3.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh3, lblDanhGiaNhanh3.Text);
            pnlDanhGiaNhanh4.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh4, lblDanhGiaNhanh4.Text);
            pnlDanhGiaNhanh5.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh5, lblDanhGiaNhanh5.Text);

            // Gắn event click cho các label bên trong panel
            lblDanhGiaNhanh1.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh1, lblDanhGiaNhanh1.Text);
            lblDanhGiaNhanh2.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh2, lblDanhGiaNhanh2.Text);
            lblDanhGiaNhanh3.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh3, lblDanhGiaNhanh3.Text);
            lblDanhGiaNhanh4.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh4, lblDanhGiaNhanh4.Text);
            lblDanhGiaNhanh5.Click += (s, e) => QuickReviewPanel_Click(pnlDanhGiaNhanh5, lblDanhGiaNhanh5.Text);

            // Đặt cursor hand cho các panel và label
            pnlDanhGiaNhanh1.Cursor = Cursors.Hand;
            pnlDanhGiaNhanh2.Cursor = Cursors.Hand;
            pnlDanhGiaNhanh3.Cursor = Cursors.Hand;
            pnlDanhGiaNhanh4.Cursor = Cursors.Hand;
            pnlDanhGiaNhanh5.Cursor = Cursors.Hand;
            lblDanhGiaNhanh1.Cursor = Cursors.Hand;
            lblDanhGiaNhanh2.Cursor = Cursors.Hand;
            lblDanhGiaNhanh3.Cursor = Cursors.Hand;
            lblDanhGiaNhanh4.Cursor = Cursors.Hand;
            lblDanhGiaNhanh5.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Xử lý khi click vào panel đánh giá nhanh
        /// </summary>
        private void QuickReviewPanel_Click(Guna2CustomGradientPanel panel, string reviewText)
        {
            // Kiểm tra xem panel này đã được chọn chưa (toggle)
            bool isCurrentlySelected = panel.BorderColor == Color.Orange && 
                                       panel.BorderThickness == 2 &&
                                       _selectedQuickReview == reviewText;

            if (isCurrentlySelected)
            {
                // Nếu đã được chọn, bỏ chọn (reset)
                ResetQuickReviewPanels();
            }
            else
            {
                // Reset tất cả panel về trạng thái bình thường
                ResetQuickReviewPanels();

                // Đánh dấu panel được chọn
                panel.BorderColor = Color.Orange;
                panel.BorderThickness = 2;
                panel.FillColor = Color.FromArgb(255, 255, 200); // Màu vàng nhạt
                panel.FillColor2 = Color.FromArgb(255, 255, 200); // Đảm bảo gradient cũng có màu vàng
                panel.FillColor3 = Color.FromArgb(255, 255, 200);
                panel.FillColor4 = Color.FromArgb(255, 255, 200);

                // Lưu đánh giá nhanh đã chọn
                _selectedQuickReview = reviewText;

                // Disable txtDanhGia khi đã chọn đánh giá nhanh
                txtDanhGia.Enabled = false;
                txtDanhGia.Text = "";
                txtDanhGia.PlaceholderText = "Đã chọn đánh giá nhanh, không thể bình luận";
            }
        }

        /// <summary>
        /// Reset tất cả panel đánh giá nhanh về trạng thái ban đầu (màu trắng)
        /// </summary>
        private void ResetQuickReviewPanels()
        {
            var panels = new[] { pnlDanhGiaNhanh1, pnlDanhGiaNhanh2, pnlDanhGiaNhanh3, pnlDanhGiaNhanh4, pnlDanhGiaNhanh5 };
            
            foreach (var panel in panels)
            {
                panel.BorderColor = Color.Silver;
                panel.BorderThickness = 1;
                panel.FillColor = Color.White; // Về màu trắng thay vì Transparent
                panel.FillColor2 = Color.White;
                panel.FillColor3 = Color.White;
                panel.FillColor4 = Color.White;
            }

            _selectedQuickReview = null;
            txtDanhGia.Enabled = true;
            txtDanhGia.PlaceholderText = "Bình luận tại đây...";
        }

        private void lblTieuDe_Click(object sender, EventArgs e)
        {
            // Không cần xử lý gì
        }

        /// <summary>
        /// Event handler cho nút Đánh Giá - lưu đánh giá vào database
        /// </summary>
        private void BtnDanhGia_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn điểm chưa
                if (_selectedRating == 0)
                {
                    MessageBox.Show("Vui lòng chọn điểm đánh giá (1-5 sao)!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra đã chọn đánh giá nhanh hoặc nhập bình luận chưa
                if (string.IsNullOrWhiteSpace(_selectedQuickReview) && string.IsNullOrWhiteSpace(txtDanhGia.Text))
                {
                    MessageBox.Show("Vui lòng chọn đánh giá nhanh hoặc nhập bình luận!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi đánh giá!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu đánh giá vào database
                SaveRating();

                MessageBox.Show("Đánh giá đã được lưu thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lưu đánh giá vào database
        /// </summary>
        private void SaveRating()
        {
            using (var context = new WF_HealthTracker())
            {
                // Lấy PTID đúng: ưu tiên từ booking, sau đó từ assignments
                string ptId = _booking?.PTID;
                if (string.IsNullOrWhiteSpace(ptId) && _assignments != null && _assignments.Count > 0)
                {
                    var assignmentWithPT = _assignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PTID));
                    ptId = assignmentWithPT?.PTID;
                }

                // Lấy DatLichID: ưu tiên từ booking, sau đó từ assignments
                string datLichId = _booking?.DatLichID;
                if (string.IsNullOrWhiteSpace(datLichId) && _assignments != null && _assignments.Count > 0)
                {
                    var assignmentWithDatLich = _assignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.DatLichID));
                    datLichId = assignmentWithDatLich?.DatLichID;
                }

                // Nếu vẫn không có DatLichID (dummy booking), tạo một ID tạm
                if (string.IsNullOrWhiteSpace(datLichId))
                {
                    datLichId = _booking?.DatLichID ?? "NO_SESSION_" + Guid.NewGuid().ToString().Substring(0, 8);
                }

                // Kiểm tra đã đánh giá chưa (theo DatLichID và PTID)
                var existingRating = context.DanhGiaPT
                    .FirstOrDefault(d => d.DatLichID == datLichId && 
                                         d.KhachHangID == CurrentUser.UserID &&
                                         d.PTID == ptId);

                string binhLuan = _selectedQuickReview ?? txtDanhGia.Text?.Trim();

                if (existingRating != null)
                {
                    // Cập nhật đánh giá cũ
                    existingRating.Diem = _selectedRating;
                    existingRating.BinhLuan = binhLuan;
                    existingRating.NgayDanhGia = DateTime.Now;
                }
                else
                {
                    // Tạo đánh giá mới
                    var danhGia = new DanhGiaPT
                    {
                        DanhGiaID = GenerateDanhGiaID(context),
                        DatLichID = datLichId,
                        KhachHangID = CurrentUser.UserID,
                        PTID = ptId,
                        Diem = _selectedRating,
                        BinhLuan = binhLuan,
                        NgayDanhGia = DateTime.Now
                    };

                    context.DanhGiaPT.Add(danhGia);
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Tạo ID cho đánh giá mới
        /// </summary>
        private string GenerateDanhGiaID(WF_HealthTracker context)
        {
            var lastRating = context.DanhGiaPT
                .OrderByDescending(d => d.DanhGiaID)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastRating != null && !string.IsNullOrEmpty(lastRating.DanhGiaID))
            {
                // Format: DG_0001, DG_0002, ...
                if (lastRating.DanhGiaID.StartsWith("DG_") &&
                    int.TryParse(lastRating.DanhGiaID.Substring(3), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"DG_{nextNumber:D4}";
        }
    }
}
