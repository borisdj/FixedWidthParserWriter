using System;

namespace FixedWidthParserWriter.Tests
{
    public class Invoice
    {
        [FixedWidthFileField(Start = 1, Length = 18)] // StructureTypeId = (int)EBank.Halcom,
        public string CompanyName { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)] 
        public string CompanyAddress { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)] 
        public DateTime Date { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)] 
        public DateTime DueDate { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)]
        public string BuyerName { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)]
        public string BuyerAddress { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)]
        public decimal AmountTotal { get; set; }

        [FixedWidthFileField(Start = 1, Length = 0)]
        public string SignatureName { get; set; }
    }
}
