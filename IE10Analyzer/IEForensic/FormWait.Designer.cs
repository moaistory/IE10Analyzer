namespace IE10Analyzer
{
    partial class FormWait
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWait));
            this.label = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.labelPecentage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(12, 11);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(197, 12);
            this.label.TabIndex = 0;
            this.label.Text = "Please wait while loading records";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(14, 37);
            this.progressBar.MarqueeAnimationSpeed = 150;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(418, 23);
            this.progressBar.TabIndex = 1;
            // 
            // labelPecentage
            // 
            this.labelPecentage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelPecentage.Location = new System.Drawing.Point(332, 11);
            this.labelPecentage.Name = "labelPecentage";
            this.labelPecentage.Size = new System.Drawing.Size(100, 12);
            this.labelPecentage.TabIndex = 2;
            this.labelPecentage.Text = "(0/0)";
            this.labelPecentage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 76);
            this.Controls.Add(this.labelPecentage);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWait";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wait";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelPecentage;
    }
}