# HealthApp
- Đăng nhập, đăng ký (phần này phải có đầy đủ quên mk, đổi mk)

=>Sau phần đăng nhập này sẽ cho user nhập vào các thông tin cần thiết

- Giao diện trang chủ -- Hiển thị tổng quan
		+ Tên, tuổi, giới tính...
		+ Tiến độ mục tiêu
		+ Tình hình sức khỏe
		+ Biểu đồ
		=>Trang chủ này chỉ hiển thị sơ thôi, khi user click vào từng mục thì sẽ đc đưa đến cửa sổ chi tiết
		+ Customizable view: cái này là cái theme(dark/light), Ngày tháng, API đến thời tiết....
			=> Chuyển đổi ngôn ngữ: cái này trước đó Lộc với Đ.Anh có làm, nếu có thể thì mình cũng làm luôn
- Các chức năng cụ thể
	+ Kế hoạch tập luyện: gồm lên kế hoạch và quản lý tập luyện; cái này sẽ là user đưa ra mục tiêu, yêu cầu, hệ thống sẽ đưa ra các gợi ý, phán đoán dựa trên tình trạng hiện tại của bệnh nhân (cái này cũng tích hợp AI)
	+ Mục tiêu cá nhân: có thể gộp với lên kế hoạch tập luyện như trên hoặc tách ra, mỗi cái quản lý một thứ cho nó bớt rối khi dùng chung 1 giao diện
	+ Chế độ dinh dưỡng: Ngoài đưa ra qua trình luyện tập thì còn đưa ra chế độ ăn uống nữa, cũng là hệ thống gợi ý và user lựa chọn.
	+ Xếp hạng: cái này thì chỉ là cập nhật thông số rồi đưa ra xếp hạng giữa các user. Xếp hạng sẽ theo từng khoảng(theo ngày, tuần, tháng...)
	+ Thống kê: Export dữ liệu, báo cáo với pdf, excel (gồm các chỉ số thống kê, thành tựu, biểu đồ so sánh) đây là một giao diện cho user thao tác và có thêm mấy cái button xuất excel, pdf
	+ Sắp xếp mục tiêu: CÁI NÀY KHÓ, sử dụng kéo thả để sắp xếp các mục tiêu trong listView, làm kéo thả thì khá khó. Mục này có thể gộp đc vào "Mục tiêu cá nhân" hoặc "Kế hoạch tập luyện". Cái này còn tùy điều kiện
	+ Nhắc nhở và gửi thông báo: cái này thì làm cái biểu tượng hộp thư rồi nhét vào trong đấy
- Tích hợp AI:
	+ Phân tích dữ liệu: Phân tích xu hướng từ dữ liệu (lượng calo, cân nặng...)...
	+ Dự đoán rủi ro sức khỏe: đưa ra các cảnh báo dựa trên chỉ số sức khỏe, quá trình luyện tập, chế độ ăn uống... để user cải thiện, thay đổi và phòng ngừa...
	+ Gợi ý tập luyện: cũng dựa trên việc luyện tập của user và "Kế hoạch tập luyện" để đưa ra các gợi ý (vd: "Giảm 500 calo hôm nay để đạt mục tiêu" hoặc "nên chạy bộ 30 phút") mấy cái này như mấy cái banner chứ không nên là một trang. Khi user làm theo các gợi ý thì hệ thống sẽ cộng thêm chút điểm vào điểm xếp hạng, có dấu "x" để user tắt nha
- Tích hợp API:
	+ Gọi OpenWeatherMap API để lấy dữ liệu thời tiết realtime và gợi ý hoạt động (Này kết hợp với AI nữa)
	+ Tích hợp dinh dưỡng: Phân tích calo, dinh dưỡng từ món ăn. Cái này để hỗ trợ theo dõi chế độ ăn
	+ Reminders thông minh: Gửi mấy cái icon hay mấy câu cổ vũ (vd: "Bạn mới đi 3000 bước, cố lên!"), tích hợp vào việc luyện tập của user 
- Mấy cái t cho rằng là nâng cao: 
	+ Voice input: Nhập dữ liệu bằng giọng nói
	+ Theme chuyển đổi dark<->light cũng khá dâm
	+ Kéo thả để sắp xếp mục tiêu luyệ tập nữa
	=> mấy cái này làm được thì tốt
