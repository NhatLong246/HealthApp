namespace HealthApp.Views.Dashboard
{
    partial class frmDashBoard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnSettings = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.Back = new Guna.UI2.WinForms.Guna2ImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemDangXuat = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemThanhToanPT = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.ptrDangKyLamPT = new System.Windows.Forms.PictureBox();
            this.picAnUong = new System.Windows.Forms.PictureBox();
            this.picHome = new System.Windows.Forms.PictureBox();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSettings)).BeginInit();
            this.contextMenuUser.SuspendLayout();
            this.guna2Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptrDangKyLamPT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAnUong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHome)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.White;
            this.guna2Panel1.Controls.Add(this.btnSettings);
            this.guna2Panel1.Controls.Add(this.Back);
            this.guna2Panel1.Controls.Add(this.label1);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1348, 110);
            this.guna2Panel1.TabIndex = 0;
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FillColor = System.Drawing.Color.Transparent;
            this.btnSettings.Image = global::HealthApp.Properties.Resources.settingIcon;
            this.btnSettings.ImageRotate = 0F;
            this.btnSettings.Location = new System.Drawing.Point(1282, 16);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnSettings.Size = new System.Drawing.Size(48, 48);
            this.btnSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSettings.TabIndex = 3;
            this.btnSettings.TabStop = false;
            // 
            // Back
            // 
            this.Back.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.Back.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Back.HoverState.ImageSize = new System.Drawing.Size(30, 30);
            this.Back.Image = global::HealthApp.Properties.Resources.backIcon;
            this.Back.ImageOffset = new System.Drawing.Point(0, 0);
            this.Back.ImageRotate = 0F;
            this.Back.ImageSize = new System.Drawing.Size(24, 24);
            this.Back.Location = new System.Drawing.Point(3, 3);
            this.Back.Name = "Back";
            this.Back.PressedState.ImageSize = new System.Drawing.Size(26, 26);
            this.Back.Size = new System.Drawing.Size(56, 72);
            this.Back.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 16.2F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(82, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "HealthApp";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // contextMenuUser
            // 
            this.contextMenuUser.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemDangXuat,
            this.menuItemThanhToanPT});
            this.contextMenuUser.Name = "contextMenuUser";
            this.contextMenuUser.Size = new System.Drawing.Size(173, 52);
            // 
            // menuItemDangXuat
            // 
            this.menuItemDangXuat.Name = "menuItemDangXuat";
            this.menuItemDangXuat.Size = new System.Drawing.Size(172, 24);
            this.menuItemDangXuat.Text = "Đăng xuất";
            this.menuItemDangXuat.Click += new System.EventHandler(this.MenuItemDangXuat_Click);
            // 
            // menuItemThanhToanPT
            // 
            this.menuItemThanhToanPT.Name = "menuItemThanhToanPT";
            this.menuItemThanhToanPT.Size = new System.Drawing.Size(172, 24);
            this.menuItemThanhToanPT.Text = "Thanh toán PT";
            this.menuItemThanhToanPT.Click += new System.EventHandler(this.MenuItemThanhToanPT_Click);
            // 
            // pnlBody
            // 
            this.pnlBody.AutoScroll = true;
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 110);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(1348, 539);
            this.pnlBody.TabIndex = 1;
            this.pnlBody.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBody_Paint);
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BackColor = System.Drawing.Color.White;
            this.guna2Panel2.Controls.Add(this.ptrDangKyLamPT);
            this.guna2Panel2.Controls.Add(this.picAnUong);
            this.guna2Panel2.Controls.Add(this.picHome);
            this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel2.Location = new System.Drawing.Point(0, 649);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(1348, 90);
            this.guna2Panel2.TabIndex = 2;
            // 
            // ptrDangKyLamPT
            // 
            this.ptrDangKyLamPT.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptrDangKyLamPT.Location = new System.Drawing.Point(982, 15);
            this.ptrDangKyLamPT.Name = "ptrDangKyLamPT";
            this.ptrDangKyLamPT.Size = new System.Drawing.Size(80, 60);
            this.ptrDangKyLamPT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptrDangKyLamPT.TabIndex = 2;
            this.ptrDangKyLamPT.TabStop = false;
            // 
            // picAnUong
            // 
            this.picAnUong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAnUong.Location = new System.Drawing.Point(642, 15);
            this.picAnUong.Name = "picAnUong";
            this.picAnUong.Size = new System.Drawing.Size(80, 60);
            this.picAnUong.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAnUong.TabIndex = 1;
            this.picAnUong.TabStop = false;
            // 
            // picHome
            // 
            this.picHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picHome.Location = new System.Drawing.Point(308, 15);
            this.picHome.Name = "picHome";
            this.picHome.Size = new System.Drawing.Size(80, 60);
            this.picHome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picHome.TabIndex = 0;
            this.picHome.TabStop = false;
            // 
            // frmDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 739);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.guna2Panel2);
            this.Controls.Add(this.guna2Panel1);
            this.Name = "frmDashBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmDashBoard";
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSettings)).EndInit();
            this.contextMenuUser.ResumeLayout(false);
            this.guna2Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ptrDangKyLamPT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAnUong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHome)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlBody;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private System.Windows.Forms.PictureBox picHome;
        private System.Windows.Forms.PictureBox picAnUong;
        private System.Windows.Forms.PictureBox ptrDangKyLamPT;
        private Guna.UI2.WinForms.Guna2ImageButton Back;
        private Guna.UI2.WinForms.Guna2CirclePictureBox btnSettings;
        private System.Windows.Forms.ContextMenuStrip contextMenuUser;
        private System.Windows.Forms.ToolStripMenuItem menuItemDangXuat;
        private System.Windows.Forms.ToolStripMenuItem menuItemThanhToanPT;
    }
}