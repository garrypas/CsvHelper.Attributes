using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CsvHelper
{
    public static class CsvReaderToObject
    {
        public static ICollection<T> ToObject<T>(this CsvReader reader) where T : new()
        {
            var rows = new List<T>();
            while( reader.Read() )
            {
                var t = new T(); 
                foreach (var pInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public))
                {

                    foreach( CsvColumnAttribute attr in pInfo.GetCustomAttributes( true ).Where( a => a.GetType() == typeof( CsvColumnAttribute ) ) )
                    {
                        var csvColumnName = attr.ColumnName;
                        pInfo.SetValue( t, reader.GetField( pInfo.PropertyType, csvColumnName ), null);
                    }

                    foreach( CsvColumnRangeAttribute attr in pInfo.GetCustomAttributes( true ).Where( a => a.GetType() == typeof( CsvColumnRangeAttribute ) ) )
                    {
                        var collectionValue = GetCollection(pInfo.PropertyType, reader, attr.ColumnNames);
                        SetValue(collectionValue, pInfo, t);
                    }

                    foreach (CsvColumnRegexMatchedRangeAttribute attr in pInfo.GetCustomAttributes(true).Where(a => a.GetType() == typeof(CsvColumnRegexMatchedRangeAttribute)))
                    {
                        var collectionValue = GetCollectionRegexMatch(pInfo.PropertyType, reader, attr.RegexMatch);
                        SetValue(collectionValue, pInfo, t);
                    }
                }
                rows.Add(t);
            }
            return rows;
        }

        private static void SetValue<T>(object collectionValue, PropertyInfo pInfo, T t) where T : new()
        {
            if (collectionValue != null)
            {
                pInfo.SetValue(t, collectionValue, null);
            }
            else
            {
                throw new InvalidOperationException("CSVHelper multi-column Attributes must be used on a property of type Array or of a type that inherits from ICollection<T>");
            }
        }

        private static object GetCollectionRegexMatch(Type propertyType, CsvReader reader, string regexMatch)
        {
            var columnNames = reader.FieldHeaders.Where(fh => Regex.IsMatch(fh, regexMatch, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).ToList();
            return GetCollection(propertyType, reader, columnNames);
        }

        private static object GetCollection(Type propertyType, ICsvReader reader, IReadOnlyList<string> columnNames)
        {
            object collectionValue = null;
            if (propertyType.IsArray)
            {
                collectionValue = columnNames.Select(colName => reader.GetField(propertyType.GetElementType(), colName) as string).ToArray();
            }
            else if (propertyType.IsGenericType && propertyType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                collectionValue = columnNames.Select(colName => reader.GetField(propertyType.GetGenericArguments().First(), colName) as string).ToList();
            }
            return collectionValue;
        }
    }
}
