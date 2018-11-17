namespace FixedWidthParserWriter
{
    public static class TypeText
    {
        public static string Int32 => nameof(System.Int32);
        public static string Int64 => nameof(System.Int64);
        public static string Decimal => nameof(System.Decimal);
        public static string Single => nameof(System.Single);
        public static string Double => nameof(System.Double);
        public static string Boolean => nameof(System.Decimal);
        public static string DateTime => nameof(System.DateTime);
        public static string String => nameof(System.DateTime);
        public static string Char => nameof(System.DateTime);
    }

    public class DefaultConfig
    {
        // Formats
        public virtual string FormatNumberInteger { get; set; } = "0";
        public virtual string FormatNumberDecimal { get; set; } = "0.00";
        public virtual string FormatBoolean { get; set; } = "1;;0";
        public virtual string FormatDateTime { get; set; } = "yyyyMMdd";

        //public virtual string FormatInt32 { get; set; }   // FormatNumberInteger
        //public virtual string FormatInt64 { get; set; }   // FormatNumberInteger
        //public virtual string FormatDecimal { get; set; } // FormatNumberDecimal
        //public virtual string FormatSingle { get; set; }  // FormatNumberDecimal
        //public virtual string FormatDouble { get; set; }  // FormatNumberDecimal

        // Pads
        public virtual char PadSeparatorNumeric { get; set; } = ' ';
        public virtual char PadSeparatorNonNumeric { get; set; } = ' ';

        public virtual PadSide PadSideNumeric { get; set; } = PadSide.Left;
        public virtual PadSide PadSideNonNumeric { get; set; } = PadSide.Right;
    }

    public enum FieldType
    {
        LineField,
        FileField
    }

    public enum PadSide
    {
        Default,
        Right,
        Left
    }
}
