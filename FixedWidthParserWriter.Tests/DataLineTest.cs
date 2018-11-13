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
            var dataLines = GetDataLines(FormatType.Alpha);

            var invoiceItems = new List<InvoiceItem>();
            foreach (var line in dataLines)
            {
                invoiceItems.Add(new InvoiceItem().Parse(line));
            }

            var invoiceItem = new InvoiceItem()
            {
                Number = 1,
                Description = "Laptop Dell xps13",
                Unit = "Pcs", // Pcs - pieces
                Quantity = 1,
                Price = 856.00m
            };

            Assert.Equal(invoiceItem.Number, invoiceItems[0].Number);
            Assert.Equal(invoiceItem.Description, invoiceItems[0].Description);
            Assert.Equal(invoiceItem.Unit, invoiceItems[0].Unit);
            Assert.Equal(invoiceItem.Quantity, invoiceItems[0].Quantity);
            Assert.Equal(invoiceItem.Price, invoiceItems[0].Price);

        }

        [Fact]
        public void LineWriterTest()
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem()
                {
                    Number = 1,
                    Description = "Laptop Dell xps13",
                    Unit = "Pcs", // Pcs - pieces
                    Quantity = 1,
                    Price = 856.00m
                },
                new InvoiceItem()
                {
                    Number = 2,
                    Description = "Monitor Asus 32''",
                    Unit = "Pcs",
                    Quantity = 2,
                    Price = 568.00m
                }
            };

            string resultAlpha = string.Empty;
            string resultBeta = string.Empty;
            foreach (var item in invoiceItems)
            {
                resultAlpha += item.ToString() + Environment.NewLine;
                resultBeta += item.ToString((int)FormatType.Beta) + Environment.NewLine;
            }

            List<string> dataLines;

            dataLines = GetDataLines(FormatType.Alpha);
            string expectedAlpha = string.Empty;
            foreach (var line in dataLines)
            {
                expectedAlpha += line + Environment.NewLine;
            };

            dataLines = GetDataLines(FormatType.Beta);
            string expectedBeta = string.Empty;
            foreach (var line in dataLines)
            {
                expectedBeta += line + Environment.NewLine;
            };

            Assert.Equal(expectedAlpha, resultAlpha);
            Assert.Equal(expectedBeta, resultBeta);
        }

        public List<string> GetDataLines(FormatType formatType)
        {
            //var head = "No |         Description         |Unit| Qty |   Price    |Disc%|   Amount   |";

            List<string> dataLines = null;
            switch (formatType)
            {
                case FormatType.Alpha:
                    dataLines = new List<string>
                    {
                        "0001Laptop Dell xps13             Pcs  0000010000000856.00000.000000000856.00",
                        "0002Monitor Asus 32''             Pcs  0000020000000568.00000.000000001136.00"
                    };
                    break;
                case FormatType.Beta:
                    dataLines = new List<string>
                    {
                        "  1.Laptop Dell xps13             Pcs       1       856.00  0.00       856.00",
                        "  2.Monitor Asus 32''             Pcs       2       568.00  0.00      1136.00"
                    };
                    break;
            }
            return dataLines;
        }
    }
}
