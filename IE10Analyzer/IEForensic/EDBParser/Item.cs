using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IE10Analyzer.EDBParser
{
    public class Item
    {
        private int offset;
        private int size;
        private int id;
        private int maxLength;
        private string columnName;
        private ColType.TYPE type;
        private byte taggedDataItemFlag;
        private HexReader hexReader;
        private string UTCAddHour;
        private string UTCAddMinute;
        public Item() { }


        public Item(HexReader hexReader, int id, string columnName, ColType.TYPE type, int maxLength, int offset, int size)
        {
            this.hexReader = hexReader;
            this.id = id;
            this.columnName = columnName;
            this.offset = offset;
            this.size = size;
            this.maxLength = maxLength;
            this.type = type;
            if (this.id >= 0x100)
            {
                this.taggedDataItemFlag = hexReader.readByteDump(offset, 1)[0];
                this.offset++;
                this.size--;
            }
            else
            {
                this.taggedDataItemFlag = 0;
            }
        }

        public void UTCTime(string UTCAddHour, string UTCAddMinute)
        {
            this.UTCAddHour = UTCAddHour;
            this.UTCAddMinute = UTCAddMinute;
        }

        public int getID()
        {
            return this.id;
        }

        public ColType.TYPE getType()
        {
            return this.type;
        }


        public byte getTaggedDataItemFlag()
        {
            return this.taggedDataItemFlag;
        }

        public int getSize()
        {
            return this.size;
        }

        public void setIdType(int id, ColType.TYPE type)
        {
            this.id = id;
            this.type = type;
        }

        public int getPointNumber()
        {
            Byte[] data = hexReader.readByteDump(offset, 4);
            return BitConverter.ToInt32(data, 0);
        }

        public string getValue()
        {
            try
            {
                Byte[] data = hexReader.readByteDump(offset, size);

                if (columnName.Contains("Time"))
                {
                    try
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
                    catch
                    {
                        return "0";
                    }
                }

                if (this.type == ColType.TYPE.JET_coltypNil) //bool
                {
                    return "NULL";
                }
                else if (this.type == ColType.TYPE.JET_coltypBit) //bool
                {
                    return data[0] == 1 ? "TRUE" : "FALSE";
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedByte) //byte
                {
                    return data[0].ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypShort) //short
                {
                    return BitConverter.ToInt16(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypLong) //integer
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypCurrency) //binary
                {
                    double temp = (double)BitConverter.ToInt64(data, 0) / 1E4;
                    return temp.ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypIEEESingle) //float
                {
                    return BitConverter.ToSingle(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypIEEEDouble) //double
                {
                    return BitConverter.ToDouble(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypDateTime) //dateTime
                {
                    long longVar = BitConverter.ToInt64(data, 0);
                    DateTime dateTimeVar = new DateTime(1980, 1, 1).AddMilliseconds(longVar);
                    dateTimeVar.AddHours((Double)Int32.Parse(UTCAddHour));
                    dateTimeVar.AddMinutes((Double)Int32.Parse(UTCAddMinute));
                    return dateTimeVar.ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypBinary) //binary
                {
                    string hex = BitConverter.ToString(data);
                    return hex.Replace("-", "");
                }
                else if (this.type == ColType.TYPE.JET_coltypText) //text
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
                else if (this.type == ColType.TYPE.JET_coltypLongBinary) //binary
                {
                    string hex = BitConverter.ToString(data);
                    return hex.Replace("-", "");
                }
                else if (this.type == ColType.TYPE.JET_coltypLongText) //text
                {
                    return Encoding.Unicode.GetString(data);
                }
                else if (this.type == ColType.TYPE.JET_coltypSLV) //integer
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedLong) //unsigned integer
                {
                    return BitConverter.ToUInt32(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypLongLong) //unsigned long long
                {
                    return BitConverter.ToUInt64(data, 0).ToString();
                }
                else if (this.type == ColType.TYPE.JET_coltypGUID) //text
                {
                    return Encoding.UTF8.GetString(data);
                }
                else if (this.type == ColType.TYPE.JET_coltypUnsignedShort) //unsigned short
                {
                    return BitConverter.ToInt16(data, 0).ToString();
                }
                else
                {
                    return "UNKNOWN";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message.ToString() + "\r\noffset : " + offset, "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Parsing Error : (Offset : " + offset + ")";
            }
        }

        private string ByteToBitStream(byte value){
            string buffer = "";
            buffer += (value & 0x80)!= 0  ? '1':'0';
            buffer += (value & 0x40)!= 0  ? '1':'0';
            buffer += (value & 0x20)!= 0  ? '1':'0';
            buffer += (value & 0x10)!= 0  ? '1':'0';
            buffer += (value & 0x08)!= 0  ? '1':'0';
            buffer += (value & 0x04)!= 0  ? '1':'0';
            buffer += (value & 0x02)!= 0  ? '1':'0';
            buffer += (value & 0x01)!= 0  ? '1':'0';
            return buffer;
        }
        private byte BitStreamToByte(string value){
            int toShift = 0;
            char[] buffer = value.ToCharArray();
	        int size = value.Length;
	        for(int i = 0; i < size; i++) {
		        toShift = (toShift << 1);
                if(buffer[i] == '1') {    
			        toShift++;
                }
            }
            return (byte)toShift;
        }


        public string getDecompressText()
        {
            List<byte> data = new List<byte>();
            string temp1 = "";
            string temp2 = "";

            byte byteValue = 0;
            for (int i = 1; i < this.size; i++)
            {
                byteValue = this.hexReader.readByteDump(offset + i,1)[0];
                temp2 = ByteToBitStream(byteValue);
                for (int j = temp2.Length; j < 8; j++)
                {
                    temp2 = '0' + temp2;
                }
                temp1 = temp2 + temp1;
                while (temp1.Length >= 7)
                {
                    
                    temp2 = temp1.Substring(temp1.Length - 7);
                    data.Add(BitStreamToByte(temp2));
                    temp1 = temp1.Substring(0, temp1.Length - 7);
                }
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}
