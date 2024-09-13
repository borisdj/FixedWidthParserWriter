using System;
using System.Collections.Generic;

namespace FixedWidthParserWriter
{
    public class CustomFileProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public T Parse(List<string> lines, FixedWidthConfig fixedWidthConfig = null)
        {
            fixedWidthConfig = fixedWidthConfig ?? new FixedWidthConfig();
            StructureTypeId = fixedWidthConfig.StructureTypeId;

            return ParseData<T>(lines, FieldType.CustomFileField, fixedWidthConfig);
        }

        // DEPRECATED: From v1.2.0 Settings args wrapped into object FixedWidthConfig as second argument (kept to reduce breakingChange)
        [Obsolete("Use instead Parse(List<string> lines, FixedWidthConfig fixedWidthConfig)")]
        public T Parse(List<string> lines, int structureTypeId)
        {
            return Parse(lines, new FixedWidthConfig() { StructureTypeId = structureTypeId });
        }
        // DEPRECATED SEGMENT END --
    }
}
