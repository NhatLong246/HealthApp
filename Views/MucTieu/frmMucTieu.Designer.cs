namespace HealthApp.Views.MucTieu
{
    partial class frmMucTieu
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
            this.pnlMucTieu = new Guna.UI2.WinForms.Guna2Panel();
            this.SuspendLayout();
            // 
            // pnlMucTieu
            // 
            this.pnlMucTieu.AutoScroll = true;
            this.pnlMucTieu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.pnlMucTieu.Location = new System.Drawing.Point(1, 69);
            this.pnlMucTieu.Name = "pnlMucTieu";
            this.pnlMucTieu.Size = new System.Drawing.Size(1345, 598);
            this.pnlMucTieu.TabIndex = 0;
            this.pnlMucTieu.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlMucTieu_Paint);
            // 
            // frmMucTieu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1348, 739);
            this.Controls.Add(this.pnlMucTieu);
            this.Name = "frmMucTieu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMucTieu";
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMucTieu;
    }
}