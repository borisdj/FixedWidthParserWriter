using System;
using System.Collections.Generic;
using Xunit;

namespace FixedWidthParserWriter.Tests
{
    public class CustomFileTest
    {
        [Fact]
        public void CustomFileParserTest()
        {
            List<string> fileLines = GetDataFile();

            Report report = new CustomFileProvider<Report>().Parse(fileLines);

            Assert.Equal("SoftysTech LCC", report.CompanyName);
            Assert.Equal(new DateTime(2020, 6, 30), report.Date);
            Assert.Equal("11/20", report.Number);
            Assert.Equal("162-677-169-796", report.Account);
            Assert.Equal(1234.55m, report.Revenue);
        }

        public List<string> GetDataFile()
        {
            var dataLines = new List<string>
            {
                "SoftysTech LCC company",
                "__________________________________________________________________",
                "",
                "Date generated: 30.06.2020.",
                "",
                "                       Q REPORT no. 11/20**  ",
                "1569ccACTN.162-677-169-796",
                "...",
                "Revenue:",
                "1,234.55",
                "...",
                "xx",
            };
            return dataLines;
        }
    }
}
