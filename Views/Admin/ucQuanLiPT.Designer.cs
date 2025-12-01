namespace HealthApp.Views.Admin
{
    partial class ucQuanLiPT
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // Dispose DbContext if it exists
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucQuanLiPT));
            this.pnlNen = new System.Windows.Forms.Panel();
            this.pnlDanhSachHuanLuyenVien = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.pnlThongTinPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.btnXoa = new Guna.UI2.WinForms.Guna2Button();
            this.btnXemChiTietPT = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pnlThongTinNghiepVu = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2CustomGradientPanel3 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoDoanhThuPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.lblDoanhThuPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel2 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoKhachHangDaThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.lblKhachHangDaThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoDanhGiaPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDanhGiaPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pnlThongTinCaNhanPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.lblGiaThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDiaChi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblSoDienThoai = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblGmail = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblMaPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblHovaTen = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.ptrAnhDaiDien = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.lblDanhSachHuanLuyenVien = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlChucNang = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblDiemTrungBinh = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblNhanKhach = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDiaDiem = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblChuyenMon = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTimKiem = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnDatLai = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnApDung = new Guna.UI2.WinForms.Guna2GradientButton();
            this.nudDanhGia = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.cboNhanKhach = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cboDiaChi = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cboChuyenMon = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtTiemKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.pnlTrungBinhDangCoLichThue = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoTrungBinhPTDangCoLich = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.lblBuoi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTrungBinhDangCoLichThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlTrungBinhKhachDangThuePT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoTrungBinhKhachHangDangThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.lblKhach = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTrungBinhKhachDangThuePT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlTyLeKhachChonThue = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoTyLeKhachThuePT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.lblTyLe = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTyLeKhachChonThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlDoanhThuTrungBinhPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoDoanhThuTrungBinh = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblVND = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDoanhThuTrungBinhPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlDanhGiaTrungBinh = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblSoDanhGiaTrungBinh = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.lblDanhGia = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDanhGiaTrungBinh = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlTongPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblTongSoPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTongPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlTieuDe = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.btnXacMinhDangKy = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblTieuDe = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlThongTinPT2 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2GradientButton1 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2CustomGradientPanel5 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2CustomGradientPanel6 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel7 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox16 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel4 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel8 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox17 = new System.Windows.Forms.PictureBox();
            this.guna2CustomGradientPanel9 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.pictureBox18 = new System.Windows.Forms.PictureBox();
            this.pictureBox19 = new System.Windows.Forms.PictureBox();
            this.pictureBox20 = new System.Windows.Forms.PictureBox();
            this.pictureBox21 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel7 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel8 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel9 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel10 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel11 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel12 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pnlThongTinPT3 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2GradientButton2 = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2CustomGradientPanel11 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2CustomGradientPanel12 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel13 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox22 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel14 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel13 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel15 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox23 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel16 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CustomGradientPanel14 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.guna2HtmlLabel17 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel18 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox24 = new System.Windows.Forms.PictureBox();
            this.guna2CustomGradientPanel15 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.pictureBox25 = new System.Windows.Forms.PictureBox();
            this.pictureBox26 = new System.Windows.Forms.PictureBox();
            this.pictureBox27 = new System.Windows.Forms.PictureBox();
            this.pictureBox28 = new System.Windows.Forms.PictureBox();
            this.guna2HtmlLabel19 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel20 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel21 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel22 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel23 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel24 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CirclePictureBox2 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pnlNen.SuspendLayout();
            this.pnlDanhSachHuanLuyenVien.SuspendLayout();
            this.pnlThongTinPT.SuspendLayout();
            this.pnlThongTinNghiepVu.SuspendLayout();
            this.guna2CustomGradientPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            this.guna2CustomGradientPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            this.guna2CustomGradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            this.pnlThongTinCaNhanPT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptrAnhDaiDien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.pnlChucNang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDanhGia)).BeginInit();
            this.pnlTrungBinhDangCoLichThue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.pnlTrungBinhKhachDangThuePT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.pnlTyLeKhachChonThue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.pnlDoanhThuTrungBinhPT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlDanhGiaTrungBinh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.pnlTongPT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlTieuDe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.pnlThongTinPT2.SuspendLayout();
            this.guna2CustomGradientPanel5.SuspendLayout();
            this.guna2CustomGradientPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            this.guna2CustomGradientPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).BeginInit();
            this.guna2CustomGradientPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).BeginInit();
            this.guna2CustomGradientPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.pnlThongTinPT3.SuspendLayout();
            this.guna2CustomGradientPanel11.SuspendLayout();
            this.guna2CustomGradientPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox22)).BeginInit();
            this.guna2CustomGradientPanel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox23)).BeginInit();
            this.guna2CustomGradientPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox24)).BeginInit();
            this.guna2CustomGradientPanel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlNen
            // 
            this.pnlNen.BackColor = System.Drawing.SystemColors.Control;
            this.pnlNen.Controls.Add(this.pnlDanhSachHuanLuyenVien);
            this.pnlNen.Controls.Add(this.pnlChucNang);
            this.pnlNen.Controls.Add(this.pnlTrungBinhDangCoLichThue);
            this.pnlNen.Controls.Add(this.pnlTrungBinhKhachDangThuePT);
            this.pnlNen.Controls.Add(this.pnlTyLeKhachChonThue);
            this.pnlNen.Controls.Add(this.pnlDoanhThuTrungBinhPT);
            this.pnlNen.Controls.Add(this.pnlDanhGiaTrungBinh);
            this.pnlNen.Controls.Add(this.pnlTongPT);
            this.pnlNen.Controls.Add(this.pnlTieuDe);
            this.pnlNen.Location = new System.Drawing.Point(3, 24);
            this.pnlNen.Name = "pnlNen";
            this.pnlNen.Size = new System.Drawing.Size(1050, 945);
            this.pnlNen.TabIndex = 0;
            // 
            // pnlDanhSachHuanLuyenVien
            // 
            this.pnlDanhSachHuanLuyenVien.AutoScroll = true;
            this.pnlDanhSachHuanLuyenVien.BorderRadius = 20;
            this.pnlDanhSachHuanLuyenVien.BorderThickness = 1;
            this.pnlDanhSachHuanLuyenVien.Controls.Add(this.pnlThongTinPT3);
            this.pnlDanhSachHuanLuyenVien.Controls.Add(this.pnlThongTinPT2);
            this.pnlDanhSachHuanLuyenVien.Controls.Add(this.pnlThongTinPT);
            this.pnlDanhSachHuanLuyenVien.Controls.Add(this.pictureBox7);
            this.pnlDanhSachHuanLuyenVien.Controls.Add(this.lblDanhSachHuanLuyenVien);
            this.pnlDanhSachHuanLuyenVien.Location = new System.Drawing.Point(16, 405);
            this.pnlDanhSachHuanLuyenVien.Name = "pnlDanhSachHuanLuyenVien";
            this.pnlDanhSachHuanLuyenVien.Size = new System.Drawing.Size(1024, 560);
            this.pnlDanhSachHuanLuyenVien.TabIndex = 7;
            // 
            // pnlThongTinPT
            // 
            this.pnlThongTinPT.BackColor = System.Drawing.Color.Transparent;
            this.pnlThongTinPT.BorderColor = System.Drawing.Color.Silver;
            this.pnlThongTinPT.BorderRadius = 20;
            this.pnlThongTinPT.BorderThickness = 1;
            this.pnlThongTinPT.Controls.Add(this.btnXoa);
            this.pnlThongTinPT.Controls.Add(this.btnXemChiTietPT);
            this.pnlThongTinPT.Controls.Add(this.pnlThongTinNghiepVu);
            this.pnlThongTinPT.Controls.Add(this.pnlThongTinCaNhanPT);
            this.pnlThongTinPT.Controls.Add(this.lblMaPT);
            this.pnlThongTinPT.Controls.Add(this.lblHovaTen);
            this.pnlThongTinPT.Controls.Add(this.ptrAnhDaiDien);
            this.pnlThongTinPT.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlThongTinPT.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlThongTinPT.Location = new System.Drawing.Point(15, 71);
            this.pnlThongTinPT.Name = "pnlThongTinPT";
            this.pnlThongTinPT.Size = new System.Drawing.Size(328, 461);
            this.pnlThongTinPT.TabIndex = 14;
            // 
            // btnXoa
            // 
            this.btnXoa.BorderRadius = 5;
            this.btnXoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXoa.FillColor = System.Drawing.Color.White;
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnXoa.ForeColor = System.Drawing.Color.White;
            this.btnXoa.Image = ((System.Drawing.Image)(resources.GetObject("btnXoa.Image")));
            this.btnXoa.ImageSize = new System.Drawing.Size(30, 30);
            this.btnXoa.Location = new System.Drawing.Point(258, 422);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(46, 34);
            this.btnXoa.TabIndex = 15;
            // 
            // btnXemChiTietPT
            // 
            this.btnXemChiTietPT.BorderRadius = 10;
            this.btnXemChiTietPT.BorderThickness = 1;
            this.btnXemChiTietPT.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXemChiTietPT.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXemChiTietPT.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXemChiTietPT.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXemChiTietPT.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXemChiTietPT.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXemChiTietPT.ForeColor = System.Drawing.Color.White;
            this.btnXemChiTietPT.Location = new System.Drawing.Point(134, 422);
            this.btnXemChiTietPT.Name = "btnXemChiTietPT";
            this.btnXemChiTietPT.Size = new System.Drawing.Size(118, 34);
            this.btnXemChiTietPT.TabIndex = 11;
            this.btnXemChiTietPT.Text = "Chi tiết";
            // 
            // pnlThongTinNghiepVu
            // 
            this.pnlThongTinNghiepVu.BorderRadius = 20;
            this.pnlThongTinNghiepVu.BorderThickness = 1;
            this.pnlThongTinNghiepVu.Controls.Add(this.guna2CustomGradientPanel3);
            this.pnlThongTinNghiepVu.Controls.Add(this.guna2CustomGradientPanel2);
            this.pnlThongTinNghiepVu.Controls.Add(this.guna2CustomGradientPanel1);
            this.pnlThongTinNghiepVu.FillColor = System.Drawing.Color.LightSteelBlue;
            this.pnlThongTinNghiepVu.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlThongTinNghiepVu.Location = new System.Drawing.Point(13, 215);
            this.pnlThongTinNghiepVu.Name = "pnlThongTinNghiepVu";
            this.pnlThongTinNghiepVu.Size = new System.Drawing.Size(305, 201);
            this.pnlThongTinNghiepVu.TabIndex = 13;
            // 
            // guna2CustomGradientPanel3
            // 
            this.guna2CustomGradientPanel3.BorderRadius = 10;
            this.guna2CustomGradientPanel3.BorderThickness = 1;
            this.guna2CustomGradientPanel3.Controls.Add(this.lblSoDoanhThuPT);
            this.guna2CustomGradientPanel3.Controls.Add(this.pictureBox14);
            this.guna2CustomGradientPanel3.Controls.Add(this.lblDoanhThuPT);
            this.guna2CustomGradientPanel3.Location = new System.Drawing.Point(6, 129);
            this.guna2CustomGradientPanel3.Name = "guna2CustomGradientPanel3";
            this.guna2CustomGradientPanel3.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel3.TabIndex = 2;
            // 
            // lblSoDoanhThuPT
            // 
            this.lblSoDoanhThuPT.BackColor = System.Drawing.Color.Transparent;
            this.lblSoDoanhThuPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDoanhThuPT.ForeColor = System.Drawing.Color.Black;
            this.lblSoDoanhThuPT.Location = new System.Drawing.Point(56, 27);
            this.lblSoDoanhThuPT.Name = "lblSoDoanhThuPT";
            this.lblSoDoanhThuPT.Size = new System.Drawing.Size(31, 17);
            this.lblSoDoanhThuPT.TabIndex = 15;
            this.lblSoDoanhThuPT.Text = "1.3M";
            // 
            // pictureBox14
            // 
            this.pictureBox14.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox14.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox14.Image")));
            this.pictureBox14.Location = new System.Drawing.Point(10, 9);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(40, 34);
            this.pictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox14.TabIndex = 21;
            this.pictureBox14.TabStop = false;
            // 
            // lblDoanhThuPT
            // 
            this.lblDoanhThuPT.BackColor = System.Drawing.Color.Transparent;
            this.lblDoanhThuPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoanhThuPT.ForeColor = System.Drawing.Color.Silver;
            this.lblDoanhThuPT.Location = new System.Drawing.Point(56, 9);
            this.lblDoanhThuPT.Name = "lblDoanhThuPT";
            this.lblDoanhThuPT.Size = new System.Drawing.Size(96, 17);
            this.lblDoanhThuPT.TabIndex = 16;
            this.lblDoanhThuPT.Text = "DOANH THU PT";
            // 
            // guna2CustomGradientPanel2
            // 
            this.guna2CustomGradientPanel2.BorderRadius = 10;
            this.guna2CustomGradientPanel2.BorderThickness = 1;
            this.guna2CustomGradientPanel2.Controls.Add(this.lblSoKhachHangDaThue);
            this.guna2CustomGradientPanel2.Controls.Add(this.pictureBox13);
            this.guna2CustomGradientPanel2.Controls.Add(this.lblKhachHangDaThue);
            this.guna2CustomGradientPanel2.Location = new System.Drawing.Point(6, 73);
            this.guna2CustomGradientPanel2.Name = "guna2CustomGradientPanel2";
            this.guna2CustomGradientPanel2.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel2.TabIndex = 1;
            // 
            // lblSoKhachHangDaThue
            // 
            this.lblSoKhachHangDaThue.BackColor = System.Drawing.Color.Transparent;
            this.lblSoKhachHangDaThue.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoKhachHangDaThue.ForeColor = System.Drawing.Color.Black;
            this.lblSoKhachHangDaThue.Location = new System.Drawing.Point(56, 27);
            this.lblSoKhachHangDaThue.Name = "lblSoKhachHangDaThue";
            this.lblSoKhachHangDaThue.Size = new System.Drawing.Size(10, 17);
            this.lblSoKhachHangDaThue.TabIndex = 20;
            this.lblSoKhachHangDaThue.Text = "3";
            // 
            // pictureBox13
            // 
            this.pictureBox13.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox13.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox13.Image")));
            this.pictureBox13.Location = new System.Drawing.Point(10, 8);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Size = new System.Drawing.Size(40, 34);
            this.pictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox13.TabIndex = 20;
            this.pictureBox13.TabStop = false;
            // 
            // lblKhachHangDaThue
            // 
            this.lblKhachHangDaThue.BackColor = System.Drawing.Color.Transparent;
            this.lblKhachHangDaThue.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKhachHangDaThue.ForeColor = System.Drawing.Color.Silver;
            this.lblKhachHangDaThue.Location = new System.Drawing.Point(56, 9);
            this.lblKhachHangDaThue.Name = "lblKhachHangDaThue";
            this.lblKhachHangDaThue.Size = new System.Drawing.Size(69, 17);
            this.lblKhachHangDaThue.TabIndex = 21;
            this.lblKhachHangDaThue.Text = "SỐ KHÁCH";
            // 
            // guna2CustomGradientPanel1
            // 
            this.guna2CustomGradientPanel1.BorderRadius = 10;
            this.guna2CustomGradientPanel1.BorderThickness = 1;
            this.guna2CustomGradientPanel1.Controls.Add(this.lblSoDanhGiaPT);
            this.guna2CustomGradientPanel1.Controls.Add(this.lblDanhGiaPT);
            this.guna2CustomGradientPanel1.Controls.Add(this.pictureBox12);
            this.guna2CustomGradientPanel1.Location = new System.Drawing.Point(6, 17);
            this.guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            this.guna2CustomGradientPanel1.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel1.TabIndex = 0;
            // 
            // lblSoDanhGiaPT
            // 
            this.lblSoDanhGiaPT.BackColor = System.Drawing.Color.Transparent;
            this.lblSoDanhGiaPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDanhGiaPT.ForeColor = System.Drawing.Color.Black;
            this.lblSoDanhGiaPT.Location = new System.Drawing.Point(56, 27);
            this.lblSoDanhGiaPT.Name = "lblSoDanhGiaPT";
            this.lblSoDanhGiaPT.Size = new System.Drawing.Size(20, 17);
            this.lblSoDanhGiaPT.TabIndex = 11;
            this.lblSoDanhGiaPT.Text = "4.7";
            // 
            // lblDanhGiaPT
            // 
            this.lblDanhGiaPT.BackColor = System.Drawing.Color.Transparent;
            this.lblDanhGiaPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDanhGiaPT.ForeColor = System.Drawing.Color.Silver;
            this.lblDanhGiaPT.Location = new System.Drawing.Point(56, 9);
            this.lblDanhGiaPT.Name = "lblDanhGiaPT";
            this.lblDanhGiaPT.Size = new System.Drawing.Size(83, 17);
            this.lblDanhGiaPT.TabIndex = 11;
            this.lblDanhGiaPT.Text = "ĐÁNH GIÁ PT";
            // 
            // pictureBox12
            // 
            this.pictureBox12.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox12.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox12.Image")));
            this.pictureBox12.Location = new System.Drawing.Point(10, 9);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(40, 34);
            this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox12.TabIndex = 19;
            this.pictureBox12.TabStop = false;
            // 
            // pnlThongTinCaNhanPT
            // 
            this.pnlThongTinCaNhanPT.BorderRadius = 10;
            this.pnlThongTinCaNhanPT.Controls.Add(this.pictureBox10);
            this.pnlThongTinCaNhanPT.Controls.Add(this.pictureBox11);
            this.pnlThongTinCaNhanPT.Controls.Add(this.pictureBox9);
            this.pnlThongTinCaNhanPT.Controls.Add(this.pictureBox8);
            this.pnlThongTinCaNhanPT.Controls.Add(this.lblGiaThue);
            this.pnlThongTinCaNhanPT.Controls.Add(this.lblDiaChi);
            this.pnlThongTinCaNhanPT.Controls.Add(this.lblSoDienThoai);
            this.pnlThongTinCaNhanPT.Controls.Add(this.lblGmail);
            this.pnlThongTinCaNhanPT.FillColor = System.Drawing.Color.CornflowerBlue;
            this.pnlThongTinCaNhanPT.FillColor4 = System.Drawing.Color.LightSteelBlue;
            this.pnlThongTinCaNhanPT.Location = new System.Drawing.Point(13, 93);
            this.pnlThongTinCaNhanPT.Name = "pnlThongTinCaNhanPT";
            this.pnlThongTinCaNhanPT.Size = new System.Drawing.Size(305, 115);
            this.pnlThongTinCaNhanPT.TabIndex = 12;
            // 
            // pictureBox10
            // 
            this.pictureBox10.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox10.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox10.Image")));
            this.pictureBox10.Location = new System.Drawing.Point(6, 83);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(22, 22);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox10.TabIndex = 18;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox11.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox11.Image")));
            this.pictureBox11.Location = new System.Drawing.Point(6, 59);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(22, 22);
            this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox11.TabIndex = 17;
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox9.Image")));
            this.pictureBox9.Location = new System.Drawing.Point(6, 34);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(22, 22);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox9.TabIndex = 16;
            this.pictureBox9.TabStop = false;
            this.pictureBox9.Click += new System.EventHandler(this.pictureBox9_Click);
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox8.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox8.Image")));
            this.pictureBox8.Location = new System.Drawing.Point(6, 10);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(22, 22);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox8.TabIndex = 15;
            this.pictureBox8.TabStop = false;
            // 
            // lblGiaThue
            // 
            this.lblGiaThue.BackColor = System.Drawing.Color.Transparent;
            this.lblGiaThue.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGiaThue.ForeColor = System.Drawing.Color.Gray;
            this.lblGiaThue.Location = new System.Drawing.Point(34, 86);
            this.lblGiaThue.Name = "lblGiaThue";
            this.lblGiaThue.Size = new System.Drawing.Size(60, 17);
            this.lblGiaThue.TabIndex = 14;
            this.lblGiaThue.Text = "400000/giờ";
            // 
            // lblDiaChi
            // 
            this.lblDiaChi.BackColor = System.Drawing.Color.Transparent;
            this.lblDiaChi.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiaChi.ForeColor = System.Drawing.Color.Gray;
            this.lblDiaChi.Location = new System.Drawing.Point(34, 63);
            this.lblDiaChi.Name = "lblDiaChi";
            this.lblDiaChi.Size = new System.Drawing.Size(134, 17);
            this.lblDiaChi.TabIndex = 13;
            this.lblDiaChi.Text = "Thành phố Hồ Chí Minh";
            // 
            // lblSoDienThoai
            // 
            this.lblSoDienThoai.BackColor = System.Drawing.Color.Transparent;
            this.lblSoDienThoai.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDienThoai.ForeColor = System.Drawing.Color.Gray;
            this.lblSoDienThoai.Location = new System.Drawing.Point(34, 37);
            this.lblSoDienThoai.Name = "lblSoDienThoai";
            this.lblSoDienThoai.Size = new System.Drawing.Size(63, 17);
            this.lblSoDienThoai.TabIndex = 12;
            this.lblSoDienThoai.Text = "0398764627";
            // 
            // lblGmail
            // 
            this.lblGmail.BackColor = System.Drawing.Color.Transparent;
            this.lblGmail.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGmail.ForeColor = System.Drawing.Color.Gray;
            this.lblGmail.Location = new System.Drawing.Point(34, 13);
            this.lblGmail.Name = "lblGmail";
            this.lblGmail.Size = new System.Drawing.Size(135, 17);
            this.lblGmail.TabIndex = 11;
            this.lblGmail.Text = "nhantran9q5@gmail.com";
            // 
            // lblMaPT
            // 
            this.lblMaPT.BackColor = System.Drawing.Color.Transparent;
            this.lblMaPT.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaPT.ForeColor = System.Drawing.Color.Gray;
            this.lblMaPT.Location = new System.Drawing.Point(96, 35);
            this.lblMaPT.Name = "lblMaPT";
            this.lblMaPT.Size = new System.Drawing.Size(52, 21);
            this.lblMaPT.TabIndex = 11;
            this.lblMaPT.Text = "pt_001";
            // 
            // lblHovaTen
            // 
            this.lblHovaTen.BackColor = System.Drawing.Color.Transparent;
            this.lblHovaTen.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHovaTen.ForeColor = System.Drawing.Color.Black;
            this.lblHovaTen.Location = new System.Drawing.Point(96, 12);
            this.lblHovaTen.Name = "lblHovaTen";
            this.lblHovaTen.Size = new System.Drawing.Size(125, 25);
            this.lblHovaTen.TabIndex = 11;
            this.lblHovaTen.Text = "Nguyễn Văn A";
            // 
            // ptrAnhDaiDien
            // 
            this.ptrAnhDaiDien.Image = ((System.Drawing.Image)(resources.GetObject("ptrAnhDaiDien.Image")));
            this.ptrAnhDaiDien.ImageRotate = 0F;
            this.ptrAnhDaiDien.Location = new System.Drawing.Point(13, 12);
            this.ptrAnhDaiDien.Name = "ptrAnhDaiDien";
            this.ptrAnhDaiDien.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.ptrAnhDaiDien.Size = new System.Drawing.Size(64, 64);
            this.ptrAnhDaiDien.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptrAnhDaiDien.TabIndex = 0;
            this.ptrAnhDaiDien.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(306, 13);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(35, 31);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 13;
            this.pictureBox7.TabStop = false;
            // 
            // lblDanhSachHuanLuyenVien
            // 
            this.lblDanhSachHuanLuyenVien.BackColor = System.Drawing.Color.Transparent;
            this.lblDanhSachHuanLuyenVien.Enabled = false;
            this.lblDanhSachHuanLuyenVien.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDanhSachHuanLuyenVien.ForeColor = System.Drawing.Color.Black;
            this.lblDanhSachHuanLuyenVien.Location = new System.Drawing.Point(23, 13);
            this.lblDanhSachHuanLuyenVien.Name = "lblDanhSachHuanLuyenVien";
            this.lblDanhSachHuanLuyenVien.Size = new System.Drawing.Size(277, 29);
            this.lblDanhSachHuanLuyenVien.TabIndex = 12;
            this.lblDanhSachHuanLuyenVien.Text = "Danh sách huấn luyện viên";
            // 
            // pnlChucNang
            // 
            this.pnlChucNang.BorderColor = System.Drawing.Color.Silver;
            this.pnlChucNang.BorderRadius = 15;
            this.pnlChucNang.BorderThickness = 1;
            this.pnlChucNang.Controls.Add(this.lblDiemTrungBinh);
            this.pnlChucNang.Controls.Add(this.lblNhanKhach);
            this.pnlChucNang.Controls.Add(this.lblDiaDiem);
            this.pnlChucNang.Controls.Add(this.lblChuyenMon);
            this.pnlChucNang.Controls.Add(this.lblTimKiem);
            this.pnlChucNang.Controls.Add(this.btnDatLai);
            this.pnlChucNang.Controls.Add(this.btnApDung);
            this.pnlChucNang.Controls.Add(this.nudDanhGia);
            this.pnlChucNang.Controls.Add(this.cboNhanKhach);
            this.pnlChucNang.Controls.Add(this.cboDiaChi);
            this.pnlChucNang.Controls.Add(this.cboChuyenMon);
            this.pnlChucNang.Controls.Add(this.txtTiemKiem);
            this.pnlChucNang.Location = new System.Drawing.Point(16, 259);
            this.pnlChucNang.Name = "pnlChucNang";
            this.pnlChucNang.Size = new System.Drawing.Size(1024, 114);
            this.pnlChucNang.TabIndex = 3;
            // 
            // lblDiemTrungBinh
            // 
            this.lblDiemTrungBinh.BackColor = System.Drawing.Color.Transparent;
            this.lblDiemTrungBinh.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiemTrungBinh.ForeColor = System.Drawing.Color.Black;
            this.lblDiemTrungBinh.Location = new System.Drawing.Point(906, 6);
            this.lblDiemTrungBinh.Name = "lblDiemTrungBinh";
            this.lblDiemTrungBinh.Size = new System.Drawing.Size(93, 17);
            this.lblDiemTrungBinh.TabIndex = 10;
            this.lblDiemTrungBinh.Text = "Điểm trung bình";
            // 
            // lblNhanKhach
            // 
            this.lblNhanKhach.BackColor = System.Drawing.Color.Transparent;
            this.lblNhanKhach.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNhanKhach.ForeColor = System.Drawing.Color.Black;
            this.lblNhanKhach.Location = new System.Drawing.Point(682, 6);
            this.lblNhanKhach.Name = "lblNhanKhach";
            this.lblNhanKhach.Size = new System.Drawing.Size(69, 17);
            this.lblNhanKhach.TabIndex = 9;
            this.lblNhanKhach.Text = "Nhận khách";
            // 
            // lblDiaDiem
            // 
            this.lblDiaDiem.BackColor = System.Drawing.Color.Transparent;
            this.lblDiaDiem.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiaDiem.ForeColor = System.Drawing.Color.Black;
            this.lblDiaDiem.Location = new System.Drawing.Point(459, 6);
            this.lblDiaDiem.Name = "lblDiaDiem";
            this.lblDiaDiem.Size = new System.Drawing.Size(93, 17);
            this.lblDiaDiem.TabIndex = 8;
            this.lblDiaDiem.Text = "Tỉnh/ Thành phố";
            // 
            // lblChuyenMon
            // 
            this.lblChuyenMon.BackColor = System.Drawing.Color.Transparent;
            this.lblChuyenMon.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChuyenMon.ForeColor = System.Drawing.Color.Black;
            this.lblChuyenMon.Location = new System.Drawing.Point(238, 6);
            this.lblChuyenMon.Name = "lblChuyenMon";
            this.lblChuyenMon.Size = new System.Drawing.Size(71, 17);
            this.lblChuyenMon.TabIndex = 7;
            this.lblChuyenMon.Text = "Chuyên môn";
            // 
            // lblTimKiem
            // 
            this.lblTimKiem.BackColor = System.Drawing.Color.Transparent;
            this.lblTimKiem.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimKiem.ForeColor = System.Drawing.Color.Black;
            this.lblTimKiem.Location = new System.Drawing.Point(18, 6);
            this.lblTimKiem.Name = "lblTimKiem";
            this.lblTimKiem.Size = new System.Drawing.Size(57, 17);
            this.lblTimKiem.TabIndex = 4;
            this.lblTimKiem.Text = "Tìm Kiếm";
            this.lblTimKiem.Click += new System.EventHandler(this.guna2HtmlLabel1_Click_1);
            // 
            // btnDatLai
            // 
            this.btnDatLai.BackColor = System.Drawing.Color.Transparent;
            this.btnDatLai.BorderRadius = 15;
            this.btnDatLai.BorderThickness = 1;
            this.btnDatLai.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDatLai.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDatLai.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDatLai.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDatLai.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDatLai.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDatLai.FillColor2 = System.Drawing.Color.Red;
            this.btnDatLai.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDatLai.ForeColor = System.Drawing.Color.White;
            this.btnDatLai.Location = new System.Drawing.Point(893, 72);
            this.btnDatLai.Name = "btnDatLai";
            this.btnDatLai.Size = new System.Drawing.Size(118, 34);
            this.btnDatLai.TabIndex = 6;
            this.btnDatLai.Text = "Đặt lại";
            // 
            // btnApDung
            // 
            this.btnApDung.BackColor = System.Drawing.Color.Transparent;
            this.btnApDung.BorderRadius = 15;
            this.btnApDung.BorderThickness = 1;
            this.btnApDung.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnApDung.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnApDung.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnApDung.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnApDung.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnApDung.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApDung.ForeColor = System.Drawing.Color.White;
            this.btnApDung.Location = new System.Drawing.Point(769, 72);
            this.btnApDung.Name = "btnApDung";
            this.btnApDung.Size = new System.Drawing.Size(118, 34);
            this.btnApDung.TabIndex = 5;
            this.btnApDung.Text = "Áp dụng";
            // 
            // nudDanhGia
            // 
            this.nudDanhGia.BackColor = System.Drawing.Color.Transparent;
            this.nudDanhGia.BorderRadius = 10;
            this.nudDanhGia.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nudDanhGia.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDanhGia.Location = new System.Drawing.Point(897, 26);
            this.nudDanhGia.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudDanhGia.Name = "nudDanhGia";
            this.nudDanhGia.Size = new System.Drawing.Size(114, 36);
            this.nudDanhGia.TabIndex = 4;
            // 
            // cboNhanKhach
            // 
            this.cboNhanKhach.BackColor = System.Drawing.Color.Transparent;
            this.cboNhanKhach.BorderRadius = 10;
            this.cboNhanKhach.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboNhanKhach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhanKhach.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboNhanKhach.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboNhanKhach.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboNhanKhach.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboNhanKhach.ItemHeight = 30;
            this.cboNhanKhach.Location = new System.Drawing.Point(682, 26);
            this.cboNhanKhach.Name = "cboNhanKhach";
            this.cboNhanKhach.Size = new System.Drawing.Size(205, 36);
            this.cboNhanKhach.TabIndex = 3;
            // 
            // cboDiaChi
            // 
            this.cboDiaChi.BackColor = System.Drawing.Color.Transparent;
            this.cboDiaChi.BorderRadius = 10;
            this.cboDiaChi.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDiaChi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDiaChi.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboDiaChi.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboDiaChi.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDiaChi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboDiaChi.ItemHeight = 30;
            this.cboDiaChi.Location = new System.Drawing.Point(459, 26);
            this.cboDiaChi.Name = "cboDiaChi";
            this.cboDiaChi.Size = new System.Drawing.Size(205, 36);
            this.cboDiaChi.TabIndex = 2;
            // 
            // cboChuyenMon
            // 
            this.cboChuyenMon.BackColor = System.Drawing.Color.Transparent;
            this.cboChuyenMon.BorderRadius = 10;
            this.cboChuyenMon.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboChuyenMon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChuyenMon.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboChuyenMon.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboChuyenMon.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboChuyenMon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboChuyenMon.ItemHeight = 30;
            this.cboChuyenMon.Location = new System.Drawing.Point(238, 26);
            this.cboChuyenMon.Name = "cboChuyenMon";
            this.cboChuyenMon.Size = new System.Drawing.Size(205, 36);
            this.cboChuyenMon.TabIndex = 1;
            // 
            // txtTiemKiem
            // 
            this.txtTiemKiem.BackColor = System.Drawing.Color.Transparent;
            this.txtTiemKiem.BorderRadius = 10;
            this.txtTiemKiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTiemKiem.DefaultText = "Tìm kiếm..";
            this.txtTiemKiem.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTiemKiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTiemKiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTiemKiem.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTiemKiem.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTiemKiem.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTiemKiem.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTiemKiem.Location = new System.Drawing.Point(5, 26);
            this.txtTiemKiem.Name = "txtTiemKiem";
            this.txtTiemKiem.PlaceholderText = "";
            this.txtTiemKiem.SelectedText = "";
            this.txtTiemKiem.Size = new System.Drawing.Size(216, 36);
            this.txtTiemKiem.TabIndex = 0;
            // 
            // pnlTrungBinhDangCoLichThue
            // 
            this.pnlTrungBinhDangCoLichThue.BorderColor = System.Drawing.Color.Silver;
            this.pnlTrungBinhDangCoLichThue.BorderRadius = 15;
            this.pnlTrungBinhDangCoLichThue.BorderThickness = 1;
            this.pnlTrungBinhDangCoLichThue.Controls.Add(this.lblSoTrungBinhPTDangCoLich);
            this.pnlTrungBinhDangCoLichThue.Controls.Add(this.pictureBox6);
            this.pnlTrungBinhDangCoLichThue.Controls.Add(this.lblBuoi);
            this.pnlTrungBinhDangCoLichThue.Controls.Add(this.lblTrungBinhDangCoLichThue);
            this.pnlTrungBinhDangCoLichThue.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.pnlTrungBinhDangCoLichThue.Location = new System.Drawing.Point(879, 109);
            this.pnlTrungBinhDangCoLichThue.Name = "pnlTrungBinhDangCoLichThue";
            this.pnlTrungBinhDangCoLichThue.Size = new System.Drawing.Size(160, 124);
            this.pnlTrungBinhDangCoLichThue.TabIndex = 6;
            // 
            // lblSoTrungBinhPTDangCoLich
            // 
            this.lblSoTrungBinhPTDangCoLich.BackColor = System.Drawing.Color.Transparent;
            this.lblSoTrungBinhPTDangCoLich.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoTrungBinhPTDangCoLich.ForeColor = System.Drawing.Color.Black;
            this.lblSoTrungBinhPTDangCoLich.Location = new System.Drawing.Point(62, 43);
            this.lblSoTrungBinhPTDangCoLich.Name = "lblSoTrungBinhPTDangCoLich";
            this.lblSoTrungBinhPTDangCoLich.Size = new System.Drawing.Size(16, 31);
            this.lblSoTrungBinhPTDangCoLich.TabIndex = 23;
            this.lblSoTrungBinhPTDangCoLich.Text = "x";
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(4, 37);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(52, 48);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 20;
            this.pictureBox6.TabStop = false;
            // 
            // lblBuoi
            // 
            this.lblBuoi.BackColor = System.Drawing.Color.Transparent;
            this.lblBuoi.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuoi.ForeColor = System.Drawing.Color.Silver;
            this.lblBuoi.Location = new System.Drawing.Point(62, 84);
            this.lblBuoi.Name = "lblBuoi";
            this.lblBuoi.Size = new System.Drawing.Size(29, 17);
            this.lblBuoi.TabIndex = 22;
            this.lblBuoi.Text = "Buổi";
            // 
            // lblTrungBinhDangCoLichThue
            // 
            this.lblTrungBinhDangCoLichThue.AutoSize = false;
            this.lblTrungBinhDangCoLichThue.BackColor = System.Drawing.Color.Transparent;
            this.lblTrungBinhDangCoLichThue.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrungBinhDangCoLichThue.ForeColor = System.Drawing.Color.Silver;
            this.lblTrungBinhDangCoLichThue.Location = new System.Drawing.Point(62, 10);
            this.lblTrungBinhDangCoLichThue.Name = "lblTrungBinhDangCoLichThue";
            this.lblTrungBinhDangCoLichThue.Size = new System.Drawing.Size(95, 38);
            this.lblTrungBinhDangCoLichThue.TabIndex = 21;
            this.lblTrungBinhDangCoLichThue.Text = "Trung bình đang có lịch thuê";
            // 
            // pnlTrungBinhKhachDangThuePT
            // 
            this.pnlTrungBinhKhachDangThuePT.BorderColor = System.Drawing.Color.Silver;
            this.pnlTrungBinhKhachDangThuePT.BorderRadius = 15;
            this.pnlTrungBinhKhachDangThuePT.BorderThickness = 1;
            this.pnlTrungBinhKhachDangThuePT.Controls.Add(this.lblSoTrungBinhKhachHangDangThue);
            this.pnlTrungBinhKhachDangThuePT.Controls.Add(this.pictureBox3);
            this.pnlTrungBinhKhachDangThuePT.Controls.Add(this.lblKhach);
            this.pnlTrungBinhKhachDangThuePT.Controls.Add(this.lblTrungBinhKhachDangThuePT);
            this.pnlTrungBinhKhachDangThuePT.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlTrungBinhKhachDangThuePT.Location = new System.Drawing.Point(358, 109);
            this.pnlTrungBinhKhachDangThuePT.Name = "pnlTrungBinhKhachDangThuePT";
            this.pnlTrungBinhKhachDangThuePT.Size = new System.Drawing.Size(160, 124);
            this.pnlTrungBinhKhachDangThuePT.TabIndex = 3;
            this.pnlTrungBinhKhachDangThuePT.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTrungBinhKhachDangThuePT_Paint);
            // 
            // lblSoTrungBinhKhachHangDangThue
            // 
            this.lblSoTrungBinhKhachHangDangThue.BackColor = System.Drawing.Color.Transparent;
            this.lblSoTrungBinhKhachHangDangThue.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoTrungBinhKhachHangDangThue.ForeColor = System.Drawing.Color.Black;
            this.lblSoTrungBinhKhachHangDangThue.Location = new System.Drawing.Point(61, 40);
            this.lblSoTrungBinhKhachHangDangThue.Name = "lblSoTrungBinhKhachHangDangThue";
            this.lblSoTrungBinhKhachHangDangThue.Size = new System.Drawing.Size(16, 31);
            this.lblSoTrungBinhKhachHangDangThue.TabIndex = 11;
            this.lblSoTrungBinhKhachHangDangThue.Text = "x";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(3, 37);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(52, 48);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // lblKhach
            // 
            this.lblKhach.BackColor = System.Drawing.Color.Transparent;
            this.lblKhach.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKhach.ForeColor = System.Drawing.Color.Silver;
            this.lblKhach.Location = new System.Drawing.Point(61, 81);
            this.lblKhach.Name = "lblKhach";
            this.lblKhach.Size = new System.Drawing.Size(38, 17);
            this.lblKhach.TabIndex = 10;
            this.lblKhach.Text = "Khách";
            // 
            // lblTrungBinhKhachDangThuePT
            // 
            this.lblTrungBinhKhachDangThuePT.AutoSize = false;
            this.lblTrungBinhKhachDangThuePT.BackColor = System.Drawing.Color.Transparent;
            this.lblTrungBinhKhachDangThuePT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrungBinhKhachDangThuePT.ForeColor = System.Drawing.Color.Silver;
            this.lblTrungBinhKhachDangThuePT.Location = new System.Drawing.Point(61, 10);
            this.lblTrungBinhKhachDangThuePT.Name = "lblTrungBinhKhachDangThuePT";
            this.lblTrungBinhKhachDangThuePT.Size = new System.Drawing.Size(77, 37);
            this.lblTrungBinhKhachDangThuePT.TabIndex = 9;
            this.lblTrungBinhKhachDangThuePT.Text = "Trung bình khách thuê";
            // 
            // pnlTyLeKhachChonThue
            // 
            this.pnlTyLeKhachChonThue.BorderColor = System.Drawing.Color.Silver;
            this.pnlTyLeKhachChonThue.BorderRadius = 15;
            this.pnlTyLeKhachChonThue.BorderThickness = 1;
            this.pnlTyLeKhachChonThue.Controls.Add(this.lblSoTyLeKhachThuePT);
            this.pnlTyLeKhachChonThue.Controls.Add(this.pictureBox5);
            this.pnlTyLeKhachChonThue.Controls.Add(this.lblTyLe);
            this.pnlTyLeKhachChonThue.Controls.Add(this.lblTyLeKhachChonThue);
            this.pnlTyLeKhachChonThue.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlTyLeKhachChonThue.Location = new System.Drawing.Point(708, 109);
            this.pnlTyLeKhachChonThue.Name = "pnlTyLeKhachChonThue";
            this.pnlTyLeKhachChonThue.Size = new System.Drawing.Size(160, 124);
            this.pnlTyLeKhachChonThue.TabIndex = 5;
            // 
            // lblSoTyLeKhachThuePT
            // 
            this.lblSoTyLeKhachThuePT.BackColor = System.Drawing.Color.Transparent;
            this.lblSoTyLeKhachThuePT.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoTyLeKhachThuePT.ForeColor = System.Drawing.Color.Black;
            this.lblSoTyLeKhachThuePT.Location = new System.Drawing.Point(61, 43);
            this.lblSoTyLeKhachThuePT.Name = "lblSoTyLeKhachThuePT";
            this.lblSoTyLeKhachThuePT.Size = new System.Drawing.Size(16, 31);
            this.lblSoTyLeKhachThuePT.TabIndex = 19;
            this.lblSoTyLeKhachThuePT.Text = "x";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(3, 37);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(52, 48);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 16;
            this.pictureBox5.TabStop = false;
            // 
            // lblTyLe
            // 
            this.lblTyLe.BackColor = System.Drawing.Color.Transparent;
            this.lblTyLe.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTyLe.ForeColor = System.Drawing.Color.Silver;
            this.lblTyLe.Location = new System.Drawing.Point(61, 84);
            this.lblTyLe.Name = "lblTyLe";
            this.lblTyLe.Size = new System.Drawing.Size(18, 17);
            this.lblTyLe.TabIndex = 18;
            this.lblTyLe.Text = "%";
            // 
            // lblTyLeKhachChonThue
            // 
            this.lblTyLeKhachChonThue.AutoSize = false;
            this.lblTyLeKhachChonThue.BackColor = System.Drawing.Color.Transparent;
            this.lblTyLeKhachChonThue.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTyLeKhachChonThue.ForeColor = System.Drawing.Color.Silver;
            this.lblTyLeKhachChonThue.Location = new System.Drawing.Point(61, 13);
            this.lblTyLeKhachChonThue.Name = "lblTyLeKhachChonThue";
            this.lblTyLeKhachChonThue.Size = new System.Drawing.Size(74, 35);
            this.lblTyLeKhachChonThue.TabIndex = 17;
            this.lblTyLeKhachChonThue.Text = "Tỉ lệ khách chọn thuê ";
            // 
            // pnlDoanhThuTrungBinhPT
            // 
            this.pnlDoanhThuTrungBinhPT.BorderColor = System.Drawing.Color.Silver;
            this.pnlDoanhThuTrungBinhPT.BorderRadius = 15;
            this.pnlDoanhThuTrungBinhPT.BorderThickness = 1;
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.lblSoDoanhThuTrungBinh);
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.pictureBox2);
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.lblVND);
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.lblDoanhThuTrungBinhPT);
            this.pnlDoanhThuTrungBinhPT.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pnlDoanhThuTrungBinhPT.Location = new System.Drawing.Point(184, 109);
            this.pnlDoanhThuTrungBinhPT.Name = "pnlDoanhThuTrungBinhPT";
            this.pnlDoanhThuTrungBinhPT.Size = new System.Drawing.Size(160, 124);
            this.pnlDoanhThuTrungBinhPT.TabIndex = 2;
            // 
            // lblSoDoanhThuTrungBinh
            // 
            this.lblSoDoanhThuTrungBinh.BackColor = System.Drawing.Color.Transparent;
            this.lblSoDoanhThuTrungBinh.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDoanhThuTrungBinh.ForeColor = System.Drawing.Color.Black;
            this.lblSoDoanhThuTrungBinh.Location = new System.Drawing.Point(66, 43);
            this.lblSoDoanhThuTrungBinh.Name = "lblSoDoanhThuTrungBinh";
            this.lblSoDoanhThuTrungBinh.Size = new System.Drawing.Size(16, 31);
            this.lblSoDoanhThuTrungBinh.TabIndex = 7;
            this.lblSoDoanhThuTrungBinh.Text = "x";
            this.lblSoDoanhThuTrungBinh.Click += new System.EventHandler(this.guna2HtmlLabel1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Lime;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(4, 37);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(52, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // lblVND
            // 
            this.lblVND.BackColor = System.Drawing.Color.Transparent;
            this.lblVND.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVND.ForeColor = System.Drawing.Color.Silver;
            this.lblVND.Location = new System.Drawing.Point(66, 84);
            this.lblVND.Name = "lblVND";
            this.lblVND.Size = new System.Drawing.Size(30, 17);
            this.lblVND.TabIndex = 6;
            this.lblVND.Text = "VND";
            // 
            // lblDoanhThuTrungBinhPT
            // 
            this.lblDoanhThuTrungBinhPT.AutoSize = false;
            this.lblDoanhThuTrungBinhPT.BackColor = System.Drawing.Color.Transparent;
            this.lblDoanhThuTrungBinhPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoanhThuTrungBinhPT.ForeColor = System.Drawing.Color.Silver;
            this.lblDoanhThuTrungBinhPT.Location = new System.Drawing.Point(66, 10);
            this.lblDoanhThuTrungBinhPT.Name = "lblDoanhThuTrungBinhPT";
            this.lblDoanhThuTrungBinhPT.Size = new System.Drawing.Size(94, 34);
            this.lblDoanhThuTrungBinhPT.TabIndex = 5;
            this.lblDoanhThuTrungBinhPT.Text = "Doanh thu trung bình PT/Tháng";
            // 
            // pnlDanhGiaTrungBinh
            // 
            this.pnlDanhGiaTrungBinh.BorderColor = System.Drawing.Color.Silver;
            this.pnlDanhGiaTrungBinh.BorderRadius = 15;
            this.pnlDanhGiaTrungBinh.BorderThickness = 1;
            this.pnlDanhGiaTrungBinh.Controls.Add(this.lblSoDanhGiaTrungBinh);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.pictureBox4);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.lblDanhGia);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.lblDanhGiaTrungBinh);
            this.pnlDanhGiaTrungBinh.FillColor4 = System.Drawing.Color.Yellow;
            this.pnlDanhGiaTrungBinh.Location = new System.Drawing.Point(532, 109);
            this.pnlDanhGiaTrungBinh.Name = "pnlDanhGiaTrungBinh";
            this.pnlDanhGiaTrungBinh.Size = new System.Drawing.Size(160, 124);
            this.pnlDanhGiaTrungBinh.TabIndex = 4;
            // 
            // lblSoDanhGiaTrungBinh
            // 
            this.lblSoDanhGiaTrungBinh.BackColor = System.Drawing.Color.Transparent;
            this.lblSoDanhGiaTrungBinh.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDanhGiaTrungBinh.ForeColor = System.Drawing.Color.Black;
            this.lblSoDanhGiaTrungBinh.Location = new System.Drawing.Point(62, 43);
            this.lblSoDanhGiaTrungBinh.Name = "lblSoDanhGiaTrungBinh";
            this.lblSoDanhGiaTrungBinh.Size = new System.Drawing.Size(16, 31);
            this.lblSoDanhGiaTrungBinh.TabIndex = 15;
            this.lblSoDanhGiaTrungBinh.Text = "x";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(4, 37);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(52, 48);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 12;
            this.pictureBox4.TabStop = false;
            // 
            // lblDanhGia
            // 
            this.lblDanhGia.BackColor = System.Drawing.Color.Transparent;
            this.lblDanhGia.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDanhGia.ForeColor = System.Drawing.Color.Silver;
            this.lblDanhGia.Location = new System.Drawing.Point(62, 84);
            this.lblDanhGia.Name = "lblDanhGia";
            this.lblDanhGia.Size = new System.Drawing.Size(24, 17);
            this.lblDanhGia.TabIndex = 14;
            this.lblDanhGia.Text = "/5.0";
            // 
            // lblDanhGiaTrungBinh
            // 
            this.lblDanhGiaTrungBinh.AutoSize = false;
            this.lblDanhGiaTrungBinh.BackColor = System.Drawing.Color.Transparent;
            this.lblDanhGiaTrungBinh.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDanhGiaTrungBinh.ForeColor = System.Drawing.Color.Silver;
            this.lblDanhGiaTrungBinh.Location = new System.Drawing.Point(62, 13);
            this.lblDanhGiaTrungBinh.Name = "lblDanhGiaTrungBinh";
            this.lblDanhGiaTrungBinh.Size = new System.Drawing.Size(78, 35);
            this.lblDanhGiaTrungBinh.TabIndex = 13;
            this.lblDanhGiaTrungBinh.Text = "Đánh giá trung bình";
            // 
            // pnlTongPT
            // 
            this.pnlTongPT.BorderColor = System.Drawing.Color.Silver;
            this.pnlTongPT.BorderRadius = 15;
            this.pnlTongPT.BorderThickness = 1;
            this.pnlTongPT.Controls.Add(this.lblTongSoPT);
            this.pnlTongPT.Controls.Add(this.lblPT);
            this.pnlTongPT.Controls.Add(this.lblTongPT);
            this.pnlTongPT.Controls.Add(this.pictureBox1);
            this.pnlTongPT.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlTongPT.Location = new System.Drawing.Point(12, 109);
            this.pnlTongPT.Name = "pnlTongPT";
            this.pnlTongPT.Size = new System.Drawing.Size(160, 124);
            this.pnlTongPT.TabIndex = 1;
            // 
            // lblTongSoPT
            // 
            this.lblTongSoPT.BackColor = System.Drawing.Color.Transparent;
            this.lblTongSoPT.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongSoPT.ForeColor = System.Drawing.Color.Black;
            this.lblTongSoPT.Location = new System.Drawing.Point(66, 43);
            this.lblTongSoPT.Name = "lblTongSoPT";
            this.lblTongSoPT.Size = new System.Drawing.Size(16, 31);
            this.lblTongSoPT.TabIndex = 3;
            this.lblTongSoPT.Text = "x";
            // 
            // lblPT
            // 
            this.lblPT.BackColor = System.Drawing.Color.Transparent;
            this.lblPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPT.ForeColor = System.Drawing.Color.Silver;
            this.lblPT.Location = new System.Drawing.Point(66, 84);
            this.lblPT.Name = "lblPT";
            this.lblPT.Size = new System.Drawing.Size(19, 17);
            this.lblPT.TabIndex = 2;
            this.lblPT.Text = "PT";
            // 
            // lblTongPT
            // 
            this.lblTongPT.BackColor = System.Drawing.Color.Transparent;
            this.lblTongPT.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongPT.ForeColor = System.Drawing.Color.Silver;
            this.lblTongPT.Location = new System.Drawing.Point(66, 10);
            this.lblTongPT.Name = "lblTongPT";
            this.lblTongPT.Size = new System.Drawing.Size(50, 17);
            this.lblTongPT.TabIndex = 1;
            this.lblTongPT.Text = "Tổng PT";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(52, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pnlTieuDe
            // 
            this.pnlTieuDe.BorderColor = System.Drawing.Color.Silver;
            this.pnlTieuDe.BorderThickness = 1;
            this.pnlTieuDe.Controls.Add(this.btnXacMinhDangKy);
            this.pnlTieuDe.Controls.Add(this.guna2PictureBox1);
            this.pnlTieuDe.Controls.Add(this.lblTieuDe);
            this.pnlTieuDe.Location = new System.Drawing.Point(0, -21);
            this.pnlTieuDe.Name = "pnlTieuDe";
            this.pnlTieuDe.Size = new System.Drawing.Size(1053, 111);
            this.pnlTieuDe.TabIndex = 0;
            // 
            // btnXacMinhDangKy
            // 
            this.btnXacMinhDangKy.BackColor = System.Drawing.Color.Transparent;
            this.btnXacMinhDangKy.BorderRadius = 10;
            this.btnXacMinhDangKy.BorderThickness = 1;
            this.btnXacMinhDangKy.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXacMinhDangKy.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXacMinhDangKy.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXacMinhDangKy.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXacMinhDangKy.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXacMinhDangKy.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXacMinhDangKy.ForeColor = System.Drawing.Color.White;
            this.btnXacMinhDangKy.Location = new System.Drawing.Point(839, 28);
            this.btnXacMinhDangKy.Name = "btnXacMinhDangKy";
            this.btnXacMinhDangKy.Size = new System.Drawing.Size(181, 34);
            this.btnXacMinhDangKy.TabIndex = 11;
            this.btnXacMinhDangKy.Text = "Xác minh đăng ký";
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("guna2PictureBox1.Image")));
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(159, 26);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(40, 36);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox1.TabIndex = 2;
            this.guna2PictureBox1.TabStop = false;
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.BackColor = System.Drawing.Color.Transparent;
            this.lblTieuDe.Enabled = false;
            this.lblTieuDe.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.ForeColor = System.Drawing.Color.Black;
            this.lblTieuDe.Location = new System.Drawing.Point(16, 27);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(137, 35);
            this.lblTieuDe.TabIndex = 1;
            this.lblTieuDe.Text = "Quản lý PT";
            // 
            // pnlThongTinPT2
            // 
            this.pnlThongTinPT2.BackColor = System.Drawing.Color.Transparent;
            this.pnlThongTinPT2.BorderColor = System.Drawing.Color.Silver;
            this.pnlThongTinPT2.BorderRadius = 20;
            this.pnlThongTinPT2.BorderThickness = 1;
            this.pnlThongTinPT2.Controls.Add(this.guna2Button1);
            this.pnlThongTinPT2.Controls.Add(this.guna2GradientButton1);
            this.pnlThongTinPT2.Controls.Add(this.guna2CustomGradientPanel5);
            this.pnlThongTinPT2.Controls.Add(this.guna2CustomGradientPanel9);
            this.pnlThongTinPT2.Controls.Add(this.guna2HtmlLabel11);
            this.pnlThongTinPT2.Controls.Add(this.guna2HtmlLabel12);
            this.pnlThongTinPT2.Controls.Add(this.guna2CirclePictureBox1);
            this.pnlThongTinPT2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlThongTinPT2.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlThongTinPT2.Location = new System.Drawing.Point(348, 71);
            this.pnlThongTinPT2.Name = "pnlThongTinPT2";
            this.pnlThongTinPT2.Size = new System.Drawing.Size(328, 461);
            this.pnlThongTinPT2.TabIndex = 16;
            // 
            // guna2Button1
            // 
            this.guna2Button1.BorderRadius = 5;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.White;
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button1.Image")));
            this.guna2Button1.ImageSize = new System.Drawing.Size(30, 30);
            this.guna2Button1.Location = new System.Drawing.Point(258, 422);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(46, 34);
            this.guna2Button1.TabIndex = 15;
            // 
            // guna2GradientButton1
            // 
            this.guna2GradientButton1.BorderRadius = 10;
            this.guna2GradientButton1.BorderThickness = 1;
            this.guna2GradientButton1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2GradientButton1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2GradientButton1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2GradientButton1.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2GradientButton1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2GradientButton1.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2GradientButton1.ForeColor = System.Drawing.Color.White;
            this.guna2GradientButton1.Location = new System.Drawing.Point(134, 422);
            this.guna2GradientButton1.Name = "guna2GradientButton1";
            this.guna2GradientButton1.Size = new System.Drawing.Size(118, 34);
            this.guna2GradientButton1.TabIndex = 11;
            this.guna2GradientButton1.Text = "Chi tiết";
            // 
            // guna2CustomGradientPanel5
            // 
            this.guna2CustomGradientPanel5.BorderRadius = 20;
            this.guna2CustomGradientPanel5.BorderThickness = 1;
            this.guna2CustomGradientPanel5.Controls.Add(this.guna2CustomGradientPanel6);
            this.guna2CustomGradientPanel5.Controls.Add(this.guna2CustomGradientPanel7);
            this.guna2CustomGradientPanel5.Controls.Add(this.guna2CustomGradientPanel8);
            this.guna2CustomGradientPanel5.FillColor = System.Drawing.Color.LightSteelBlue;
            this.guna2CustomGradientPanel5.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.guna2CustomGradientPanel5.Location = new System.Drawing.Point(13, 215);
            this.guna2CustomGradientPanel5.Name = "guna2CustomGradientPanel5";
            this.guna2CustomGradientPanel5.Size = new System.Drawing.Size(305, 201);
            this.guna2CustomGradientPanel5.TabIndex = 13;
            // 
            // guna2CustomGradientPanel6
            // 
            this.guna2CustomGradientPanel6.BorderRadius = 10;
            this.guna2CustomGradientPanel6.BorderThickness = 1;
            this.guna2CustomGradientPanel6.Controls.Add(this.guna2HtmlLabel1);
            this.guna2CustomGradientPanel6.Controls.Add(this.pictureBox15);
            this.guna2CustomGradientPanel6.Controls.Add(this.guna2HtmlLabel2);
            this.guna2CustomGradientPanel6.Location = new System.Drawing.Point(6, 129);
            this.guna2CustomGradientPanel6.Name = "guna2CustomGradientPanel6";
            this.guna2CustomGradientPanel6.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel6.TabIndex = 2;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(31, 17);
            this.guna2HtmlLabel1.TabIndex = 15;
            this.guna2HtmlLabel1.Text = "1.3M";
            // 
            // pictureBox15
            // 
            this.pictureBox15.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox15.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox15.Image")));
            this.pictureBox15.Location = new System.Drawing.Point(10, 9);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(40, 34);
            this.pictureBox15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox15.TabIndex = 21;
            this.pictureBox15.TabStop = false;
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(96, 17);
            this.guna2HtmlLabel2.TabIndex = 16;
            this.guna2HtmlLabel2.Text = "DOANH THU PT";
            // 
            // guna2CustomGradientPanel7
            // 
            this.guna2CustomGradientPanel7.BorderRadius = 10;
            this.guna2CustomGradientPanel7.BorderThickness = 1;
            this.guna2CustomGradientPanel7.Controls.Add(this.guna2HtmlLabel3);
            this.guna2CustomGradientPanel7.Controls.Add(this.pictureBox16);
            this.guna2CustomGradientPanel7.Controls.Add(this.guna2HtmlLabel4);
            this.guna2CustomGradientPanel7.Location = new System.Drawing.Point(6, 73);
            this.guna2CustomGradientPanel7.Name = "guna2CustomGradientPanel7";
            this.guna2CustomGradientPanel7.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel7.TabIndex = 1;
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(10, 17);
            this.guna2HtmlLabel3.TabIndex = 20;
            this.guna2HtmlLabel3.Text = "3";
            // 
            // pictureBox16
            // 
            this.pictureBox16.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox16.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox16.Image")));
            this.pictureBox16.Location = new System.Drawing.Point(10, 8);
            this.pictureBox16.Name = "pictureBox16";
            this.pictureBox16.Size = new System.Drawing.Size(40, 34);
            this.pictureBox16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox16.TabIndex = 20;
            this.pictureBox16.TabStop = false;
            // 
            // guna2HtmlLabel4
            // 
            this.guna2HtmlLabel4.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel4.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel4.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel4.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel4.Name = "guna2HtmlLabel4";
            this.guna2HtmlLabel4.Size = new System.Drawing.Size(69, 17);
            this.guna2HtmlLabel4.TabIndex = 21;
            this.guna2HtmlLabel4.Text = "SỐ KHÁCH";
            // 
            // guna2CustomGradientPanel8
            // 
            this.guna2CustomGradientPanel8.BorderRadius = 10;
            this.guna2CustomGradientPanel8.BorderThickness = 1;
            this.guna2CustomGradientPanel8.Controls.Add(this.guna2HtmlLabel5);
            this.guna2CustomGradientPanel8.Controls.Add(this.guna2HtmlLabel6);
            this.guna2CustomGradientPanel8.Controls.Add(this.pictureBox17);
            this.guna2CustomGradientPanel8.Location = new System.Drawing.Point(6, 17);
            this.guna2CustomGradientPanel8.Name = "guna2CustomGradientPanel8";
            this.guna2CustomGradientPanel8.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel8.TabIndex = 0;
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel5.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(20, 17);
            this.guna2HtmlLabel5.TabIndex = 11;
            this.guna2HtmlLabel5.Text = "4.7";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(83, 17);
            this.guna2HtmlLabel6.TabIndex = 11;
            this.guna2HtmlLabel6.Text = "ĐÁNH GIÁ PT";
            // 
            // pictureBox17
            // 
            this.pictureBox17.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox17.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox17.Image")));
            this.pictureBox17.Location = new System.Drawing.Point(10, 9);
            this.pictureBox17.Name = "pictureBox17";
            this.pictureBox17.Size = new System.Drawing.Size(40, 34);
            this.pictureBox17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox17.TabIndex = 19;
            this.pictureBox17.TabStop = false;
            // 
            // guna2CustomGradientPanel9
            // 
            this.guna2CustomGradientPanel9.BorderRadius = 10;
            this.guna2CustomGradientPanel9.Controls.Add(this.pictureBox18);
            this.guna2CustomGradientPanel9.Controls.Add(this.pictureBox19);
            this.guna2CustomGradientPanel9.Controls.Add(this.pictureBox20);
            this.guna2CustomGradientPanel9.Controls.Add(this.pictureBox21);
            this.guna2CustomGradientPanel9.Controls.Add(this.guna2HtmlLabel7);
            this.guna2CustomGradientPanel9.Controls.Add(this.guna2HtmlLabel8);
            this.guna2CustomGradientPanel9.Controls.Add(this.guna2HtmlLabel9);
            this.guna2CustomGradientPanel9.Controls.Add(this.guna2HtmlLabel10);
            this.guna2CustomGradientPanel9.FillColor = System.Drawing.Color.CornflowerBlue;
            this.guna2CustomGradientPanel9.FillColor4 = System.Drawing.Color.LightSteelBlue;
            this.guna2CustomGradientPanel9.Location = new System.Drawing.Point(13, 93);
            this.guna2CustomGradientPanel9.Name = "guna2CustomGradientPanel9";
            this.guna2CustomGradientPanel9.Size = new System.Drawing.Size(305, 115);
            this.guna2CustomGradientPanel9.TabIndex = 12;
            // 
            // pictureBox18
            // 
            this.pictureBox18.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox18.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox18.Image")));
            this.pictureBox18.Location = new System.Drawing.Point(6, 83);
            this.pictureBox18.Name = "pictureBox18";
            this.pictureBox18.Size = new System.Drawing.Size(22, 22);
            this.pictureBox18.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox18.TabIndex = 18;
            this.pictureBox18.TabStop = false;
            // 
            // pictureBox19
            // 
            this.pictureBox19.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox19.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox19.Image")));
            this.pictureBox19.Location = new System.Drawing.Point(6, 59);
            this.pictureBox19.Name = "pictureBox19";
            this.pictureBox19.Size = new System.Drawing.Size(22, 22);
            this.pictureBox19.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox19.TabIndex = 17;
            this.pictureBox19.TabStop = false;
            // 
            // pictureBox20
            // 
            this.pictureBox20.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox20.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox20.Image")));
            this.pictureBox20.Location = new System.Drawing.Point(6, 34);
            this.pictureBox20.Name = "pictureBox20";
            this.pictureBox20.Size = new System.Drawing.Size(22, 22);
            this.pictureBox20.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox20.TabIndex = 16;
            this.pictureBox20.TabStop = false;
            // 
            // pictureBox21
            // 
            this.pictureBox21.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox21.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox21.Image")));
            this.pictureBox21.Location = new System.Drawing.Point(6, 10);
            this.pictureBox21.Name = "pictureBox21";
            this.pictureBox21.Size = new System.Drawing.Size(22, 22);
            this.pictureBox21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox21.TabIndex = 15;
            this.pictureBox21.TabStop = false;
            // 
            // guna2HtmlLabel7
            // 
            this.guna2HtmlLabel7.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel7.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel7.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel7.Location = new System.Drawing.Point(34, 86);
            this.guna2HtmlLabel7.Name = "guna2HtmlLabel7";
            this.guna2HtmlLabel7.Size = new System.Drawing.Size(60, 17);
            this.guna2HtmlLabel7.TabIndex = 14;
            this.guna2HtmlLabel7.Text = "400000/giờ";
            // 
            // guna2HtmlLabel8
            // 
            this.guna2HtmlLabel8.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel8.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel8.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel8.Location = new System.Drawing.Point(34, 63);
            this.guna2HtmlLabel8.Name = "guna2HtmlLabel8";
            this.guna2HtmlLabel8.Size = new System.Drawing.Size(134, 17);
            this.guna2HtmlLabel8.TabIndex = 13;
            this.guna2HtmlLabel8.Text = "Thành phố Hồ Chí Minh";
            // 
            // guna2HtmlLabel9
            // 
            this.guna2HtmlLabel9.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel9.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel9.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel9.Location = new System.Drawing.Point(34, 37);
            this.guna2HtmlLabel9.Name = "guna2HtmlLabel9";
            this.guna2HtmlLabel9.Size = new System.Drawing.Size(63, 17);
            this.guna2HtmlLabel9.TabIndex = 12;
            this.guna2HtmlLabel9.Text = "0398764627";
            // 
            // guna2HtmlLabel10
            // 
            this.guna2HtmlLabel10.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel10.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel10.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel10.Location = new System.Drawing.Point(34, 13);
            this.guna2HtmlLabel10.Name = "guna2HtmlLabel10";
            this.guna2HtmlLabel10.Size = new System.Drawing.Size(135, 17);
            this.guna2HtmlLabel10.TabIndex = 11;
            this.guna2HtmlLabel10.Text = "nhantran9q5@gmail.com";
            // 
            // guna2HtmlLabel11
            // 
            this.guna2HtmlLabel11.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel11.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel11.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel11.Location = new System.Drawing.Point(96, 35);
            this.guna2HtmlLabel11.Name = "guna2HtmlLabel11";
            this.guna2HtmlLabel11.Size = new System.Drawing.Size(52, 21);
            this.guna2HtmlLabel11.TabIndex = 11;
            this.guna2HtmlLabel11.Text = "pt_001";
            // 
            // guna2HtmlLabel12
            // 
            this.guna2HtmlLabel12.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel12.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel12.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel12.Location = new System.Drawing.Point(96, 12);
            this.guna2HtmlLabel12.Name = "guna2HtmlLabel12";
            this.guna2HtmlLabel12.Size = new System.Drawing.Size(125, 25);
            this.guna2HtmlLabel12.TabIndex = 11;
            this.guna2HtmlLabel12.Text = "Nguyễn Văn A";
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("guna2CirclePictureBox1.Image")));
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(13, 12);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(64, 64);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox1.TabIndex = 0;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // pnlThongTinPT3
            // 
            this.pnlThongTinPT3.BackColor = System.Drawing.Color.Transparent;
            this.pnlThongTinPT3.BorderColor = System.Drawing.Color.Silver;
            this.pnlThongTinPT3.BorderRadius = 20;
            this.pnlThongTinPT3.BorderThickness = 1;
            this.pnlThongTinPT3.Controls.Add(this.guna2Button2);
            this.pnlThongTinPT3.Controls.Add(this.guna2GradientButton2);
            this.pnlThongTinPT3.Controls.Add(this.guna2CustomGradientPanel11);
            this.pnlThongTinPT3.Controls.Add(this.guna2CustomGradientPanel15);
            this.pnlThongTinPT3.Controls.Add(this.guna2HtmlLabel23);
            this.pnlThongTinPT3.Controls.Add(this.guna2HtmlLabel24);
            this.pnlThongTinPT3.Controls.Add(this.guna2CirclePictureBox2);
            this.pnlThongTinPT3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlThongTinPT3.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlThongTinPT3.Location = new System.Drawing.Point(680, 71);
            this.pnlThongTinPT3.Name = "pnlThongTinPT3";
            this.pnlThongTinPT3.Size = new System.Drawing.Size(328, 461);
            this.pnlThongTinPT3.TabIndex = 17;
            // 
            // guna2Button2
            // 
            this.guna2Button2.BorderRadius = 5;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button2.FillColor = System.Drawing.Color.White;
            this.guna2Button2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button2.ForeColor = System.Drawing.Color.White;
            this.guna2Button2.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button2.Image")));
            this.guna2Button2.ImageSize = new System.Drawing.Size(30, 30);
            this.guna2Button2.Location = new System.Drawing.Point(258, 422);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Size = new System.Drawing.Size(46, 34);
            this.guna2Button2.TabIndex = 15;
            // 
            // guna2GradientButton2
            // 
            this.guna2GradientButton2.BorderRadius = 10;
            this.guna2GradientButton2.BorderThickness = 1;
            this.guna2GradientButton2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2GradientButton2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2GradientButton2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2GradientButton2.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2GradientButton2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2GradientButton2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2GradientButton2.ForeColor = System.Drawing.Color.White;
            this.guna2GradientButton2.Location = new System.Drawing.Point(134, 422);
            this.guna2GradientButton2.Name = "guna2GradientButton2";
            this.guna2GradientButton2.Size = new System.Drawing.Size(118, 34);
            this.guna2GradientButton2.TabIndex = 11;
            this.guna2GradientButton2.Text = "Chi tiết";
            // 
            // guna2CustomGradientPanel11
            // 
            this.guna2CustomGradientPanel11.BorderRadius = 20;
            this.guna2CustomGradientPanel11.BorderThickness = 1;
            this.guna2CustomGradientPanel11.Controls.Add(this.guna2CustomGradientPanel12);
            this.guna2CustomGradientPanel11.Controls.Add(this.guna2CustomGradientPanel13);
            this.guna2CustomGradientPanel11.Controls.Add(this.guna2CustomGradientPanel14);
            this.guna2CustomGradientPanel11.FillColor = System.Drawing.Color.LightSteelBlue;
            this.guna2CustomGradientPanel11.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.guna2CustomGradientPanel11.Location = new System.Drawing.Point(13, 215);
            this.guna2CustomGradientPanel11.Name = "guna2CustomGradientPanel11";
            this.guna2CustomGradientPanel11.Size = new System.Drawing.Size(305, 201);
            this.guna2CustomGradientPanel11.TabIndex = 13;
            // 
            // guna2CustomGradientPanel12
            // 
            this.guna2CustomGradientPanel12.BorderRadius = 10;
            this.guna2CustomGradientPanel12.BorderThickness = 1;
            this.guna2CustomGradientPanel12.Controls.Add(this.guna2HtmlLabel13);
            this.guna2CustomGradientPanel12.Controls.Add(this.pictureBox22);
            this.guna2CustomGradientPanel12.Controls.Add(this.guna2HtmlLabel14);
            this.guna2CustomGradientPanel12.Location = new System.Drawing.Point(6, 129);
            this.guna2CustomGradientPanel12.Name = "guna2CustomGradientPanel12";
            this.guna2CustomGradientPanel12.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel12.TabIndex = 2;
            // 
            // guna2HtmlLabel13
            // 
            this.guna2HtmlLabel13.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel13.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel13.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel13.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel13.Name = "guna2HtmlLabel13";
            this.guna2HtmlLabel13.Size = new System.Drawing.Size(31, 17);
            this.guna2HtmlLabel13.TabIndex = 15;
            this.guna2HtmlLabel13.Text = "1.3M";
            // 
            // pictureBox22
            // 
            this.pictureBox22.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox22.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox22.Image")));
            this.pictureBox22.Location = new System.Drawing.Point(10, 9);
            this.pictureBox22.Name = "pictureBox22";
            this.pictureBox22.Size = new System.Drawing.Size(40, 34);
            this.pictureBox22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox22.TabIndex = 21;
            this.pictureBox22.TabStop = false;
            // 
            // guna2HtmlLabel14
            // 
            this.guna2HtmlLabel14.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel14.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel14.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel14.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel14.Name = "guna2HtmlLabel14";
            this.guna2HtmlLabel14.Size = new System.Drawing.Size(96, 17);
            this.guna2HtmlLabel14.TabIndex = 16;
            this.guna2HtmlLabel14.Text = "DOANH THU PT";
            // 
            // guna2CustomGradientPanel13
            // 
            this.guna2CustomGradientPanel13.BorderRadius = 10;
            this.guna2CustomGradientPanel13.BorderThickness = 1;
            this.guna2CustomGradientPanel13.Controls.Add(this.guna2HtmlLabel15);
            this.guna2CustomGradientPanel13.Controls.Add(this.pictureBox23);
            this.guna2CustomGradientPanel13.Controls.Add(this.guna2HtmlLabel16);
            this.guna2CustomGradientPanel13.Location = new System.Drawing.Point(6, 73);
            this.guna2CustomGradientPanel13.Name = "guna2CustomGradientPanel13";
            this.guna2CustomGradientPanel13.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel13.TabIndex = 1;
            // 
            // guna2HtmlLabel15
            // 
            this.guna2HtmlLabel15.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel15.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel15.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel15.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel15.Name = "guna2HtmlLabel15";
            this.guna2HtmlLabel15.Size = new System.Drawing.Size(10, 17);
            this.guna2HtmlLabel15.TabIndex = 20;
            this.guna2HtmlLabel15.Text = "3";
            // 
            // pictureBox23
            // 
            this.pictureBox23.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox23.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox23.Image")));
            this.pictureBox23.Location = new System.Drawing.Point(10, 8);
            this.pictureBox23.Name = "pictureBox23";
            this.pictureBox23.Size = new System.Drawing.Size(40, 34);
            this.pictureBox23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox23.TabIndex = 20;
            this.pictureBox23.TabStop = false;
            // 
            // guna2HtmlLabel16
            // 
            this.guna2HtmlLabel16.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel16.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel16.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel16.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel16.Name = "guna2HtmlLabel16";
            this.guna2HtmlLabel16.Size = new System.Drawing.Size(69, 17);
            this.guna2HtmlLabel16.TabIndex = 21;
            this.guna2HtmlLabel16.Text = "SỐ KHÁCH";
            // 
            // guna2CustomGradientPanel14
            // 
            this.guna2CustomGradientPanel14.BorderRadius = 10;
            this.guna2CustomGradientPanel14.BorderThickness = 1;
            this.guna2CustomGradientPanel14.Controls.Add(this.guna2HtmlLabel17);
            this.guna2CustomGradientPanel14.Controls.Add(this.guna2HtmlLabel18);
            this.guna2CustomGradientPanel14.Controls.Add(this.pictureBox24);
            this.guna2CustomGradientPanel14.Location = new System.Drawing.Point(6, 17);
            this.guna2CustomGradientPanel14.Name = "guna2CustomGradientPanel14";
            this.guna2CustomGradientPanel14.Size = new System.Drawing.Size(290, 50);
            this.guna2CustomGradientPanel14.TabIndex = 0;
            // 
            // guna2HtmlLabel17
            // 
            this.guna2HtmlLabel17.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel17.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel17.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel17.Location = new System.Drawing.Point(56, 27);
            this.guna2HtmlLabel17.Name = "guna2HtmlLabel17";
            this.guna2HtmlLabel17.Size = new System.Drawing.Size(20, 17);
            this.guna2HtmlLabel17.TabIndex = 11;
            this.guna2HtmlLabel17.Text = "4.7";
            // 
            // guna2HtmlLabel18
            // 
            this.guna2HtmlLabel18.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel18.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel18.ForeColor = System.Drawing.Color.Silver;
            this.guna2HtmlLabel18.Location = new System.Drawing.Point(56, 9);
            this.guna2HtmlLabel18.Name = "guna2HtmlLabel18";
            this.guna2HtmlLabel18.Size = new System.Drawing.Size(83, 17);
            this.guna2HtmlLabel18.TabIndex = 11;
            this.guna2HtmlLabel18.Text = "ĐÁNH GIÁ PT";
            // 
            // pictureBox24
            // 
            this.pictureBox24.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox24.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox24.Image")));
            this.pictureBox24.Location = new System.Drawing.Point(10, 9);
            this.pictureBox24.Name = "pictureBox24";
            this.pictureBox24.Size = new System.Drawing.Size(40, 34);
            this.pictureBox24.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox24.TabIndex = 19;
            this.pictureBox24.TabStop = false;
            // 
            // guna2CustomGradientPanel15
            // 
            this.guna2CustomGradientPanel15.BorderRadius = 10;
            this.guna2CustomGradientPanel15.Controls.Add(this.pictureBox25);
            this.guna2CustomGradientPanel15.Controls.Add(this.pictureBox26);
            this.guna2CustomGradientPanel15.Controls.Add(this.pictureBox27);
            this.guna2CustomGradientPanel15.Controls.Add(this.pictureBox28);
            this.guna2CustomGradientPanel15.Controls.Add(this.guna2HtmlLabel19);
            this.guna2CustomGradientPanel15.Controls.Add(this.guna2HtmlLabel20);
            this.guna2CustomGradientPanel15.Controls.Add(this.guna2HtmlLabel21);
            this.guna2CustomGradientPanel15.Controls.Add(this.guna2HtmlLabel22);
            this.guna2CustomGradientPanel15.FillColor = System.Drawing.Color.CornflowerBlue;
            this.guna2CustomGradientPanel15.FillColor4 = System.Drawing.Color.LightSteelBlue;
            this.guna2CustomGradientPanel15.Location = new System.Drawing.Point(13, 93);
            this.guna2CustomGradientPanel15.Name = "guna2CustomGradientPanel15";
            this.guna2CustomGradientPanel15.Size = new System.Drawing.Size(305, 115);
            this.guna2CustomGradientPanel15.TabIndex = 12;
            // 
            // pictureBox25
            // 
            this.pictureBox25.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox25.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox25.Image")));
            this.pictureBox25.Location = new System.Drawing.Point(6, 83);
            this.pictureBox25.Name = "pictureBox25";
            this.pictureBox25.Size = new System.Drawing.Size(22, 22);
            this.pictureBox25.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox25.TabIndex = 18;
            this.pictureBox25.TabStop = false;
            // 
            // pictureBox26
            // 
            this.pictureBox26.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox26.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox26.Image")));
            this.pictureBox26.Location = new System.Drawing.Point(6, 59);
            this.pictureBox26.Name = "pictureBox26";
            this.pictureBox26.Size = new System.Drawing.Size(22, 22);
            this.pictureBox26.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox26.TabIndex = 17;
            this.pictureBox26.TabStop = false;
            // 
            // pictureBox27
            // 
            this.pictureBox27.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox27.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox27.Image")));
            this.pictureBox27.Location = new System.Drawing.Point(6, 34);
            this.pictureBox27.Name = "pictureBox27";
            this.pictureBox27.Size = new System.Drawing.Size(22, 22);
            this.pictureBox27.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox27.TabIndex = 16;
            this.pictureBox27.TabStop = false;
            // 
            // pictureBox28
            // 
            this.pictureBox28.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox28.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox28.Image")));
            this.pictureBox28.Location = new System.Drawing.Point(6, 10);
            this.pictureBox28.Name = "pictureBox28";
            this.pictureBox28.Size = new System.Drawing.Size(22, 22);
            this.pictureBox28.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox28.TabIndex = 15;
            this.pictureBox28.TabStop = false;
            // 
            // guna2HtmlLabel19
            // 
            this.guna2HtmlLabel19.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel19.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel19.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel19.Location = new System.Drawing.Point(34, 86);
            this.guna2HtmlLabel19.Name = "guna2HtmlLabel19";
            this.guna2HtmlLabel19.Size = new System.Drawing.Size(60, 17);
            this.guna2HtmlLabel19.TabIndex = 14;
            this.guna2HtmlLabel19.Text = "400000/giờ";
            // 
            // guna2HtmlLabel20
            // 
            this.guna2HtmlLabel20.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel20.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel20.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel20.Location = new System.Drawing.Point(34, 63);
            this.guna2HtmlLabel20.Name = "guna2HtmlLabel20";
            this.guna2HtmlLabel20.Size = new System.Drawing.Size(134, 17);
            this.guna2HtmlLabel20.TabIndex = 13;
            this.guna2HtmlLabel20.Text = "Thành phố Hồ Chí Minh";
            // 
            // guna2HtmlLabel21
            // 
            this.guna2HtmlLabel21.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel21.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel21.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel21.Location = new System.Drawing.Point(34, 37);
            this.guna2HtmlLabel21.Name = "guna2HtmlLabel21";
            this.guna2HtmlLabel21.Size = new System.Drawing.Size(63, 17);
            this.guna2HtmlLabel21.TabIndex = 12;
            this.guna2HtmlLabel21.Text = "0398764627";
            // 
            // guna2HtmlLabel22
            // 
            this.guna2HtmlLabel22.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel22.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel22.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel22.Location = new System.Drawing.Point(34, 13);
            this.guna2HtmlLabel22.Name = "guna2HtmlLabel22";
            this.guna2HtmlLabel22.Size = new System.Drawing.Size(135, 17);
            this.guna2HtmlLabel22.TabIndex = 11;
            this.guna2HtmlLabel22.Text = "nhantran9q5@gmail.com";
            // 
            // guna2HtmlLabel23
            // 
            this.guna2HtmlLabel23.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel23.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel23.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel23.Location = new System.Drawing.Point(96, 35);
            this.guna2HtmlLabel23.Name = "guna2HtmlLabel23";
            this.guna2HtmlLabel23.Size = new System.Drawing.Size(52, 21);
            this.guna2HtmlLabel23.TabIndex = 11;
            this.guna2HtmlLabel23.Text = "pt_001";
            // 
            // guna2HtmlLabel24
            // 
            this.guna2HtmlLabel24.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel24.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel24.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel24.Location = new System.Drawing.Point(96, 12);
            this.guna2HtmlLabel24.Name = "guna2HtmlLabel24";
            this.guna2HtmlLabel24.Size = new System.Drawing.Size(125, 25);
            this.guna2HtmlLabel24.TabIndex = 11;
            this.guna2HtmlLabel24.Text = "Nguyễn Văn A";
            // 
            // guna2CirclePictureBox2
            // 
            this.guna2CirclePictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("guna2CirclePictureBox2.Image")));
            this.guna2CirclePictureBox2.ImageRotate = 0F;
            this.guna2CirclePictureBox2.Location = new System.Drawing.Point(13, 12);
            this.guna2CirclePictureBox2.Name = "guna2CirclePictureBox2";
            this.guna2CirclePictureBox2.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox2.Size = new System.Drawing.Size(64, 64);
            this.guna2CirclePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox2.TabIndex = 0;
            this.guna2CirclePictureBox2.TabStop = false;
            // 
            // ucQuanLiPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.pnlNen);
            this.Name = "ucQuanLiPT";
            this.Size = new System.Drawing.Size(1058, 739);
            this.pnlNen.ResumeLayout(false);
            this.pnlDanhSachHuanLuyenVien.ResumeLayout(false);
            this.pnlDanhSachHuanLuyenVien.PerformLayout();
            this.pnlThongTinPT.ResumeLayout(false);
            this.pnlThongTinPT.PerformLayout();
            this.pnlThongTinNghiepVu.ResumeLayout(false);
            this.guna2CustomGradientPanel3.ResumeLayout(false);
            this.guna2CustomGradientPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            this.guna2CustomGradientPanel2.ResumeLayout(false);
            this.guna2CustomGradientPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            this.guna2CustomGradientPanel1.ResumeLayout(false);
            this.guna2CustomGradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            this.pnlThongTinCaNhanPT.ResumeLayout(false);
            this.pnlThongTinCaNhanPT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptrAnhDaiDien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            this.pnlChucNang.ResumeLayout(false);
            this.pnlChucNang.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDanhGia)).EndInit();
            this.pnlTrungBinhDangCoLichThue.ResumeLayout(false);
            this.pnlTrungBinhDangCoLichThue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.pnlTrungBinhKhachDangThuePT.ResumeLayout(false);
            this.pnlTrungBinhKhachDangThuePT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.pnlTyLeKhachChonThue.ResumeLayout(false);
            this.pnlTyLeKhachChonThue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.pnlDoanhThuTrungBinhPT.ResumeLayout(false);
            this.pnlDoanhThuTrungBinhPT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.pnlDanhGiaTrungBinh.ResumeLayout(false);
            this.pnlDanhGiaTrungBinh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.pnlTongPT.ResumeLayout(false);
            this.pnlTongPT.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlTieuDe.ResumeLayout(false);
            this.pnlTieuDe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.pnlThongTinPT2.ResumeLayout(false);
            this.pnlThongTinPT2.PerformLayout();
            this.guna2CustomGradientPanel5.ResumeLayout(false);
            this.guna2CustomGradientPanel6.ResumeLayout(false);
            this.guna2CustomGradientPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            this.guna2CustomGradientPanel7.ResumeLayout(false);
            this.guna2CustomGradientPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).EndInit();
            this.guna2CustomGradientPanel8.ResumeLayout(false);
            this.guna2CustomGradientPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).EndInit();
            this.guna2CustomGradientPanel9.ResumeLayout(false);
            this.guna2CustomGradientPanel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.pnlThongTinPT3.ResumeLayout(false);
            this.pnlThongTinPT3.PerformLayout();
            this.guna2CustomGradientPanel11.ResumeLayout(false);
            this.guna2CustomGradientPanel12.ResumeLayout(false);
            this.guna2CustomGradientPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox22)).EndInit();
            this.guna2CustomGradientPanel13.ResumeLayout(false);
            this.guna2CustomGradientPanel13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox23)).EndInit();
            this.guna2CustomGradientPanel14.ResumeLayout(false);
            this.guna2CustomGradientPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox24)).EndInit();
            this.guna2CustomGradientPanel15.ResumeLayout(false);
            this.guna2CustomGradientPanel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlNen;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTieuDe;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTieuDe;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTrungBinhDangCoLichThue;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTrungBinhKhachDangThuePT;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTyLeKhachChonThue;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlDoanhThuTrungBinhPT;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlDanhGiaTrungBinh;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTongPT;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTongSoPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTongPT;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoTrungBinhPTDangCoLich;
        private System.Windows.Forms.PictureBox pictureBox6;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBuoi;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTrungBinhDangCoLichThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoTrungBinhKhachHangDangThue;
        private System.Windows.Forms.PictureBox pictureBox3;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKhach;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTrungBinhKhachDangThuePT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoTyLeKhachThuePT;
        private System.Windows.Forms.PictureBox pictureBox5;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTyLe;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTyLeKhachChonThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoDoanhThuTrungBinh;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblVND;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDoanhThuTrungBinhPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoDanhGiaTrungBinh;
        private System.Windows.Forms.PictureBox pictureBox4;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDanhGia;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDanhGiaTrungBinh;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlChucNang;
        private Guna.UI2.WinForms.Guna2TextBox txtTiemKiem;
        private Guna.UI2.WinForms.Guna2GradientButton btnApDung;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudDanhGia;
        private Guna.UI2.WinForms.Guna2ComboBox cboNhanKhach;
        private Guna.UI2.WinForms.Guna2ComboBox cboChuyenMon;
        private Guna.UI2.WinForms.Guna2GradientButton btnDatLai;
        private Guna.UI2.WinForms.Guna2ComboBox cboDiaChi;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDiemTrungBinh;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNhanKhach;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDiaDiem;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblChuyenMon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTimKiem;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlDanhSachHuanLuyenVien;
        private Guna.UI2.WinForms.Guna2GradientButton btnXacMinhDangKy;
        private System.Windows.Forms.PictureBox pictureBox7;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDanhSachHuanLuyenVien;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlThongTinPT;
        private Guna.UI2.WinForms.Guna2CirclePictureBox ptrAnhDaiDien;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlThongTinCaNhanPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMaPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblHovaTen;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGiaThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDiaChi;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoDienThoai;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGmail;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox11;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlThongTinNghiepVu;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel3;
        private System.Windows.Forms.PictureBox pictureBox14;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel2;
        private System.Windows.Forms.PictureBox pictureBox13;
        private System.Windows.Forms.PictureBox pictureBox12;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoDoanhThuPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDoanhThuPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoKhachHangDaThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKhachHangDaThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoDanhGiaPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDanhGiaPT;
        private Guna.UI2.WinForms.Guna2GradientButton btnXemChiTietPT;
        private Guna.UI2.WinForms.Guna2Button guna2Button4;
        private Guna.UI2.WinForms.Guna2Button btnXoa;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlThongTinPT3;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Guna.UI2.WinForms.Guna2GradientButton guna2GradientButton2;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel11;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel12;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel13;
        private System.Windows.Forms.PictureBox pictureBox22;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel14;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel13;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel15;
        private System.Windows.Forms.PictureBox pictureBox23;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel16;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel14;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel17;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel18;
        private System.Windows.Forms.PictureBox pictureBox24;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel15;
        private System.Windows.Forms.PictureBox pictureBox25;
        private System.Windows.Forms.PictureBox pictureBox26;
        private System.Windows.Forms.PictureBox pictureBox27;
        private System.Windows.Forms.PictureBox pictureBox28;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel19;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel20;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel21;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel22;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel23;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel24;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox2;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlThongTinPT2;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2GradientButton guna2GradientButton1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel5;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel6;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private System.Windows.Forms.PictureBox pictureBox15;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel7;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private System.Windows.Forms.PictureBox pictureBox16;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel4;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel8;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private System.Windows.Forms.PictureBox pictureBox17;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel9;
        private System.Windows.Forms.PictureBox pictureBox18;
        private System.Windows.Forms.PictureBox pictureBox19;
        private System.Windows.Forms.PictureBox pictureBox20;
        private System.Windows.Forms.PictureBox pictureBox21;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel7;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel8;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel9;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel10;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel11;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel12;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
    }
}
