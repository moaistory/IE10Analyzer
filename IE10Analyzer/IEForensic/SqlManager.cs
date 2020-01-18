using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace IE10Analyzer
{
    public class SqlManager
    {
        private String filePath = @".\ESEDATABASE.db";
        
        SQLiteConnection m_dbConnection;
        public SqlManager(String filePath)
        {
            this.filePath = filePath;
        }

        public Boolean open()
        {
            /*
            if (File.Exists(filePath))
            {
                MessageBox.Show("this file already exist");
                return false;
            }
             * */
            m_dbConnection = new SQLiteConnection("Data Source=" + filePath);
            m_dbConnection.Open();
            return true;
        }

        public void close()
        {
            
            m_dbConnection.Close();
        }

        public bool createTable(String tableName ,List<String> columnsList)
        {
            
            int columnsCount = columnsList.Count;
            
            if(columnsCount ==0){
                return false;
            }

            
            string sql = "CREATE TABLE IF NOT EXISTS " + tableName +" (" ; 
            
            for(int i=0; i<columnsCount; i++){
                if (i == columnsCount - 1)
                {
                    sql += "_" + columnsList[i] + " TEXT)";
                }
                else
                {
                    sql += "_" + columnsList[i] + " TEXT, ";
                }
            }
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            return true;
        }

        public bool insertTable(String tableName, List<String> ColumnsList, List<ListViewItem> dataList){
            tableName = tableName.Replace('(', '_').Replace(')', '_').Replace(':', '_');
            if (createTable(tableName, ColumnsList))
            {
                string sql = "INSERT INTO " + tableName + " (";
                for (int i = 0; i < ColumnsList.Count - 1; i++)
                {
                    sql += "_" + ColumnsList[i] + ", ";
                }
                sql += "_" + ColumnsList[ColumnsList.Count - 1] + ") VALUES (";


                for (int i = 0; i < ColumnsList.Count - 1; i++)
                {
                    sql += "@"+ i + ",";
                }
                sql += "@" + (ColumnsList.Count-1) + ")";


                foreach (ListViewItem listViewItem in dataList)
                {
                    SQLiteCommand insertSQL = new SQLiteCommand(sql, m_dbConnection);

                    for (int i = 1; i < listViewItem.SubItems.Count; i++)
                    {
                        insertSQL.Parameters.Add(new SQLiteParameter("@" + (i-1), listViewItem.SubItems[i].Text));
                    }

                    insertSQL.ExecuteNonQuery();
                    
            
                }
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }
}
