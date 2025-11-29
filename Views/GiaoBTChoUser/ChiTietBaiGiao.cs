using System;
using System.Windows.Forms;
using HealthApp.Models;

namespace HealthApp.Views.PT
{
    public partial class ChiTietBaiGiao : Form
    {
        private readonly ThuVienBaiTap _exercise;
        private readonly AssignmentCustomDetail _initialDetail;
        public AssignmentCustomDetail CustomDetail { get; private set; }

        public ChiTietBaiGiao()
        {
            InitializeComponent();
            btnHoanTat.Click += BtnHoanTat_Click;
        }

        public ChiTietBaiGiao(ThuVienBaiTap exercise, AssignmentCustomDetail existingDetail = null) : this()
        {
            _exercise = exercise ?? throw new ArgumentNullException(nameof(exercise));
            _initialDetail = existingDetail;
            Load += ChiTietBaiGiao_Load;
        }

        private void ChiTietBaiGiao_Load(object sender, EventArgs e)
        {
            BindExerciseToUi();
        }

        private void BindExerciseToUi()
        {
            lblTenBT.Text = _exercise?.TenBaiTap ?? "Bài tập";
            lblMucTieu.Text = _exercise?.LoaiMucTieu ?? "Mục tiêu";

            var detail = _initialDetail ?? AssignmentCustomDetail.FromExercise(_exercise);

            // Các label chỉ hiển thị text tĩnh, không load giá trị
            // Giá trị chỉ hiển thị trong textbox
            guna2TextBox1.Text = detail.Equipment;
            guna2TextBox2.Text = detail.Sets;
            guna2TextBox3.Text = detail.Reps;
            guna2TextBox4.Text = detail.RestSeconds?.ToString() ?? "60";
        }

        private void BtnHoanTat_Click(object sender, EventArgs e)
        {
            if (!TryBuildUpdatedValues(out var detail))
                return;

            CustomDetail = detail;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryBuildUpdatedValues(out AssignmentCustomDetail detail)
        {
            detail = new AssignmentCustomDetail
            {
                Equipment = (guna2TextBox1.Text ?? string.Empty).Trim(),
                Sets = (guna2TextBox2.Text ?? string.Empty).Trim(),
                Reps = (guna2TextBox3.Text ?? string.Empty).Trim()
            };

            if (string.IsNullOrWhiteSpace(detail.Sets))
            {
                MessageBox.Show("Vui lòng nhập số set.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(detail.Reps))
            {
                MessageBox.Show("Vui lòng nhập số rep.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox3.Focus();
                return false;
            }

            if (!int.TryParse((guna2TextBox4.Text ?? string.Empty).Trim(), out var restSeconds) || restSeconds < 0)
            {
                MessageBox.Show("Thời gian nghỉ phải là số nguyên không âm.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox4.Focus();
                return false;
            }

            detail.RestSeconds = restSeconds;
            return true;
        }

        public class AssignmentCustomDetail
        {
            public string Equipment { get; set; }
            public string Sets { get; set; }
            public string Reps { get; set; }
            public int? RestSeconds { get; set; }

            public static AssignmentCustomDetail FromExercise(ThuVienBaiTap exercise)
            {
                if (exercise == null) throw new ArgumentNullException(nameof(exercise));
                return new AssignmentCustomDetail
                {
                    Equipment = exercise.DungCu ?? "Không yêu cầu",
                    Sets = string.IsNullOrWhiteSpace(exercise.SoSet) ? "3" : exercise.SoSet,
                    Reps = string.IsNullOrWhiteSpace(exercise.SoRep) ? "12" : exercise.SoRep,
                    RestSeconds = exercise.ThoiGianNghi ?? 60
                };
            }
        }
    }
}
