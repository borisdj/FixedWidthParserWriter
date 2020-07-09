using System.Collections.Generic;

namespace FixedWidthParserWriter.Tests
{
    internal class Client
    {
        public List<InvoiceItem> Invoices { get; internal set; }
        
        [FixedWidthLineField(Start = 2, Length = 30)]
        public string Name { get; internal set; }

        public Client()
        {
            Invoices = new List<InvoiceItem>();
        }
    }
}