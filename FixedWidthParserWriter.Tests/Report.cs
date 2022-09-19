using System;

namespace FixedWidthParserWriter.Tests
{
    public class Report
    {
        [CustomFileField(EndsWith = "company")]
        public string CompanyName { get; set; }

        [CustomFileField(StartsWith = "Date generated: ", Format = "d.M.yyyy.")]
        public DateTime Date { get; set; }

        [CustomFileField(Contains = "Q REPORT no. ", RemoveText = "**")]
        public string Number { get; set; }

        [CustomFileField(Contains = "ACTN.", Length = -15)]
        public string Account { get; set; }

        [CustomFileField(StartsWith = "Revenue:", Offset = 1)]
        public decimal Revenue { get; set; }
    }
}
