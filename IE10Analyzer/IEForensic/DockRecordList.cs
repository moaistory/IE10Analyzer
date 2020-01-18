using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using IE10Analyzer.EDBParser;
using System.IO;
using Microsoft.Isam.Esent.Interop;

namespace IE10Analyzer
{
    public partial class DockRecordList : DockContent
    {
        FormMain formMain;
        List<ColumnInformation> columnInformationList;
        private List<Column> columnList;
        private bool useAPI;
        private List<ListViewItem> searchItemList;
        private int currentSearchItemNumber =-1;
        private List<ListViewItem> listViewItemList;
        private List<ListViewItem> checkedItemList;
        private Dictionary<int, int> clickToSearchDict;
        private List<String> columnNameList;
        public DockRecordList(FormMain formMain, bool useAPI)
        {
            InitializeComponent();
            this.formMain = formMain;
            this.useAPI = useAPI;
            searchItemList = new List<ListViewItem>();
            checkedItemList = new List<ListViewItem>();
            columnNameList = new List<string>();
            clickToSearchDict = new Dictionary<int, int>();
            currentSearchItemNumber = -1;
        }

        public void clearColumn()
        {
            listViewData.Columns.Clear();
        }

        public void clearData()
        {
            listViewData.Items.Clear();
        }
        public void setTitle(String title)
        {
            this.Text = title;
        }

