namespace HealthApp.Views.Admin
{
    partial class ucHieuSuat
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
                // Dispose database context
                var dbContextField = typeof(ucHieuSuat).GetField("_dbContext", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (dbContextField != null)
                {
                    var dbContext = dbContextField.GetValue(this) as System.IDisposable;
                    dbContext?.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucHieuSuat));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlDoanhThuTrungBinhPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lbGenTongPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pnlDanhGiaTrungBinh = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lbGenTienHoaHong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pnlTongPT = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lbGenTongNguoiDung = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTongPT = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlTieuDe = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.lblTieuDe = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlDoanhThuTrungBinhPT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlDanhGiaTrungBinh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.pnlTongPT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlTieuDe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlDoanhThuTrungBinhPT
            // 
            this.pnlDoanhThuTrungBinhPT.BorderColor = System.Drawing.Color.Silver;
            this.pnlDoanhThuTrungBinhPT.BorderRadius = 15;
            this.pnlDoanhThuTrungBinhPT.BorderThickness = 1;
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.lbGenTongPT);
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.guna2HtmlLabel6);
            this.pnlDoanhThuTrungBinhPT.Controls.Add(this.pictureBox2);
            this.pnlDoanhThuTrungBinhPT.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pnlDoanhThuTrungBinhPT.Location = new System.Drawing.Point(386, 127);
            this.pnlDoanhThuTrungBinhPT.Name = "pnlDoanhThuTrungBinhPT";
            this.pnlDoanhThuTrungBinhPT.Size = new System.Drawing.Size(274, 124);
            this.pnlDoanhThuTrungBinhPT.TabIndex = 13;
            // 
            // lbGenTongPT
            // 
            this.lbGenTongPT.BackColor = System.Drawing.Color.Transparent;
            this.lbGenTongPT.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenTongPT.ForeColor = System.Drawing.Color.Black;
            this.lbGenTongPT.Location = new System.Drawing.Point(76, 43);
            this.lbGenTongPT.Name = "lbGenTongPT";
            this.lbGenTongPT.Size = new System.Drawing.Size(16, 31);
            this.lbGenTongPT.TabIndex = 10;
            this.lbGenTongPT.Text = "x";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(15, 10);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(85, 21);
            this.guna2HtmlLabel6.TabIndex = 8;
            this.guna2HtmlLabel6.Text = "Tổng số PT";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox2.Image = global::HealthApp.Properties.Resources.icons_kcal;
            this.pictureBox2.Location = new System.Drawing.Point(15, 37);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(52, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pnlDanhGiaTrungBinh
            // 
            this.pnlDanhGiaTrungBinh.BorderColor = System.Drawing.Color.Silver;
            this.pnlDanhGiaTrungBinh.BorderRadius = 15;
            this.pnlDanhGiaTrungBinh.BorderThickness = 1;
            this.pnlDanhGiaTrungBinh.Controls.Add(this.lbGenTienHoaHong);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.guna2HtmlLabel2);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.guna2HtmlLabel3);
            this.pnlDanhGiaTrungBinh.Controls.Add(this.pictureBox4);
            this.pnlDanhGiaTrungBinh.FillColor = System.Drawing.Color.Violet;
            this.pnlDanhGiaTrungBinh.FillColor4 = System.Drawing.Color.LavenderBlush;
            this.pnlDanhGiaTrungBinh.Location = new System.Drawing.Point(739, 127);
            this.pnlDanhGiaTrungBinh.Name = "pnlDanhGiaTrungBinh";
            this.pnlDanhGiaTrungBinh.Size = new System.Drawing.Size(274, 124);
            this.pnlDanhGiaTrungBinh.TabIndex = 14;
            // 
            // lbGenTienHoaHong
            // 
            this.lbGenTienHoaHong.BackColor = System.Drawing.Color.Transparent;
            this.lbGenTienHoaHong.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenTienHoaHong.ForeColor = System.Drawing.Color.Black;
            this.lbGenTienHoaHong.Location = new System.Drawing.Point(73, 43);
            this.lbGenTienHoaHong.Name = "lbGenTienHoaHong";
            this.lbGenTienHoaHong.Size = new System.Drawing.Size(16, 31);
            this.lbGenTienHoaHong.TabIndex = 7;
            this.lbGenTienHoaHong.Text = "x";
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(73, 84);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(38, 21);
            this.guna2HtmlLabel2.TabIndex = 6;
            this.guna2HtmlLabel2.Text = "VNĐ";
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.Gray;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(12, 10);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(198, 21);
            this.guna2HtmlLabel3.TabIndex = 5;
            this.guna2HtmlLabel3.Text = "Doanh thu app trong tháng";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.White;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(12, 37);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(52, 48);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // pnlTongPT
            // 
            this.pnlTongPT.BorderColor = System.Drawing.Color.Silver;
            this.pnlTongPT.BorderRadius = 15;
            this.pnlTongPT.BorderThickness = 1;
            this.pnlTongPT.Controls.Add(this.lbGenTongNguoiDung);
            this.pnlTongPT.Controls.Add(this.lblTongPT);
            this.pnlTongPT.Controls.Add(this.pictureBox1);
            this.pnlTongPT.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlTongPT.Location = new System.Drawing.Point(34, 127);
            this.pnlTongPT.Name = "pnlTongPT";
            this.pnlTongPT.Size = new System.Drawing.Size(274, 124);
            this.pnlTongPT.TabIndex = 12;
            // 
            // lbGenTongNguoiDung
            // 
            this.lbGenTongNguoiDung.BackColor = System.Drawing.Color.Transparent;
            this.lbGenTongNguoiDung.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenTongNguoiDung.ForeColor = System.Drawing.Color.Black;
            this.lbGenTongNguoiDung.Location = new System.Drawing.Point(66, 43);
            this.lbGenTongNguoiDung.Name = "lbGenTongNguoiDung";
            this.lbGenTongNguoiDung.Size = new System.Drawing.Size(16, 31);
            this.lbGenTongNguoiDung.TabIndex = 3;
            this.lbGenTongNguoiDung.Text = "x";
            // 
            // lblTongPT
            // 
            this.lblTongPT.BackColor = System.Drawing.Color.Transparent;
            this.lblTongPT.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongPT.ForeColor = System.Drawing.Color.Gray;
            this.lblTongPT.Location = new System.Drawing.Point(14, 10);
            this.lblTongPT.Name = "lblTongPT";
            this.lblTongPT.Size = new System.Drawing.Size(147, 21);
            this.lblTongPT.TabIndex = 1;
            this.lblTongPT.Text = "Tổng số người dùng";
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
            this.pnlTieuDe.Controls.Add(this.pictureBox7);
            this.pnlTieuDe.Controls.Add(this.lblTieuDe);
            this.pnlTieuDe.Location = new System.Drawing.Point(0, 0);
            this.pnlTieuDe.Name = "pnlTieuDe";
            this.pnlTieuDe.Size = new System.Drawing.Size(1110, 111);
            this.pnlTieuDe.TabIndex = 16;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox7.Image = global::HealthApp.Properties.Resources.ListIcon;
            this.pictureBox7.Location = new System.Drawing.Point(290, 14);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(52, 48);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 12;
            this.pictureBox7.TabStop = false;
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.BackColor = System.Drawing.Color.Transparent;
            this.lblTieuDe.Enabled = false;
            this.lblTieuDe.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.ForeColor = System.Drawing.Color.Black;
            this.lblTieuDe.Location = new System.Drawing.Point(16, 27);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(268, 35);
            this.lblTieuDe.TabIndex = 1;
            this.lblTieuDe.Text = "Thống kê Tổng Quan";
            // 
            // chart1
            // 
            this.chart1.BorderlineColor = System.Drawing.Color.LightGray;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(20, 265);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(533, 385);
            this.chart1.TabIndex = 17;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            this.chart2.BorderlineColor = System.Drawing.Color.LightGray;
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(580, 265);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(533, 385);
            this.chart2.TabIndex = 18;
            this.chart2.Text = "chart2";
            // 
            // ucHieuSuat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.pnlTieuDe);
            this.Controls.Add(this.pnlDoanhThuTrungBinhPT);
            this.Controls.Add(this.pnlDanhGiaTrungBinh);
            this.Controls.Add(this.pnlTongPT);
            this.Name = "ucHieuSuat";
            this.Size = new System.Drawing.Size(1053, 770);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlDoanhThuTrungBinhPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenTongPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlDanhGiaTrungBinh;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenTienHoaHong;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTongPT;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenTongNguoiDung;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTongPT;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTieuDe;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTieuDe;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
    }
}
