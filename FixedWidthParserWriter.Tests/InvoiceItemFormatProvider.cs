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
            DefaultPad defaultPad = null;
            switch (formatType)
            {
                case FormatType.Alpha:
                    defaultPad = new DefaultPad();
                    break;
                case FormatType.Beta:
                    defaultPad = new DefaultPad()
                    {
                        NumericSeparator = ' '
                    };
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
}