- Thêm vào chức năng PT cho app:
	+ Trong trang PT này sẽ có 2 model chính: Thuê PT và Đăng ký làm PT
	+ User có thể tìm PT theo nhu cầu của mình. User sẽ xem được profile(chứng chỉ, chuyên môn, giá cả), đánh giá... của PT trong app
	+ Các user có thể đăng ký làm PT: cần phải tạo profile, nhận job. PT nhận job thì cũng sẽ bị app khấu trừ tiền
	+ Các user đã đăng ký làm PT thì giao diện này cũng sẽ thay đổi. PT có thể truy cập được sức khỏe của client
	+ Thêm chức năng chat giữa PT và client trong mục này
	+ Có thể để một icon PT ở trang chủ để user bấm vào sẽ đưa đến trang này luôn
APP NÀY SẼ LÀM THEO CÁC NHU CẦU THỰC TẾ CỦA USER, NÊN CHÚNG MÀY CÓ CÁI NÀO THỰC TẾ VÀ HỢP LÝ THÌ THÊM VÀO LUÔN NHA. CÒN LÀM ĐƯỢC HAY KHÔNG THÌ CHƯA BIẾT

--------------------Lộc — Authentication + Dashboard + Notifications + Achievements-----------------------
Nhánh chính:
feature/auth-loc
feature/dashboard-loc
feature/notifications-loc
feature/achievements-loc
Phạm vi (theo README):
Auth & User Management: LoginForm, RegisterForm, ForgotPasswordForm, quản lý profile, đổi mật khẩu, xóa tài khoản
Dashboard tổng quan: Today summary, quick stats cards, weekly trends, recent achievements, upcoming reminders, optional weather widget
Notifications & Reminders: uống nước, vận động, đo chỉ số, đi ngủ, daily/weekly summary, achievement unlocked
Achievements System: thiết kế badge/event, hiển thị và logic tính điểm
Tích hợp chéo:
Lấy “quick stats” từ Activity/Health/Sleep/Nutrition để render dashboard
Nhận “goal completion” từ Goals để bật achievements/notifications
----------------------------Long — Health Metrics + Reports & Analytics-------------------------------
Nhánh chính:
feature/health-metrics-long
feature/reports-analytics-long
Phạm vi:
Health Metrics: HealthMetricsForm, AddMetricForm, HeartRateForm, BloodPressureForm, WeightTrackingForm, BMICalculatorForm
Service phân tích: BMI, BMR, trend, cảnh báo bất thường
Reports & Analytics: Weekly/Monthly/Custom reports, export PDF/Excel, comparative analysis, health score, insights
Tích hợp chéo:
Widget “current BMI/weight trend” cho Dashboard
Cảnh báo đẩy sang Notifications
Dữ liệu HealthMetrics là nguồn chính cho Reports
---------------------------Nhân — Sleep Tracking + Goals & Progress------------------------------------
Nhánh chính:
feature/sleep-tracking-nhan
feature/goals-progress-nhan
Phạm vi:
Sleep Tracking: SleepTrackingForm, AddSleepDataForm, SleepAnalysisForm, SleepChartUserControl (Deep/Light/REM), sleep score
Goals & Progress: đặt mục tiêu (weight, steps, sleep, nutrition…), progress %, biểu đồ tiến độ, milestone, completion notifications
Tích hợp chéo:
Widget sleep (giờ ngủ, score) cho Dashboard
Goals liên kết Activity (steps/calories), Health (weight), Nutrition (calories/macros), Sleep (duration/quality)
Push completion qua Notifications; cấp Achievements
---------------------------------Đức Anh — Nutrition Management + Settings---------------------------------
Nhánh chính:
feature/nutrition-ducanh
feature/settings-ducanh
Phạm vi:
Nutrition: NutritionForm, AddMealForm, FoodDatabaseForm, CalorieTrackerForm, NutritionChartUserControl (calories in, macro split P/C/F, so sánh mục tiêu), water intake
Settings: profile settings, notification preferences, theme (light/dark), language, units (metric/imperial), data management (backup/restore), export all data
Tích hợp chéo:
Widget “calories in/macros” cho Dashboard
Đồng bộ nutrition goals với Goals
Preferences ảnh hưởng Notifications và Reports









