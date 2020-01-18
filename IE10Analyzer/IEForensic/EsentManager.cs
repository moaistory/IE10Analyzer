using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Web;
using System.Globalization;
namespace IE10Analyzer
{
    public class EsentManager
    {
        private Instance _instance;
        private String _databaseFile = "";
        private Session _primarySessionId;
        private JET_DBID _primaryDatabaseId;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private int DatabasePageSize = 32768;
        private int VersionStorePageSize = 32768 * 2;
        private const int JET_paramLogFileCreateAsynch = 69;
        private const int JET_paramCommitDefault = 16;
        private BackgroundWorker backgroudWoker;
        private FormMain formMain;
        private FormWait formWait;
        private FormSave formSave;
        private bool isSaveData = false;
        private bool isReadData = false;
        private Dictionary<String, String> tableNameDict;
        /*
        // set db page size to 8K and version store page size to 16K
        private const int DatabasePageSize = 2 * 4 * 1024;
        private const int VersionStorePageSize = 4 * 4 * 1024;

        // JET parameter consts
        private const int JET_paramIOPriority = 152;
        private const int JET_paramCheckpointIOMax = 135;
        private const int JET_paramVerPageSize = 128;
        private const int JET_paramDisablePerfmon = 107;
        private const int JET_paramPageHintCacheSize = 101;
        private const int JET_paramLogFileCreateAsynch = 69;
        private const int JET_paramOutstandingIOMax = 30;
        private const int JET_paramLRUKHistoryMax = 26;
        private const int JET_paramCommitDefault = 16;
        */

        public EsentManager(FormMain formMain){
            this.formMain = formMain;
            
        }

        public bool isOpen(){
            if (_instance != null)
                return true;
            else
                return false;
        }
        public bool Initialize(String filePath, int pageSize)
        {
            this._databaseFile = filePath;
            DatabasePageSize = pageSize;
            VersionStorePageSize = pageSize * 2;
            _instance = CreateEsentInstance();

            _primarySessionId = new Session(_instance);
            InitializeDatabaseAndTables();
            JET_TABLEID tableid;
            
            bool result = Api.TryOpenTable(_primarySessionId, _primaryDatabaseId, "Containers", OpenTableGrbit.None, out tableid);

            if (result == false)
            {
                return false;
            }
            this.tableNameDict = new Dictionary<string, string>();
            
            using (var trx = new Transaction(_primarySessionId))
            {
                if (Api.TryMoveFirst(_primarySessionId, tableid))
                {
                    do
                    {
                        
                        JET_COLUMNBASE colBaseName;
                        Api.JetGetColumnInfo(_primarySessionId, _primaryDatabaseId, "Containers", "Name", out colBaseName);
                        ColumnStream streamName = new ColumnStream(_primarySessionId, tableid, colBaseName.columnid);
                        Byte[] dataName = new Byte[streamName.Length];
                        streamName.Read(dataName, 0, (int)streamName.Length);
                        String tableConvertName = Encoding.Unicode.GetString(dataName).Replace("\0", string.Empty);

                        JET_COLUMNBASE colBasePartition;
                        Api.JetGetColumnInfo(_primarySessionId, _primaryDatabaseId, "Containers", "PartitionId", out colBasePartition);
                        ColumnStream streamPartition = new ColumnStream(_primarySessionId, tableid, colBasePartition.columnid);
                        Byte[] dataPartition = new Byte[streamPartition.Length];
                        streamPartition.Read(dataPartition, 0, (int)streamPartition.Length);
                        String tableConvertPartition = Encoding.Unicode.GetString(dataPartition).Replace("\0", string.Empty);
                        tableConvertName += "(" + tableConvertPartition + ")";

                        JET_COLUMNBASE colBaseID;
                        Api.JetGetColumnInfo(_primarySessionId, _primaryDatabaseId, "Containers", "ContainerId", out colBaseID);
                        ColumnStream streamID = new ColumnStream(_primarySessionId, tableid, colBaseID.columnid);
                        Byte[] dataID = new Byte[streamID.Length];
                        streamID.Read(dataID, 0, (int)streamID.Length);
                        String tableConvertID=BitConverter.ToUInt64(dataID, 0).ToString();

                        tableNameDict[tableConvertName] = tableConvertID;

                        
                        
                    }
                    while (Api.TryMoveNext(_primarySessionId, tableid));
                }
                Api.JetCloseTable(_primarySessionId, tableid);
            }
           
            
            return true;
        }

