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
        private void InitializeComponent()
        {
            btnConnect = new Button();
            btnDisconnect = new Button();
            btnMeasure = new Button();
            btnStop = new Button();
            btnSave = new Button();
            numBinning = new NumericUpDown();
            numAcqTime = new NumericUpDown();
            numSyncDiv = new NumericUpDown();
            tableLayoutPanel1 = new TableLayoutPanel();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            numCFDLevel0 = new NumericUpDown();
            numCFDZeroCross0 = new NumericUpDown();
            numCFDLevel1 = new NumericUpDown();
            numCFDZeroCross1 = new NumericUpDown();
            lblResolution = new Label();
            label2 = new Label();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblCountRate1 = new ToolStripStatusLabel();
            lblCountRate0 = new ToolStripStatusLabel();
            numTacq = new Label();
            label12 = new Label();
            richtxtLog = new RichTextBox();
            histogramPlot = new ScottPlot.WinForms.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)numBinning).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAcqTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSyncDiv).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross1).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 41);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(189, 59);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(217, 41);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(189, 59);
            btnDisconnect.TabIndex = 0;
            btnDisconnect.Text = "DisConnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnMeasure
            // 
            btnMeasure.Location = new Point(12, 106);
            btnMeasure.Name = "btnMeasure";
            btnMeasure.Size = new Size(189, 59);
            btnMeasure.TabIndex = 0;
            btnMeasure.Text = "Measure";
            btnMeasure.UseVisualStyleBackColor = true;
            btnMeasure.Click += btnMeasure_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(217, 106);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(189, 59);
            btnStop.TabIndex = 0;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 171);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(189, 59);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save CSV";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
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
            tableLayoutPanel1.Location = new Point(12, 236);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(360, 139);
            tableLayoutPanel1.TabIndex = 4;
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
            label5.Text = "Resolution";
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
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, lblCountRate1, lblCountRate0 });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RightToLeft = RightToLeft.Yes;
            statusStrip1.Size = new Size(1170, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(40, 17);
            lblStatus.Text = "Status";
            // 
            // lblCountRate1
            // 
            lblCountRate1.Name = "lblCountRate1";
            lblCountRate1.Size = new Size(74, 17);
            lblCountRate1.Text = "CountRate 1";
            // 
            // lblCountRate0
            // 
            lblCountRate0.Name = "lblCountRate0";
            lblCountRate0.Size = new Size(74, 17);
            lblCountRate0.Text = "CountRate 0";
            // 
            // numTacq
            // 
            numTacq.AutoSize = true;
            numTacq.Location = new Point(132, 384);
            numTacq.Name = "numTacq";
            numTacq.Size = new Size(46, 15);
            numTacq.TabIndex = 7;
            numTacq.Text = "label11";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(12, 384);
            label12.Name = "label12";
            label12.Size = new Size(110, 15);
            label12.TabIndex = 8;
            label12.Text = "Measurement Time";
            // 
            // richtxtLog
            // 
            richtxtLog.Location = new Point(496, 274);
            richtxtLog.Name = "richtxtLog";
            richtxtLog.Size = new Size(419, 102);
            richtxtLog.TabIndex = 10;
            richtxtLog.Text = "";
            // 
            // histogramPlot
            // 
            histogramPlot.Location = new Point(473, 27);
            histogramPlot.Name = "histogramPlot";
            histogramPlot.Size = new Size(442, 212);
            histogramPlot.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1170, 450);
            Controls.Add(histogramPlot);
            Controls.Add(richtxtLog);
            Controls.Add(label12);
            Controls.Add(numTacq);
            Controls.Add(statusStrip1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(btnSave);
            Controls.Add(btnStop);
            Controls.Add(btnMeasure);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)numBinning).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAcqTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSyncDiv).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel0).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross0).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDLevel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCFDZeroCross1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnMeasure;
        private Button btnStop;
        private Button btnSave;
        private NumericUpDown numBinning;
        private NumericUpDown numAcqTime;
        private NumericUpDown numSyncDiv;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label5;
        private Label label4;
        private Label label3;
        private NumericUpDown numCFDLevel0;
        private NumericUpDown numCFDZeroCross0;
        private NumericUpDown numCFDLevel1;
        private NumericUpDown numCFDZeroCross1;
        private Label lblResolution;
        private Label label2;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblCountRate1;
        private ToolStripStatusLabel lblCountRate0;
        private Label numTacq;
        private Label label12;
        private RichTextBox richtxtLog;
        private ScottPlot.WinForms.FormsPlot histogramPlot;
    }
}
