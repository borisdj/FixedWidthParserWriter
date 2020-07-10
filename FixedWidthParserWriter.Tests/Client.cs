using System.Collections.Generic;

namespace FixedWidthParserWriter.Tests
{
    [FixedWidth(RecordType='1')]
    internal class Client
    {
        public List<InvoiceItem> Invoices { get; internal set; }
        
        [FixedWidthLineField(Start = 2, Length = 66)]
        public string Name { get; internal set; }

        public Client()
        {
            Invoices = new List<InvoiceItem>();
        }
    }
}