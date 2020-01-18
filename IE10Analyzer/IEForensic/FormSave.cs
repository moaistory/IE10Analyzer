using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using IE10Analyzer.EDBParser;
using System.Reflection;

namespace IE10Analyzer
{
    public partial class FormSave : Form
    {
        private String saveForm;
        private String savePath;
        private String saveTime;
        private List<ListViewItem> tableList;
        private EsentManager esentManager;
        private EDBParserManager edbParserManager;
        private bool useAPI = false;
        private bool recoveryRecord = false;
        private bool parseRecord = false;
        public FormSave(String saveForm, List<ListViewItem>tableList, bool useAPI, bool parseRecord, bool recoveryRecord)
        {
            InitializeComponent();
            this.useAPI = useAPI;
            this.recoveryRecord = recoveryRecord;
            this.parseRecord = parseRecord;
            this.saveForm = saveForm;
            this.tableList = tableList;
            listViewTable.BeginUpdate();
            foreach (ListViewItem listViewItem in tableList)
            {
                ListViewItem inputIistViewItem = (ListViewItem)listViewItem.Clone();
                this.listViewTable.Items.Add(inputIistViewItem);
                
            }
            listViewTable.EndUpdate();
            this.esentManager = null;
            this.edbParserManager = null;
            
        }

        public void FormSaveSetData(EsentManager esentManager, EDBParserManager edbParserManager)
        {
            this.esentManager = esentManager;
            this.edbParserManager = edbParserManager;
        }

        public delegate void saveRecordList(List<String> columnList, List<ListViewItem> listViewItemList);
        public saveRecordList saveRecordListDelegate;
        public bool saveRecordListMethod(String tableName, List<String> columnList, List<ListViewItem> listViewItemList)
        {
            if (listViewItemList.Count == 0)
            {
                return false;
            }
            listViewItemList.Sort(compareRecord);
            if (saveForm.Equals("CSV"))
            {
                writeCSV(tableName, columnList, listViewItemList);
            }
            else if(saveForm.Equals("SQL"))
            {
                if (!File.Exists("System.Data.SQLite.dll"))
                {
                    using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("IEForensic.Resources.System.Data.SQLite.dll"))
                    using (Stream output = File.Create("System.Data.SQLite.dll"))
                    {
                        byte[] buffer = new byte[8192];

                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                writeSQL(tableName, columnList, listViewItemList);
            }
            
            return true;
        }

        public void closeForm(){
            this.Close();
        }

        private void saveUsingAPI(){
            if (listViewTable.CheckedItems.Count != 0)
            {
                for (int i = 0; i < listViewTable.CheckedItems.Count; i++)
                {
                    String tableName = listViewTable.CheckedItems[i].SubItems[1].Text.ToString();
                    List<ColumnInformation> columnInformationList = esentManager.getColumn(tableName);
                    columnInformationList.Sort(compareColumn);
                    esentManager.saveData(this, tableName, columnInformationList);
                }
            }
            MessageBox.Show("The operation has been completed!");
        }

        private void saveParsingRecord(){
            if (listViewTable.CheckedItems.Count != 0)
            {
                for (int i = 0; i < listViewTable.CheckedItems.Count; i++)
                {
                    String tableNumber = listViewTable.CheckedItems[i].SubItems[0].Text;
                    String tableName = listViewTable.CheckedItems[i].SubItems[1].Text;
                    Table table = edbParserManager.getTable(Int32.Parse(tableNumber));
                    edbParserManager.saveData(this, table, true); //normal
                }
            }
            MessageBox.Show("The operation has been completed!");
        }

        private void saveRecoveryRecord()
        {
            if (listViewTable.CheckedItems.Count != 0)
            {
                for (int i = 0; i < listViewTable.CheckedItems.Count; i++)
                {
                    String tableNumber = listViewTable.CheckedItems[i].SubItems[0].Text;
                    String tableName = listViewTable.CheckedItems[i].SubItems[1].Text;
                    Table table = edbParserManager.getTable(Int32.Parse(tableNumber));
                    edbParserManager.saveData(this, table, false); //recovery
                }
            }
            MessageBox.Show("The operation has been completed!");
        }

        private bool writeCSV(String tableName, List<String> columnList, List<ListViewItem> listViewItemList)
        {

            CsvFileWriter csvFileWriter;
            FileStream m_fileStream;
            try
            {
                m_fileStream = new FileStream(this.savePath +"\\"+ this.saveTime + "_" + tableName+".csv", FileMode.Append, FileAccess.Write);
                
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

        private bool writeSQL(String tableName, List<String> columnList, List<ListViewItem> listViewItemList)
        {
            
            SqlManager sqlManager = new SqlManager(this.savePath);
            
            if (sqlManager.open())
            {
                if (!sqlManager.insertTable(tableName, columnList, listViewItemList))
                {
                    return false;
                }
                sqlManager.close();
                return true;
            }
            return false;
            
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


        public void setAutoColumnSize()
        {
            //listViewTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewTable.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewTable.Update();
        }

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            listViewTable.BeginUpdate();
            for (int i = 0; i < listViewTable.Items.Count; i++)
            {
                listViewTable.Items[i].Checked = true;
            }
            listViewTable.EndUpdate();
        }

        private void buttonUnCheck_Click(object sender, EventArgs e)
        {
            listViewTable.BeginUpdate();
            for (int i = 0; i < listViewTable.Items.Count; i++)
            {
                listViewTable.Items[i].Checked = false;
            }
            listViewTable.EndUpdate();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.CheckFileExists = false;
            this.saveTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string dummyFileName = this.saveTime;
            if(saveForm.Equals("SQL")){
                dummyFileName += ".db";
                sf.Filter = "db (*.db)|*.db";
            }else{
                 dummyFileName += "_tableName.csv";
                 sf.Filter = "csv(*.csv)|*.csv";
            }
            sf.FileName = dummyFileName;
            
            if (sf.ShowDialog() == DialogResult.OK)
            {
                if(saveForm.Equals("CSV")){
                    this.savePath = Path.GetDirectoryName(sf.FileName);
                }
                else if(saveForm.Equals("SQL")){
                    this.savePath = sf.FileName;
                }

                if (useAPI) { 
                    saveUsingAPI(); 
                }else if (parseRecord) { 
                    saveParsingRecord(); 
                }
                else if (recoveryRecord) {
                    saveRecoveryRecord();
                }
                
            }
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

        public int compareRecord(ListViewItem listViewItemX, ListViewItem listViewItemY)
        {
            int x = 0;
            int y = 0;
            try { x = Int32.Parse(listViewItemX.SubItems[1].Text); }
            catch { }
            try { y = Int32.Parse(listViewItemY.SubItems[1].Text); }
            catch { }
            if (x < y)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private void buttonCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
