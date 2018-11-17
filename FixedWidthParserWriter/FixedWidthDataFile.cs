using System;
using System.Collections.Generic;
using System.Linq;

namespace FixedWidthParserWriter
{
    public class FixedWidthDataFile<T> : FixedWidthData where T : class, new()
    {
        public T Parse(List<string> lines, int structureTypeId = 0)
        {
            return ParseData<T>(lines, FieldType.FileField, structureTypeId);
        }

        public void UpdateContent(int structureTypeId = 0)
        {
            SetDefaultConfig();

            var orderProperties = this.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthAttribute))).ToList();
            string orderLine = String.Empty;

            foreach (var prop in orderProperties)
            {
                ToFormatedString(prop, FieldType.FileField, structureTypeId);
            }
        }
    }
}
