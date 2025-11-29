using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Controllers;
using HealthApp.Models;
using Newtonsoft.Json;
using AssignmentCustomDetail = HealthApp.Views.PT.ChiTietBaiGiao.AssignmentCustomDetail;

namespace HealthApp.Views.PT
{
    public partial class GiaoBaiTap : Form
    {
        private readonly WF_HealthTracker _dbContext;
        private readonly DatLichPT _booking;
        private readonly PTController _ptController;
        private readonly Guna2ShadowPanel _exerciseTemplate;
        // Bài tập được chọn lần cuối (dùng cho UX), nhưng lưu thực tế dựa trên danh sách CheckBox
        private ThuVienBaiTap _selectedExercise;
        // Lưu thông số custom theo BaiTapID để hiển thị lại ở list và dùng khi giao bài
        private readonly Dictionary<string, AssignmentCustomDetail> _customDetails =
            new Dictionary<string, AssignmentCustomDetail>();

        public GiaoBaiTap()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _ptController = new PTController();
            _exerciseTemplate = guna2ShadowPanel1;
            _exerciseTemplate.Visible = false;
            if (flpExercises.Controls.Contains(guna2ShadowPanel1))
            {
                flpExercises.Controls.Remove(guna2ShadowPanel1);
            }

            btnHoanTat.Click += BtnHoanTat_Click;
        }

        public GiaoBaiTap(DatLichPT booking) : this()
        {
            _booking = booking ?? throw new ArgumentNullException(nameof(booking));
            this.Load += GiaoBaiTap_Load;
        }

