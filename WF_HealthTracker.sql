-- Tạo database cho ứng dụng theo dõi sức khỏe
CREATE DATABASE WF_HealthTracker;
GO

USE WF_HealthTracker;
GO

-- Bảng Users: Quản lý thông tin tài khoản người dùng (hỗ trợ multi-user profiles) 
CREATE TABLE Users (
    UserID VARCHAR(20) PRIMARY KEY,  -- "user_0001"
    Username NVARCHAR(50) UNIQUE NOT NULL, -- Tên đăng nhập, duy nhất, không null
    PasswordHash NVARCHAR(256) NOT NULL, -- Mật khẩu đã hash để bảo mật
	Role NVARCHAR(20) DEFAULT 'Client', -- 'Client', 'PT', 'Admin'
	CHECK (Role IN ('Client', 'PT', 'Admin')), -- Chỉ cho phép 3 roles
    Email NVARCHAR(100) UNIQUE, -- Email, duy nhất, tùy chọn
	SDT NVARCHAR(20) UNIQUE,
    HoTen NVARCHAR(100), -- Họ tên đầy đủ của người dùng
    NgaySinh DATE, -- Ngày sinh, dùng để tính tuổi hoặc gợi ý sức khỏe
	CHECK (NgaySinh < GETDATE()), -- Không thể sinh trong tương lai
    GioiTinh NVARCHAR(10), -- Giới tính (Male/Female/Other), tùy chọn
	AnhDaiDien NVARCHAR(200),
	Theme NVARCHAR(10) DEFAULT 'Light', -- Theme giao diện: 'Light' (sáng) hoặc 'Dark' (tối)
    NgonNgu NVARCHAR(10) DEFAULT 'vi', -- Ngôn ngữ: 'vi' (Tiếng Việt), 'en' (English)
	TimeZone NVARCHAR(50) DEFAULT 'SE Asia Standard Time', -- Múi giờ user, dùng cho reminder chính xác
    ResetToken NVARCHAR(256), -- Token reset mật khẩu (random string), gửi qua email khi quên MK
    ResetTokenExpiry DATETIME, -- Thời gian hết hạn token (thường 15-30 phút), tránh bị hack
    CreatedDate DATETIME DEFAULT GETDATE() -- Ngày tạo tài khoản, tự động lấy thời gian hiện tại
);
GO

CREATE TABLE HoSoBenhLi (
	BenhID VARCHAR(20) PRIMARY KEY,
	TenBenh NVARCHAR(200),
	LoaiBenh NVARCHAR(200)
);
GO

CREATE TABLE TinhTrangTongQuan  (
    BanGhiID VARCHAR(20) PRIMARY KEY, -- rec_0001
    UserID VARCHAR(20) NOT NULL, -- Liên kết với Users
    NgayGhiNhan  DATE NOT NULL, -- Ngày ghi nhận dữ liệu, không null
    CanNang FLOAT, -- Cân nặng (kg), tùy chọn
    ChieuCao FLOAT, -- Chiều cao (cm), tùy chọn
    BMI FLOAT, -- Chỉ số BMI, tính tự động qua trigger
	SoDoVong1 FLOAT,
	SoDoVong2 FLOAT,
	SoDoVong3 FLOAT,
	SoDoBapTay FLOAT,
	SoDoBapChan FLOAT,
	TheTrang NVARCHAR(100),-- Cân đối,Thừa cân,Béo phì,...
    BenhID VARCHAR(20),
	TrinhDoCaNhan NVARCHAR(200),
	NgayCapNhat DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(500), -- Ghi chú thêm đánh giá tổng quan, tùy chọn
	-- Ràng buộc toàn vẹn dữ liệu
    CONSTRAINT FK_TinhTrang_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_TinhTrang_Benh FOREIGN KEY (BenhID)
        REFERENCES HoSoBenhLi(BenhID) ON DELETE SET NULL,
    CONSTRAINT UK_TinhTrang UNIQUE (UserID, NgayGhiNhan)-- Đảm bảo mỗi user chỉ có 1 record/ngày
);
GO

-- Trigger tính tự động chỉ số BMI sau khi thêm hoặc cập nhật dữ liệu sức khỏe
CREATE TRIGGER TR_TinhBMI
ON TinhTrangTongQuan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật BMI = CanNang / (ChieuCao/100)^2, chỉ khi có đầy đủ dữ liệu hợp lệ
    UPDATE ls
    SET ls.BMI = 
        CASE 
            WHEN i.ChieuCao IS NOT NULL AND i.ChieuCao > 0 
                 AND i.CanNang IS NOT NULL 
            THEN ROUND(i.CanNang / POWER(i.ChieuCao / 100.0, 2), 2)
            ELSE NULL
        END
    FROM TinhTrangTongQuan ls
    INNER JOIN inserted i ON ls.BanGhiID = i.BanGhiID;