# HealthApp - Ứng dụng quản lý sức khỏe cá nhân (MVC Pattern)

## 📋 Mô tả dự án
Ứng dụng Windows Forms quản lý sức khỏe người dùng theo mô hình **MVC (Model-View-Controller)**, lấy cảm hứng từ Apple Health và Samsung Health, được xây dựng bằng C# với phương pháp **Database First**.




## 📂 Cấu trúc thư mục chi tiết

### 🔷 **Models/** - Entity Classes (M trong MVC)
**Mục đích**: Chứa các entity classes được generate từ database (Database First)

**Các Models chính**:
```
Models/
├── User.cs                    // Thông tin người dùng
├── UserProfile.cs             // Profile chi tiết
├── Activity.cs                // Hoạt động thể chất
├── ActivityType.cs            // Loại hoạt động
├── HealthMetric.cs            // Chỉ số sức khỏe tổng quát
├── HeartRate.cs               // Nhịp tim
├── BloodPressure.cs           // Huyết áp
├── Weight.cs                  // Cân nặng
├── Height.cs                  // Chiều cao
├── Sleep.cs                   // Dữ liệu giấc ngủ
├── SleepStage.cs              // Giai đoạn ngủ
├── Nutrition.cs               // Bữa ăn
├── NutritionDetail.cs         // Chi tiết dinh dưỡng
├── Food.cs                    // Thực phẩm
├── Goal.cs                    // Mục tiêu sức khỏe
├── GoalProgress.cs            // Tiến độ mục tiêu
├── Reminder.cs                // Nhắc nhở
├── Achievement.cs             // Thành tích
└── UserAchievement.cs         // Thành tích của user
```

**Quy tắc**:
- Mỗi Model tương ứng với 1 bảng trong database
- Sử dụng Data Annotations để validate
- Không chứa business logic

---

### 🔷 **Views/** - User Interface (V trong MVC)
**Mục đích**: Chứa tất cả Windows Forms và UserControls, được tổ chức theo tính năng

#### 📁 **Views/Dashboard/**
```
DashboardForm.cs              // Form dashboard chính
DashboardForm.Designer.cs
DashboardForm.resx
OverviewUserControl.cs        // UserControl tổng quan
QuickStatsUserControl.cs      // Thống kê nhanh
TodaySummaryUserControl.cs    // Tóm tắt hôm nay
```

#### 📁 **Views/Activity/**
```
ActivityForm.cs               // Form quản lý hoạt động
ActivityListForm.cs           // Danh sách hoạt động
AddActivityForm.cs            // Thêm hoạt động mới
ActivityDetailForm.cs         // Chi tiết hoạt động
ActivityChartUserControl.cs   // Biểu đồ hoạt động
```

#### 📁 **Views/HealthMetrics/**
```
HealthMetricsForm.cs          // Form chỉ số sức khỏe
AddMetricForm.cs              // Nhập chỉ số mới
HeartRateForm.cs              // Theo dõi nhịp tim
BloodPressureForm.cs          // Theo dõi huyết áp
WeightTrackingForm.cs         // Theo dõi cân nặng
BMICalculatorForm.cs          // Tính BMI
MetricChartUserControl.cs     // Biểu đồ chỉ số
```

#### 📁 **Views/Sleep/**
```
SleepTrackingForm.cs          // Form theo dõi giấc ngủ
AddSleepDataForm.cs           // Thêm dữ liệu ngủ
SleepAnalysisForm.cs          // Phân tích giấc ngủ
SleepChartUserControl.cs      // Biểu đồ giấc ngủ
```

#### 📁 **Views/Nutrition/**
```
NutritionForm.cs              // Form quản lý dinh dưỡng
AddMealForm.cs                // Thêm bữa ăn
FoodDatabaseForm.cs           // Database thực phẩm
CalorieTrackerForm.cs         // Theo dõi calories
NutritionChartUserControl.cs  // Biểu đồ dinh dưỡng
```

#### 📁 **Views/Goals/**
```
GoalsForm.cs                  // Form quản lý mục tiêu
AddGoalForm.cs                // Thêm mục tiêu mới
GoalDetailForm.cs             // Chi tiết mục tiêu
GoalProgressUserControl.cs    // Tiến độ mục tiêu
```

