/*
    Script: 2025_11_27_CreateGiaoBaiTapChoUser.sql
    Purpose: ensure table GiaoBaiTapChoUser exists.
*/

USE WF_HealthTracker;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.GiaoBaiTapChoUser')
      AND type = 'U'
)
BEGIN
    PRINT 'Creating table GiaoBaiTapChoUser...';

    CREATE TABLE GiaoBaiTapChoUser (
        GiaoBaiTapID VARCHAR(20) PRIMARY KEY,
        PTID VARCHAR(20) NOT NULL,
        UserID VARCHAR(20) NOT NULL,
        DatLichID VARCHAR(20),
        ThuVienBaiTapID VARCHAR(20),
        TieuDe NVARCHAR(200) NOT NULL,
        MoTa NVARCHAR(1000),
        MucTieuBuoiTap NVARCHAR(200),
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
            REFERENCES Users(UserID) ON DELETE NO ACTION,
        CONSTRAINT FK_GiaoBaiTap_DatLich FOREIGN KEY (DatLichID)
            REFERENCES DatLichPT(DatLichID) ON DELETE NO ACTION,
        CONSTRAINT FK_GiaoBaiTap_ThuVien FOREIGN KEY (ThuVienBaiTapID)
            REFERENCES ThuVienBaiTap(BaiTapID) ON DELETE NO ACTION,
        CONSTRAINT CK_GiaoBaiTap_Han CHECK (HanHoanThanh IS NULL OR HanHoanThanh >= NgayGiao)
    );
END
ELSE
BEGIN
    PRINT 'Table GiaoBaiTapChoUser already exists.';
END
GO