END;
GO

CREATE TABLE MucTieu (
    MucTieuID VARCHAR(20) PRIMARY KEY, -- goal_0001
    UserID VARCHAR(20) NOT NULL, -- Liên kết với Users
    LoaiMucTieu NVARCHAR(50) NOT NULL,
	TenMucTieu NVARCHAR(200), 
    GiaTriMucTieu FLOAT, -- Dựa vào LoaiMucTieu để thực hiện logic chỉ nhận 'Giảm/Tăng cân' ví dụ giảm cân thì x kg (x là giá trị mục tiêu)
    NgayBatDau DATE NOT NULL, 
    NgayKetThucDuKien DATE NOT NULL,
	NgayKetThucThucTe DATE,
	TrangThai NVARCHAR(20) DEFAULT N'Đang thực hiện',
    CHECK (TrangThai IN (N'Đang thực hiện', N'Hoàn thành', N'Đã hủy')),
	PTID VARCHAR(20),
    GhiChu NVARCHAR(500), -- Ghi chú về mục tiêu, tùy chọn
	NgayTao DATETIME DEFAULT GETDATE(),
	 -- Ràng buộc toàn vẹn dữ liệu
    CONSTRAINT FK_MucTieu_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID) ON DELETE CASCADE,
    -- Ràng buộc hợp lệ logic ngày tháng
    CONSTRAINT CK_MucTieu_Date CHECK (
        (NgayKetThucDuKien IS NULL OR NgayKetThucDuKien >= NgayBatDau) AND
        (NgayKetThucThucTe IS NULL OR NgayKetThucThucTe >= NgayBatDau)
    )
);
GO

CREATE TABLE KeHoachAnUong (
    KeHoachAnID VARCHAR(20) PRIMARY KEY, -- meal_0001
    MucTieuID VARCHAR(20),
    TongCalories FLOAT, -- Tổng calories/ngày theo kế hoạch
    TongProtein FLOAT, -- Gram protein/ngày
    TongCarbs FLOAT, -- Gram carbs/ngày
    TongFat FLOAT, -- Gram chất béo/ngày
	Fiber FLOAT,
    MoTa NVARCHAR(500), 
	TrangThai NVARCHAR(20) DEFAULT N'Đang hoạt động',
    CHECK (TrangThai IN (N'Đang hoạt động', N'Tạm dừng', N'Hoàn thành')),
    CONSTRAINT FK_KeHoachAnUong_MucTieu FOREIGN KEY (MucTieuID)
        REFERENCES MucTieu(MucTieuID) ON DELETE CASCADE
);
GO

