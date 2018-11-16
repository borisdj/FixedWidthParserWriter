using System;

namespace FixedWidthParserWriter.Tests
{
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
        [FixedWidthFileField(Line = 5, Start = 15, Length = 19/*, Format = "yyyy-MM-dd"*/)]
        public DateTime Date { get; set; }

        [FixedWidthFileField(Line = 6, Start = 15, Length = 19/*, Format = "yyyy-MM-dd"*/)]
        public DateTime DueDate { get; set; }

        [FixedWidthFileField(Line = 5, Start = 43)]
        public string BuyerName { get; set; }

        [FixedWidthFileField(Line = 6, Start = 43)]
        public string BuyerAddress { get; set; }

        [FixedWidthFileField(Line = 8, Start = 37)]
        public string InvoiceNumber { get; set; }

        [FixedWidthFileField(Line = -4, Length = 66, Pad = ' ', Format = "0,000.00")]
        public decimal AmountTotal { get; set; }

        [FixedWidthFileField(Line = -2, Start = 7, Length = 10)]
        public DateTime DateCreated { get; set; }

        [FixedWidthFileField(Line = -2, Start = 17, Length = 50, PadSide = PadSide.Left)]
        public string SignatoryTitle { get; set; }

        [FixedWidthFileField(Line = -1, Length = 66, PadSide = PadSide.Left)]
        public string SignatureName { get; set; }
    }

    public class InvoiceDefaultFormat : DefaultFormat
    {
        public override string DateTimeFormat { get; set; } = "yyyy-MM-dd";
    }
}
