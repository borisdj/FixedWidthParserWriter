using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FixedWidthParserWriter
{
    public class FixedWidthDataLine<T> : FixedWidthData where T : class, new()
    {
        public T Parse(string line)
        {
            var lines = new List<string> { line };
            return ParseData<T>(lines, FieldType.LineField);
        }

        public int StructureTypeId { get; set; }

        public string ToString(int structureTypeId)
        {
            StructureTypeId = structureTypeId;
            return ToString();
        }

        public override string ToString()
        {
            SetDefaultConfig();

            var orderProperties = this.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthLineFieldAttribute)));
            orderProperties = orderProperties.Where(a => a.GetCustomAttributes<FixedWidthLineFieldAttribute>().Any(b => b.StructureTypeId == StructureTypeId));
            orderProperties = orderProperties.OrderBy(a => a.GetCustomAttributes<FixedWidthLineFieldAttribute>().Single(b => b.StructureTypeId == StructureTypeId).Start);

            string orderLine = String.Empty;

            int startPrev = 1;
            int lengthPrev = 0;
            foreach (var prop in orderProperties)
            {
                var attribute = prop.GetCustomAttributes<FixedWidthLineFieldAttribute>().Single(a => a.StructureTypeId == StructureTypeId);
                if (startPrev + lengthPrev != attribute.Start)
                    throw new InvalidOperationException($"Invalid Start or Length parameter, {attribute.Start} !=  {startPrev + lengthPrev}, on FixedLineFieldAttribute (property {prop.Name}) for StructureTypeId {StructureTypeId}");
                startPrev = attribute.Start;
                lengthPrev = attribute.Length;

                orderLine += ToFormatedString(prop, FieldType.LineField, StructureTypeId);
            }
            return orderLine;
        }
    }
}
