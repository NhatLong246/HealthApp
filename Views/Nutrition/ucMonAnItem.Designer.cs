namespace HealthApp.Views.Nutrition
{
    partial class ucMonAnItem
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
            this.lblFat = new System.Windows.Forms.Label();
            this.lblCarbs = new System.Windows.Forms.Label();
            this.lblProtein = new System.Windows.Forms.Label();
            this.lblCalories = new System.Windows.Forms.Label();
            this.lblTenMonAn = new System.Windows.Forms.Label();
            this.pnlMonAn.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMonAn
            // 
            this.pnlMonAn.BackColor = System.Drawing.Color.Transparent;
            this.pnlMonAn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(217)))), ((int)(((byte)(195)))));
            this.pnlMonAn.BorderRadius = 25;
            this.pnlMonAn.BorderThickness = 2;
            this.pnlMonAn.Controls.Add(this.lblFat);
            this.pnlMonAn.Controls.Add(this.lblCarbs);
            this.pnlMonAn.Controls.Add(this.lblProtein);
            this.pnlMonAn.Controls.Add(this.lblCalories);
            this.pnlMonAn.Controls.Add(this.lblTenMonAn);
            this.pnlMonAn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlMonAn.FillColor = System.Drawing.Color.White;
            this.pnlMonAn.Location = new System.Drawing.Point(0, 0);
            this.pnlMonAn.Name = "pnlMonAn";
            this.pnlMonAn.Size = new System.Drawing.Size(318, 80);
            this.pnlMonAn.TabIndex = 0;
            this.pnlMonAn.Click += new System.EventHandler(this.pnlMonAn_Click);
            // 
            // lblFat
            // 
            this.lblFat.AutoSize = true;
            this.lblFat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFat.Location = new System.Drawing.Point(245, 50);
            this.lblFat.Name = "lblFat";
            this.lblFat.Size = new System.Drawing.Size(43, 20);
            this.lblFat.TabIndex = 4;
            this.lblFat.Text = "F: 0g";
            this.lblFat.Click += new System.EventHandler(this.pnlMonAn_Click);
            // 
            // lblCarbs
            // 
            this.lblCarbs.AutoSize = true;
            this.lblCarbs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCarbs.Location = new System.Drawing.Point(160, 50);
            this.lblCarbs.Name = "lblCarbs";
            this.lblCarbs.Size = new System.Drawing.Size(44, 20);
            this.lblCarbs.TabIndex = 3;
            this.lblCarbs.Text = "C: 0g";
            this.lblCarbs.Click += new System.EventHandler(this.pnlMonAn_Click);
            // 
            // lblProtein
            // 
            this.lblProtein.AutoSize = true;
            this.lblProtein.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProtein.Location = new System.Drawing.Point(95, 50);
            this.lblProtein.Name = "lblProtein";
            this.lblProtein.Size = new System.Drawing.Size(44, 20);
            this.lblProtein.TabIndex = 2;
            this.lblProtein.Text = "P: 0g";
            this.lblProtein.Click += new System.EventHandler(this.pnlMonAn_Click);
            // 
            // lblCalories
            // 
            this.lblCalories.AutoSize = true;
            this.lblCalories.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCalories.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblCalories.Location = new System.Drawing.Point(15, 50);
            this.lblCalories.Name = "lblCalories";
            this.lblCalories.Size = new System.Drawing.Size(54, 23);
            this.lblCalories.TabIndex = 1;
            this.lblCalories.Text = "0 kcal";
            this.lblCalories.Click += new System.EventHandler(this.pnlMonAn_Click);
            // 
            // lblTenMonAn
            // 
            this.lblTenMonAn.AutoSize = true;
            this.lblTenMonAn.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenMonAn.Location = new System.Drawing.Point(15, 15);
            this.lblTenMonAn.Name = "lblTenMonAn";
            this.lblTenMonAn.Size = new System.Drawing.Size(109, 25);
            this.lblTenMonAn.TabIndex = 0;
            this.lblTenMonAn.Text = "Tên món ăn";
            this.lblTenMonAn.Click += new System.EventHandler(this.lblTenMonAn_Click);
            // 
            // ucMonAnItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlMonAn);
            this.Name = "ucMonAnItem";
            this.Size = new System.Drawing.Size(480, 80);
            this.pnlMonAn.ResumeLayout(false);
            this.pnlMonAn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMonAn;
        private System.Windows.Forms.Label lblTenMonAn;
        private System.Windows.Forms.Label lblCalories;
        private System.Windows.Forms.Label lblProtein;
        private System.Windows.Forms.Label lblCarbs;
        private System.Windows.Forms.Label lblFat;
    }
}

