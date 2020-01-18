using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IE10Analyzer.EDBParser
{
    public class Column
    {
        private int id;
        private int type;
        private int spaceUsage;
        private string name;
        public Column() { }

        public Column(int id, int type, int spaceUsage, string name)
        {
            this.id = id;
            this.type = type;
            this.spaceUsage = spaceUsage;
            this.name = name;
        }


        public int getID()
        {
            return this.id;
        }

        public int getType()
        {
            return this.type;
        }

        public int getSpaceUsage()
        {
            return this.spaceUsage;
        }

        public string getName()
        {
            return this.name;
        }
    }
}
