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
        [FixedWidthFileField(Line = 5, Start = 15, Length = 25/*, Format = "yyyy-MM-dd"*/)]
        public DateTime Date { get; set; }

        [FixedWidthFileField(Line = 6, Start = 15, Length = 25/*, Format = "yyyy-MM-dd"*/)]
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

        [FixedWidthFileField(Line = -1, Length = 78, PadSide = PadSide.Left)]
        public string SignatureName { get; set; }
    }

    public class InvoiceDefaultFormat : DefaultFormat
    {
        public override string DateTimeFormat { get; set; } = "yyyy-MM-dd";
    }
}
