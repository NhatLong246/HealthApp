namespace HealthApp.Views.Activity
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.siticoneActivityButton1 = new SiticoneNetFrameworkUI.SiticoneActivityButton();
            this.siticoneBluetoothButton1 = new SiticoneNetFrameworkUI.SiticoneBluetoothButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(100, 190);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(317, 233);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 1;
            // 
            // siticoneActivityButton1
            // 
            this.siticoneActivityButton1.ActivityDuration = 2000;
            this.siticoneActivityButton1.ActivityIndicatorColor = System.Drawing.Color.White;
            this.siticoneActivityButton1.ActivityIndicatorSize = 4;
            this.siticoneActivityButton1.ActivityIndicatorSpeed = 100;
            this.siticoneActivityButton1.ActivityText = "Processing...";
            this.siticoneActivityButton1.AnimationEasing = SiticoneNetFrameworkUI.SiticoneActivityButton.AnimationEasingType.EaseOutQuad;
            this.siticoneActivityButton1.BackColor = System.Drawing.Color.Transparent;
            this.siticoneActivityButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(50)))));
            this.siticoneActivityButton1.BorderWidth = 2;
            this.siticoneActivityButton1.CornerRadiusBottomLeft = 5;
            this.siticoneActivityButton1.CornerRadiusBottomRight = 5;
            this.siticoneActivityButton1.CornerRadiusTopLeft = 5;
            this.siticoneActivityButton1.CornerRadiusTopRight = 5;
            this.siticoneActivityButton1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.siticoneActivityButton1.Elevation = 2F;
            this.siticoneActivityButton1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.siticoneActivityButton1.HoverAnimationDuration = 200;
            this.siticoneActivityButton1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(165)))), ((int)(((byte)(245)))));
            this.siticoneActivityButton1.Location = new System.Drawing.Point(349, 106);
            this.siticoneActivityButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.siticoneActivityButton1.Name = "siticoneActivityButton1";
            this.siticoneActivityButton1.PressAnimationDuration = 150;
            this.siticoneActivityButton1.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.siticoneActivityButton1.PressedElevation = 1F;
            this.siticoneActivityButton1.RippleColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.siticoneActivityButton1.RippleDuration = 1800;
            this.siticoneActivityButton1.RippleSize = 5;
            this.siticoneActivityButton1.ShowActivityText = true;
            this.siticoneActivityButton1.Size = new System.Drawing.Size(253, 75);
            this.siticoneActivityButton1.TabIndex = 2;
            this.siticoneActivityButton1.Text = "siticoneActivityButton1";
            this.siticoneActivityButton1.TextColor = System.Drawing.Color.White;
            this.siticoneActivityButton1.UseAnimation = true;
            this.siticoneActivityButton1.UseElevation = false;
            this.siticoneActivityButton1.UseRippleEffect = true;
            this.siticoneActivityButton1.Click += new System.EventHandler(this.siticoneActivityButton1_Click);
            // 
            // siticoneBluetoothButton1
            // 
            this.siticoneBluetoothButton1.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.siticoneBluetoothButton1.BackColor = System.Drawing.Color.Transparent;
            this.siticoneBluetoothButton1.BorderColor = System.Drawing.Color.Transparent;
            this.siticoneBluetoothButton1.ConnectingSignalColor = System.Drawing.Color.Gray;
            this.siticoneBluetoothButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.siticoneBluetoothButton1.ErrorColor = System.Drawing.Color.Red;
            this.siticoneBluetoothButton1.HighSignalColor = System.Drawing.Color.Green;
            this.siticoneBluetoothButton1.IdleColor = System.Drawing.Color.Gray;
            this.siticoneBluetoothButton1.InactiveColor = System.Drawing.Color.LightGray;
            this.siticoneBluetoothButton1.Location = new System.Drawing.Point(201, 343);
            this.siticoneBluetoothButton1.LockColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(205)))), ((int)(((byte)(50)))));
            this.siticoneBluetoothButton1.LowSignalColor = System.Drawing.Color.Red;
            this.siticoneBluetoothButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.siticoneBluetoothButton1.MediumSignalColor = System.Drawing.Color.Orange;
            this.siticoneBluetoothButton1.Name = "siticoneBluetoothButton1";
            this.siticoneBluetoothButton1.OffLineColor = System.Drawing.Color.Red;
            this.siticoneBluetoothButton1.RippleColor = System.Drawing.Color.White;
            this.siticoneBluetoothButton1.Size = new System.Drawing.Size(53, 49);
            this.siticoneBluetoothButton1.State = SiticoneNetFrameworkUI.BluetoothState.Idle;
            this.siticoneBluetoothButton1.TabIndex = 3;
            this.siticoneBluetoothButton1.Text = "siticoneBluetoothButton1";
            this.siticoneBluetoothButton1.WarningColor = System.Drawing.Color.Orange;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.siticoneBluetoothButton1);
            this.Controls.Add(this.siticoneActivityButton1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private SiticoneNetFrameworkUI.SiticoneActivityButton siticoneActivityButton1;
        private SiticoneNetFrameworkUI.SiticoneBluetoothButton siticoneBluetoothButton1;
    }
}

