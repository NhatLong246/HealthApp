@echo off
echo Creating HealthApp MVC project structure...

REM Tạo các thư mục theo mô hình MVC
mkdir Models 2>nul
mkdir Views 2>nul
mkdir Views\Dashboard 2>nul
mkdir Views\Activity 2>nul
mkdir Views\HealthMetrics 2>nul
mkdir Views\Sleep 2>nul
mkdir Views\Nutrition 2>nul
mkdir Views\Goals 2>nul
mkdir Views\Reports 2>nul
mkdir Views\Settings 2>nul
mkdir Views\Auth 2>nul
mkdir Views\Shared 2>nul
mkdir Controllers 2>nul
mkdir Data 2>nul
mkdir Data\Configurations 2>nul
mkdir Repositories 2>nul
mkdir Repositories\Interfaces 2>nul
mkdir Services 2>nul
mkdir Services\Interfaces 2>nul
mkdir Common 2>nul
mkdir Common\Helpers 2>nul
mkdir Common\Extensions 2>nul
mkdir Common\Constants 2>nul
mkdir Common\Enums 2>nul
mkdir ViewModels 2>nul
mkdir DTOs 2>nul
mkdir Resources 2>nul
mkdir Resources\Images 2>nul
mkdir Resources\Images\Dashboard 2>nul
mkdir Resources\Images\Activities 2>nul
mkdir Resources\Images\Health 2>nul
mkdir Resources\Icons 2>nul
mkdir Resources\Fonts 2>nul
mkdir Resources\Themes 2>nul
mkdir Assets 2>nul
mkdir Assets\Reports 2>nul
mkdir Assets\Reports\Templates 2>nul
mkdir Assets\Reports\Generated 2>nul
mkdir Migrations 2>nul
mkdir Configs 2>nul
mkdir Tests 2>nul
mkdir Tests\UnitTests 2>nul
mkdir Tests\IntegrationTests 2>nul
mkdir Docs 2>nul
mkdir Docs\Images 2>nul
mkdir Docs\Database 2>nul
mkdir Logs 2>nul
mkdir Backups 2>nul

REM Tạo file .gitkeep cho mỗi thư mục
type nul > Models\.gitkeep
type nul > Views\.gitkeep
type nul > Views\Dashboard\.gitkeep
type nul > Views\Activity\.gitkeep
type nul > Views\HealthMetrics\.gitkeep
type nul > Views\Sleep\.gitkeep
type nul > Views\Nutrition\.gitkeep
type nul > Views\Goals\.gitkeep
type nul > Views\Reports\.gitkeep
type nul > Views\Settings\.gitkeep
type nul > Views\Auth\.gitkeep
type nul > Views\Shared\.gitkeep
type nul > Controllers\.gitkeep
type nul > Data\.gitkeep
type nul > Data\Configurations\.gitkeep
type nul > Repositories\.gitkeep
type nul > Repositories\Interfaces\.gitkeep
type nul > Services\.gitkeep
type nul > Services\Interfaces\.gitkeep
type nul > Common\.gitkeep
type nul > Common\Helpers\.gitkeep
type nul > Common\Extensions\.gitkeep
type nul > Common\Constants\.gitkeep
type nul > Common\Enums\.gitkeep
type nul > ViewModels\.gitkeep
type nul > DTOs\.gitkeep
type nul > Resources\.gitkeep
type nul > Resources\Images\.gitkeep
type nul > Resources\Images\Dashboard\.gitkeep
type nul > Resources\Images\Activities\.gitkeep
type nul > Resources\Images\Health\.gitkeep
type nul > Resources\Icons\.gitkeep
type nul > Resources\Fonts\.gitkeep
type nul > Resources\Themes\.gitkeep
type nul > Assets\.gitkeep
type nul > Assets\Reports\.gitkeep
type nul > Assets\Reports\Templates\.gitkeep
type nul > Assets\Reports\Generated\.gitkeep
type nul > Migrations\.gitkeep
type nul > Configs\.gitkeep
type nul > Tests\.gitkeep
type nul > Tests\UnitTests\.gitkeep
type nul > Tests\IntegrationTests\.gitkeep
type nul > Docs\.gitkeep
type nul > Docs\Images\.gitkeep
type nul > Docs\Database\.gitkeep
type nul > Logs\.gitkeep
type nul > Backups\.gitkeep

echo.
echo ========================================
echo MVC Project Structure Created Successfully!
echo ========================================
echo.
echo Created folders following MVC pattern:
echo.
echo [MODELS] - Entity classes from database
echo   - Models/
echo.
echo [VIEWS] - Windows Forms UI (grouped by feature)
echo   - Views/Dashboard/
echo   - Views/Activity/
echo   - Views/HealthMetrics/
echo   - Views/Sleep/
echo   - Views/Nutrition/
echo   - Views/Goals/
echo   - Views/Reports/
echo   - Views/Settings/
echo   - Views/Auth/
echo   - Views/Shared/ (Common UserControls)
echo.
echo [CONTROLLERS] - Business logic controllers
echo   - Controllers/
echo.
echo [DATA] - Database context and configurations
echo   - Data/
echo   - Data/Configurations/
echo.
echo [REPOSITORIES] - Data access layer
echo   - Repositories/
echo   - Repositories/Interfaces/
echo.
echo [SERVICES] - Application services
echo   - Services/
echo   - Services/Interfaces/
echo.
echo [VIEWMODELS ^& DTOs] - Data transfer objects
echo   - ViewModels/
echo   - DTOs/
echo.
echo [COMMON] - Shared utilities
echo   - Common/Helpers/
echo   - Common/Extensions/
echo   - Common/Constants/
echo   - Common/Enums/
echo.
echo [RESOURCES] - Static resources
echo   - Resources/Images/
echo   - Resources/Icons/
echo   - Resources/Fonts/
echo   - Resources/Themes/
echo.
echo [ASSETS] - Reports and generated files
echo   - Assets/Reports/Templates/
echo   - Assets/Reports/Generated/
echo.
echo [MIGRATIONS] - EF Core migrations
echo   - Migrations/
echo.
echo [CONFIGS] - Configuration files
echo   - Configs/
echo.
echo [TESTS] - Unit and integration tests
echo   - Tests/UnitTests/
echo   - Tests/IntegrationTests/
echo.
echo [DOCS] - Documentation
echo   - Docs/
echo   - Docs/Images/
echo   - Docs/Database/
echo.
echo [LOGS] - Application logs
echo   - Logs/
echo.
echo [BACKUPS] - Database backups
echo   - Backups/
echo.
echo ========================================
pause