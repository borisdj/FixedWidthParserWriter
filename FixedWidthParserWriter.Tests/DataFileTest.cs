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

            Invoice invoice = new FixedWidthFileProvider<Invoice>().Parse(fileLines);

            Assert.Equal("SoftysTech LCC", invoice.CompanyName);
            Assert.Equal(new DateTime(2018, 10, 30), invoice.Date);
            Assert.Equal("SysCompanik", invoice.BuyerName);
            Assert.Equal("0169/18", invoice.InvoiceNumber);
            Assert.Equal(1299.00m, invoice.AmountTotal);
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
                AmountTotal = 1299.00m,
                DateCreated = new DateTime(2018, 10, 31),
                SignatureName = "John Doe",
                SignatoryTitle = "Financial Manager",
            };

            var fileProvider = new FixedWidthFileProvider<Invoice>() { Content = GetDataFormTemplate() };
            fileProvider.UpdateContent(invoice);

            List<string> fileLines = GetDataFile();
            int numberOfLines = fileLines.Count;
            for (int i = 0; i < numberOfLines; i++)
            {
                Assert.Equal(fileLines[i], fileProvider.Content[i]);
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
                "                                                          1,299.00",
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
