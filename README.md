## FixedWidthParserWriter
Library (C# .NET) for Parsing(Reading) & Writing fixed-width/flat data files (.txt or other).<br>
It is targeting NetStandard 2.0 so it can be used on project targeting NetCore(2.0+) or NetFramework(4.6.1+).

Available on [![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/FixedWidthParserWriter/) latest version.<br>
Package manager console command for installation: *Install-Package FixedWidthParserWriter*

There are are 2 main types of usage:<br>

### 1. Data in LineFields
First is regular flat data file (**record per Line**), for example:
```
No |         Description         |Unit| Qty |   Price    |Disc%|   Amount   |
  1.Laptop Dell xps13             Pcs       1       856.00  0.00       856.00
  2.Monitor Asus 32''             Pcs       2       568.00  0.00      1136.00
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

    [FixedWidthLineField(Start = 35, Length = 5)]
    public string Unit { get; set; }

    [FixedWidthLineField(Start = 40, Length = 6)]
    public int Quantity { get; set; }

    [FixedWidthLineField(Start = 46, Length = 13)]
    public decimal Price { get; set; }

    [FixedWidthLineField(Start = 59, Length = 6)]
    public decimal Discount { get; set; }

    [FixedWidthLineField(Start = 65, Length = 13)]
    public decimal Amount => Quantity * Price;
}
```
Then we can call it like this:
```C#
public List<Invoiceitem> ParseDataLineFields(new List<string> dataLines)
{
    var invoiceItems = new List<InvoiceItem>();
    foreach (var line in dataLines)
    {
        invoiceItems.Add(new InvoiceItem().Parse(line));
    }
    return invoiceItems;
}

public List<string> WriteDataFileFields()
{
    var invoiceItems = new List<InvoiceItem> {
        new InvoiceItem() { 
            Number = 1, Description = "Laptop Dell xps13", Unit = "Pcs", Quantity = 1, Price = 856.00m
        },
        new InvoiceItem() {
            Number = 2, Description = "Monitor Asus 32''", Unit = "Pcs", Quantity = 2, Price = 568.00m
        }
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
- *Format* (Defaults per data type: DateTime=""yyyyMMdd"", Int="", Decimal="0.00", Boolean="1;;0")
- *Pad* (Defaults per data group type: NonNumericSeparator{bool,string,DateTime}=' ', NumericSeparator='0')
- *PadSide* {Right, Left} (Defaults: NonNumeric=PadSide.Left, Numeric=PadSide.Right)
- *StructureTypeId* (used when having multiple files with different structure or format for same data)

*_*Format* types, [DateTime](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) and [Numeric](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings)(int, decimal):<br>
  -`DateTimeFormat` Default = ""yyyyMMdd"<br>
  -`Int32Format` Default = "0"<br>
  -`DecimalFormat` Default = "0.00" ("0;00" - Special custom Format that removes decimal separator: 123.45 -> 12345)<br>
  -`BooleanFormat` Default = "1;;0" ("ValueForTrue;ValueForNull;ValueForFalse")<br>

When need more then 1 file structure/format we can put multiple Attributes with different StructureId for each Property<br>
(Next example shows 2 structure with different pad(NumericSeparator: zero('0') or space(' '):
```C#
public class InvoiceItem : FixedWidthDataLine<InvoiceItem>
{
    public override void SetFormatAndPad()
    {
        Pad = InvoiceItemStructureProvider.GetDefaultPad((FormatType)StructureTypeId);
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

public static class InvoiceItemStructureProvider
{
    public static DefaultPad GetDefaultPad(FormatType formatType)
    {
        DefaultPad defaultPad = new DefaultPad();
        switch (formatType)
        {
            case FormatType.Beta:
                defaultPad = new InvoiceItemDefaultPadBeta();
                break;
        }
        return defaultPad;
    }
}

public class InvoiceItemDefaultPadBeta : DefaultPad
{
    public override char NumericSeparator { get; set; } = ' ';
}

public enum FormatType { Alpha, Beta }
```
Beta Structure:
```
No |         Description         |Unit| Qty |   Price    |Disc%|   Amount   |
0001Laptop Dell xps13             Pcs  0000010000000856.00000.000000000856.00
0002Monitor Asus 32''             Pcs  0000020000000568.00000.000000001136.00
```
Full Examples are in Tests of the project.

### 2. Data in FileFields
Second usage is when one data record is in different rows at defined positions (**record per File**), for example:
```
SoftysTech LCC
Local Street NN
______________________________________________________________________________

Invoice Date: 2018-10-30               Buyer:   SysCompanik
Due Date:     2018-11-15               Address: Some Location

                              INVOICE no. 0169/18
...                                                                           
...                                                                           
------------------------------------------------------------------------------
                                                                     1,192.00 

Date: 2018-10-31                                             Financial Manager
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

    [FixedWidthFileField(Line = 5, Start = 15, Length = 25, Format = "yyyy-MM-dd")]
    public DateTime Date { get; set; }

    [FixedWidthFileField(Line = 6, Start = 15, Length = 25, Format = "yyyy-MM-dd")]
    public DateTime DueDate { get; set; }

    [FixedWidthFileField(Line = 5, Start = 49)]
    public string BuyerName { get; set; }

    [FixedWidthFileField(Line = 6, Start = 49)]
    public string BuyerAddress { get; set; }

    [FixedWidthFileField(Line = 8, Start = 43)]
    public string InvoiceNumber { get; set; }

    [FixedWidthFileField(Line = -4, Length = 77, Pad = ' ', Format = "0,000.00")]
    public decimal AmountTotal { get; set; }

    [FixedWidthFileField(Line = -2, Start = 7, Length = 10)]
    public DateTime DateCreated { get; set; }

    [FixedWidthFileField(Line = -2, Start = 17, Length = 62, PadSide = PadSide.Left)]
    public string SignatoryTitle { get; set; }

    [FixedWidthFileField(Line = -1, Length = 78, PadSide = PadSide.Left)] // Line negative Value counts number from bottom
    public string SignatureName { get; set; }
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
DataFormTemplate looks like this:
```
{CompanyName}
{CompanyAddress}
______________________________________________________________________________

Invoice Date: {InvoiceDate}            Buyer:   {BuyerName}
Due Date:     {DueDatee}               Address: {BuyerAdd}

                              INVOICE no. NNNN/YY
...                                                                           
...                                                                           
------------------------------------------------------------------------------
                                                                         0.00 

Date: {Date}                                                  {SignatoryTitle}
                                                               {SignatureName}
```

`[FixedWidthFileField]` has additinaly parameters:
- *Line* in which we define line number where the value is (Negative values are used to define certain row from bottom)
For Fil type Here *Length* is not required, and if not set(remains 0) that means the value is entire row, trimmed.

In situation where many same type properties have Format different from default one, instead of setting that format individualy for each one, it is possible to override default format for certain data type in that class:
```C#
    public class InvoiceDefaultFormat : DefaultFormat
    {
        public override string DateTimeFormat { get; set; } = "yyyy-MM-dd";
    }
    
    public class Invoice : FixedWidthDataFile<Invoice>
    {
        public override void SetFormatAndPad()
        {
            Format = new InvoiceDefaultFormat();
        }
        
        [FixedWidthFileField(Line = 1)]
        public string CompanyName { get; set; }

        [FixedWidthFileField(Line = 2)]
        public string CompanyAddress { get; set; }

        // Format set on class with custom DefaultFormat so not required on each Attribute of DateTime Property
        [FixedWidthFileField(Line = 5, Start = 15, Length = 25/*, Format = "yyyy-MM-dd"*/)]
        public DateTime Date { get; set; }
        
        /* ... Other Properties */
    }
```

## Contributing

If you find this project useful you can mark it by leaving a Github **\*Star**.</br>

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.

[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/FixedWidthParserWriter/blob/master/LICENSE)

## Contact
Want to contact us for Hire (Development & Consulting):</br>
[www.codis.tech](http://www.codis.tech)
