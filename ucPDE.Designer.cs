namespace SpadApp
{
    partial class ucPDE
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
            label1 = new Label();
            lbl_DCR = new Label();
            lbl_PDE = new Label();
            label2 = new Label();
            PDEplot = new ScottPlot.WinForms.FormsPlot();
            btnPDEmeasure = new Button();
            lbl_DPN = new Label();
            label4 = new Label();
            progressBarPDE = new ProgressBar();
            label5 = new Label();
            label6 = new Label();
            lbl_IPN = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(74, 147);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 0;
            label1.Text = "Averaged DCR";
            // 
            // lbl_DCR
            // 
            lbl_DCR.AutoSize = true;
            lbl_DCR.BorderStyle = BorderStyle.FixedSingle;
            lbl_DCR.Location = new Point(87, 179);
            lbl_DCR.Name = "lbl_DCR";
            lbl_DCR.Size = new Size(46, 17);
            lbl_DCR.TabIndex = 0;
            lbl_DCR.Text = "lblDCR";
            // 
            // lbl_PDE
            // 
            lbl_PDE.AutoSize = true;
            lbl_PDE.BorderStyle = BorderStyle.FixedSingle;
            lbl_PDE.Location = new Point(131, 374);
            lbl_PDE.Name = "lbl_PDE";
            lbl_PDE.Size = new Size(45, 17);
            lbl_PDE.TabIndex = 1;
            lbl_PDE.Text = "N_PDE";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(76, 338);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 2;
            label2.Text = "Averaged PDE";
            // 
            // PDEplot
            // 
            PDEplot.Location = new Point(244, 46);
            PDEplot.Name = "PDEplot";
            PDEplot.Size = new Size(574, 220);
            PDEplot.TabIndex = 3;
            // 
            // btnPDEmeasure
            // 
            btnPDEmeasure.Location = new Point(34, 74);
            btnPDEmeasure.Name = "btnPDEmeasure";
            btnPDEmeasure.Size = new Size(139, 54);
            btnPDEmeasure.TabIndex = 4;
            btnPDEmeasure.Text = "Measure";
            btnPDEmeasure.UseVisualStyleBackColor = true;
            btnPDEmeasure.Click += btnPDEmeasure_Click;
            // 
            // lbl_DPN
            // 
            lbl_DPN.AutoSize = true;
            lbl_DPN.BorderStyle = BorderStyle.FixedSingle;
            lbl_DPN.Location = new Point(87, 228);
            lbl_DPN.Name = "lbl_DPN";
            lbl_DPN.Size = new Size(52, 17);
            lbl_DPN.TabIndex = 5;
            lbl_DPN.Text = "lbl_DPN";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(28, 204);
            label4.Name = "label4";
            label4.Size = new Size(146, 15);
            label4.TabIndex = 6;
            label4.Text = "Detected Photon Number";
            // 
            // progressBarPDE
            // 
            progressBarPDE.Location = new Point(337, 320);
            progressBarPDE.Name = "progressBarPDE";
            progressBarPDE.Size = new Size(100, 23);
            progressBarPDE.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(345, 301);
            label5.Name = "label5";
            label5.Size = new Size(98, 15);
            label5.TabIndex = 8;
            label5.Text = "ProcessBar (PDE)";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(34, 264);
            label6.Name = "label6";
            label6.Size = new Size(141, 15);
            label6.TabIndex = 10;
            label6.Text = "Incident Photon Number";
            // 
            // lbl_IPN
            // 
            lbl_IPN.AutoSize = true;
            lbl_IPN.BorderStyle = BorderStyle.FixedSingle;
            lbl_IPN.Location = new Point(93, 288);
            lbl_IPN.Name = "lbl_IPN";
            lbl_IPN.Size = new Size(46, 17);
            lbl_IPN.TabIndex = 9;
            lbl_IPN.Text = "lbl_IPN";
            // 
            // ucPDE
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label6);
            Controls.Add(lbl_IPN);
            Controls.Add(label5);
            Controls.Add(progressBarPDE);
            Controls.Add(label4);
            Controls.Add(lbl_DPN);
            Controls.Add(btnPDEmeasure);
            Controls.Add(PDEplot);
            Controls.Add(label2);
            Controls.Add(lbl_PDE);
            Controls.Add(lbl_DCR);
            Controls.Add(label1);
            Name = "ucPDE";
            Size = new Size(1200, 600);
            Load += ucPDE_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label lbl_DCR;
        private Label lbl_PDE;
        private Label label2;
        private ScottPlot.WinForms.FormsPlot PDEplot;
        private Button btnPDEmeasure;
        private Label lbl_DPN;
        private Label label4;
        private ProgressBar progressBarPDE;
        private Label label5;
        private Label label6;
        private Label lbl_IPN;
    }
}
