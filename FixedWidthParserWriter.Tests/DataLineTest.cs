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
            List<InvoiceItem> invoiceItems = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLines); // StructureTypeId argument not explicity set, default = 0 (ConfigType.Alpha)

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
        public void TypedLineParserTest()
        {
            var dataLines = GetTypedDataLines();

            var actualClients = new Stack<Client>();
            var clientProvider = new FixedWidthLinesProvider<Client>();
            var invoceProvider = new FixedWidthLinesProvider<InvoiceItem>();

            for (int i = 0; i < dataLines.Count; i++)
            {
                string line = dataLines[i];
                char recordType = line[0];
                switch (recordType)
                {
                    case '1':
                        actualClients.Push(clientProvider.Parse(line));
                        break;
                    case '2':
                        actualClients.Peek().Invoices.Add(invoceProvider.Parse(line, (int)ConfigType.Omega));
                        break;
                    default:
                        throw new ArgumentException($"Invalid data type at line {i}");
                }
            }

            var expectedClients = new List<Client>
            {
                new Client() { Name = "John Mike" },
                new Client() { Name = "Miranda Klein" }
            };

            Assert.Equal(expectedClients[0].Name, actualClients.ToArray()[1].Name);
            Assert.True(actualClients.ToArray()[0].Invoices.Count == 2);

            Assert.Equal(expectedClients[1].Name, actualClients.ToArray()[0].Name);
            Assert.True(actualClients.ToArray()[1].Invoices.Count == 3);
        }

        [Theory]
        [InlineData(ConfigType.Alpha)]
        [InlineData(ConfigType.Beta)]
        public void LineWriterTest(ConfigType configType)
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            List<string> resultLines = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)configType);

            string result = string.Empty;
            foreach (var line in resultLines)
            {
                result += line + Environment.NewLine;
            }

            List<string> expectedLines = GetDataLines(configType);
            string expected = string.Empty;
            foreach (var line in expectedLines)
            {
                expected += line + Environment.NewLine;
            };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TypedLineWriterTest(ConfigType configType)
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            List<string> resultLines = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)configType);

            string result = string.Empty;
            foreach (var line in resultLines)
            {
                result += line + Environment.NewLine;
            }

            List<string> expectedLines = GetDataLines(configType);
            string expected = string.Empty;
            foreach (var line in expectedLines)
            {
                expected += line + Environment.NewLine;
            };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void LineParserNullableTest()
        {
            List<string> fileLines = GetDataLinesNullable();

            List<NullableModel> fields = new FixedWidthLinesProvider<NullableModel>().Parse(fileLines);

            List<NullableModel> expectedFields = new List<NullableModel>
            {
                new NullableModel()
                {
                    Bool = true,
                    Char = (char)"char"[0],
                    DateTime = new DateTime(2019, 1, 1),
                    Int32 = 1000,
                    Int64 = 1000,
                    Decimal = (Decimal)1000.1,
                    Double = (double)1000.1,
                    Single = (Single)1000.1
                },
                new NullableModel()
            };

            for (int i = 0; i < expectedFields.Count; i++)
            {
                Assert.Equal(expectedFields[i].Bool, fields[i].Bool);
                Assert.Equal(expectedFields[i].Char, fields[i].Char);
                Assert.Equal(expectedFields[i].Int32, fields[i].Int32);
                Assert.Equal(expectedFields[i].Int64, fields[i].Int64);
                Assert.Equal(expectedFields[i].Decimal, fields[i].Decimal);
                Assert.Equal(expectedFields[i].Single, fields[i].Single);
                Assert.Equal(expectedFields[i].Double, fields[i].Double);
                Assert.Equal(expectedFields[i].DateTime, fields[i].DateTime);
            }
        }

        [Fact]
        public void LineParserOverflowExceptionTest()
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 23456, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            Assert.Throws<OverflowException>(() => { new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)ConfigType.Gamma); });

            invoiceItems = new List<InvoiceItem>
            {
                new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m },
                new InvoiceItem() { Number = 23456, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m }
            };

            List<string> resultLines = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems, (int)ConfigType.Beta);
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

        public List<string> GetTypedDataLines()
        {
            //var header ="No |         Description         | Qty |   Price    |   Amount   |";

            List<string> dataLines = null;
            dataLines = new List<string>
                    {
                        "1John Mike                                                         ",
                        "2  1.Laptop Dell xps13                  1       821.00       821.00",
                        "2  2.Monitor Asus 32''                  2       478.00       956.00",
                        "2  3.Generic Keyboard                   1        19.00        19.00",
                        "1Miranda Klein                                                     ",
                        "2  1.Laptop HP DM4                      1       372.00       372.00",
                        "2  2.Monitor Asus 24''                  1       298.00       298.00",
                    };
            return dataLines;
        }

        public List<string> GetDataLinesNullable()
        {
            var dataLines = new List<string>
            {
                "char     1000      1000                1000.10             1000.10   1000.10   2019-01-011",
                "                                                                                          ",
            };

            return dataLines;
        }
    }
}
