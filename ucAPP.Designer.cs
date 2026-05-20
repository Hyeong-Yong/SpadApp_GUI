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
        private void InitializeComponent() {
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            btnAPPmeasure = new Button();
            numericUpDown1 = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // formsPlot1
            // 
            formsPlot1.Location = new Point(375, 27);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(559, 255);
            formsPlot1.TabIndex = 0;
            // 
            // btnAPPmeasure
            // 
            btnAPPmeasure.Location = new Point(80, 71);
            btnAPPmeasure.Name = "btnAPPmeasure";
            btnAPPmeasure.Size = new Size(146, 53);
            btnAPPmeasure.TabIndex = 1;
            btnAPPmeasure.Text = "Measure";
            btnAPPmeasure.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(106, 146);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(120, 23);
            numericUpDown1.TabIndex = 2;
            // 
            // ucAPP
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(numericUpDown1);
            Controls.Add(btnAPPmeasure);
            Controls.Add(formsPlot1);
            Name = "ucAPP";
            Size = new Size(1200, 600);
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private Button btnAPPmeasure;
        private NumericUpDown numericUpDown1;
    }
}
