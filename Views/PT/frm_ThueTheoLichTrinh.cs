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

namespace HealthApp.Views.PT
{
    public partial class frm_ThueTheoLichTrinh : Form
    {
        private DateTime _currentMonth;
        private DateTime _startDate;
        private int _numberOfWeeks;
        private HashSet<DateTime> _selectedDates;
        private List<Guna2Button> _dateButtons;

        public List<DateTime> SelectedDates => _selectedDates.OrderBy(d => d).ToList();

        public frm_ThueTheoLichTrinh()
        {
            InitializeComponent();
            _currentMonth = DateTime.Now;
            _startDate = DateTime.Today;
            _numberOfWeeks = (int)numSoTuan.Value;
            _selectedDates = new HashSet<DateTime>();
            _dateButtons = new List<Guna2Button>();
            dtpNgayBatDau.MinDate = DateTime.Today;
            dtpNgayBatDau.Value = DateTime.Today;
            
            // Initialize buttons first
            InitializeDateButtons();
            InitializeDateButtonProperties();
            InitializeEventHandlers();
            CalculateSelectedDates();
            
            // Load calendar after form is shown
            this.Load += (s, e) => LoadCalendar();
            LoadCalendar();
        }

        private void InitializeDateButtonProperties()
        {
            var buttons = new[] { btnLich1, btnLich2, btnLich3, btnLich4, btnLich5, btnLich6, btnLich7, btnLich8, btnLich9, btnLich10,
                btnLich11, btnLich12, btnLich13, btnLich14, btnLich15, btnLich16, btnLich17, btnLich18, btnLich19, btnLich20,
                btnLich21, btnLich22, btnLich23, btnLich24, btnLich25, btnLich26, btnLich27, btnLich28, btnLich29, btnLich30,
                btnLich31, btnLich32, btnLich33, btnLich34, btnLich35 };

            foreach (var btn in buttons)
            {
                btn.BorderColor = Color.DarkGray;
                btn.BorderRadius = 5;
                btn.BorderThickness = 1;
                btn.DisabledState.BorderColor = Color.DarkGray;
                btn.DisabledState.CustomBorderColor = Color.DarkGray;
                btn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
                btn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
                btn.FillColor = Color.FromArgb(233, 252, 255);
                btn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                btn.ForeColor = Color.FromArgb(0, 64, 64);
                btn.Size = new System.Drawing.Size(55, 50);
                // Don't set Text here - it will be set in UpdateCalendarDisplay
            }
        }

        private void InitializeDateButtons()
        {
            _dateButtons.Clear();
            // Use direct button references instead of searching in flowLayoutPanel1
            _dateButtons.AddRange(new[] { 
                btnLich1, btnLich2, btnLich3, btnLich4, btnLich5, btnLich6, btnLich7, 
                btnLich8, btnLich9, btnLich10, btnLich11, btnLich12, btnLich13, btnLich14, 
                btnLich15, btnLich16, btnLich17, btnLich18, btnLich19, btnLich20, 
                btnLich21, btnLich22, btnLich23, btnLich24, btnLich25, btnLich26, btnLich27, 
                btnLich28, btnLich29, btnLich30, btnLich31, btnLich32, btnLich33, btnLich34, btnLich35 
            });
        }

        private void InitializeEventHandlers()
        {
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnXacNhan.Click += BtnXacNhan_Click;
            btnHuy.Click += BtnHuy_Click;
            numSoTuan.ValueChanged += NumSoTuan_ValueChanged;
            dtpNgayBatDau.ValueChanged += DtpNgayBatDau_ValueChanged;
        }

        private void NumSoTuan_ValueChanged(object sender, EventArgs e)
        {
            _numberOfWeeks = (int)numSoTuan.Value;
            if (_numberOfWeeks > 0)
            {
                CalculateSelectedDates();
                LoadCalendar();
            }
        }

