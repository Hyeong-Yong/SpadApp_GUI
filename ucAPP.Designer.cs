namespace SpadApp {
    partial class ucAPP {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            APPplot = new ScottPlot.WinForms.FormsPlot();
            btnAPPmeasure = new Button();
            numAcqTime = new NumericUpDown();
            btnAPPsettings = new Button();
            lblAPPelapsedTime = new Label();
            lblAPPtotalRecords = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            radBtnT2Mode = new RadioButton();
            radBtnT3Mode = new RadioButton();
            btnResetXLimits = new Button();
            label14 = new Label();
            lblPlotMinX = new Label();
            numPlotMaxX = new NumericUpDown();
            numPlotMinX = new NumericUpDown();
            btnLogOrLinearScaleX = new Button();
            btnLogOrLinearScaleY = new Button();
            ((System.ComponentModel.ISupportInitialize)numAcqTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMaxX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMinX).BeginInit();
            SuspendLayout();
            // 
            // APPplot
            // 
            APPplot.Location = new Point(375, 27);
            APPplot.Name = "APPplot";
            APPplot.Size = new Size(559, 255);
            APPplot.TabIndex = 0;
            // 
            // btnAPPmeasure
            // 
            btnAPPmeasure.Location = new Point(80, 71);
            btnAPPmeasure.Name = "btnAPPmeasure";
            btnAPPmeasure.Size = new Size(146, 53);
            btnAPPmeasure.TabIndex = 1;
            btnAPPmeasure.Text = "Measure";
            btnAPPmeasure.UseVisualStyleBackColor = true;
            btnAPPmeasure.Click += btnAPPmeasure_Click;
            // 
            // numAcqTime
            // 
            numAcqTime.Location = new Point(129, 198);
            numAcqTime.Name = "numAcqTime";
            numAcqTime.Size = new Size(97, 23);
            numAcqTime.TabIndex = 2;
            // 
            // btnAPPsettings
            // 
            btnAPPsettings.Location = new Point(80, 12);
            btnAPPsettings.Name = "btnAPPsettings";
            btnAPPsettings.Size = new Size(146, 53);
            btnAPPsettings.TabIndex = 1;
            btnAPPsettings.Text = "Setting";
            btnAPPsettings.UseVisualStyleBackColor = true;
            btnAPPsettings.Click += btnAPPsettings_Click;
            // 
            // lblAPPelapsedTime
            // 
            lblAPPelapsedTime.AutoSize = true;
            lblAPPelapsedTime.Location = new Point(127, 239);
            lblAPPelapsedTime.Name = "lblAPPelapsedTime";
            lblAPPelapsedTime.Size = new Size(95, 15);
            lblAPPelapsedTime.TabIndex = 3;
            lblAPPelapsedTime.Text = "APPelapsedTime";
            // 
            // lblAPPtotalRecords
            // 
            lblAPPtotalRecords.AutoSize = true;
            lblAPPtotalRecords.Location = new Point(129, 267);
            lblAPPtotalRecords.Name = "lblAPPtotalRecords";
            lblAPPtotalRecords.Size = new Size(95, 15);
            lblAPPtotalRecords.TabIndex = 4;
            lblAPPtotalRecords.Text = "APPtotalRecords";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 239);
            label1.Name = "label1";
            label1.Size = new Size(94, 15);
            label1.TabIndex = 3;
            label1.Text = "Elapsed Time (s)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(49, 267);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 3;
            label2.Text = "Total Record";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 200);
            label3.Name = "label3";
            label3.Size = new Size(111, 15);
            label3.TabIndex = 3;
            label3.Text = "Acqusition Time (s)";
            // 
            // radBtnT2Mode
            // 
            radBtnT2Mode.AutoSize = true;
            radBtnT2Mode.Location = new Point(64, 130);
            radBtnT2Mode.Name = "radBtnT2Mode";
            radBtnT2Mode.Size = new Size(73, 19);
            radBtnT2Mode.TabIndex = 5;
            radBtnT2Mode.TabStop = true;
            radBtnT2Mode.Text = "T2 Mode";
            radBtnT2Mode.UseVisualStyleBackColor = true;
            // 
            // radBtnT3Mode
            // 
            radBtnT3Mode.AutoSize = true;
            radBtnT3Mode.Location = new Point(149, 130);
            radBtnT3Mode.Name = "radBtnT3Mode";
            radBtnT3Mode.Size = new Size(73, 19);
            radBtnT3Mode.TabIndex = 5;
            radBtnT3Mode.TabStop = true;
            radBtnT3Mode.Text = "T3 Mode";
            radBtnT3Mode.UseVisualStyleBackColor = true;
            // 
            // btnResetXLimits
            // 
            btnResetXLimits.Location = new Point(16, 374);
            btnResetXLimits.Name = "btnResetXLimits";
            btnResetXLimits.Size = new Size(104, 23);
            btnResetXLimits.TabIndex = 46;
            btnResetXLimits.Text = "Reset X scale";
            btnResetXLimits.UseVisualStyleBackColor = true;
            btnResetXLimits.Click += btnResetXLimits_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(129, 401);
            label14.Name = "label14";
            label14.Size = new Size(73, 15);
            label14.TabIndex = 44;
            label14.Text = "X Maximum";
            // 
            // lblPlotMinX
            // 
            lblPlotMinX.AutoSize = true;
            lblPlotMinX.Location = new Point(132, 352);
            lblPlotMinX.Name = "lblPlotMinX";
            lblPlotMinX.Size = new Size(71, 15);
            lblPlotMinX.TabIndex = 45;
            lblPlotMinX.Text = "X minimum";
            // 
            // numPlotMaxX
            // 
            numPlotMaxX.Location = new Point(129, 419);
            numPlotMaxX.Name = "numPlotMaxX";
            numPlotMaxX.Size = new Size(84, 23);
            numPlotMaxX.TabIndex = 42;
            numPlotMaxX.ValueChanged += numPlotMaxX_ValueChanged;
            // 
            // numPlotMinX
            // 
            numPlotMinX.Location = new Point(129, 371);
            numPlotMinX.Name = "numPlotMinX";
            numPlotMinX.Size = new Size(84, 23);
            numPlotMinX.TabIndex = 43;
            numPlotMinX.ValueChanged += numPlotMinX_ValueChanged;
            // 
            // btnLogOrLinearScaleX
            // 
            btnLogOrLinearScaleX.Location = new Point(16, 348);
            btnLogOrLinearScaleX.Name = "btnLogOrLinearScaleX";
            btnLogOrLinearScaleX.Size = new Size(104, 23);
            btnLogOrLinearScaleX.TabIndex = 40;
            btnLogOrLinearScaleX.Text = "X: Log or Linear";
            btnLogOrLinearScaleX.UseVisualStyleBackColor = true;
            btnLogOrLinearScaleX.Click += btnLogOrLinearScaleX_Click;
            // 
            // btnLogOrLinearScaleY
            // 
            btnLogOrLinearScaleY.Location = new Point(16, 320);
            btnLogOrLinearScaleY.Name = "btnLogOrLinearScaleY";
            btnLogOrLinearScaleY.Size = new Size(104, 23);
            btnLogOrLinearScaleY.TabIndex = 41;
            btnLogOrLinearScaleY.Text = "Y: Log or Linear";
            btnLogOrLinearScaleY.UseVisualStyleBackColor = true;
            btnLogOrLinearScaleY.Click += btnLogOrLinearScaleY_Click;
            // 
            // ucAPP
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnResetXLimits);
            Controls.Add(label14);
            Controls.Add(lblPlotMinX);
            Controls.Add(numPlotMaxX);
            Controls.Add(numPlotMinX);
            Controls.Add(btnLogOrLinearScaleX);
            Controls.Add(btnLogOrLinearScaleY);
            Controls.Add(radBtnT3Mode);
            Controls.Add(radBtnT2Mode);
            Controls.Add(lblAPPtotalRecords);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(lblAPPelapsedTime);
            Controls.Add(numAcqTime);
            Controls.Add(btnAPPsettings);
            Controls.Add(btnAPPmeasure);
            Controls.Add(APPplot);
            Name = "ucAPP";
            Size = new Size(1200, 600);
            Load += ucAPP_Load;
            ((System.ComponentModel.ISupportInitialize)numAcqTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMaxX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMinX).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot APPplot;
        private Button btnAPPmeasure;
        private NumericUpDown numAcqTime;
        private Button btnAPPsettings;
        private Label lblAPPelapsedTime;
        private Label lblAPPtotalRecords;
        private Label label1;
        private Label label2;
        private Label label3;
        private RadioButton radBtnT2Mode;
        private RadioButton radBtnT3Mode;
        private Button btnResetXLimits;
        private Label label14;
        private Label lblPlotMinX;
        private NumericUpDown numPlotMaxX;
        private NumericUpDown numPlotMinX;
        private Button btnLogOrLinearScaleX;
        private Button btnLogOrLinearScaleY;
    }
}
