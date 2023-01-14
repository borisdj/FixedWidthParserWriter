## FixedWidthParserWriter
Library (C# .NET) for Parsing(Reading) & Writing fixed-width/flat data files (.txt, others). Uses [FastMember](https://github.com/mgravell/fast-member) instead of slower Reflection.<br>
It is targeting NetStandard 2.0 so it can be used on project targeting NetCore(2.0+) or NetFramework(4.6.1+).

Available on [![NuGet](https://img.shields.io/nuget/v/FixedWidthParserWriter.svg)](https://www.nuget.org/packages/FixedWidthParserWriter/) latest version.<br>
Package manager console command for installation: *Install-Package FixedWidthParserWriter*

There are 2 main types of usage that are explained in the following segments: **1. LineFields** & **2. FileFields**<br>
Both are simple to use and easily configured with Attributes.

## Contributing
If you find this project useful you can mark it by leaving a Github **\*Star**.</br>

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.<br>
[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/FixedWidthParserWriter/blob/master/LICENSE)<br>
Want to **Contact** us for Hire (Development & Consulting): [www.codis.tech](http://www.codis.tech)

## 1. Data in LineFields
### 1.1 Regular flat data file
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
public List<Invoiceitem> Parse(new List<string> dataLines) 
// dataLines are stripped of header
{
    List<InvoiceItem> invoiceItems = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLines);
    return invoiceItems;
}

public List<string> Write(List<InvoiceItem> invoiceItems)
{
    List<string> dataLines = new FixedWidthLinesProvider<InvoiceItem>().Write(invoiceItems);
    return dataLines;
}
```
### 1.2 Mixed types data file
When you are dealing with a data file with mixed types, **record per Line (Fixed-Width)**, for [example](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/clientInvoiceItems.txt):
```
1|Client Name
2|No |         Description         | Qty |   Price    |   Amount   |

1John Mike                                                         
2  1.Laptop Dell xps13                  1       821.00       821.00
2  2.Monitor Asus 32''                  2       478.00       956.00
2  3.Generic Keyboard                   1        19.00        19.00
1Miranda Klein                                                     
2  1.Laptop HP DM4                      1       372.00       372.00
2  2.Monitor Asus 24''                  1       298.00       298.00
```
For parsing/writing we make a model which Properties have `[FixedWidthLineField]` Attribute:
```C#
class Client
{
    public List<InvoiceItem> Invoices { get; internal set; }
    
    [FixedWidthLineField(Start = 2, Length = 30)]
    public string Name { get; internal set; }

    public Client()
    {
        Invoices = new List<InvoiceItem>();
    }
}

public class InvoiceItems
{
    [FixedWidthLineField(Start = 2, Length = 3)]
    public int Number { get; set; }

    [FixedWidthLineField(Start = 4, Length = 1)]
    public string NumberedBullet { get; set; } = ".";

    [FixedWidthLineField(Start = 6, Length = 30)]
    public string Description { get; set; }

    [FixedWidthLineField(Start = 36, Length = 6)]
    public int Quantity { get; set; }

    [FixedWidthLineField(Start = 42, Length = 13)]
    public decimal Price { get; set; }

    [FixedWidthLineField(Start = 55, Length = 13)]
    public decimal Amount => Quantity * Price;
}
```
Then we can loop through lines and parse them by type, like this:
```C#
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
```
### 1.3 Attribute
`[FixedWidthLineField]` has following parameters that can be configured for each Property:
- *Start* - required for LineType so that order of lineFields does not depends on order of modelPropertis
- *Length* - when writing if Property has longer value then defined in Length it will be cut from the right to fit - valueTrim</br>
 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; negative values means from Right side
- *Format* - Defaults per data type or group
- *Pad* - Defaults per data category: { *PadNumeric* = ' ', *PadNonNumeric* = ' ' }
- *PadSide* - Defaults per data category: { *PadSideNumeric = PadSide.Left, PadSideNonNumeric = PadSide.Right* }
- *DoTrim* - Default is 'True' when text will be trimmed before casting
- *StructureTypeId* - Default = 0, used when having multiple files with different structure or format for same data
- *NullPattern* - Default = string.Empty, Pattern used to represent a null value
- *ExceptionOnOverflow* - Default = false, used when an exception is desired when the field value is bigger than it's Length attribute

*_*Format* types:<br>
  -`FormatIntegerNumber` Default = "**0**", \*groupFormat:`Int32`,`Int64`<br>
  -`FormatDecimalNumber` Default = "**0.00**", \*groupFormat:`Decimal`,`Single`,`Double`<br>
   &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
  			 ("*0;00*" - Special custom Format that removes decimal separator: 123.45 -> 12345)</pre><br>
  -`FormatBoolean` . . . . . . Default = "**1; ;0**" ("ValueForTrue;ValueForNull;ValueForFalse")<br>
  -`FormatDateTime`. . . . . .Default = "**yyyyMMdd**"<br>
 Custom format strings for [Numeric](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings) and [DateTime](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).

When need more then 1 file structure/format we can put multiple Attributes per Property with different *StructureTypeId*.<br>
Next example shows 2 structures, second has one less Property and different PadNumeric: '0' instead of ' '(space).<br>
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
                defaultConfig.PadNumeric = '0';
                break;
        }
        return defaultConfig;
    }

    [FixedWidthLineField(StructureTypeId = (int)ConfigType.Alpha, Start = 1, Length = 3)]
    [FixedWidthLineField(StructureTypeId = (int)ConfigType.Beta,  Start = 1, Length = 4)]
    public int Number { get; set; }

    [FixedWidthLineField(StructureTypeId = (int)ConfigType.Alpha, Start = 4, Length = 1)]
    public string NumberedBullet { get; set; } = ".";

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

Calling the methods:
```
List<InvoiceItem> itemsA = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLinesA, (int)ConfigType.Alpha);
List<InvoiceItem> itemsB = new FixedWidthLinesProvider<InvoiceItem>().Parse(dataLinesB, (int)ConfigType.Alpha);
```
`Parse` PARSE method for all use cases can also optionally have third parameter `List<string> errorLog` which when sent as Empty list(not null) will be loaded with list of Exceptions if any were to happen during parsing and casting operations. When method is called without this param, which remains null, in that case first error with throw Exception and procedure will be stopped.

Full Examples are in Tests of the project.

## 2. Data in FileFields
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
- *Line* - in which we define line number where the value is (Negative values represent row number from bottom)

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

When there is some special situation where some Fields can not be configured with existing options, then we can make additional custom parsing prior or/and after calling the method - PRE & POST Processing.

## 3. Data in CustomFileFields
Third use case is when one data field is relative to some text position.
```
SoftysTech LCC company
__________________________________________________________________

Date generated: 30.06.2020.

                       Q REPORT no. 11/20**  
1569ccACTN.162-677-169-796
...
Revenue:
1,234.55
...
xx
```

```C#
public class Report
{
  [CustomFileField(EndsWith = " company")]
  public string CompanyName { get; set; }

  [CustomFileField(StartsWith = "Date generated: ", Format = "d.M.yyyy.")]
  public DateTime Date { get; set; }

  [CustomFileField(Contains = "Q REPORT no. ", RemoveText = "**")]
  public string Number { get; set; }

  [CustomFileField(Contains = "ACTN.", Length = -15)]
  public string Account { get; set; }

  [CustomFileField(StartsWith = "Revenue:", Offset = 1)]
  public decimal Revenue { get; set; }
}
```

For parsing [CustomFileField] attributes are used, with additional params:

- *StartsWith*, *EndsWith*, *Contains* - finds lines with first occurance of search criteria
- *Offset* - moves found line up(is positive) or down(negative value) for defined number of rows
- *RemoveText* - to clear custom substring from text value before additional casting
- *RemoveStartsWith*, *RemoveEndsWith*, *RemoveContains* - defaults are 'True' meaning that search string is also cleared
