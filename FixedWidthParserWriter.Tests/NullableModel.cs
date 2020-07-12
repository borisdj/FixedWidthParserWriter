using System;

namespace FixedWidthParserWriter.Tests
{
    public class NullableModel : IFixedWidth
    {
        [FixedWidthLineField(Start = 1, Length = 9, NullPattern = "ooooooooo")]
        public char? Char { get; set; }

        [FixedWidthLineField(Start = 10, Length = 10, NullPattern = "0000000000")]
        public Int32? Int32 { get; set; }

        [FixedWidthLineField(Start = 20, Length = 20, NullPattern = "11111111111111111111")]
        public Int64? Int64 { get; set; }

        [FixedWidthLineField(Start = 40, Length = 20, NullPattern = "00000.00000000000000")]
        public Decimal? Decimal { get; set; }

        [FixedWidthLineField(Start = 60, Length = 10, NullPattern = "00000000.0")]
        public Single? Single { get; set; }

        [FixedWidthLineField(Start = 70, Length = 10, NullPattern = "0000000.00")]
        public Double? Double { get; set; }

        [FixedWidthLineField(Start = 80, Length = 10, NullPattern = "0000-00-00")]
        public DateTime? DateTime { get; set; }

        [FixedWidthLineField(Start = 90, Length = 1, NullPattern = "3")]
        public bool? Bool { get; set; }

        public DefaultConfig GetDefaultConfig(int StructureTypeId)
        {
            return new DefaultConfig
            {
                FormatDateTime = "yyyy-MM-dd"
            };
        }
    }
}
