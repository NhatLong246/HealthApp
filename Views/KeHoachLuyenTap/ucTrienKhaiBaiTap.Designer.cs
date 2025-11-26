namespace HealthApp.Views.KeHoachLuyenTap
{
    partial class ucTrienKhaiBaiTap
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
                // Dispose timer and db context
                DisposeTimer();
                DisposeDbContext();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucTrienKhaiBaiTap));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTroVe = new Guna.UI2.WinForms.Guna2Button();
            this.guna2GroupBox1 = new Guna.UI2.WinForms.Guna2GroupBox();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            this.btnTamNghi = new Guna.UI2.WinForms.Guna2Button();
            this.btnXong = new Guna.UI2.WinForms.Guna2Button();
            this.btnBatDauTap = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbDauHaiCham = new System.Windows.Forms.Label();
            this.txtGiay = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtPhut = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtGio = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2GroupBox2 = new Guna.UI2.WinForms.Guna2GroupBox();
            this.lbGenThietBi = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.guna2GradientPanel3 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenMucDo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2GradientPanel4 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2HtmlLabel9 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel8 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2GradientPanel5 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2HtmlLabel7 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenCalo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2GradientPanel2 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenHieuQua = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenThoiLuong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnRank = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbSoBuoiTap = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenSoBuoiTap = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnHoanThanh = new Guna.UI2.WinForms.Guna2Button();
            this.guna2GroupBox1.SuspendLayout();
            this.guna2Panel2.SuspendLayout();
            this.guna2GroupBox2.SuspendLayout();
            this.guna2GradientPanel3.SuspendLayout();
            this.guna2GradientPanel4.SuspendLayout();
            this.guna2GradientPanel5.SuspendLayout();
            this.guna2GradientPanel2.SuspendLayout();
            this.guna2GradientPanel1.SuspendLayout();
            this.pnRank.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(82, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(523, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Giúp bạn có thể quan sát và hỗ trợ bạn trong quá trình luyện tập của bản thân";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(80, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 28);
            this.label1.TabIndex = 11;
            this.label1.Text = "TRIỂN KHAI LUYỆN TẬP";
            // 
            // btnTroVe
            // 
            this.btnTroVe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTroVe.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnTroVe.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnTroVe.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTroVe.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnTroVe.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.btnTroVe.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnTroVe.ForeColor = System.Drawing.Color.White;
            this.btnTroVe.Image = ((System.Drawing.Image)(resources.GetObject("btnTroVe.Image")));
            this.btnTroVe.ImageSize = new System.Drawing.Size(35, 35);
            this.btnTroVe.Location = new System.Drawing.Point(15, 12);
            this.btnTroVe.Name = "btnTroVe";
            this.btnTroVe.Size = new System.Drawing.Size(45, 45);
            this.btnTroVe.TabIndex = 10;
            // 
            // guna2GroupBox1
            // 
            this.guna2GroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2GroupBox1.BorderColor = System.Drawing.Color.Gray;
            this.guna2GroupBox1.BorderRadius = 20;
            this.guna2GroupBox1.Controls.Add(this.guna2Button2);
            this.guna2GroupBox1.Controls.Add(this.btnTamNghi);
            this.guna2GroupBox1.Controls.Add(this.btnXong);
            this.guna2GroupBox1.Controls.Add(this.btnBatDauTap);
            this.guna2GroupBox1.Controls.Add(this.guna2Panel2);
            this.guna2GroupBox1.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2GroupBox1.ForeColor = System.Drawing.Color.Teal;
            this.guna2GroupBox1.Location = new System.Drawing.Point(58, 136);
            this.guna2GroupBox1.Name = "guna2GroupBox1";
            this.guna2GroupBox1.Size = new System.Drawing.Size(488, 369);
            this.guna2GroupBox1.TabIndex = 13;
            this.guna2GroupBox1.Text = "Thời Gian Tập";
            // 
            // guna2Button2
            // 
            this.guna2Button2.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button2.BorderRadius = 20;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.guna2Button2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2Button2.ForeColor = System.Drawing.Color.White;
            this.guna2Button2.Location = new System.Drawing.Point(363, 174);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Size = new System.Drawing.Size(111, 31);
            this.guna2Button2.TabIndex = 17;
            this.guna2Button2.Text = "Đặt Lại";
            // 
            // btnTamNghi
            // 
            this.btnTamNghi.BackColor = System.Drawing.Color.Transparent;
            this.btnTamNghi.BorderRadius = 20;
            this.btnTamNghi.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnTamNghi.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnTamNghi.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTamNghi.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnTamNghi.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnTamNghi.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTamNghi.ForeColor = System.Drawing.Color.White;
            this.btnTamNghi.Location = new System.Drawing.Point(129, 174);
            this.btnTamNghi.Name = "btnTamNghi";
            this.btnTamNghi.Size = new System.Drawing.Size(111, 31);
            this.btnTamNghi.TabIndex = 16;
            this.btnTamNghi.Text = "Tạm Nghỉ";
            // 
            // btnXong
            // 
            this.btnXong.BackColor = System.Drawing.Color.Transparent;
            this.btnXong.BorderRadius = 20;
            this.btnXong.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXong.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXong.FillColor = System.Drawing.Color.LimeGreen;
            this.btnXong.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXong.ForeColor = System.Drawing.Color.White;
            this.btnXong.Location = new System.Drawing.Point(246, 174);
            this.btnXong.Name = "btnXong";
            this.btnXong.Size = new System.Drawing.Size(111, 31);
            this.btnXong.TabIndex = 15;
            this.btnXong.Text = "Xong";
            // 
            // btnBatDauTap
            // 
            this.btnBatDauTap.BackColor = System.Drawing.Color.Transparent;
            this.btnBatDauTap.BorderRadius = 20;
            this.btnBatDauTap.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBatDauTap.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBatDauTap.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBatDauTap.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBatDauTap.FillColor = System.Drawing.Color.LimeGreen;
            this.btnBatDauTap.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBatDauTap.ForeColor = System.Drawing.Color.White;
            this.btnBatDauTap.Location = new System.Drawing.Point(12, 174);
            this.btnBatDauTap.Name = "btnBatDauTap";
            this.btnBatDauTap.Size = new System.Drawing.Size(111, 31);
            this.btnBatDauTap.TabIndex = 14;
            this.btnBatDauTap.Text = "Bắt đầu bài tập";
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BackColor = System.Drawing.Color.White;
            this.guna2Panel2.BorderColor = System.Drawing.Color.DimGray;
            this.guna2Panel2.BorderRadius = 20;
            this.guna2Panel2.BorderThickness = 1;
            this.guna2Panel2.Controls.Add(this.label3);
            this.guna2Panel2.Controls.Add(this.lbDauHaiCham);
            this.guna2Panel2.Controls.Add(this.txtGiay);
            this.guna2Panel2.Controls.Add(this.txtPhut);
            this.guna2Panel2.Controls.Add(this.txtGio);
            this.guna2Panel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.guna2Panel2.Location = new System.Drawing.Point(12, 59);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(462, 109);
            this.guna2Panel2.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(261, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 38);
            this.label3.TabIndex = 48;
            this.label3.Text = ":";
            // 
            // lbDauHaiCham
            // 
            this.lbDauHaiCham.AutoSize = true;
            this.lbDauHaiCham.BackColor = System.Drawing.Color.Transparent;
            this.lbDauHaiCham.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDauHaiCham.Location = new System.Drawing.Point(179, 28);
            this.lbDauHaiCham.Name = "lbDauHaiCham";
            this.lbDauHaiCham.Size = new System.Drawing.Size(25, 38);
            this.lbDauHaiCham.TabIndex = 47;
            this.lbDauHaiCham.Text = ":";
            // 
            // txtGiay
            // 
            this.txtGiay.BorderRadius = 10;
            this.txtGiay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGiay.DefaultText = "";
            this.txtGiay.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtGiay.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtGiay.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtGiay.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtGiay.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtGiay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtGiay.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtGiay.Location = new System.Drawing.Point(285, 28);
            this.txtGiay.Margin = new System.Windows.Forms.Padding(5);
            this.txtGiay.Name = "txtGiay";
            this.txtGiay.PlaceholderText = "";
            this.txtGiay.SelectedText = "";
            this.txtGiay.Size = new System.Drawing.Size(60, 45);
            this.txtGiay.TabIndex = 46;
            // 
            // txtPhut
            // 
            this.txtPhut.BorderRadius = 10;
            this.txtPhut.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhut.DefaultText = "";
            this.txtPhut.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtPhut.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtPhut.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPhut.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPhut.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPhut.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPhut.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPhut.Location = new System.Drawing.Point(202, 28);
            this.txtPhut.Margin = new System.Windows.Forms.Padding(5);
            this.txtPhut.Name = "txtPhut";
            this.txtPhut.PlaceholderText = "";
            this.txtPhut.SelectedText = "";
            this.txtPhut.Size = new System.Drawing.Size(60, 45);
            this.txtPhut.TabIndex = 45;
            // 
            // txtGio
            // 
            this.txtGio.BorderRadius = 10;
            this.txtGio.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGio.DefaultText = "";
            this.txtGio.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtGio.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtGio.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtGio.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtGio.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtGio.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtGio.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtGio.Location = new System.Drawing.Point(118, 28);
            this.txtGio.Margin = new System.Windows.Forms.Padding(5);
            this.txtGio.Name = "txtGio";
            this.txtGio.PlaceholderText = "";
            this.txtGio.SelectedText = "";
            this.txtGio.Size = new System.Drawing.Size(60, 45);
            this.txtGio.TabIndex = 44;
            // 
            // guna2GroupBox2
            // 
            this.guna2GroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.guna2GroupBox2.BorderColor = System.Drawing.Color.Gray;
            this.guna2GroupBox2.BorderRadius = 20;
            this.guna2GroupBox2.Controls.Add(this.lbGenThietBi);
            this.guna2GroupBox2.Controls.Add(this.label4);
            this.guna2GroupBox2.Controls.Add(this.guna2GradientPanel3);
            this.guna2GroupBox2.Controls.Add(this.guna2GradientPanel4);
            this.guna2GroupBox2.Controls.Add(this.guna2GradientPanel5);
            this.guna2GroupBox2.Controls.Add(this.guna2GradientPanel2);
            this.guna2GroupBox2.Controls.Add(this.guna2GradientPanel1);
            this.guna2GroupBox2.Controls.Add(this.pnRank);
            this.guna2GroupBox2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2GroupBox2.ForeColor = System.Drawing.Color.Teal;
            this.guna2GroupBox2.Location = new System.Drawing.Point(629, 136);
            this.guna2GroupBox2.Name = "guna2GroupBox2";
            this.guna2GroupBox2.Size = new System.Drawing.Size(611, 394);
            this.guna2GroupBox2.TabIndex = 15;
            this.guna2GroupBox2.Text = "Thông tin chi tiết";
            // 
            // lbGenThietBi
            // 
            this.lbGenThietBi.AutoSize = true;
            this.lbGenThietBi.Location = new System.Drawing.Point(189, 322);
            this.lbGenThietBi.Name = "lbGenThietBi";
            this.lbGenThietBi.Size = new System.Drawing.Size(28, 31);
            this.lbGenThietBi.TabIndex = 10;
            this.lbGenThietBi.Text = "#";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 322);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 31);
            this.label4.TabIndex = 9;
            this.label4.Text = "Thiết bị cần:";
            // 
            // guna2GradientPanel3
            // 
            this.guna2GradientPanel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel3.BorderRadius = 20;
            this.guna2GradientPanel3.Controls.Add(this.guna2HtmlLabel3);
            this.guna2GradientPanel3.Controls.Add(this.lbGenMucDo);
            this.guna2GradientPanel3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel3.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel3.Location = new System.Drawing.Point(446, 194);
            this.guna2GradientPanel3.Name = "guna2GradientPanel3";
            this.guna2GradientPanel3.Size = new System.Drawing.Size(136, 90);
            this.guna2GradientPanel3.TabIndex = 8;
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(34, 53);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(56, 20);
            this.guna2HtmlLabel3.TabIndex = 1;
            this.guna2HtmlLabel3.Text = "Mức Độ";
            // 
            // lbGenMucDo
            // 
            this.lbGenMucDo.BackColor = System.Drawing.Color.Transparent;
            this.lbGenMucDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenMucDo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenMucDo.Location = new System.Drawing.Point(58, 20);
            this.lbGenMucDo.Name = "lbGenMucDo";
            this.lbGenMucDo.Size = new System.Drawing.Size(15, 27);
            this.lbGenMucDo.TabIndex = 0;
            this.lbGenMucDo.Text = "#";
            // 
            // guna2GradientPanel4
            // 
            this.guna2GradientPanel4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel4.BorderRadius = 20;
            this.guna2GradientPanel4.Controls.Add(this.guna2HtmlLabel9);
            this.guna2GradientPanel4.Controls.Add(this.guna2HtmlLabel8);
            this.guna2GradientPanel4.Controls.Add(this.guna2HtmlLabel5);
            this.guna2GradientPanel4.Controls.Add(this.guna2HtmlLabel6);
            this.guna2GradientPanel4.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel4.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel4.Location = new System.Drawing.Point(240, 194);
            this.guna2GradientPanel4.Name = "guna2GradientPanel4";
            this.guna2GradientPanel4.Size = new System.Drawing.Size(136, 90);
            this.guna2GradientPanel4.TabIndex = 7;
            // 
            // guna2HtmlLabel9
            // 
            this.guna2HtmlLabel9.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.guna2HtmlLabel9.Location = new System.Drawing.Point(79, 20);
            this.guna2HtmlLabel9.Name = "guna2HtmlLabel9";
            this.guna2HtmlLabel9.Size = new System.Drawing.Size(15, 27);
            this.guna2HtmlLabel9.TabIndex = 3;
            this.guna2HtmlLabel9.Text = "#";
            // 
            // guna2HtmlLabel8
            // 
            this.guna2HtmlLabel8.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.guna2HtmlLabel8.Location = new System.Drawing.Point(58, 20);
            this.guna2HtmlLabel8.Name = "guna2HtmlLabel8";
            this.guna2HtmlLabel8.Size = new System.Drawing.Size(11, 27);
            this.guna2HtmlLabel8.TabIndex = 2;
            this.guna2HtmlLabel8.Text = "-";
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(37, 53);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(57, 20);
            this.guna2HtmlLabel5.TabIndex = 1;
            this.guna2HtmlLabel5.Text = "Set-Rep";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(37, 20);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(15, 27);
            this.guna2HtmlLabel6.TabIndex = 0;
            this.guna2HtmlLabel6.Text = "#";
            // 
            // guna2GradientPanel5
            // 
            this.guna2GradientPanel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel5.BorderRadius = 20;
            this.guna2GradientPanel5.Controls.Add(this.guna2HtmlLabel7);
            this.guna2GradientPanel5.Controls.Add(this.lbGenCalo);
            this.guna2GradientPanel5.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel5.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel5.Location = new System.Drawing.Point(29, 194);
            this.guna2GradientPanel5.Name = "guna2GradientPanel5";
            this.guna2GradientPanel5.Size = new System.Drawing.Size(136, 90);
            this.guna2GradientPanel5.TabIndex = 6;
            // 
            // guna2HtmlLabel7
            // 
            this.guna2HtmlLabel7.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.guna2HtmlLabel7.Location = new System.Drawing.Point(27, 53);
            this.guna2HtmlLabel7.Name = "guna2HtmlLabel7";
            this.guna2HtmlLabel7.Size = new System.Drawing.Size(69, 20);
            this.guna2HtmlLabel7.TabIndex = 1;
            this.guna2HtmlLabel7.Text = "Calo-Buổi";
            // 
            // lbGenCalo
            // 
            this.lbGenCalo.BackColor = System.Drawing.Color.Transparent;
            this.lbGenCalo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenCalo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenCalo.Location = new System.Drawing.Point(58, 20);
            this.lbGenCalo.Name = "lbGenCalo";
            this.lbGenCalo.Size = new System.Drawing.Size(15, 27);
            this.lbGenCalo.TabIndex = 0;
            this.lbGenCalo.Text = "#";
            // 
            // guna2GradientPanel2
            // 
            this.guna2GradientPanel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel2.BorderRadius = 20;
            this.guna2GradientPanel2.Controls.Add(this.guna2HtmlLabel2);
            this.guna2GradientPanel2.Controls.Add(this.lbGenHieuQua);
            this.guna2GradientPanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel2.Location = new System.Drawing.Point(437, 59);
            this.guna2GradientPanel2.Name = "guna2GradientPanel2";
            this.guna2GradientPanel2.Size = new System.Drawing.Size(136, 90);
            this.guna2GradientPanel2.TabIndex = 5;
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(34, 53);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(65, 20);
            this.guna2HtmlLabel2.TabIndex = 1;
            this.guna2HtmlLabel2.Text = "Hiệu Quả";
            // 
            // lbGenHieuQua
            // 
            this.lbGenHieuQua.BackColor = System.Drawing.Color.Transparent;
            this.lbGenHieuQua.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenHieuQua.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenHieuQua.Location = new System.Drawing.Point(58, 20);
            this.lbGenHieuQua.Name = "lbGenHieuQua";
            this.lbGenHieuQua.Size = new System.Drawing.Size(15, 27);
            this.lbGenHieuQua.TabIndex = 0;
            this.lbGenHieuQua.Text = "#";
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel1.BorderRadius = 20;
            this.guna2GradientPanel1.Controls.Add(this.guna2HtmlLabel1);
            this.guna2GradientPanel1.Controls.Add(this.lbGenThoiLuong);
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(231, 59);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.Size = new System.Drawing.Size(136, 90);
            this.guna2GradientPanel1.TabIndex = 4;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(13, 53);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(111, 20);
            this.guna2HtmlLabel1.TabIndex = 1;
            this.guna2HtmlLabel1.Text = "Thời Lượng Phút";
            // 
            // lbGenThoiLuong
            // 
            this.lbGenThoiLuong.BackColor = System.Drawing.Color.Transparent;
            this.lbGenThoiLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenThoiLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenThoiLuong.Location = new System.Drawing.Point(58, 20);
            this.lbGenThoiLuong.Name = "lbGenThoiLuong";
            this.lbGenThoiLuong.Size = new System.Drawing.Size(15, 27);
            this.lbGenThoiLuong.TabIndex = 0;
            this.lbGenThoiLuong.Text = "#";
            // 
            // pnRank
            // 
            this.pnRank.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.BorderRadius = 20;
            this.pnRank.Controls.Add(this.lbSoBuoiTap);
            this.pnRank.Controls.Add(this.lbGenSoBuoiTap);
            this.pnRank.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.Location = new System.Drawing.Point(20, 59);
            this.pnRank.Name = "pnRank";
            this.pnRank.Size = new System.Drawing.Size(136, 90);
            this.pnRank.TabIndex = 3;
            // 
            // lbSoBuoiTap
            // 
            this.lbSoBuoiTap.BackColor = System.Drawing.Color.Transparent;
            this.lbSoBuoiTap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSoBuoiTap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.lbSoBuoiTap.Location = new System.Drawing.Point(20, 53);
            this.lbSoBuoiTap.Name = "lbSoBuoiTap";
            this.lbSoBuoiTap.Size = new System.Drawing.Size(85, 20);
            this.lbSoBuoiTap.TabIndex = 1;
            this.lbSoBuoiTap.Text = "Số Buổi Tập";
            // 
            // lbGenSoBuoiTap
            // 
            this.lbGenSoBuoiTap.BackColor = System.Drawing.Color.Transparent;
            this.lbGenSoBuoiTap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenSoBuoiTap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenSoBuoiTap.Location = new System.Drawing.Point(58, 20);
            this.lbGenSoBuoiTap.Name = "lbGenSoBuoiTap";
            this.lbGenSoBuoiTap.Size = new System.Drawing.Size(15, 27);
            this.lbGenSoBuoiTap.TabIndex = 0;
            this.lbGenSoBuoiTap.Text = "#";
            // 
            // btnHoanThanh
            // 
            this.btnHoanThanh.BorderRadius = 20;
            this.btnHoanThanh.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHoanThanh.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHoanThanh.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHoanThanh.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHoanThanh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnHoanThanh.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoanThanh.ForeColor = System.Drawing.Color.White;
            this.btnHoanThanh.Location = new System.Drawing.Point(1028, 629);
            this.btnHoanThanh.Name = "btnHoanThanh";
            this.btnHoanThanh.Size = new System.Drawing.Size(203, 51);
            this.btnHoanThanh.TabIndex = 16;
            this.btnHoanThanh.Text = "Hoàn thành";
            // 
            // ucTrienKhaiBaiTap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btnHoanThanh);
            this.Controls.Add(this.guna2GroupBox2);
            this.Controls.Add(this.guna2GroupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTroVe);
            this.Name = "ucTrienKhaiBaiTap";
            this.Size = new System.Drawing.Size(1345, 1000);
            this.guna2GroupBox1.ResumeLayout(false);
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.guna2GroupBox2.ResumeLayout(false);
            this.guna2GroupBox2.PerformLayout();
            this.guna2GradientPanel3.ResumeLayout(false);
            this.guna2GradientPanel3.PerformLayout();
            this.guna2GradientPanel4.ResumeLayout(false);
            this.guna2GradientPanel4.PerformLayout();
            this.guna2GradientPanel5.ResumeLayout(false);
            this.guna2GradientPanel5.PerformLayout();
            this.guna2GradientPanel2.ResumeLayout(false);
            this.guna2GradientPanel2.PerformLayout();
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2GradientPanel1.PerformLayout();
            this.pnRank.ResumeLayout(false);
            this.pnRank.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button btnTroVe;
        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox2;
        private Guna.UI2.WinForms.Guna2TextBox txtGiay;
        private Guna.UI2.WinForms.Guna2TextBox txtPhut;
        private Guna.UI2.WinForms.Guna2TextBox txtGio;
        private System.Windows.Forms.Label lbDauHaiCham;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2Button btnBatDauTap;
        private Guna.UI2.WinForms.Guna2Button btnXong;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Guna.UI2.WinForms.Guna2Button btnTamNghi;
        private Guna.UI2.WinForms.Guna2GradientPanel pnRank;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbSoBuoiTap;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenSoBuoiTap;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenThoiLuong;
        private System.Windows.Forms.Label lbGenThietBi;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel3;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenMucDo;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel4;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel9;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel8;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel7;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenCalo;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenHieuQua;
        private Guna.UI2.WinForms.Guna2Button btnHoanThanh;
    }
}
