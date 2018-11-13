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
            Assert.Equal("Local Street NN", invoice.CompanyAddress);
            Assert.Equal(new DateTime(2018, 10, 30), invoice.Date);
            Assert.Equal(new DateTime(2018, 11, 15), invoice.DueDate);
            Assert.Equal("SysCompanik", invoice.BuyerName);
            Assert.Equal("Some Location", invoice.BuyerAddress);
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
                CompanyAddress = "Local Street NN",
                Date = new DateTime(2018, 10, 30),
                DueDate = new DateTime(2018, 11, 15),
                BuyerName = "SysCompanik",
                BuyerAddress = "Some Location",
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
                "Local Street NN",
                "______________________________________________________________________________",
                "",
                "Invoice Date: 2018-10-30               Buyer:   SysCompanik",
                "Due Date:     2018-11-15               Address: Some Location",
                "",
                "                              INVOICE no. 0169/18",
                "...                                                                           ",
                "...                                                                           ",
                "------------------------------------------------------------------------------",
                "                                                                     1,192.00 ",
                "",
                "Date: 2018-10-31                                             Financial Manager",
                "                                                                      John Doe"
            };
            return dataLines;
        }

        public List<string> GetDataFormTemplate()
        {
            var dataLines = new List<string>
            {
                "{CompanyName}",
                "{CompanyAddress}",
                "______________________________________________________________________________",
                "",
                "Invoice Date: {InvoiceDate}            Buyer:   {BuyerName}",
                "Due Date:     {DueDatee}               Address: {BuyerAdd}",
                "",
                "                              INVOICE no. NNNN/YY",
                "...                                                                           ",
                "...                                                                           ",
                "------------------------------------------------------------------------------",
                "                                                                         0.00 ",
                "",
                "Date: {Date}                                                  {SignatoryTitle}",
                "                                                               {SignatureName}"
            };
            return dataLines;
        }
    }
}
