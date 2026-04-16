# Khắc phục: Build treo / Chương trình không chạy (VS 2022)

## 1. Output chỉ hiển thị "Build started..." rồi dừng

**Nguyên nhân thường gặp:** Build bị treo (đặc biệt khi resolve COM references như Windows Media Player) hoặc build **thất bại** nhưng bạn đang xem **Output** thay vì **Error List**.

### Cần làm:

1. **Mở Error List**  
   Menu **View → Error List**. Nếu có lỗi build (màu đỏ), sửa hết rồi build lại.

2. **Clean + Rebuild**  
   Menu **Build → Clean Solution**, sau đó **Build → Rebuild Solution**. Đợi đến khi Output hiện `Build succeeded` hoặc danh sách lỗi.

3. **Build từ Command Line (để xem build có treo hay không)**  
   Mở **Developer Command Prompt for VS 2022** (hoặc **x64 Native Tools Command Prompt**), chạy:
   ```bat
   cd /d D:\LTTQ\BTL\HealthApp
   msbuild HealthApp.sln /t:Rebuild /p:Configuration=Debug
   ```
   - Nếu build **thành công**: sẽ có dòng `Build succeeded`.
   - Nếu **treo**: thường dừng tại bước `ResolveComReference` (COM/WMP). Xem mục 4.
   - Nếu **fail**: in ra lỗi cụ thể, sửa theo đó.

4. **Lỗi / treo do COM (WMP – Windows Media Player)**  
   Project có tham chiếu **AxWMPLib**, **WMPLib**. Một số máy bị treo hoặc lỗi khi build:
   - Chạy **Visual Studio 2022 bằng quyền Administrator** (chuột phải icon VS → Run as administrator), rồi Clean + Rebuild.
   - Hoặc tạm thời **bỏ COM references** để test build:
     - Trong **Solution Explorer** → **HealthApp** → **References**, chuột phải **AxWMPLib** và **WMPLib** → **Remove**.
     - Build lại. Nếu build được thì khả năng cao lỗi do COM. Phần dùng WMP (video, v.v.) sẽ lỗi khi chạy; chỉ dùng để kiểm tra build.

5. **Đảm bảo Startup Project**  
   Trong Solution Explorer, chuột phải **HealthApp** → **Set as Startup Project**. Sau đó **Debug → Start Debugging (F5)**.

---

## 2. Build thành công nhưng "không thấy chương trình chạy"

### Có thể gặp:

- **Chỉ thấy form Đăng nhập rồi tắt:** Ứng dụng mở **LoginForm** trước. Nếu bạn **đóng form đăng nhập** (X hoặc Cancel) mà **không đăng nhập**, chương trình sẽ **thoát** – không có form chính.  
  → **Đăng nhập thành công** (OK) thì mới mở Dashboard/Admin.

- **Bật Debug (F5)** và xem **Output** chọn **Debug**: có thể có exception khi khởi động. Nếu đã thêm xử lý lỗi trong `Program.Main`, lỗi sẽ hiện **MessageBox** "Lỗi khởi động ứng dụng" kèm nội dung lỗi.

- **Chạy trực tiếp file .exe** (không qua VS):
  ```text
  D:\LTTQ\BTL\HealthApp\bin\Debug\HealthApp.exe
  ```
  Nếu double‑click mà không lên gì, có thể crash ngay khi start. Chạy từ **Command Prompt** để xem có báo lỗi không.

---

## 3. Kiểm tra SQL Server và Connection String

- Ứng dụng dùng **Entity Framework** với SQL Server. **App.config** có connection string `WF_HealthTracker`.

- Đảm bảo:
  - **SQL Server** (hoặc **SQL Server Express**) đang chạy.
  - **Server name** và **instance** trong connection string trùng với máy bạn (ví dụ `DESKTOP-XXX\SQLEXPRESS`).
  - **User / password** (nếu dùng SQL Authentication) đúng.

- Nếu sai server hoặc DB không tồn tại, có thể **lỗi khi mở form** dùng DB (ví dụ sau khi đăng nhập). Khi đó MessageBox "Lỗi khởi động" (nếu có) hoặc exception trong Debug Output sẽ gợi ý lỗi kết nối.

---

## 4. Tóm tắt nhanh

| Triệu chứng | Việc cần làm |
|------------|----------------|
| Output chỉ "Build started...", không có "succeeded" | Xem **Error List**, **Clean + Rebuild**, hoặc build bằng **msbuild** ở trên |
| Build treo khi build trong VS | Thử **chạy VS as Admin** hoặc tạm **bỏ COM ref** (WMP) để test |
| Build OK nhưng không thấy app | Đảm bảo **đăng nhập** (không đóng login form khi chưa login); chạy **F5** và xem **Output > Debug** |
| MessageBox "Lỗi khởi động" | Đọc nội dung lỗi; kiểm tra **SQL Server**, **connection string**, **App.config** |

Nếu vẫn không chạy được, gửi kèm: (1) nội dung **Error List** (toàn bộ lỗi màu đỏ), (2) vài dòng cuối **Output** khi build, (3) nội dung **MessageBox** "Lỗi khởi động" (nếu có).
