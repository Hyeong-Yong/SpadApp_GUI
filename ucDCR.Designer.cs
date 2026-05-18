namespace SpadApp
{
    partial class ucDCR
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
            lblAverageDCR = new Label();
            lblProcessPercent = new Label();
            progressBar1 = new ProgressBar();
            label4 = new Label();
            label2 = new Label();
            numAverageCount = new NumericUpDown();
            btnMeasure = new Button();
            DcrPlot = new ScottPlot.WinForms.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)numAverageCount).BeginInit();
            SuspendLayout();
            // 
            // lblAverageDCR
            // 
            lblAverageDCR.AutoSize = true;
            lblAverageDCR.BorderStyle = BorderStyle.FixedSingle;
            lblAverageDCR.Location = new Point(28, 298);
            lblAverageDCR.Name = "lblAverageDCR";
            lblAverageDCR.Size = new Size(87, 17);
            lblAverageDCR.TabIndex = 12;
            lblAverageDCR.Text = "Averaged DCR";
            // 
            // lblProcessPercent
            // 
            lblProcessPercent.AutoSize = true;
            lblProcessPercent.Location = new Point(158, 423);
            lblProcessPercent.Name = "lblProcessPercent";
            lblProcessPercent.Size = new Size(96, 15);
            lblProcessPercent.TabIndex = 11;
            lblProcessPercent.Text = "Progress percent";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(52, 415);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(100, 23);
            progressBar1.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(28, 271);
            label4.Name = "label4";
            label4.Size = new Size(85, 15);
            label4.TabIndex = 8;
            label4.Text = "Averaged DCR";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 198);
            label2.Name = "label2";
            label2.Size = new Size(96, 15);
            label2.TabIndex = 9;
            label2.Text = "Averaging Times";
            // 
            // numAverageCount
            // 
            numAverageCount.Location = new Point(17, 226);
            numAverageCount.Name = "numAverageCount";
            numAverageCount.Size = new Size(135, 23);
            numAverageCount.TabIndex = 7;
            // 
            // btnMeasure
            // 
            btnMeasure.Location = new Point(17, 127);
            btnMeasure.Name = "btnMeasure";
            btnMeasure.Size = new Size(135, 53);
            btnMeasure.TabIndex = 6;
            btnMeasure.Text = "Measure";
            btnMeasure.UseVisualStyleBackColor = true;
            btnMeasure.Click += btnMeasure_Click;
            // 
            // DcrPlot
            // 
            DcrPlot.Location = new Point(199, 70);
            DcrPlot.Name = "DcrPlot";
            DcrPlot.Size = new Size(493, 297);
            DcrPlot.TabIndex = 13;
            // 
            // ucDCR
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(DcrPlot);
            Controls.Add(lblAverageDCR);
            Controls.Add(lblProcessPercent);
            Controls.Add(progressBar1);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(numAverageCount);
            Controls.Add(btnMeasure);
            Name = "ucDCR";
            Size = new Size(1200, 600);
            ((System.ComponentModel.ISupportInitialize)numAverageCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAverageDCR;
        private Label lblProcessPercent;
        private ProgressBar progressBar1;
        private Label label4;
        private Label label2;
        private NumericUpDown numAverageCount;
        private Button btnMeasure;
        private ScottPlot.WinForms.FormsPlot DcrPlot;
    }
}
