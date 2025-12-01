extern alias ef6;

using System;
using System.Linq;
using System.Threading.Tasks;
using ef6::System.Data.Entity;
using HealthApp.Models;
using HealthApp.Repositories.Interfaces;

namespace HealthApp.Repositories
{
    /// <summary>
    /// Repository implementation cho User operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly WF_HealthTracker _context;

        public UserRepository(WF_HealthTracker context)
        {
            _context = context;
        }

        public Task<Users> GetByUsernameAsync(string username)
        {
            return Task.Run(() => _context.Users
                .FirstOrDefault(u => u.Username == username));
        }

        public Task<Users> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult<Users>(null);

            string trimmedEmail = email.Trim().ToLower();
            
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Searching for email: '{trimmedEmail}'");
            
            return Task.Run(() =>
            {
                try
                {
                    // Query case-insensitive và trim
                    var users = _context.Users.ToList(); // Load all để tránh EF translation issues
                    
                    // Debug: Log tất cả users và emails
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Total users loaded: {users.Count}");
                    foreach (var u in users)
                    {
                        string emailInfo = u.Email != null ? $"'{u.Email}' (len={u.Email.Length})" : "NULL";
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User: {u.Username}, Email: {emailInfo}");
                    }
                    
                    // Filter với case-insensitive và trim
                    var user = users.FirstOrDefault(u => 
                        u.Email != null && 
                        !string.IsNullOrWhiteSpace(u.Email) &&
                        u.Email.Trim().ToLower() == trimmedEmail);
                    
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Searching for: '{trimmedEmail}'");
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Found user: {(user != null ? user.Username : "NULL")}");
                    if (user != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User Email in DB: '{user.Email}'");
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User Email trimmed/lower: '{user.Email.Trim().ToLower()}'");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] No user found matching email '{trimmedEmail}'");
                    }
                    
                    return user;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Stack: {ex.StackTrace}");
                    throw;
                }
            });
        }

        public Task<Users> GetByIdAsync(string userId)
        {
            return Task.Run(() => _context.Users
                .FirstOrDefault(u => u.UserID == userId));
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return Task.Run(() => _context.Users
                .Any(u => u.Username == username));
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult(false);

            string trimmedEmail = email.Trim();
            return Task.Run(() => _context.Users
                .Any(u => u.Email != null && u.Email.Trim() == trimmedEmail));
        }

        public Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Task.FromResult(false);
            
            string trimmedPhone = phoneNumber.Trim();
            return Task.Run(() => _context.Users
                .Any(u => u.SDT != null && u.SDT == trimmedPhone));
        }

        public async Task<Users> CreateAsync(Users user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await Task.Run(() =>
            {
                // Tạo context mới riêng để tránh conflict với các entities đang được track
                // Đảm bảo chỉ save entity Users, không bị ảnh hưởng bởi các entities khác
                using (var isolatedContext = new WF_HealthTracker())
                {
                    // TẮT AutoDetectChanges để tránh EF tự động track các entities không mong muốn
                    isolatedContext.Configuration.AutoDetectChangesEnabled = false;
                    isolatedContext.Configuration.ValidateOnSaveEnabled = true;
                    
                    // Lưu email gốc để hiển thị trong error message
                    string originalEmail = user?.Email;
                    
                    // Tạo một user mới từ dữ liệu của user hiện tại để tránh tracking issues
                    var newUser = new Users
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        PasswordHash = user.PasswordHash,
                        Role = user.Role,
                        Email = user.Email, // Sẽ được set lại bên dưới
                        SDT = user.SDT,
                        HoTen = user.HoTen,
                        NgaySinh = user.NgaySinh,
                        GioiTinh = user.GioiTinh,
                        AnhDaiDien = user.AnhDaiDien,
                        Theme = user.Theme,
                        NgonNgu = user.NgonNgu,
                        TimeZone = user.TimeZone,
                        ResetToken = user.ResetToken,
                        ResetTokenExpiry = user.ResetTokenExpiry,
                        CreatedDate = user.CreatedDate
                    };
                    
                    // ĐẢM BẢO: Không có navigation properties nào được load
                    // Set tất cả navigation properties về null hoặc empty collection
                    newUser.BanBe = null;
                    newUser.BanBe1 = null;
                    newUser.ChiaSeThanhTuu = null;
                    newUser.DanhGiaPT = null;
                    newUser.DatLichPT = null;
                    newUser.GoiThanhVien = null;
                    newUser.GiaoDich = null;
                    newUser.HuanLuyenVien = null;
                    newUser.KeHoachLuyenTap = null;
                    newUser.LuotThichChiaSeThanhTuu = null;
                    newUser.MucTieu = null;
                    newUser.TapTin = null;
                    newUser.TinhTrangTongQuan = null;
                    newUser.ThanhTuu = null;
                    newUser.ThongBao = null;
                    newUser.ThuVienBaiTap = null;
                    
                    try
                    {
                            // Kiểm tra username đã tồn tại chưa (case-sensitive theo database)
                            if (!string.IsNullOrWhiteSpace(newUser.Username))
                            {
                                bool usernameExists = isolatedContext.Users.Any(u => u.Username == newUser.Username);
                                if (usernameExists)
                                {
                                    throw new InvalidOperationException($"Tên đăng nhập '{newUser.Username}' đã được sử dụng. Vui lòng chọn tên đăng nhập khác.");
                                }
                            }

                            // XỬ LÝ EMAIL - ĐẢM BẢO KHÔNG BAO GIỜ NULL
                            // Theo schema: Email NVARCHAR(100) UNIQUE (cho phép NULL nhưng chỉ 1 NULL)
                            // Để tránh conflict, chúng ta LUÔN tạo email (kể cả placeholder)
                            
                            // Lấy email từ user object, xử lý null-safe
                            string finalEmail = newUser?.Email;
                            
                            // Nếu email null hoặc empty, tạo placeholder
                            if (string.IsNullOrWhiteSpace(finalEmail))
                            {
                                finalEmail = $"placeholder_{Guid.NewGuid():N}@noemail.healthapp";
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email was null/empty, created placeholder: '{finalEmail}'");
                            }
                            else
                            {
                                // Trim email và kiểm tra lại
                                finalEmail = finalEmail.Trim();
                                
                                // Đảm bảo email không rỗng sau khi trim
                                if (string.IsNullOrWhiteSpace(finalEmail))
                                {
                                    finalEmail = $"placeholder_{Guid.NewGuid():N}@noemail.healthapp";
                                    System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email was empty after trim, created placeholder: '{finalEmail}'");
                                }
                                else
                                {
                                    // Kiểm tra email đã tồn tại chưa (case-sensitive theo database)
                                    bool emailExists = isolatedContext.Users
                                        .Any(u => u.Email != null && u.Email == finalEmail);
                                    
                                    if (emailExists)
                                    {
                                        throw new InvalidOperationException($"Email '{originalEmail ?? finalEmail}' đã được sử dụng. Vui lòng sử dụng email khác.");
                                    }
                                }
                            }
                            
                            // Set email cuối cùng vào user object
                            newUser.Email = finalEmail;
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] Final email: '{newUser.Email}' (original: '{originalEmail ?? "NULL"}')");
                            
                            // XỬ LÝ SDT - Có thể NULL nhưng phải unique nếu không NULL
                            // Theo schema: SDT NVARCHAR(20) UNIQUE với filtered index
                            if (!string.IsNullOrWhiteSpace(newUser.SDT))
                            {
                                string trimmedSDT = newUser.SDT.Trim();
                                newUser.SDT = trimmedSDT;
                                
                                // Kiểm tra SDT đã tồn tại chưa (chỉ kiểm tra nếu không NULL)
                                bool sdtExists = isolatedContext.Users
                                    .Any(u => u.SDT != null && u.SDT == trimmedSDT);
                                
                                if (sdtExists)
                                {
                                    throw new InvalidOperationException($"Số điện thoại '{trimmedSDT}' đã được sử dụng. Vui lòng sử dụng số điện thoại khác.");
                                }
                            }
                            else
                            {
                                // Set SDT về null nếu empty
                                newUser.SDT = null;
                            }

                            // Kiểm tra UserID đã tồn tại chưa
                            if (!string.IsNullOrWhiteSpace(newUser.UserID))
                            {
                                bool userIdExists = isolatedContext.Users.Any(u => u.UserID == newUser.UserID);
                                if (userIdExists)
                                {
                                    throw new InvalidOperationException($"UserID '{newUser.UserID}' đã tồn tại trong hệ thống.");
                                }
                            }

                            // VALIDATION CUỐI CÙNG: Đảm bảo tất cả giá trị bắt buộc không null
                            if (string.IsNullOrWhiteSpace(newUser.Username))
                            {
                                throw new InvalidOperationException("Tên đăng nhập không được để trống.");
                            }
                            
                            if (string.IsNullOrWhiteSpace(newUser.UserID))
                            {
                                throw new InvalidOperationException("UserID không được để trống.");
                            }
                            
                            // Đảm bảo email không bao giờ null (quan trọng cho UNIQUE constraint)
                            if (string.IsNullOrWhiteSpace(newUser.Email))
                            {
                                newUser.Email = $"placeholder_{Guid.NewGuid():N}@noemail.healthapp";
                                finalEmail = newUser.Email;
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email was null/empty in final validation, set to: '{newUser.Email}'");
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] Before SaveChanges - Username: '{newUser.Username}', Email: '{newUser.Email}', UserID: '{newUser.UserID}'");
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email is null: {newUser.Email == null}, Email is empty: {string.IsNullOrEmpty(newUser.Email)}, Email length: {newUser.Email?.Length ?? 0}");
                            
                            // ĐẢM BẢO: Kiểm tra và xóa tất cả entities đang được track trước khi add user
                            var trackedBeforeAdd = isolatedContext.ChangeTracker.Entries().ToList();
                            if (trackedBeforeAdd.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] WARNING: Có {trackedBeforeAdd.Count} entities đang được track TRƯỚC KHI add user!");
                                foreach (var entry in trackedBeforeAdd)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[CreateAsync]   - Xóa entity: {entry.Entity.GetType().Name}, State={entry.State}");
                                    entry.State = EntityState.Detached;
                                }
                            }

                            // Thêm user vào context mới (isolated) - CHỈ ADD USER, KHÔNG ADD BẤT KỲ ENTITY NÀO KHÁC
                            isolatedContext.Users.Add(newUser);
                            
                            // Log số lượng entities đang được track SAU KHI add user
                            var trackedAfterAdd = isolatedContext.ChangeTracker.Entries().ToList();
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] Số entities đang được track SAU KHI add user: {trackedAfterAdd.Count}");
                            foreach (var trackedEntry in trackedAfterAdd)
                            {
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync]   - {trackedEntry.Entity.GetType().Name}: State={trackedEntry.State}");
                                
                                // NẾU CÓ ENTITY KHÁC NGOÀI Users, THÌ DETACH NÓ NGAY LẬP TỨC
                                if (trackedEntry.Entity.GetType() != typeof(Users))
                                {
                                    System.Diagnostics.Debug.WriteLine($"[CreateAsync] ERROR: Phát hiện entity không mong muốn: {trackedEntry.Entity.GetType().Name}! Đang detach...");
                                    trackedEntry.State = EntityState.Detached;
                                }
                            }
                            
                            // Force Entity Framework nhận diện thay đổi
                            isolatedContext.Configuration.AutoDetectChangesEnabled = true;
                            isolatedContext.ChangeTracker.DetectChanges();
                            
                            var userEntry = isolatedContext.Entry(newUser);
                            
                            // KIỂM TRA VÀ SET EMAIL LẦN CUỐI TRƯỚC KHI SAVE
                            // Đảm bảo email KHÔNG BAO GIỜ NULL hoặc empty
                            // Sử dụng lại biến finalEmail đã khai báo ở trên (dòng 191)
                            if (string.IsNullOrWhiteSpace(newUser.Email))
                            {
                                newUser.Email = $"placeholder_{Guid.NewGuid():N}@noemail.healthapp";
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email was null/empty before save, set to: '{newUser.Email}'");
                            }
                            
                            // Đảm bảo email và SDT được set đúng trong entity entry
                            if (userEntry.State == EntityState.Added)
                            {
                                // Set email
                                var emailInEntry = userEntry.Property("Email").CurrentValue as string;
                                if (string.IsNullOrWhiteSpace(emailInEntry) || emailInEntry != newUser.Email)
                                {
                                    userEntry.Property("Email").CurrentValue = newUser.Email;
                                    System.Diagnostics.Debug.WriteLine($"[CreateAsync] Set email in entry: '{userEntry.Property("Email").CurrentValue}'");
                                }
                                
                                // Set SDT (có thể null)
                                try
                                {
                                    var sdtInEntry = userEntry.Property("SDT").CurrentValue as string;
                                    if (sdtInEntry != newUser.SDT)
                                    {
                                        userEntry.Property("SDT").CurrentValue = newUser.SDT;
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] Set SDT in entry: '{newUser.SDT ?? "NULL"}'");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Nếu SDT property không tồn tại (do mapping), bỏ qua
                                    System.Diagnostics.Debug.WriteLine($"[CreateAsync] Warning: Could not set SDT in entry: {ex.Message}");
                                }
                            }
                            
                            // Kiểm tra cuối cùng trước khi save
                            var finalEmailCheck = userEntry.Property("Email").CurrentValue as string;
                            if (string.IsNullOrWhiteSpace(finalEmailCheck))
                            {
                                // Force set lại email nếu vẫn null
                                newUser.Email = $"placeholder_{Guid.NewGuid():N}@noemail.healthapp";
                                userEntry.Property("Email").CurrentValue = newUser.Email;
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] Email was still null, force set to: '{newUser.Email}'");
                            }
                            
                            // Đảm bảo object và entry đồng bộ
                            newUser.Email = finalEmailCheck ?? newUser.Email;
                            
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] FINAL CHECK - Entity state: {userEntry.State}, Email: '{newUser.Email}', SDT: '{newUser.SDT ?? "NULL"}', UserID: '{newUser.UserID}'");
                            
                            // KIỂM TRA LẦN CUỐI: Chỉ có Users entity được track
                            var finalTracked = isolatedContext.ChangeTracker.Entries().ToList();
                            if (finalTracked.Count != 1 || finalTracked[0].Entity.GetType() != typeof(Users))
                            {
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] ERROR: Có {finalTracked.Count} entities được track, không phải chỉ có Users!");
                                foreach (var entry in finalTracked)
                                {
                                    if (entry.Entity.GetType() != typeof(Users))
                                    {
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync]   - Detaching: {entry.Entity.GetType().Name}");
                                        entry.State = EntityState.Detached;
                                    }
                                }
                            }
                            
                            // Save changes trong context isolated (chỉ có entity Users)
                            isolatedContext.SaveChanges();
                            
                            // Cập nhật lại user gốc với dữ liệu đã lưu
                            user.UserID = newUser.UserID;
                            user.Username = newUser.Username;
                            user.Email = newUser.Email;
                            user.PasswordHash = newUser.PasswordHash;
                            user.CreatedDate = newUser.CreatedDate;
                            
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] Đã tạo user thành công: Username='{newUser.Username}', Email='{newUser.Email}', UserID='{newUser.UserID}'");
                            
                return user;
                        }
                        catch (ef6::System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                        {
                            // Xử lý lỗi database constraint
                            System.Diagnostics.Debug.WriteLine($"[CreateAsync] DbUpdateException: {dbEx.Message}");
                            if (dbEx.InnerException != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"[CreateAsync] InnerException: {dbEx.InnerException.Message}");
                                
                                string innerMessage = dbEx.InnerException.Message;
                                
                                // Kiểm tra lỗi UNIQUE constraint
                                if (innerMessage.Contains("UNIQUE KEY") || innerMessage.Contains("duplicate key"))
                                {
                                    // Kiểm tra duplicate key value
                                    if (innerMessage.Contains("(NULL)") || innerMessage.Contains("duplicate key value is (NULL)"))
                                    {
                                        // Lỗi do NULL trong UNIQUE constraint (thường là Email)
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] NULL duplicate key error. User.Email before insert: '{newUser.Email}'");
                                        throw new InvalidOperationException("Lỗi hệ thống: Không thể tạo email duy nhất. Vui lòng thử lại hoặc liên hệ hỗ trợ.", dbEx);
                                    }
                                    else if (innerMessage.Contains("Username") || innerMessage.Contains("UQ_Users_Username") || innerMessage.ToLower().Contains("username"))
                                    {
                                        throw new InvalidOperationException($"Tên đăng nhập '{newUser.Username}' đã được sử dụng. Vui lòng chọn tên đăng nhập khác.", dbEx);
                                    }
                                    else if (innerMessage.Contains("Email") || innerMessage.Contains("UQ_Users_Email") || innerMessage.ToLower().Contains("email"))
                                    {
                                        string emailToShow = !string.IsNullOrWhiteSpace(originalEmail) ? originalEmail : (newUser.Email ?? "N/A");
                                        throw new InvalidOperationException($"Email '{emailToShow}' đã được sử dụng. Vui lòng sử dụng email khác.", dbEx);
                                    }
                                    else if (innerMessage.Contains("BuaAnChiTiet") || innerMessage.Contains("meal_"))
                                    {
                                        // Lỗi từ BuaAnChiTiet - không nên xảy ra với context isolated
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] WARNING: BuaAnChiTiet error in isolated context! This should not happen.");
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] Inner message: {innerMessage}");
                                        throw new InvalidOperationException("Lỗi hệ thống: Có xung đột dữ liệu không mong muốn. Vui lòng thử lại.", dbEx);
                                    }
                                    else
                                    {
                                        // Log chi tiết để debug
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] UNIQUE constraint violation - Inner message: {innerMessage}");
                                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] User.Username: '{newUser.Username}', User.Email: '{newUser.Email}', User.UserID: '{newUser.UserID}'");
                                        throw new InvalidOperationException("Thông tin đăng ký đã tồn tại trong hệ thống. Vui lòng kiểm tra lại tên đăng nhập và email.", dbEx);
                                    }
                                }
                            }
                            
                            // Nếu không phải lỗi constraint, throw lại exception gốc
                            throw;
                        }
                        catch (InvalidOperationException)
                        {
                            // Re-throw InvalidOperationException (đã có message rõ ràng)
                            throw;
                        }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] Exception: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[CreateAsync] StackTrace: {ex.StackTrace}");
                        throw new Exception($"Lỗi khi tạo tài khoản: {ex.Message}", ex);
                    }
                }
            });
        }

        private static string PrepareEmailValue(string email, string identityFallback)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return email.Trim();
            }

            // SQL unique constraint không cho nhiều giá trị NULL,
            // tạo placeholder duy nhất để thỏa mãn ràng buộc.
            var safeIdentity = string.IsNullOrWhiteSpace(identityFallback)
                ? Guid.NewGuid().ToString("N")
                : identityFallback.Trim().Replace("@", "_");

            return $"placeholder_{safeIdentity}_{Guid.NewGuid():N}@noemail.healthapp";
        }

        public async Task<bool> UpdateResetTokenAsync(string email, string resetToken, DateTime? expiryTime)
        {
            return await Task.Run(() =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return false;

                user.ResetToken = resetToken;
                user.ResetTokenExpiry = expiryTime;
                _context.SaveChanges();
                return true;
            });
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPasswordHash)
        {
            return await Task.Run(() =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return false;

                user.PasswordHash = newPasswordHash;
                user.ResetToken = null; // Xóa token sau khi đổi mật khẩu thành công
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
                return true;
            });
        }
    }
}
