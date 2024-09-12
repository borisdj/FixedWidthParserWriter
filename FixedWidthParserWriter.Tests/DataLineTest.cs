using System;
using System.Collections.Generic;
using Xunit;

namespace FixedWidthParserWriter.Tests
{
    public enum FixedWidthSettingsType
    {
        Regular = 0,
        Dynamic = 1,
    }


    public class DataLineTest
    {
        [Theory]
        [InlineData(FixedWidthSettingsType.Regular)]
        //[InlineData(FixedWidthSettingsType.Dynamic)]
        public void LineParserTest(FixedWidthSettingsType settings)
        {
            var dynamicSettings = new Dictionary<string, FixedWidthAttribute>();
            var attDescription = new FixedWidthLineFieldAttribute() { StructureTypeId = (int)ConfigType.Alpha, Start = 5, Length = 12 };
            dynamicSettings.Add(nameof(InvoiceItem.Description), attDescription);

            var dataLines = GetDataLines(ConfigType.Alpha);

            var provider = new FixedWidthLinesProvider<InvoiceItem>();

            List<InvoiceItem> invoiceItems =
                settings == FixedWidthSettingsType.Regular ? provider.Parse(dataLines) // StructureTypeId argument not explicity set, default = 0 (ConfigType.Alpha)
                                                           : provider.Parse(dataLines, dynamicSettings: dynamicSettings);

            var expectedInvoiceItems = new List<InvoiceItem>
            {
                new() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m, StatusCode = 1, ProductCode = 123},
                new() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m, StatusCode = 2, ProductCode = 125}
            };

            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(expectedInvoiceItems[i].Number, invoiceItems[i].Number);
                Assert.Equal(expectedInvoiceItems[i].Description, invoiceItems[i].Description);
                Assert.Equal(expectedInvoiceItems[i].Quantity, invoiceItems[i].Quantity);
                Assert.Equal(expectedInvoiceItems[i].Price, invoiceItems[i].Price);
                Assert.Equal(expectedInvoiceItems[i].Amount, invoiceItems[i].Amount);
                Assert.Equal(expectedInvoiceItems[i].StatusCode, invoiceItems[i].StatusCode);
            }
        }

        [Fact]
        public void LineWriterTest()
        {
            var invoiceItems = new List<InvoiceItem>
            {
                new() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 821.00m, StatusCode = 1, ProductCode = 123},
                new() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 478.00m, StatusCode = 2, ProductCode = 125}
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

        [Fact]
        public void LineParserNullableTest()
        {
            List<string> fileLines = GetDataLinesNullable();

            var fields = new FixedWidthLinesProvider<NullableModel>().Parse(fileLines);

            var expectedFields = new List<NullableModel>
            {
                new()
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
                new()
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

        public static List<string> GetDataLines(ConfigType formatType)
        {
          //var header ="No |         Description         | Qty |   Price    |   Amount   |";

            List<string> dataLines = null;
            switch (formatType)
            {
                case ConfigType.Alpha:
                    dataLines =
                    [
                        "  1.Laptop Dell xps13                  1       821.00       821.001  123",
                        "  2.Monitor Asus 32''                  2       478.00       956.002  125"
                    ];
                    break;
                case ConfigType.Beta:
                    dataLines =
                    [
                        "0001Laptop Dell xps13             0000010000000821.000000000821.00100123",
                        "0002Monitor Asus 32''             0000020000000478.000000000956.00200125"
                    ];
                    break;
            }
            return dataLines;
        }

        public static List<string> GetDataLinesNullable()
        {
            List<string> dataLines =
            [
                "char     1000      1000                1000.10             1000.10   1000.10   2019-01-011",
                "                                                                                          ",
                "ooooooooo00000000001111111111111111111100000.0000000000000000000000.00000000.000000-00-003",
            ];

            return dataLines;
        }
    }
}