#### 📁 **Views/Reports/**
```
ReportsForm.cs                // Form báo cáo
WeeklyReportForm.cs           // Báo cáo tuần
MonthlyReportForm.cs          // Báo cáo tháng
CustomReportForm.cs           // Báo cáo tùy chỉnh
ExportOptionsForm.cs          // Tùy chọn export
```

#### 📁 **Views/Settings/**
```
SettingsForm.cs               // Form cài đặt
ProfileSettingsForm.cs        // Cài đặt profile
NotificationSettingsForm.cs   // Cài đặt thông báo
DataManagementForm.cs         // Quản lý dữ liệu
ThemeSettingsForm.cs          // Cài đặt giao diện
```

#### 📁 **Views/Auth/**
```
LoginForm.cs                  // Form đăng nhập
RegisterForm.cs               // Form đăng ký
ForgotPasswordForm.cs         // Quên mật khẩu
```

#### 📁 **Views/Shared/**
```
LoadingUserControl.cs         // Loading indicator
ChartBaseUserControl.cs       // Base class cho charts
DateRangePickerControl.cs     // Chọn khoảng thời gian
CustomMessageBox.cs           // Message box tùy chỉnh
```

**Quy tắc Views**:
- Mỗi Form/UserControl chỉ giao tiếp với Controller
- Không chứa business logic
- Sử dụng Data Binding khi có thể
- Không truy cập database trực tiếp

---

### 🔷 **Controllers/** - Business Logic (C trong MVC)
**Mục đích**: Điều phối giữa Views và Models, xử lý logic nghiệp vụ

**Các Controllers chính**:
```
Controllers/
├── AuthController.cs          // Xác thực, đăng nhập
├── DashboardController.cs     // Logic dashboard
├── ActivityController.cs      // Quản lý hoạt động
├── HealthMetricController.cs  // Quản lý chỉ số sức khỏe
├── SleepController.cs         // Quản lý giấc ngủ
├── NutritionController.cs     // Quản lý dinh dưỡng
├── GoalController.cs          // Quản lý mục tiêu
├── ReportController.cs        // Tạo báo cáo
├── SettingsController.cs      // Cài đặt ứng dụng
└── NotificationController.cs  // Quản lý thông báo
```

**Nhiệm vụ Controller**:
- Nhận input từ View
- Gọi Service để xử lý nghiệp vụ
- Cập nhật Model
- Trả kết quả về View
- Handle exceptions

**Example Pattern**:
```csharp
public class ActivityController
{
    private readonly IActivityService _activityService;
    
    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }
    
    public async Task<bool> AddActivity(ActivityViewModel model)
    {
        try
        {
            return await _activityService.CreateActivity(model);
        }
        catch (Exception ex)
        {
            // Log and handle
            return false;
        }
    }
}
```

---

### 🔷 **Data/** - Database Context
**Mục đích**: Entity Framework DbContext và configurations

```
Data/
├── HealthAppDbContext.cs          // DbContext chính
├── DatabaseInitializer.cs         // Seed data
└── Configurations/
    ├── UserConfiguration.cs       // Fluent API cho User
    ├── ActivityConfiguration.cs   // Fluent API cho Activity
    ├── HealthMetricConfiguration.cs
    ├── SleepConfiguration.cs
    └── NutritionConfiguration.cs
```

---

### 🔷 **Repositories/** - Data Access Layer
**Mục đích**: Trừu tượng hóa việc truy cập database

```
Repositories/
├── Interfaces/
│   ├── IRepository.cs             // Generic repository interface
│   ├── IUserRepository.cs
│   ├── IActivityRepository.cs
│   ├── IHealthMetricRepository.cs
│   ├── ISleepRepository.cs
│   ├── INutritionRepository.cs
│   ├── IGoalRepository.cs
│   └── IUnitOfWork.cs            // Unit of Work pattern
└── Implementations/
    ├── Repository.cs              // Generic repository
    ├── UserRepository.cs
    ├── ActivityRepository.cs
    ├── HealthMetricRepository.cs
    ├── SleepRepository.cs
    ├── NutritionRepository.cs
    ├── GoalRepository.cs
    └── UnitOfWork.cs
```

---

### 🔷 **Services/** - Business Services
**Mục đích**: Xử lý business logic phức tạp