CREATE TABLE ThuVienMonAn (
    MonAnID VARCHAR(20) PRIMARY KEY, -- food_0001
	imageURL  NVARCHAR(500),
    TenMonAn NVARCHAR(200) NOT NULL,
    Loai NVARCHAR(100), -- "Thịt", "Rau củ", "Trái cây", "Hải sản"
    Donvi NVARCHAR(10), -- "g"
	KhoiLuongChuan float, -- 10
    Calories FLOAT,
    Protein FLOAT,
    Carbs FLOAT,
    Fat FLOAT,
    Fiber FLOAT, -- Chất xơ
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE BuaAnChiTiet (
    BuaAnID VARCHAR(20) PRIMARY KEY, -- meal_item_0001
    KeHoachAnID VARCHAR(20) NOT NULL,
	MonAnID VARCHAR(20) NOT NULL,
    LoaiBuaAn NVARCHAR(50) NOT NULL, -- 'Sáng', 'Trưa', 'Tối', 'Phụ'
    CHECK (LoaiBuaAn IN (N'Sáng', N'Trưa', N'Tối', N'Phụ')),
	NgayAn Date,
    TenMonAn NVARCHAR(200) NOT NULL,
    Donvi NVARCHAR(10), -- "g"
	KhoiLuongChuan float, -- 10
    Calories FLOAT,
    Protein FLOAT,
    Carbs FLOAT,
    Fat FLOAT,
	Fiber FLOAT,
    GhiChu NVARCHAR(500),
	NgayCapNhat DATETIME DEFAULT GETDATE()
    CONSTRAINT FK_BuaAnChiTiet_KeHoachAn FOREIGN KEY (KeHoachAnID)
        REFERENCES KeHoachAnUong(KeHoachAnID) ON DELETE CASCADE,
	CONSTRAINT FK_BuaAnChiTiet_MonAn FOREIGN KEY (MonAnID)
        REFERENCES ThuVienMonAn(MonAnID) ON DELETE NO ACTION
);
GO

-- Bảng Thư viện bài tập
CREATE TABLE ThuVienBaiTap (
    BaiTapID VARCHAR(20) PRIMARY KEY, -- exercise_lib_0001
    TenBaiTap NVARCHAR(200) NOT NULL, 
    LoaiMucTieu NVARCHAR(50) NOT NULL,
    NhomCoChinhNhat NVARCHAR(100) NOT NULL, -- Nhóm cơ chính
    NhomCoPhu NVARCHAR(200), -- Nhóm cơ phụ (phân cách bởi dấu phẩy)
    -- VD: N'Ngực', N'Lưng', N'Vai', N'Tay trước', N'Tay sau', N'Chân trước', N'Chân sau', N'Bụng', N'Core'
    CapDo NVARCHAR(50),
    CHECK (CapDo IN ('Beginner', 'Intermediate', 'Advanced', 'All Levels')),
    DungCu NVARCHAR(200), -- Dụng cụ cần thiết (VD: 'Tạ đòn', 'Dumbbell', 'Không dụng cụ')
    MoTa NVARCHAR(1000), -- Mô tả cách thực hiện
    HuongDan NVARCHAR(2000), -- Hướng dẫn chi tiết từng bước
    LuuY NVARCHAR(1000), -- Lưu ý khi tập (tránh chấn thương)
    AnhMinhHoa NVARCHAR(500), -- URL hoặc path ảnh minh họa
    VideoHuongDan NVARCHAR(500), -- URL video hướng dẫn
    CaloriesMoiRep FLOAT, -- Ước tính calories/rep (cho strength)
    ThoiLuongDeNghi INT, -- Thời lượng đề nghị (giây) cho mỗi set
    SoRep NVARCHAR(50), -- VD: '8-12', '12-15', '20-30'
    SoSet NVARCHAR(50), -- VD: '3-4', '4-5'
    ThoiGianNghi INT, -- Giây nghỉ đề nghị giữa các set
    DoPhoBien INT DEFAULT 0, -- Độ phổ biến (số lần được sử dụng)
    NguoiTao VARCHAR(20), -- UserID người tạo (admin hoặc PT)
    NgayTao DATETIME DEFAULT GETDATE(),
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    TheLoaiBenh NVARCHAR(100), -- User mắc bệnh này sẽ không tập được bài này
    CONSTRAINT FK_ThuVienBaiTap_NguoiTao FOREIGN KEY (NguoiTao)
        REFERENCES Users(UserID)
);
GO

CREATE TABLE KeHoachLuyenTap (
    KeHoachTapID VARCHAR(20) PRIMARY KEY, -- workout_0001
	UserID VARCHAR(20) NOT NULL,
    MucTieuID VARCHAR(20),
	TongCalories FLOAT, -- Tổng calories tất cả ngày đã tập theo kế hoạch
    CapDo NVARCHAR(50), -- 'Beginner', 'Intermediate', 'Advanced'
    CHECK (CapDo IN ('Beginner', 'Intermediate', 'Advanced')),
	TrangThai NVARCHAR(20) DEFAULT N'Đang hoạt động',
    CHECK (TrangThai IN (N'Đang hoạt động', N'Tạm dừng', N'Hoàn thành')),
    MoTa NVARCHAR(500),
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    
	CONSTRAINT FK_KeHoachLuyenTap_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_KeHoachLuyenTap_MucTieu FOREIGN KEY (MucTieuID)
        REFERENCES MucTieu(MucTieuID) ON DELETE NO ACTION
);
GO

-- Bảng Buổi tập (MỚI - bảng trung gian)
CREATE TABLE BuoiTap (
    BuoiTapID VARCHAR(20) PRIMARY KEY, -- session_0001
    KeHoachTapID VARCHAR(20) NOT NULL,
    ThuNgay VARCHAR(50), -- Thời gian tập vào thứ x
	ThoiGianNgoaiLe VARCHAR(1000), -- Nếu thứ x trùng ngày với các ngày ngoại lệ này vào thực tế sẽ ko có lịch tập ví dụ như tết hay sinh nhật.
    ThoiGianBatDau DATETIME, -- thời gian bài tập bắt đầu ví dụ 7h
    ThoiGianKetThuc DATETIME, -- thời gian bài tập kết thúc ví dụ 11h
    TrangThai NVARCHAR(20) DEFAULT N'Chưa thực hiện',
    CHECK (TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện', N'Hoàn thành', N'Hủy')),
    Calories FLOAT, -- Tổng calories đốt trong buổi được tính thông qua tổng BaiTapChiTiet của buổi đó
    GhiChu NVARCHAR(500),
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Người dùng có thể sửa đổi các nội dung trong đây 
    NgayThucHien DATETIME, -- là ngày thực tế người dùng luyện tập buổi tập
    CONSTRAINT FK_BuoiTap_KeHoachTap FOREIGN KEY (KeHoachTapID)
        REFERENCES KeHoachLuyenTap(KeHoachTapID) ON DELETE CASCADE,
    CONSTRAINT CK_BuoiTap_Time CHECK (ThoiGianKetThuc IS NULL OR ThoiGianKetThuc >= ThoiGianBatDau)
);
GO

-- Bảng giao bài tập từ PT cho user đã thuê
CREATE TABLE GiaoBaiTapChoUser (
    GiaoBaiTapID VARCHAR(20) PRIMARY KEY, -- assign_0001
    PTID VARCHAR(20) NOT NULL, -- PT phụ trách
    UserID VARCHAR(20) NOT NULL, -- User nhận bài tập
    DatLichID VARCHAR(20), -- Tham chiếu lịch PT cụ thể (nếu có)
    ThuVienBaiTapID VARCHAR(20), -- Template bài tập gốc trong thư viện
    TieuDe NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(1000),
    MucTieuBuoiTap NVARCHAR(200), -- ví dụ: "Tăng sức bền"
    TrangThai NVARCHAR(20) DEFAULT 'Assigned',
        CHECK (TrangThai IN ('Assigned', 'InProgress', 'Completed', 'Overdue')),
    NgayGiao DATETIME DEFAULT GETDATE(),
    HanHoanThanh DATETIME,
    NgayHoanThanh DATETIME,
    GhiChuPT NVARCHAR(500),
    PhanHoiUser NVARCHAR(500),
    CONSTRAINT FK_GiaoBaiTap_PT FOREIGN KEY (PTID)
        REFERENCES HuanLuyenVien(PTID) ON DELETE CASCADE,
    CONSTRAINT FK_GiaoBaiTap_User FOREIGN KEY (UserID)
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_GiaoBaiTap_DatLich FOREIGN KEY (DatLichID)
        REFERENCES DatLichPT(DatLichID) ON DELETE SET NULL,
    CONSTRAINT FK_GiaoBaiTap_ThuVienBaiTap FOREIGN KEY (ThuVienBaiTapID)
        REFERENCES ThuVienBaiTap(BaiTapID) ON DELETE SET NULL,
    CONSTRAINT CK_GiaoBaiTap_HanNgay CHECK (HanHoanThanh IS NULL OR HanHoanThanh >= NgayGiao)
);
GO

-- Bảng Bài tập chi tiết (cải tiến - liên kết với BuoiTap)
CREATE TABLE BaiTapChiTiet (
    BaiTapChiTietID VARCHAR(20) PRIMARY KEY,
    BuoiTapID VARCHAR(20) NOT NULL, -- Thay đổi: liên kết với BuoiTap thay vì KeHoachTap
    BaiTapID VARCHAR(20) NOT NULL, -- Liên kết với danh mục bài tập (nếu có bảng BaiTap)
    SoSet INT,
    SoRep INT, -- Số lần lặp/set
	ThoiLuongDeNghi INT, -- Thời lượng đề nghị (giây) cho mỗi set
	ThoiGianNghi INT, -- Giây nghỉ đề nghị giữa các set
    TrongLuong FLOAT, -- Trọng lượng (kg)
    Calories FLOAT,
    ThuTuThucHien INT,
	ThoiGianBatDau DATETIME, -- Thời điểm bắt đầu bài tập
    ThoiGianKetThuc DATETIME, -- Thời điểm kết thúc bài tập
    TrangThai NVARCHAR(20) DEFAULT N'Chưa thực hiện',
    CHECK (TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện', N'Hoàn thành', N'Bỏ qua')),
    GhiChu NVARCHAR(500),
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_BaiTapChiTiet_BuoiTap FOREIGN KEY (BuoiTapID)
        REFERENCES BuoiTap(BuoiTapID) ON DELETE CASCADE,
	CONSTRAINT FK_BaiTapChiTiet_ThuVienBaiTap FOREIGN KEY (BaiTapID)
    REFERENCES ThuVienBaiTap(BaiTapID) ON DELETE NO ACTION
);
GO

-- Bảng HuanLuyenVien: Chi tiết profile PT
CREATE TABLE HuanLuyenVien (
    PTID VARCHAR(20) PRIMARY KEY, -- ptr_0001
    UserID VARCHAR(20) NOT NULL, -- Liên kết với Users (Role='PT')
    ChungChi NVARCHAR(500), -- Chứng chỉ (e.g., 'ACE Certified, NASM-CPT')
    ChuyenMon NVARCHAR(200), -- Chuyên môn (e.g., 'Giảm cân, Tăng cơ, Yoga')
    SoNamKinhNghiem INT, -- Số năm kinh nghiệm
    ThanhPho NVARCHAR(50), -- Thành phố (cho in-person sessions)
    GiaTheoGio FLOAT, -- Giá/giờ (VND)
    TieuSu NVARCHAR(1000), -- Tiểu sử, kinh nghiệm làm việc
    AnhDaiDien NVARCHAR(255), -- Đường dẫn ảnh đại diện
    AnhCCCD NVARCHAR(255), -- Đường dẫn ảnh CCCD (verification)
    AnhChanDung NVARCHAR(255), -- Đường dẫn ảnh chân dung
    FileTaiLieu NVARCHAR(255), -- Đường dẫn file tài liệu chứng chỉ
    DaXacMinh BIT DEFAULT 0, -- Xác minh chứng chỉ: 1 (đã xác minh), 0 (chưa)
    GioRanh NVARCHAR(500), -- Giờ rảnh, lưu dạng JSON: {"Mon": ["08:00-12:00"], "Wed": ["14:00-18:00"]}
    SoKhachHienTai INT DEFAULT 0, -- Số client hiện tại đang quản lý
    NhanKhach BIT DEFAULT 1, -- 1: nhận client mới, 0: full (không nhận thêm)
    TongDanhGia INT DEFAULT 0, -- Tổng số lượt đánh giá
    DiemTrungBinh FLOAT, -- Điểm đánh giá trung bình (1-5), tính từ DanhGiaPT
    TiLeThanhCong FLOAT, -- % client đạt mục tiêu (tính từ MucTieu.TrangThai='Hoàn thành')
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo profile PT
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật cuối
    -- RÀNG BUỘC
    CONSTRAINT FK_HuanLuyenVien_Users 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE, -- Xóa user → xóa profile PT
    -- ĐẢM BẢO 1 USER CHỈ CÓ 1 PROFILE PT
    CONSTRAINT UQ_HuanLuyenVien_UserID UNIQUE (UserID)
);
GO

-- Bảng DatLichPT: Quản lý đặt lịch thuê PT
CREATE TABLE DatLichPT (
    DatLichID VARCHAR(20) PRIMARY KEY, -- bkg_0001
    KhachHangID VARCHAR(20) NOT NULL, -- User đặt lịch (Role='Client')
    PTID VARCHAR(20) NULL, -- PT được chọn (có thể null nếu chưa phân công)
    NgayGioDat DATETIME NOT NULL, -- Ngày giờ tập (e.g., '2025-10-10 08:00')
    ThoiLuong INT, -- Thời lượng buổi tập (phút), e.g., 60, 90
    LoaiBuoiTap NVARCHAR(50), -- 'Online' (video call), 'In-person' (trực tiếp)
        CHECK (LoaiBuoiTap IN ('Online', 'In-person')),
    TrangThai NVARCHAR(20) DEFAULT 'Pending', 
        CHECK (TrangThai IN ('Pending', 'Confirmed', 'Completed', 'Cancelled')),
    LyDoTuChoi NVARCHAR(500), -- Lý do PT từ chối (nếu TrangThai='Cancelled')
    NguoiHuy VARCHAR(20), -- UserID của người hủy (Client hoặc PT)
    TienHoan FLOAT, -- Số tiền hoàn lại (nếu cancel trước 24h)
    ChoXemSucKhoe BIT DEFAULT 0, -- Cấp quyền PT xem TinhTrangTongQuan: 1 (cho phép), 0 (không)
    MucTieuLuyenTap NVARCHAR(200), -- Mục tiêu khách hàng đặt cho buổi tập
    GhiChu NVARCHAR(500), -- Ghi chú đặc biệt (e.g., 'Tập tại phòng gym A')
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo booking
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật cuối
    -- KHÓA NGOẠI
    CONSTRAINT FK_DatLichPT_KhachHang
        FOREIGN KEY (KhachHangID) REFERENCES Users(UserID)
        ON DELETE CASCADE, -- Xóa client → xóa booking
    CONSTRAINT FK_DatLichPT_HuanLuyenVien
        FOREIGN KEY (PTID) REFERENCES HuanLuyenVien(PTID)
        ON DELETE NO ACTION, -- Xóa PT → set PTID = NULL (giữ lịch sử)
    -- CONSTRAINT FK_DatLichPT_NguoiHuy FOREIGN KEY (NguoiHuy) REFERENCES Users(UserID) ON DELETE NO ACTION, RÀNG BUỘC BẰNG C#
    -- RÀNG BUỘC LOGIC
    CONSTRAINT CK_DatLichPT_NgayGio 
        CHECK (NgayGioDat >= GETDATE()), -- Không đặt lịch quá khứ
    CONSTRAINT CK_DatLichPT_ThoiLuong
        CHECK (ThoiLuong > 0)
);
GO

-- Bảng DanhGiaPT: Đánh giá PT sau buổi tập
CREATE TABLE DanhGiaPT (
    DanhGiaID VARCHAR(20) PRIMARY KEY, -- rev_0001
    DatLichID VARCHAR(20) NOT NULL, -- Liên kết với booking đã hoàn thành
    KhachHangID VARCHAR(20) NOT NULL, -- Người đánh giá
    PTID VARCHAR(20) NOT NULL, -- PT được đánh giá
    Diem INT NOT NULL, 
        CHECK (Diem BETWEEN 1 AND 5), -- Điểm 1-5 sao
    BinhLuan NVARCHAR(500), -- Bình luận (e.g., 'PT rất nhiệt tình')
    NgayDanhGia DATETIME DEFAULT GETDATE(), -- Ngày đánh giá
    -- RÀNG BUỘC
    CONSTRAINT FK_DanhGiaPT_DatLichPT
        FOREIGN KEY (DatLichID) REFERENCES DatLichPT(DatLichID)
        ON DELETE CASCADE, -- Xóa booking → xóa review
    CONSTRAINT FK_DanhGiaPT_KhachHang
        FOREIGN KEY (KhachHangID) REFERENCES Users(UserID)
        ON DELETE NO ACTION,
    CONSTRAINT FK_DanhGiaPT_HuanLuyenVien
        FOREIGN KEY (PTID) REFERENCES HuanLuyenVien(PTID)
        ON DELETE NO ACTION,
    -- MỖI BOOKING CHỈ ĐƯỢC ĐÁNH GIÁ 1 LẦN
    CONSTRAINT UQ_DanhGiaPT_DatLichID UNIQUE (DatLichID)
);
GO

-- Bảng GiaoDich: Quản lý thanh toán PT
CREATE TABLE GiaoDich (
    GiaoDichID VARCHAR(20) PRIMARY KEY, -- txn_0001
    DatLichID VARCHAR(20) NOT NULL, -- Liên kết booking
    KhachHangID VARCHAR(20) NOT NULL, -- Người thanh toán
    PTID VARCHAR(20) NOT NULL, -- PT nhận tiền
    SoTien FLOAT NOT NULL, -- Số tiền gốc (VND)
        CHECK (SoTien > 0),
    HoaHongApp FLOAT, -- Hoa hồng app (%), e.g., 15
        CHECK (HoaHongApp >= 0 AND HoaHongApp <= 100),
    SoTienHoaHong FLOAT, -- Số tiền hoa hồng (VND) = SoTien * HoaHongApp / 100
    SoTienPTNhan FLOAT, -- Số tiền PT nhận = SoTien - SoTienHoaHong
    TrangThaiThanhToan NVARCHAR(20) DEFAULT 'Pending', 
        CHECK (TrangThaiThanhToan IN ('Pending', 'Completed', 'Refunded')),
    PhuongThucThanhToan NVARCHAR(50), -- 'Credit Card', 'Bank Transfer', 'E-Wallet'
    MaGiaoDich NVARCHAR(100), -- Mã giao dịch từ payment gateway
    NgayGiaoDich DATETIME DEFAULT GETDATE(), -- Ngày giao dịch
    -- KHÓA NGOẠI
    CONSTRAINT FK_GiaoDich_DatLichPT
        FOREIGN KEY (DatLichID) REFERENCES DatLichPT(DatLichID)
        ON DELETE CASCADE,
    CONSTRAINT FK_GiaoDich_KhachHang
        FOREIGN KEY (KhachHangID) REFERENCES Users(UserID)
        ON DELETE NO ACTION,
    CONSTRAINT FK_GiaoDich_HuanLuyenVien
        FOREIGN KEY (PTID) REFERENCES HuanLuyenVien(PTID)
        ON DELETE NO ACTION,
    -- ĐẢM BẢO 1 BOOKING CHỈ CÓ 1 GIAO DỊCH
    CONSTRAINT UQ_GiaoDich_DatLichID UNIQUE (DatLichID)
);
GO

-- Bảng GoiThanhVien: Quản lý gói thành viên
CREATE TABLE GoiThanhVien (
    GoiThanhVienID VARCHAR(20) PRIMARY KEY, -- sub_0001
    UserID VARCHAR(20) NOT NULL, -- User nào subscribe
    LoaiGoi NVARCHAR(20) NOT NULL, -- 'Free', 'Basic', 'Premium'
        CHECK (LoaiGoi IN ('Free', 'Basic', 'Premium')),
    NgayBatDau DATE NOT NULL, -- Ngày bắt đầu subscription
    NgayKetThuc DATE, -- Ngày hết hạn (NULL nếu lifetime hoặc đang active)
    TrangThai NVARCHAR(20) DEFAULT 'Active', -- 'Active', 'Expired', 'Cancelled', 'Suspended'
        CHECK (TrangThai IN ('Active', 'Expired', 'Cancelled', 'Suspended')),
    SoTien FLOAT, -- Số tiền (VND), NULL nếu Free plan
        CHECK (SoTien IS NULL OR SoTien >= 0),
    ChuKyThanhToan NVARCHAR(20), -- 'Monthly', 'Yearly', 'Lifetime'
        CHECK (ChuKyThanhToan IN ('Monthly', 'Yearly', 'Lifetime')),
    NgayGiaHan DATE, -- Ngày gia hạn tiếp theo (auto-renewal)
    PhuongThucThanhToan NVARCHAR(50), -- 'CreditCard', 'PayPal', 'BankTransfer', 'Momo', 'ZaloPay'
    TuDongGiaHan BIT DEFAULT 1, -- 1: bật, 0: tắt
    NgayDangKy DATETIME DEFAULT GETDATE(), -- Ngày đăng ký subscription
    NgayHuy DATETIME, -- Ngày user hủy
    LyDoHuy NVARCHAR(500), -- Lý do hủy (feedback)
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật cuối
    -- RÀNG BUỘC
    CONSTRAINT FK_GoiThanhVien_User 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE,
    CONSTRAINT CK_GoiThanhVien_NgayKetThuc 
        CHECK (NgayKetThuc IS NULL OR NgayKetThuc >= NgayBatDau)
);
GO

-- Bảng TinhNangGoi: Quyền truy cập tính năng theo gói
CREATE TABLE TinhNangGoi (
    TinhNangID VARCHAR(20) PRIMARY KEY, -- feat_0001
    TenTinhNang NVARCHAR(100) UNIQUE NOT NULL, -- Tên feature (e.g., 'AI_Suggestions')
    GoiToiThieu NVARCHAR(20), -- 'Free', 'Basic', 'Premium'
        CHECK (GoiToiThieu IN ('Free', 'Basic', 'Premium')),
    MoTa NVARCHAR(500), -- Mô tả feature
    ConHoatDong BIT DEFAULT 1, -- 1: đang dùng, 0: deprecated
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo feature
    NgayCapNhat DATETIME DEFAULT GETDATE() -- Ngày cập nhật
);
GO

-- Bảng TapTin: Quản lý file upload
CREATE TABLE TapTin (
    TapTinID VARCHAR(20) PRIMARY KEY, -- file_0001
    UserID VARCHAR(20) NOT NULL, -- User nào upload
    TenTapTin NVARCHAR(255) NOT NULL, -- Tên file gốc (e.g., "anh_dai_dien.jpg")
    TenLuuTrenServer NVARCHAR(255) UNIQUE NOT NULL, -- Tên file trên server (UUID-based)
    DuongDan NVARCHAR(500) NOT NULL, -- Đường dẫn (e.g., "/uploads/users/123/")
    KichThuoc BIGINT, -- Kích thước (bytes)
        CHECK (KichThuoc > 0 AND KichThuoc <= 104857600), -- Max 100MB
    MimeType NVARCHAR(100), -- MIME type (e.g., "image/jpeg", "application/pdf")
    LoaiFile NVARCHAR(50), -- 'Image', 'PDF', 'Excel', 'Video', 'Document'
        CHECK (LoaiFile IN ('Image', 'PDF', 'Excel', 'Video', 'Document')),
    MucDich NVARCHAR(50), -- 'AnhDaiDien', 'BaoCao', 'ChungChi', 'AnhBuaAn', 'VideoTap'
    NgayUpload DATETIME DEFAULT GETDATE(), -- Ngày upload
    DaXoa BIT DEFAULT 0, -- Soft delete: 0 (active), 1 (deleted)
    NgayXoa DATETIME, -- Ngày đánh dấu deleted
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    -- RÀNG BUỘC
    CONSTRAINT FK_TapTin_Users 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE
);
GO

-- Bảng BanBe: Quản lý quan hệ bạn bè
CREATE TABLE BanBe (
    BanBeID VARCHAR(20) PRIMARY KEY, -- friend_0001
    UserID VARCHAR(20) NOT NULL, -- User gửi friend request
    NguoiNhanID VARCHAR(20) NOT NULL, -- User nhận friend request
    TrangThai NVARCHAR(20) DEFAULT 'Pending', -- 'Pending', 'Accepted', 'Blocked'
        CHECK (TrangThai IN ('Pending', 'Accepted', 'Blocked')),
    NgayGui DATETIME DEFAULT GETDATE(), -- Ngày gửi friend request
    NgayChapNhan DATETIME, -- Ngày accept
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    -- RÀNG BUỘC
    CONSTRAINT CK_BanBe_NotSelf 
        CHECK (UserID != NguoiNhanID), -- Không tự kết bạn
    CONSTRAINT UK_BanBe 
        UNIQUE (UserID, NguoiNhanID), -- Mỗi cặp chỉ 1 record
    CONSTRAINT FK_BanBe_User 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE,
    CONSTRAINT FK_BanBe_NguoiNhan 
        FOREIGN KEY (NguoiNhanID) REFERENCES Users(UserID)
        ON DELETE NO ACTION
);
GO

-- Bảng ThanhTuu: Quản lý thành tích/badges
CREATE TABLE ThanhTuu (
    ThanhTuuID VARCHAR(20) PRIMARY KEY, -- achv_0001
    UserID VARCHAR(20) NOT NULL, -- Liên kết với Users
    LoaiThanhTuu NVARCHAR(50) NOT NULL, -- 'Badge', 'Milestone', 'Streak', 'Challenge'
    TenThanhTuu NVARCHAR(100) NOT NULL, -- Tên (e.g., 'Marathon Runner', '30 Days Streak')
    Diem INT DEFAULT 0, -- Điểm thưởng
        CHECK (Diem >= 0),
    NgayDatDuoc DATETIME DEFAULT GETDATE(), -- Ngày đạt được
    MoTa NVARCHAR(500), -- Mô tả (e.g., 'Đạt 10.000 bước 3 ngày liên tiếp')
    BieuTuong NVARCHAR(200), -- Icon/emoji (e.g., '🏃', '⭐')
    CapDo INT DEFAULT 1, -- Cấp độ (1, 2, 3... cho tiered achievements)
        CHECK (CapDo > 0),
    -- RÀNG BUỘC
    CONSTRAINT FK_ThanhTuu_Users 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE
);
GO

-- Bảng ChiaSeThanhTuu: Chia sẻ thành tích với bạn bè
CREATE TABLE ChiaSeThanhTuu (
    ChiaSeID VARCHAR(20) PRIMARY KEY, -- share_0001
    ThanhTuuID VARCHAR(20) NOT NULL, -- Thành tích được chia sẻ
    NguoiChiaSe VARCHAR(20) NOT NULL, -- Người chia sẻ
    NgayChiaSe DATETIME DEFAULT GETDATE(), -- Ngày share
    DoiTuongXem NVARCHAR(20) DEFAULT 'Friends', -- 'Public', 'Friends', 'Private'
        CHECK (DoiTuongXem IN ('Public', 'Friends', 'Private')),
    ChuThich NVARCHAR(500), -- Chú thích khi share
    SoLuongThich INT DEFAULT 0, -- Số lượt like
        CHECK (SoLuongThich >= 0),
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    -- RÀNG BUỘC
    CONSTRAINT FK_ChiaSeThanhTuu_ThanhTuu 
        FOREIGN KEY (ThanhTuuID) REFERENCES ThanhTuu(ThanhTuuID)
        ON DELETE CASCADE,
    CONSTRAINT FK_ChiaSeThanhTuu_User 
        FOREIGN KEY (NguoiChiaSe) REFERENCES Users(UserID)
        ON DELETE NO ACTION
);
GO

-- Bảng LuotThichChiaSeThanhTuu: Likes cho shared achievements
CREATE TABLE LuotThichChiaSeThanhTuu (
    ThichID VARCHAR(20) PRIMARY KEY, -- like_0001
    ChiaSeID VARCHAR(20) NOT NULL, -- Thành tích được like
    UserID VARCHAR(20) NOT NULL, -- Người like
    NgayThich DATETIME DEFAULT GETDATE(), -- Ngày like
    -- RÀNG BUỘC
    CONSTRAINT UK_LuotThich 
        UNIQUE (ChiaSeID, UserID), -- Mỗi user chỉ like 1 lần
    CONSTRAINT FK_LuotThichChiaSeThanhTuu_ChiaSe 
        FOREIGN KEY (ChiaSeID) REFERENCES ChiaSeThanhTuu(ChiaSeID)
        ON DELETE CASCADE,
    CONSTRAINT FK_LuotThichChiaSeThanhTuu_User 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE NO ACTION
);
GO


-- Bảng ThongBao: Lưu lịch sử thông báo
CREATE TABLE ThongBao (
    ThongBaoID VARCHAR(20) PRIMARY KEY, -- notif_0001
    UserID VARCHAR(20) NOT NULL, -- Liên kết với Users
    NoiDung NVARCHAR(500) NOT NULL, -- Nội dung thông báo
    TieuDe NVARCHAR(200), -- Tiêu đề thông báo (e.g., 'Chúc mừng!')
    Loai NVARCHAR(50),
    MaLienQuan VARCHAR(20), -- ID liên quan (GoalID, AchievementID, BookingID)
    DaDoc BIT DEFAULT 0, -- 0: chưa đọc (highlight), 1: đã đọc
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo (sort newest first)
    -- RÀNG BUỘC
    CONSTRAINT FK_ThongBao_Users 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
        ON DELETE CASCADE
);
GO