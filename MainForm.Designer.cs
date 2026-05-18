namespace SpadApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            panelMainView = new Panel();
            btnMinimize = new FontAwesome.Sharp.IconButton();
            btnMaximize = new FontAwesome.Sharp.IconButton();
            btnClose = new FontAwesome.Sharp.IconButton();
            iconButton8 = new FontAwesome.Sharp.IconButton();
            btnAppView = new FontAwesome.Sharp.IconButton();
            btnJitterView = new FontAwesome.Sharp.IconButton();
            iconButton4 = new FontAwesome.Sharp.IconButton();
            btnSetting = new FontAwesome.Sharp.IconButton();
            btnMainView = new FontAwesome.Sharp.IconButton();
            label1 = new Label();
            panel1 = new Panel();
            btnMenu = new FontAwesome.Sharp.IconButton();
            pictureBox1 = new PictureBox();
            panelDeskTop = new Panel();
            panelTitleBar = new Panel();
            panelMenu = new Panel();
            btnSPCview = new FontAwesome.Sharp.IconButton();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelDeskTop.SuspendLayout();
            panelTitleBar.SuspendLayout();
            panelMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panelMainView
            // 
            panelMainView.Dock = DockStyle.Fill;
            panelMainView.Location = new Point(0, 0);
            panelMainView.Name = "panelMainView";
            panelMainView.Size = new Size(2160, 1304);
            panelMainView.TabIndex = 0;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.BackColor = Color.DarkTurquoise;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.IconChar = FontAwesome.Sharp.IconChar.Tasks;
            btnMinimize.IconColor = Color.White;
            btnMinimize.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMinimize.IconSize = 25;
            btnMinimize.Location = new Point(1989, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(60, 30);
            btnMinimize.TabIndex = 6;
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // btnMaximize
            // 
            btnMaximize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMaximize.BackColor = Color.RoyalBlue;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.IconChar = FontAwesome.Sharp.IconChar.Square;
            btnMaximize.IconColor = Color.White;
            btnMaximize.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMaximize.IconSize = 25;
            btnMaximize.Location = new Point(2044, 0);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(60, 30);
            btnMaximize.TabIndex = 7;
            btnMaximize.UseVisualStyleBackColor = false;
            btnMaximize.Click += btnMaximize_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(255, 74, 130);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.IconChar = FontAwesome.Sharp.IconChar.Close;
            btnClose.IconColor = Color.White;
            btnClose.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnClose.IconSize = 25;
            btnClose.Location = new Point(2100, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(60, 30);
            btnClose.TabIndex = 7;
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // iconButton8
            // 
            iconButton8.AutoSize = true;
            iconButton8.Dock = DockStyle.Bottom;
            iconButton8.FlatAppearance.BorderSize = 0;
            iconButton8.FlatStyle = FlatStyle.Flat;
            iconButton8.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            iconButton8.ForeColor = Color.White;
            iconButton8.IconChar = FontAwesome.Sharp.IconChar.SignOutAlt;
            iconButton8.IconColor = Color.White;
            iconButton8.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton8.IconSize = 30;
            iconButton8.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton8.Location = new Point(0, 1295);
            iconButton8.Name = "iconButton8";
            iconButton8.Padding = new Padding(10, 0, 0, 10);
            iconButton8.Size = new Size(296, 60);
            iconButton8.TabIndex = 7;
            iconButton8.Tag = "Exit";
            iconButton8.Text = "  iconButton2";
            iconButton8.TextAlign = ContentAlignment.MiddleLeft;
            iconButton8.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton8.UseVisualStyleBackColor = true;
            // 
            // btnAppView
            // 
            btnAppView.AutoSize = true;
            btnAppView.Dock = DockStyle.Top;
            btnAppView.FlatAppearance.BorderSize = 0;
            btnAppView.FlatStyle = FlatStyle.Flat;
            btnAppView.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAppView.ForeColor = Color.White;
            btnAppView.IconChar = FontAwesome.Sharp.IconChar.Institution;
            btnAppView.IconColor = Color.White;
            btnAppView.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnAppView.IconSize = 30;
            btnAppView.ImageAlign = ContentAlignment.MiddleLeft;
            btnAppView.Location = new Point(0, 318);
            btnAppView.Name = "btnAppView";
            btnAppView.Padding = new Padding(10, 15, 15, 0);
            btnAppView.Size = new Size(296, 57);
            btnAppView.TabIndex = 5;
            btnAppView.Tag = "APP";
            btnAppView.Text = "  APP";
            btnAppView.TextAlign = ContentAlignment.MiddleLeft;
            btnAppView.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAppView.UseVisualStyleBackColor = true;
            // 
            // btnJitterView
            // 
            btnJitterView.AutoSize = true;
            btnJitterView.Dock = DockStyle.Top;
            btnJitterView.FlatAppearance.BorderSize = 0;
            btnJitterView.FlatStyle = FlatStyle.Flat;
            btnJitterView.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnJitterView.ForeColor = Color.White;
            btnJitterView.IconChar = FontAwesome.Sharp.IconChar.HouseFlag;
            btnJitterView.IconColor = Color.White;
            btnJitterView.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnJitterView.IconSize = 30;
            btnJitterView.ImageAlign = ContentAlignment.MiddleLeft;
            btnJitterView.Location = new Point(0, 261);
            btnJitterView.Name = "btnJitterView";
            btnJitterView.Padding = new Padding(10, 15, 15, 0);
            btnJitterView.Size = new Size(296, 57);
            btnJitterView.TabIndex = 4;
            btnJitterView.Tag = "Jitter";
            btnJitterView.Text = "  Jitter";
            btnJitterView.TextAlign = ContentAlignment.MiddleLeft;
            btnJitterView.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnJitterView.UseVisualStyleBackColor = true;
            // 
            // iconButton4
            // 
            iconButton4.AutoSize = true;
            iconButton4.Dock = DockStyle.Top;
            iconButton4.FlatAppearance.BorderSize = 0;
            iconButton4.FlatStyle = FlatStyle.Flat;
            iconButton4.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            iconButton4.ForeColor = Color.White;
            iconButton4.IconChar = FontAwesome.Sharp.IconChar.IdCardClip;
            iconButton4.IconColor = Color.White;
            iconButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton4.IconSize = 30;
            iconButton4.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton4.Location = new Point(0, 204);
            iconButton4.Name = "iconButton4";
            iconButton4.Padding = new Padding(10, 15, 15, 0);
            iconButton4.Size = new Size(296, 57);
            iconButton4.TabIndex = 3;
            iconButton4.Tag = "Hospital";
            iconButton4.Text = "  PDE";
            iconButton4.TextAlign = ContentAlignment.MiddleLeft;
            iconButton4.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton4.UseVisualStyleBackColor = true;
            // 
            // btnSetting
            // 
            btnSetting.AutoSize = true;
            btnSetting.Dock = DockStyle.Top;
            btnSetting.FlatAppearance.BorderSize = 0;
            btnSetting.FlatStyle = FlatStyle.Flat;
            btnSetting.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSetting.ForeColor = Color.White;
            btnSetting.IconChar = FontAwesome.Sharp.IconChar.HockeyPuck;
            btnSetting.IconColor = Color.White;
            btnSetting.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnSetting.IconSize = 30;
            btnSetting.ImageAlign = ContentAlignment.MiddleLeft;
            btnSetting.Location = new Point(0, 147);
            btnSetting.Name = "btnSetting";
            btnSetting.Padding = new Padding(10, 15, 15, 0);
            btnSetting.Size = new Size(296, 57);
            btnSetting.TabIndex = 2;
            btnSetting.Tag = "Horse";
            btnSetting.Text = "  DCR";
            btnSetting.TextAlign = ContentAlignment.MiddleLeft;
            btnSetting.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSetting.UseVisualStyleBackColor = true;
            // 
            // btnMainView
            // 
            btnMainView.AutoSize = true;
            btnMainView.Dock = DockStyle.Top;
            btnMainView.FlatAppearance.BorderSize = 0;
            btnMainView.FlatStyle = FlatStyle.Flat;
            btnMainView.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMainView.ForeColor = Color.White;
            btnMainView.IconChar = FontAwesome.Sharp.IconChar.HomeUser;
            btnMainView.IconColor = Color.White;
            btnMainView.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMainView.IconSize = 30;
            btnMainView.ImageAlign = ContentAlignment.MiddleLeft;
            btnMainView.Location = new Point(0, 90);
            btnMainView.Name = "btnMainView";
            btnMainView.Padding = new Padding(10, 15, 15, 0);
            btnMainView.Size = new Size(296, 57);
            btnMainView.TabIndex = 1;
            btnMainView.Tag = "Home";
            btnMainView.Text = "  MainView";
            btnMainView.TextAlign = ContentAlignment.MiddleLeft;
            btnMainView.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnMainView.UseVisualStyleBackColor = true;
            btnMainView.Click += btnMainView_Click;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Left;
            label1.Font = new Font("Pretendard", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(222, 51);
            label1.TabIndex = 3;
            label1.Text = "SPAD TEST APP";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnMenu);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(296, 90);
            panel1.TabIndex = 0;
            // 
            // btnMenu
            // 
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.IconChar = FontAwesome.Sharp.IconChar.Bars;
            btnMenu.IconColor = Color.White;
            btnMenu.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMenu.IconSize = 40;
            btnMenu.Location = new Point(236, 5);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(60, 74);
            btnMenu.TabIndex = 1;
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 7);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(241, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panelDeskTop
            // 
            panelDeskTop.BackColor = Color.FromArgb(245, 245, 255);
            panelDeskTop.Controls.Add(panelMainView);
            panelDeskTop.Dock = DockStyle.Fill;
            panelDeskTop.Location = new Point(296, 51);
            panelDeskTop.Name = "panelDeskTop";
            panelDeskTop.Size = new Size(2160, 1304);
            panelDeskTop.TabIndex = 5;
            panelDeskTop.Tag = "Horse";
            // 
            // panelTitleBar
            // 
            panelTitleBar.BackColor = Color.White;
            panelTitleBar.Controls.Add(btnMinimize);
            panelTitleBar.Controls.Add(btnMaximize);
            panelTitleBar.Controls.Add(btnClose);
            panelTitleBar.Controls.Add(label1);
            panelTitleBar.Dock = DockStyle.Top;
            panelTitleBar.Location = new Point(296, 0);
            panelTitleBar.Name = "panelTitleBar";
            panelTitleBar.Size = new Size(2160, 51);
            panelTitleBar.TabIndex = 4;
            panelTitleBar.MouseDoubleClick += panelTitleBar_MouseDoubleClick;
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.FromArgb(98, 102, 242);
            panelMenu.Controls.Add(btnSPCview);
            panelMenu.Controls.Add(iconButton8);
            panelMenu.Controls.Add(btnAppView);
            panelMenu.Controls.Add(btnJitterView);
            panelMenu.Controls.Add(iconButton4);
            panelMenu.Controls.Add(btnSetting);
            panelMenu.Controls.Add(btnMainView);
            panelMenu.Controls.Add(panel1);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(296, 1355);
            panelMenu.TabIndex = 3;
            // 
            // btnSPCview
            // 
            btnSPCview.AutoSize = true;
            btnSPCview.Dock = DockStyle.Top;
            btnSPCview.FlatAppearance.BorderSize = 0;
            btnSPCview.FlatStyle = FlatStyle.Flat;
            btnSPCview.Font = new Font("Pretendard SemiBold", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSPCview.ForeColor = Color.White;
            btnSPCview.IconChar = FontAwesome.Sharp.IconChar.Institution;
            btnSPCview.IconColor = Color.White;
            btnSPCview.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnSPCview.IconSize = 30;
            btnSPCview.ImageAlign = ContentAlignment.MiddleLeft;
            btnSPCview.Location = new Point(0, 375);
            btnSPCview.Name = "btnSPCview";
            btnSPCview.Padding = new Padding(10, 15, 15, 0);
            btnSPCview.Size = new Size(296, 57);
            btnSPCview.TabIndex = 8;
            btnSPCview.Tag = "SPC";
            btnSPCview.Text = "  SPC";
            btnSPCview.TextAlign = ContentAlignment.MiddleLeft;
            btnSPCview.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSPCview.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2456, 1355);
            Controls.Add(panelDeskTop);
            Controls.Add(panelTitleBar);
            Controls.Add(panelMenu);
            Margin = new Padding(6);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelDeskTop.ResumeLayout(false);
            panelTitleBar.ResumeLayout(false);
            panelMenu.ResumeLayout(false);
            panelMenu.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMainView;
        private FontAwesome.Sharp.IconButton iconButton8;
        private FontAwesome.Sharp.IconButton iconButton7;
        private FontAwesome.Sharp.IconButton btnAppView;
        private FontAwesome.Sharp.IconButton btnJitterView;
        private FontAwesome.Sharp.IconButton iconButton4;
        private FontAwesome.Sharp.IconButton btnSetting;
        private FontAwesome.Sharp.IconButton btnMainView;
        private Label label1;

        private Panel panel1;
        private FontAwesome.Sharp.IconButton btnMenu;
        private PictureBox pictureBox1;
        private Panel panelDeskTop;
        private Panel panelTitleBar;
        private Panel panelMenu;
        private FontAwesome.Sharp.IconButton btnSPCview;
        private FontAwesome.Sharp.IconButton iconButton2;
        private FontAwesome.Sharp.IconButton btnMinimize;
        private FontAwesome.Sharp.IconButton btnClose;
        private FontAwesome.Sharp.IconButton btnMaximize;
    }
}