```
Services/
├── Interfaces/
│   ├── IAuthService.cs
│   ├── IActivityService.cs
│   ├── IHealthAnalysisService.cs
│   ├── ISleepAnalysisService.cs
│   ├── INutritionService.cs
│   ├── IGoalTrackingService.cs
│   ├── IReportService.cs
│   ├── INotificationService.cs
│   ├── IChartService.cs
│   └── IExportService.cs
└── Implementations/
    ├── AuthService.cs             // Authentication & Authorization
    ├── ActivityService.cs         // Tính calories, phân tích hoạt động
    ├── HealthAnalysisService.cs   // Phân tích chỉ số sức khỏe
    ├── SleepAnalysisService.cs    // Phân tích giấc ngủ
    ├── NutritionService.cs        // Tính toán dinh dưỡng
    ├── GoalTrackingService.cs     // Theo dõi tiến độ mục tiêu
    ├── ReportService.cs           // Generate reports
    ├── NotificationService.cs     // Push notifications
    ├── ChartService.cs            // Tạo biểu đồ
    └── ExportService.cs           // Export PDF/Excel
```

---

### 🔷 **ViewModels/** - View Models
**Mục đích**: Chứa dữ liệu được format để hiển thị trên View

```
ViewModels/
├── DashboardViewModel.cs
├── ActivityViewModel.cs
├── HealthMetricViewModel.cs
├── SleepViewModel.cs
├── NutritionViewModel.cs
├── GoalViewModel.cs
├── ReportViewModel.cs
├── ChartViewModel.cs
└── UserViewModel.cs
```

---

### 🔷 **DTOs/** - Data Transfer Objects
**Mục đích**: Transfer data giữa các layers

```
DTOs/
├── Request/
│   ├── CreateActivityRequest.cs
│   ├── UpdateHealthMetricRequest.cs
│   ├── AddSleepDataRequest.cs
│   └── CreateGoalRequest.cs
└── Response/
    ├── ActivityResponse.cs
    ├── HealthMetricResponse.cs
    ├── SleepAnalysisResponse.cs
    └── ReportResponse.cs
```

---

### 🔷 **Common/** - Shared Utilities
**Mục đích**: Code dùng chung cho toàn dự án

#### **Common/Helpers/**
```
DateTimeHelper.cs              // Format date/time
ValidationHelper.cs            // Validate inputs
EncryptionHelper.cs            // Hash passwords
ChartHelper.cs                 // Chart utilities
CalculationHelper.cs           // BMI, BMR, calories
ImageHelper.cs                 // Image processing
FileHelper.cs                  // File operations
```

#### **Common/Extensions/**
```
StringExtensions.cs            // String utilities
DateTimeExtensions.cs          // DateTime utilities
CollectionExtensions.cs        // Collection utilities
FormExtensions.cs              // Form utilities
```

#### **Common/Constants/**
```
AppConstants.cs                // Application constants
MessageConstants.cs            // User messages
ColorConstants.cs              // UI colors
PathConstants.cs               // File paths
```

#### **Common/Enums/**
```
ActivityType.cs                // Walking, Running, Cycling...
HealthMetricType.cs            // Weight, Height, BloodPressure...
GoalType.cs                    // WeightLoss, MuscleGain...
SleepQuality.cs                // Excellent, Good, Fair, Poor
MealType.cs                    // Breakfast, Lunch, Dinner, Snack
NotificationType.cs            // Reminder, Achievement, Warning
```

---

### 🔷 **Resources/** - Static Resources
```
Resources/
├── Images/
│   ├── Dashboard/             // Dashboard images
│   ├── Activities/            // Activity icons/images
│   └── Health/                // Health metric icons
├── Icons/
│   ├── menu-icons/
│   └── action-icons/
├── Fonts/
│   ├── Roboto/
│   └── OpenSans/
└── Themes/
    ├── light-theme.xml
    └── dark-theme.xml
```

---

### 🔷 **Assets/** - Generated Assets
```
Assets/
└── Reports/
    ├── Templates/
    │   ├── weekly-report-template.html
    │   ├── monthly-report-template.html
    │   └── custom-report-template.html
    └── Generated/
        └── [Auto-generated reports here]
```

---

### 🔷 **Migrations/** - EF Core Migrations
```
Migrations/
├── 20240101000000_InitialCreate.cs
├── 20240102000000_AddHealthMetrics.cs
└── 20240103000000_AddGoalsTable.cs
```

