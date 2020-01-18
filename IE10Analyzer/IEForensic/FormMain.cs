using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using IE10Analyzer.EDBParser;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace IE10Analyzer
{
    public partial class FormMain : Form
    {
        private String fileName = "";
        private String folderPath = "";
        private int pageSize = 0;
        private EsentManager esentManager;
        EDBParserManager edbParserManager;

        private DockTableList dockTableList;
        private DockSearch dockSearch;
        private String UTCTime = "UTC+0";
        private bool useAPI = false;
        private bool recoveryRecord = false;
        private bool parseRecord = false;
        private bool carveRecord = false;
        private BackgroundWorker backgroudWoker;
        private String topTableName;
        private DockRecordList topDockRecordList;
        private FormWait formWait;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";

        public FormMain()
        {
            InitializeComponent();
            esentManager = new EsentManager(this);
            edbParserManager = new EDBParserManager(this);
            DockPanel_Load();
            init();            
        }

        public delegate void setRecordList(List<ListViewItem> listViewItemList);
        public setRecordList setRecordListDelegate;
        public void setRecordListMethod(List<ListViewItem> listViewItemList)
        {
            listViewItemList.Sort(compareRecord);
            topDockRecordList.setData(listViewItemList);
            topDockRecordList.Show(dockPanel, DockState.Document);
        }


        public void init()
        {
            setRecordListDelegate = new setRecordList(setRecordListMethod);
            dockTableList = new DockTableList(this);
            dockTableList.Show(dockPanel, DockState.DockLeft);
            dockSearch = new DockSearch(this);
            
            //dockPanel.DockWindows[DockState.DockTop].Height = 100;
            //dockPanel.ResumeLayout(true, true);
            
        }

        public void openFileUsingAPI()
        {
            List<string> tablesNamesList = esentManager.getTableNames();
            int tableNumber = 0;
            foreach (String tableName in tablesNamesList)
            {
                int count = esentManager.getCount(tableName);
                if (count < 0)
                {
                    continue;
                }
                ListViewItem item = new ListViewItem("" + tableNumber++);
                item.SubItems.Add(tableName);
                item.SubItems.Add("" + count);
                dockTableList.addItem(item);
            }
            dockTableList.setAutoColumnSize();
        }

        public void openFileUsingParsing()
        {
            this.formWait = new FormWait();
            this.formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            this.formWait.setStyle(ProgressBarStyle.Blocks);
            this.formWait.end("");
            this.formWait.setLabel("parsing normal records in the file");
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(parseTableDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(parseTableRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerAsync();
            this.formWait.ShowDialog();
        }

        public void parseTableDoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<int, Table> tableDict = this.edbParserManager.getTableDict();
            this.formWait.maxProgress(tableDict.Keys.Count);
            int number = 0;
            foreach (int tableID in tableDict.Keys)
            {
                if (backgroudWoker.CancellationPending == true)
                {
                    break;
                }
                Table table = tableDict[tableID];
                table.parseRecord();
                this.formWait.setPrgress(++number);
                
                int count = table.getRecordCount();
                if (count == 0)
                {
                    continue;
                }
                ListViewItem item = new ListViewItem("" + tableID);
                item.SubItems.Add(table.getTableName());
                item.SubItems.Add("" + count);
                dockTableList.addItem(item);
            }
        }

        public void parseTableRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            formWait.Close();
            dockTableList.setAutoColumnSize();

        }
        public void openFileUsingRecovery()
        {
            this.formWait = new FormWait();
            this.formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            this.formWait.setStyle(ProgressBarStyle.Blocks);
            this.formWait.end("");
            this.formWait.setLabel("Recovering deleted records in the file");
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(recoverTableDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(recoverTableRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerAsync();
            this.formWait.ShowDialog();
            
        }

        
        public void recoverTableDoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<int, Table> tableDict = this.edbParserManager.getTableDict();
            this.formWait.maxProgress(tableDict.Keys.Count);
            int totalTable = tableDict.Keys.Count;
            int currentTable = 0;
            foreach (int tableID in tableDict.Keys)
            {
                if (backgroudWoker.CancellationPending == true)
                {
                    break;
                }
                Table table = tableDict[tableID];
                table.recoverRecord();
                this.formWait.setPrgress(++currentTable);
                int count = table.getDeletedRecordCount();
                
                
                if (count == 0)
                {
                    continue;
                }
                ListViewItem item = new ListViewItem("" + tableID);
                item.SubItems.Add(table.getTableName());
                item.SubItems.Add("" + count);
                dockTableList.addItem(item);
            }    
        }

        public void openFileUsingCarving()
        {
            this.formWait = new FormWait();
            this.formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            this.formWait.setStyle(ProgressBarStyle.Blocks);
            this.formWait.end("");
            this.formWait.setLabel("Carving records in the file");
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(carveDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(recoverTableRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerAsync();
            this.formWait.ShowDialog();

        }

        
        public void carveDoWork(object sender, DoWorkEventArgs e)
        {
            //1. Page Parse and then Table Setting
            int totalPageCount = edbParserManager.getTootalPageCount();
            this.formWait.maxProgress(totalPageCount);
            for (int curPageNum = 0; curPageNum < totalPageCount-1; curPageNum++)
            {
                if (backgroudWoker.CancellationPending == true)
                {
                    break;
                }
                edbParserManager.carvePage(curPageNum);
                this.formWait.setPrgress(curPageNum+2);
            }
            Dictionary<int, Table> tableDict = this.edbParserManager.getTableDict();
            int totalTable = tableDict.Keys.Count;
            
            foreach (int tableID in tableDict.Keys)
            {
                Table table = tableDict[tableID];
                int count = table.getDeletedRecordCount();
                ListViewItem item = new ListViewItem("" + tableID);
                item.SubItems.Add(table.getTableName());
                item.SubItems.Add("" + count);
                dockTableList.addItem(item);
            }
        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                backgroudWoker.CancelAsync();
            }
        }

        public void recoverTableRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            formWait.Close();
            dockTableList.setAutoColumnSize();
        }

        public void selectTable(string tableNumber, ListViewItem listViewItem)
        {
            if (useAPI)
            {
                this.selectTableUsingAPI(listViewItem);
                return;
            }
            else if(parseRecord)
            {
                this.selectTableParsng(tableNumber);
                return;
            }
            else if(recoveryRecord | carveRecord)
            {
                this.selectTableRecovery(tableNumber);
                return;
            }
        }
        public bool checkDockDocument(String tableName)
        {
            //tableName = tableName.Replace("\0", string.Empty);
            IDockContent[] idockContentArray = this.dockPanel.DocumentsToArray();
            foreach (IDockContent idockContent in idockContentArray)
            {
                DockContentHandler dockContentHandler = idockContent.DockHandler;
                if (dockContentHandler.TabText.Equals(tableName))
                {
                    //MessageBox.Show("[" + tableName + "] has already selected.");
                    dockContentHandler.Activate();
                    return true;
                }
            }
            return false;
        }
        public void selectTableUsingAPI(ListViewItem listViewItem)
        {
            
            topTableName = listViewItem.SubItems[1].Text.ToString();
            if (checkDockDocument(topTableName)) //이미 존재할경우
            {
                return;
            }
            DockRecordList dockRecordList = new DockRecordList(this, useAPI);
            topDockRecordList = dockRecordList;
            dockRecordList.setTitle(topTableName);
            List<ColumnInformation> columnInformationList = esentManager.getColumn(topTableName);
            columnInformationList.Sort(compareColumn);
            dockRecordList.setColumn(columnInformationList);
            esentManager.getData(topTableName, columnInformationList);
        }


        public void selectTableParsng(string tableNumber)
        {
            Table table = edbParserManager.getTable(Int32.Parse(tableNumber));
            topTableName = table.getTableName();
            if (checkDockDocument(topTableName)) //이미 존재할경우
            {
                return;
            }
            DockRecordList dockRecordList = new DockRecordList(this, useAPI);
            topDockRecordList = dockRecordList;
            dockRecordList.setTitle(table.getTableName());
            List<Column> columnList = table.getColumnList();
            dockRecordList.setColumn(columnList);
            edbParserManager.parseRecord(table, true);
        }

        public void selectTableRecovery(string tableNumber)
        {
            Table table = edbParserManager.getTable(Int32.Parse(tableNumber));
            topTableName = table.getTableName();
            if (checkDockDocument(topTableName)) //이미 존재할경우
            {
                return;
            }
            DockRecordList dockRecordList = new DockRecordList(this, useAPI);
            topDockRecordList = dockRecordList;
            dockRecordList.setTitle(table.getTableName());
            List<Column> columnList = table.getColumnList();
            dockRecordList.setColumn(columnList);
            edbParserManager.parseRecord(table, false);
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
            try{x = Int32.Parse(listViewItemX.SubItems[1].Text);}catch{}
            try{y = Int32.Parse(listViewItemY.SubItems[1].Text);}catch{}
            if (x < y)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public void FastCopy(string source, string destination)
        {
            
            int array_length = (int)Math.Pow(2, 19);
            byte[] dataArray = new byte[array_length];
            using (FileStream fsread = new FileStream
            (source, FileMode.Open, FileAccess.Read, FileShare.None, array_length))
            {
                using (BinaryReader bwread = new BinaryReader(fsread))
                {
                    if(File.Exists(destination)){
                        File.Delete(destination);
                    }
                    using (FileStream fswrite = new FileStream
                    (destination, FileMode.Create, FileAccess.Write, FileShare.None, array_length))
                    {
                        using (BinaryWriter bwwrite = new BinaryWriter(fswrite))
                        {
                            for (; ; )
                            {
                                int read = bwread.Read(dataArray, 0, array_length);
                                if (0 == read)
                                    break;
                                bwwrite.Write(dataArray, 0, read);
                            }
                        }
                    }
                }
            }
            //File.Delete(source);
        }

        public void openAPI(String fileName, String workingDirectoryPath)
        {
            useAPI = true;
            recoveryRecord = false;
            parseRecord = false;
            carveRecord = false;
            edbParserManager.colse();
            HexReader hexReader;
            try
            {
                hexReader = new HexReader(fileName);
            }
            catch
            {
                MessageBox.Show("This file is currently in use", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            long signature = hexReader.readLong(4);
            if (signature != 6736818458095L)
            {
                MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int status = hexReader.readInt(0x34);
            if (status == 2)
            {
                MessageBox.Show("Error : Use Open(parsing)", "This file is Dirty status", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            pageSize = hexReader.readInt(236);
            hexReader.close();

            FormInputUTCTime formInputUTCTime = new FormInputUTCTime();
            DialogResult dialogResut = formInputUTCTime.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                UTCTime = formInputUTCTime.getUTCTime();
            }

            formInputUTCTime.Dispose();

            

            try
            {
                FastCopy(fileName, workingDirectoryPath);
            }
            catch
            {
                MessageBox.Show("This file is currently in use", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            fileName = workingDirectoryPath;
            try
            {
                if (!esentManager.Initialize(fileName, pageSize))
                {
                    MessageBox.Show("This file is not WebCacheV01.dat(Internet Explorer DB)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                String[] Time = UTCTime.Replace("UTC-", "").Replace("UTC+", "").Split(':');
                this.UTCAddHour = "0";
                this.UTCAddMinute = "0";
                if (Time.Length == 1)
                {
                    UTCAddHour = Time[0];
                }
                else if (Time.Length == 2)
                {
                    UTCAddHour = Time[0];
                    UTCAddMinute = Time[1];
                }
                esentManager.setUTCTime(UTCAddHour, UTCAddMinute);
                dockTableList.clearList();
                openFileUsingAPI();

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void fileOpenCheck(String fileName)
        {
            HexReader hexReader = new HexReader(fileName);
            long signature = hexReader.readLong(4);
            if (signature != 6736818458095L)
            {
                MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            hexReader.close();

            FormInputUTCTime formInputUTCTime = new FormInputUTCTime();
            DialogResult dialogResut = formInputUTCTime.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                UTCTime = formInputUTCTime.getUTCTime();
                String[] Time = UTCTime.Replace("UTC-", "").Replace("UTC+", "").Split(':');

                if (Time.Length == 1)
                {
                    UTCAddHour = Time[0];
                }
                else if (Time.Length == 2)
                {
                    UTCAddHour = Time[0];
                    UTCAddMinute = Time[1];
                }

            }
        }

        public void openParsing(String fileName)
        {
            useAPI = false;
            parseRecord = true;
            recoveryRecord = false;
            carveRecord = false;
            edbParserManager.colse();
            fileOpenCheck(fileName);
            edbParserManager.init(fileName);
            edbParserManager.setUTCTime(UTCAddHour, UTCAddMinute);
            edbParserManager.parseDatabaseHeader();
            edbParserManager.makeTable();
            Dictionary<int, Table> tableDict = edbParserManager.getTableDict();
            dockTableList.clearList();
            openFileUsingParsing();


        }

        public void openRecovery(String fileName)
        {
            useAPI = false;
            parseRecord = false;
            recoveryRecord = true;
            carveRecord = false;
            edbParserManager.colse();
            fileOpenCheck(fileName);
            edbParserManager.init(fileName);
            edbParserManager.setUTCTime(UTCAddHour, UTCAddMinute);
            edbParserManager.parseDatabaseHeader();
            edbParserManager.makeTable();
            edbParserManager.findTablePage();

            //Dictionary<int, Table> tableDict = edbParserManager.getTableDict();
            dockTableList.clearList();

            openFileUsingRecovery();
        }

        public void openCaving(String fileName)
        {
            useAPI = false;
            parseRecord = false;
            recoveryRecord = false;
            carveRecord = true;
            edbParserManager.colse();
            fileOpenCheck(fileName);
            edbParserManager.init(fileName);
            edbParserManager.setUTCTime(UTCAddHour, UTCAddMinute);
            edbParserManager.parseDatabaseHeader();
            edbParserManager.setCarveTable();
            dockTableList.clearList();
            openFileUsingCarving();
        }

        #region DockPanel Load
        private void DockPanel_Load()
        {
            // DockPanel 초기화
            dockPanel.AllowEndUserDocking = true;
            dockPanel.DocumentStyle = DocumentStyle.DockingMdi;
            dockPanel.Parent = this;
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.BorderStyle = BorderStyle.None;
            MainForm_DockPanelColorSetting();                   // DockPanel 색상 정의
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }
        #endregion

        #region DockPanel UI 초기화 함수
        private void MainForm_DockPanelColorSetting()
        {
            DockPaneStripSkin dockPaneSkin = new DockPaneStripSkin();

            dockPaneSkin.DocumentGradient.DockStripGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.DockStripGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.ActiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.DocumentGradient.InactiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.InactiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.DocumentGradient.InactiveTabGradient.TextColor = System.Drawing.Color.White;

            dockPaneSkin.ToolWindowGradient.DockStripGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.DockStripGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveTabGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = System.Drawing.Color.White;
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = System.Drawing.Color.FromArgb(80, 80, 80);
            dockPaneSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = System.Drawing.Color.White;
            dockPanel.Skin.DockPaneStripSkin = dockPaneSkin;
        }
        #endregion



       

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (useAPI == true)
            {
                try
                {
                    if (esentManager.isOpen())
                    {
                        esentManager.close();
                    }
                    // Delete the temp file (if it exists)
                    if (Directory.Exists(folderPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(folderPath);
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        Directory.Delete(folderPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    


        private void sqliteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSave formSave = new FormSave("SQLite", dockTableList.getTableList(), useAPI, parseRecord, recoveryRecord);

            DialogResult dialogResut = formSave.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                MessageBox.Show("Finish saving the file");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cSVToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSave formSave = new FormSave("CSV", dockTableList.getTableList(), useAPI, parseRecord, recoveryRecord);
            formSave.FormSaveSetData(esentManager, edbParserManager);
            DialogResult dialogResut = formSave.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                MessageBox.Show("Finish saving the file");
            }
        } 


        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            dockTableList.setAutoColumnSize();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!dockSearch.IsActivated)
            {   
                dockSearch.Show(dockPanel, DockState.DockTop);
                dockPanel.DockTopPortion = 45;
            }
        }

        public bool searchListView(String searchData)
        {
            try
            {
                return topDockRecordList.searchListView(searchData);
            }
            catch
            {
                return false;
            }
        }


        public bool searchPrev()
        {
            return topDockRecordList.searchPrev();
        }

        public bool searchNext()
        {
            return topDockRecordList.searchNext();
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            try
            {
                topDockRecordList = (DockRecordList)dockPanel.ActiveDocument;
                dockTableList.selectTable(topDockRecordList.Text);
            }
            catch
            {
                //nullReferenceException
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSave formSave = new FormSave("CSV", dockTableList.getTableList(), useAPI, parseRecord, recoveryRecord);
            formSave.FormSaveSetData(esentManager, edbParserManager);
            DialogResult dialogResut = formSave.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                MessageBox.Show("Finish saving the file");
            }
        }

        private void sqliteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FormSave formSave = new FormSave("SQL", dockTableList.getTableList(), useAPI, parseRecord, recoveryRecord);
            formSave.FormSaveSetData(esentManager, edbParserManager);
            DialogResult dialogResut = formSave.ShowDialog();
            if (dialogResut == DialogResult.OK)
            {
                MessageBox.Show("Finish saving the file");
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            return;
        }



        public String getMD5(String str)
        {
            StringBuilder MD5Str = new StringBuilder();
            byte[] byteArr = Encoding.ASCII.GetBytes(str);
            byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(byteArr);
            for (int cnti = 0; cnti < resultArr.Length; cnti++)
            {
                MD5Str.Append(resultArr[cnti].ToString("X2"));
            }
            return MD5Str.ToString();
        }

        public String getMacAddress()
        {
            NetworkInterface[] networkInterfaceArray = NetworkInterface.GetAllNetworkInterfaces();
            if (networkInterfaceArray.Length != 0)
            {
                return networkInterfaceArray[0].GetPhysicalAddress().ToString();
            }
            else
            {
                return SystemInformation.ComputerName + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOpen formOpen = new FormOpen(this);
            DialogResult dialogResut = formOpen.ShowDialog();
        }

        public bool isUseAPI()
        {
            return useAPI;
        }

        public bool isParsing()
        {
            return parseRecord;
        }

        public bool isRecovery()
        {
            return recoveryRecord;
        }

        public bool isCarve()
        {
            return carveRecord;
        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://moaistory.blogspot.com/2016/07/internet-explorer-10-microsoft-edge.html");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                if (!dockSearch.IsActivated)
                {
                    dockSearch.Show(dockPanel, DockState.DockTop);
                    dockPanel.DockTopPortion = 45;
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            FormOpen formOpen = new FormOpen(this);
            formOpen.radioButtonOpen_Check();
            if(formOpen.fileOpen(FileList[0]))
            {
                DialogResult dialogResut = formOpen.ShowDialog();
            }
            
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
    }
}


