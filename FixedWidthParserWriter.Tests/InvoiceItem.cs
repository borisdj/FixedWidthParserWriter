using System;

namespace FixedWidthParserWriter.Tests
{
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

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 35, Length = 6)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 35, Length = 6)]
        public int Quantity { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 41, Length = 13)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 41, Length = 13)]
        public decimal Price { get; set; }

        [FixedWidthLineField(StructureTypeId = (int)FormatType.Alpha, Start = 54, Length = 13)]
        [FixedWidthLineField(StructureTypeId = (int)FormatType.Beta,  Start = 54, Length = 13)]
        public decimal Amount => Quantity * Price;
    }
}
