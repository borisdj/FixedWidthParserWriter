using System;
using System.Collections.Generic;
using Xunit;

namespace FixedWidthParserWriter.Tests
{
    public class DataLineTest
    {
        [Fact]
        public void LineParserTest()
        {
            var dataLines = GetDataLines(ConfigType.Alpha);
            List<InvoiceItem> invoiceItems = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLines); // StrucutreTypeId argument not explicity set, default = 0 (ConfigType.Alpha)

            var expectedInvoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(expectedInvoiceItems[i].Number, invoiceItems[i].Number);
                Assert.Equal(expectedInvoiceItems[i].Description, invoiceItems[i].Description);
                Assert.Equal(expectedInvoiceItems[i].Quantity, invoiceItems[i].Quantity);
                Assert.Equal(expectedInvoiceItems[i].Price, invoiceItems[i].Price);
                Assert.Equal(expectedInvoiceItems[i].Amount, invoiceItems[i].Amount);
            }
        }

        [Fact]
        public void LineWriterTest()
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            List<string> resultLinesAlpha = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)ConfigType.Alpha);
            List<string> resultLinesBeta = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)ConfigType.Beta);

            string resultAlpha = string.Empty;
            foreach (var line in resultLinesAlpha)
            {
                resultAlpha += line + Environment.NewLine;
            }

            string resultBeta = string.Empty;
            foreach (var line in resultLinesBeta)
            {
                resultBeta += line + Environment.NewLine;
            }

            List<string> expectedLinesAlpha = GetDataLines(ConfigType.Alpha);
            string expectedAlpha = string.Empty;
            foreach (var line in expectedLinesAlpha)
            {
                expectedAlpha += line + Environment.NewLine;
            };

            List<string> expectedLinesBeta = GetDataLines(ConfigType.Beta);
            string expectedBeta = string.Empty;
            foreach (var line in expectedLinesBeta)
            {
                expectedBeta += line + Environment.NewLine;
            };

            Assert.Equal(expectedAlpha, resultAlpha);
            Assert.Equal(expectedBeta, resultBeta);
        }

        public List<string> GetDataLines(ConfigType formatType)
        {
          //var header ="No |         Description         | Qty |   Price    |   Amount   |";

            List<string> dataLines = null;
            switch (formatType)
            {
                case ConfigType.Alpha:
                    dataLines = new List<string>
                    {
                        "  1.Laptop Dell xps13                  1       821.00       821.00",
                        "  2.Monitor Asus 32''                  2       478.00       956.00"
                    };
                    break;
                case ConfigType.Beta:
                    dataLines = new List<string>
                    {
                        "0001Laptop Dell xps13             0000010000000821.000000000821.00",
                        "0002Monitor Asus 32''             0000020000000478.000000000956.00"
                    };
                    break;
            }
            return dataLines;
        }
    }
}
