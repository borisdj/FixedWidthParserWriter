## FixedWidthParserWriter
Library (C# .NET) for Parsing(Reading) & Writing fixed-width/flat data files (.txt or other).<br>
It is targeting NetStandard 2.0 so it can be used on project targeting NetCore(2.0+) or NetFramework(4.6.1+).

Available on [![NuGet](https://img.shields.io/badge/NuGet-1.0.1-blue.svg)](https://www.nuget.org/packages/FixedWidthParserWriter/) latest version.<br>
Package manager console command for installation: *Install-Package FixedWidthParserWriter*

There are 2 main types of usage that are explained in the following segments: **1. LineFields** & **2. FileFields**<br>

## Contributing
If you find this project useful you can mark it by leaving a Github **\*Star**.</br>

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.<br>
[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/FixedWidthParserWriter/blob/master/LICENSE)

Want to **Contact** us for Hire (Development & Consulting): [www.codis.tech](http://www.codis.tech)

### 1. Data in LineFields
First is regular flat data file, **record per Line (Fixed-Width)**, for [example](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceItems.txt):
```
No |         Description         | Qty |   Price    |   Amount   |
  1.Laptop Dell xps13                  1       821.00       821.00
  2.Monitor Asus 32''                  2       478.00       956.00
```
For parsing/writing we make a model which Properties have `[FixedWidthLineField]` Attribute:
```C#
public class InvoiceItem
{
    [FixedWidthLineField(Start = 1, Length = 3)]
    public int Number { get; set; }

    [FixedWidthLineField(Start = 4, Length = 1)]
    public string NumberedBullet { get; set; } = ".";

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
public List<Invoiceitem> ParseFieldsFromLines(new List<string> dataLines) // dataLines are stripped of header
{
    List<InvoiceItem> invoiceItems = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLines);
    return invoiceItems;
}

public List<string> WriteFieldsToLines(List<InvoiceItem> invoiceItems)
{
    List<string> dataLines = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems);
    return dataLines;
}
```
`[FixedWidthLineField]` has following parameters that can be configured for each Property:
- *Start*
- *Length* (when writing if Property has longer value then defined in Length it will be cut from the right to fit)
- *Format* (Defaults per data type or group)
- *Pad* (Defaults per data category{*Numeric*, *NonNumeric*} type, initially: ' ')
- *PadSide* {Right, Left} (Defaults per data category: *NonNumeric = PadSide.Left, Numeric = PadSide.Right*)
- *StructureTypeId* (Default = 0, used when having multiple files with different structure or format for same data)

*_*Format* types:<br>
  -`FormatIntegerNumber` Default = "**0**", \*groupFormat:`Int32`,`Int64`<br>
  -`FormatDecimalNumber` Default = "**0.00**", \*groupFormat:`Decimal`,`Single`,`Double`<br>
   &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
  			 ("*0;00*" - Special custom Format that removes decimal separator: 123.45 -> 12345)</pre><br>
  -`FormatBoolean` . . . . . . Default = "**1; ;0**" ("ValueForTrue;ValueForNull;ValueForFalse")<br>
  -`FormatDateTime`. . . . . .Default = "**yyyyMMdd**"<br>
 Custom format strings for [Numeric](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings) and [DateTime](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).

When need more then 1 file structure/format we can put multiple Attributes per Property with different *StructureTypeId*.<br>
Next example shows 2 structures, second has one less Property and different PadSeparatorNumeric: '0' instead of ' '(space).<br>
To change `DefaultConfig` per StructureType, model should implement `IFixedWidth` interface with `SetDefaultConfig()` func.
```C#
public enum ConfigType { Alpha, Beta }

public class InvoiceItem : IFixedWidth
{
	public DefaultConfig GetDefaultConfig(int StructureTypeId)
	{
		var defaultConfig = new DefaultConfig();
		switch ((ConfigType)StructureTypeId)
		{
			case ConfigType.Alpha:
				// config remains initial default
				break;
			case ConfigType.Beta:
				defaultConfig.PadSeparatorNumeric = '0';
				break;
		}
		return defaultConfig;
	}

	[FixedWidthLineField(StructureTypeId = (int)ConfigType.Alpha, Start = 1, Length = 3)]
	[FixedWidthLineField(StructureTypeId = (int)ConfigType.Beta,  Start = 1, Length = 4)]
	public int Number { get; set; }

	[FixedWidthLineField(StructureTypeId = (int)ConfigType.Alpha, Start = 4, Length = 1)]
	public string SeparatorNumDesc { get; set; } = ".";

	[FixedWidthLineField(StructureTypeId = (int)ConfigType.Alpha, Start = 5, Length = 30)]
	[FixedWidthLineField(StructureTypeId = (int)ConfigType.Beta,  Start = 5, Length = 30)]
	public string Description { get; set; }

	//... Others Properties
}
```
Beta Structure:
```
No |         Description         | Qty |   Price    |   Amount   |
0001Laptop Dell xps13             0000010000000821.000000000821.00
0002Monitor Asus 32''             0000020000000478.000000000956.00
```
Full Examples are in Tests of the project.

### 2. Data in FileFields
Second usage is when one data record is in different rows at defined positions, **record per File (Fixed/Relative-Height)**, [E.g.](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoice.txt):
```
SoftysTech LCC
__________________________________________________________________

Invoice Date: 2018-10-30         Buyer:   SysCompanik

                        INVOICE no. 0169/18
						
No |         Description         | Qty |   Price    |   Amount   |
...
...
------------------------------------------------------------------
                                                          1,299.00 

Date: 2018-10-31                                 Financial Manager
                                                          John Doe
```
For parsing/writing `[FixedWidthFileField]` attributes are used, that have additional parameter:
- *Line* in which we define line number where the value is (Negative values represent row number from bottom)

For type FileField param. *Length* not required, if not set means value goes till end of row(trimmed), and *Start* has default = 1.
```C#
public class Invoice
{
    [FixedWidthFileField(Line = 1)]
    public string CompanyName { get; set; }

    [FixedWidthFileField(Line = 4, Start = 15, Length = 19, Format = "yyyy-MM-dd")]
    public DateTime Date { get; set; }

    [FixedWidthFileField(Line = 4, Start = 43)]
    public string BuyerName { get; set; }

    [FixedWidthFileField(Line = 6, Start = 37)]
    public string InvoiceNumber { get; set; }

    [FixedWidthFileField(Line = -4, Length = 66, Format = "0,000.00")]
    public decimal AmountTotal { get; set; }

    [FixedWidthFileField(Line = -2, Start = 7, Length = 10, Format = "yyyy-MM-dd")]
    public DateTime DateCreated { get; set; }

    [FixedWidthFileField(Line = -2, Start = 17, Length = 50, PadSide = PadSide.Left)]
    public string SignatoryTitle { get; set; }

    [FixedWidthFileField(Line = -1, Length = 66, PadSide = PadSide.Left)] // Line Negative - counted from bottom 
    public string SignatureName { get; set; }
}
```
Usage:
```C#
public Invoice ParseFieldsFromFile(new List<string> fileLines)
{
	invoice invoice = new FixedWidthFileProvider<Invoice>().Parse(fileLines);
    return invoice;
}

public List<string> WriteFieldsToFile(Invoice)
{
	List<string> templateLines = GetDataFormTemplate();
	var fileProvider = new FixedWidthFileProvider<Invoice>() { Content = templateLines };
    fileProvider.UpdateContent(invoice);
    invoice.UpdateContent();
    return invoice.Content;
}
```
[DataFormTemplate](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceTemplate.txt) looks like this:
```
{CompanyName}
__________________________________________________________________

Invoice Date: {InvoiceDate}      Buyer:   {BuyerName}

                        INVOICE no. NNNN/YY
						
No |         Description         | Qty |   Price    |   Amount   |
...
...
------------------------------------------------------------------
                                                              0.00 

Date: {DateCreated}                               {SignatoryTitle}
                                                   {SignatureName}
```
In situation where many same type properties have Format different from default one, instead of setting custom format individually for each Property, it is possible to override DefaultConfig for certain data types/groups in that class:
```C#
    public class Invoice : IFixedWidth
    {
        public DefaultConfig GetDefaultConfig(int StructureTypeId)
        {
            return new DefaultConfig
            {
                FormatDateTime = "yyyy-MM-dd"
            };
        }
        
        [FixedWidthFileField(Line = 1)]
        public string CompanyName { get; set; }

        // Format set on class with FormatDateTime so not required on each Attribute of DateTime Property
        [FixedWidthFileField(Line = 4, Start = 15, Length = 19/*, Format = "yyyy-MM-dd"*/)]
        public DateTime Date { get; set; }
        
        /* ... Other Properties */
    }
```
If we need to changed DefaultConfig(Format) for multiple models then we could override entire Provider to keep it [DRY](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself).

Combining both previous usages we can make complex file structures like [invoiceFull](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceFull.txt).
