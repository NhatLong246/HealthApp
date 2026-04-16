<div align="center">

# 🏋️ HealthApp - Comprehensive Health & Fitness Management System

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-9.0-purple?logo=csharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-6.5.1-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red?logo=microsoftsqlserver)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows)
![License](https://img.shields.io/badge/License-MIT-yellow)

**A modern Windows desktop application for comprehensive health tracking, workout management, nutrition planning, and personal trainer connectivity.**

[Features](#-key-features) • [Tech Stack](#-tech-stack) • [Installation](#-installation) • [Screenshots](#-screenshots) • [Architecture](#-architecture) • [Contributing](#-contributing)

</div>

---

## 📋 Overview

**HealthApp** is a full-featured health and fitness management system designed to help users achieve their wellness goals. The application combines workout tracking, nutrition management, personal trainer services, and social features into a seamless desktop experience with a modern, intuitive interface.

### 🎯 Target Users
- **Fitness Enthusiasts** - Track workouts, set goals, and monitor progress
- **Personal Trainers (PT)** - Manage clients, schedule sessions, and provide guidance
- **Administrators** - Oversee platform operations, approve PTs, and manage content

---

## ✨ Key Features

### 🏃 Workout & Exercise Management
- **Exercise Library** - 500+ exercises categorized by muscle group and difficulty level
- **Training Plans** - Customizable workout schedules with automatic reminders
- **Session Tracking** - Real-time workout logging with timer and progress tracking
- **Video Tutorials** - Integrated exercise demonstrations using VLC.DotNet
- **Goal-based Filtering** - Find exercises matching your fitness objectives (Weight Loss, Muscle Gain, etc.)

### 🥗 Nutrition & Diet Planning
- **Food Database** - Comprehensive food library with nutritional information
- **AI-Powered Recommendations** - ChatGPT integration for personalized meal suggestions
- **Meal Planning** - Create and manage daily/weekly eating schedules
- **Macro Tracking** - Monitor calories, protein, carbs, fat, and fiber intake
- **Diet Templates** - Pre-built dietary plans for various goals

### 📊 Health Analytics & Metrics
- **BMI Calculator** - Body Mass Index calculation with health category classification
- **BMR Calculator** - Basal Metabolic Rate using Mifflin-St Jeor formula
- **TDEE Calculator** - Total Daily Energy Expenditure based on activity level
- **Progress Reports** - Visual charts and statistics for workout history
- **Export Features** - Generate PDF/Excel reports for tracking progress

### 👨‍🏫 Personal Trainer (PT) System
- **PT Marketplace** - Search and filter trainers by specialty, rating, and availability
- **Booking System** - Schedule sessions with preferred trainers
- **Rating & Reviews** - Community-driven trainer evaluation system
- **PT Dashboard** - Dedicated interface for trainers to manage clients
- **Verification System** - Admin-approved trainer certification

### 💳 Payment Integration
- **MoMo Wallet** - Seamless mobile payment integration
- **ZaloPay** - Alternative payment gateway support
- **QR Code Payments** - Quick payment via QR scanning
- **Transaction History** - Complete payment records and invoices
- **Membership Packages** - Subscription-based premium features

### 👥 Social & Gamification
- **Friends System** - Connect and follow other users
- **Leaderboard** - Competitive rankings based on achievements
- **Achievement Sharing** - Post and celebrate fitness milestones
- **Community Engagement** - Like and comment on shared achievements

### 🔔 Smart Notifications
- **Email Reminders** - Automated workout schedule notifications
- **In-app Alerts** - Real-time notifications for bookings, achievements, etc.
- **Background Service** - WorkoutNotificationService for scheduled reminders

### 🔐 Security & Authentication
- **Secure Login** - Password hashing with ASP.NET Core Cryptography
- **Password Recovery** - Email-based OTP verification system
- **Role-based Access** - Distinct permissions for Users, PTs, and Admins

---

## 🛠 Tech Stack

### Core Technologies
| Technology | Version | Purpose |
|------------|---------|---------|
| **C#** | 9.0 | Primary programming language |
| **.NET Framework** | 4.8 | Application framework |
| **Windows Forms** | - | Desktop UI framework |
| **Entity Framework** | 6.5.1 | ORM for database operations |
| **SQL Server** | 2019+ | Relational database |

### UI & Visualization
| Library | Purpose |
|---------|---------|
| **Guna.UI2.WinForms** | Modern UI components with Material Design |
| **DataVisualization** | Charts and graphs for analytics |
| **WebView2** | Embedded web content rendering |
| **VLC.DotNet** | Video playback for exercise tutorials |

### External Services & APIs
| Service | Integration |
|---------|-------------|
| **OpenAI ChatGPT** | AI-powered food recommendations |
| **MoMo Payment** | Mobile wallet payments |
| **ZaloPay** | Payment gateway |
| **SMTP Email** | Notification delivery |

### Additional Libraries
| Library | Purpose |
|---------|---------|
| **QRCoder** | QR code generation for payments |
| **Newtonsoft.Json** | JSON serialization |
| **System.Text.Json** | High-performance JSON handling |
| **ASP.NET Cryptography** | Secure password hashing |

---

## 📁 Project Structure

```
HealthApp/
├── 📂 Common/
│   ├── Constants/          # Application constants
│   ├── Enums/              # Enumeration types
│   ├── Extensions/         # Extension methods
│   └── Helpers/            # Utility classes (CurrentUser, UserProfileHelper)
│
├── 📂 Controllers/         # Business logic controllers
│   ├── AuthController.cs
│   ├── DashboardController.cs
│   ├── GoalController.cs
│   ├── NutritionController.cs
│   ├── PTController.cs
│   └── ReportController.cs
│
├── 📂 Models/              # Entity Framework models (25+ entities)
│   ├── Users.cs                  # User accounts
│   ├── HuanLuyenVien.cs         # Personal Trainers
│   ├── KeHoachLuyenTap.cs       # Training Plans
│   ├── ThuVienBaiTap.cs         # Exercise Library
│   ├── ThuVienMonAn.cs          # Food Library
│   ├── MucTieu.cs               # Goals
│   ├── ThanhTuu.cs              # Achievements
│   └── ...
│
├── 📂 Services/            # Business services
│   ├── AuthService.cs           # Authentication
│   ├── ChatGPTService.cs        # AI recommendations
│   ├── EmailService.cs          # Email notifications
│   ├── PaymentService.cs        # MoMo/ZaloPay integration
│   ├── PTService.cs             # PT operations
│   ├── ReportService.cs         # Analytics & reports
│   └── WorkoutNotificationService.cs
│
├── 📂 Views/               # Windows Forms UI
│   ├── Admin/              # Admin dashboard & management
│   ├── Auth/               # Login, Register, Password recovery
│   ├── Dashboard/          # Main user dashboard
│   ├── Food/               # Food library browser
│   ├── KeHoachLuyenTap/    # Training plan management
│   ├── LeaderBoard/        # Rankings & achievements
│   ├── Nutrition/          # Meal planning & tracking
│   ├── PT/                 # Personal trainer features
│   ├── Reports/            # Progress reports
│   └── Settings/           # User preferences
│
├── 📂 Repositories/        # Data access layer
│   ├── Interfaces/
│   ├── UserRepository.cs
│   └── ReportRepository.cs
│
├── 📂 Data/
│   └── Configurations/     # EF Fluent API configurations
│
├── 📂 Resources/           # Static assets
│   ├── Icons/              # Application icons
│   ├── Images/             # UI images
│   ├── Fonts/              # Custom fonts
│   └── Themes/             # UI themes
│
├── 📂 Scripts/             # SQL migration scripts
├── 📂 Migrations/          # EF migrations
├── 📂 DTOs/                # Data Transfer Objects
└── 📂 ViewModels/          # MVVM view models
```

---

## 🏗 Architecture

The application follows a **layered architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│              (Windows Forms - Views)                    │
├─────────────────────────────────────────────────────────┤
│                   Controller Layer                      │
│         (Business Logic & Orchestration)                │
├─────────────────────────────────────────────────────────┤
│                    Service Layer                        │
│    (Business Services, External API Integration)        │
├─────────────────────────────────────────────────────────┤
│                  Repository Layer                       │
│               (Data Access Patterns)                    │
├─────────────────────────────────────────────────────────┤
│                     Data Layer                          │
│         (Entity Framework + SQL Server)                 │
└─────────────────────────────────────────────────────────┘
```

### Design Patterns Used
- **Repository Pattern** - Abstraction for data access
- **Dependency Injection** - Service decoupling via interfaces
- **MVC-like Structure** - Controllers, Models, Views separation
- **Service Layer Pattern** - Business logic encapsulation
- **Singleton** - CurrentUser for session management

---

## 🚀 Installation

### Prerequisites
- Windows 10/11
- .NET Framework 4.8 Runtime
- SQL Server 2019+ (or SQL Server Express)
- Visual Studio 2019/2022 (for development)

### Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/HealthApp.git
   cd HealthApp
   ```

2. **Database Setup**
   ```sql
   -- Create database
   CREATE DATABASE WF_HealthTracker;
   
   -- Run migration scripts from /Scripts folder
   ```

3. **Configure Connection String**
   
   Update `App.config`:
   ```xml
   <connectionStrings>
     <add name="WF_HealthTracker" 
          connectionString="data source=YOUR_SERVER;initial catalog=WF_HealthTracker;user id=YOUR_USER;password=YOUR_PASSWORD;..." 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Configure API Keys** (Optional)
   ```xml
   <appSettings>
     <add key="ChatGPTApiKey" value="your-openai-api-key" />
     <add key="MoMo_PartnerCode" value="your-momo-partner-code" />
     <!-- Additional payment configurations -->
   </appSettings>
   ```

5. **Build & Run**
   ```bash
   # Using Visual Studio
   Open HealthApp.sln → Build → Run
   
   # Or using MSBuild
   msbuild HealthApp.sln /p:Configuration=Release
   ```

---

## 📸 Screenshots

> *Add screenshots of your application here*

| Dashboard | Workout Tracking | Nutrition Planning |
|-----------|------------------|-------------------|
| ![Dashboard](Docs/Images/dashboard.png) | ![Workout](Docs/Images/workout.png) | ![Nutrition](Docs/Images/nutrition.png) |

| PT Booking | Leaderboard | Admin Panel |
|------------|-------------|-------------|
| ![PT](Docs/Images/pt.png) | ![Leaderboard](Docs/Images/leaderboard.png) | ![Admin](Docs/Images/admin.png) |

---

## 🗄 Database Schema

### Core Entities
- **Users** - User accounts with roles (Admin, PT, Client)
- **HuanLuyenVien** - Personal trainer profiles
- **ThuVienBaiTap** - Exercise library (500+ exercises)
- **ThuVienMonAn** - Food database with nutritional info
- **KeHoachLuyenTap** - User training plans
- **KeHoachAnUong** - Meal planning records
- **BuoiTap** - Workout session logs
- **MucTieu** - User fitness goals
- **ThanhTuu** - Achievements and badges
- **DatLichPT** - PT booking appointments
- **GiaoDich** - Payment transactions
- **ThongBao** - Notification records

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<div align="center">

### ⭐ If you found this project helpful, please give it a star!

Made with ❤️ and C#

</div>
