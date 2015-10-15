using System;

namespace CsvHelper
{
    public class CsvColumnRangeAttribute : Attribute
    {
        public string[] ColumnNames { get; private set; }

        public CsvColumnRangeAttribute(params string[] columnNames)
        {
            this.ColumnNames = columnNames;
        }
    }
}