        private async void GiaoBaiTap_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = $"Giao bài tập cho {_booking.KhachHangID} - {_booking.NgayGioDat:dd/MM/yyyy}";
                await LoadExercisesAsync();
            }
            catch
            {
                MessageBox.Show("Không thể tải danh sách bài tập phù hợp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadExercisesAsync()
        {
            var goalKey = DetermineGoalKey(_booking);
            var exercises = await _ptController.GetExercisesByGoalAsync(goalKey);

            if (exercises == null || exercises.Count == 0)
            {
                MessageBox.Show($"Chưa có bài tập mẫu cho mục tiêu: {goalKey}.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Khi mở màn hình giao bài tập, coi như giao lại từ đầu -> xóa các bài cũ của DatLich này
            if (!string.IsNullOrWhiteSpace(_booking.DatLichID))
            {
                await _ptController.ClearAssignmentsForBookingAsync(_booking.DatLichID);
            }

            RenderExerciseCards(exercises);
        }

        private void RenderExerciseCards(IEnumerable<ThuVienBaiTap> exercises)
        {
            flpExercises.SuspendLayout();
            flpExercises.Controls.Clear();

            foreach (var exercise in exercises)
            {
                var card = CreateExerciseCard(exercise);
                flpExercises.Controls.Add(card);
            }

            flpExercises.ResumeLayout();

            _selectedExercise = exercises.First();
        }

        private Control CreateExerciseCard(ThuVienBaiTap exercise)
        {
            var panel = new Guna2ShadowPanel
            {
                Width = _exerciseTemplate.Width,
                Height = _exerciseTemplate.Height,
                FillColor = _exerciseTemplate.FillColor,
                Radius = _exerciseTemplate.Radius,
                ShadowColor = _exerciseTemplate.ShadowColor,
                ShadowShift = _exerciseTemplate.ShadowShift,
                Margin = new Padding(0, 0, 0, 15),
                Tag = exercise
            };

            var chkSelect = new CheckBox
            {
                Location = new Point(7, 20),
                AutoSize = true,
                Tag = exercise
            };
            chkSelect.CheckedChanged += ExerciseRadio_CheckedChanged;

            var lblName = new Label
            {
                Text = exercise.TenBaiTap,
                Font = lblTenBT.Font,
                ForeColor = lblTenBT.ForeColor,
                Location = new Point(30, 15),
                AutoSize = true
            };

            var lblGoal = new Label
            {
                Text = exercise.LoaiMucTieu,
                Font = lblMucTieu.Font,
                ForeColor = lblMucTieu.ForeColor,
                Location = new Point(31, 49),
                AutoSize = true
            };

            var lblEquip = new Label
            {
                Text = $"Dụng Cụ: {exercise.DungCu ?? "Không yêu cầu"}",
                Font = lbDungCu.Font,
                ForeColor = lbDungCu.ForeColor,
                // Đưa Dụng Cụ xuống dòng dưới để không đè lên Số Rep
                Location = new Point(32, 113),
                AutoSize = true
            };

            var lblSet = new Label
            {
                Text = "Số Set:",
                Font = lbSoSet.Font,
                ForeColor = lbSoSet.ForeColor,
                Location = new Point(169, 49),
                AutoSize = true
            };

            // Lấy thông số hiển thị (ưu tiên custom nếu đã chỉnh)
            if (!_customDetails.TryGetValue(exercise.BaiTapID, out var detailForView))
            {
                detailForView = AssignmentCustomDetail.FromExercise(exercise);
            }

            var lblSetValue = new Label
            {
                Name = $"lblSetValue_{exercise.BaiTapID}",
                Text = detailForView.Sets ?? "-",
                Font = lbGenSoSet.Font,
                ForeColor = lbGenSoSet.ForeColor,
                Location = new Point(235, 49),
                AutoSize = true
            };

            // Canh Số Rep thẳng cột với Thời Gian Nghỉ
            var lblRep = new Label
            {
                Text = "Số Rep:",
                Font = lbSoRep.Font,
                ForeColor = lbSoRep.ForeColor,
                // Giữ cùng hàng với Thời Gian Nghỉ
                Location = new Point(290, 81),
                AutoSize = true
            };

            var lblRepValue = new Label
            {
                Name = $"lblRepValue_{exercise.BaiTapID}",
                Text = detailForView.Reps ?? "-",
                Font = label1.Font,
                ForeColor = label1.ForeColor,
                Location = new Point(426, 81),
                AutoSize = true
            };

            var lblRest = new Label
            {
                Text = "Thời Gian Nghỉ:",
                Font = lbThoiGianNghi.Font,
                ForeColor = lbThoiGianNghi.ForeColor,
                Location = new Point(290, 49),
                AutoSize = true
            };

            var lblRestValue = new Label
            {
                Name = $"lblRestValue_{exercise.BaiTapID}",
                Text = $"{(detailForView.RestSeconds ?? 0)} giây",
                Font = lbGenThoiGianNghi.Font,
                ForeColor = lbGenThoiGianNghi.ForeColor,
                Location = new Point(426, 49),
                AutoSize = true
            };

            var btnView = new Guna2Button
            {
                Text = "Xem chi tiết",
                Font = btnGiaoBT.Font,
                ForeColor = btnGiaoBT.ForeColor,
                FillColor = btnGiaoBT.FillColor,
                BorderRadius = btnGiaoBT.BorderRadius,
                Size = btnGiaoBT.Size,
                Location = new Point(513, 38),
                Tag = exercise
            };
            btnView.Click += ExerciseDetail_Click;

            panel.Controls.Add(chkSelect);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblGoal);
            panel.Controls.Add(lblEquip);
            panel.Controls.Add(lblSet);
            panel.Controls.Add(lblSetValue);
            panel.Controls.Add(lblRep);
            panel.Controls.Add(lblRepValue);
            panel.Controls.Add(lblRest);
            panel.Controls.Add(lblRestValue);
            panel.Controls.Add(btnView);

            return panel;
        }

        private void ExerciseRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk && chk.Checked && chk.Tag is ThuVienBaiTap exercise)
            {
                _selectedExercise = exercise;
            }
        }

        private void ExerciseDetail_Click(object sender, EventArgs e)
        {
            if (sender is Guna2Button button && button.Tag is ThuVienBaiTap exercise)
            {
                _selectedExercise = exercise;
                _customDetails.TryGetValue(exercise.BaiTapID, out var existingDetail);

                using (var frm = new ChiTietBaiGiao(exercise, existingDetail))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) == DialogResult.OK && frm.CustomDetail != null)
                    {
                        _customDetails[exercise.BaiTapID] = frm.CustomDetail;
                        UpdateExerciseCardsFor(exercise, frm.CustomDetail);
                    }
                }
            }
        }

        private async void BtnHoanTat_Click(object sender, EventArgs e)
        {
            var selectedExercises = GetCheckedExercises().ToList();
            if (!selectedExercises.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất một bài tập để giao.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Với mỗi bài chưa có cấu hình custom, mở form thiết lập một lần
            foreach (var exercise in selectedExercises)
            {
                if (_customDetails.ContainsKey(exercise.BaiTapID))
                    continue;

                using (var frm = new ChiTietBaiGiao(exercise))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog(this) == DialogResult.OK && frm.CustomDetail != null)
                    {
                        _customDetails[exercise.BaiTapID] = frm.CustomDetail;
                        UpdateExerciseCardsFor(exercise, frm.CustomDetail);
                    }
                    else
                    {
                        // Nếu user hủy cho bài này, bỏ chọn bài đó
                        UncheckExercise(exercise);
                    }
                }
            }

            // Giao tất cả bài hiện còn đang được chọn
            selectedExercises = GetCheckedExercises().ToList();
            if (!selectedExercises.Any())
                return;

            await AssignMultipleExercisesAsync(selectedExercises);
        }

        private static string DetermineGoalKey(DatLichPT booking)
        {
            if (!string.IsNullOrWhiteSpace(booking.MucTieuLuyenTap))
                return booking.MucTieuLuyenTap.Trim();

            if (!string.IsNullOrWhiteSpace(booking.GhiChu))
                return booking.GhiChu.Trim();

            if (!string.IsNullOrWhiteSpace(booking.LoaiBuoiTap))
                return booking.LoaiBuoiTap.Trim();

            return string.Empty;
        }

        private async Task AssignSelectedExerciseAsync()
        {
            try
            {
                btnHoanTat.Enabled = false;
                if (_selectedExercise == null)
                    return;

                var detail = _customDetails.TryGetValue(_selectedExercise.BaiTapID, out var customDetail)
                    ? customDetail
                    : AssignmentCustomDetail.FromExercise(_selectedExercise);

                var payload = JsonConvert.SerializeObject(detail);
                var result = await _ptController.SaveAssignmentAsync(_booking, _selectedExercise, payload);

                if (result == null)
                {
                    MessageBox.Show("Không thể giao bài tập cho khách hàng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Đã giao bài tập cho khách hàng!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể giao bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnHoanTat.Enabled = true;
            }
        }

        private async Task AssignMultipleExercisesAsync(IEnumerable<ThuVienBaiTap> exercises)
        {
            try
            {
                btnHoanTat.Enabled = false;
                foreach (var exercise in exercises)
                {
                    var detail = _customDetails.TryGetValue(exercise.BaiTapID, out var customDetail)
                        ? customDetail
                        : AssignmentCustomDetail.FromExercise(exercise);

                    var payload = JsonConvert.SerializeObject(detail);
                    var result = await _ptController.SaveAssignmentAsync(_booking, exercise, payload);

                    if (result == null)
                    {
                        MessageBox.Show($"Không thể giao bài tập: {exercise.TenBaiTap}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                MessageBox.Show("Đã giao bài tập cho khách hàng!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể giao bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnHoanTat.Enabled = true;
            }
        }

        private IEnumerable<ThuVienBaiTap> GetCheckedExercises()
        {
            foreach (Control ctrl in flpExercises.Controls)
            {
                if (ctrl is Guna2ShadowPanel panel && panel.Tag is ThuVienBaiTap exercise)
                {
                    var chk = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                    if (chk != null && chk.Checked)
                        yield return exercise;
                }
            }
        }

        private void UncheckExercise(ThuVienBaiTap exercise)
        {
            foreach (Control ctrl in flpExercises.Controls)
            {
                if (ctrl is Guna2ShadowPanel panel && panel.Tag is ThuVienBaiTap ex &&
                    ex.BaiTapID == exercise.BaiTapID)
                {
                    var chk = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                    if (chk != null)
                        chk.Checked = false;
                }
            }
        }

        /// <summary>
        /// Cập nhật lại text Số Set / Số Rep / Thời Gian Nghỉ trên các card khi user chỉnh trong ChiTietBaiGiao.
        /// </summary>
        private void UpdateExerciseCardsFor(ThuVienBaiTap exercise, AssignmentCustomDetail detail)
        {
            if (exercise == null || detail == null)
                return;

            foreach (Control ctrl in flpExercises.Controls)
            {
                if (ctrl is Guna2ShadowPanel panel && panel.Tag is ThuVienBaiTap ex &&
                    ex.BaiTapID == exercise.BaiTapID)
                {
                    var setLabel = panel.Controls
                        .OfType<Label>()
                        .FirstOrDefault(l => l.Name == $"lblSetValue_{exercise.BaiTapID}");
                    if (setLabel != null)
                        setLabel.Text = detail.Sets ?? "-";

                    var repLabel = panel.Controls
                        .OfType<Label>()
                        .FirstOrDefault(l => l.Name == $"lblRepValue_{exercise.BaiTapID}");
                    if (repLabel != null)
                        repLabel.Text = detail.Reps ?? "-";

                    var restLabel = panel.Controls
                        .OfType<Label>()
                        .FirstOrDefault(l => l.Name == $"lblRestValue_{exercise.BaiTapID}");
                    if (restLabel != null)
                        restLabel.Text = $"{(detail.RestSeconds ?? 0)} giây";
                }
            }
        }
    }
}