        public void readRecordsRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            
            Tuple<String, List<String>, List<ListViewItem>> result = (Tuple<String, List<String>, List<ListViewItem>>)e.Result;
            String tableName = result.Item1;
            List<ListViewItem> listViewItemList = result.Item3;
            formMain.setRecordListMethod(listViewItemList);
            formWait.Close();
            
        }

        public void saveRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            
            Tuple<String, List<String>, List<ListViewItem>> result = (Tuple<String, List<String>, List<ListViewItem>>)e.Result;
            String tableName = result.Item1;            
            List<String> columnDataList = result.Item2;
            List<ListViewItem> listViewItemList = (List<ListViewItem>)result.Item3;
            
            formSave.saveRecordListMethod(tableName, columnDataList, listViewItemList);
            formWait.Close();
            formSave.closeForm();
        }


        public void readRecordsDoWork(object sender, DoWorkEventArgs e)
        {

            Tuple<String, List<ColumnInformation>> tableData = (Tuple<String, List<ColumnInformation>>)e.Argument;
            String tableName = tableData.Item1;
            List<ColumnInformation> columnInformationList = tableData.Item2;
            List<String> coulumnDataList = new List<String>();
            JET_TABLEID tableid;
            List<ListViewItem> listViewItemList = new List<ListViewItem>();
            using (var trx = new Transaction(_primarySessionId))
            {
                String originalTableName = "Container_" + tableNameDict[tableName];
                Api.JetOpenTable(_primarySessionId, _primaryDatabaseId, originalTableName, null, 0, OpenTableGrbit.None, out tableid);
                int count;
                int index =0;
                Api.JetIndexRecordCount(_primarySessionId, tableid, out count, Int32.MaxValue);
                this.formWait.setPrgress(0);
                this.formWait.maxProgress(count);
                this.formWait.end("- load table : " + tableName);
                if (Api.TryMoveFirst(_primarySessionId, tableid))
                {
                    do
                    {
                        if (backgroudWoker.CancellationPending == true)
                        {
                            break;
                        }
                        ListViewItem listViewItem = new ListViewItem();
                        foreach (ColumnInformation columnInformation in columnInformationList)
                        {
                            if (columnInformation.getId() == 256 && columnInformation.getName().Equals("Url"))
                            {
                                JET_COLUMNBASE colBase;
                                Api.JetGetColumnInfo(_primarySessionId, _primaryDatabaseId, originalTableName, columnInformation.getName(), out colBase);
                                ColumnStream stream = new ColumnStream(_primarySessionId, tableid, colBase.columnid);
                                Byte[] data = new Byte[stream.Length];
                                stream.Read(data, 0, (int)stream.Length);
                                String url = convertValue(data, columnInformation);
                                while (url.Contains('%'))
                                {

                                    url = HttpUtility.UrlDecode(url);
                                }
                                listViewItem.SubItems.Add(url);
                            }
                            else if (columnInformation.getId() == 999)
                            {
                                if (tableName.Contains("History"))
                                {
                                    String responseHeader = listViewItem.SubItems[22].Text;
                                    int startOffset = responseHeader.IndexOf("000000001F000000");
                                    if (startOffset == -1)
                                    {
                                        listViewItem.SubItems.Add("");
                                    }
                                    else
                                    {
                                        startOffset += 24;
                                        int len = BitConverter.ToInt32(hextoByte(responseHeader.Substring(startOffset - 8, 8)), 0) * 4;
                                        listViewItem.SubItems.Add(Encoding.Unicode.GetString(hextoByte(responseHeader.Substring(startOffset, len))));
                                    }
                                }
                                else if (tableName.Contains("Content"))
                                {
                                    String responseHeader = listViewItem.SubItems[22].Text;
                                    int startOffset = responseHeader.IndexOf("48545450");
                                    if (startOffset == -1)
                                    {
                                        listViewItem.SubItems.Add("");
                                    }
                                    else
                                    {
                                        int len = responseHeader.Length - 4;
                                        String url = Encoding.UTF8.GetString(hextoByte(responseHeader.Substring(startOffset, len)));
                                        while (url.Contains('%'))
                                        {

                                            url = HttpUtility.UrlDecode(url);
                                        }
                                        listViewItem.SubItems.Add(url);
                                    }
                                }
                                else if (tableName.Contains("iecompatua"))
                                {
                                    String responseHeader = listViewItem.SubItems[22].Text;
                                    listViewItem.SubItems.Add(responseHeader);
                                }
                                else if (tableName.Contains("feedplat"))
                                {
                                    String responseHeader = listViewItem.SubItems[22].Text;
                                    int len = responseHeader.IndexOf("00000000");
                                    if (len == -1)
                                    {
                                        listViewItem.SubItems.Add("");
                                    }
                                    else
                                    {
                                        if (len % 2 == 1)
                                            len++;
                                        listViewItem.SubItems.Add(Encoding.UTF8.GetString(hextoByte(responseHeader.Substring(0, len))));
                                    }
                                }
                                else if (tableName.Contains("iedownload"))
                                {
                                    String responseHeader = listViewItem.SubItems[22].Text;
                                    int startOffset = responseHeader.LastIndexOf("00000000");
                                    if (startOffset == -1)
                                    {
                                        listViewItem.SubItems.Add("");
                                    }
                                    else
                                    {
                                        if (startOffset % 2 == 1)
                                            startOffset++;

                                        int j = 0;
                                        byte[] bytes = hextoByte(responseHeader.Substring(startOffset + 8));
                                        byte[] newBytes = new byte[bytes.Length + 100];
                                        for (int i = 0; i < bytes.Length; i += 2)
                                        {
                                            if (bytes.Length > i + 1)
                                            {
                                                newBytes[j] = bytes[i];
                                                newBytes[j + 1] = bytes[i + 1];
                                                if (bytes[i] == 0x00 && bytes[i + 1] == 0x00)
                                                {
                                                    newBytes[j] = 0x0D;
                                                    newBytes[j + 1] = 0x00;
                                                    newBytes[j + 2] = 0x0A;
                                                    newBytes[j + 3] = 0x00;
                                                    newBytes[j + 4] = 0x0D;
                                                    newBytes[j + 5] = 0x00;
                                                    newBytes[j + 6] = 0x0A;
                                                    newBytes[j + 7] = 0x00;
                                                    j += 6;
                                                }
                                                j += 2;
                                            }
                                        }
                                        newBytes[j] = 0x00;
                                        newBytes[j + 1] = 0x00;

                                        listViewItem.SubItems.Add(Encoding.Unicode.GetString(newBytes));
                                    }
                                }
                            }
                            else
                            {
                                JET_COLUMNBASE colBase;
                                Api.JetGetColumnInfo(_primarySessionId, _primaryDatabaseId, originalTableName, columnInformation.getName(), out colBase);
                                ColumnStream stream = new ColumnStream(_primarySessionId, tableid, colBase.columnid);
                                Byte[] data = new Byte[stream.Length];
                                stream.Read(data, 0, (int)stream.Length);
                                listViewItem.SubItems.Add(convertValue(data, columnInformation));
                            }
                            
                            
                        }
                        listViewItemList.Add(listViewItem);
                        this.formWait.setPrgress((++index));
                    }
                    while (Api.TryMoveNext(_primarySessionId, tableid));
                }
                Api.JetCloseTable(_primarySessionId, tableid);
            }
            foreach (ColumnInformation columnInformation in columnInformationList)
            {
                coulumnDataList.Add(columnInformation.getName());
            }
            this.formWait.end("");

            if (isSaveData)
            {
                this.formWait.setLabel("Save table : " + tableName);
            }
            else
            {
                this.formWait.setLabel("Show table : " + tableName);
            }
            
            
            var result = new Tuple<String, List<String>, List<ListViewItem>>(tableName, coulumnDataList, listViewItemList);
            e.Result = result;
            
        }


        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                backgroudWoker.CancelAsync(); 
            }
        }

        public void close()
        {
            Api.JetEndSession(_primarySessionId, EndSessionGrbit.None);
            Api.JetTerm(_instance);
        }

        public void setUTCTime(String UTCAddHour, String UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
        }

        public List<string> getTableNames()
        {
            //IEnumerable<string> tableNamesIEnumerable = Api.GetTableNames(_primarySessionId, _primaryDatabaseId);
            //IEnumerator<string> tableNamesIEnumerator = tableNamesIEnumerable.GetEnumerator();
            List<String> tableNameList = tableNameDict.Keys.ToList<String>();
            
            /*
            foreach (String tableName in tableNameList){
                dct
            }
             * */
            return tableNameList;
        }

        public int getCount(String tableName)
        {
            int count = 0;
            String originalTableName = "Container_" + tableNameDict[tableName];
            
            JET_TABLEID tableid;
            if(!Api.TryOpenTable(_primarySessionId, _primaryDatabaseId, originalTableName, OpenTableGrbit.None, out tableid)){
                return -1;
            }
            Api.JetIndexRecordCount(_primarySessionId, tableid, out count, Int32.MaxValue);
            Api.JetCloseTable(_primarySessionId, tableid);
            return count;
        }
        
        public List<ColumnInformation> getColumn(String tableName)
        {
            JET_TABLEID tableid;

            List<ColumnInformation> columnInformationList = new List<ColumnInformation>();
            using (var trx = new Transaction(_primarySessionId))
            {
                String originalTableName = "Container_" + tableNameDict[tableName];
                Api.JetOpenTable(_primarySessionId, _primaryDatabaseId, originalTableName, null, 0, OpenTableGrbit.None, out tableid);
                IEnumerable<ColumnInfo> columnInfoIEnumerable = Api.GetTableColumns(_primarySessionId, tableid);
                IEnumerator<ColumnInfo> columnInfoIEnumerator = columnInfoIEnumerable.GetEnumerator();
                
                while (columnInfoIEnumerator.MoveNext())
                {
                    ColumnInfo columnInfo = columnInfoIEnumerator.Current;
                    columnInformationList.Add(new ColumnInformation(columnInfo.Coltyp, columnInfo.Columnid.GetHashCode(),  columnInfo.MaxLength, columnInfo.Name));
                }
                Api.JetCloseTable(_primarySessionId, tableid);   
            }


            if (tableName.Contains("History"))
            {
                columnInformationList.Add(new ColumnInformation(JET_coltyp.LongText, 999, 65565, "WebPageInfo"));
            }
            else if (tableName.Contains("Content"))
            {
                columnInformationList.Add(new ColumnInformation(JET_coltyp.LongText, 999, 65565, "HTTPHeader"));
            }
            else if (tableName.Contains("iecompatua"))
            {
                columnInformationList.Add(new ColumnInformation(JET_coltyp.LongText, 999, 65565, "UserAgent"));
            }
            else if (tableName.Contains("feedplat"))
            {
                columnInformationList.Add(new ColumnInformation(JET_coltyp.LongText, 999, 65565, "ResponsHeaderString"));
            }
            else if (tableName.Contains("iedownload"))
            {
                columnInformationList.Add(new ColumnInformation(JET_coltyp.LongText, 999, 65565, "iedownload"));
            }

            return columnInformationList;
        }

        public void getData(String tableName, List<ColumnInformation> columnInformationList)
        {
            isSaveData = false;
            isReadData = true;
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(readRecordsDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readRecordsRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;

            var tableData = new Tuple<String, List<ColumnInformation>>(tableName, columnInformationList);
            formWait = new FormWait();
            backgroudWoker.RunWorkerAsync(tableData);
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            formWait.ShowDialog();
        }


        public void saveData(FormSave formSave, String tableName, List<ColumnInformation> columnInformationList)
        {
            isSaveData = true;
            isReadData = false;
            this.formSave = formSave;
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(readRecordsDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(saveRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            var tableData = new Tuple<String, List<ColumnInformation>>(tableName, columnInformationList);
            formWait = new FormWait();
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            backgroudWoker.RunWorkerAsync(tableData);
            formWait.ShowDialog();
        }

        public String convertValue(byte[] data, ColumnInformation colInfo)
        {
            int colType = (int) colInfo.getColType();
            int maxLength = colInfo.getMaxLength();
            if (colInfo.getName().Contains("Time"))
            {
                long timeValue = (long)BitConverter.ToUInt64(data, 0);
                if (timeValue == 0)
                {
                    return "0";
                }
                DateTime datetime = DateTime.FromBinary(timeValue);
                datetime = datetime.AddYears(1600);
                datetime = datetime.AddHours(Int32.Parse(UTCAddHour));
                datetime = datetime.AddMinutes(Int32.Parse(UTCAddMinute));
                return datetime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (colType == 1) //bool
            {
                return "NULL";
            }
            else if (colType == 1) //bool
            {
                return data[0] == 1 ? "TRUE" : "FALSE";
            }
            else if (colType == 2) //byte
            {
                return data[0].ToString();
            }
            else if (colType == 3) //short
            {
                return BitConverter.ToInt16(data, 0).ToString();
            }
            else if (colType == 4) //integer
            {
                return BitConverter.ToInt32(data, 0).ToString();
            }
            else if (colType == 5) //binary
            {
                double temp = (double)BitConverter.ToInt64(data, 0) / 1E4;
                return temp.ToString();
            }
            else if (colType == 6) //float
            {
                return BitConverter.ToSingle(data, 0).ToString();
            }
            else if (colType == 7) //double
            {
                return BitConverter.ToDouble(data, 0).ToString();
            }
            else if (colType == 8) //dateTime
            {
                long longVar = BitConverter.ToInt64(data, 0);
                DateTime dateTimeVar = new DateTime(1980,1,1).AddMilliseconds(longVar);
                dateTimeVar.AddHours((Double)Int32.Parse(UTCAddHour));
                dateTimeVar.AddMinutes((Double)Int32.Parse(UTCAddMinute));
                return dateTimeVar.ToString();
            }
            else if (colType == 9) //binary
            {
                string hex = BitConverter.ToString(data);
                return hex.Replace("-", "");
            }
            else if (colType == 10) //text
            {
                if (maxLength == 255)
                {
                    return Encoding.UTF8.GetString(data);
                }
                else
                {
                    return Encoding.Unicode.GetString(data);
                }
                
            }
            else if (colType == 11) //binary
            {
                string hex = BitConverter.ToString(data);
                return hex.Replace("-", "");
            }
            else if (colType == 12) //text
            {
                return Encoding.Unicode.GetString(data);
            }
            else if (colType == 13) //integer
            {
                return BitConverter.ToInt32(data, 0).ToString();
            }
            else if (colType == 14) //unsigned integer
            {
                return BitConverter.ToUInt32(data, 0).ToString();
            }
            else if (colType == 15) //unsigned long long
            {
                return BitConverter.ToUInt64(data, 0).ToString();
            }
            else if (colType == 16) //text
            {
                return Encoding.UTF8.GetString(data);
            }
            else if (colType == 17) //unsigned short
            {
                return BitConverter.ToInt16(data, 0).ToString();
            }
            else
            {
                return "UNKNOWN";
            }
        }
        public void deleteAllTables()
        {

            IEnumerable<string> tableNamesIEnumerable = Api.GetTableNames(_primarySessionId, _primaryDatabaseId);
            IEnumerator<string> tableNamesIEnumerator = tableNamesIEnumerable.GetEnumerator();
            using (var trx = new Transaction(_primarySessionId))
            {
                while (tableNamesIEnumerator.MoveNext())
                {
                    String tableName = tableNamesIEnumerator.Current;
                    Api.JetDeleteTable(_primarySessionId, _primaryDatabaseId, tableName);
                }
                trx.Commit(CommitTransactionGrbit.None);
            }
        }

        public void deleteSelectTables(List<String> tableNamesList)
        {
            using (var trx = new Transaction(_primarySessionId))
            {
                foreach (String tableName in tableNamesList)
                {
                    Api.JetDeleteTable(_primarySessionId, _primaryDatabaseId, tableName);
                }
                trx.Commit(CommitTransactionGrbit.None);
            }
        }

        public void deleteRecordsInAllTables()
        {
            IEnumerable<string> tableNamesIEnumerable = Api.GetTableNames(_primarySessionId, _primaryDatabaseId);
            IEnumerator<string> tableNamesIEnumerator = tableNamesIEnumerable.GetEnumerator();
            using (var trx = new Transaction(_primarySessionId))
            {
                while (tableNamesIEnumerator.MoveNext())
                {
                    String tableName = tableNamesIEnumerator.Current;
                    JET_TABLEID tableid;
                    Api.JetOpenTable(_primarySessionId, _primaryDatabaseId, tableName, null, 0, OpenTableGrbit.None, out tableid);
                    if (Api.TryMoveFirst(_primarySessionId, tableid))
                    {
                        do
                        {
                            Api.JetDelete(_primarySessionId, tableid);
                        }
                        while (Api.TryMoveNext(_primarySessionId, tableid));
                    }
                    Api.JetCloseTable(_primarySessionId, tableid);
                }
                trx.Commit(CommitTransactionGrbit.None);
            }
        }

        private void InitializeDatabaseAndTables()
        {
            // open database for the first time: database file will be created if necessary

            // first quick check whether file exist
            if (!File.Exists(_databaseFile))
            {
                //Api.JetCreateDatabase(_primarySessionId, _databaseFile, null, out _primaryDatabaseId, CreateDatabaseGrbit.None);
                //CreateTables();
                return;
            }

            // file exist, just attach the db.
            try
            {
                // if this succeed, it will lock the file.
                Api.JetAttachDatabase(_primarySessionId, _databaseFile, AttachDatabaseGrbit.None);
            }
            catch (EsentFileNotFoundException)
            {
                // if someone has deleted the file, while we are attaching.
                //Api.JetCreateDatabase(_primarySessionId, _databaseFile, null, out _primaryDatabaseId, CreateDatabaseGrbit.None);
                //CreateTables();
                return;
            }

            Api.JetOpenDatabase(_primarySessionId, _databaseFile, null, out _primaryDatabaseId, OpenDatabaseGrbit.None);
            //InitializeTables();
        }

        private Instance CreateEsentInstance()
        {
            var instanceDataFolder = Path.GetDirectoryName(_databaseFile);

            TryInitializeGlobalParameters();

            var instance = new Instance(Path.GetFileName(_databaseFile), _databaseFile, TermGrbit.Complete);
            
            // create log file preemptively
            Api.JetSetSystemParameter(instance.JetInstance, JET_SESID.Nil, (JET_param)JET_paramLogFileCreateAsynch, /* true */ 1, null);

            // set default commit mode
            Api.JetSetSystemParameter(instance.JetInstance, JET_SESID.Nil, (JET_param)JET_paramCommitDefault, /* lazy */ 1, null);

            // remove transaction log file that is not needed anymore
            instance.Parameters.CircularLog = true;

            // transaction log file buffer 1M (1024 * 2 * 512 bytes)
            instance.Parameters.LogBuffers = 2 * 1024;

            // transaction log file is 2M (2 * 1024 * 1024 bytes)
            instance.Parameters.LogFileSize = 2 * 1024;

            // db directories
            instance.Parameters.LogFileDirectory = instanceDataFolder;
            instance.Parameters.SystemDirectory = instanceDataFolder;
            instance.Parameters.TempDirectory = instanceDataFolder;

            // Esent uses version pages to store intermediate non-committed data during transactions
            // smaller values may cause VersionStoreOutOfMemory error when dealing with multiple transactions\writing lot's of data in transaction or both
            // it is about 16MB - this is okay to be big since most of it is temporary memory that will be released once the last transaction goes away
            instance.Parameters.MaxVerPages = 16 * 1024 * 1024 / VersionStorePageSize;

            // set the size of max transaction log size (in bytes) that should be replayed after the crash
            // small values: smaller log files but potentially longer transaction flushes if there was a crash (6M)
            instance.Parameters.CheckpointDepthMax = 6 * 1024 * 1024;

            // how much db grows when it finds db is full (1M)
            // (less I/O as value gets bigger)
            instance.Parameters.DbExtensionSize = 1024 * 1024 / DatabasePageSize;

            // fail fast if log file is wrong. we will recover from it by creating db from scratch
            instance.Parameters.CleanupMismatchedLogFiles = true;
            instance.Parameters.EnableIndexChecking = true;
            
            // now, actually initialize instance
            instance.Init();

            return instance;
        }

        public void deleteAllRecords(List<String> tableNameList)
        {
            using (var trx = new Transaction(_primarySessionId))
            {
                foreach (String tableName in tableNameList)
                {
                    JET_TABLEID tableid;
                    Api.JetOpenTable(_primarySessionId, _primaryDatabaseId, tableName, null, 0, OpenTableGrbit.None, out tableid);
                    if (Api.TryMoveFirst(_primarySessionId, tableid))
                    {
                        do
                        {
                            Api.JetDelete(_primarySessionId, tableid);
                        }
                        while (Api.TryMoveNext(_primarySessionId, tableid));
                    }
                    Api.JetCloseTable(_primarySessionId, tableid);
                }
                trx.Commit(CommitTransactionGrbit.None);
            }
        }
        public byte[] hextoByte(string hexString)
        {
            int len = hexString.Length / 2;
            byte[] data = new Byte[len];
            for (int i = 0; i < len; ++i)
            {
                string toParse = hexString.Substring(i * 2, 2);
                data[i] = byte.Parse(toParse, NumberStyles.HexNumber);
            }
            return data;
        }
        private void TryInitializeGlobalParameters()
        {
            int instances;
            JET_INSTANCE_INFO[] infos;
            Api.JetGetInstanceInfo(out instances, out infos);

            // already initialized nothing we can do.
            if (instances != 0)
            {
                return;
            }

            try
            {
                // use small configuration so that esent use process heap and windows file cache
                SystemParameters.Configuration = 0;

                // allow many esent instances
                SystemParameters.MaxInstances = 1024;
                /*
                // enable perf monitor if requested
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramDisablePerfmon, _enablePerformanceMonitor ? 0 : 1, null);

                // set max IO queue (bigger value better IO perf)
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramOutstandingIOMax, 1024, null);

                // set max current write to db
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramCheckpointIOMax, 32, null);

                // better cache management (4M)
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramLRUKHistoryMax, 4 * 1024 * 1024 / DatabasePageSize, null);

                // better db performance (100K bytes)
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramPageHintCacheSize, 100 * 1024, null);

                // set version page size to normal 16K
                Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)JET_paramVerPageSize, VersionStorePageSize, null);
                */
                // use windows file system cache
                SystemParameters.EnableFileCache = true;

                // don't use mapped file for database. this will waste more VM.
                SystemParameters.EnableViewCache = false;

                // this is the unit where chunks are loaded into memory/locked and etc
                SystemParameters.DatabasePageSize = DatabasePageSize;

                // set max cache size - don't use too much memory for cache (8MB)
                SystemParameters.CacheSizeMax = 8 * 1024 * 1024 / DatabasePageSize;

                // set min cache size - Esent tries to adjust this value automatically but often it is better to help him.
                // small cache sizes => more I\O during random seeks
                // currently set to 2MB
                SystemParameters.CacheSizeMin = 2 * 1024 * 1024 / DatabasePageSize;

                // set box of when cache eviction starts (1% - 2% of max cache size)
                SystemParameters.StartFlushThreshold = 20;
                SystemParameters.StopFlushThreshold = 40;
            }
            catch (EsentAlreadyInitializedException)
            {
                // can't change global status
            }
        }

    }
}