---

### 🔷 **Configs/** - Configuration Files
```
Configs/
├── AppSettings.json           // App settings
├── DatabaseConfig.cs          // Database configuration
├── LoggingConfig.cs           // Logging settings
└── EmailConfig.cs             // Email configuration
```

---

### 🔷 **Tests/** - Testing
```
Tests/
├── UnitTests/
│   ├── Services/
│   │   ├── ActivityServiceTests.cs
│   │   ├── HealthAnalysisServiceTests.cs
│   │   └── NutritionServiceTests.cs
│   ├── Repositories/
│   │   └── ActivityRepositoryTests.cs
│   └── Controllers/
│       └── ActivityControllerTests.cs
└── IntegrationTests/
    ├── DatabaseTests.cs
    └── EndToEndTests.cs
```

---

### 🔷 **Docs/** - Documentation
```
Docs/
├── README.md
├── ARCHITECTURE.md            // Kiến trúc hệ thống
├── API_DOCUMENTATION.md       // API docs
├── USER_GUIDE.md              // Hướng dẫn sử dụng
├── DEVELOPER_GUIDE.md         // Hướng dẫn dev
├── Images/
│   ├── screenshots/
│   └── diagrams/
└── Database/
    ├── ERD.png
    ├── DatabaseSchema.sql
    └── SampleData.sql
```

---

### 🔷 **Logs/** - Application Logs
```
Logs/
├── app-log-2024-01-01.txt
├── error-log-2024-01-01.txt
└── debug-log-2024-01-01.txt
```

---

### 🔷 **Backups/** - Database Backups
```
Backups/
├── backup-2024-01-01.bak
└── backup-2024-01-02.bak
```

---

## 🚀 Quy trình phát triển theo MVC

### **Phase 1: Database Design (Tuần 1)**

#### 1.1. Thiết kế Database Schema
```sql
-- Users
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- User Profiles
CREATE TABLE UserProfiles (
    ProfileId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    FullName NVARCHAR(100),
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Height DECIMAL(5,2),
    TargetWeight DECIMAL(5,2)
);

-- Activities
CREATE TABLE Activities (
    ActivityId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    ActivityTypeId INT,
    StartTime DATETIME,
    EndTime DATETIME,
    Duration INT, -- minutes
    Distance DECIMAL(10,2), -- km
    CaloriesBurned DECIMAL(10,2),
    Steps INT,
    Notes NVARCHAR(500)
);

-- Health Metrics
CREATE TABLE HealthMetrics (
    MetricId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    RecordedAt DATETIME,
    Weight DECIMAL(5,2),
    HeartRate INT,
    SystolicBP INT,
    DiastolicBP INT,
    BloodGlucose DECIMAL(5,2)
);

-- Sleep Data
CREATE TABLE SleepRecords (
    SleepId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    SleepDate DATE,
    BedTime DATETIME,
    WakeTime DATETIME,
    TotalSleep INT, -- minutes
    DeepSleep INT,
    LightSleep INT,
    REM INT,
    Awake INT,
    SleepQuality NVARCHAR(20)
);

-- Nutrition
CREATE TABLE Meals (
    MealId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    MealDate DATE,
    MealType NVARCHAR(20), -- Breakfast, Lunch, Dinner, Snack
    TotalCalories DECIMAL(10,2),
    Protein DECIMAL(10,2),
    Carbs DECIMAL(10,2),
    Fat DECIMAL(10,2),
    Notes NVARCHAR(500)
);

-- Goals
CREATE TABLE Goals (
    GoalId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    GoalType NVARCHAR(50),
    TargetValue DECIMAL(10,2),
    CurrentValue DECIMAL(10,2),
    StartDate DATE,
    EndDate DATE,
    IsActive BIT,
    IsCompleted BIT
);
```

#### 1.2. Scaffold Models từ Database
```bash
Scaffold-DbContext "Server=.;Database=HealthAppDB;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context HealthAppDbContext -ContextDir Data -Force
```

---

### **Phase 2: Data Layer (Tuần 1-2)**

#### 2.1. Tạo Repository Pattern
```csharp
// IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### 2.2. Implement Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IActivityRepository Activities { get; }
    IHealthMetricRepository HealthMetrics { get; }
    Task<int> CommitAsync();
}
```

