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
    public partial class DockTableList : DockContent
    {
        private FormMain formMain;
        
        public DockTableList(FormMain formMain)
        {
            InitializeComponent();
            this.formMain = formMain;
        }

        public void clearList()
        {
            listViewTable.Items.Clear();
        }

        public void selectTable(String tableName)
        {
            foreach (ListViewItem listViewItem in listViewTable.Items)
            {
                if (listViewItem.SubItems[1].Text.Equals(tableName))
                {
                    listViewItem.Selected = true;
                    break;
                }
            }
            return;
        }
        public void setAutoColumnSize()
        {
            //listViewTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewTable.Update();
        }

        public List<ListViewItem> getTableList()
        {
            List<ListViewItem> listViewItemList = new List<ListViewItem>();
            foreach (ListViewItem listViewItem in listViewTable.Items)
            {
                listViewItemList.Add(listViewItem);
            }
            return listViewItemList;
        }

        public void addItem(ListViewItem item)
        {
            listViewTable.Items.Add(item);
        }

        private void listViewTable_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 방향 초기화
            for (int i = 0; i < listViewTable.Columns.Count; i++)
            {
                listViewTable.Columns[i].Text = listViewTable.Columns[i].Text.Replace(" △", "");
                listViewTable.Columns[i].Text = listViewTable.Columns[i].Text.Replace(" ▽", "");
            }

            // DESC
            if (this.listViewTable.Sorting == SortOrder.Ascending || listViewTable.Sorting == SortOrder.None)
            {
                this.listViewTable.ListViewItemSorter = new ListViewItemComparer(e.Column, "desc");
                listViewTable.Sorting = SortOrder.Descending;
                listViewTable.Columns[e.Column].Text = listViewTable.Columns[e.Column].Text + " ▽";
            }
            // ASC
            else
            {
                this.listViewTable.ListViewItemSorter = new ListViewItemComparer(e.Column, "asc");
                listViewTable.Sorting = SortOrder.Ascending;
                listViewTable.Columns[e.Column].Text = listViewTable.Columns[e.Column].Text + " △";
            }
            listViewTable.Sort();
        }

        private void listViewTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTable.Items.Count == 0)
                return;
            if (listViewTable.SelectedIndices.Count == 0)
                return;
            ListViewItem listViewItem  = listViewTable.SelectedItems[0];
            //ListViewItem listViewItem = listViewTable.Items[listViewTable.FocusedItem.Index];


            formMain.selectTable(listViewItem.SubItems[0].Text, listViewItem);
        }
    }
}
