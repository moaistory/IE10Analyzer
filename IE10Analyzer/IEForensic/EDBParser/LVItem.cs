using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IE10Analyzer.EDBParser
{
    public class LVItem
    {
        private HexReader hexReader;
        private int lvPtrNumber;
        private int totalSize;
        private List<Item> dataList;
        public LVItem(){}
        public LVItem(HexReader hexReader, int lvPtrNumber, int totalSize){
	        this.hexReader = hexReader;
	        this.lvPtrNumber = lvPtrNumber;
	        this.totalSize = totalSize;
	        this.dataList = new List<Item>();
        }


        public void addData(Item item){	        
	        this.dataList.Add(item);
        }

        public string getData(int id, ColType.TYPE type){
	        string value = "";
	        foreach(Item item in this.dataList){			
		        item.setIdType(id, type);
		        value += item.getValue();
	        }
	
	        return value;
        }
    }
}
