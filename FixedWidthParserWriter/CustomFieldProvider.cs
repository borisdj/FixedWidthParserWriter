using System.Collections.Generic;

namespace FixedWidthParserWriter
{
    public class CustomFileProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public T Parse(List<string> lines, int structureTypeId = 0, List<string> errorLog = null)
        {
            StructureTypeId = structureTypeId;
            ErrorLog = errorLog;
            return ParseData<T>(lines, FieldType.CustomFileField);
        }
    }
}
