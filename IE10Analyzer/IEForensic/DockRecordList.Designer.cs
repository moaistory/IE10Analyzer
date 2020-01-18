namespace IE10Analyzer
{
    partial class DockRecordList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockRecordList));
            this.listViewData = new System.Windows.Forms.ListView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedReocrdsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRecordsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewData
            // 
            this.listViewData.CheckBoxes = true;
            this.listViewData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewData.FullRowSelect = true;
            this.listViewData.Location = new System.Drawing.Point(0, 0);
            this.listViewData.MultiSelect = false;
            this.listViewData.Name = "listViewData";
            this.listViewData.Size = new System.Drawing.Size(564, 500);
            this.listViewData.TabIndex = 0;
            this.listViewData.UseCompatibleStateImageBehavior = false;
            this.listViewData.View = System.Windows.Forms.View.Details;
            this.listViewData.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewData_ColumnClick);
            this.listViewData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewData_MouseClick);
            this.listViewData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewData_MouseDoubleClick);
            this.listViewData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewData_MouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.exportToCSVToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 92);
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("checkToolStripMenuItem.Image")));
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.checkToolStripMenuItem.Text = "Check";
            this.checkToolStripMenuItem.Click += new System.EventHandler(this.checkToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkedReocrdsToolStripMenuItem,
            this.allRecordsToolStripMenuItem});
            this.viewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewToolStripMenuItem.Image")));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // checkedReocrdsToolStripMenuItem
            // 
            this.checkedReocrdsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("checkedReocrdsToolStripMenuItem.Image")));
            this.checkedReocrdsToolStripMenuItem.Name = "checkedReocrdsToolStripMenuItem";
            this.checkedReocrdsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.checkedReocrdsToolStripMenuItem.Text = "Checked reocrds";
            this.checkedReocrdsToolStripMenuItem.Click += new System.EventHandler(this.checkedReocrdsToolStripMenuItem_Click);
            // 
            // allRecordsToolStripMenuItem
            // 
            this.allRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("allRecordsToolStripMenuItem.Image")));
            this.allRecordsToolStripMenuItem.Name = "allRecordsToolStripMenuItem";
            this.allRecordsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.allRecordsToolStripMenuItem.Text = "All records";
            this.allRecordsToolStripMenuItem.Click += new System.EventHandler(this.allRecordsToolStripMenuItem_Click);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkedRecordsToolStripMenuItem,
            this.allRecordsToolStripMenuItem1});
            this.exportToCSVToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportToCSVToolStripMenuItem.Image")));
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export to CSV";
            // 
            // checkedRecordsToolStripMenuItem
            // 
            this.checkedRecordsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("checkedRecordsToolStripMenuItem.Image")));
            this.checkedRecordsToolStripMenuItem.Name = "checkedRecordsToolStripMenuItem";
            this.checkedRecordsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.checkedRecordsToolStripMenuItem.Text = "Checked records";
            this.checkedRecordsToolStripMenuItem.Click += new System.EventHandler(this.checkedRecordsToolStripMenuItem_Click);
            // 
            // allRecordsToolStripMenuItem1
            // 
            this.allRecordsToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("allRecordsToolStripMenuItem1.Image")));
            this.allRecordsToolStripMenuItem1.Name = "allRecordsToolStripMenuItem1";
            this.allRecordsToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.allRecordsToolStripMenuItem1.Text = "All records";
            this.allRecordsToolStripMenuItem1.Click += new System.EventHandler(this.allRecordsToolStripMenuItem1_Click);
            // 
            // DockRecordList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 500);
            this.Controls.Add(this.listViewData);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DockRecordList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewData;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkedReocrdsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkedRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allRecordsToolStripMenuItem1;
    }
}