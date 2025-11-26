# Hướng dẫn cập nhật Database để phù hợp với Models

## Tổng quan

File `Update_Database_To_Match_Models.sql` chứa các lệnh SQL để cập nhật database `WF_HealthTracker` cho phù hợp với các Models trong dự án.

## Các thay đổi chính

### 1. Bảng `DatLichPT`
- **Thêm cột `ThoiGianBatDau`**: DATETIME NOT NULL
  - Được tính từ `NgayGioDat` cho dữ liệu cũ
  - Model yêu cầu: `DateTime ThoiGianBatDau { get; set; }` (Required)

- **Thêm cột `ThoiGianKetThuc`**: DATETIME NOT NULL
  - Được tính từ `NgayGioDat + ThoiLuong` cho dữ liệu cũ
  - Model yêu cầu: `DateTime ThoiGianKetThuc { get; set; }` (Required)

- **Thêm ràng buộc CHECK**: `CK_DatLichPT_ThoiGian`
  - Đảm bảo `ThoiGianKetThuc > ThoiGianBatDau`

- **Cập nhật ràng buộc CHECK**: `CK_DatLichPT_ThoiGianBatDau`
  - Kiểm tra `ThoiGianBatDau >= GETDATE()` (không đặt lịch quá khứ)

### 2. Bảng `Users`
- **Kiểm tra cột `SDT`**: NVARCHAR(20) NULL
  - Đảm bảo cột tồn tại (đã có trong SQL gốc)
  - Thêm UNIQUE constraint với filtered index (cho phép nhiều NULL)

### 3. Bảng `BaiTapChiTiet`
- **Kiểm tra PRIMARY KEY**: Đảm bảo `BaiTapChiTietID` là PRIMARY KEY
  - Model không có `[Key]` attribute nhưng SQL có PRIMARY KEY

### 4. Bảng `BuaAnChiTiet`
- **Kiểm tra PRIMARY KEY**: Đảm bảo `BuaAnID` là PRIMARY KEY
  - Model có `[Key]` attribute

### 5. Các ràng buộc CHECK
- **DatLichPT.TrangThai**: Kiểm tra giá trị hợp lệ
- **DatLichPT.LoaiBuoiTap**: Kiểm tra giá trị hợp lệ

## Cách sử dụng

### Bước 1: Backup Database
```sql
-- Tạo backup trước khi chạy script
BACKUP DATABASE WF_HealthTracker 
TO DISK = 'C:\Backup\WF_HealthTracker_BeforeUpdate.bak';
```

### Bước 2: Chạy Script
1. Mở SQL Server Management Studio (SSMS)
2. Kết nối đến database server
3. Mở file `Update_Database_To_Match_Models.sql`
4. Chạy toàn bộ script (F5)

### Bước 3: Cập nhật Code (QUAN TRỌNG)

Sau khi chạy script SQL, cần cập nhật code trong `Models/WF_HealthTracker.cs`:

**Tìm dòng 320-328:**
```csharp
// Tạm thời ignore SDT nếu column chưa tồn tại trong database
// Sau khi chạy script Scripts/AddSDTColumnIfMissing.sql, bỏ ignore và uncomment mapping bên dưới
modelBuilder.Entity<Users>()
    .Ignore(e => e.SDT);

// Sau khi đã thêm column SDT vào database, uncomment dòng dưới và comment dòng Ignore ở trên
// modelBuilder.Entity<Users>()
//     .Property(e => e.SDT)
//     .HasColumnName("SDT")
//     .IsUnicode(false)
//     .IsOptional();
```

**Thay đổi thành:**
```csharp
// SDT đã được thêm vào database
modelBuilder.Entity<Users>()
    .Property(e => e.SDT)
    .HasColumnName("SDT")
    .IsUnicode(false)
    .IsOptional();
```

### Bước 4: Kiểm tra

1. Build lại project để đảm bảo không có lỗi
2. Chạy ứng dụng và kiểm tra các chức năng liên quan đến:
   - Đặt lịch PT (DatLichPT)
   - Quản lý người dùng (Users.SDT)
   - Bài tập chi tiết (BaiTapChiTiet)
   - Bữa ăn chi tiết (BuaAnChiTiet)

## Lưu ý

1. **Dữ liệu cũ**: Script sẽ tự động cập nhật dữ liệu cũ:
   - `ThoiGianBatDau` = `NgayGioDat`
   - `ThoiGianKetThuc` = `NgayGioDat + ThoiLuong` (mặc định 60 phút nếu NULL)

2. **Ràng buộc**: Các ràng buộc CHECK sẽ được thêm tự động nếu chưa tồn tại

3. **UNIQUE constraint cho SDT**: Sử dụng filtered index để cho phép nhiều giá trị NULL nhưng chỉ một giá trị không NULL duy nhất

4. **Backward compatibility**: Script sử dụng `IF NOT EXISTS` để tránh lỗi nếu đã chạy trước đó

## Xử lý lỗi

### Lỗi: "Cannot insert duplicate key"
- Nguyên nhân: Có dữ liệu trùng lặp trong `SDT`
- Giải pháp: Xóa hoặc cập nhật dữ liệu trùng lặp trước khi chạy script

### Lỗi: "Cannot add NOT NULL column"
- Nguyên nhân: Có dữ liệu NULL trong bảng
- Giải pháp: Script đã xử lý tự động bằng cách cập nhật dữ liệu cũ trước khi đặt NOT NULL

### Lỗi: "Foreign key constraint"
- Nguyên nhân: Dữ liệu không thỏa mãn ràng buộc
- Giải pháp: Kiểm tra và sửa dữ liệu trước khi chạy script

## Liên hệ

Nếu gặp vấn đề, vui lòng kiểm tra:
1. Log trong SQL Server Management Studio
2. Connection string trong `App.config`
3. Entity Framework mapping trong `WF_HealthTracker.cs`