        private void DtpNgayBatDau_ValueChanged(object sender, EventArgs e)
        {
            _startDate = dtpNgayBatDau.Value.Date;
            if (_startDate < DateTime.Today)
            {
                MessageBox.Show("Ngày bắt đầu phải từ hôm nay trở đi!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayBatDau.Value = DateTime.Today;
                _startDate = DateTime.Today;
            }
            // Update current month to show the start date month
            _currentMonth = new DateTime(_startDate.Year, _startDate.Month, 1);
            CalculateSelectedDates();
            LoadCalendar();
        }

        private void CalculateSelectedDates()
        {
            _selectedDates.Clear();
            if (_numberOfWeeks > 0 && _startDate >= DateTime.Today)
            {
                DateTime currentDate = _startDate;
                DateTime endDate = _startDate.AddDays(_numberOfWeeks * 7 - 1);
                
                while (currentDate <= endDate)
                {
                    _selectedDates.Add(currentDate.Date);
                    currentDate = currentDate.AddDays(1);
                }
            }
        }

        private void LoadCalendar()
        {
            UpdateCalendarDisplay();
        }

        private void UpdateCalendarDisplay()
        {
            // Ensure buttons are initialized
            if (_dateButtons == null || _dateButtons.Count == 0)
            {
                InitializeDateButtons();
            }

            // Update month/year label
            lblThangNam.Text = $"Tháng {_currentMonth:MM}, {_currentMonth:yyyy}";

            // Calculate first day of the month
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            
            // Calculate which day of week the first day falls on (Monday = 0, Sunday = 6)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayOfWeek == 0) firstDayOfWeek = 7; // Convert Sunday from 0 to 7
            firstDayOfWeek -= 1; // Now Monday = 0, Sunday = 6

            // Start from the first day that should be displayed (could be from previous month)
            DateTime currentDate = firstDayOfMonth.AddDays(-firstDayOfWeek);

            // Update each button in the calendar grid
            for (int i = 0; i < _dateButtons.Count && i < 35; i++)
            {
                var btn = _dateButtons[i];
                
                if (btn == null) continue;
                
                // Set the day number - this is critical!
                btn.Text = currentDate.Day.ToString();
                btn.Tag = currentDate.Date; // Store the date in Tag
                btn.Enabled = true;
                btn.Visible = true;

                // Determine button state
                bool isCurrentMonth = currentDate.Month == _currentMonth.Month && currentDate.Year == _currentMonth.Year;
                bool isSelected = _selectedDates.Contains(currentDate.Date);
                bool isPast = currentDate.Date < DateTime.Today;

                // Apply styling based on state
                if (isCurrentMonth)
                {
                    if (isSelected)
                    {
                        // Selected date - highlight with purple
                        btn.FillColor = Color.FromArgb(200, 190, 255);
                        btn.ForeColor = Color.FromArgb(100, 88, 255);
                        btn.Enabled = true;
                    }
                    else if (isPast)
                    {
                        // Past date - gray out and disable
                        btn.FillColor = Color.FromArgb(240, 240, 240);
                        btn.ForeColor = Color.FromArgb(150, 150, 150);
                        btn.Enabled = false;
                    }
                    else
                    {
                        // Future date in current month - normal style
                        btn.FillColor = Color.FromArgb(233, 252, 255);
                        btn.ForeColor = Color.FromArgb(0, 64, 64);
                        btn.Enabled = true;
                    }
                }
                else
                {
                    // Date from previous/next month - gray out
                    btn.FillColor = Color.FromArgb(240, 240, 240);
                    btn.ForeColor = Color.FromArgb(150, 150, 150);
                    btn.Enabled = false;
                }

                // Remove old event handler and add new one
                btn.Click -= DateButton_Click;
                btn.Click += DateButton_Click;

                // Move to next day
                currentDate = currentDate.AddDays(1);
            }
        }

        private void DateButton_Click(object sender, EventArgs e)
        {
            var button = sender as Guna2Button;
            if (button == null || button.Tag == null || !button.Enabled)
                return;

            DateTime clickedDate = (DateTime)button.Tag;

            if (clickedDate.Date < DateTime.Today)
            {
                MessageBox.Show("Không thể chọn ngày trong quá khứ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Toggle selection
            if (_selectedDates.Contains(clickedDate.Date))
            {
                _selectedDates.Remove(clickedDate.Date);
            }
            else
            {
                _selectedDates.Add(clickedDate.Date);
            }

            LoadCalendar();
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            LoadCalendar();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            LoadCalendar();
        }

        private void BtnXacNhan_Click(object sender, EventArgs e)
        {
            if (_numberOfWeeks <= 0)
            {
                MessageBox.Show("Vui lòng chọn số tuần tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_startDate < DateTime.Today)
            {
                MessageBox.Show("Ngày bắt đầu phải từ hôm nay trở đi!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedDates.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một ngày!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
