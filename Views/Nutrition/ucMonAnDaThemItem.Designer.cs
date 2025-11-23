namespace HealthApp.Views.Nutrition
{
    partial class ucMonAnDaThemItem
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
            this.pnlMonAn = new Guna.UI2.WinForms.Guna2Panel();
            this.btnXoa = new Guna.UI2.WinForms.Guna2Button();
            this.lblFat = new System.Windows.Forms.Label();
            this.lblCarbs = new System.Windows.Forms.Label();
            this.lblProtein = new System.Windows.Forms.Label();
            this.lblCalories = new System.Windows.Forms.Label();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.lblLoaiBuaAn = new System.Windows.Forms.Label();
            this.lblTenMonAn = new System.Windows.Forms.Label();
            this.pnlMonAn.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMonAn
            // 
            this.pnlMonAn.BackColor = System.Drawing.Color.Transparent;
            this.pnlMonAn.BorderColor = System.Drawing.Color.Lime;
            this.pnlMonAn.BorderRadius = 15;
            this.pnlMonAn.BorderThickness = 1;
            this.pnlMonAn.Controls.Add(this.btnXoa);
            this.pnlMonAn.Controls.Add(this.lblFat);
            this.pnlMonAn.Controls.Add(this.lblCarbs);
            this.pnlMonAn.Controls.Add(this.lblProtein);
            this.pnlMonAn.Controls.Add(this.lblCalories);
            this.pnlMonAn.Controls.Add(this.lblSoLuong);
            this.pnlMonAn.Controls.Add(this.lblLoaiBuaAn);
            this.pnlMonAn.Controls.Add(this.lblTenMonAn);
            this.pnlMonAn.FillColor = System.Drawing.Color.White;
            this.pnlMonAn.Location = new System.Drawing.Point(0, 0);
            this.pnlMonAn.Name = "pnlMonAn";
            this.pnlMonAn.Size = new System.Drawing.Size(650, 100);
            this.pnlMonAn.TabIndex = 0;
            // 
            // btnXoa
            // 
            this.btnXoa.BorderColor = System.Drawing.Color.Red;
            this.btnXoa.BorderRadius = 8;
            this.btnXoa.BorderThickness = 1;
            this.btnXoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXoa.FillColor = System.Drawing.Color.White;
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXoa.ForeColor = System.Drawing.Color.Red;
            this.btnXoa.Location = new System.Drawing.Point(550, 10);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(80, 30);
            this.btnXoa.TabIndex = 7;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // lblFat
            // 
            this.lblFat.AutoSize = true;
            this.lblFat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFat.Location = new System.Drawing.Point(380, 70);
            this.lblFat.Name = "lblFat";
            this.lblFat.Size = new System.Drawing.Size(40, 20);
            this.lblFat.TabIndex = 6;
            this.lblFat.Text = "F: 0g";
            // 
            // lblCarbs
            // 
            this.lblCarbs.AutoSize = true;
            this.lblCarbs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCarbs.Location = new System.Drawing.Point(300, 70);
            this.lblCarbs.Name = "lblCarbs";
            this.lblCarbs.Size = new System.Drawing.Size(45, 20);
            this.lblCarbs.TabIndex = 5;
            this.lblCarbs.Text = "C: 0g";
            // 
            // lblProtein
            // 
            this.lblProtein.AutoSize = true;
            this.lblProtein.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProtein.Location = new System.Drawing.Point(220, 70);
            this.lblProtein.Name = "lblProtein";
            this.lblProtein.Size = new System.Drawing.Size(50, 20);
            this.lblProtein.TabIndex = 4;
            this.lblProtein.Text = "P: 0g";
            // 
            // lblCalories
            // 
            this.lblCalories.AutoSize = true;
            this.lblCalories.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCalories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblCalories.Location = new System.Drawing.Point(15, 70);
            this.lblCalories.Name = "lblCalories";
            this.lblCalories.Size = new System.Drawing.Size(60, 23);
            this.lblCalories.TabIndex = 3;
            this.lblCalories.Text = "0 kcal";
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSoLuong.Location = new System.Drawing.Point(220, 40);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(70, 20);
            this.lblSoLuong.TabIndex = 2;
            this.lblSoLuong.Text = "Số lượng:";
            // 
            // lblLoaiBuaAn
            // 
            this.lblLoaiBuaAn.AutoSize = true;
            this.lblLoaiBuaAn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLoaiBuaAn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblLoaiBuaAn.Location = new System.Drawing.Point(15, 40);
            this.lblLoaiBuaAn.Name = "lblLoaiBuaAn";
            this.lblLoaiBuaAn.Size = new System.Drawing.Size(50, 20);
            this.lblLoaiBuaAn.TabIndex = 1;
            this.lblLoaiBuaAn.Text = "Sáng";
            // 
            // lblTenMonAn
            // 
            this.lblTenMonAn.AutoSize = true;
            this.lblTenMonAn.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenMonAn.Location = new System.Drawing.Point(15, 10);
            this.lblTenMonAn.Name = "lblTenMonAn";
            this.lblTenMonAn.Size = new System.Drawing.Size(100, 25);
            this.lblTenMonAn.TabIndex = 0;
            this.lblTenMonAn.Text = "Tên món ăn";
            // 
            // ucMonAnDaThemItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlMonAn);
            this.Name = "ucMonAnDaThemItem";
            this.Size = new System.Drawing.Size(650, 100);
            this.pnlMonAn.ResumeLayout(false);
            this.pnlMonAn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMonAn;
        private System.Windows.Forms.Label lblTenMonAn;
        private System.Windows.Forms.Label lblLoaiBuaAn;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.Label lblCalories;
        private System.Windows.Forms.Label lblProtein;
        private System.Windows.Forms.Label lblCarbs;
        private System.Windows.Forms.Label lblFat;
        private Guna.UI2.WinForms.Guna2Button btnXoa;
    }
}

