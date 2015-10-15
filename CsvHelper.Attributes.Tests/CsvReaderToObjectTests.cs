using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CsvHelper.Tests
{
    [TestClass]
    public class CsvReaderToObjectTests
    {
        [TestMethod]
        public void ReadsCsvColumnTests()
        {
            using( var reader = new CsvReader( new StreamReader(GetDataStream())) )
            {
                var csvRow = reader.ToObject<CsvRow>();
                Assert.AreEqual( 1, csvRow.Count );

                var row = csvRow.First();
                Assert.AreEqual(1, row.ColumnOne);
                Assert.AreEqual(2, row.ColumnTwo);

                Assert.AreEqual(2, row.ColumnsThreeAndFour.Count());
                Assert.AreEqual("3", row.ColumnsThreeAndFour[0]);
                Assert.AreEqual("4", row.ColumnsThreeAndFour[1]);

                Assert.AreEqual(2, row.ColumnsFiveAndSix.Count());
                Assert.AreEqual("5", row.ColumnsFiveAndSix[0]);
                Assert.AreEqual("6", row.ColumnsFiveAndSix[1]);

                Assert.AreEqual(2, row.ColumnsPrefixed.Count());
                Assert.AreEqual("M1", row.ColumnsPrefixed[0]);
                Assert.AreEqual("M2", row.ColumnsPrefixed[1]);
            }
        }

        private class CsvRow
        {
            [CsvColumn("One")]
            public int ColumnOne { get; set; }

            [CsvColumn("Two")]
            public int ColumnTwo { get; set; }

            [CsvColumnRange("Three", "Four")]
            public string[] ColumnsThreeAndFour { get; set; }

            [CsvColumnRange("Five", "Six")]
            public List<string> ColumnsFiveAndSix { get; set; }

            [CsvColumnRegexMatchedRange("^MIC_*")]
            public List<string> ColumnsPrefixed { get; set; }
        }


        public static MemoryStream GetDataStream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("One,Two,Three,Four,Five,MIC_1,Six,MIC_2");
            writer.WriteLine("1,2,3,4,5,M1,6,M2");
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}