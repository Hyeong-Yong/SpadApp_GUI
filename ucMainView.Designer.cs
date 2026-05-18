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
        private void InitializeComponent() {
            lblMeanPerPulse = new Label();
            lblPhotonFlux = new Label();
            lblPowerMeter2 = new Label();
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
            SuspendLayout();
            // 
            // lblMeanPerPulse
            // 
            lblMeanPerPulse.AutoSize = true;
            lblMeanPerPulse.Location = new Point(1352, 681);
            lblMeanPerPulse.Margin = new Padding(6, 0, 6, 0);
            lblMeanPerPulse.Name = "lblMeanPerPulse";
            lblMeanPerPulse.Size = new Size(78, 32);
            lblMeanPerPulse.TabIndex = 34;
            lblMeanPerPulse.Text = "label1";
            // 
            // lblPhotonFlux
            // 
            lblPhotonFlux.AutoSize = true;
            lblPhotonFlux.Location = new Point(1098, 679);
            lblPhotonFlux.Margin = new Padding(6, 0, 6, 0);
            lblPhotonFlux.Name = "lblPhotonFlux";
            lblPhotonFlux.Size = new Size(78, 32);
            lblPhotonFlux.TabIndex = 32;
            lblPhotonFlux.Text = "label1";
            // 
            // lblPowerMeter2
            // 
            lblPowerMeter2.AutoSize = true;
            lblPowerMeter2.Location = new Point(257, 1213);
            lblPowerMeter2.Margin = new Padding(6, 0, 6, 0);
            lblPowerMeter2.Name = "lblPowerMeter2";
            lblPowerMeter2.Size = new Size(91, 32);
            lblPowerMeter2.TabIndex = 28;
            lblPowerMeter2.Text = "label11";
            // 
            // numPMWavelength
            // 
            numPMWavelength.Location = new Point(254, 992);
            numPMWavelength.Margin = new Padding(6);
            numPMWavelength.Name = "numPMWavelength";
            numPMWavelength.Size = new Size(240, 39);
            numPMWavelength.TabIndex = 27;
            numPMWavelength.ValueChanged += numPMWavelength_ValueChanged;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(17, 242);
            btnRun.Margin = new Padding(6);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(184, 73);
            btnRun.TabIndex = 26;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // histogramPlot
            // 
            histogramPlot.Location = new Point(268, 56);
            histogramPlot.Margin = new Padding(6);
            histogramPlot.Name = "histogramPlot";
            histogramPlot.Size = new Size(1844, 542);
            histogramPlot.TabIndex = 25;
            // 
            // richtxtLog
            // 
            richtxtLog.Location = new Point(1694, 635);
            richtxtLog.Margin = new Padding(6);
            richtxtLog.Name = "richtxtLog";
            richtxtLog.Size = new Size(458, 407);
            richtxtLog.TabIndex = 24;
            richtxtLog.Text = "";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Location = new Point(545, 148);
            label9.Margin = new Padding(6, 0, 6, 0);
            label9.Name = "label9";
            label9.Size = new Size(167, 71);
            label9.TabIndex = 14;
            label9.Text = "Ch1 ZeroX";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Fill;
            label8.Location = new Point(366, 148);
            label8.Margin = new Padding(6, 0, 6, 0);
            label8.Name = "label8";
            label8.Size = new Size(165, 71);
            label8.TabIndex = 13;
            label8.Text = "Ch1 CFD Level";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Fill;
            label7.Location = new Point(187, 148);
            label7.Margin = new Padding(6, 0, 6, 0);
            label7.Name = "label7";
            label7.Size = new Size(165, 71);
            label7.TabIndex = 12;
            label7.Text = "Ch0 ZeroX";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Location = new Point(8, 148);
            label6.Margin = new Padding(6, 0, 6, 0);
            label6.Name = "label6";
            label6.Size = new Size(165, 71);
            label6.TabIndex = 9;
            label6.Text = "Ch0 CFD level";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(545, 2);
            label5.Margin = new Padding(6, 0, 6, 0);
            label5.Name = "label5";
            label5.Size = new Size(167, 71);
            label5.TabIndex = 11;
            label5.Text = "Resolution";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(366, 2);
            label4.Margin = new Padding(6, 0, 6, 0);
            label4.Name = "label4";
            label4.Size = new Size(165, 71);
            label4.TabIndex = 10;
            label4.Text = "Sync div";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(187, 2);
            label3.Margin = new Padding(6, 0, 6, 0);
            label3.Name = "label3";
            label3.Size = new Size(165, 71);
            label3.TabIndex = 9;
            label3.Text = "Acq. Time";
            // 
            // numBinning
            // 
            numBinning.Dock = DockStyle.Fill;
            numBinning.Location = new Point(8, 81);
            numBinning.Margin = new Padding(6);
            numBinning.Name = "numBinning";
            numBinning.Size = new Size(165, 39);
            numBinning.TabIndex = 2;
            numBinning.ValueChanged += numBinning_ValueChanged;
            // 
            // numAcqTime
            // 
            numAcqTime.Dock = DockStyle.Fill;
            numAcqTime.Location = new Point(187, 81);
            numAcqTime.Margin = new Padding(6);
            numAcqTime.Name = "numAcqTime";
            numAcqTime.Size = new Size(165, 39);
            numAcqTime.TabIndex = 2;
            numAcqTime.ValueChanged += numAcqTime_ValueChanged;
            // 
            // numSyncDiv
            // 
            numSyncDiv.Dock = DockStyle.Fill;
            numSyncDiv.Location = new Point(366, 81);
            numSyncDiv.Margin = new Padding(6);
            numSyncDiv.Name = "numSyncDiv";
            numSyncDiv.Size = new Size(165, 39);
            numSyncDiv.TabIndex = 2;
            numSyncDiv.ValueChanged += numSyncDiv_ValueChanged;
            // 
            // numCFDLevel0
            // 
            numCFDLevel0.Dock = DockStyle.Fill;
            numCFDLevel0.Location = new Point(8, 227);
            numCFDLevel0.Margin = new Padding(6);
            numCFDLevel0.Name = "numCFDLevel0";
            numCFDLevel0.Size = new Size(165, 39);
            numCFDLevel0.TabIndex = 3;
            numCFDLevel0.ValueChanged += numCFDLevel0_ValueChanged;
            // 
            // numCFDZeroCross0
            // 
            numCFDZeroCross0.Dock = DockStyle.Fill;
            numCFDZeroCross0.Location = new Point(187, 227);
            numCFDZeroCross0.Margin = new Padding(6);
            numCFDZeroCross0.Name = "numCFDZeroCross0";
            numCFDZeroCross0.Size = new Size(165, 39);
            numCFDZeroCross0.TabIndex = 4;
            numCFDZeroCross0.ValueChanged += numCFDZeroCross0_ValueChanged;
            // 
            // numCFDLevel1
            // 
            numCFDLevel1.Dock = DockStyle.Fill;
            numCFDLevel1.Location = new Point(366, 227);
            numCFDLevel1.Margin = new Padding(6);
            numCFDLevel1.Name = "numCFDLevel1";
            numCFDLevel1.Size = new Size(165, 39);
            numCFDLevel1.TabIndex = 5;
            numCFDLevel1.ValueChanged += numCFDLevel1_ValueChanged;
            // 
            // numCFDZeroCross1
            // 
            numCFDZeroCross1.Dock = DockStyle.Fill;
            numCFDZeroCross1.Location = new Point(545, 227);
            numCFDZeroCross1.Margin = new Padding(6);
            numCFDZeroCross1.Name = "numCFDZeroCross1";
            numCFDZeroCross1.Size = new Size(167, 39);
            numCFDZeroCross1.TabIndex = 6;
            numCFDZeroCross1.ValueChanged += numCFDZeroCross1_ValueChanged;
            // 
            // btnPM1ZeroAdjust
            // 
            btnPM1ZeroAdjust.Location = new Point(794, 664);
            btnPM1ZeroAdjust.Margin = new Padding(6);
            btnPM1ZeroAdjust.Name = "btnPM1ZeroAdjust";
            btnPM1ZeroAdjust.Size = new Size(180, 49);
            btnPM1ZeroAdjust.TabIndex = 35;
            btnPM1ZeroAdjust.Text = "Background";
            btnPM1ZeroAdjust.UseVisualStyleBackColor = true;
            btnPM1ZeroAdjust.Click += btnPM1ZeroAdjust_Click;
            // 
            // lblPDE
            // 
            lblPDE.AutoSize = true;
            lblPDE.Location = new Point(1520, 683);
            lblPDE.Margin = new Padding(6, 0, 6, 0);
            lblPDE.Name = "lblPDE";
            lblPDE.Size = new Size(78, 32);
            lblPDE.TabIndex = 31;
            lblPDE.Text = "label1";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(1302, 638);
            label10.Margin = new Padding(6, 0, 6, 0);
            label10.Name = "label10";
            label10.Size = new Size(183, 32);
            label10.TabIndex = 30;
            label10.Text = "Mean Per Pulse";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1008, 638);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(282, 32);
            label1.TabIndex = 33;
            label1.Text = "Incident Photon Number";
            // 
            // lblResolution
            // 
            lblResolution.AutoSize = true;
            lblResolution.BackColor = SystemColors.ButtonFace;
            lblResolution.BorderStyle = BorderStyle.FixedSingle;
            lblResolution.Dock = DockStyle.Fill;
            lblResolution.Location = new Point(545, 75);
            lblResolution.Margin = new Padding(6, 0, 6, 0);
            lblResolution.Name = "lblResolution";
            lblResolution.Size = new Size(167, 71);
            lblResolution.TabIndex = 7;
            lblResolution.Text = "label1";
            // 
            // lblPowerMeter1
            // 
            lblPowerMeter1.AutoSize = true;
            lblPowerMeter1.Location = new Point(254, 954);
            lblPowerMeter1.Margin = new Padding(6, 0, 6, 0);
            lblPowerMeter1.Name = "lblPowerMeter1";
            lblPowerMeter1.Size = new Size(91, 32);
            lblPowerMeter1.TabIndex = 22;
            lblPowerMeter1.Text = "label11";
            // 
            // lblCountRate0
            // 
            lblCountRate0.Name = "lblCountRate0";
            lblCountRate0.Size = new Size(148, 32);
            lblCountRate0.Text = "CountRate 0";
            // 
            // lblCountRate1
            // 
            lblCountRate1.Name = "lblCountRate1";
            lblCountRate1.Size = new Size(148, 32);
            lblCountRate1.Text = "CountRate 1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(79, 32);
            lblStatus.Text = "Status";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(32, 32);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, lblCountRate1, lblCountRate0 });
            statusStrip1.Location = new Point(0, 1072);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(28, 0, 2, 0);
            statusStrip1.RightToLeft = RightToLeft.Yes;
            statusStrip1.Size = new Size(2236, 42);
            statusStrip1.TabIndex = 21;
            statusStrip1.Text = "statusStrip1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(8, 2);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(165, 71);
            label2.TabIndex = 8;
            label2.Text = "Binning";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(1520, 640);
            label11.Margin = new Padding(6, 0, 6, 0);
            label11.Name = "label11";
            label11.Size = new Size(57, 32);
            label11.TabIndex = 29;
            label11.Text = "PDE";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(68, 954);
            label12.Margin = new Padding(6, 0, 6, 0);
            label12.Name = "label12";
            label12.Size = new Size(177, 32);
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
            tableLayoutPanel1.Location = new Point(14, 638);
            tableLayoutPanel1.Margin = new Padding(6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(720, 297);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(17, 328);
            btnSave.Margin = new Padding(6);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(184, 73);
            btnSave.TabIndex = 18;
            btnSave.Text = "Save CSV";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnMeasure
            // 
            btnMeasure.Location = new Point(17, 157);
            btnMeasure.Margin = new Padding(6);
            btnMeasure.Name = "btnMeasure";
            btnMeasure.Size = new Size(184, 73);
            btnMeasure.TabIndex = 19;
            btnMeasure.Text = "Measure";
            btnMeasure.UseVisualStyleBackColor = true;
            btnMeasure.Click += btnMeasure_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(17, 72);
            btnConnect.Margin = new Padding(6);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(184, 73);
            btnConnect.TabIndex = 17;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // ucMainView
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
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
            Name = "ucMainView";
            Size = new Size(2236, 1114);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMeanPerPulse;
        private Label lblPhotonFlux;
        private Label lblPowerMeter2;
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
    }
}
