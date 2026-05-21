namespace SpadApp
{
    partial class ucJitter
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            numLaserPulseWidth = new NumericUpDown();
            numTcspcJitter = new NumericUpDown();
            btnJitterMeasure = new Button();
            label1 = new Label();
            label2 = new Label();
            lblSystemJitter = new Label();
            lblSpadJitter = new Label();
            label3 = new Label();
            label4 = new Label();
            JitterPlot = new ScottPlot.WinForms.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)numLaserPulseWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTcspcJitter).BeginInit();
            SuspendLayout();
            // 
            // numLaserPulseWidth
            // 
            numLaserPulseWidth.Location = new Point(153, 215);
            numLaserPulseWidth.Name = "numLaserPulseWidth";
            numLaserPulseWidth.Size = new Size(120, 23);
            numLaserPulseWidth.TabIndex = 0;
            // 
            // numTcspcJitter
            // 
            numTcspcJitter.Location = new Point(155, 267);
            numTcspcJitter.Name = "numTcspcJitter";
            numTcspcJitter.Size = new Size(120, 23);
            numTcspcJitter.TabIndex = 1;
            // 
            // btnJitterMeasure
            // 
            btnJitterMeasure.Location = new Point(153, 152);
            btnJitterMeasure.Name = "btnJitterMeasure";
            btnJitterMeasure.Size = new Size(122, 34);
            btnJitterMeasure.TabIndex = 2;
            btnJitterMeasure.Text = "Measure";
            btnJitterMeasure.UseVisualStyleBackColor = true;
            btnJitterMeasure.Click += btnJitterMeasure_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(72, 319);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 3;
            label1.Text = "System Jitter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 351);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 3;
            label2.Text = "SPAD Jitter";
            // 
            // lblSystemJitter
            // 
            lblSystemJitter.AutoSize = true;
            lblSystemJitter.Location = new Point(149, 319);
            lblSystemJitter.Name = "lblSystemJitter";
            lblSystemJitter.Size = new Size(72, 15);
            lblSystemJitter.TabIndex = 3;
            lblSystemJitter.Text = "system jitter";
            // 
            // lblSpadJitter
            // 
            lblSpadJitter.AutoSize = true;
            lblSpadJitter.Location = new Point(149, 351);
            lblSpadJitter.Name = "lblSpadJitter";
            lblSpadJitter.Size = new Size(60, 15);
            lblSpadJitter.TabIndex = 3;
            lblSpadJitter.Text = "spad jitter";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(21, 217);
            label3.Name = "label3";
            label3.Size = new Size(126, 15);
            label3.TabIndex = 3;
            label3.Text = "Laser Pulse Width [ps]";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(44, 269);
            label4.Name = "label4";
            label4.Size = new Size(96, 15);
            label4.TabIndex = 3;
            label4.Text = "TCSPC Jitter [ps]";
            // 
            // JitterPlot
            // 
            JitterPlot.Location = new Point(346, 32);
            JitterPlot.Name = "JitterPlot";
            JitterPlot.Size = new Size(646, 258);
            JitterPlot.TabIndex = 4;
            // 
            // ucJitter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(JitterPlot);
            Controls.Add(lblSpadJitter);
            Controls.Add(lblSystemJitter);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(btnJitterMeasure);
            Controls.Add(numTcspcJitter);
            Controls.Add(numLaserPulseWidth);
            Name = "ucJitter";
            Size = new Size(1200, 600);
            Load += ucJitter_Load;
            ((System.ComponentModel.ISupportInitialize)numLaserPulseWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTcspcJitter).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown numLaserPulseWidth;
        private NumericUpDown numTcspcJitter;
        private Button btnJitterMeasure;
        private Label label1;
        private Label label2;
        private Label lblSystemJitter;
        private Label lblSpadJitter;
        private Label label3;
        private Label label4;
        private ScottPlot.WinForms.FormsPlot JitterPlot;
    }
}
