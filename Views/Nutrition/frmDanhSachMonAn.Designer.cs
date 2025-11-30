namespace HealthApp.Views.Nutrition
{
    partial class frmDanhSachMonAn
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
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlSearch = new Guna.UI2.WinForms.Guna2Panel();
            this.txtTimKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTimKiem = new System.Windows.Forms.Label();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlScroll.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.pnlMain.BorderRadius = 20;
            this.pnlMain.Controls.Add(this.pnlScroll);
            this.pnlMain.Controls.Add(this.pnlSearch);
            this.pnlMain.Controls.Add(this.pnlHeader);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.White;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(900, 700);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlScroll
            // 
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.BackColor = System.Drawing.Color.White;
            this.pnlScroll.Controls.Add(this.flowLayoutPanel);
            this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroll.Location = new System.Drawing.Point(0, 170);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Padding = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.pnlScroll.Size = new System.Drawing.Size(900, 530);
            this.pnlScroll.TabIndex = 2;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(20, 10);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.flowLayoutPanel.Size = new System.Drawing.Size(860, 10);
            this.flowLayoutPanel.TabIndex = 0;
            this.flowLayoutPanel.WrapContents = false;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.Controls.Add(this.txtTimKiem);
            this.pnlSearch.Controls.Add(this.lblTimKiem);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 80);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(30, 15, 30, 15);
            this.pnlSearch.Size = new System.Drawing.Size(900, 90);
            this.pnlSearch.TabIndex = 1;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(217)))), ((int)(((byte)(195)))));
            this.txtTimKiem.BorderRadius = 10;
            this.txtTimKiem.BorderThickness = 2;
            this.txtTimKiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTimKiem.DefaultText = "";
            this.txtTimKiem.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTimKiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTimKiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTimKiem.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTimKiem.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(217)))), ((int)(((byte)(195)))));
            this.txtTimKiem.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.txtTimKiem.ForeColor = System.Drawing.Color.Black;
            this.txtTimKiem.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(217)))), ((int)(((byte)(195)))));
            this.txtTimKiem.Location = new System.Drawing.Point(30, 38);
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.PlaceholderText = "Nhập tên món ăn để tìm kiếm...";
            this.txtTimKiem.SelectedText = "";
            this.txtTimKiem.Size = new System.Drawing.Size(840, 40);
            this.txtTimKiem.TabIndex = 1;
            this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged);
            // 
            // lblTimKiem
            // 
            this.lblTimKiem.AutoSize = true;
            this.lblTimKiem.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTimKiem.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimKiem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.lblTimKiem.Location = new System.Drawing.Point(30, 15);
            this.lblTimKiem.Name = "lblTimKiem";
            this.lblTimKiem.Size = new System.Drawing.Size(84, 23);
            this.lblTimKiem.TabIndex = 0;
            this.lblTimKiem.Text = "Tìm kiếm:";
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(217)))), ((int)(((byte)(195)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTieuDe);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(900, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BorderColor = System.Drawing.Color.White;
            this.btnClose.BorderRadius = 5;
            this.btnClose.BorderThickness = 2;
            this.btnClose.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClose.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(850, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "✕";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Segoe UI Semibold", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.ForeColor = System.Drawing.Color.White;
            this.lblTieuDe.Location = new System.Drawing.Point(30, 25);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(261, 38);
            this.lblTieuDe.TabIndex = 0;
            this.lblTieuDe.Text = "Danh Sách Món Ăn";
            // 
            // frmDanhSachMonAn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDanhSachMonAn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh Sách Món Ăn";
            this.pnlMain.ResumeLayout(false);
            this.pnlScroll.ResumeLayout(false);
            this.pnlScroll.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private System.Windows.Forms.Label lblTieuDe;
        private Guna.UI2.WinForms.Guna2Panel pnlSearch;
        private Guna.UI2.WinForms.Guna2TextBox txtTimKiem;
        private System.Windows.Forms.Label lblTimKiem;
        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    }
}
