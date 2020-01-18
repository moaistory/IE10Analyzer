namespace IE10Analyzer
{
    partial class DockTableList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockTableList));
            this.listViewTable = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTableName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewTable
            // 
            this.listViewTable.BackColor = System.Drawing.SystemColors.Window;
            this.listViewTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeaderTableName,
            this.columnHeaderCount});
            this.listViewTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTable.FullRowSelect = true;
            this.listViewTable.HideSelection = false;
            this.listViewTable.Location = new System.Drawing.Point(0, 0);
            this.listViewTable.MultiSelect = false;
            this.listViewTable.Name = "listViewTable";
            this.listViewTable.Size = new System.Drawing.Size(184, 500);
            this.listViewTable.TabIndex = 0;
            this.listViewTable.UseCompatibleStateImageBehavior = false;
            this.listViewTable.View = System.Windows.Forms.View.Details;
            this.listViewTable.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTable_ColumnClick);
            this.listViewTable.SelectedIndexChanged += new System.EventHandler(this.listViewTable_SelectedIndexChanged);
            // 
            // columnHeader
            // 
            this.columnHeader.Tag = "Numeric";
            this.columnHeader.Text = "No.";
            // 
            // columnHeaderTableName
            // 
            this.columnHeaderTableName.Tag = "Text";
            this.columnHeaderTableName.Text = "Table Name";
            this.columnHeaderTableName.Width = 254;
            // 
            // columnHeaderCount
            // 
            this.columnHeaderCount.Tag = "Numeric";
            this.columnHeaderCount.Text = "Count";
            this.columnHeaderCount.Width = 100;
            // 
            // DockTableList
            // 
            this.AllowEndUserDocking = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(184, 500);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.listViewTable);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DockTableList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Table";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTable;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeaderTableName;
        private System.Windows.Forms.ColumnHeader columnHeaderCount;
    }
}