        public void setAutoColumnSize()
        {
            listViewData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (this.Text.Contains("History") || this.Text.Contains("MSHist"))
            {
                int columnCount = listViewData.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    String columnName = listViewData.Columns[i].Text;
                    if (i == 0)
                    {
                        continue;
                    }
                    else if (columnName.Equals("EntryId"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("Type"))
                    {
                        listViewData.Columns[i].Width = 200;
                        continue;
                    }
                    else if (columnName.Equals("AccessCount"))
                    {
                        continue;
                    }
                    else if (columnName.Contains("Time"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("Url"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else if (columnName.Equals("WebPageInfo"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else
                    {
                        listViewData.Columns[i].Width = 0;
                    }
                }
            }

            else if (this.Text.Contains("Content"))
            {
                int columnCount = listViewData.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    String columnName = listViewData.Columns[i].Text;
                    if (i == 0)
                    {
                        continue;
                    }
                    else if (columnName.Equals("EntryId"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("FileSize"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("Type"))
                    {
                        listViewData.Columns[i].Width = 200;
                        continue;
                    }
                    else if (columnName.Equals("AccessCount"))
                    {
                        continue;
                    }
                    else if (columnName.Contains("Time"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("Url"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else if (columnName.Equals("Filename"))
                    {
                        listViewData.Columns[i].Width = 150;
                        continue;
                    }
                    else if (columnName.Equals("HTTPHeader"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else
                    {
                        listViewData.Columns[i].Width = 0;
                    }
                }
            }

            else if (this.Text.Contains("iedownload"))
            {
                int columnCount = listViewData.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    String columnName = listViewData.Columns[i].Text;
                    if (i == 0)
                    {
                        continue;
                    }
                    else if (columnName.Equals("EntryId"))
                    {
                        continue;
                    }

                    else if (columnName.Equals("Type"))
                    {
                        listViewData.Columns[i].Width = 200;
                        continue;
                    }
                    else if (columnName.Equals("AccessCount"))
                    {
                        continue;
                    }
                    else if (columnName.Contains("Time"))
                    {
                        continue;
                    }
                    else if (columnName.Equals("Url"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else if (columnName.Equals("DownloadPath"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                    else
                    {
                        listViewData.Columns[i].Width = 0;
                    }
                }
            }
            else
            {
                int columnCount = listViewData.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    String columnName = listViewData.Columns[i].Text;
                    
                    if (columnName.Equals("ContainerId"))
                    {
                        listViewData.Columns[i].Width = 0;
                        continue;
                    }
                    else if (columnName.Equals("CacheId"))
                    {
                        listViewData.Columns[i].Width = 0;
                        continue;
                    }
                    else if (columnName.Equals("UrlHash"))
                    {
                        listViewData.Columns[i].Width = 0;
                        continue;
                    }
                    else if (columnName.Equals("SecureDirectory"))
                    {
                        listViewData.Columns[i].Width = 0;
                        continue;
                    }
                    else if (columnName.Equals("Flags"))
                    {
                        listViewData.Columns[i].Width = 0;
                        continue;
                    }
                    else if (columnName.Equals("Type"))
                    {
                        listViewData.Columns[i].Width = 200;
                        continue;
                    }
                   
                    else if (columnName.Equals("Url"))
                    {
                        listViewData.Columns[i].Width = 300;
                        continue;
                    }
                }
            }
        }

        public void setColumn(List<ColumnInformation> columnInformationList)
        {
            columnNameList.Clear();
            this.columnInformationList = columnInformationList;
            //columnInformationList.Sort(compareColumn);
            listViewData.Columns.Add("", 0, HorizontalAlignment.Left);
            int index =0;
            foreach (ColumnInformation columnInformation in columnInformationList)
            {
                listViewData.Columns.Add(columnInformation.getName(), -2, HorizontalAlignment.Left);
                columnNameList.Add(columnInformation.getName());
                index++;
            }
            this.setAutoColumnSize();
            
        }
        public void setColumn(List<Column> columnList)
        {
            columnNameList.Clear();
            this.columnList = columnList;
            listViewData.Columns.Add("", 0, HorizontalAlignment.Left);
            foreach (Column column in columnList)
            {
                listViewData.Columns.Add(column.getName(), -2, HorizontalAlignment.Left);
                columnNameList.Add(column.getName());
            }
            this.setAutoColumnSize();
        }


        public void setData(List<ListViewItem> listViewItemList)
        {
            this.listViewItemList = listViewItemList;
            foreach (ListViewItem listViewItem in listViewItemList)
            {
                listViewData.Items.Add(listViewItem);
            }
            
        }
        

        private void listViewData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FormDetailView formDetailView = new FormDetailView();
            if (useAPI)
            {
                formDetailView.setColumnList(columnInformationList);
                
            }
            else
            {
                formDetailView.setColumnList(columnList);
            }
            ListViewItem listViewItem = listViewData.SelectedItems[0];
            formDetailView.setListViewItem(listViewItem);
            formDetailView.Show();
        }

        public bool searchListView(string searchData)
        {
            bool result = false;
            searchItemList.Clear();
            clickToSearchDict.Clear();
            int searchCnt = 0;
            for (int i = 0; i < listViewData.Items.Count; i++)
            {
                listViewData.Items[i].ForeColor = Color.Black;
                listViewData.Items[i].BackColor = Color.White;
                clickToSearchDict.Add(i, searchCnt);
                foreach (ListViewItem.ListViewSubItem subItem in listViewData.Items[i].SubItems)
                {
                    if (subItem.Text.ToUpper().IndexOf(searchData.ToUpper()) >= 0)
                    {
                        searchCnt++;
                        listViewData.Items[i].ForeColor = Color.White;
                        listViewData.Items[i].BackColor = Color.DarkBlue;
                        searchItemList.Add(listViewData.Items[i]);
                        result= true;
                        break;
                    }
                }
            }
            if (result)
            {
                this.listViewData.TopItem = searchItemList[0];
                listViewData.FocusedItem = searchItemList[0];
                currentSearchItemNumber = 0;
            }
            else
            {
                currentSearchItemNumber = -1;
            }
            
            return result;
        }

        public bool searchPrev()
        {
            if (currentSearchItemNumber == -1)
            {
                return false;

            }else if (currentSearchItemNumber == 0){
                this.listViewData.TopItem = searchItemList[currentSearchItemNumber];
                return false;
            }
            else
            {
                currentSearchItemNumber--;
                this.listViewData.TopItem = searchItemList[currentSearchItemNumber];
                return true;
            }
        }

        public bool searchNext()
        {
            if (currentSearchItemNumber == -1)
            {
                return false;

            }
            if (listViewData.FocusedItem != null && searchItemList.Count > clickToSearchDict[listViewData.FocusedItem.Index])
            {
                if (searchItemList[clickToSearchDict[listViewData.FocusedItem.Index]].Index != listViewData.FocusedItem.Index)
                {
                    currentSearchItemNumber = clickToSearchDict[listViewData.FocusedItem.Index] - 1;   
                }
                
            }

            
            if (currentSearchItemNumber == searchItemList.Count - 1)
            {
                this.listViewData.TopItem = searchItemList[currentSearchItemNumber];
                listViewData.FocusedItem = searchItemList[currentSearchItemNumber];
                return false;
            }
            else
            {
                currentSearchItemNumber++;
                this.listViewData.TopItem = searchItemList[currentSearchItemNumber];
                listViewData.FocusedItem = searchItemList[currentSearchItemNumber];
                return true;
            }
        }

        private void checkedReocrdsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewData.BeginUpdate();
            listViewData.Items.Clear();
            checkedItemList.Clear();
            
            foreach (ListViewItem listViewItem in listViewItemList)
            {
                if (listViewItem.Checked)
                {
                    listViewData.Items.Add(listViewItem);
                    checkedItemList.Add(listViewItem);
                }
            }
            
            listViewData.EndUpdate();

        }
        private void allRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewData.BeginUpdate();
            listViewData.Items.Clear();
            foreach (ListViewItem listViewItem in listViewItemList)
            {
                listViewData.Items.Add(listViewItem);
            }

            foreach (ListViewItem listViewItem in checkedItemList)
            {
                listViewItem.Checked = true;
            }
            listViewData.EndUpdate();

        }
        private void listViewData_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo HI = listViewData.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (listViewData.FocusedItem !=null)
                {
                    if (listViewData.FocusedItem.Bounds.Contains(e.Location) == true)
                    {
                        checkToolStripMenuItem.Enabled = true;
                        if (listViewData.SelectedItems[0].Checked == true)
                        {
                            checkToolStripMenuItem.Text = "UnCheck";
                        }
                        else
                        {
                            checkToolStripMenuItem.Text = "Check";
                        }
                        contextMenuStrip.Show(Cursor.Position);

                    }
                }
            } 
        }

        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewData.SelectedItems[0].Checked == true)
            {
                listViewData.SelectedItems[0].Checked = false;
            }
            else
            {
                listViewData.SelectedItems[0].Checked = true;
            }
        }

        private void listViewData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewData.FocusedItem != null)
                {
                    if (listViewData.FocusedItem.Bounds.Contains(e.Location) == false)
                    {
                        checkToolStripMenuItem.Enabled = false;
                        contextMenuStrip.Show(Cursor.Position);
                    }
                }
                else
                {
                    checkToolStripMenuItem.Enabled = false;
                    contextMenuStrip.Show(Cursor.Position);
                }
                
            }
        }

        private bool writeCSV(String savePath, List<String> columnList, List<ListViewItem> listViewItemList)
        {
            CsvFileWriter csvFileWriter;
            FileStream m_fileStream;
            try
            {
                m_fileStream = new FileStream(savePath, FileMode.Append, FileAccess.Write);
                
                csvFileWriter = new CsvFileWriter(m_fileStream, System.Text.Encoding.UTF8);
                csvFileWriter.WriteRow(columnList);

                List<String> dataList = new List<string>();
                foreach (ListViewItem listViewItem in listViewItemList)
                {
                    dataList.Clear();
                    foreach (ListViewItem.ListViewSubItem data in listViewItem.SubItems)
                    {
                        if (data == listViewItem.SubItems[0])
                        {
                            continue;
                        }
                        dataList.Add(data.Text);
                    }
                    csvFileWriter.WriteRow(dataList);
                }
                csvFileWriter.Close();
                m_fileStream.Close();
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("" + exception);
                return false;
            }
        }

        private void checkedRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.CheckFileExists = false;
            string dummyFileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            dummyFileName += "_" + this.Text + ".csv";
            sf.Filter = "csv(*.csv)|*.csv";
            sf.FileName = dummyFileName;
            if (sf.ShowDialog() == DialogResult.OK)
            {
                String savePath = sf.FileName;
                List<ListViewItem> checkedItemList = new List<ListViewItem>();
                foreach (ListViewItem listViewItem in listViewItemList)
                {
                    if (listViewItem.Checked)
                    {
                        checkedItemList.Add(listViewItem);
                    }
                }

                if(writeCSV(savePath, columnNameList, checkedItemList)){
                    MessageBox.Show("Comeplete!");
                }
                
            }
        }

        private void allRecordsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.CheckFileExists = false;
            string dummyFileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            dummyFileName += "_" + this.Text + ".csv";
            sf.Filter = "csv(*.csv)|*.csv";
            sf.FileName = dummyFileName;
            if (sf.ShowDialog() == DialogResult.OK)
            {
                String savePath = sf.FileName;
                writeCSV(savePath, columnNameList, listViewItemList);
            }
        }

        private void listViewData_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 방향 초기화
            for (int i = 0; i < listViewData.Columns.Count; i++)
            {
                listViewData.Columns[i].Text = listViewData.Columns[i].Text.Replace(" △", "");
                listViewData.Columns[i].Text = listViewData.Columns[i].Text.Replace(" ▽", "");
            }

            // DESC
            if (this.listViewData.Sorting == SortOrder.Ascending || listViewData.Sorting == SortOrder.None)
            {
                this.listViewData.ListViewItemSorter = new ListViewItemComparer(e.Column, "desc");
                listViewData.Sorting = SortOrder.Descending;
                listViewData.Columns[e.Column].Text = listViewData.Columns[e.Column].Text + " ▽";
            }
            // ASC
            else
            {
                this.listViewData.ListViewItemSorter = new ListViewItemComparer(e.Column, "asc");
                listViewData.Sorting = SortOrder.Ascending;
                listViewData.Columns[e.Column].Text = listViewData.Columns[e.Column].Text + " △";
            }
            listViewData.Sort();
        }




    }
}
