# Hướng dẫn thêm cột ThoiGianBatDau và ThoiGianKetThuc vào bảng DatLichPT

## Mô tả
Script này thêm 2 cột mới vào bảng `DatLichPT` để lưu thời gian bắt đầu và thời gian kết thúc của buổi tập PT.

## Các cột được thêm:
1. **ThoiGianBatDau** (DATETIME NOT NULL) - Thời gian bắt đầu buổi tập
2. **ThoiGianKetThuc** (DATETIME NOT NULL) - Thời gian kết thúc buổi tập

## Cách chạy script:

1. Mở **SQL Server Management Studio (SSMS)** hoặc công cụ quản lý database tương tự
2. Kết nối đến database `WF_HealthTracker`
3. Mở file `Scripts/Add_ThoiGian_Columns_To_DatLichPT.sql`
4. Chạy toàn bộ script (F5 hoặc Execute)

## Lưu ý:
- Script sẽ tự động cập nhật dữ liệu cũ:
  - `ThoiGianBatDau` = `NgayGioDat` (giữ nguyên)
  - `ThoiGianKetThuc` = `NgayGioDat` + `ThoiLuong` (phút)
- Script có kiểm tra để tránh chạy lại nhiều lần (idempotent)
- Sau khi chạy script, cần rebuild project để Entity Framework nhận diện các cột mới

## Sau khi chạy script:
1. Rebuild solution trong Visual Studio
2. Kiểm tra model `DatLichPT.cs` đã có 2 properties mới:
   - `public DateTime ThoiGianBatDau { get; set; }`
   - `public DateTime ThoiGianKetThuc { get; set; }`
3. Test lại ứng dụng

