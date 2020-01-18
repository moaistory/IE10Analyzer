using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Net;
namespace IE10Analyzer.EDBParser
{
    public class EDBParserManager
    {
        private string inputPath;

	    private int signature;
	    private int pagesize;
        private int totalPageCount;
	    private int version;
	    private int revision;
        private String UTCAddHour = "0";
        private String UTCAddMinute = "0";
        private HexReader hexReader = null;
	    private MSysObjects mSysObjects;
	    private Dictionary<int, Table> tableDict;
        private Dictionary<int, Table> lvTableDict;
        private FormMain formMain;
        private BackgroundWorker backgroudWoker;
        private FormWait formWait;
        private FormSave formSave;
        private bool isSaveData = false;
        private bool isReadData = false;
        private Dictionary<String, String> tableNameDict;


        public EDBParserManager(FormMain formMain)
        {
            this.formMain = formMain;
        }

        public void init(string inputPath){
	        this.inputPath = inputPath;
	        this.hexReader = new HexReader(inputPath);
	        this.parseDatabaseHeader();
	        this.mSysObjects = new MSysObjects(this.hexReader, this.pagesize);
            this.tableNameDict = new Dictionary<string, string>();
        }

        public void readRecordsRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<Table, List<ListViewItem>> result = (Tuple<Table, List<ListViewItem>>)e.Result;
            List<ListViewItem> listViewItemList = (List<ListViewItem>)result.Item2;
            formMain.setRecordListMethod(listViewItemList);
            formWait.Close();
        }

