namespace HealthApp.Views.LeaderBoard
{
    partial class LeaderBoardForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTitle = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbTitleMain = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnAllTime = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.btnToday = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnWeek = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnMonth = new Guna.UI2.WinForms.Guna2GradientButton();
            this.shpnData = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.dgvStatistic = new Guna.UI2.WinForms.Guna2DataGridView();
            this.colRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSession = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guna2ShadowPanel3 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.pnSession = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbSession = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenSession = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnTotalTime = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbTotalTime = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenTotalTime = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnPoints = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbPoints = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenPoints = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnRank = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbRank = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbGenRank = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbStatistic = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlTitle.SuspendLayout();
            this.guna2ShadowPanel1.SuspendLayout();
            this.shpnData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistic)).BeginInit();
            this.guna2ShadowPanel3.SuspendLayout();
            this.pnSession.SuspendLayout();
            this.pnTotalTime.SuspendLayout();
            this.pnPoints.SuspendLayout();
            this.pnRank.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lbTitle);
            this.pnlTitle.Controls.Add(this.lbTitleMain);
            this.pnlTitle.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(203)))), ((int)(((byte)(160)))));
            this.pnlTitle.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(149)))), ((int)(((byte)(141)))));
            this.pnlTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.ShadowDecoration.Parent = this.pnlTitle;
            this.pnlTitle.Size = new System.Drawing.Size(1345, 100);
            this.pnlTitle.TabIndex = 3;
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(463, 58);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(400, 22);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Cùng xem ai dẫn đầu trong  trong cộng đồng luyện tập";
            // 
            // lbTitleMain
            // 
            this.lbTitleMain.BackColor = System.Drawing.Color.Transparent;
            this.lbTitleMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitleMain.ForeColor = System.Drawing.Color.White;
            this.lbTitleMain.Location = new System.Drawing.Point(463, 12);
            this.lbTitleMain.Name = "lbTitleMain";
            this.lbTitleMain.Size = new System.Drawing.Size(396, 40);
            this.lbTitleMain.TabIndex = 1;
            this.lbTitleMain.Text = "Bảng xếp hạng thành tích";
            // 
            // btnAllTime
            // 
            this.btnAllTime.BorderRadius = 15;
            this.btnAllTime.CheckedState.Parent = this.btnAllTime;
            this.btnAllTime.CustomImages.Parent = this.btnAllTime;
            this.btnAllTime.FillColor = System.Drawing.Color.Silver;
            this.btnAllTime.FillColor2 = System.Drawing.Color.Silver;
            this.btnAllTime.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAllTime.ForeColor = System.Drawing.Color.Black;
            this.btnAllTime.HoverState.Parent = this.btnAllTime;
            this.btnAllTime.Location = new System.Drawing.Point(226, 20);
            this.btnAllTime.Name = "btnAllTime";
            this.btnAllTime.ShadowDecoration.Parent = this.btnAllTime;
            this.btnAllTime.Size = new System.Drawing.Size(130, 45);
            this.btnAllTime.TabIndex = 0;
            this.btnAllTime.Text = "Tất cả";
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.btnToday);
            this.guna2ShadowPanel1.Controls.Add(this.btnWeek);
            this.guna2ShadowPanel1.Controls.Add(this.btnMonth);
            this.guna2ShadowPanel1.Controls.Add(this.btnAllTime);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(0, 106);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(1336, 87);
            this.guna2ShadowPanel1.TabIndex = 4;
            // 
            // btnToday
            // 
            this.btnToday.BorderRadius = 15;
            this.btnToday.CheckedState.Parent = this.btnToday;
            this.btnToday.CustomImages.Parent = this.btnToday;
            this.btnToday.FillColor = System.Drawing.Color.Silver;
            this.btnToday.FillColor2 = System.Drawing.Color.Silver;
            this.btnToday.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToday.ForeColor = System.Drawing.Color.Black;
            this.btnToday.HoverState.Parent = this.btnToday;
            this.btnToday.Location = new System.Drawing.Point(839, 22);
            this.btnToday.Name = "btnToday";
            this.btnToday.ShadowDecoration.Parent = this.btnToday;
            this.btnToday.Size = new System.Drawing.Size(130, 45);
            this.btnToday.TabIndex = 3;
            this.btnToday.Text = "Hôm nay";
            // 
            // btnWeek
            // 
            this.btnWeek.BorderRadius = 15;
            this.btnWeek.CheckedState.Parent = this.btnWeek;
            this.btnWeek.CustomImages.Parent = this.btnWeek;
            this.btnWeek.FillColor = System.Drawing.Color.Silver;
            this.btnWeek.FillColor2 = System.Drawing.Color.Silver;
            this.btnWeek.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnWeek.ForeColor = System.Drawing.Color.Black;
            this.btnWeek.HoverState.Parent = this.btnWeek;
            this.btnWeek.Location = new System.Drawing.Point(635, 22);
            this.btnWeek.Name = "btnWeek";
            this.btnWeek.ShadowDecoration.Parent = this.btnWeek;
            this.btnWeek.Size = new System.Drawing.Size(130, 45);
            this.btnWeek.TabIndex = 2;
            this.btnWeek.Text = "Tuần này";
            // 
            // btnMonth
            // 
            this.btnMonth.BorderRadius = 15;
            this.btnMonth.CheckedState.Parent = this.btnMonth;
            this.btnMonth.CustomImages.Parent = this.btnMonth;
            this.btnMonth.FillColor = System.Drawing.Color.Silver;
            this.btnMonth.FillColor2 = System.Drawing.Color.Silver;
            this.btnMonth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMonth.ForeColor = System.Drawing.Color.Black;
            this.btnMonth.HoverState.Parent = this.btnMonth;
            this.btnMonth.Location = new System.Drawing.Point(433, 22);
            this.btnMonth.Name = "btnMonth";
            this.btnMonth.ShadowDecoration.Parent = this.btnMonth;
            this.btnMonth.Size = new System.Drawing.Size(130, 45);
            this.btnMonth.TabIndex = 1;
            this.btnMonth.Text = "Tháng này";
            // 
            // shpnData
            // 
            this.shpnData.BackColor = System.Drawing.Color.Transparent;
            this.shpnData.Controls.Add(this.dgvStatistic);
            this.shpnData.FillColor = System.Drawing.Color.White;
            this.shpnData.Location = new System.Drawing.Point(0, 199);
            this.shpnData.Name = "shpnData";
            this.shpnData.ShadowColor = System.Drawing.Color.Black;
            this.shpnData.Size = new System.Drawing.Size(1336, 292);
            this.shpnData.TabIndex = 5;
            // 
            // dgvStatistic
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvStatistic.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvStatistic.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatistic.BackgroundColor = System.Drawing.Color.White;
            this.dgvStatistic.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvStatistic.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvStatistic.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStatistic.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvStatistic.ColumnHeadersHeight = 27;
            this.dgvStatistic.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRank,
            this.ColUser,
            this.colPoints,
            this.colTotalTime,
            this.colSession});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStatistic.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvStatistic.EnableHeadersVisualStyles = false;
            this.dgvStatistic.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvStatistic.Location = new System.Drawing.Point(3, 6);
            this.dgvStatistic.Name = "dgvStatistic";
            this.dgvStatistic.RowHeadersVisible = false;
            this.dgvStatistic.RowHeadersWidth = 51;
            this.dgvStatistic.RowTemplate.Height = 24;
            this.dgvStatistic.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStatistic.Size = new System.Drawing.Size(1333, 286);
            this.dgvStatistic.TabIndex = 0;
            this.dgvStatistic.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            this.dgvStatistic.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvStatistic.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvStatistic.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvStatistic.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvStatistic.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvStatistic.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvStatistic.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvStatistic.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvStatistic.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvStatistic.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.dgvStatistic.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvStatistic.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvStatistic.ThemeStyle.HeaderStyle.Height = 27;
            this.dgvStatistic.ThemeStyle.ReadOnly = false;
            this.dgvStatistic.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvStatistic.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvStatistic.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.dgvStatistic.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvStatistic.ThemeStyle.RowsStyle.Height = 24;
            this.dgvStatistic.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvStatistic.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // colRank
            // 
            this.colRank.HeaderText = "Xếp hạng";
            this.colRank.MinimumWidth = 6;
            this.colRank.Name = "colRank";
            // 
            // ColUser
            // 
            this.ColUser.HeaderText = "Người Dùng";
            this.ColUser.MinimumWidth = 6;
            this.ColUser.Name = "ColUser";
            // 
            // colPoints
            // 
            this.colPoints.HeaderText = "Điểm thành tích";
            this.colPoints.MinimumWidth = 6;
            this.colPoints.Name = "colPoints";
            // 
            // colTotalTime
            // 
            this.colTotalTime.HeaderText = "Tổng thời gian";
            this.colTotalTime.MinimumWidth = 6;
            this.colTotalTime.Name = "colTotalTime";
            // 
            // colSession
            // 
            this.colSession.HeaderText = "Số buổi tập";
            this.colSession.MinimumWidth = 6;
            this.colSession.Name = "colSession";
            // 
            // guna2ShadowPanel3
            // 
            this.guna2ShadowPanel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel3.Controls.Add(this.pnSession);
            this.guna2ShadowPanel3.Controls.Add(this.pnTotalTime);
            this.guna2ShadowPanel3.Controls.Add(this.pnPoints);
            this.guna2ShadowPanel3.Controls.Add(this.pnRank);
            this.guna2ShadowPanel3.Controls.Add(this.lbStatistic);
            this.guna2ShadowPanel3.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel3.Location = new System.Drawing.Point(0, 497);
            this.guna2ShadowPanel3.Name = "guna2ShadowPanel3";
            this.guna2ShadowPanel3.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel3.Size = new System.Drawing.Size(1336, 189);
            this.guna2ShadowPanel3.TabIndex = 6;
            // 
            // pnSession
            // 
            this.pnSession.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(237)))));
            this.pnSession.BorderRadius = 20;
            this.pnSession.Controls.Add(this.lbSession);
            this.pnSession.Controls.Add(this.lbGenSession);
            this.pnSession.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(237)))));
            this.pnSession.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(237)))));
            this.pnSession.Location = new System.Drawing.Point(1090, 67);
            this.pnSession.Name = "pnSession";
            this.pnSession.ShadowDecoration.Parent = this.pnSession;
            this.pnSession.Size = new System.Drawing.Size(208, 97);
            this.pnSession.TabIndex = 4;
            // 
            // lbSession
            // 
            this.lbSession.BackColor = System.Drawing.Color.Transparent;
            this.lbSession.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSession.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.lbSession.Location = new System.Drawing.Point(62, 59);
            this.lbSession.Name = "lbSession";
            this.lbSession.Size = new System.Drawing.Size(78, 20);
            this.lbSession.TabIndex = 1;
            this.lbSession.Text = "Số buổi tập";
            // 
            // lbGenSession
            // 
            this.lbGenSession.BackColor = System.Drawing.Color.Transparent;
            this.lbGenSession.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenSession.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(163)))), ((int)(((byte)(126)))));
            this.lbGenSession.Location = new System.Drawing.Point(81, 14);
            this.lbGenSession.Name = "lbGenSession";
            this.lbGenSession.Size = new System.Drawing.Size(15, 27);
            this.lbGenSession.TabIndex = 0;
            this.lbGenSession.Text = "#";
            // 
            // pnTotalTime
            // 
            this.pnTotalTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.pnTotalTime.BorderRadius = 20;
            this.pnTotalTime.Controls.Add(this.lbTotalTime);
            this.pnTotalTime.Controls.Add(this.lbGenTotalTime);
            this.pnTotalTime.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.pnTotalTime.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.pnTotalTime.Location = new System.Drawing.Point(737, 67);
            this.pnTotalTime.Name = "pnTotalTime";
            this.pnTotalTime.ShadowDecoration.Parent = this.pnTotalTime;
            this.pnTotalTime.Size = new System.Drawing.Size(208, 97);
            this.pnTotalTime.TabIndex = 4;
            // 
            // lbTotalTime
            // 
            this.lbTotalTime.BackColor = System.Drawing.Color.Transparent;
            this.lbTotalTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.lbTotalTime.Location = new System.Drawing.Point(62, 59);
            this.lbTotalTime.Name = "lbTotalTime";
            this.lbTotalTime.Size = new System.Drawing.Size(96, 20);
            this.lbTotalTime.TabIndex = 1;
            this.lbTotalTime.Text = "Tổng thời gian";
            // 
            // lbGenTotalTime
            // 
            this.lbGenTotalTime.BackColor = System.Drawing.Color.Transparent;
            this.lbGenTotalTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenTotalTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(66)))), ((int)(((byte)(199)))));
            this.lbGenTotalTime.Location = new System.Drawing.Point(81, 14);
            this.lbGenTotalTime.Name = "lbGenTotalTime";
            this.lbGenTotalTime.Size = new System.Drawing.Size(15, 27);
            this.lbGenTotalTime.TabIndex = 0;
            this.lbGenTotalTime.Text = "#";
            // 
            // pnPoints
            // 
            this.pnPoints.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnPoints.BorderRadius = 20;
            this.pnPoints.Controls.Add(this.lbPoints);
            this.pnPoints.Controls.Add(this.lbGenPoints);
            this.pnPoints.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnPoints.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnPoints.Location = new System.Drawing.Point(371, 67);
            this.pnPoints.Name = "pnPoints";
            this.pnPoints.ShadowDecoration.Parent = this.pnPoints;
            this.pnPoints.Size = new System.Drawing.Size(208, 97);
            this.pnPoints.TabIndex = 3;
            // 
            // lbPoints
            // 
            this.lbPoints.BackColor = System.Drawing.Color.Transparent;
            this.lbPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPoints.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.lbPoints.Location = new System.Drawing.Point(62, 59);
            this.lbPoints.Name = "lbPoints";
            this.lbPoints.Size = new System.Drawing.Size(105, 20);
            this.lbPoints.TabIndex = 1;
            this.lbPoints.Text = "Điểm thành tích";
            // 
            // lbGenPoints
            // 
            this.lbGenPoints.BackColor = System.Drawing.Color.Transparent;
            this.lbGenPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenPoints.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(105)))), ((int)(((byte)(207)))));
            this.lbGenPoints.Location = new System.Drawing.Point(81, 14);
            this.lbGenPoints.Name = "lbGenPoints";
            this.lbGenPoints.Size = new System.Drawing.Size(15, 27);
            this.lbGenPoints.TabIndex = 0;
            this.lbGenPoints.Text = "#";
            // 
            // pnRank
            // 
            this.pnRank.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.BorderRadius = 20;
            this.pnRank.Controls.Add(this.lbRank);
            this.pnRank.Controls.Add(this.lbGenRank);
            this.pnRank.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.pnRank.Location = new System.Drawing.Point(31, 67);
            this.pnRank.Name = "pnRank";
            this.pnRank.ShadowDecoration.Parent = this.pnRank;
            this.pnRank.Size = new System.Drawing.Size(202, 97);
            this.pnRank.TabIndex = 2;
            // 
            // lbRank
            // 
            this.lbRank.BackColor = System.Drawing.Color.Transparent;
            this.lbRank.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRank.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(143)))), ((int)(((byte)(141)))));
            this.lbRank.Location = new System.Drawing.Point(57, 59);
            this.lbRank.Name = "lbRank";
            this.lbRank.Size = new System.Drawing.Size(94, 20);
            this.lbRank.TabIndex = 1;
            this.lbRank.Text = "Hạng cá nhân";
            // 
            // lbGenRank
            // 
            this.lbGenRank.BackColor = System.Drawing.Color.Transparent;
            this.lbGenRank.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbGenRank.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(103)))));
            this.lbGenRank.Location = new System.Drawing.Point(76, 14);
            this.lbGenRank.Name = "lbGenRank";
            this.lbGenRank.Size = new System.Drawing.Size(15, 27);
            this.lbGenRank.TabIndex = 0;
            this.lbGenRank.Text = "#";
            // 
            // lbStatistic
            // 
            this.lbStatistic.BackColor = System.Drawing.Color.Transparent;
            this.lbStatistic.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatistic.Location = new System.Drawing.Point(31, 13);
            this.lbStatistic.Name = "lbStatistic";
            this.lbStatistic.Size = new System.Drawing.Size(151, 22);
            this.lbStatistic.TabIndex = 0;
            this.lbStatistic.Text = "Thống kê cá nhân";
            // 
            // LeaderBoardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 739);
            this.Controls.Add(this.guna2ShadowPanel3);
            this.Controls.Add(this.shpnData);
            this.Controls.Add(this.guna2ShadowPanel1);
            this.Controls.Add(this.pnlTitle);
            this.Name = "LeaderBoardForm";
            this.Text = "LeaderBoardForm";
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.guna2ShadowPanel1.ResumeLayout(false);
            this.shpnData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistic)).EndInit();
            this.guna2ShadowPanel3.ResumeLayout(false);
            this.guna2ShadowPanel3.PerformLayout();
            this.pnSession.ResumeLayout(false);
            this.pnSession.PerformLayout();
            this.pnTotalTime.ResumeLayout(false);
            this.pnTotalTime.PerformLayout();
            this.pnPoints.ResumeLayout(false);
            this.pnPoints.PerformLayout();
            this.pnRank.ResumeLayout(false);
            this.pnRank.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel pnlTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTitleMain;
        private Guna.UI2.WinForms.Guna2GradientButton btnAllTime;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2GradientButton btnWeek;
        private Guna.UI2.WinForms.Guna2GradientButton btnMonth;
        private Guna.UI2.WinForms.Guna2ShadowPanel shpnData;
        private Guna.UI2.WinForms.Guna2DataGridView dgvStatistic;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPoints;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSession;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel3;
        private Guna.UI2.WinForms.Guna2GradientPanel pnRank;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbStatistic;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenRank;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbRank;
        private Guna.UI2.WinForms.Guna2GradientPanel pnPoints;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbPoints;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenPoints;
        private Guna.UI2.WinForms.Guna2GradientPanel pnSession;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbSession;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenSession;
        private Guna.UI2.WinForms.Guna2GradientPanel pnTotalTime;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTotalTime;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbGenTotalTime;
        private Guna.UI2.WinForms.Guna2GradientButton btnToday;
    }
}