---

### **Phase 3: Business Layer (Tuần 2-3)**

#### 3.1. Implement Services
- ActivityService: Tính calories, phân tích hoạt động
- HealthAnalysisService: BMI, BMR, xu hướng sức khỏe
- SleepAnalysisService: Phân tích chất lượng giấc ngủ
- NutritionService: Tính toán dinh dưỡng
- GoalTrackingService: Theo dõi tiến độ

---

### **Phase 4: Controllers (Tuần 3)**

#### 4.1. Implement Controllers
```csharp
public class ActivityController
{
    private readonly IActivityService _service;
    
    public async Task<ActivityViewModel> GetTodayActivities(int userId)
    {
        var activities = await _service.GetActivitiesByDate(userId, DateTime.Today);
        return MapToViewModel(activities);
    }
    
    public async Task<bool> AddActivity(ActivityViewModel model)
    {
        var dto = MapToDto(model);
        return await _service.CreateActivity(dto);
    }
}
```

---

### **Phase 5: Views (Tuần 4-6)**

#### 5.1. Dashboard
- Hiển thị tổng quan: bước chân, calories, giờ ngủ
- Biểu đồ xu hướng tuần
- Quick stats cards

#### 5.2. Activity Tracking
- Form thêm/sửa/xóa hoạt động
- Danh sách hoạt động với filter
- Biểu đồ thống kê

#### 5.3. Health Metrics
- Nhập các chỉ số: cân nặng, huyết áp, nhịp tim
- Biểu đồ theo dõi theo thời gian
- Cảnh báo bất thường

#### 5.4. Sleep Tracking
- Ghi nhận giờ ngủ/thức
- Phân tích chu kỳ ngủ
- Tips cải thiện giấc ngủ

#### 5.5. Nutrition
- Thêm bữa ăn từ database thực phẩm
- Tính tổng calories nạp vào
- Biểu đồ cân đối dinh dưỡng (Protein/Carbs/Fat)
- So sánh với mục tiêu

#### 5.6. Goals & Progress
- Đặt mục tiêu: giảm cân, tăng cơ, cải thiện giấc ngủ
- Tracking tiến độ hàng ngày
- Achievements & Badges
- Motivation messages

#### 5.7. Reports
- Báo cáo tuần/tháng/năm
- Export PDF/Excel
- Biểu đồ tổng hợp
- So sánh các khoảng thời gian

---

### **Phase 7: Advanced Features (Tuần 8)**

#### 7.1. Notifications System
- Nhắc nhở uống nước
- Nhắc nhở vận động
- Nhắc nhở ghi nhận dữ liệu
- Thông báo hoàn thành mục tiêu

#### 7.2. Data Export/Import
- Export data sang CSV/JSON
- Backup database
- Restore từ backup
- Import data từ file

#### 7.3. Advanced Charts
- Line charts cho xu hướng
- Bar charts cho so sánh
- Pie charts cho tỷ lệ
- Area charts cho tích lũy

---

### 🎯 Các tính năng chính (Feature List)

### **1. Authentication & User Management**
- ✅ Đăng ký tài khoản với email verification
- ✅ Đăng nhập/Đăng xuất
- ✅ Quên mật khẩu & reset password
- ✅ Quản lý profile (avatar, thông tin cá nhân)
- ✅ Đổi mật khẩu
- ✅ Xóa tài khoản

### **2. Dashboard**
- ✅ Tổng quan hôm nay: bước chân, calories in/out, giờ ngủ
- ✅ Quick stats: cân nặng hiện tại, BMI, mục tiêu
- ✅ Biểu đồ xu hướng tuần
- ✅ Upcoming reminders
- ✅ Recent achievements
- ✅ Weather widget (optional)

### **3. Activity Tracking**
- ✅ Ghi nhận các loại hoạt động: đi bộ, chạy, gym, bơi lội, yoga...
- ✅ Tính toán tự động calories tiêu thụ
- ✅ Đếm bước chân (manual input)
- ✅ Theo dõi quãng đường
- ✅ Lịch sử hoạt động với filter (ngày, tuần, tháng)
- ✅ Biểu đồ thống kê calories burned
- ✅ So sánh với mục tiêu hàng ngày

