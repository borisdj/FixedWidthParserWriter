namespace FixedWidthParserWriter
{
    public class DefaultConfig
    {
        // Formats
        public virtual string FormatNumberInteger { get; set; } = "0";
        public virtual string FormatNumberDecimal { get; set; } = "0.00";
        public virtual string FormatBoolean { get; set; } = "1; ;0";
        public virtual string FormatDateTime { get; set; } = "yyyyMMdd";

        //public virtual string FormatInt32 { get; set; }   // FormatNumberInteger
        //public virtual string FormatInt64 { get; set; }   // FormatNumberInteger
        //public virtual string FormatDecimal { get; set; } // FormatNumberDecimal
        //public virtual string FormatSingle { get; set; }  // FormatNumberDecimal
        //public virtual string FormatDouble { get; set; }  // FormatNumberDecimal

        // Pads
        public virtual char PadNumeric { get; set; } = ' ';
        public virtual char PadNonNumeric { get; set; } = ' ';

        public virtual PadSide PadSideNumeric { get; set; } = PadSide.Left;
        public virtual PadSide PadSideNonNumeric { get; set; } = PadSide.Right;
    }

    public enum PadSide
    {
        Default,
        Right,
        Left
    }

    public enum FieldType
    {
        LineField,
        FileField
    }
}
