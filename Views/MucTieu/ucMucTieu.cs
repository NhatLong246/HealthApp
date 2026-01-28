using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using HealthApp.Views.Dashboard;
using HealthApp.Controllers;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using HealthApp.Services.Interfaces;
using WeeklySchedule = HealthApp.Services.Interfaces.WeeklySchedule;

namespace HealthApp.Views.MucTieu
{
    public partial class ucMucTieu : UserControl
    {
        private GoalController _goalController;
        private NutritionController _nutritionController;
        private string _selectedLoaiMucTieu = null; // Mô tả mục tiêu đã chọn (có thể gồm nhiều mục tiêu)
        private readonly HashSet<string> _muscleGoals = new HashSet<string>
        {
            "Cơ Ngực","Cơ Lưng","Cơ Vai","Cơ Tay","Cơ Bụng","Cơ Mông","Cơ Đùi","Cơ Cổ"
        };
        private readonly List<string> _selectedMuscleGoals = new List<string>();
        private string _selectedSpecialGoal = null; // "Tăng Cân" hoặc "Giảm Cân"
        private DateTime? _selectedStartDate = null; // Ngày bắt đầu đã chọn (null = chưa chọn)
        private DateTime? _selectedEndDate = null; // Ngày kết thúc đã chọn (null = chưa chọn)
        private bool _isSelectingStartDate = true; // true = đang chọn ngày bắt đầu, false = đang chọn ngày kết thúc
        private DateTime _currentMonth = DateTime.Now; // Tháng hiện tại trong calendar
        private Dictionary<string, bool> _selectedDaysOfWeek = new Dictionary<string, bool>(); // Các thứ đã chọn
        private Dictionary<string, WeeklySchedule> _weeklySchedules = new Dictionary<string, WeeklySchedule>(); // Lịch tập theo thứ
        private Dictionary<string, Dictionary<string, bool>> _selectedSessions = new Dictionary<string, Dictionary<string, bool>>(); // Các buổi đã chọn cho mỗi thứ (Sáng, Chiều, Tối)
        private Guna.UI2.WinForms.Guna2Button _currentSelectedDayButton = null; // Button thứ hiện tại đang được chọn
        private string _currentSelectedDay = null; // Tên thứ hiện tại đang được chọn
        private List<ThuVienMonAn> _selectedFoods = new List<ThuVienMonAn>(); // Danh sách món ăn đã chọn
        private ThuVienMonAn _currentSelectedFood = null; // Món ăn đang được chọn để xem chi tiết
        private List<ThuVienMonAn> _foodLibraryCache = new List<ThuVienMonAn>(); // Cache toàn bộ thư viện món ăn
        private bool _suppressFoodSelectionChanged = false;
        private readonly List<SelectedExercise> _selectedExercises = new List<SelectedExercise>(); // Danh sách bài tập kèm buổi tập
        private readonly Dictionary<string, List<string>> _slotExerciseAssignments = new Dictionary<string, List<string>>(); // Slot (Thứ) -> danh sách BaiTapID
        
        // Lưu giá trị gốc từ database để validate phạm vi ±10%
        private double? _originalCalo = null;
        private double? _originalProtein = null;
        private double? _originalCarbs = null;
        private double? _originalFat = null;
        private double? _originalFiber = null;
        private bool _isValidating = false; // Flag để tránh vòng lặp khi validate

        private class SelectedExercise
        {
            public ThuVienBaiTap Exercise { get; set; }
            public HashSet<string> SlotKeys { get; } = new HashSet<string>();
        }

        private class ScheduleSlotOption
        {
            public string SlotKey { get; set; }
            public string DisplayText { get; set; }
            public int AssignedCount { get; set; }
            public string DisplayTextWithCount => $"{DisplayText} ({AssignedCount}/5)";
        }

        public ucMucTieu()
        {
            InitializeComponent();
            _goalController = new GoalController();
            _nutritionController = new NutritionController();
            InitializeData();
        }

        private void InitializeData()
        {
            // Khởi tạo dictionary cho các thứ
            _selectedDaysOfWeek["Thứ 2"] = false;
            _selectedDaysOfWeek["Thứ 3"] = false;
            _selectedDaysOfWeek["Thứ 4"] = false;
            _selectedDaysOfWeek["Thứ 5"] = false;
            _selectedDaysOfWeek["Thứ 6"] = false;
            _selectedDaysOfWeek["Thứ 7"] = false;
            _selectedDaysOfWeek["Chủ nhật"] = false;

            // Khởi tạo dictionary cho các buổi của mỗi thứ
            string[] days = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            foreach (var day in days)
            {
                _selectedSessions[day] = new Dictionary<string, bool>
                {
                    { "Sáng", false },
                    { "Chiều", false },
                    { "Tối", false }
                };
            }

            // Khởi tạo giờ bắt đầu và kết thúc
            InitializeTimeComboBoxes();

            // Load calendar
            LoadCalendar();

            // Đăng ký event handlers
            RegisterEventHandlers();

            UpdateGoalButtonStates();
        }

        private void InitializeTimeComboBoxes()
        {
            // Khởi tạo cho buổi Sáng: 05:00 – 11:00
            cboGioBatDauSang.Items.Clear();
            cboGioKetThucSang.Items.Clear();
            AddTimeRange(cboGioBatDauSang, 5, 11);
            AddTimeRange(cboGioKetThucSang, 5, 11);
            
            // Khởi tạo cho buổi Chiều: 11:00 – 17:00
            cboGioBatDauChieu.Items.Clear();
            cboGioKetThucChieu.Items.Clear();
            AddTimeRange(cboGioBatDauChieu, 11, 17);
            AddTimeRange(cboGioKetThucChieu, 11, 17);
            
            // Khởi tạo cho buổi Tối: 17:00 – 22:00
            cboGioBatDauToi.Items.Clear();
            cboGioKetThucToi.Items.Clear();
            AddTimeRange(cboGioBatDauToi, 17, 22);
            AddTimeRange(cboGioKetThucToi, 17, 22);

            // Set mặc định cho buổi Sáng
            cboGioBatDauSang.SelectedIndex = 0; // 05:00
            cboGioKetThucSang.SelectedIndex = cboGioKetThucSang.Items.Count - 1; // 11:00
            
            // Set mặc định cho buổi Chiều
            cboGioBatDauChieu.SelectedIndex = 0; // 11:00
            cboGioKetThucChieu.SelectedIndex = cboGioKetThucChieu.Items.Count - 1; // 17:00
            
            // Set mặc định cho buổi Tối
            cboGioBatDauToi.SelectedIndex = 0; // 17:00
            cboGioKetThucToi.SelectedIndex = cboGioKetThucToi.Items.Count - 1; // 22:00
        }

        private void AddTimeRange(Guna.UI2.WinForms.Guna2ComboBox comboBox, int startHour, int endHour)
        {
            for (int hour = startHour; hour <= endHour; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    // Với giờ cuối cùng, chỉ thêm :00 (không thêm :30)
                    // Ví dụ: endHour = 11 thì chỉ thêm 11:00, không thêm 11:30
                    if (hour == endHour && minute > 0)
                        break;
                    
                    string timeStr = $"{hour:D2}:{minute:D2}";
                    comboBox.Items.Add(timeStr);
                }
            }
        }

        private void RegisterEventHandlers()
        {
            // Event handlers cho các button mục tiêu
            btnCoNguc.Click += (s, e) => SelectGoalType("Cơ Ngực");
            btnCoMong.Click += (s, e) => SelectGoalType("Cơ Mông");
            btnCoDui.Click += (s, e) => SelectGoalType("Cơ Đùi");
            btnCoLung.Click += (s, e) => SelectGoalType("Cơ Lưng");
            btnCoCo.Click += (s, e) => SelectGoalType("Cơ Cổ");
            btnCoVai.Click += (s, e) => SelectGoalType("Cơ Vai");
            btnTangCan.Click += (s, e) => SelectGoalType("Tăng Cân");
            btnCoTay.Click += (s, e) => SelectGoalType("Cơ Tay");
            btnGiamCan.Click += (s, e) => SelectGoalType("Giảm Cân");
            btnCoBung.Click += (s, e) => SelectGoalType("Cơ Bụng");

            // Event handlers cho các thứ trong tuần
            btnThu2.Click += (s, e) => ToggleDayOfWeek("Thứ 2", btnThu2);
            btnThu3.Click += (s, e) => ToggleDayOfWeek("Thứ 3", btnThu3);
            btnThu4.Click += (s, e) => ToggleDayOfWeek("Thứ 4", btnThu4);
            btnThu5.Click += (s, e) => ToggleDayOfWeek("Thứ 5", btnThu5);
            btnThu6.Click += (s, e) => ToggleDayOfWeek("Thứ 6", btnThu6);
            btnThu7.Click += (s, e) => ToggleDayOfWeek("Thứ 7", btnThu7);
            btnChuNhat.Click += (s, e) => ToggleDayOfWeek("Chủ nhật", btnChuNhat);

            // Event handlers cho radio buttons trình độ
            rdoTatCa.CheckedChanged += (s, e) => { if (rdoTatCa.Checked) LoadExercises(); };
            rdoNguoiMoi.CheckedChanged += (s, e) => { if (rdoNguoiMoi.Checked) LoadExercises(); };
            rdoTrungCap.CheckedChanged += (s, e) => { if (rdoTrungCap.Checked) LoadExercises(); };
            rdoNangCao.CheckedChanged += (s, e) => { if (rdoNangCao.Checked) LoadExercises(); };

            // Event handlers cho calendar navigation
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;

            // Event handler cho dgv bài tập
            dgvBaiTapDeXuat.SelectionChanged += DgvBaiTapDeXuat_SelectionChanged;

            // Event handler cho nút tạo mục tiêu
            btnTaoMucTieu.Click += BtnTaoMucTieu_Click;

            // Event handler khi thay đổi giờ cho tất cả các buổi
            cboGioBatDauSang.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioKetThucSang.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioBatDauChieu.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioKetThucChieu.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioBatDauToi.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioKetThucToi.SelectedIndexChanged += CboGio_SelectedIndexChanged;

            // Event handlers cho các buổi tập
            btnSang.Click += (s, e) => ToggleSession("Sáng", btnSang);
            btnChieu.Click += (s, e) => ToggleSession("Chiều", btnChieu);
            btnToi.Click += (s, e) => ToggleSession("Tối", btnToi);

            // Event handlers cho các textbox dinh dưỡng - validate phạm vi ±10%
            if (txtCalo != null)
            {
                txtCalo.Leave += (s, e) => ValidateNutritionValue(txtCalo, _originalCalo, "Calo");
            }
            if (txtProtein != null)
            {
                txtProtein.Leave += (s, e) => ValidateNutritionValue(txtProtein, _originalProtein, "Protein");
            }
            if (txtCarbs != null)
            {
                txtCarbs.Leave += (s, e) => ValidateNutritionValue(txtCarbs, _originalCarbs, "Carbs");
            }
            if (txtChatBeo != null)
            {
                txtChatBeo.Leave += (s, e) => ValidateNutritionValue(txtChatBeo, _originalFat, "Chất béo");
            }
            if (txtChatXo != null)
            {
                txtChatXo.Leave += (s, e) => ValidateNutritionValue(txtChatXo, _originalFiber, "Chất xơ");
            }

        }

