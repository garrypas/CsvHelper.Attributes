using System;

namespace CsvHelper
{
    public class CsvColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public CsvColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
