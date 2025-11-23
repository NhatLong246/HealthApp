namespace HealthApp.Views.Auth
{
    partial class ForgotPasswordForm
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
            this.pnlTitle = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.picIcon = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.lbTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbTitleMain = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbEmail = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtSendOTP = new Guna.UI2.WinForms.Guna2GradientButton();
            this.lnkBackLogin = new System.Windows.Forms.LinkLabel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.picIcon);
            this.pnlTitle.Controls.Add(this.lbTitle);
            this.pnlTitle.Controls.Add(this.lbTitleMain);
            this.pnlTitle.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(203)))), ((int)(((byte)(160)))));
            this.pnlTitle.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(149)))), ((int)(((byte)(141)))));
            this.pnlTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pnlTitle.Location = new System.Drawing.Point(2, 12);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(443, 152);
            this.pnlTitle.TabIndex = 2;
            // 
            // picIcon
            // 
            this.picIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(203)))), ((int)(((byte)(160)))));
            this.picIcon.ImageRotate = 0F;
            this.picIcon.Location = new System.Drawing.Point(41, 25);
            this.picIcon.Name = "picIcon";
            this.picIcon.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.picIcon.Size = new System.Drawing.Size(78, 72);
            this.picIcon.TabIndex = 3;
            this.picIcon.TabStop = false;
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(41, 112);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(294, 22);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Nhập email để nhận mã OTP khôi phục";
            // 
            // lbTitleMain
            // 
            this.lbTitleMain.BackColor = System.Drawing.Color.Transparent;
            this.lbTitleMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitleMain.ForeColor = System.Drawing.SystemColors.Control;
            this.lbTitleMain.Location = new System.Drawing.Point(143, 43);
            this.lbTitleMain.Name = "lbTitleMain";
            this.lbTitleMain.Size = new System.Drawing.Size(223, 38);
            this.lbTitleMain.TabIndex = 1;
            this.lbTitleMain.Text = "Quên Mật Khẩu";
            // 
            // lbEmail
            // 
            this.lbEmail.BackColor = System.Drawing.Color.White;
            this.lbEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEmail.ForeColor = System.Drawing.Color.Black;
            this.lbEmail.Location = new System.Drawing.Point(27, 198);
            this.lbEmail.Name = "lbEmail";
            this.lbEmail.Size = new System.Drawing.Size(105, 22);
            this.lbEmail.TabIndex = 27;
            this.lbEmail.Text = "Địa Chỉ Email";
            // 
            // txtEmail
            // 
            this.txtEmail.BorderRadius = 10;
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.DefaultText = "";
            this.txtEmail.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtEmail.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtEmail.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtEmail.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtEmail.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEmail.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtEmail.Location = new System.Drawing.Point(27, 228);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(5);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PlaceholderText = "";
            this.txtEmail.SelectedText = "";
            this.txtEmail.Size = new System.Drawing.Size(388, 45);
            this.txtEmail.TabIndex = 26;
            // 
            // txtSendOTP
            // 
            this.txtSendOTP.BorderRadius = 10;
            this.txtSendOTP.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(180)))), ((int)(((byte)(152)))));
            this.txtSendOTP.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(140)))), ((int)(((byte)(156)))));
            this.txtSendOTP.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSendOTP.ForeColor = System.Drawing.Color.White;
            this.txtSendOTP.Location = new System.Drawing.Point(27, 301);
            this.txtSendOTP.Name = "txtSendOTP";
            this.txtSendOTP.Size = new System.Drawing.Size(388, 49);
            this.txtSendOTP.TabIndex = 29;
            this.txtSendOTP.Text = "Gửi mã OTP";
            // 
            // lnkBackLogin
            // 
            this.lnkBackLogin.AutoSize = true;
            this.lnkBackLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkBackLogin.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkBackLogin.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(155)))), ((int)(((byte)(111)))));
            this.lnkBackLogin.Location = new System.Drawing.Point(142, 387);
            this.lnkBackLogin.Name = "lnkBackLogin";
            this.lnkBackLogin.Size = new System.Drawing.Size(137, 18);
            this.lnkBackLogin.TabIndex = 31;
            this.lnkBackLogin.TabStop = true;
            this.lnkBackLogin.Text = " Quay lại đăng nhập";
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.Red;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(138, 198);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(9, 22);
            this.guna2HtmlLabel3.TabIndex = 28;
            this.guna2HtmlLabel3.Text = "*";
            // 
            // ForgotPasswordForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(446, 450);
            this.Controls.Add(this.lnkBackLogin);
            this.Controls.Add(this.txtSendOTP);
            this.Controls.Add(this.guna2HtmlLabel3);
            this.Controls.Add(this.lbEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.pnlTitle);
            this.Name = "ForgotPasswordForm";
            this.Text = "ForgotPasswordForm";
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel pnlTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTitleMain;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picIcon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbEmail;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2GradientButton txtSendOTP;
        private System.Windows.Forms.LinkLabel lnkBackLogin;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
    }
}