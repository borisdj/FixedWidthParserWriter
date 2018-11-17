## FixedWidthParserWriter
Library (C# .NET) for Parsing(Reading) & Writing fixed-width/flat data files (.txt or other).<br>
It is targeting NetStandard 2.0 so it can be used on project targeting NetCore(2.0+) or NetFramework(4.6.1+).

Available on [![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/FixedWidthParserWriter/) latest version.<br>
Package manager console command for installation: *Install-Package FixedWidthParserWriter*

There are 2 main types of usage that are explained in the following segments: **1. LineFields** & **2. FileFields**<br>

## Contributing
If you find this project useful you can mark it by leaving a Github **\*Star**.</br>

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.<br>
[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/EFCore.BulkExtensions/blob/master/LICENSE)

Want to **Contact** us for Hire (Development & Consulting): [www.codis.tech](http://www.codis.tech)

### 1. Data in LineFields
First is regular flat data file (**record per Line**), for [example](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceItems.txt):
```
No |         Description         | Qty |   Price    |   Amount   |
  1.Laptop Dell xps13                  1       856.00       856.00
  2.Monitor Asus 32''                  2       568.00      1136.00
```
For parsing/writing we make a model that inherits `FixedWidthDataLine` which Properties have `[FixedWidthLineField]` Att:
```C#
public class InvoiceItem : FixedWidthDataLine<InvoiceItem>
{
    [FixedWidthLineField(Start = 1, Length = 3)]
    public int Number { get; set; }

    [FixedWidthLineField(Start = 4, Length = 1)]
    public string SeparatorNumDesc { get; set; } = ".";

    [FixedWidthLineField(Start = 5, Length = 30)]
    public string Description { get; set; }

    [FixedWidthLineField(Start = 35, Length = 6)]
    public int Quantity { get; set; }

    [FixedWidthLineField(Start = 41, Length = 13)]
    public decimal Price { get; set; }

    [FixedWidthLineField(Start = 54, Length = 13)]
    public decimal Amount => Quantity * Price;
}
```
Then we can call it like this:
```C#
public List<Invoiceitem> ParseDataLineFields(new List<string> dataLines) // dataLines are stripped of header
{
    var invoiceItems = new List<InvoiceItem>();
    foreach (var line in dataLines)
    {
        invoiceItems.Add(new InvoiceItem().Parse(line));
    }
    return invoiceItems;
}

public List<string> WriteDataLineFields()
{
    var invoiceItems = new List<InvoiceItem> {
        new InvoiceItem() { Number = 1, Description = "Laptop Dell xps13", Quantity = 1, Price = 856.00m },
        new InvoiceItem() { Number = 2, Description = "Monitor Asus 32''", Quantity = 2, Price = 568.00m }
    };

    string itemsLineData = string.Empty;
    foreach (var item in invoiceItems)
    {
        itemsLineData += item.ToString() + Environment.NewLine;
    }
    return itemsLineData;
}
```
`[FixedWidthLineField]` has following parameters that can be configured for each Property:
- *Start*
- *Length*
- *Format* (Defaults per data type or group)
- *Pad* (Defaults per data category type: ' ')
- *PadSide* {Right, Left} (Defaults per data category: NonNumeric = PadSide.Left, Numeric = PadSide.Right)
- *StructureTypeId* (used when having multiple files with different structure or format for same data)

*_*Format* types, [DateTime](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) and [Numeric](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings):<br>
  -`FormatIntegerNumber` Default = "0", \*groupFormat:`Int32`,`Int64`<br>
  -`FormatDecimal` Default = "0.00", \*groupFormat:`Decimal`,`Single`,`Double`<br>
                   ("0;00" - Special custom Format that removes decimal separator: 123.45 -> 12345)<br>
  -`FormatBoolean` Default = "1;;0" ("ValueForTrue;ValueForNull;ValueForFalse")<br>
  -`FormatDateTime` Default = ""yyyyMMdd"<br>
  
When need more then 1 file structure/format we can put multiple Attributes with different StructureId for each Property<br>
(Next example shows 2 structure with different pad(NumericSeparator: zero('0') or space(' '):
```C#
public class InvoiceItem : FixedWidthDataLine<InvoiceItem>
{
	public override void SetDefaultConfig()
	{
		switch ((FormatType)StructureTypeId)
		{
			case FormatType.Alpha:
				// config remains initial default
				break;
			case FormatType.Beta:
				DefaultConfig.PadSeparatorNumeric = '0';
				break;
		}
	}
	
	[FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 1, Length = 4)]
	[FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 1, Length = 3)]
	public int Number { get; set; }

	[FixedWidthLineField(StructureTypeId = (int)FormatType.Beta, Start = 4, Length = 1)]
	public string SeparatorNumDesc { get; set; } = ".";

	[FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 5, Length = 30)]
	[FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 5, Length = 30)]
	public string Description { get; set; }

	//... Others Properties
}

public enum FormatType { Alpha, Beta }
```
Beta Structure:
```
No |         Description         | Qty |   Price    |   Amount   |
0001Laptop Dell xps13             0000010000000856.000000000856.00
0002Monitor Asus 32''             0000020000000568.000000001136.00
```
Full Examples are in Tests of the project.

### 2. Data in FileFields
Second usage is when one data record is in different rows at defined positions (**record per File**), for [example](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoice.txt):
```
SoftysTech LCC
Local Street NN
__________________________________________________________________

Invoice Date: 2018-10-30         Buyer:   SysCompanik
Due Date:     2018-11-15         Address: Some Location

                        INVOICE no. 0169/18
						
No |         Description         | Qty |   Price    |   Amount   |
...
...
------------------------------------------------------------------
                                                          1,192.00 

Date: 2018-10-31                                 Financial Manager
                                                          John Doe
```
For parsing/writing we make a model that inherits `FixedWidthDataFile` which Properties have `[FixedWidthLineField]` Att:
```C#
public class Invoice : FixedWidthDataFile<Invoice>
{
    [FixedWidthFileField(Line = 1)]
    public string CompanyName { get; set; }

    [FixedWidthFileField(Line = 2)]
    public string CompanyAddress { get; set; }

    [FixedWidthFileField(Line = 5, Start = 15, Length = 19, Format = "yyyy-MM-dd")]
    public DateTime Date { get; set; }

    [FixedWidthFileField(Line = 6, Start = 15, Length = 19, Format = "yyyy-MM-dd")]
    public DateTime DueDate { get; set; }

    [FixedWidthFileField(Line = 5, Start = 43)]
    public string BuyerName { get; set; }

    [FixedWidthFileField(Line = 6, Start = 43)]
    public string BuyerAddress { get; set; }

    [FixedWidthFileField(Line = 8, Start = 37)]
    public string InvoiceNumber { get; set; }

    [FixedWidthFileField(Line = -4, Length = 66, Pad = ' ', Format = "0,000.00")]
    public decimal AmountTotal { get; set; }

    [FixedWidthFileField(Line = -2, Start = 7, Length = 10)]
    public DateTime DateCreated { get; set; }

    [FixedWidthFileField(Line = -2, Start = 17, Length = 50, PadSide = PadSide.Left)]
    public string SignatoryTitle { get; set; }

    [FixedWidthFileField(Line = -1, Length = 66, PadSide = PadSide.Left)] // When Line is negative Value it counts from bottom
    public string SignatureName { get; set; }
    
    public override void SetDefaultConfig()
    {
        DefaultConfig.FormatDateTime = "yyyy-MM-dd";
    }
}
```
Usage:
```C#
public Invoice ParseDataFileFields(new List<string> fileLines)
{
    var invoice = new Invoice().Parse(fileLines);
    return invoice;
}

public List<string> WriteDataLineFields()
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
    return invoice.Content;
}
```
[DataFormTemplate](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceTemplate.txt) looks like this:
```
{CompanyName}
{CompanyAddress}
__________________________________________________________________

Invoice Date: {InvoiceDate}      Buyer:   {BuyerName}
Due Date:     {DueDatee}         Address: {BuyerAdd}

                        INVOICE no. NNNN/YY
						
No |         Description         | Qty |   Price    |   Amount   |
...
...
------------------------------------------------------------------
                                                              0.00 

Date: {Date}                                      {SignatoryTitle}
                                                   {SignatureName}
```

`[FixedWidthFileField]` has additional parameter:
- *Line* in which we define line number where the value is (Negative values are used to define certain row from bottom)<br>

For File type *Length* is not required, if not set(remains 0) it means the value is entire row(trimmed), and *Start* has default = 1.

In situation where many same type properties have Format different from default one, instead of setting that format individually for each one, it is possible to override default format for certain data type in that class:
```C#
    public class Invoice : FixedWidthDataFile<Invoice>
    {
        public override void SetDefaultConfig()
        {
            DefaultConfig.FormatDateTime = "yyyy-MM-dd";
        }
        
        [FixedWidthFileField(Line = 1)]
        public string CompanyName { get; set; }

        [FixedWidthFileField(Line = 2)]
        public string CompanyAddress { get; set; }

        // Format set on class with custom DefaultFormat so not required on each Attribute of DateTime Property
        [FixedWidthFileField(Line = 5, Start = 15, Length = 19/*, Format = "yyyy-MM-dd"*/)]
        public DateTime Date { get; set; }
        
        /* ... Other Properties */
    }
```

Combining both previous usages we can make complex file structures like [invoiceFull](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceFull.txt).
