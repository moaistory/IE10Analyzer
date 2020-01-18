using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
namespace IE10Analyzer
{

    public class ColumnInformation
    {
        public ColumnInformation(JET_coltyp coltyp, int colId, int maxLength, String name)
        {
            this.coltyp = coltyp;
            this.colId = colId;
            this.maxLength = maxLength;
            this.name = name;
        }
        private JET_coltyp coltyp;
        private int colId;
        private int maxLength;
        private String name;

        public String getName()
        {
            return this.name;
        }
        public int getId()
        {
            return this.colId;
        }

        public JET_coltyp getColType()
        {
            return this.coltyp;
        }

        public int getMaxLength()
        {
            return this.maxLength;
        }
    }
}
