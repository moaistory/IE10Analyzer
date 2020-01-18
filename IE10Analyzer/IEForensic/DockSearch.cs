using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace IE10Analyzer
{
    public partial class DockSearch : DockContent
    {
        private FormMain formMain;
        public DockSearch(FormMain formMain)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(50,50);
            this.ResumeLayout(true);
            this.formMain = formMain;
        }

        private void DockSearch_DockStateChanged(object sender, EventArgs e)
        {
            this.AutoHidePortion = 45;
            
        }

        private void DockSearch_Resize(object sender, EventArgs e)
        {
            textBoxSearch.Width = panel1.Width;
        }


        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (!formMain.searchListView(textBoxSearch.Text))
            {
                MessageBox.Show("Cannot find : " + textBoxSearch.Text);
            }
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (!formMain.searchPrev())
            {
                MessageBox.Show("This is the first!");
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (!formMain.searchNext())
            {
                MessageBox.Show("This is the last!");
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter))
            {
                if (!formMain.searchListView(textBoxSearch.Text))
                {
                    MessageBox.Show("Cannot find : " + textBoxSearch.Text);
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
