using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace IE10Analyzer
{
    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }
        public CsvFileWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(List<string> rows)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string row in rows)
            {
                string value = row.Replace(Environment.NewLine, "[NewLine]");
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            WriteLine(builder.ToString());
        }
    }
}