        private void SelectGoalType(string loaiMucTieu)
        {
            if (_muscleGoals.Contains(loaiMucTieu))
            {
                ToggleMuscleGoal(loaiMucTieu);
            }
            else
            {
                ToggleSpecialGoal(loaiMucTieu);
            }

            UpdateSelectedGoalSummary();
            UpdateGoalButtonStates();

            // Load danh sách bài tập
            LoadExercises();

            // Load chế độ dinh dưỡng mẫu
            LoadNutritionPreset();
        }

        private void ToggleMuscleGoal(string goal)
        {
            if (_selectedMuscleGoals.Contains(goal))
            {
                _selectedMuscleGoals.Remove(goal);
                return;
            }

            if (!string.IsNullOrEmpty(_selectedSpecialGoal))
            {
                // Khi chọn mục tiêu cơ, bỏ mục tiêu tăng/giảm cân
                _selectedSpecialGoal = null;
            }

            if (_selectedMuscleGoals.Count >= 2)
            {
                MessageBox.Show("Bạn chỉ được chọn tối đa 2 mục tiêu cơ cùng lúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _selectedMuscleGoals.Add(goal);
        }

        private void ToggleSpecialGoal(string goal)
        {
            if (_selectedSpecialGoal == goal)
            {
                _selectedSpecialGoal = null;
                return;
            }

            _selectedSpecialGoal = goal;
            _selectedMuscleGoals.Clear(); // Khi chọn mục tiêu tăng/giảm cân thì bỏ các mục tiêu cơ
        }

        private void UpdateSelectedGoalSummary()
        {
            if (!string.IsNullOrWhiteSpace(_selectedSpecialGoal))
            {
                _selectedLoaiMucTieu = _selectedSpecialGoal;
            }
            else if (_selectedMuscleGoals.Count > 0)
            {
                _selectedLoaiMucTieu = string.Join(" + ", _selectedMuscleGoals);
            }
            else
            {
                _selectedLoaiMucTieu = null;
            }
        }

        private void UpdateGoalButtonStates()
        {
            SetGoalButtonState(btnCoNguc, _selectedMuscleGoals.Contains("Cơ Ngực"));
            SetGoalButtonState(btnCoLung, _selectedMuscleGoals.Contains("Cơ Lưng"));
            SetGoalButtonState(btnCoVai, _selectedMuscleGoals.Contains("Cơ Vai"));
            SetGoalButtonState(btnCoTay, _selectedMuscleGoals.Contains("Cơ Tay"));
            SetGoalButtonState(btnCoBung, _selectedMuscleGoals.Contains("Cơ Bụng"));
            SetGoalButtonState(btnCoMong, _selectedMuscleGoals.Contains("Cơ Mông"));
            SetGoalButtonState(btnCoDui, _selectedMuscleGoals.Contains("Cơ Đùi"));
            SetGoalButtonState(btnCoCo, _selectedMuscleGoals.Contains("Cơ Cổ"));

            SetGoalButtonState(btnTangCan, _selectedSpecialGoal == "Tăng Cân");
            SetGoalButtonState(btnGiamCan, _selectedSpecialGoal == "Giảm Cân");
        }

        private void SetGoalButtonState(Guna.UI2.WinForms.Guna2Button button, bool isSelected)
        {
            if (button == null) return;

            if (isSelected)
            {
                button.FillColor = Color.FromArgb(100, 88, 255);
                button.BorderThickness = 3;
                button.BorderColor = Color.FromArgb(100, 88, 255);
                button.ForeColor = Color.White;
            }
            else
            {
                button.FillColor = Color.FromArgb(233, 252, 255);
                button.BorderThickness = 1;
                button.BorderColor = Color.DimGray;
                button.ForeColor = Color.Black;
            }
        }

        private bool HasAnyScheduledSlot()
        {
            return GetScheduleSlotOptions().Count > 0;
        }

        private List<ScheduleSlotOption> GetScheduleSlotOptions()
        {
            var orderedDays = new List<string> { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            var options = new List<ScheduleSlotOption>();

            foreach (var day in orderedDays)
            {
                if (_weeklySchedules.TryGetValue(day, out var schedule))
                {
                    options.Add(new ScheduleSlotOption
                    {
                        SlotKey = day,
                        DisplayText = FormatScheduleSlotDisplay(schedule),
                        AssignedCount = GetSlotAssignmentCount(day)
                    });
                }
            }

            // Thêm các ngày khác nếu có
            foreach (var kvp in _weeklySchedules)
            {
                if (orderedDays.Contains(kvp.Key))
                    continue;

                options.Add(new ScheduleSlotOption
                {
                    SlotKey = kvp.Key,
                    DisplayText = FormatScheduleSlotDisplay(kvp.Value),
                    AssignedCount = GetSlotAssignmentCount(kvp.Key)
                });
            }

            return options;
        }

        private string FormatScheduleSlotDisplay(WeeklySchedule schedule)
        {
            if (schedule == null || string.IsNullOrWhiteSpace(schedule.ThuNgay))
                return "Buổi không xác định";

            if (schedule.GioBatDau.HasValue && schedule.GioKetThuc.HasValue)
            {
                return $"{schedule.ThuNgay} ({schedule.GioBatDau.Value:hh\\:mm} - {schedule.GioKetThuc.Value:hh\\:mm})";
            }

            return schedule.ThuNgay;
        }

        private int GetSlotAssignmentCount(string slotKey)
        {
            if (string.IsNullOrWhiteSpace(slotKey))
                return 0;

            if (_slotExerciseAssignments.TryGetValue(slotKey, out var list))
            {
                return list.Count;
            }

            return 0;
        }

        private List<string> PromptForSlotSelection(ThuVienBaiTap exercise)
        {
            var slotOptions = GetScheduleSlotOptions()
                .Where(opt => opt.AssignedCount < 5)
                .ToList();

            if (slotOptions.Count == 0)
            {
                MessageBox.Show("Tất cả các buổi tập đã đủ 5 bài tập. Vui lòng bỏ bớt hoặc thêm buổi mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            List<string> selectedSlots = null;

            using (var form = new Form())
            {
                form.Text = $"Chọn buổi cho bài tập \"{exercise?.TenBaiTap}\"";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(420, 360);
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var instructionLabel = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Text = "Chọn các buổi sẽ thực hiện bài tập (mỗi buổi tối đa 5 bài):"
                };

                var checkedListBox = new CheckedListBox
                {
                    Dock = DockStyle.Top,
                    Height = 220,
                    CheckOnClick = true,
                    DisplayMember = nameof(ScheduleSlotOption.DisplayTextWithCount)
                };

                foreach (var option in slotOptions)
                {
                    checkedListBox.Items.Add(option);
                }

                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.RightToLeft,
                    Height = 50,
                    Padding = new Padding(5)
                };

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    DialogResult = DialogResult.Cancel,
                    Width = 100
                };

                var btnOk = new Button
                {
                    Text = "Xác nhận",
                    DialogResult = DialogResult.None,
                    Width = 120
                };

                btnOk.Click += (s, e) =>
                {
                    if (checkedListBox.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Vui lòng chọn ít nhất một buổi tập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    selectedSlots = checkedListBox.CheckedItems
                        .Cast<ScheduleSlotOption>()
                        .Select(opt => opt.SlotKey)
                        .ToList();

                    form.DialogResult = DialogResult.OK;
                    form.Close();
                };

                buttonPanel.Controls.Add(btnCancel);
                buttonPanel.Controls.Add(btnOk);

                form.Controls.Add(buttonPanel);
                form.Controls.Add(checkedListBox);
                form.Controls.Add(instructionLabel);

                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }
            }

            return selectedSlots;
        }

        private void AddExerciseAssignment(ThuVienBaiTap exercise, List<string> slotKeys)
        {
            if (exercise == null || slotKeys == null || slotKeys.Count == 0)
                return;

            var assignment = new SelectedExercise
            {
                Exercise = exercise
            };

            foreach (var slot in slotKeys.Distinct())
            {
                if (!_weeklySchedules.ContainsKey(slot))
                {
                    System.Diagnostics.Debug.WriteLine($"Slot {slot} không tồn tại trong lịch hiện tại, bỏ qua.");
                    continue;
                }

                assignment.SlotKeys.Add(slot);

                if (!_slotExerciseAssignments.ContainsKey(slot))
                {
                    _slotExerciseAssignments[slot] = new List<string>();
                }

                _slotExerciseAssignments[slot].Add(exercise.BaiTapID);
            }

            if (assignment.SlotKeys.Count > 0)
            {
                _selectedExercises.Add(assignment);
            }
        }

        private void RemoveExerciseAssignment(string baiTapId)
        {
            var assignment = _selectedExercises.FirstOrDefault(x => x.Exercise?.BaiTapID == baiTapId);
            if (assignment == null)
                return;

            foreach (var slot in assignment.SlotKeys)
            {
                if (_slotExerciseAssignments.TryGetValue(slot, out var list))
                {
                    list.Remove(baiTapId);
                    if (list.Count == 0)
                    {
                        _slotExerciseAssignments.Remove(slot);
                    }
                }
            }

            _selectedExercises.Remove(assignment);
        }

        private void RemoveAssignmentsForSlot(string slotKey)
        {
            if (string.IsNullOrWhiteSpace(slotKey))
                return;

            if (!_slotExerciseAssignments.TryGetValue(slotKey, out var exerciseIds))
                return;

            foreach (var exerciseId in exerciseIds.ToList())
            {
                var assignment = _selectedExercises.FirstOrDefault(x => x.Exercise?.BaiTapID == exerciseId);
                if (assignment != null)
                {
                    assignment.SlotKeys.Remove(slotKey);
                    if (assignment.SlotKeys.Count == 0)
                    {
                        _selectedExercises.Remove(assignment);
                    }
                }
            }

            _slotExerciseAssignments.Remove(slotKey);
            UpdateButtonTexts();
        }

        private List<string> GetSelectedGoalDbValues()
        {
            var result = new List<string>();

            if (!string.IsNullOrWhiteSpace(_selectedSpecialGoal))
            {
                var mapped = MapGoalTypeToLoaiMucTieuDB(_selectedSpecialGoal);
                if (!string.IsNullOrWhiteSpace(mapped))
                    result.Add(mapped);
            }
            else
            {
                foreach (var goal in _selectedMuscleGoals)
                {
                    var mapped = MapGoalTypeToLoaiMucTieuDB(goal);
                    if (!string.IsNullOrWhiteSpace(mapped))
                        result.Add(mapped);
                }
            }

            return result;
        }

        private void ToggleDayOfWeek(string thuNgay, Guna.UI2.WinForms.Guna2Button button)
        {
            // Nếu click vào button thứ đã được highlight (cùng button)
            if (_currentSelectedDay == thuNgay && _selectedDaysOfWeek[thuNgay])
            {
                // Bỏ highlight và xóa tất cả nội dung
                button.FillColor = Color.FromArgb(233, 252, 255);
                button.ForeColor = Color.FromArgb(0, 64, 64);
                _selectedDaysOfWeek[thuNgay] = false;
                _currentSelectedDayButton = null;
                _currentSelectedDay = null;
                
                // Xóa tất cả nội dung của thứ này
                if (_selectedSessions.ContainsKey(thuNgay))
                {
                    _selectedSessions[thuNgay]["Sáng"] = false;
                    _selectedSessions[thuNgay]["Chiều"] = false;
                    _selectedSessions[thuNgay]["Tối"] = false;
                }
                _weeklySchedules.Remove(thuNgay);
                RemoveAssignmentsForSlot(thuNgay);
                
                // Reset các buổi về ban đầu
                ResetSessionButtons();
                return;
            }

            // Nếu click vào button thứ khác, kiểm tra button thứ trước đó
            if (_currentSelectedDayButton != null && _currentSelectedDay != thuNgay)
            {
                // Nếu button thứ trước đó chưa chọn buổi, reset highlight
                if (!HasSelectedSession(_currentSelectedDay))
                {
                    _currentSelectedDayButton.FillColor = Color.FromArgb(233, 252, 255);
                    _currentSelectedDayButton.ForeColor = Color.FromArgb(0, 64, 64);
                    _selectedDaysOfWeek[_currentSelectedDay] = false;
                    _weeklySchedules.Remove(_currentSelectedDay);
                    RemoveAssignmentsForSlot(_currentSelectedDay);
                }
            }

            // Nếu click vào button thứ chưa được highlight
            if (!_selectedDaysOfWeek[thuNgay])
            {
                // Highlight tạm thời button thứ mới (chưa lưu vào _selectedDaysOfWeek)
                button.FillColor = Color.FromArgb(100, 88, 255);
                button.ForeColor = Color.White;
                _currentSelectedDayButton = button;
                _currentSelectedDay = thuNgay;
                
                // Reset các buổi về ban đầu
                ResetSessionButtons();
                
                // Reset các buổi của thứ này trong dictionary
                if (_selectedSessions.ContainsKey(thuNgay))
                {
                    _selectedSessions[thuNgay]["Sáng"] = false;
                    _selectedSessions[thuNgay]["Chiều"] = false;
                    _selectedSessions[thuNgay]["Tối"] = false;
                }
                _weeklySchedules.Remove(thuNgay);
                RemoveAssignmentsForSlot(thuNgay);
                
                // Chưa set _selectedDaysOfWeek[thuNgay] = true
                // Chỉ set khi đã chọn ít nhất 1 buổi
            }
            else
            {
                // Nếu click vào button thứ đã được highlight (button khác)
                // Load các buổi đã chọn của button đó
                _currentSelectedDayButton = button;
                _currentSelectedDay = thuNgay;
                LoadSessionsForDay(thuNgay);
            }
        }

        private bool HasSelectedSession(string thuNgay)
        {
            if (!_selectedSessions.ContainsKey(thuNgay))
                return false;
            
            return _selectedSessions[thuNgay]["Sáng"] || 
                   _selectedSessions[thuNgay]["Chiều"] || 
                   _selectedSessions[thuNgay]["Tối"];
        }

        private void ToggleSession(string sessionName, Guna.UI2.WinForms.Guna2Button button)
        {
            // Chỉ cho phép chọn buổi nếu đã chọn thứ
            if (string.IsNullOrWhiteSpace(_currentSelectedDay))
            {
                MessageBox.Show("Vui lòng chọn thứ trong tuần trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Toggle trạng thái buổi
            _selectedSessions[_currentSelectedDay][sessionName] = !_selectedSessions[_currentSelectedDay][sessionName];
            
            if (_selectedSessions[_currentSelectedDay][sessionName])
            {
                // Highlight button buổi
                button.FillColor = Color.FromArgb(100, 88, 255);
                button.ForeColor = Color.White;
                
                // Đảm bảo button thứ vẫn được highlight
                if (_currentSelectedDayButton != null)
                {
                    _currentSelectedDayButton.FillColor = Color.FromArgb(100, 88, 255);
                    _currentSelectedDayButton.ForeColor = Color.White;
                    _selectedDaysOfWeek[_currentSelectedDay] = true;
                }
                
                // Cập nhật lịch cho thứ này
                UpdateWeeklyScheduleForDay(_currentSelectedDay);
            }
            else
            {
                // Reset button buổi
                button.FillColor = Color.FromArgb(233, 252, 255);
                button.ForeColor = Color.FromArgb(0, 64, 64);
                
                // Kiểm tra xem còn buổi nào được chọn không
                if (!HasSelectedSession(_currentSelectedDay))
                {
                    // Nếu không còn buổi nào, reset button thứ về màu ban đầu
                    if (_currentSelectedDayButton != null)
                    {
                        _currentSelectedDayButton.FillColor = Color.FromArgb(233, 252, 255);
                        _currentSelectedDayButton.ForeColor = Color.FromArgb(0, 64, 64);
                        _selectedDaysOfWeek[_currentSelectedDay] = false;
                        _weeklySchedules.Remove(_currentSelectedDay);
                        RemoveAssignmentsForSlot(_currentSelectedDay);
                    }
                }
                else
                {
                    // Cập nhật lại lịch
                    UpdateWeeklyScheduleForDay(_currentSelectedDay);
                }
            }
        }

        private void UpdateWeeklyScheduleForDay(string thuNgay)
        {
            // Lấy giờ từ buổi đầu tiên được chọn
            TimeSpan? gioBatDau = null;
            TimeSpan? gioKetThuc = null;
            
            if (_selectedSessions[thuNgay]["Sáng"])
            {
                gioBatDau = ParseTime(cboGioBatDauSang.SelectedItem?.ToString());
                gioKetThuc = ParseTime(cboGioKetThucSang.SelectedItem?.ToString());
            }
            else if (_selectedSessions[thuNgay]["Chiều"])
            {
                gioBatDau = ParseTime(cboGioBatDauChieu.SelectedItem?.ToString());
                gioKetThuc = ParseTime(cboGioKetThucChieu.SelectedItem?.ToString());
            }
            else if (_selectedSessions[thuNgay]["Tối"])
            {
                gioBatDau = ParseTime(cboGioBatDauToi.SelectedItem?.ToString());
                gioKetThuc = ParseTime(cboGioKetThucToi.SelectedItem?.ToString());
            }
            
            if (gioBatDau.HasValue && gioKetThuc.HasValue)
            {
                if (!_weeklySchedules.ContainsKey(thuNgay))
                {
                    _weeklySchedules[thuNgay] = new WeeklySchedule
                    {
                        ThuNgay = thuNgay,
                        GioBatDau = gioBatDau,
                        GioKetThuc = gioKetThuc
                    };
                }
                else
                {
                    _weeklySchedules[thuNgay].GioBatDau = gioBatDau;
                    _weeklySchedules[thuNgay].GioKetThuc = gioKetThuc;
                }
            }
        }

        private void ResetSessionButtons()
        {
            // Reset màu các button buổi
            btnSang.FillColor = Color.FromArgb(233, 252, 255);
            btnSang.ForeColor = Color.FromArgb(0, 64, 64);
            btnChieu.FillColor = Color.FromArgb(233, 252, 255);
            btnChieu.ForeColor = Color.FromArgb(0, 64, 64);
            btnToi.FillColor = Color.FromArgb(233, 252, 255);
            btnToi.ForeColor = Color.FromArgb(0, 64, 64);
            
            // Reset combo box giờ về giá trị mặc định
            if (cboGioBatDauSang.Items.Count > 0)
                cboGioBatDauSang.SelectedIndex = 0;
            if (cboGioKetThucSang.Items.Count > 0)
                cboGioKetThucSang.SelectedIndex = cboGioKetThucSang.Items.Count - 1;
            
            if (cboGioBatDauChieu.Items.Count > 0)
                cboGioBatDauChieu.SelectedIndex = 0;
            if (cboGioKetThucChieu.Items.Count > 0)
                cboGioKetThucChieu.SelectedIndex = cboGioKetThucChieu.Items.Count - 1;
            
            if (cboGioBatDauToi.Items.Count > 0)
                cboGioBatDauToi.SelectedIndex = 0;
            if (cboGioKetThucToi.Items.Count > 0)
                cboGioKetThucToi.SelectedIndex = cboGioKetThucToi.Items.Count - 1;
        }

        private void LoadSessionsForDay(string thuNgay)
        {
            if (!_selectedSessions.ContainsKey(thuNgay))
                return;

            // Load trạng thái các buổi
            if (_selectedSessions[thuNgay]["Sáng"])
            {
                btnSang.FillColor = Color.FromArgb(100, 88, 255);
                btnSang.ForeColor = Color.White;
            }
            else
            {
                btnSang.FillColor = Color.FromArgb(233, 252, 255);
                btnSang.ForeColor = Color.FromArgb(0, 64, 64);
            }

            if (_selectedSessions[thuNgay]["Chiều"])
            {
                btnChieu.FillColor = Color.FromArgb(100, 88, 255);
                btnChieu.ForeColor = Color.White;
            }
            else
            {
                btnChieu.FillColor = Color.FromArgb(233, 252, 255);
                btnChieu.ForeColor = Color.FromArgb(0, 64, 64);
            }

            if (_selectedSessions[thuNgay]["Tối"])
            {
                btnToi.FillColor = Color.FromArgb(100, 88, 255);
                btnToi.ForeColor = Color.White;
            }
            else
            {
                btnToi.FillColor = Color.FromArgb(233, 252, 255);
                btnToi.ForeColor = Color.FromArgb(0, 64, 64);
            }

            // Load giờ từ lịch đã lưu (nếu có)
            // Lưu ý: Hiện tại mỗi thứ chỉ lưu một cặp giờ, nên sẽ load vào buổi đầu tiên được chọn
            if (_weeklySchedules.ContainsKey(thuNgay))
            {
                var schedule = _weeklySchedules[thuNgay];
                
                if (schedule.GioBatDau.HasValue && schedule.GioKetThuc.HasValue)
                {
                    // Format TimeSpan thành chuỗi "HH:mm"
                    string gioBatDau = $"{schedule.GioBatDau.Value.Hours:D2}:{schedule.GioBatDau.Value.Minutes:D2}";
                    string gioKetThuc = $"{schedule.GioKetThuc.Value.Hours:D2}:{schedule.GioKetThuc.Value.Minutes:D2}";
                    
                    // Load vào buổi đầu tiên được chọn
                    if (_selectedSessions[thuNgay]["Sáng"])
                    {
                        if (cboGioBatDauSang.Items.Contains(gioBatDau))
                            cboGioBatDauSang.SelectedItem = gioBatDau;
                        if (cboGioKetThucSang.Items.Contains(gioKetThuc))
                            cboGioKetThucSang.SelectedItem = gioKetThuc;
                    }
                    else if (_selectedSessions[thuNgay]["Chiều"])
                    {
                        if (cboGioBatDauChieu.Items.Contains(gioBatDau))
                            cboGioBatDauChieu.SelectedItem = gioBatDau;
                        if (cboGioKetThucChieu.Items.Contains(gioKetThuc))
                            cboGioKetThucChieu.SelectedItem = gioKetThuc;
                    }
                    else if (_selectedSessions[thuNgay]["Tối"])
                    {
                        if (cboGioBatDauToi.Items.Contains(gioBatDau))
                            cboGioBatDauToi.SelectedItem = gioBatDau;
                        if (cboGioKetThucToi.Items.Contains(gioKetThuc))
                            cboGioKetThucToi.SelectedItem = gioKetThuc;
                    }
                }
            }
        }

        private TimeSpan? ParseTime(string timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr))
                return null;

            if (TimeSpan.TryParse(timeStr, out TimeSpan result))
                return result;

            return null;
        }

        private void CboGio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as Guna.UI2.WinForms.Guna2ComboBox;
            if (comboBox == null) return;

            // Validate giờ bắt đầu và kết thúc cho từng buổi
            ValidateTimeRange(comboBox);
            
            // Chỉ cập nhật nếu đã chọn thứ
            if (string.IsNullOrWhiteSpace(_currentSelectedDay))
                return;
            
            // Cập nhật lịch cho thứ hiện tại dựa trên buổi đã chọn
            UpdateWeeklyScheduleForDay(_currentSelectedDay);
        }

        private void ValidateTimeRange(Guna.UI2.WinForms.Guna2ComboBox changedComboBox)
        {
            // Validate cho buổi Sáng
            if (changedComboBox == cboGioBatDauSang || changedComboBox == cboGioKetThucSang)
            {
                ValidateTimePair(cboGioBatDauSang, cboGioKetThucSang);
            }
            // Validate cho buổi Chiều
            else if (changedComboBox == cboGioBatDauChieu || changedComboBox == cboGioKetThucChieu)
            {
                ValidateTimePair(cboGioBatDauChieu, cboGioKetThucChieu);
            }
            // Validate cho buổi Tối
            else if (changedComboBox == cboGioBatDauToi || changedComboBox == cboGioKetThucToi)
            {
                ValidateTimePair(cboGioBatDauToi, cboGioKetThucToi);
            }
        }

        private void ValidateTimePair(Guna.UI2.WinForms.Guna2ComboBox cboBatDau, Guna.UI2.WinForms.Guna2ComboBox cboKetThuc)
        {
            if (cboBatDau.SelectedItem == null || cboKetThuc.SelectedItem == null)
                return;

            TimeSpan? gioBatDau = ParseTime(cboBatDau.SelectedItem.ToString());
            TimeSpan? gioKetThuc = ParseTime(cboKetThuc.SelectedItem.ToString());

            if (!gioBatDau.HasValue || !gioKetThuc.HasValue)
                return;

            // Nếu giờ kết thúc <= giờ bắt đầu, điều chỉnh giờ kết thúc
            if (gioKetThuc.Value <= gioBatDau.Value)
            {
                // Tìm giờ kết thúc hợp lệ tiếp theo (ít nhất 30 phút sau giờ bắt đầu)
                TimeSpan newGioKetThuc = gioBatDau.Value.Add(TimeSpan.FromMinutes(30));
                
                // Tìm item gần nhất trong combo box
                string timeStr = $"{newGioKetThuc.Hours:D2}:{newGioKetThuc.Minutes:D2}";
                
                // Tìm index của item gần nhất
                int targetIndex = -1;
                for (int i = 0; i < cboKetThuc.Items.Count; i++)
                {
                    TimeSpan? itemTime = ParseTime(cboKetThuc.Items[i].ToString());
                    if (itemTime.HasValue && itemTime.Value >= newGioKetThuc)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                
                // Nếu không tìm thấy, chọn item cuối cùng
                if (targetIndex == -1 && cboKetThuc.Items.Count > 0)
                {
                    targetIndex = cboKetThuc.Items.Count - 1;
                }
                
                if (targetIndex >= 0)
                {
                    cboKetThuc.SelectedIndex = targetIndex;
                }
            }
        }

        private async void LoadExercises()
        {
            try
            {
                // Clear DataGridView trước
                dgvBaiTapDeXuat.DataSource = null;
                dgvBaiTapDeXuat.Rows.Clear();
                dgvBaiTapDeXuat.Columns.Clear();

                var selectedGoalDbValues = GetSelectedGoalDbValues();

                // Kiểm tra xem đã chọn mục tiêu chưa
                if (selectedGoalDbValues.Count == 0)
                {
                    // Hiển thị thông báo chưa chọn mục tiêu
                    dgvBaiTapDeXuat.Columns.Clear();
                    dgvBaiTapDeXuat.Columns.Add("Message", "Thông báo");
                    dgvBaiTapDeXuat.Rows.Add("Vui lòng chọn loại mục tiêu để xem các bài tập đề xuất.");
                    dgvBaiTapDeXuat.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    return;
                }

                // Xác định trình độ từ radio button
                string capDoDB = null;
                if (rdoNguoiMoi.Checked) capDoDB = "Beginner";
                else if (rdoTrungCap.Checked) capDoDB = "Intermediate";
                else if (rdoNangCao.Checked) capDoDB = "Advanced";
                // Nếu rdoTatCa.Checked thì capDoDB = null (không filter)

                // Load bài tập từ database
                var allExercises = await _goalController.GetAllExercisesAsync();
                
                // Filter theo LoaiMucTieu đã chọn
                var exercises = allExercises?
                    .Where(e => !string.IsNullOrWhiteSpace(e.LoaiMucTieu) &&
                                selectedGoalDbValues.Any(goal => e.LoaiMucTieu.Equals(goal, StringComparison.OrdinalIgnoreCase)))
                    .ToList() ?? new List<ThuVienBaiTap>();
                
                // Filter theo trình độ nếu đã chọn
                if (!string.IsNullOrWhiteSpace(capDoDB))
                {
                    exercises = exercises.Where(e => 
                        !string.IsNullOrWhiteSpace(e.CapDo) && 
                        (e.CapDo.Equals(capDoDB, StringComparison.OrdinalIgnoreCase) || 
                         e.CapDo.Equals("All Levels", StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }
                
                System.Diagnostics.Debug.WriteLine($"=== LoadExercises ===");
                System.Diagnostics.Debug.WriteLine($"Selected Goal Type (UI): {_selectedLoaiMucTieu ?? "None"}");
                System.Diagnostics.Debug.WriteLine($"Mapped LoaiMucTieu (DB): {string.Join(", ", selectedGoalDbValues)}");
                System.Diagnostics.Debug.WriteLine($"Selected Level (DB): {capDoDB ?? "Tất cả"}");
                System.Diagnostics.Debug.WriteLine($"Found {exercises.Count} exercises");

                if (exercises == null || exercises.Count == 0)
                {
                    // Hiển thị thông báo không có dữ liệu
                    dgvBaiTapDeXuat.Columns.Clear();
                    dgvBaiTapDeXuat.Columns.Add("Message", "Thông báo");
                    dgvBaiTapDeXuat.Rows.Add($"Không tìm thấy bài tập nào cho mục tiêu '{_selectedLoaiMucTieu ?? "chưa chọn"}'.");
                    dgvBaiTapDeXuat.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    return;
                }

                // Tạo DataTable để bind dữ liệu
                DataTable dt = new DataTable();
                dt.Columns.Add("BaiTapID", typeof(string));
                dt.Columns.Add("TenBaiTap", typeof(string));
                dt.Columns.Add("LoaiMucTieu", typeof(string));
                dt.Columns.Add("NhomCoChinhNhat", typeof(string));
                dt.Columns.Add("DungCu", typeof(string));

                // Điền dữ liệu vào DataTable
                foreach (var exercise in exercises)
                {
                    DataRow row = dt.NewRow();
                    row["BaiTapID"] = exercise.BaiTapID ?? "";
                    row["TenBaiTap"] = exercise.TenBaiTap ?? "";
                    row["LoaiMucTieu"] = exercise.LoaiMucTieu ?? "";
                    row["NhomCoChinhNhat"] = exercise.NhomCoChinhNhat ?? "";
                    row["DungCu"] = exercise.DungCu ?? "";
                    dt.Rows.Add(row);
                }

                // Bind DataTable vào DataGridView
                dgvBaiTapDeXuat.DataSource = dt;

                // Cấu hình columns
                if (dgvBaiTapDeXuat.Columns.Count > 0)
                {
                    // Ẩn cột BaiTapID
                    if (dgvBaiTapDeXuat.Columns["BaiTapID"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["BaiTapID"].Visible = false;
                    }

                    // Set header text
                    if (dgvBaiTapDeXuat.Columns["TenBaiTap"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["TenBaiTap"].HeaderText = "Tên bài tập";
                        dgvBaiTapDeXuat.Columns["TenBaiTap"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (dgvBaiTapDeXuat.Columns["LoaiMucTieu"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["LoaiMucTieu"].HeaderText = "Loại mục tiêu";
                        dgvBaiTapDeXuat.Columns["LoaiMucTieu"].Width = 120;
                    }

                    if (dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"].HeaderText = "Nhóm cơ chính";
                        dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"].Width = 120;
                    }

                    if (dgvBaiTapDeXuat.Columns["DungCu"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["DungCu"].HeaderText = "Dụng cụ";
                        dgvBaiTapDeXuat.Columns["DungCu"].Width = 150;
                    }

                    // Thêm cột nút chọn ở cuối
                    var buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.Name = "Chon";
                    buttonColumn.HeaderText = "Chọn";
                    buttonColumn.Width = 80;
                    buttonColumn.UseColumnTextForButtonValue = false;
                    buttonColumn.FlatStyle = FlatStyle.Flat;
                    buttonColumn.DefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                    buttonColumn.DefaultCellStyle.ForeColor = Color.White;
                    buttonColumn.DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 70, 200);
                    buttonColumn.DefaultCellStyle.SelectionForeColor = Color.White;
                    
                    // Thêm cột vào cuối
                    dgvBaiTapDeXuat.Columns.Add(buttonColumn);
                }

                // Cấu hình DataGridView
                dgvBaiTapDeXuat.AllowUserToAddRows = false;
                dgvBaiTapDeXuat.AllowUserToDeleteRows = false;
                dgvBaiTapDeXuat.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvBaiTapDeXuat.MultiSelect = false;

                // Đăng ký event handler cho cell click (button)
                dgvBaiTapDeXuat.CellContentClick -= DgvBaiTapDeXuat_CellContentClick;
                dgvBaiTapDeXuat.CellContentClick += DgvBaiTapDeXuat_CellContentClick;
                
                // Cập nhật text của button dựa trên trạng thái đã chọn
                UpdateButtonTexts();

                // Clear selection
                dgvBaiTapDeXuat.ClearSelection();

                System.Diagnostics.Debug.WriteLine($"DataGridView loaded with {dt.Rows.Count} rows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadExercises error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                MessageBox.Show($"Lỗi khi tải danh sách bài tập:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Clear DataGridView nếu có lỗi
                dgvBaiTapDeXuat.DataSource = null;
                dgvBaiTapDeXuat.Rows.Clear();
                dgvBaiTapDeXuat.Columns.Clear();
            }
        }

        private async void DgvBaiTapDeXuat_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý khi click vào cột button "Chọn"
            var dgv = sender as DataGridView;
            if (dgv == null || e.RowIndex < 0) return;

            // Kiểm tra xem có phải cột button không
            if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn && dgv.Columns[e.ColumnIndex].Name == "Chon")
            {
                // Lấy BaiTapID từ row
                string baiTapId = null;
                if (dgv.Rows[e.RowIndex].Cells["BaiTapID"] != null && dgv.Rows[e.RowIndex].Cells["BaiTapID"].Value != null)
                {
                    baiTapId = dgv.Rows[e.RowIndex].Cells["BaiTapID"].Value.ToString();
                }
                else if (dgv.DataSource is DataTable dt && e.RowIndex < dt.Rows.Count)
                {
                    baiTapId = dt.Rows[e.RowIndex]["BaiTapID"]?.ToString();
                }

                if (string.IsNullOrWhiteSpace(baiTapId))
                    return;

                var existingAssignment = _selectedExercises.FirstOrDefault(ex => ex.Exercise?.BaiTapID == baiTapId);

                if (existingAssignment != null)
                {
                    // Xóa khỏi danh sách
                    RemoveExerciseAssignment(baiTapId);
                    UpdateButtonTexts();
                    return;
                }

                if (!HasAnyScheduledSlot())
                {
                    MessageBox.Show("Vui lòng tạo lịch tập (chọn thứ và thời gian) trước khi thêm bài tập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy thông tin bài tập
                var exercise = await _goalController.GetExerciseDetailAsync(baiTapId);
                if (exercise == null)
                    return;

                var selectedSlots = PromptForSlotSelection(exercise);
                if (selectedSlots == null || selectedSlots.Count == 0)
                    return;

                AddExerciseAssignment(exercise, selectedSlots);

                // Cập nhật text của button
                UpdateButtonTexts();

                System.Diagnostics.Debug.WriteLine($"Exercise {baiTapId} assigned to {string.Join(", ", selectedSlots)}. Total selected: {_selectedExercises.Count}");
            }
        }

        private void UpdateButtonTexts()
        {
            if (dgvBaiTapDeXuat.DataSource is DataTable dt && dgvBaiTapDeXuat.Columns["Chon"] != null)
            {
                for (int i = 0; i < dgvBaiTapDeXuat.Rows.Count; i++)
                {
                    if (i < dt.Rows.Count)
                    {
                        string baiTapId = dt.Rows[i]["BaiTapID"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(baiTapId))
                        {
                            bool isSelected = _selectedExercises.Any(ex => ex.Exercise?.BaiTapID == baiTapId);
                            dgvBaiTapDeXuat.Rows[i].Cells["Chon"].Value = isSelected ? "Bỏ chọn" : "Chọn";
                            
                            // Cập nhật màu button
                            if (isSelected)
                            {
                                dgvBaiTapDeXuat.Rows[i].Cells["Chon"].Style.BackColor = Color.FromArgb(220, 53, 69); // Đỏ
                                dgvBaiTapDeXuat.Rows[i].Cells["Chon"].Style.ForeColor = Color.White;
                            }
                            else
                            {
                                dgvBaiTapDeXuat.Rows[i].Cells["Chon"].Style.BackColor = Color.FromArgb(100, 88, 255); // Tím
                                dgvBaiTapDeXuat.Rows[i].Cells["Chon"].Style.ForeColor = Color.White;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map từ tên mục tiêu trong UI sang giá trị trong database
        /// Trả về tuple: (LoaiMucTieu, NhomCoChinhNhat, SearchBy)
        /// SearchBy: "LoaiMucTieu" hoặc "NhomCoChinhNhat"
        /// </summary>
        private (string loaiMucTieu, string nhomCoChinhNhat, string searchBy) MapGoalTypeToDatabase(string goalTypeUI)
        {
            // Dựa vào dữ liệu mẫu:
            // - "Tăng Cân", "Giảm Cân" -> tìm theo LoaiMucTieu
            // - "Cơ Ngực", "Cơ Mông", etc. -> tìm theo NhomCoChinhNhat
            switch (goalTypeUI)
            {
                case "Cơ Ngực":
                    return (null, "Ngực", "NhomCoChinhNhat");
                case "Cơ Mông":
                    return (null, "Mông", "NhomCoChinhNhat");
                case "Cơ Đùi":
                    return (null, "Chân", "NhomCoChinhNhat");
                case "Cơ Lưng":
                    return (null, "Lưng", "NhomCoChinhNhat");
                case "Cơ Cổ":
                    return (null, "Cổ", "NhomCoChinhNhat");
                case "Cơ Vai":
                    return (null, "Vai", "NhomCoChinhNhat");
                case "Cơ Tay":
                    return (null, "Tay", "NhomCoChinhNhat");
                case "Cơ Bụng":
                    return (null, "Bụng", "NhomCoChinhNhat");
                case "Tăng Cân":
                    return ("Tăng cân", null, "LoaiMucTieu");
                case "Giảm Cân":
                    return ("Giảm cân", null, "LoaiMucTieu");
                default:
                    // Mặc định tìm theo cả hai
                    return (goalTypeUI, goalTypeUI, "Both");
            }
        }

        /// <summary>
        /// Map từ tên mục tiêu UI sang LoaiMucTieu trong database
        /// </summary>
        private string MapGoalTypeToLoaiMucTieuDB(string goalTypeUI)
        {
            // Dựa vào dữ liệu mẫu trong data_ThuVienBaiTap.sql
            switch (goalTypeUI)
            {
                case "Cơ Ngực":
                    return "Cơ Ngực";
                case "Cơ Mông":
                    return "Cơ Mông";
                case "Cơ Đùi":
                    return "Cơ Đùi";
                case "Cơ Lưng":
                    return "Cơ Lưng";
                case "Cơ Cổ":
                    return "Cơ Cổ";
                case "Cơ Vai":
                    return "Cơ Vai";
                case "Cơ Tay":
                    return "Cơ Tay";
                case "Cơ Bụng":
                    return "Cơ Bụng";
                case "Tăng Cân":
                    return "Tăng cân";
                case "Giảm Cân":
                    return "Giảm cân";
                default:
                    return goalTypeUI;
            }
        }

        /// <summary>
        /// Map từ trình độ tiếng Anh sang tiếng Việt để hiển thị
        /// </summary>
        private string MapCapDoToVietnamese(string capDoEn)
        {
            if (string.IsNullOrWhiteSpace(capDoEn))
                return "N/A";

            switch (capDoEn.ToLower())
            {
                case "beginner":
                    return "Người mới";
                case "intermediate":
                    return "Trung cấp";
                case "advanced":
                    return "Nâng cao";
                case "all levels":
                    return "Tất cả";
                default:
                    return capDoEn;
            }
        }

        private async void DgvBaiTapDeXuat_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBaiTapDeXuat.SelectedRows.Count == 0 || dgvBaiTapDeXuat.DataSource == null)
                return;

            try
            {
                // Lấy BaiTapID từ row được chọn
                DataGridViewRow selectedRow = dgvBaiTapDeXuat.SelectedRows[0];
                
                // Kiểm tra xem có cột BaiTapID không (có thể bị ẩn)
                string baiTapId = null;
                if (selectedRow.Cells["BaiTapID"] != null && selectedRow.Cells["BaiTapID"].Value != null)
                {
                    baiTapId = selectedRow.Cells["BaiTapID"].Value.ToString();
                }
                else
                {
                    // Nếu không có cột BaiTapID, thử lấy từ DataBoundItem
                    if (selectedRow.DataBoundItem is DataRowView rowView)
                    {
                        baiTapId = rowView["BaiTapID"]?.ToString();
                    }
                    else if (dgvBaiTapDeXuat.DataSource is DataTable dt)
                    {
                        int rowIndex = selectedRow.Index;
                        if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                        {
                            baiTapId = dt.Rows[rowIndex]["BaiTapID"]?.ToString();
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(baiTapId))
                {
                    System.Diagnostics.Debug.WriteLine("BaiTapID is null or empty");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Loading exercise detail for BaiTapID: {baiTapId}");

                var exercise = await _goalController.GetExerciseDetailAsync(baiTapId);
                if (exercise == null)
                {
                    System.Diagnostics.Debug.WriteLine("Exercise not found");
                    return;
                }

                // Hiển thị chi tiết bài tập
                await DisplayExerciseDetail(exercise);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DgvBaiTapDeXuat_SelectionChanged error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                MessageBox.Show($"Lỗi khi tải chi tiết bài tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DisplayExerciseDetail(ThuVienBaiTap exercise)
        {
            if (exercise == null)
            {
                // Clear tất cả thông tin nếu exercise null
                lblSoKcal.Text = "0";
                lblSoRep.Text = "N/A";
                lblSoSet.Text = "N/A";
                lblThoiLuong.Text = "0";
                lblGioNghi.Text = "0";
                lblDoPhoBien.Text = "0";
                txtHuongDan.Text = "";
                txtLuuY.Text = "";
                return;
            }

            try
            {
                // Hiển thị thông tin chi tiết
                // Kcal (Calories mỗi rep)
                lblSoKcal.Text = exercise.CaloriesMoiRep?.ToString("F1") ?? "0";

                // Số Rep (VD: "8-12")
                lblSoRep.Text = exercise.SoRep ?? "N/A";

                // Số Set (VD: "3-4")
                lblSoSet.Text = exercise.SoSet ?? "N/A";

                // Thời lượng (giây) - format: "X giây" hoặc "X phút Y giây"
                if (exercise.ThoiLuongDeNghi.HasValue && exercise.ThoiLuongDeNghi.Value > 0)
                {
                    int seconds = exercise.ThoiLuongDeNghi.Value;
                    if (seconds >= 60)
                    {
                        int minutes = seconds / 60;
                        int remainingSeconds = seconds % 60;
                        if (remainingSeconds > 0)
                            lblThoiLuong.Text = $"{minutes}p {remainingSeconds}s";
                        else
                            lblThoiLuong.Text = $"{minutes} phút";
                    }
                    else
                    {
                        lblThoiLuong.Text = $"{seconds} giây";
                    }
                }
                else
                {
                    lblThoiLuong.Text = "0";
                }

                // Giờ nghỉ (giây) - format tương tự
                if (exercise.ThoiGianNghi.HasValue && exercise.ThoiGianNghi.Value > 0)
                {
                    int seconds = exercise.ThoiGianNghi.Value;
                    if (seconds >= 60)
                    {
                        int minutes = seconds / 60;
                        int remainingSeconds = seconds % 60;
                        if (remainingSeconds > 0)
                            lblGioNghi.Text = $"{minutes}p {remainingSeconds}s";
                        else
                            lblGioNghi.Text = $"{minutes} phút";
                    }
                    else
                    {
                        lblGioNghi.Text = $"{seconds} giây";
                    }
                }
                else
                {
                    lblGioNghi.Text = "0";
                }

                // Độ phổ biến
                lblDoPhoBien.Text = exercise.DoPhoBien?.ToString() ?? "0";

                // Hướng dẫn
                txtHuongDan.Text = exercise.HuongDan ?? "";

                // Lưu ý
                txtLuuY.Text = exercise.LuuY ?? "";

                // Load video nếu có
                if (!string.IsNullOrWhiteSpace(exercise.VideoHuongDan))
                {
                    try
                    {
                        // Đảm bảo WebView2 được khởi tạo trước khi navigate
                        if (webViewVideoHuongDan.CoreWebView2 == null)
                        {
                            await webViewVideoHuongDan.EnsureCoreWebView2Async();
                        }

                        if (webViewVideoHuongDan.CoreWebView2 != null)
                        {
                            webViewVideoHuongDan.CoreWebView2.Navigate(exercise.VideoHuongDan);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading video: {ex.Message}");
                        // Không hiển thị lỗi cho user, chỉ log
                    }
                }
                else
                {
                    // Nếu không có video, clear WebView2
                    if (webViewVideoHuongDan.CoreWebView2 != null)
                    {
                        webViewVideoHuongDan.CoreWebView2.Navigate("about:blank");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DisplayExerciseDetail error: {ex.Message}");
                MessageBox.Show($"Lỗi khi hiển thị chi tiết bài tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCalendar()
        {
            UpdateCalendarDisplay();
            UpdateCalendarButtons();
        }

        private void UpdateCalendarDisplay()
        {
            lblThangNam.Text = $"Tháng {_currentMonth:MM}, {_currentMonth:yyyy}";
            
            // Lấy ngày đầu tiên và cuối cùng của tháng
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            
            // Tính ngày đầu tiên trong tuần (có thể là ngày cuối tháng trước)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            // Chuyển đổi: Sunday = 0 -> 7, Monday = 1 -> 1, ..., Saturday = 6 -> 6
            if (firstDayOfWeek == 0) firstDayOfWeek = 7; // Chủ nhật = 7
            firstDayOfWeek -= 1; // Chuyển về 0-based (Thứ 2 = 0, Chủ nhật = 6)
            
            // Lấy danh sách các nút ngày (bỏ qua 7 nút đầu là thứ trong tuần)
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich"))
                .OrderBy(b => int.Parse(b.Name.Replace("btnLich", "")))
                .ToList();
            
            // Điền các ngày vào calendar
            DateTime currentDate = firstDayOfMonth.AddDays(-firstDayOfWeek); // Bắt đầu từ ngày đầu tiên hiển thị
            
            for (int i = 0; i < dateButtons.Count; i++)
            {
                var btn = dateButtons[i];
                btn.Text = currentDate.Day.ToString();
                btn.Tag = currentDate; // Lưu ngày vào Tag để dễ truy xuất
                btn.Enabled = true;
                
                // Xác định màu sắc dựa trên việc ngày có thuộc tháng hiện tại không
                if (currentDate.Month == _currentMonth.Month && currentDate.Year == _currentMonth.Year)
                {
                    // Ngày thuộc tháng hiện tại
                    btn.FillColor = Color.FromArgb(233, 252, 255);
                    btn.ForeColor = Color.FromArgb(0, 64, 64);
                }
                else
                {
                    // Ngày thuộc tháng trước hoặc sau
                    btn.FillColor = Color.FromArgb(240, 240, 240);
                    btn.ForeColor = Color.FromArgb(150, 150, 150);
                }
                
                // Highlight nếu nằm trong khoảng đã chọn
                if (_selectedStartDate.HasValue && _selectedEndDate.HasValue)
                {
                    if (currentDate >= _selectedStartDate.Value && currentDate <= _selectedEndDate.Value)
                    {
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                }
                else if (_selectedStartDate.HasValue && currentDate.Date == _selectedStartDate.Value.Date)
                {
                    // Chỉ highlight ngày bắt đầu nếu chưa có ngày kết thúc
                    btn.FillColor = Color.FromArgb(100, 88, 255);
                    btn.ForeColor = Color.White;
                }
                
                // Đăng ký event handler
                btn.Click -= DateButton_Click;
                btn.Click += DateButton_Click;
                
                currentDate = currentDate.AddDays(1);
            }
        }

        private void DateButton_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2Button;
            if (button == null || button.Tag == null) return;

            // Lấy ngày từ Tag (đã được lưu khi load calendar)
            DateTime selectedDate = (DateTime)button.Tag;
            
            // Chỉ cho phép chọn ngày thuộc tháng hiện tại
            if (selectedDate.Month != _currentMonth.Month || selectedDate.Year != _currentMonth.Year)
            {
                // Nếu click vào ngày tháng trước/sau, chuyển tháng
                if (selectedDate < _currentMonth)
                {
                    _currentMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                    LoadCalendar();
                    return;
                }
                else
                {
                    _currentMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                    LoadCalendar();
                    return;
                }
            }
            
            // Kiểm tra ngày không được trong quá khứ (trước hôm nay)
            if (selectedDate.Date < DateTime.Today)
            {
                MessageBox.Show("Không thể chọn ngày trong quá khứ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Logic chọn ngày: lần đầu = bắt đầu, lần 2 = kết thúc
            if (_isSelectingStartDate)
            {
                // Chọn ngày bắt đầu
                _selectedStartDate = selectedDate;
                _selectedEndDate = null; // Reset ngày kết thúc
                _isSelectingStartDate = false; // Lần sau sẽ chọn ngày kết thúc
            }
            else
            {
                // Chọn ngày kết thúc
                if (!_selectedStartDate.HasValue)
                {
                    // Nếu chưa có ngày bắt đầu, chọn làm ngày bắt đầu
                    _selectedStartDate = selectedDate;
                    _isSelectingStartDate = false;
                }
                else if (selectedDate <= _selectedStartDate.Value)
                {
                    // Nếu chọn ngày trước hoặc bằng ngày bắt đầu, reset và chọn làm ngày bắt đầu mới
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _selectedStartDate = selectedDate;
                    _selectedEndDate = null;
                    _isSelectingStartDate = false;
                }
                else
                {
                    // Chọn ngày kết thúc hợp lệ
                    _selectedEndDate = selectedDate;
                    _isSelectingStartDate = true; // Reset để lần sau chọn lại từ đầu
                }
            }

            UpdateDateButtons();
        }

        private void UpdateDateButtons()
        {
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich"))
                .OrderBy(b => int.Parse(b.Name.Replace("btnLich", "")))
                .ToList();

            foreach (var btn in dateButtons)
            {
                if (btn.Tag != null)
                {
                    DateTime btnDate = (DateTime)btn.Tag;
                    
                    // Xác định màu sắc dựa trên việc ngày có thuộc tháng hiện tại không
                    bool isCurrentMonth = btnDate.Month == _currentMonth.Month && btnDate.Year == _currentMonth.Year;
                    
                    // Kiểm tra nếu ngày nằm trong khoảng đã chọn
                    bool isInSelectedRange = false;
                    if (_selectedStartDate.HasValue && _selectedEndDate.HasValue)
                    {
                        isInSelectedRange = btnDate >= _selectedStartDate.Value && btnDate <= _selectedEndDate.Value;
                    }
                    else if (_selectedStartDate.HasValue)
                    {
                        // Chỉ có ngày bắt đầu, highlight ngày đó
                        isInSelectedRange = btnDate.Date == _selectedStartDate.Value.Date;
                    }
                    
                    if (isInSelectedRange)
                    {
                        // Ngày được chọn (trong khoảng hoặc là ngày bắt đầu)
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (btnDate.Date == _selectedStartDate?.Date)
                    {
                        // Ngày bắt đầu
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (btnDate.Date == _selectedEndDate?.Date)
                    {
                        // Ngày kết thúc
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (isCurrentMonth)
                    {
                        // Ngày thuộc tháng hiện tại nhưng chưa chọn
                        btn.FillColor = Color.FromArgb(233, 252, 255);
                        btn.ForeColor = Color.FromArgb(0, 64, 64);
                    }
                    else
                    {
                        // Ngày thuộc tháng trước/sau
                        btn.FillColor = Color.FromArgb(240, 240, 240);
                        btn.ForeColor = Color.FromArgb(150, 150, 150);
                    }
                }
            }
        }

        private void ResetDateButtons()
        {
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich")).ToList();
            
            foreach (var btn in dateButtons)
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
            }
        }

        private void UpdateCalendarButtons()
        {
            // Update day-of-week buttons (Thứ 2, Thứ 3, ...)
            // This is a simplified version - you may need to adjust based on your actual calendar implementation
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

        private async void BtnTaoMucTieu_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để tạo mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedLoaiMucTieu))
            {
                MessageBox.Show("Vui lòng chọn loại mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_weeklySchedules.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một ngày trong tuần để tập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_selectedStartDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_selectedEndDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày kết thúc mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedEndDate.Value <= _selectedStartDate.Value)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedStartDate.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Ngày bắt đầu không được trong quá khứ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string userId = CurrentUser.UserID;
                string capDo = rdoNguoiMoi.Checked ? "Beginner" : 
                              (rdoTrungCap.Checked ? "Intermediate" : 
                              (rdoNangCao.Checked ? "Advanced" : "Beginner"));

                // Tạo mục tiêu
                var goalResult = await _goalController.CreateGoalAsync(
                    userId: userId,
                    loaiMucTieu: _selectedLoaiMucTieu,
                    tenMucTieu: $"Mục tiêu {_selectedLoaiMucTieu}",
                    giaTriMucTieu: null,
                    ngayBatDau: _selectedStartDate.Value,
                    ngayKetThucDuKien: _selectedEndDate.Value
                );

                if (!goalResult.Success)
                {
                    MessageBox.Show(goalResult.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo kế hoạch luyện tập
                var weeklySchedulesList = _weeklySchedules.Values.ToList();
                var workoutPlan = await _goalController.CreateWorkoutPlanAsync(
                    userId: userId,
                    mucTieuId: goalResult.Goal.MucTieuID,
                    ngayBatDau: _selectedStartDate.Value,
                    ngayKetThuc: _selectedEndDate.Value,
                    capDo: capDo,
                    weeklySchedules: weeklySchedulesList
                );

                // Tạo BaiTapChiTiet cho các buổi tập nếu có bài tập được chọn
                if (_selectedExercises.Count > 0 && workoutPlan != null)
                {
                    // Lấy tất cả BuoiTap của kế hoạch này
                    var buoiTapList = await _goalController.GetBuoiTapByKeHoachTapIdAsync(workoutPlan.KeHoachTapID);
                    
                    // Gom buổi tập theo thứ (slot key)
                    var buoiTapByDay = buoiTapList
                        .GroupBy(b => b.ThuNgay)
                        .ToDictionary(g => g.Key, g => g.ToList());
                    
                    // Lấy số bắt đầu cho BaiTapChiTietID để tránh trùng
                    int startBaiTapChiTietNumber = await _goalController.GetNextBaiTapChiTietNumberAsync();
                    
                    int counter = 0;
                    foreach (var assignment in _selectedExercises)
                    {
                        if (assignment.Exercise == null || assignment.SlotKeys.Count == 0)
                            continue;

                        foreach (var slot in assignment.SlotKeys)
                        {
                            if (!buoiTapByDay.TryGetValue(slot, out var buoiTapCuaNgay))
                            {
                                System.Diagnostics.Debug.WriteLine($"Không tìm thấy buổi tập tương ứng với {slot}");
                                continue;
                            }

                            foreach (var buoiTap in buoiTapCuaNgay)
                            {
                                await _goalController.AddBaiTapChiTietAsync(
                                    buoiTapId: buoiTap.BuoiTapID,
                                    baiTapId: assignment.Exercise.BaiTapID,
                                    startNumber: startBaiTapChiTietNumber + counter
                                );
                                counter++;
                            }
                        }
                    }
                }

                // Tạo kế hoạch ăn uống
                string keHoachAnId = null;
                
                // Lấy giá trị dinh dưỡng từ textBox (ưu tiên) hoặc từ label nếu textBox trống
                double? tongCalories = ParseDouble(txtCalo.Text);
                if (!tongCalories.HasValue)
                    tongCalories = ParseDouble(label32.Text);
                
                double? tongProtein = ParseDouble(txtProtein.Text);
                if (!tongProtein.HasValue)
                    tongProtein = ParseDouble(label30.Text);
                
                double? tongCarbs = ParseDouble(txtCarbs.Text);
                if (!tongCarbs.HasValue)
                    tongCarbs = ParseDouble(label22.Text);
                
                double? tongFat = ParseDouble(txtChatBeo.Text);
                if (!tongFat.HasValue)
                    tongFat = ParseDouble(label28.Text);
                
                double? tongFiber = ParseDouble(txtChatXo.Text);
                if (!tongFiber.HasValue)
                    tongFiber = ParseDouble(label11.Text);

                // Tạo KeHoachAnUong nếu có ít nhất một giá trị dinh dưỡng được nhập
                if (tongCalories.HasValue || tongProtein.HasValue || tongCarbs.HasValue || tongFat.HasValue || tongFiber.HasValue)
                {
                    keHoachAnId = await _nutritionController.CreateMealPlanAsync(
                        mucTieuId: goalResult.Goal.MucTieuID,
                        tongCalories: tongCalories,
                        tongProtein: tongProtein,
                        tongCarbs: tongCarbs,
                        tongFat: tongFat,
                        tongFiber: tongFiber,
                        moTa: $"Kế hoạch ăn uống cho mục tiêu {_selectedLoaiMucTieu}"
                    );

                    // Tạo BuaAnChiTiet cho mỗi món ăn đã chọn (nếu có)
                    if (_selectedFoods.Count > 0)
                    {
                        foreach (var food in _selectedFoods)
                        {
                            await _nutritionController.AddMealToPlanAsync(
                                keHoachAnId: keHoachAnId,
                                monAnId: food.MonAnID,
                                loaiBuaAn: "Trưa", // Mặc định bữa trưa
                                ngayAn: _selectedStartDate.Value,
                                tenMonAn: food.TenMonAn,
                                khoiLuongChuan: food.KhoiLuongChuan,
                                donVi: food.Donvi
                            );
                        }
                    }
                }

                // Tạo thông báo chi tiết
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Tạo mục tiêu và kế hoạch luyện tập thành công!");
                sb.AppendLine($"Mục tiêu: {_selectedLoaiMucTieu}");
                sb.AppendLine($"Thời gian: {_selectedStartDate.Value:dd/MM/yyyy} - {_selectedEndDate.Value:dd/MM/yyyy}");
                sb.AppendLine($"Trình độ: {capDo}");
                sb.AppendLine($"Số ngày tập trong tuần: {_weeklySchedules.Count}");
                
                if (_selectedExercises.Count > 0)
                {
                    sb.AppendLine($"\nDanh sách bài tập đã chọn ({_selectedExercises.Count} bài):");
                    foreach (var assignment in _selectedExercises)
                    {
                        string slotInfo = assignment.SlotKeys.Count > 0
                            ? $" ({string.Join(", ", assignment.SlotKeys)})"
                            : string.Empty;
                        sb.AppendLine($"  - {assignment.Exercise?.TenBaiTap}{slotInfo}");
                    }
                }
                
                if (keHoachAnId != null)
                {
                    sb.AppendLine($"\nKế hoạch dinh dưỡng:");
                    if (tongCalories.HasValue) sb.AppendLine($"  - Calories: {tongCalories.Value:F0} kcal");
                    if (tongProtein.HasValue) sb.AppendLine($"  - Protein: {tongProtein.Value:F0} g");
                    if (tongCarbs.HasValue) sb.AppendLine($"  - Carbs: {tongCarbs.Value:F0} g");
                    if (tongFat.HasValue) sb.AppendLine($"  - Chất béo: {tongFat.Value:F0} g");
                    if (tongFiber.HasValue) sb.AppendLine($"  - Chất xơ: {tongFiber.Value:F0} g");
                }
                
                if (_selectedFoods.Count > 0)
                {
                    sb.AppendLine($"\nDanh sách món ăn đã chọn ({_selectedFoods.Count} món):");
                    foreach (var food in _selectedFoods)
                    {
                        sb.AppendLine($"  - {food.TenMonAn}");
                    }
                }

                MessageBox.Show(sb.ToString(), "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mục tiêu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadNutritionPreset()
        {
            var goalDbValues = GetSelectedGoalDbValues();
            if (goalDbValues.Count == 0)
            {
                ResetNutritionDisplay();
                return;
            }

            // Nếu có mục tiêu tăng/giảm cân -> chỉ load mục tiêu đó
            if (!string.IsNullOrWhiteSpace(_selectedSpecialGoal))
            {
                var preset = _goalController.GetNutritionPresetByGoal(goalDbValues[0]);
                ApplyNutritionPreset(preset);
                return;
            }

            // Nếu chỉ có một mục tiêu cơ
            if (_selectedMuscleGoals.Count == 1)
            {
                var preset = _goalController.GetNutritionPresetByGoal(goalDbValues[0]);
                ApplyNutritionPreset(preset);
                return;
            }

            // Nếu có hai mục tiêu cơ -> chia trọng số 50%
            if (_selectedMuscleGoals.Count == 2 && goalDbValues.Count >= 2)
            {
                var preset1 = _goalController.GetNutritionPresetByGoal(goalDbValues[0]);
                var preset2 = _goalController.GetNutritionPresetByGoal(goalDbValues[1]);

                if (preset1 != null && preset2 != null)
                {
                    ApplyNutritionValues(
                        Average(preset1.Calo, preset2.Calo),
                        Average(preset1.Protein, preset2.Protein),
                        Average(preset1.Carbs, preset2.Carbs),
                        Average(preset1.Fat, preset2.Fat),
                        Average(preset1.Fiber, preset2.Fiber));
                    return;
                }
                else
                {
                    ApplyNutritionPreset(preset1 ?? preset2);
                    return;
                }
            }

            ResetNutritionDisplay();
        }

        private void ResetNutritionDisplay()
        {
            // Reset giá trị gốc
            _originalCalo = null;
            _originalProtein = null;
            _originalCarbs = null;
            _originalFat = null;
            _originalFiber = null;

            label32.Text = "0";
            label30.Text = "0";
            label22.Text = "0";
            label28.Text = "0";
            label11.Text = "0";

            _isValidating = true;
            txtCalo.Text = string.Empty;
            txtProtein.Text = string.Empty;
            txtCarbs.Text = string.Empty;
            txtChatBeo.Text = string.Empty;
            txtChatXo.Text = string.Empty;
            _isValidating = false;
        }

        private void ApplyNutritionPreset(CheDoDinhDuongMau preset)
        {
            if (preset == null)
            {
                ResetNutritionDisplay();
                return;
            }

            ApplyNutritionValues(preset.Calo, preset.Protein, preset.Carbs, preset.Fat, preset.Fiber);
        }

        private void ApplyNutritionValues(int calo, int protein, int carbs, int fat, int fiber)
        {
            // Lưu giá trị gốc từ database để validate phạm vi ±10%
            _originalCalo = calo;
            _originalProtein = protein;
            _originalCarbs = carbs;
            _originalFat = fat;
            _originalFiber = fiber;

            label32.Text = calo.ToString();
            label30.Text = protein.ToString();
            label22.Text = carbs.ToString();
            label28.Text = fat.ToString();
            label11.Text = fiber.ToString();

            // Set giá trị vào textbox (tạm thời tắt validation để tránh vòng lặp)
            _isValidating = true;
            txtCalo.Text = calo.ToString();
            txtProtein.Text = protein.ToString();
            txtCarbs.Text = carbs.ToString();
            txtChatBeo.Text = fat.ToString();
            txtChatXo.Text = fiber.ToString();
            _isValidating = false;
        }

        private int Average(int value1, int value2)
        {
            return (int)Math.Round((value1 + value2) / 2.0);
        }

        private void ResetForm()
        {
            _selectedLoaiMucTieu = null;
            _selectedSpecialGoal = null;
            _selectedMuscleGoals.Clear();
            _selectedStartDate = null;
            _selectedEndDate = null;
            _isSelectingStartDate = true;
            _selectedDaysOfWeek.Keys.ToList().ForEach(k => _selectedDaysOfWeek[k] = false);
            _weeklySchedules.Clear();
            
            // Reset các buổi đã chọn
            string[] days = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            foreach (var day in days)
            {
                if (_selectedSessions.ContainsKey(day))
                {
                    _selectedSessions[day]["Sáng"] = false;
                    _selectedSessions[day]["Chiều"] = false;
                    _selectedSessions[day]["Tối"] = false;
                }
            }
            
            // Reset button thứ và buổi hiện tại
            _currentSelectedDayButton = null;
            _currentSelectedDay = null;
            ResetSessionButtons();
            
            // Reset các button thứ về màu ban đầu
            btnThu2.FillColor = Color.FromArgb(233, 252, 255);
            btnThu2.ForeColor = Color.FromArgb(0, 64, 64);
            btnThu3.FillColor = Color.FromArgb(233, 252, 255);
            btnThu3.ForeColor = Color.FromArgb(0, 64, 64);
            btnThu4.FillColor = Color.FromArgb(233, 252, 255);
            btnThu4.ForeColor = Color.FromArgb(0, 64, 64);
            btnThu5.FillColor = Color.FromArgb(233, 252, 255);
            btnThu5.ForeColor = Color.FromArgb(0, 64, 64);
            btnThu6.FillColor = Color.FromArgb(233, 252, 255);
            btnThu6.ForeColor = Color.FromArgb(0, 64, 64);
            btnThu7.FillColor = Color.FromArgb(233, 252, 255);
            btnThu7.ForeColor = Color.FromArgb(0, 64, 64);
            btnChuNhat.FillColor = Color.FromArgb(233, 252, 255);
            btnChuNhat.ForeColor = Color.FromArgb(0, 64, 64);
            
            _selectedFoods.Clear();
            _currentSelectedFood = null;
            _selectedExercises.Clear(); // Reset danh sách bài tập đã chọn
            _slotExerciseAssignments.Clear();
            ResetNutritionDisplay();
            UpdateGoalButtonStates();
            LoadCalendar();
            LoadExercises();
            UpdateButtonTexts();
        }

        private string ResolveFoodImagePath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return null;

            try
            {
                string normalized = rawPath.Replace("/", "\\").Trim('\\');
                if (Path.IsPathRooted(normalized) && File.Exists(normalized))
                {
                    return normalized;
                }

                string baseDir = Application.StartupPath;
                string directPath = Path.Combine(baseDir, normalized);
                if (File.Exists(directPath))
                    return directPath;

                string resourcesPath = Path.Combine(baseDir, "Resources", normalized);
                if (File.Exists(resourcesPath))
                    return resourcesPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ResolveFoodImagePath error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Dispose controllers - được gọi từ Designer
        /// </summary>
        private void DisposeControllers()
        {
            _goalController?.Dispose();
            _nutritionController?.Dispose();
        }

        /// <summary>
        /// Event handler cho button Trở Về - điều hướng về trang chủ (Dashboard)
        /// </summary>
        private void btnTroVe_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard1
                frmDashBoard1 parentForm = null;

                // Cách 1: Tìm qua FindForm()
                Form form = this.FindForm();
                if (form is frmDashBoard1)
                {
                    parentForm = form as frmDashBoard1;
                }
                // Cách 2: Tìm qua Application.OpenForms
                else
                {
                    foreach (Form openForm in Application.OpenForms)
                    {
                        if (openForm is frmDashBoard1)
                        {
                            parentForm = openForm as frmDashBoard1;
                            break;
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Reload dashboard (frmDashBoard1 có giao diện cố định, không cần load UserControl)
                    parentForm.ReloadDashboard();
                    parentForm.Show();
                    parentForm.BringToFront();
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy giá trị từ các textBox
                double? calo = ParseDouble(txtCalo.Text);
                double? protein = ParseDouble(txtProtein.Text);
                double? carbs = ParseDouble(txtCarbs.Text);
                double? chatBeo = ParseDouble(txtChatBeo.Text);
                double? chatXo = ParseDouble(txtChatXo.Text);

                // Validate tất cả các giá trị trong phạm vi ±10%
                bool isValid = true;
                string errorMessage = "Các giá trị sau vượt quá phạm vi cho phép (±10%):\n";

                if (!ValidateNutritionValue(calo, _originalCalo, "Calo", false))
                {
                    isValid = false;
                    errorMessage += $"- Calo: {_originalCalo?.ToString("0") ?? "0"} ±10%\n";
                }
                if (!ValidateNutritionValue(protein, _originalProtein, "Protein", false))
                {
                    isValid = false;
                    errorMessage += $"- Protein: {_originalProtein?.ToString("0") ?? "0"} ±10%\n";
                }
                if (!ValidateNutritionValue(carbs, _originalCarbs, "Carbs", false))
                {
                    isValid = false;
                    errorMessage += $"- Carbs: {_originalCarbs?.ToString("0") ?? "0"} ±10%\n";
                }
                if (!ValidateNutritionValue(chatBeo, _originalFat, "Chất béo", false))
                {
                    isValid = false;
                    errorMessage += $"- Chất béo: {_originalFat?.ToString("0") ?? "0"} ±10%\n";
                }
                if (!ValidateNutritionValue(chatXo, _originalFiber, "Chất xơ", false))
                {
                    isValid = false;
                    errorMessage += $"- Chất xơ: {_originalFiber?.ToString("0") ?? "0"} ±10%\n";
                }

                if (!isValid)
                {
                    MessageBox.Show(errorMessage, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật các label hiển thị
                label32.Text = calo?.ToString("0") ?? "0";
                label30.Text = protein?.ToString("0") ?? "0";
                label22.Text = carbs?.ToString("0") ?? "0";
                label28.Text = chatBeo?.ToString("0") ?? "0";
                label11.Text = chatXo?.ToString("0") ?? "0";

                MessageBox.Show("Đã cập nhật thông số dinh dưỡng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dinh dưỡng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validate giá trị dinh dưỡng có nằm trong phạm vi ±10% so với giá trị gốc không
        /// </summary>
        /// <param name="textBox">TextBox cần validate (null nếu chỉ kiểm tra giá trị)</param>
        /// <param name="originalValue">Giá trị gốc từ database</param>
        /// <param name="fieldName">Tên trường để hiển thị trong thông báo</param>
        /// <param name="showMessage">Có hiển thị message box không</param>
        /// <returns>True nếu hợp lệ, False nếu vượt quá phạm vi</returns>
        private bool ValidateNutritionValue(Guna.UI2.WinForms.Guna2TextBox textBox, double? originalValue, string fieldName, bool showMessage = true)
        {
            return ValidateNutritionValue(ParseDouble(textBox?.Text), originalValue, fieldName, showMessage, textBox);
        }

        /// <summary>
        /// Validate giá trị dinh dưỡng có nằm trong phạm vi ±10% so với giá trị gốc không
        /// </summary>
        /// <param name="newValue">Giá trị mới</param>
        /// <param name="originalValue">Giá trị gốc từ database</param>
        /// <param name="fieldName">Tên trường để hiển thị trong thông báo</param>
        /// <param name="showMessage">Có hiển thị message box không</param>
        /// <param name="textBox">TextBox để khôi phục giá trị (optional)</param>
        /// <returns>True nếu hợp lệ, False nếu vượt quá phạm vi</returns>
        private bool ValidateNutritionValue(double? newValue, double? originalValue, string fieldName, bool showMessage = true, Guna.UI2.WinForms.Guna2TextBox textBox = null)
        {
            // Nếu đang trong quá trình validate (tránh vòng lặp)
            if (_isValidating)
                return true;

            // Nếu không có giá trị gốc (chưa load từ database), cho phép nhập tự do
            if (!originalValue.HasValue || originalValue.Value == 0)
                return true;

            // Nếu giá trị mới rỗng, cho phép (người dùng có thể xóa)
            if (!newValue.HasValue)
                return true;

            // Tính phạm vi ±10%
            double minValue = originalValue.Value * 0.9; // 90% của giá trị gốc
            double maxValue = originalValue.Value * 1.1; // 110% của giá trị gốc

            // Kiểm tra giá trị mới có nằm trong phạm vi không
            if (newValue.Value < minValue || newValue.Value > maxValue)
            {
                if (showMessage)
                {
                    string message = $"{fieldName} chỉ được thay đổi trong phạm vi ±10% so với giá trị gốc.\n\n" +
                                   $"Giá trị gốc: {originalValue.Value:F0}\n" +
                                   $"Phạm vi cho phép: {minValue:F0} - {maxValue:F0}\n" +
                                   $"Giá trị bạn nhập: {newValue.Value:F0}";
                    MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Khôi phục giá trị gốc nếu có textBox
                if (textBox != null)
                {
                    _isValidating = true;
                    textBox.Text = originalValue.Value.ToString("0");
                    _isValidating = false;
                    textBox.Focus();
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse string thành double, trả về null nếu không hợp lệ
        /// </summary>
        private double? ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (double.TryParse(value.Trim(), out double result))
            {
                // Chỉ chấp nhận giá trị >= 0
                if (result >= 0)
                    return result;
                return null;
            }

            return null;
        }
    }
}
