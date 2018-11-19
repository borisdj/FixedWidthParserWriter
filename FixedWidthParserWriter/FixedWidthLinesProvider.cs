using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FixedWidthParserWriter
{
    public class FixedWidthLinesProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public List<T> Parse(List<string> lines, int structureTypeId = 0)
        {
            StructureTypeId = structureTypeId;
            List<T> result = new List<T>();
            foreach (var line in lines)
            {
                result.Add(ParseData<T>(new List<string> { line }, FieldType.LineField));
            }
            return result;
        }

        public List<string> Write(List<T> data, int structureTypeId = 0)
        {
            StructureTypeId = structureTypeId;
            LoadNewDefaultConfig(data[0]);

            List<string> resultLines = new List<string>();
            foreach (T element in data)
            {
                var orderProperties = element.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthLineFieldAttribute)));
                orderProperties = orderProperties.Where(a => a.GetCustomAttributes<FixedWidthLineFieldAttribute>().Any(b => b.StructureTypeId == StructureTypeId));
                orderProperties = orderProperties.OrderBy(a => a.GetCustomAttributes<FixedWidthLineFieldAttribute>().Single(b => b.StructureTypeId == StructureTypeId).Start);

                string line = String.Empty;

                int startPrev = 1;
                int lengthPrev = 0;
                foreach (PropertyInfo prop in orderProperties)
                {
                    var attribute = prop.GetCustomAttributes<FixedWidthLineFieldAttribute>().Single(a => a.StructureTypeId == StructureTypeId);
                    if (startPrev + lengthPrev != attribute.Start)
                        throw new InvalidOperationException($"Invalid Start or Length parameter, {attribute.Start} !=  {startPrev + lengthPrev}" +
                                                            $", on FixedLineFieldAttribute (property {prop.Name}) for StructureTypeId {StructureTypeId}");
                    startPrev = attribute.Start;
                    lengthPrev = attribute.Length;

                    line += WriteData(element, prop, FieldType.LineField);
                }
                resultLines.Add(line);
            }
            return resultLines;
        }
    }
}
