namespace HealthApp.Views.PT
{
    partial class UserControlChonNgay
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlChonNgay));
            this.pnlChonNgay = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblChonNgay = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.ptrIcon = new Guna.UI2.WinForms.Guna2PictureBox();
            this.dtpChonNgay = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.lblGioBatDau = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.cboGioBatDau = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cboGioKetThuc = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblGioKetThuc = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2PictureBox2 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.btnXoa = new Guna.UI2.WinForms.Guna2CircleButton();
            this.pnlChonNgay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptrIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlChonNgay
            // 
            this.pnlChonNgay.Controls.Add(this.btnXoa);
            this.pnlChonNgay.Controls.Add(this.lblGioKetThuc);
            this.pnlChonNgay.Controls.Add(this.guna2PictureBox2);
            this.pnlChonNgay.Controls.Add(this.cboGioKetThuc);
            this.pnlChonNgay.Controls.Add(this.cboGioBatDau);
            this.pnlChonNgay.Controls.Add(this.lblGioBatDau);
            this.pnlChonNgay.Controls.Add(this.guna2PictureBox1);
            this.pnlChonNgay.Controls.Add(this.dtpChonNgay);
            this.pnlChonNgay.Controls.Add(this.lblChonNgay);
            this.pnlChonNgay.Controls.Add(this.ptrIcon);
            this.pnlChonNgay.Location = new System.Drawing.Point(3, 3);
            this.pnlChonNgay.Name = "pnlChonNgay";
            this.pnlChonNgay.Size = new System.Drawing.Size(614, 256);
            this.pnlChonNgay.TabIndex = 12;
            // 
            // lblChonNgay
            // 
            this.lblChonNgay.BackColor = System.Drawing.Color.Transparent;
            this.lblChonNgay.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChonNgay.Location = new System.Drawing.Point(39, 12);
            this.lblChonNgay.Name = "lblChonNgay";
            this.lblChonNgay.Size = new System.Drawing.Size(96, 25);
            this.lblChonNgay.TabIndex = 14;
            this.lblChonNgay.Text = "Chọn Ngày ";
            // 
            // ptrIcon
            // 
            this.ptrIcon.BackColor = System.Drawing.Color.White;
            this.ptrIcon.Image = ((System.Drawing.Image)(resources.GetObject("ptrIcon.Image")));
            this.ptrIcon.ImageRotate = 0F;
            this.ptrIcon.Location = new System.Drawing.Point(5, 11);
            this.ptrIcon.Name = "ptrIcon";
            this.ptrIcon.Size = new System.Drawing.Size(28, 26);
            this.ptrIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptrIcon.TabIndex = 13;
            this.ptrIcon.TabStop = false;
            // 
            // dtpChonNgay
            // 
            this.dtpChonNgay.Checked = true;
            this.dtpChonNgay.FillColor = System.Drawing.Color.White;
            this.dtpChonNgay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpChonNgay.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtpChonNgay.Location = new System.Drawing.Point(32, 43);
            this.dtpChonNgay.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpChonNgay.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpChonNgay.Name = "dtpChonNgay";
            this.dtpChonNgay.Size = new System.Drawing.Size(542, 36);
            this.dtpChonNgay.TabIndex = 15;
            this.dtpChonNgay.Value = new System.DateTime(2025, 11, 23, 14, 59, 38, 301);
            // 
            // lblGioBatDau
            // 
            this.lblGioBatDau.BackColor = System.Drawing.Color.Transparent;
            this.lblGioBatDau.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioBatDau.Location = new System.Drawing.Point(66, 101);
            this.lblGioBatDau.Name = "lblGioBatDau";
            this.lblGioBatDau.Size = new System.Drawing.Size(109, 25);
            this.lblGioBatDau.TabIndex = 17;
            this.lblGioBatDau.Text = "Giờ Bắt Đầu";
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.BackColor = System.Drawing.Color.White;
            this.guna2PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("guna2PictureBox1.Image")));
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(32, 100);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(28, 26);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox1.TabIndex = 16;
            this.guna2PictureBox1.TabStop = false;
            // 
            // cboGioBatDau
            // 
            this.cboGioBatDau.BackColor = System.Drawing.Color.Transparent;
            this.cboGioBatDau.BorderRadius = 15;
            this.cboGioBatDau.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGioBatDau.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGioBatDau.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGioBatDau.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGioBatDau.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGioBatDau.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboGioBatDau.ItemHeight = 30;
            this.cboGioBatDau.Location = new System.Drawing.Point(191, 97);
            this.cboGioBatDau.Name = "cboGioBatDau";
            this.cboGioBatDau.Size = new System.Drawing.Size(140, 36);
            this.cboGioBatDau.TabIndex = 18;
            // 
            // cboGioKetThuc
            // 
            this.cboGioKetThuc.BackColor = System.Drawing.Color.Transparent;
            this.cboGioKetThuc.BorderRadius = 15;
            this.cboGioKetThuc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGioKetThuc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGioKetThuc.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGioKetThuc.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGioKetThuc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGioKetThuc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboGioKetThuc.ItemHeight = 30;
            this.cboGioKetThuc.Location = new System.Drawing.Point(191, 163);
            this.cboGioKetThuc.Name = "cboGioKetThuc";
            this.cboGioKetThuc.Size = new System.Drawing.Size(140, 36);
            this.cboGioKetThuc.TabIndex = 19;
            // 
            // lblGioKetThuc
            // 
            this.lblGioKetThuc.BackColor = System.Drawing.Color.Transparent;
            this.lblGioKetThuc.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGioKetThuc.Location = new System.Drawing.Point(63, 167);
            this.lblGioKetThuc.Name = "lblGioKetThuc";
            this.lblGioKetThuc.Size = new System.Drawing.Size(119, 25);
            this.lblGioKetThuc.TabIndex = 21;
            this.lblGioKetThuc.Text = "Giờ Kết Thúc";
            // 
            // guna2PictureBox2
            // 
            this.guna2PictureBox2.BackColor = System.Drawing.Color.White;
            this.guna2PictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("guna2PictureBox2.Image")));
            this.guna2PictureBox2.ImageRotate = 0F;
            this.guna2PictureBox2.Location = new System.Drawing.Point(29, 166);
            this.guna2PictureBox2.Name = "guna2PictureBox2";
            this.guna2PictureBox2.Size = new System.Drawing.Size(28, 26);
            this.guna2PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox2.TabIndex = 20;
            this.guna2PictureBox2.TabStop = false;
            // 
            // btnXoa
            // 
            this.btnXoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXoa.FillColor = System.Drawing.Color.Red;
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnXoa.ForeColor = System.Drawing.Color.White;
            this.btnXoa.Image = ((System.Drawing.Image)(resources.GetObject("btnXoa.Image")));
            this.btnXoa.Location = new System.Drawing.Point(561, 208);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnXoa.Size = new System.Drawing.Size(50, 45);
            this.btnXoa.TabIndex = 22;
            // 
            // UserControlChonNgay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlChonNgay);
            this.Name = "UserControlChonNgay";
            this.Size = new System.Drawing.Size(622, 261);
            this.pnlChonNgay.ResumeLayout(false);
            this.pnlChonNgay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptrIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlChonNgay;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblChonNgay;
        private Guna.UI2.WinForms.Guna2PictureBox ptrIcon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGioBatDau;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpChonNgay;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGioKetThuc;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox2;
        private Guna.UI2.WinForms.Guna2ComboBox cboGioKetThuc;
        private Guna.UI2.WinForms.Guna2ComboBox cboGioBatDau;
        private Guna.UI2.WinForms.Guna2CircleButton btnXoa;
    }
}
