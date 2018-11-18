using System;
using System.Collections.Generic;
using Xunit;

namespace FixedWidthParserWriter.Tests
{
    public class DataFileTest
    {
        [Fact]
        public void FileParserTest()
        {
            List<string> fileLines = GetDataFile();

            var invoice = new Invoice().Parse(fileLines);

            Assert.Equal("SoftysTech LCC", invoice.CompanyName);
            Assert.Equal(new DateTime(2018, 10, 30), invoice.Date);
            Assert.Equal("SysCompanik", invoice.BuyerName);
            Assert.Equal("0169/18", invoice.InvoiceNumber);
            Assert.Equal(1192.00m, invoice.AmountTotal);
            Assert.Equal(new DateTime(2018, 10, 31), invoice.DateCreated);
            Assert.Equal("Financial Manager", invoice.SignatoryTitle);
            Assert.Equal("John Doe", invoice.SignatureName);
        }

        [Fact]
        public void FileWriterTest()
        {
            var invoice = new Invoice()
            {
                CompanyName = "SoftysTech LCC",
                Date = new DateTime(2018, 10, 30),
                BuyerName = "SysCompanik",
                InvoiceNumber = "0169/18",
                AmountTotal = 1192.00m,
                DateCreated = new DateTime(2018, 10, 31),
                SignatureName = "John Doe",
                SignatoryTitle = "Financial Manager",
            };

            invoice.Content = GetDataFormTemplate();
            invoice.UpdateContent();

            List<string> fileLines = GetDataFile();
            int numberOfLines = fileLines.Count;
            for (int i = 0; i < numberOfLines; i++)
            {
                Assert.Equal(fileLines[i], invoice.Content[i]);
            }
        }

        public List<string> GetDataFile()
        {
            var dataLines = new List<string>
            {
                "SoftysTech LCC",
                "__________________________________________________________________",
                "",
                "Invoice Date: 2018-10-30         Buyer:   SysCompanik",
                "",
                "                        INVOICE no. 0169/18",
                "",
                "No |         Description         | Qty |   Price    |   Amount   |",
                "...                                                               ",
                "...                                                               ",
                "------------------------------------------------------------------",          
                "                                                          1,192.00",
                "",
                "Date: 2018-10-31                                 Financial Manager",
                "                                                          John Doe"
            };
            return dataLines;
        }

        public List<string> GetDataFormTemplate()
        {
            var dataLines = new List<string>
            {
                "{CompanyName}",
                "__________________________________________________________________",
                "",
                "Invoice Date: {InvoiceDate}      Buyer:   {BuyerName}",
                "",
                "                        INVOICE no. NNNN/YY",
                "",
                "No |         Description         | Qty |   Price    |   Amount   |",
                "...                                                               ",
                "...                                                               ",
                "------------------------------------------------------------------",
                "                                                              0.00",
                "",
                "Date: {DateCreated}                               {SignatoryTitle}",
                "                                                   {SignatureName}"
            };
            return dataLines;
        }
    }
}
