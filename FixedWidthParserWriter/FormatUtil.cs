namespace FixedWidthParserWriter
{
    public class DefaultFormat
    {
        public virtual string DateTimeFormat { get; set; } = "yyyyMMdd";
        public virtual string Int32Format { get; set; } = "0";
        public virtual string DecimalFormat { get; set; } = "0.00";
        public virtual string BooleanFormat { get; set; } = "1;;0";
    }

    public class DefaultPad
    {
        public virtual char CharacterSeparator { get; set; } = ' ';
        public virtual char NumericSeparator { get; set; } = '0';
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
