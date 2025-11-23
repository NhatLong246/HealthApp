namespace HealthApp.Views.Nutrition
{
    partial class ucMonAnDeXuat
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
            this.lblFat = new System.Windows.Forms.Label();
            this.lblCarbs = new System.Windows.Forms.Label();
            this.lblProtein = new System.Windows.Forms.Label();
            this.lblCalories = new System.Windows.Forms.Label();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.lblLoaiBuaAn = new System.Windows.Forms.Label();
            this.lblTenMonAn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFat
            // 
            this.lblFat.AutoSize = true;
            this.lblFat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFat.Location = new System.Drawing.Point(386, 72);
            this.lblFat.Name = "lblFat";
            this.lblFat.Size = new System.Drawing.Size(43, 20);
            this.lblFat.TabIndex = 13;
            this.lblFat.Text = "F: 0g";
            // 
            // lblCarbs
            // 
            this.lblCarbs.AutoSize = true;
            this.lblCarbs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCarbs.Location = new System.Drawing.Point(306, 72);
            this.lblCarbs.Name = "lblCarbs";
            this.lblCarbs.Size = new System.Drawing.Size(44, 20);
            this.lblCarbs.TabIndex = 12;
            this.lblCarbs.Text = "C: 0g";
            // 
            // lblProtein
            // 
            this.lblProtein.AutoSize = true;
            this.lblProtein.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProtein.Location = new System.Drawing.Point(226, 72);
            this.lblProtein.Name = "lblProtein";
            this.lblProtein.Size = new System.Drawing.Size(44, 20);
            this.lblProtein.TabIndex = 11;
            this.lblProtein.Text = "P: 0g";
            // 
            // lblCalories
            // 
            this.lblCalories.AutoSize = true;
            this.lblCalories.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCalories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblCalories.Location = new System.Drawing.Point(21, 72);
            this.lblCalories.Name = "lblCalories";
            this.lblCalories.Size = new System.Drawing.Size(54, 23);
            this.lblCalories.TabIndex = 10;
            this.lblCalories.Text = "0 kcal";
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSoLuong.Location = new System.Drawing.Point(226, 42);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(72, 20);
            this.lblSoLuong.TabIndex = 9;
            this.lblSoLuong.Text = "Số lượng:";
            // 
            // lblLoaiBuaAn
            // 
            this.lblLoaiBuaAn.AutoSize = true;
            this.lblLoaiBuaAn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLoaiBuaAn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblLoaiBuaAn.Location = new System.Drawing.Point(21, 42);
            this.lblLoaiBuaAn.Name = "lblLoaiBuaAn";
            this.lblLoaiBuaAn.Size = new System.Drawing.Size(43, 20);
            this.lblLoaiBuaAn.TabIndex = 8;
            this.lblLoaiBuaAn.Text = "Sáng";
            // 
            // lblTenMonAn
            // 
            this.lblTenMonAn.AutoSize = true;
            this.lblTenMonAn.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenMonAn.Location = new System.Drawing.Point(21, 12);
            this.lblTenMonAn.Name = "lblTenMonAn";
            this.lblTenMonAn.Size = new System.Drawing.Size(111, 25);
            this.lblTenMonAn.TabIndex = 7;
            this.lblTenMonAn.Text = "Tên món ăn";
            // 
            // ucMonAnDeXuat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFat);
            this.Controls.Add(this.lblCarbs);
            this.Controls.Add(this.lblProtein);
            this.Controls.Add(this.lblCalories);
            this.Controls.Add(this.lblSoLuong);
            this.Controls.Add(this.lblLoaiBuaAn);
            this.Controls.Add(this.lblTenMonAn);
            this.Name = "ucMonAnDeXuat";
            this.Size = new System.Drawing.Size(452, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFat;
        private System.Windows.Forms.Label lblCarbs;
        private System.Windows.Forms.Label lblProtein;
        private System.Windows.Forms.Label lblCalories;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.Label lblLoaiBuaAn;
        private System.Windows.Forms.Label lblTenMonAn;
    }
}
