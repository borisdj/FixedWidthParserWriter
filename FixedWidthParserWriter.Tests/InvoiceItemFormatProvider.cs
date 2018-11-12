namespace FixedWidthParserWriter.Tests
{
    public enum FormatType
    {
        Alpha,
        Beta
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

        /*public static DefaultFormat GetDefaultFormat(FormatType formatType)
        {
            DefaultFormat defaultFormat = new DefaultFormat();
            switch (formatType)
            {
                case FormatType.Alpha:
                    defaultFormat = new InvoiceItemDefaultFormatAplha();
                    break;
                case FormatType.Beta:
                    defaultFormat = new InvoiceItemDefaultFormatBeta();
                    break;
            }
            return defaultFormat;
        }*/
    }

    public class InvoiceItemDefaultPadBeta : DefaultPad
    {
        public override char NumericSeparator { get; set; } = ' ';
    }

    /*public class InvoiceItemDefaultFormatBeta : DefaultFormat
    {
        public override string DateTimeFormat { get; set; } = "ddMMyy";
        public override string DecimalFormat { get; set; } = "0;00"; // ';' - Special custom Format that removes decimal separator 123.45 -> 12345
    }*/
}