using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IE10Analyzer.EDBParser;

namespace IE10Analyzer
{
    public partial class FormDetailView : Form
    {
        private ListViewItem listViewItem;
        
        public FormDetailView()
        {
            InitializeComponent();
        }
        
        public void setListViewItem(ListViewItem listViewItem)
        {
            this.listViewItem = listViewItem;
            listViewColumn.Items[0].Selected = true;
            listViewColumn.Select();
        }


        
        public void setAutoColumnSize()
        {
            listViewColumn.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewColumn.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }



        public void setColumnList(List<ColumnInformation> columnInformationList)
        {
            columnInformationList.Sort(compareColumn);
            foreach (ColumnInformation columnInformation in columnInformationList)
            {
                listViewColumn.Items.Add(columnInformation.getName());
            }
            setAutoColumnSize();
        }

        public void setColumnList(List<Column> columnList)
        {
            foreach (Column column in columnList)
            {
                listViewColumn.Items.Add(column.getName());
            }
            setAutoColumnSize();
        }

        public int compareColumn(ColumnInformation x, ColumnInformation y)
        {
            if (x.getId() < y.getId())
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private void listViewColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewColumn.Items.Count == 0)
                return;
            if (listViewColumn.SelectedIndices.Count == 0)
                return;

            textBoxDetail.Text = this.listViewItem.SubItems[listViewColumn.SelectedIndices[0]+1].Text.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
