namespace SpadApp {
    partial class ucMainView {
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
            lblMeanPerPulse = new Label();
            lblPhotonFlux = new Label();
            numPMWavelength = new NumericUpDown();
            btnRun = new Button();
            histogramPlot = new ScottPlot.WinForms.FormsPlot();
            richtxtLog = new RichTextBox();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            numBinning = new NumericUpDown();
            numAcqTime = new NumericUpDown();
            numSyncDiv = new NumericUpDown();
            numCFDLevel0 = new NumericUpDown();
            numCFDZeroCross0 = new NumericUpDown();
            numCFDLevel1 = new NumericUpDown();
            numCFDZeroCross1 = new NumericUpDown();
            btnPM1ZeroAdjust = new Button();
            lblPDE = new Label();
            label10 = new Label();
            label1 = new Label();
            lblResolution = new Label();
            lblPowerMeter1 = new Label();
            lblCountRate0 = new ToolStripStatusLabel();
            lblCountRate1 = new ToolStripStatusLabel();
            lblStatus = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            label2 = new Label();
            label11 = new Label();
            label12 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnSave = new Button();
            btnMeasure = new Button();
            btnConnect = new Button();
            btnLogOrLinearScaleY = new Button();
            btnLogOrLinearScaleX = new Button();
            numPlotMinX = new NumericUpDown();
            numPlotMaxX = new NumericUpDown();
            lblPlotMinX = new Label();
            label14 = new Label();
            lblPowerMeter2 = new Label();
            btnResetXLimits = new Button();
            numAcqOffset = new NumericUpDown();
            label13 = new Label();
            numSyncOffset = new NumericUpDown();
            label15 = new Label();
            ((System.ComponentModel.ISupportInitialize)numPMWavelength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBinning).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAcqTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSyncDiv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross1).BeginInit();
            statusStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPlotMinX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMaxX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAcqOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSyncOffset).BeginInit();
            SuspendLayout();
            // 
            // lblMeanPerPulse
            // 
            lblMeanPerPulse.AutoSize = true;
            lblMeanPerPulse.Location = new Point(605, 450);
            lblMeanPerPulse.Name = "lblMeanPerPulse";
            lblMeanPerPulse.Size = new Size(39, 15);
            lblMeanPerPulse.TabIndex = 34;
            lblMeanPerPulse.Text = "label1";
            // 
            // lblPhotonFlux
            // 
            lblPhotonFlux.AutoSize = true;
            lblPhotonFlux.Location = new Point(478, 449);
            lblPhotonFlux.Name = "lblPhotonFlux";
            lblPhotonFlux.Size = new Size(39, 15);
            lblPhotonFlux.TabIndex = 32;
            lblPhotonFlux.Text = "label1";
            // 
            // numPMWavelength
            // 
            numPMWavelength.Location = new Point(244, 518);
            numPMWavelength.Name = "numPMWavelength";
            numPMWavelength.Size = new Size(120, 23);
            numPMWavelength.TabIndex = 27;
            numPMWavelength.ValueChanged += numPMWavelength_ValueChanged;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(7, 74);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(92, 34);
            btnRun.TabIndex = 26;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // histogramPlot
            // 
            histogramPlot.Location = new Point(190, 22);
            histogramPlot.Name = "histogramPlot";
            histogramPlot.Size = new Size(922, 254);
            histogramPlot.TabIndex = 25;
            // 
            // richtxtLog
            // 
            richtxtLog.Location = new Point(847, 298);
            richtxtLog.Name = "richtxtLog";
            richtxtLog.Size = new Size(231, 193);
            richtxtLog.TabIndex = 24;
            richtxtLog.Text = "";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Location = new Point(272, 70);
            label9.Name = "label9";
            label9.Size = new Size(83, 32);
            label9.TabIndex = 14;
            label9.Text = "Ch1 ZeroX";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Fill;
            label8.Location = new Point(183, 70);
            label8.Name = "label8";
            label8.Size = new Size(81, 32);
            label8.TabIndex = 13;
            label8.Text = "Ch1 CFD Level";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Fill;
            label7.Location = new Point(94, 70);
            label7.Name = "label7";
            label7.Size = new Size(81, 32);
            label7.TabIndex = 12;
            label7.Text = "Ch0 ZeroX";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Location = new Point(5, 70);
            label6.Name = "label6";
            label6.Size = new Size(81, 32);
            label6.TabIndex = 9;
            label6.Text = "Ch0 CFD level";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(272, 2);
            label5.Name = "label5";
            label5.Size = new Size(83, 32);
            label5.TabIndex = 11;
            label5.Text = "Resolution (ps)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(183, 2);
            label4.Name = "label4";
            label4.Size = new Size(81, 32);
            label4.TabIndex = 10;
            label4.Text = "Sync div";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(94, 2);
            label3.Name = "label3";
            label3.Size = new Size(81, 32);
            label3.TabIndex = 9;
            label3.Text = "Acq. Time";
            // 
            // numBinning
            // 
            numBinning.Dock = DockStyle.Fill;
            numBinning.Location = new Point(5, 39);
            numBinning.Name = "numBinning";
            numBinning.Size = new Size(81, 23);
            numBinning.TabIndex = 2;
            numBinning.ValueChanged += numBinning_ValueChanged;
            // 
            // numAcqTime
            // 
            numAcqTime.Dock = DockStyle.Fill;
            numAcqTime.Location = new Point(94, 39);
            numAcqTime.Name = "numAcqTime";
            numAcqTime.Size = new Size(81, 23);
            numAcqTime.TabIndex = 2;
            numAcqTime.ValueChanged += numAcqTime_ValueChanged;
            // 
            // numSyncDiv
            // 
            numSyncDiv.Dock = DockStyle.Fill;
            numSyncDiv.Location = new Point(183, 39);
            numSyncDiv.Name = "numSyncDiv";
            numSyncDiv.Size = new Size(81, 23);
            numSyncDiv.TabIndex = 2;
            numSyncDiv.ValueChanged += numSyncDiv_ValueChanged;
            // 
            // numCFDLevel0
            // 
            numCFDLevel0.Dock = DockStyle.Fill;
            numCFDLevel0.Location = new Point(5, 107);
            numCFDLevel0.Name = "numCFDLevel0";
            numCFDLevel0.Size = new Size(81, 23);
            numCFDLevel0.TabIndex = 3;
            numCFDLevel0.ValueChanged += numCFDLevel0_ValueChanged;
            // 
            // numCFDZeroCross0
            // 
            numCFDZeroCross0.Dock = DockStyle.Fill;
            numCFDZeroCross0.Location = new Point(94, 107);
            numCFDZeroCross0.Name = "numCFDZeroCross0";
            numCFDZeroCross0.Size = new Size(81, 23);
            numCFDZeroCross0.TabIndex = 4;
            numCFDZeroCross0.ValueChanged += numCFDZeroCross0_ValueChanged;
            // 
            // numCFDLevel1
            // 
            numCFDLevel1.Dock = DockStyle.Fill;
            numCFDLevel1.Location = new Point(183, 107);
            numCFDLevel1.Name = "numCFDLevel1";
            numCFDLevel1.Size = new Size(81, 23);
            numCFDLevel1.TabIndex = 5;
            numCFDLevel1.ValueChanged += numCFDLevel1_ValueChanged;
            // 
            // numCFDZeroCross1
            // 
            numCFDZeroCross1.Dock = DockStyle.Fill;
            numCFDZeroCross1.Location = new Point(272, 107);
            numCFDZeroCross1.Name = "numCFDZeroCross1";
            numCFDZeroCross1.Size = new Size(83, 23);
            numCFDZeroCross1.TabIndex = 6;
            numCFDZeroCross1.ValueChanged += numCFDZeroCross1_ValueChanged;
            // 
            // btnPM1ZeroAdjust
            // 
            btnPM1ZeroAdjust.Location = new Point(543, 404);
            btnPM1ZeroAdjust.Name = "btnPM1ZeroAdjust";
            btnPM1ZeroAdjust.Size = new Size(90, 23);
            btnPM1ZeroAdjust.TabIndex = 35;
            btnPM1ZeroAdjust.Text = "Background";
            btnPM1ZeroAdjust.UseVisualStyleBackColor = true;
            btnPM1ZeroAdjust.Click += btnPM1ZeroAdjust_Click;
            // 
            // lblPDE
            // 
            lblPDE.AutoSize = true;
            lblPDE.Location = new Point(689, 451);
            lblPDE.Name = "lblPDE";
            lblPDE.Size = new Size(39, 15);
            lblPDE.TabIndex = 31;
            lblPDE.Text = "label1";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(580, 430);
            label10.Name = "label10";
            label10.Size = new Size(90, 15);
            label10.TabIndex = 30;
            label10.Text = "Mean Per Pulse";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(433, 430);
            label1.Name = "label1";
            label1.Size = new Size(141, 15);
            label1.TabIndex = 33;
            label1.Text = "Incident Photon Number";
            // 
            // lblResolution
            // 
            lblResolution.AutoSize = true;
            lblResolution.BackColor = SystemColors.ButtonFace;
            lblResolution.BorderStyle = BorderStyle.FixedSingle;
            lblResolution.Dock = DockStyle.Fill;
            lblResolution.Location = new Point(272, 36);
            lblResolution.Name = "lblResolution";
            lblResolution.Size = new Size(83, 32);
            lblResolution.TabIndex = 7;
            lblResolution.Text = "label1";
            // 
            // lblPowerMeter1
            // 
            lblPowerMeter1.AutoSize = true;
            lblPowerMeter1.Location = new Point(244, 500);
            lblPowerMeter1.Name = "lblPowerMeter1";
            lblPowerMeter1.Size = new Size(91, 15);
            lblPowerMeter1.TabIndex = 22;
            lblPowerMeter1.Text = "lblPowerMeter1";
            // 
            // lblCountRate0
            // 
            lblCountRate0.Name = "lblCountRate0";
            lblCountRate0.Size = new Size(74, 17);
            lblCountRate0.Text = "CountRate 0";
            // 
            // lblCountRate1
            // 
            lblCountRate1.Name = "lblCountRate1";
            lblCountRate1.Size = new Size(74, 17);
            lblCountRate1.Text = "CountRate 1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(40, 17);
            lblStatus.Text = "Status";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(32, 32);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, lblCountRate1, lblCountRate0 });
            statusStrip1.Location = new Point(0, 578);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RightToLeft = RightToLeft.Yes;
            statusStrip1.Size = new Size(1200, 22);
            statusStrip1.TabIndex = 21;
            statusStrip1.Text = "statusStrip1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(5, 2);
            label2.Name = "label2";
            label2.Size = new Size(81, 32);
            label2.TabIndex = 8;
            label2.Text = "Binning";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(689, 431);
            label11.Name = "label11";
            label11.Size = new Size(29, 15);
            label11.TabIndex = 29;
            label11.Text = "PDE";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(151, 500);
            label12.Name = "label12";
            label12.Size = new Size(87, 15);
            label12.TabIndex = 23;
            label12.Text = "Average Power";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(label9, 3, 2);
            tableLayoutPanel1.Controls.Add(label8, 2, 2);
            tableLayoutPanel1.Controls.Add(label7, 1, 2);
            tableLayoutPanel1.Controls.Add(label6, 0, 2);
            tableLayoutPanel1.Controls.Add(label5, 3, 0);
            tableLayoutPanel1.Controls.Add(label4, 2, 0);
            tableLayoutPanel1.Controls.Add(label3, 1, 0);
            tableLayoutPanel1.Controls.Add(numBinning, 0, 1);
            tableLayoutPanel1.Controls.Add(numAcqTime, 1, 1);
            tableLayoutPanel1.Controls.Add(numSyncDiv, 2, 1);
            tableLayoutPanel1.Controls.Add(numCFDLevel0, 0, 3);
            tableLayoutPanel1.Controls.Add(numCFDZeroCross0, 1, 3);
            tableLayoutPanel1.Controls.Add(numCFDLevel1, 2, 3);
            tableLayoutPanel1.Controls.Add(numCFDZeroCross1, 3, 3);
            tableLayoutPanel1.Controls.Add(lblResolution, 3, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Location = new Point(7, 299);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(360, 139);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 259);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(92, 34);
            btnSave.TabIndex = 18;
            btnSave.Text = "Save CSV";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnMeasure
            // 
            btnMeasure.Location = new Point(8, 120);
            btnMeasure.Name = "btnMeasure";
            btnMeasure.Size = new Size(92, 34);
            btnMeasure.TabIndex = 19;
            btnMeasure.Text = "Measure";
            btnMeasure.UseVisualStyleBackColor = true;
            btnMeasure.Click += btnMeasure_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(8, 34);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(92, 34);
            btnConnect.TabIndex = 17;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnLogOrLinearScaleY
            // 
            btnLogOrLinearScaleY.Location = new Point(383, 282);
            btnLogOrLinearScaleY.Name = "btnLogOrLinearScaleY";
            btnLogOrLinearScaleY.Size = new Size(104, 23);
            btnLogOrLinearScaleY.TabIndex = 36;
            btnLogOrLinearScaleY.Text = "Y: Log or Linear";
            btnLogOrLinearScaleY.UseVisualStyleBackColor = true;
            btnLogOrLinearScaleY.Click += btnLogOrLinearScaleY_Click;
            // 
            // btnLogOrLinearScaleX
            // 
            btnLogOrLinearScaleX.Location = new Point(383, 310);
            btnLogOrLinearScaleX.Name = "btnLogOrLinearScaleX";
            btnLogOrLinearScaleX.Size = new Size(104, 23);
            btnLogOrLinearScaleX.TabIndex = 36;
            btnLogOrLinearScaleX.Text = "X: Log or Linear";
            btnLogOrLinearScaleX.UseVisualStyleBackColor = true;
            btnLogOrLinearScaleX.Click += btnLogOrLinearScaleX_Click;
            // 
            // numPlotMinX
            // 
            numPlotMinX.Location = new Point(496, 333);
            numPlotMinX.Name = "numPlotMinX";
            numPlotMinX.Size = new Size(84, 23);
            numPlotMinX.TabIndex = 37;
            numPlotMinX.ValueChanged += numPlotMinX_ValueChanged;
            // 
            // numPlotMaxX
            // 
            numPlotMaxX.Location = new Point(622, 333);
            numPlotMaxX.Name = "numPlotMaxX";
            numPlotMaxX.Size = new Size(84, 23);
            numPlotMaxX.TabIndex = 37;
            numPlotMaxX.ValueChanged += numPlotMaxX_ValueChanged;
            // 
            // lblPlotMinX
            // 
            lblPlotMinX.AutoSize = true;
            lblPlotMinX.Location = new Point(499, 314);
            lblPlotMinX.Name = "lblPlotMinX";
            lblPlotMinX.Size = new Size(71, 15);
            lblPlotMinX.TabIndex = 38;
            lblPlotMinX.Text = "X minimum";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(622, 315);
            label14.Name = "label14";
            label14.Size = new Size(73, 15);
            label14.TabIndex = 38;
            label14.Text = "X Maximum";
            // 
            // lblPowerMeter2
            // 
            lblPowerMeter2.AutoSize = true;
            lblPowerMeter2.Location = new Point(244, 557);
            lblPowerMeter2.Name = "lblPowerMeter2";
            lblPowerMeter2.Size = new Size(91, 15);
            lblPowerMeter2.TabIndex = 28;
            lblPowerMeter2.Text = "lblPowerMeter2";
            // 
            // btnResetXLimits
            // 
            btnResetXLimits.Location = new Point(383, 336);
            btnResetXLimits.Name = "btnResetXLimits";
            btnResetXLimits.Size = new Size(104, 23);
            btnResetXLimits.TabIndex = 39;
            btnResetXLimits.Text = "Reset X scale";
            btnResetXLimits.UseVisualStyleBackColor = true;
            btnResetXLimits.Click += btnResetXLimits_Click;
            // 
            // numAcqOffset
            // 
            numAcqOffset.Location = new Point(25, 477);
            numAcqOffset.Name = "numAcqOffset";
            numAcqOffset.Size = new Size(68, 23);
            numAcqOffset.TabIndex = 40;
            numAcqOffset.ValueChanged += numOffset_ValueChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(25, 451);
            label13.Name = "label13";
            label13.Size = new Size(84, 15);
            label13.TabIndex = 41;
            label13.Text = "AcqOffset (ps)";
            // 
            // numSyncOffset
            // 
            numSyncOffset.Location = new Point(25, 535);
            numSyncOffset.Name = "numSyncOffset";
            numSyncOffset.Size = new Size(68, 23);
            numSyncOffset.TabIndex = 40;
            numSyncOffset.ValueChanged += numOffset_ValueChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(20, 517);
            label15.Name = "label15";
            label15.Size = new Size(93, 15);
            label15.TabIndex = 41;
            label15.Text = "Sync Offset (ps)";
            // 
            // ucMainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label15);
            Controls.Add(label13);
            Controls.Add(numSyncOffset);
            Controls.Add(numAcqOffset);
            Controls.Add(btnResetXLimits);
            Controls.Add(label14);
            Controls.Add(lblPlotMinX);
            Controls.Add(numPlotMaxX);
            Controls.Add(numPlotMinX);
            Controls.Add(btnLogOrLinearScaleX);
            Controls.Add(btnLogOrLinearScaleY);
            Controls.Add(lblMeanPerPulse);
            Controls.Add(lblPhotonFlux);
            Controls.Add(lblPowerMeter2);
            Controls.Add(numPMWavelength);
            Controls.Add(btnRun);
            Controls.Add(histogramPlot);
            Controls.Add(richtxtLog);
            Controls.Add(btnPM1ZeroAdjust);
            Controls.Add(lblPDE);
            Controls.Add(label10);
            Controls.Add(label1);
            Controls.Add(lblPowerMeter1);
            Controls.Add(statusStrip1);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(btnSave);
            Controls.Add(btnMeasure);
            Controls.Add(btnConnect);
            Margin = new Padding(2, 1, 2, 1);
            Name = "ucMainView";
            Size = new Size(1200, 600);
            Load += ucMainView_Load;
            ((System.ComponentModel.ISupportInitialize)numPMWavelength).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBinning).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAcqTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSyncDiv).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel0).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross0).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPlotMinX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPlotMaxX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAcqOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSyncOffset).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMeanPerPulse;
        private Label lblPhotonFlux;
        private NumericUpDown numPMWavelength;
        private Button btnRun;
        private ScottPlot.WinForms.FormsPlot histogramPlot;
        private RichTextBox richtxtLog;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private NumericUpDown numBinning;
        private NumericUpDown numAcqTime;
        private NumericUpDown numSyncDiv;
        private NumericUpDown numCFDLevel0;
        private NumericUpDown numCFDZeroCross0;
        private NumericUpDown numCFDLevel1;
        private NumericUpDown numCFDZeroCross1;
        private Button btnPM1ZeroAdjust;
        private Label lblPDE;
        private Label label10;
        private Label label1;
        private Label lblResolution;
        private Label lblPowerMeter1;
        private ToolStripStatusLabel lblCountRate0;
        private ToolStripStatusLabel lblCountRate1;
        private ToolStripStatusLabel lblStatus;
        private StatusStrip statusStrip1;
        private Label label2;
        private Label label11;
        private Label label12;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnSave;
        private Button btnMeasure;
        private Button btnConnect;
        private Button btnLogOrLinearScaleY;
        private Button btnLogOrLinearScaleX;
        private NumericUpDown numPlotMinX;
        private NumericUpDown numPlotMaxX;
        private Label lblPlotMinX;
        private Label label14;
        private Label lblPowerMeter2;
        private Button btnResetXLimits;
        private NumericUpDown numAcqOffset;
        private Label label13;
        private NumericUpDown numSyncOffset;
        private Label label15;
    }
}
