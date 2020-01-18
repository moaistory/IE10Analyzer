namespace IE10Analyzer
{
    partial class FormSave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSave));
            this.buttonSave = new System.Windows.Forms.Button();
            this.listViewTable = new System.Windows.Forms.ListView();
            this.columnHeaderNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTableName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonCancle = new System.Windows.Forms.Button();
            this.buttonCheckAll = new System.Windows.Forms.Button();
            this.buttonUnCheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(395, 9);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(85, 23);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // listViewTable
            // 
            this.listViewTable.CheckBoxes = true;
            this.listViewTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNumber,
            this.columnHeaderTableName,
            this.columnHeaderCount});
            this.listViewTable.FullRowSelect = true;
            this.listViewTable.Location = new System.Drawing.Point(9, 12);
            this.listViewTable.Name = "listViewTable";
            this.listViewTable.Size = new System.Drawing.Size(367, 438);
            this.listViewTable.TabIndex = 1;
            this.listViewTable.UseCompatibleStateImageBehavior = false;
            this.listViewTable.View = System.Windows.Forms.View.Details;
            this.listViewTable.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTable_ColumnClick);
            // 
            // columnHeaderNumber
            // 
            this.columnHeaderNumber.Tag = "Numeric";
            this.columnHeaderNumber.Text = "No.";
            // 
            // columnHeaderTableName
            // 
            this.columnHeaderTableName.Tag = "Text";
            this.columnHeaderTableName.Text = "Table Name";
            this.columnHeaderTableName.Width = 227;
            // 
            // columnHeaderCount
            // 
            this.columnHeaderCount.Tag = "Numeric";
            this.columnHeaderCount.Text = "Count";
            this.columnHeaderCount.Width = 80;
            // 
            // buttonCancle
            // 
            this.buttonCancle.Location = new System.Drawing.Point(395, 41);
            this.buttonCancle.Name = "buttonCancle";
            this.buttonCancle.Size = new System.Drawing.Size(86, 23);
            this.buttonCancle.TabIndex = 2;
            this.buttonCancle.Text = "Cancle";
            this.buttonCancle.UseVisualStyleBackColor = true;
            this.buttonCancle.Click += new System.EventHandler(this.buttonCancle_Click);
            // 
            // buttonCheckAll
            // 
            this.buttonCheckAll.Location = new System.Drawing.Point(395, 93);
            this.buttonCheckAll.Name = "buttonCheckAll";
            this.buttonCheckAll.Size = new System.Drawing.Size(85, 23);
            this.buttonCheckAll.TabIndex = 3;
            this.buttonCheckAll.Text = "Check All";
            this.buttonCheckAll.UseVisualStyleBackColor = true;
            this.buttonCheckAll.Click += new System.EventHandler(this.buttonCheckAll_Click);
            // 
            // buttonUnCheck
            // 
            this.buttonUnCheck.Location = new System.Drawing.Point(395, 124);
            this.buttonUnCheck.Name = "buttonUnCheck";
            this.buttonUnCheck.Size = new System.Drawing.Size(84, 23);
            this.buttonUnCheck.TabIndex = 4;
            this.buttonUnCheck.Text = "Uncheck";
            this.buttonUnCheck.UseVisualStyleBackColor = true;
            this.buttonUnCheck.Click += new System.EventHandler(this.buttonUnCheck_Click);
            // 
            // FormSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 462);
            this.Controls.Add(this.buttonUnCheck);
            this.Controls.Add(this.buttonCheckAll);
            this.Controls.Add(this.buttonCancle);
            this.Controls.Add(this.listViewTable);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSave";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save As...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ListView listViewTable;
        private System.Windows.Forms.Button buttonCancle;
        private System.Windows.Forms.ColumnHeader columnHeaderNumber;
        private System.Windows.Forms.ColumnHeader columnHeaderTableName;
        private System.Windows.Forms.ColumnHeader columnHeaderCount;
        private System.Windows.Forms.Button buttonCheckAll;
        private System.Windows.Forms.Button buttonUnCheck;
    }
}