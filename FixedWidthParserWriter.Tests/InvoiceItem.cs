namespace FixedWidthParserWriter.Tests
{
    public class InvoiceItem : FixedWidthDataLine<InvoiceItem>
    {
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 1, Length = 3)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta, Start = 1, Length = 4)]
        public int Number { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 4, Length = 1)]
        public string SeparatorNumDesc { get; set; } = ".";

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 5, Length = 30)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 5, Length = 30)]
        public string Description { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 35, Length = 6)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 35, Length = 6)]
        public int Quantity { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 41, Length = 13)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 41, Length = 13)]
        public decimal Price { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 54, Length = 13)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 54, Length = 13)]
        public decimal Amount => Quantity * Price;

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
    }

    public enum FormatType
    {
        Alpha,
        Beta
    }
}