        public void readRecordsDoWork(object sender, DoWorkEventArgs e)
        {
            
            Tuple<Table, bool> tableData = (Tuple<Table, bool>)e.Argument;
            Table table = tableData.Item1;
            bool status = tableData.Item2;
            List<ListViewItem> listVIewItemList = new List<ListViewItem>();
            List<Dictionary<int, Item>> recordList;
            this.formWait.end("");
            this.formWait.setLabel("Table Analyzing : " + table.getTableName());
            if (status == true)//normal
            {
                recordList = table.getRecordList();
            }
            else
            {
                recordList = table.getDeletedRecordList();
            }
            int totalSize = recordList.Count;
            formWait.maxProgress(totalSize);
            int longValueNumber = table.getLongValueNumber();
            if (longValueNumber != -1)
            {
                if (lvTableDict.ContainsKey(longValueNumber))
                {
                    Table lvTable = lvTableDict[longValueNumber];
                    Dictionary<int, LVItem> lvItems = lvTable.parseLVItems();
                    table.setLVItemDict(lvItems);
                }
            }
            if (recordList.Count == 0)
            {
                e.Result =listVIewItemList;
            }
            this.formWait.end("");
            List<Column> columnList = table.getColumnList();
            
            int count = 0;
            foreach (Dictionary<int, Item> record in recordList)
            {
                if (backgroudWoker.CancellationPending == true)
                {
                    break;
                }
                ++count;
                if (count % 1000 == 0 || recordList.Count == count)
                    formWait.setPrgress(count);
                ListViewItem listVIewItem = new ListViewItem();
                foreach (Column column in columnList)
                {
                    /*
                    if (table.getTableName().Contains("Cookies"))
                    {
                        String a = "";
                        a += column.getID(); a += ",";
                        a += column.getType(); a += ",";
                        a += column.getSpaceUsage(); a += ",";
                        a += column.getName();
                        using (System.IO.StreamWriter file =
                            new System.IO.StreamWriter(@"C:\Users\KJH\Desktop\d.txt", true))
                        {
                            file.WriteLine(a);
                        }
                    }
                    */
                    int columnID = column.getID();

                    if (columnID == 999)
                    {
                        try
                        {
                            if (table.getTableName().Contains("History"))
                            {
                                String responseHeader = listVIewItem.SubItems[22].Text;
                                int startOffset = responseHeader.IndexOf("000000001F000000");
                                if (startOffset == -1)
                                {
                                    listVIewItem.SubItems.Add("");
                                }
                                else
                                {
                                    startOffset += 24;
                                    int len = BitConverter.ToInt32(hextoByte(responseHeader.Substring(startOffset - 8, 8)), 0) * 4;
                                    listVIewItem.SubItems.Add(Encoding.Unicode.GetString(hextoByte(responseHeader.Substring(startOffset, len))));
                                }
                                

                            }
                            else if (table.getTableName().Contains("Content"))
                            {
                                String responseHeader = listVIewItem.SubItems[22].Text;
                                int startOffset = responseHeader.IndexOf("48545450");
                                if (startOffset == -1)
                                {
                                    listVIewItem.SubItems.Add("");
                                }
                                else
                                {
                                    int len = responseHeader.Length - 4;
                                    String url = Encoding.UTF8.GetString(hextoByte(responseHeader.Substring(startOffset, len)));
                                    try { 
                                        url = WebUtility.HtmlDecode(url);
                                        url = Uri.UnescapeDataString(url);
                                    }
                                    catch{}
                                    listVIewItem.SubItems.Add(url);
                                }
                            }
                            else if (table.getTableName().Contains("iecompatua"))
                            {
                                String responseHeader = listVIewItem.SubItems[22].Text;
                                listVIewItem.SubItems.Add(responseHeader);
                            }
                            else if (table.getTableName().Contains("feedplat"))
                            {
                                String responseHeader = listVIewItem.SubItems[22].Text;
                                int len = responseHeader.IndexOf("00000000");
                                if (len == -1)
                                {
                                    listVIewItem.SubItems.Add("");
                                }
                                else
                                {
                                    if (len % 2 == 1)
                                        len++;
                                    listVIewItem.SubItems.Add(Encoding.UTF8.GetString(hextoByte(responseHeader.Substring(0, len))));
                                }
                            }
                            else if (table.getTableName().Contains("iedownload"))
                            {
                                String responseHeader = listVIewItem.SubItems[22].Text;
                                int startOffset = responseHeader.LastIndexOf("00000000");
                                if (startOffset == -1)
                                {
                                    listVIewItem.SubItems.Add("");
                                }
                                else
                                {
                                    if (startOffset % 2 == 1)
                                        startOffset++;

                                    int j = 0;
                                    byte[] bytes = hextoByte(responseHeader.Substring(startOffset + 8));
                                    byte[] newBytes = new byte[bytes.Length+100];
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
                                    newBytes[j+1] = 0x00;

                                    listVIewItem.SubItems.Add(Encoding.Unicode.GetString(newBytes));
                                    
                                    /*
                                    

                                    


                                    for (int i = 0; i < bytes.Length; i += 2)
                                    {
                                        newBytes[j] = bytes[i];
                                        newBytes[j + 1] = bytes[i + 1];
                                        j += 2;
                                        if (bytes[i] == 0 && bytes[i + 1] == 0)
                                        {
                                            writeString += Encoding.Unicode.GetString(newBytes) + @"\r\n";
                                            j = 0;
                                        }
                                    }
                                    writeString = writeString.Replace(Encoding.Unicode.GetString(new byte[] { 0, 0 }), @"\r\n");
                                    writeString += Encoding.Unicode.GetString(new byte[] { 0, 0 });
                                     * */
                                    //listVIewItem.SubItems.Add(writeString);
                                    /*
                                    if (startOffset % 2 == 1)
                                        startOffset++;
                                    listVIewItem.SubItems.Add(Encoding.Unicode.GetString(hextoByte(responseHeader.Substring(startOffset + 8).Replace("000000", "000D000A000D000A00"))));
                                     * */
                                }
                            }
                        }
                        catch
                        {
                            listVIewItem.SubItems.Add("");
                        }
                    }
                    else if (!record.ContainsKey(columnID))
                    {
                        listVIewItem.SubItems.Add("");
                    }
                    else
                    {
                        Item item = record[columnID];
                        int recordID = item.getID();
                        int recordSize = item.getSize();
                        if (recordID > 0 && recordID <= 0x80)
                        {
                            listVIewItem.SubItems.Add(item.getValue());
                            if (columnID == 7 && column.getName().Equals("Type"))
                            {
                                int index = listVIewItem.SubItems.Count - 1;
                                String strType = listVIewItem.SubItems[index].Text;
                                int iType = 0;
                                if (Int32.TryParse(strType, out iType))
                                {
                                    strType = "";

                                    if ((iType & 0x00000001) == 0x00000001)
                                    {
                                        strType += "[Normal]";
                                    }
                                    if ((iType & 0x00000400) == 0x00000400)
                                    {
                                        strType += "[Download]";
                                    }
                                    if ((iType & 0x00000800) == 0x00000800)
                                    {
                                        strType += "[Redirect]";
                                    }
                                    if ((iType & 0x00020000) == 0x00020000)
                                    {
                                        strType += "[PrivateBrowsing]";
                                    }
                                    if ((iType & 0x00100000) == 0x00100000)
                                    {
                                        strType += "[Cookie]";
                                    }
                                    if ((iType & 0x00200000) == 0x00200000)
                                    {
                                        strType += "[Url]";
                                    }
                                    if ((iType & 0x00400000) == 0x00400000)
                                    {
                                        strType += "[PendingDelete]";
                                    }
                                    if ((iType & 0x04000000) == 0x04000000)
                                    {
                                        strType += "[PostResponse]";
                                    }
                                    if ((iType & 0x10000000) == 0x10000000)
                                    {
                                        strType += "[Installed]";
                                    }
                                    listVIewItem.SubItems[index].Text = strType;
                                }


                            }

                        }
                        else if (recordID >= 0x80 && recordID < 0x100)
                        {
                            if (recordSize < 0 || recordSize >= 256)
                            {
                                listVIewItem.SubItems.Add("Parsig Error : fixed size item");
                            }
                            else
                            {
                                listVIewItem.SubItems.Add(item.getValue());
                            }
                        }
                        else if (recordID >= 0x100 && recordID < 0xFFFF)
                        {
                            if (recordSize < 0 || recordSize >= 65535)
                            {
                                listVIewItem.SubItems.Add("Parsig Error : variable size item");
                            }
                            else
                            {
                                byte tagFlag = item.getTaggedDataItemFlag();
                                if (tagFlag == 0 || tagFlag == 1)
                                { //common
                                    listVIewItem.SubItems.Add(item.getValue());
                                }
                                else if (tagFlag == 3)
                                { //compress text
                                    listVIewItem.SubItems.Add(item.getDecompressText());
                                }
                                else
                                { //Pointer
                                    int pointerNumber = item.getPointNumber();
                                    Dictionary<int, LVItem> lvItemDict = table.getLvItemDict();
                                    if (lvItemDict.ContainsKey(pointerNumber))
                                    {
                                        listVIewItem.SubItems.Add(lvItemDict[pointerNumber].getData(item.getID(), item.getType()));
                                    }
                                    else
                                    {
                                        listVIewItem.SubItems.Add("Parsig Error : lV pointer");
                                    }
                                }
                            }


                            if (columnID == 256 && column.getName().Equals("Url"))
                            {
                                
                                int index = listVIewItem.SubItems.Count - 1;
                                String url = listVIewItem.SubItems[index].Text;
                                try { 
                                    url = WebUtility.HtmlDecode(url);
                                    url = Uri.UnescapeDataString(url);
                                }
                                catch{}
                                listVIewItem.SubItems[index].Text =url;
                            }
                        }
                    }

                    

                }
                listVIewItemList.Add(listVIewItem);
            }
            this.formWait.end("");
            if (isSaveData)
            {
                this.formWait.setLabel("Saving table : " + table.getTableName());
            }
            else
            {
                this.formWait.setLabel("Showing table : " + table.getTableName());
            }
            
            
            var result = new Tuple<Table, List<ListViewItem>>(table, listVIewItemList);
            e.Result = result;

        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                backgroudWoker.CancelAsync();
            }
        }

        public void colse()
        {
            if (this.hexReader != null)
            {
                this.hexReader.close();
                this.hexReader = null;
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

        public bool parseDatabaseHeader(){
	        DatabaseHeader databaseHeader = new DatabaseHeader(this.hexReader);
	        databaseHeader.parseHeaderArea();
	        this.signature = databaseHeader.getSignature();
	        if(this.signature != -1985229329){
                return false;
	        }
	        this.pagesize = databaseHeader.getPagesize();
	        this.version = databaseHeader.getVersion();
	        this.revision = databaseHeader.getRevision();
            return true;
        }

        public void makeTableRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            formWait.Close();
        }

        public void makeTableDoWork(object sender, DoWorkEventArgs e)
        {
            this.mSysObjects.makeTables();
            this.lvTableDict = this.mSysObjects.getLvTableDict();
            Table containers = this.mSysObjects.getContainersTable();

            Dictionary<int, Table> allTableDict = this.mSysObjects.getTableDict();

            this.tableDict = new Dictionary<int, Table>();
            List<Dictionary<int, Item>> containerList = containers.parseRecord();
            
            foreach (Dictionary<int, Item> recordList in containerList)
            {
                
                String containerID = recordList[1].getValue();
                String containerName = recordList[128].getValue().Replace("\0", string.Empty);


                String partitionID = recordList[256].getValue().Replace("\0", string.Empty);

                String newTableName = containerName + "(" + partitionID + ")";

                foreach (int id in allTableDict.Keys)
                {
                    String tableName = allTableDict[id].getTableName();
                    if (tableName.Equals("Container_" + containerID))
                    {
                        this.tableDict.Add(id, allTableDict[id]);

                        if (this.formMain.isParsing())
                        {
                            this.tableDict[id].setTableName(newTableName);
                        }
                        else if (this.formMain.isRecovery())
                        {
                            this.tableDict[id].setTableName(newTableName + "_Recovered");
                        }
                        
                        
                        if (containerName.Equals("History"))
                        {
                            Column column = new Column(999, 12, 65565, "WebPageInfo");
                            tableDict[id].addColumn(column);
                        }
                        else if (containerName.Equals("Content"))
                        {
                            Column column = new Column(999, 12, 65565, "HTTPHeader");
                            tableDict[id].addColumn(column);
                        }
                        else if (containerName.Equals("iecompatua") )
                        {
                            Column column = new Column(999, 12, 65565, "UserAgent");
                            tableDict[id].addColumn(column);
                        }
                        else if (containerName.Equals("feedplat"))
                        {
                            Column column = new Column(999, 12, 65565, "ResponsHeaderString");
                            tableDict[id].addColumn(column);
                        }
                        else if (containerName.Equals("iedownload"))
                        {
                            Column column = new Column(999, 12, 65565, "DownloadPath");
                            tableDict[id].addColumn(column);
                        }

                        break;
                    }
                    
                }
            }


            foreach (int id in allTableDict.Keys)
            {
                if (!tableDict.ContainsKey(id))
                {
                    String tableName = allTableDict[id].getTableName();
                    if(tableName.Contains("Container_"))
                    {
                        
                        tableDict.Add(id, allTableDict[id]);
                    }
                }
            }
        }

        public void makeTable(){
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(makeTableDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(makeTableRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            formWait = new FormWait();
            formWait.end("");
            formWait.setLabel("Please wait while parsing tables");
            formWait.setStyle(ProgressBarStyle.Marquee);
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            backgroudWoker.RunWorkerAsync();
            formWait.ShowDialog();
        }

        public Dictionary<int,Table> getTableDict(){
            return this.tableDict;
        }

        public Table getTable(int tableNumber)
        {
            if(tableDict.ContainsKey(tableNumber))
                return this.tableDict[tableNumber];
            else
                return null;
        }

       

        public void parseRecord(Table table, bool status)
        {
            isSaveData = false;
            isReadData = true;
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(readRecordsDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readRecordsRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            formWait = new FormWait();
            formWait.setStyle(ProgressBarStyle.Blocks);
            formWait.setLabel("Please wait while loading records");
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            backgroudWoker.RunWorkerAsync(new Tuple<Table, bool>(table, status));
            formWait.ShowDialog();
        }

        public void findTablePage()
        {
	        int totalPageCount = (int)this.hexReader.getFileSize() / this.pagesize;
	        int offset = 0;
	        int tableNumber = 0;
	        int pageFlags = 0;
	        for(int curPageNum = 1; curPageNum<totalPageCount-1; curPageNum++){
		        offset = (curPageNum + 1) * this.pagesize;
		        pageFlags = this.hexReader.readInt(offset + 36);
		        if((pageFlags & 0x20) == 0x20){ //space page flag
			        continue;
		        }else if((pageFlags & 0x40) == 0x40){ //index page flag
			        continue;
		        }else if((pageFlags & 0x80) == 0x80){ //int value page flag
			        continue;
		        }
		        if ((pageFlags & 0x00000004) != 0x00000004){ // branch page flag
			        if ((pageFlags & 0x000000EF) != 0x00000001){ // branch page flag
				        if((pageFlags & 0x08) != 0x08){ //empty page flag
					        continue;
				        }
			        }
		        }
		        tableNumber = this.hexReader.readInt(offset + 24);
		        if(this.tableDict.ContainsKey(tableNumber)){
                    Table table = this.tableDict[tableNumber];
			        table.addDeletedPageSet(curPageNum);
		        }
	        }
        }

        public void saveData(FormSave formSave, Table table, Boolean status)
        {
            isSaveData = true;
            isReadData = false;
            this.formSave = formSave;
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(readRecordsDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(saveRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;

            var tableData = new Tuple<Table, Boolean>(table, status);
            backgroudWoker.RunWorkerAsync(tableData);
            formWait = new FormWait();
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            formWait.ShowDialog();
        }

        public void saveRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<Table, List<ListViewItem>> result = (Tuple<Table, List<ListViewItem>>)e.Result;
            Table table = result.Item1;
            String tableName = table.getTableName();
            List<String> columnNameList = table.getColumnNameList();
            List<ListViewItem> listViewItemList = (List<ListViewItem>)result.Item2;
            formSave.saveRecordListMethod(tableName, columnNameList, listViewItemList);
            formWait.Close();
            formSave.closeForm();
        }

        public void setUTCTime(String UTCAddHour, String UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
            this.mSysObjects.setUTCTime(UTCAddHour, UTCAddMinute);
        }

        public void setCarveTable()
        {
            this.tableDict = new Dictionary<int, Table>();
            Table CarveHistoryTable = getDefaultWebCacheTable("History");
            Table CarveContentTable = getDefaultWebCacheTable("Contents");
            Table CarveDownloadsTable = getDefaultWebCacheTable("iedownload");
            Table CarveCookiesTable = getDefaultWebCacheTable("Cookies");
            Table CarveDOMStoreTable = getDefaultWebCacheTable("DOMStore");
            Table CarveIecompatuaTable = getDefaultWebCacheTable("iecompatua");
            Table CarveIecompatTable = getDefaultWebCacheTable("iecompat");
            Table CarvePrivateBrowsingTable = getDefaultWebCacheTable("PrivateBrowsing");
            Table CarveMSHistTable = getDefaultWebCacheTable("MSHist(History)");
            Table CarveETCTable = getDefaultWebCacheTable("ETC.");
            CarveHistoryTable.addColumn(new Column(999, 12, 65565, "WebPageInfo"));
            CarveMSHistTable.addColumn(new Column(999, 12, 65565, "WebPageInfo"));
            CarveContentTable.addColumn(new Column(999, 12, 65565, "HTTPHeader"));
            CarveDownloadsTable.addColumn(new Column(999, 12, 65565, "DownloadPath"));
            CarveIecompatuaTable.addColumn(new Column(999, 12, 65565, "UserAgent"));
            this.tableDict.Add(1, CarveHistoryTable);
            this.tableDict.Add(2, CarveContentTable);
            this.tableDict.Add(3, CarveDownloadsTable);
            this.tableDict.Add(4, CarveCookiesTable);
            this.tableDict.Add(5, CarveDOMStoreTable);
            this.tableDict.Add(6, CarveIecompatuaTable);
            this.tableDict.Add(7, CarveIecompatTable);
            this.tableDict.Add(8, CarvePrivateBrowsingTable);
            this.tableDict.Add(9, CarveMSHistTable);
            this.tableDict.Add(10, CarveETCTable);
            this.totalPageCount = (int)this.hexReader.getFileSize() / this.pagesize;
        }

        public int getTootalPageCount()
        {
            return this.totalPageCount;
        }

        public void carvePage(int pageNum)
        {
            List<Dictionary<int, Item>> recordList;
            Table CarveTempTable = getDefaultWebCacheTable("Temp_Carved");
            CarveTempTable.addDeletedPageSet(pageNum);
            CarveTempTable.carveRecord();
            recordList = CarveTempTable.getDeletedRecordList();
            
            if (recordList.Count > 0)
            {
                foreach (Dictionary<int, Item> record in recordList)
                {
                    if (record.Keys.Contains(7))
                    {
                        int iType = 0;
                        String strType = record[7].getValue();
                        String URL = "";
                        if (record.Keys.Contains(256))
                        {
                            URL = record[256].getValue();
                        }
                        if (Int32.TryParse(strType, out iType))
                        {
                            if ((iType & 0x00020000) == 0x00020000)
                            {
                                this.tableDict[8].addRecord(record); //PrivateBrowsing
                                continue;
                            }
                            if ((iType & 0x00100000) == 0x00100000)
                            {
                                this.tableDict[4].addRecord(record); //Cookie
                                continue;
                            }
                            if ((iType & 0x00200000) == 0x00200000)
                            {
                                if (URL.StartsWith("Visited"))
                                {
                                    this.tableDict[1].addRecord(record); //History
                                }
                                else
                                {
                                    this.tableDict[9].addRecord(record); //MSHist
                                }
                                continue;
                            }
                            if ((iType & 0x00000400) == 0x00000400)
                            {
                                this.tableDict[3].addRecord(record); //Download
                                continue;
                            }
                        }
                        if (URL != "") 
                        {
                            if (URL.StartsWith("DOMStore"))
                            {
                                this.tableDict[5].addRecord(record); //DOMStore
                                continue;
                            }
                            if (URL.StartsWith("iecompatua"))
                            {
                                this.tableDict[6].addRecord(record); //iecompatua
                                continue;
                            }
                            if (URL.StartsWith("iecompat"))
                            {
                                this.tableDict[7].addRecord(record); //iecompat
                                continue;
                            }
                            if (URL.StartsWith("iedownload"))
                            {
                                this.tableDict[3].addRecord(record); //download
                                continue;
                            }
                            if (record.Keys.Contains(11) && record.Keys.Contains(257))
                            {
                                String creationTime = record[11].getValue();
                                String fileName = record[257].getValue();
                                if(creationTime != "0" && fileName != "" && (URL.StartsWith("res") || URL.StartsWith("http")))
                                {
                                    this.tableDict[2].addRecord(record); //content
                                    continue;
                                }
                            }
                        }
                        this.tableDict[10].addRecord(record); //ETC
                    }
                }
            }
        }

        public Table getDefaultWebCacheTable(String tableName)
        {
            Table table = new Table(this.hexReader, this.pagesize, tableName, 1000, 1000);
            table.addColumn(new Column(1, 15, 8, "EntryId"));
            table.addColumn(new Column(2, 15, 8, "ContainerId"));
            table.addColumn(new Column(3, 15, 8, "CacheId"));
            table.addColumn(new Column(4, 15, 8, "UrlHash"));
            table.addColumn(new Column(5, 14, 4, "SecureDirectory"));
            table.addColumn(new Column(6, 15, 8, "FileSize"));
            table.addColumn(new Column(7, 14, 4, "Type"));
            table.addColumn(new Column(8, 14, 4, "Flags"));
            table.addColumn(new Column(9, 14, 4, "AccessCount"));
            table.addColumn(new Column(10, 15, 8, "SyncTime"));
            table.addColumn(new Column(11, 15, 8, "CreationTime"));
            table.addColumn(new Column(12, 15, 8, "ExpiryTime"));
            table.addColumn(new Column(13, 15, 8, "ModifiedTime"));
            table.addColumn(new Column(14, 15, 8, "AccessedTime"));
            table.addColumn(new Column(15, 15, 8, "PostCheckTime"));
            table.addColumn(new Column(16, 14, 4, "SyncCount"));
            table.addColumn(new Column(17, 14, 4, "ExemptionDelta"));
            table.addColumn(new Column(256, 12, 65536, "Url"));
            table.addColumn(new Column(257, 12, 1024, "Filename"));
            table.addColumn(new Column(258, 12, 1024, "FileExtension"));
            table.addColumn(new Column(259, 11, 65536, "RequestHeaders"));
            table.addColumn(new Column(260, 11, 65536, "ResponseHeaders"));
            table.addColumn(new Column(261, 12, 65536, "RedirectUrl"));
            table.addColumn(new Column(262, 11, 65536, "Group"));
            table.addColumn(new Column(263, 11, 65536, "ExtraData"));
            return table;
        }
        
    }
}
