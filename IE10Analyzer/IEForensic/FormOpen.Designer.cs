﻿namespace IE10Analyzer
{
    partial class FormOpen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOpen));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonOpen = new System.Windows.Forms.RadioButton();
            this.radioButtonExtract = new System.Windows.Forms.RadioButton();
            this.groupBoxWorking = new System.Windows.Forms.GroupBox();
            this.buttonWorkingDirectory = new System.Windows.Forms.Button();
            this.textBoxWorkingDirectory = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonRecovery = new System.Windows.Forms.RadioButton();
            this.radioButtonParsing = new System.Windows.Forms.RadioButton();
            this.radioButtonUsingAPI = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxBrowser = new System.Windows.Forms.ComboBox();
            this.labelFileStatus = new System.Windows.Forms.Label();
            this.buttonFilePath = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonCarving = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBoxWorking.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonOpen);
            this.groupBox1.Controls.Add(this.radioButtonExtract);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 86);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "How to open";
            // 
            // radioButtonOpen
            // 
            this.radioButtonOpen.AutoSize = true;
            this.radioButtonOpen.Location = new System.Drawing.Point(7, 55);
            this.radioButtonOpen.Name = "radioButtonOpen";
            this.radioButtonOpen.Size = new System.Drawing.Size(348, 16);
            this.radioButtonOpen.TabIndex = 1;
            this.radioButtonOpen.TabStop = true;
            this.radioButtonOpen.Text = "Open WebCacheV01.dat that have already been extracted";
            this.radioButtonOpen.UseVisualStyleBackColor = true;
            this.radioButtonOpen.CheckedChanged += new System.EventHandler(this.radioButtonOpen_CheckedChanged);
            // 
            // radioButtonExtract
            // 
            this.radioButtonExtract.AutoSize = true;
            this.radioButtonExtract.Location = new System.Drawing.Point(7, 26);
            this.radioButtonExtract.Name = "radioButtonExtract";
            this.radioButtonExtract.Size = new System.Drawing.Size(412, 16);
            this.radioButtonExtract.TabIndex = 0;
            this.radioButtonExtract.TabStop = true;
            this.radioButtonExtract.Text = "Extract WebCacheV01.dat from current system and then open the file";
            this.radioButtonExtract.UseVisualStyleBackColor = true;
            this.radioButtonExtract.CheckedChanged += new System.EventHandler(this.radioButtonExtract_CheckedChanged);
            // 
            // groupBoxWorking
            // 
            this.groupBoxWorking.Controls.Add(this.buttonWorkingDirectory);
            this.groupBoxWorking.Controls.Add(this.textBoxWorkingDirectory);
            this.groupBoxWorking.Location = new System.Drawing.Point(12, 334);
            this.groupBoxWorking.Name = "groupBoxWorking";
            this.groupBoxWorking.Size = new System.Drawing.Size(451, 55);
            this.groupBoxWorking.TabIndex = 1;
            this.groupBoxWorking.TabStop = false;
            this.groupBoxWorking.Text = "Working directory";
            // 
            // buttonWorkingDirectory
            // 
            this.buttonWorkingDirectory.Location = new System.Drawing.Point(403, 20);
            this.buttonWorkingDirectory.Name = "buttonWorkingDirectory";
            this.buttonWorkingDirectory.Size = new System.Drawing.Size(42, 23);
            this.buttonWorkingDirectory.TabIndex = 1;
            this.buttonWorkingDirectory.Text = "...";
            this.buttonWorkingDirectory.UseVisualStyleBackColor = true;
            this.buttonWorkingDirectory.Click += new System.EventHandler(this.buttonWorkingDirectory_Click);
            // 
            // textBoxWorkingDirectory
            // 
            this.textBoxWorkingDirectory.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBoxWorkingDirectory.Location = new System.Drawing.Point(7, 22);
            this.textBoxWorkingDirectory.Name = "textBoxWorkingDirectory";
            this.textBoxWorkingDirectory.ReadOnly = true;
            this.textBoxWorkingDirectory.Size = new System.Drawing.Size(389, 21);
            this.textBoxWorkingDirectory.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonCarving);
            this.groupBox3.Controls.Add(this.radioButtonRecovery);
            this.groupBox3.Controls.Add(this.radioButtonParsing);
            this.groupBox3.Controls.Add(this.radioButtonUsingAPI);
            this.groupBox3.Location = new System.Drawing.Point(13, 231);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(451, 87);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Analysis method";
            // 
            // radioButtonRecovery
            // 
            this.radioButtonRecovery.AutoSize = true;
            this.radioButtonRecovery.Location = new System.Drawing.Point(6, 57);
            this.radioButtonRecovery.Name = "radioButtonRecovery";
            this.radioButtonRecovery.Size = new System.Drawing.Size(168, 16);
            this.radioButtonRecovery.TabIndex = 2;
            this.radioButtonRecovery.TabStop = true;
            this.radioButtonRecovery.Text = "Recovery deleted records";
            this.radioButtonRecovery.UseVisualStyleBackColor = true;
            this.radioButtonRecovery.CheckedChanged += new System.EventHandler(this.radioButtonRecovery_CheckedChanged);
            // 
            // radioButtonParsing
            // 
            this.radioButtonParsing.AutoSize = true;
            this.radioButtonParsing.Location = new System.Drawing.Point(239, 25);
            this.radioButtonParsing.Name = "radioButtonParsing";
            this.radioButtonParsing.Size = new System.Drawing.Size(156, 16);
            this.radioButtonParsing.TabIndex = 1;
            this.radioButtonParsing.TabStop = true;
            this.radioButtonParsing.Text = "Parsing normal records";
            this.radioButtonParsing.UseVisualStyleBackColor = true;
            this.radioButtonParsing.CheckedChanged += new System.EventHandler(this.radioButtonParsing_CheckedChanged);
            // 
            // radioButtonUsingAPI
            // 
            this.radioButtonUsingAPI.AutoSize = true;
            this.radioButtonUsingAPI.Location = new System.Drawing.Point(7, 25);
            this.radioButtonUsingAPI.Name = "radioButtonUsingAPI";
            this.radioButtonUsingAPI.Size = new System.Drawing.Size(78, 16);
            this.radioButtonUsingAPI.TabIndex = 0;
            this.radioButtonUsingAPI.TabStop = true;
            this.radioButtonUsingAPI.Text = "Using API";
            this.radioButtonUsingAPI.UseVisualStyleBackColor = true;
            this.radioButtonUsingAPI.CheckedChanged += new System.EventHandler(this.radioButtonUsingAPI_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxBrowser);
            this.groupBox4.Controls.Add(this.labelFileStatus);
            this.groupBox4.Controls.Add(this.buttonFilePath);
            this.groupBox4.Controls.Add(this.textBoxFilePath);
            this.groupBox4.Location = new System.Drawing.Point(12, 108);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(451, 113);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "File path";
            // 
            // comboBoxBrowser
            // 
            this.comboBoxBrowser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBrowser.FormattingEnabled = true;
            this.comboBoxBrowser.Items.AddRange(new object[] {
            "Internet Explorer Browser (10 or 11) and Edge"});
            this.comboBoxBrowser.Location = new System.Drawing.Point(7, 21);
            this.comboBoxBrowser.Name = "comboBoxBrowser";
            this.comboBoxBrowser.Size = new System.Drawing.Size(438, 20);
            this.comboBoxBrowser.TabIndex = 3;
            this.comboBoxBrowser.SelectedIndexChanged += new System.EventHandler(this.comboBoxBrowser_SelectedIndexChanged);
            // 
            // labelFileStatus
            // 
            this.labelFileStatus.AutoSize = true;
            this.labelFileStatus.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.labelFileStatus.Location = new System.Drawing.Point(9, 91);
            this.labelFileStatus.Name = "labelFileStatus";
            this.labelFileStatus.Size = new System.Drawing.Size(75, 12);
            this.labelFileStatus.TabIndex = 2;
            this.labelFileStatus.Text = "File status : ";
            // 
            // buttonFilePath
            // 
            this.buttonFilePath.Location = new System.Drawing.Point(403, 57);
            this.buttonFilePath.Name = "buttonFilePath";
            this.buttonFilePath.Size = new System.Drawing.Size(42, 23);
            this.buttonFilePath.TabIndex = 1;
            this.buttonFilePath.Text = "...";
            this.buttonFilePath.UseVisualStyleBackColor = true;
            this.buttonFilePath.Click += new System.EventHandler(this.buttonFilePath_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBoxFilePath.Location = new System.Drawing.Point(7, 59);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.ReadOnly = true;
            this.textBoxFilePath.Size = new System.Drawing.Size(389, 21);
            this.textBoxFilePath.TabIndex = 0;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(304, 402);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 3;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(388, 403);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // radioButtonCarving
            // 
            this.radioButtonCarving.AutoSize = true;
            this.radioButtonCarving.Location = new System.Drawing.Point(239, 57);
            this.radioButtonCarving.Name = "radioButtonCarving";
            this.radioButtonCarving.Size = new System.Drawing.Size(113, 16);
            this.radioButtonCarving.TabIndex = 3;
            this.radioButtonCarving.TabStop = true;
            this.radioButtonCarving.Text = "Carving records";
            this.radioButtonCarving.UseVisualStyleBackColor = true;
            this.radioButtonCarving.CheckedChanged += new System.EventHandler(this.radioButtonCarving_CheckedChanged);
            // 
            // FormOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 438);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxWorking);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOpen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOpen_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxWorking.ResumeLayout(false);
            this.groupBoxWorking.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonOpen;
        private System.Windows.Forms.RadioButton radioButtonExtract;
        private System.Windows.Forms.GroupBox groupBoxWorking;
        private System.Windows.Forms.Button buttonWorkingDirectory;
        private System.Windows.Forms.TextBox textBoxWorkingDirectory;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonRecovery;
        private System.Windows.Forms.RadioButton radioButtonParsing;
        private System.Windows.Forms.RadioButton radioButtonUsingAPI;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label labelFileStatus;
        private System.Windows.Forms.Button buttonFilePath;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxBrowser;
        private System.Windows.Forms.RadioButton radioButtonCarving;
    }
}