### **4. Health Metrics**
- ✅ Nhập cân nặng định kỳ
- ✅ Tính BMI tự động
- ✅ Theo dõi huyết áp
- ✅ Theo dõi nhịp tim nghỉ
- ✅ Theo dõi đường huyết (cho người tiểu đường)
- ✅ Biểu đồ xu hướng các chỉ số
- ✅ Cảnh báo khi chỉ số bất thường
- ✅ Notes cho mỗi lần đo

### **5. Sleep Tracking**
- ✅ Ghi nhận giờ đi ngủ và thức dậy
- ✅ Tính tổng giờ ngủ
- ✅ Phân tích chu kỳ ngủ (deep, light, REM, awake)
- ✅ Đánh giá chất lượng giấc ngủ
- ✅ Sleep score
- ✅ Biểu đồ pattern ngủ
- ✅ Tips cải thiện giấc ngủ
- ✅ Sleep goal tracking

### **6. Nutrition Management**
- ✅ Database thực phẩm (1000+ items)
- ✅ Thêm bữa ăn (breakfast, lunch, dinner, snack)
- ✅ Tính calories nạp vào
- ✅ Tracking macros (protein, carbs, fat)
- ✅ Water intake tracking
- ✅ Biểu đồ cân đối dinh dưỡng
- ✅ So sánh với nutrition goals
- ✅ Meal planning (optional)

### **7. Goals & Progress**
- ✅ Đặt các loại mục tiêu:
  - Weight loss/gain
  - Muscle building
  - Improve sleep quality
  - Increase daily steps
  - Maintain healthy eating
- ✅ Tracking tiến độ hàng ngày
- ✅ Progress percentage
- ✅ Biểu đồ tiến độ
- ✅ Goal completion notifications
- ✅ Milestone celebrations

### **8. Reports & Analytics**
- ✅ Báo cáo tuần (weekly summary)
- ✅ Báo cáo tháng (monthly report)
- ✅ Báo cáo tùy chỉnh (custom date range)
- ✅ Export PDF với charts
- ✅ Export Excel với raw data
- ✅ Comparative analysis (so sánh các khoảng thời gian)
- ✅ Health score calculation
- ✅ Insights & recommendations

### **9. Notifications & Reminders**
- ✅ Nhắc nhở uống nước
- ✅ Nhắc nhở vận động
- ✅ Nhắc nhở đo chỉ số sức khỏe
- ✅ Nhắc nhở đi ngủ
- ✅ Thông báo hoàn thành mục tiêu
- ✅ Achievement unlocked notifications
- ✅ Daily/weekly summary notifications

### **10. Settings**
- ✅ Profile settings
- ✅ Notification preferences
- ✅ Theme selection (light/dark)
- ✅ Language selection
- ✅ Units (metric/imperial)
- ✅ Data management (backup/restore)
- ✅ Export all data
- ✅ Delete account

### **11. Achievements System**
- ✅ First workout
- ✅ 7-day streak
- ✅ 30-day streak
- ✅ 10,000 steps in a day
- ✅ Goal completed
- ✅ Weight milestone reached
- ✅ Perfect week (all goals met)
- ✅ Early bird (consistent sleep schedule)

---

## 📐 Design Patterns sử dụng

### **1. MVC (Model-View-Controller)**
- Tách biệt UI, Business Logic, và Data
- Views không biết về Database
- Controllers orchestrate giữa Views và Services

### **2. Repository Pattern**
- Trừu tượng hóa data access
- Dễ test và maintain
- Có thể đổi database dễ dàng

### **3. Unit of Work Pattern**
- Quản lý transactions
- Đảm bảo data consistency
- Commit multiple changes cùng lúc

### **4. Dependency Injection**
- Loose coupling
- Dễ test với mock objects
- Centralized configuration

### **5. Factory Pattern**
- Tạo objects phức tạp
- ChartFactory, ReportFactory

### **6. Strategy Pattern**
- Các thuật toán tính calories khác nhau
- Export strategies (PDF, Excel, CSV)

### **7. Observer Pattern**
- Event handling trong Forms
- Notification system

### **8. Singleton Pattern**
- DbContext instance
- Configuration manager

---
**Last Updated**: [Current Date]  
**Version**: 1.0  
**Author**: [Your Name]