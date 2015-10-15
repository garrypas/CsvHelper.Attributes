using System;

namespace CsvHelper
{
    public class CsvColumnRegexMatchedRangeAttribute : Attribute
    {
        public string RegexMatch { get; private set; }

        public CsvColumnRegexMatchedRangeAttribute(string regexMatch)
        {
            this.RegexMatch = regexMatch;
        }
    }
}
