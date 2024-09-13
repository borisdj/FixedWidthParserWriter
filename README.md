## FixedWidthParserWriter
Library (C# .NET) for Parsing(Reading) & Writing fixed-width/flat data files (.txt, others).  
Uses [FastMember](https://github.com/mgravell/fast-member) instead of slower Reflection.  
It is targeting NetStandard 2.0 so it can be used on project targeting NetCore(2.0+) or NetFramework(4.6.1+).

Available on [![NuGet](https://img.shields.io/nuget/v/FixedWidthParserWriter.svg)](https://www.nuget.org/packages/FixedWidthParserWriter/) latest version.  
Package manager console command for installation: *Install-Package FixedWidthParserWriter*

There are 2 main types of usage that are explained in the following segments:  
**1. LineFields**  
**2. FileFields** and  
3. *CustomFileFields*  
Both are simple to use and easily configured with Attributes.

## Contributing
If you find this project useful you can mark it by leaving a Github **\*Star**.  

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.  
[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/FixedWidthParserWriter/blob/master/LICENSE)  
Want to **Contact** us for Development & Consulting: [www.codis.tech](http://www.codis.tech)

Also take a look into others packages:  
-Open source (MIT or cFOSS) authored [.Net libraries](https://infopedia.io/dot-net-libraries/) (@**Infopedia.io** personal blog post)
| №  | .Net library             | Description                                              |
| -  | ------------------------ | -------------------------------------------------------- |
| 1  | [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) | EF Core Bulk CRUD Ops (Flagship Lib) |
| 2  | [EFCore.UtilExtensions](https://github.com/borisdj/EFCore.UtilExtensions) | EF Core Custom Annotations and AuditInfo |
| 3  | [EFCore.FluentApiToAnnotation](https://github.com/borisdj/EFCore.FluentApiToAnnotation) | Converting FluentApi configuration to Annotations |
| 4  | [FixedWidthParserWriter](https://github.com/borisdj/FixedWidthParserWriter) | Reading & Writing fixed-width/flat data files |
| 5  | [CsCodeGenerator](https://github.com/borisdj/CsCodeGenerator) | C# code generation based on Classes and elements |
| 6  | [CsCodeExample](https://github.com/borisdj/CsCodeExample) | Examples of C# code in form of a simple tutorial |

## 1. Data in LineFields
First is regular flat data file;  
**Record per Line (Fixed-Width)**, for [example](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoiceItems.txt):
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
// dataLines stripped of header
public List<Invoiceitem> ParseFieldsFromLines(new List<string> dataLines)
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
- *Start* - required for LineType so that order of lineFields does not depends on order of modelPropertis
- *Length* - when writing if Property value longer then defined in Length it is cut from the right to fit - valueTrim;  
 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; zero 0 value means entire line and negative values means start from Right side
- *Format* - Defaults per data type or group
- *Pad* - Defaults per data category: { *PadNumeric* = ' ', *PadNonNumeric* = ' ' }
- *PadSide* - Defaults per data category: { *PadSideNumeric = PadSide.Left, PadSideNonNumeric = PadSide.Right* }
- *DoTrim* - Default is 'True' when text will be trimmed before casting
- *StructureTypeId* - Default = 0, used when having multiple files with different structure or format for same data
- *NullPattern* - Default = string.Empty, Pattern used to represent a null value

*_*Format* types:  
  -`FormatNumberInteger` Default = "**0**", \*groupFormat:`Int32`,`Int64`  
  -`FormatNumberDecimal` Default = "**0.00**", \*groupFormat:`Decimal`,`Single`,`Double`  
   &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
  			 ("*0;00*" - Special custom Format that removes decimal separator: 123.45 -> 12345)</pre>  
  -`FormatBoolean` . . . . . . Default = "**1; ;0**" ("ValueForTrue; ValueForNull; ValueForFalse")  
  -`FormatDateTime`. . . . . .Default = "**yyyyMMdd**"  
 Custom format strings for [Numeric](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings) and [DateTime](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).

**FixedWidthConfig options (provides configuration for the Provider): defaultValue**
```C#
int StructureTypeId : 0	............................... // for multiple Attributes structure
Dictionary<string, FixedWidthAttribute> DynamicSettings	// Attributes defined at runtime
bool LogAndSkipErrors: false .......................... // If set True no parsing exception skipped, instead ErrorMessage logged
List<string> ErrorsLog : new List<string>(); .......... // output field, logs ErrorMessages
List<string> WarningsLog : new List<string>(); .........// output field, logged value when Writing string is cut to fit
```

*_Special feature is '**DYNAMIC Settings**' with which Attributes values can be defined at runtime, for all usage types.  
Data is forwarded using Dict with PropertyName and new independent Attribute with parameter values: `Dictionary<string, FixedWidthAttribute> dynamicSettings`.  
It can be sett for all needed Properties when having no Attributes, or just add/override some specific. And if need to exclude ones that has regular Atribute then set it with Null.  
Sample in test [LineParserTest](https://github.com/borisdj/FixedWidthParserWriter/blob/39da95cef3d8d1f4a4f8ffb72466bdaf528b500d/FixedWidthParserWriter.Tests/DataLineTest.cs).

When need more then 1 file structure/format we can put multiple Attributes per Property with different ***StructureTypeId***.  
Next example shows 2 structures, second has one less Property and different PadNumeric: '0' instead of ' '(space).  
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
var linesProvider = new FixedWidthLinesProvider<InvoiceItem>();
List<InvoiceItem> itemsA = linesProvider.Parse(dataLinesA, (int)ConfigType.Alpha);
List<InvoiceItem> itemsB = linesProvider.Parse(dataLinesB, (int)ConfigType.Alpha);
```
PARSE method for all use cases can also optionally have third parameter `List<string> errorLog` which when sent as Empty list(not null) will be loaded with list of Exceptions if any were to happen during parsing and casting operations. When method is called without this param, which remains null, in that case first error with throw Exception and procedure will be stopped.

Full Examples are in Tests of the project.

## 2. Data in FileFields
Second usage is when one data record is in different rows at defined positions;  
**Record per File (Fixed/Relative-Height)**, [E.g.](https://github.com/borisdj/FixedWidthParserWriter/blob/master/FileExamples/invoice.txt):
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

    // Line Negative - counted from bottom 
    [FixedWidthFileField(Line = -1, Length = 66, PadSide = PadSide.Left)]
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

    // Format set on class with FormatDateTime - not required on each Attribute of DateTime Property
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
