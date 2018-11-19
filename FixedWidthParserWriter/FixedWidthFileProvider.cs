using System;
using System.Collections.Generic;
using System.Linq;

namespace FixedWidthParserWriter
{
    public class FixedWidthFileProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public T Parse(List<string> lines, int structureTypeId = 0)
        {
            StructureTypeId = structureTypeId;
            return ParseData<T>(lines, FieldType.FileField);
        }

        public void UpdateContent(T data, int structureTypeId = 0)
        {
            StructureTypeId = structureTypeId;

            SetDefaultConfig();
            if (data is IFixedWidth fixedWidth)
            {
                DefaultConfig = fixedWidth.GetDefaultConfig(StructureTypeId);
            }

            var orderProperties = data.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthAttribute))).ToList();
            string orderLine = String.Empty;

            foreach (var prop in orderProperties)
            {
                WriteData(data, prop, FieldType.FileField);
            }
        }
    }
}
