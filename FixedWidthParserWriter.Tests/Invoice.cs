using System;

namespace FixedWidthParserWriter.Tests
{
    public class Invoice : IFixedWidth
    {
        [FixedWidthFileField(Line = 1)]
        public string CompanyName { get; set; }

        // Format set on class with FormatDateTime so not required on each Attribute of DateTime Property
        [FixedWidthFileField(Line = 4, Start = 15, Length = 19/*, Format = "yyyy-MM-dd"*/)] // In general Length could be = 10 but name of this DateField in Template is 13 (>10) so we have to use at least 13 or max spaces which is 19.
        public DateTime Date { get; set; }

        [FixedWidthFileField(Line = 4, Start = 43)]
        public string BuyerName { get; set; }

        [FixedWidthFileField(Line = 6, Start = 37)]
        public string InvoiceNumber { get; set; }

        [FixedWidthFileField(Line = -4, Length = 66, Format = "0,000.00")]
        public decimal AmountTotal { get; set; }

        [FixedWidthFileField(Line = -2, Start = 7, Length = 10/*, Format = "yyyy-MM-dd"*/)]
        public DateTime DateCreated { get; set; }

        [FixedWidthFileField(Line = -2, Start = 17, Length = 50, PadSide = PadSide.Left)]
        public string SignatoryTitle { get; set; }

        [FixedWidthFileField(Line = -1, Length = 66, PadSide = PadSide.Left)] // Line Negative - counted from bottom
        public string SignatureName { get; set; }

        public DefaultConfig GetDefaultConfig(int StructureTypeId)
        {
            return new DefaultConfig
            {
                FormatDateTime = "yyyy-MM-dd"
                //FormatNumberDecimal = "0,000.00"
            };
        }
    }
}
