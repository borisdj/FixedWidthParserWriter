using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;

namespace FixedWidthParserWriter
{
    public class FixedWidthLinesProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public List<T> Parse(List<string> lines, FixedWidthConfig fixedWidthConfig = null)
        {
            fixedWidthConfig = fixedWidthConfig ?? new FixedWidthConfig();

            StructureTypeId = fixedWidthConfig.StructureTypeId;

            List<T> result = new List<T>();
            foreach (var line in lines)
            {
                result.Add(ParseData<T>(new List<string> { line }, FieldType.LineField, fixedWidthConfig));
            }
            return result;
        }

        public List<string> Write(List<T> data, FixedWidthConfig fixedWidthConfig = null)
        {
            fixedWidthConfig = fixedWidthConfig ?? new FixedWidthConfig();

            StructureTypeId = fixedWidthConfig.StructureTypeId;

            var dynamicSettings = fixedWidthConfig.DynamicSettings;
            LoadNewDefaultConfig(new T());

            var accessor = TypeAccessor.Create(typeof(T), true);
            var memberSet = accessor.GetMembers().Where(a => a.IsDefined(typeof(FixedWidthLineFieldAttribute)));
            var membersDict = new Dictionary<int, Member>();
            var memberNameTypeNameDict = new Dictionary<string, string>();
            var attributesDict = new Dictionary<string, FixedWidthAttribute>();

            foreach (var member in memberSet)
            {
                FixedWidthAttribute attributeData = null;
                if (dynamicSettings != null && dynamicSettings.ContainsKey(member.Name))
                {
                    attributeData = dynamicSettings[member.Name];
                }
                else
                {
                    attributeData = member.GetMemberAttributes<FixedWidthLineFieldAttribute>().SingleOrDefault(a => a.StructureTypeId == StructureTypeId);
                }

                if (attributeData != null)
                {
                    membersDict.Add(attributeData.Start, member);
                    attributesDict.Add(member.Name, attributeData);
                    memberNameTypeNameDict.Add(member.Name, member.Type.Name);
                }
            }
            var membersData = membersDict.OrderBy(a => a.Key).Select(a => a.Value);

            List<string> resultLines = new List<string>();
            foreach (T element in data)
            {
                string line = string.Empty;

                int startPrev = 1;
                int lengthPrev = 0;
                foreach (var propertyMember in membersData)
                {
                    var attribute = attributesDict[propertyMember.Name];
                    if (startPrev + lengthPrev != attribute.Start)
                    {
                        var errorMessage = $"Invalid Start or Length parameter, {attribute.Start} !=  {startPrev + lengthPrev}, " +
                                           $"on FixedLineFieldAttribute (property {propertyMember.Name}) for StructureTypeId {StructureTypeId}";
                        if (fixedWidthConfig.LogAndSkipErrors)
                        {
                            fixedWidthConfig.ErrorsLog.Add(errorMessage);
                            continue;
                        }
                        else
                        {
                            throw new InvalidOperationException(errorMessage);
                        }
                    }
                    startPrev = attribute.Start;
                    lengthPrev = attribute.Length;

                    var memberData = new FastMemberData()
                    {
                        Member = propertyMember,
                        Accessor = accessor,
                        Attribute = attribute,
                        MemberNameTypeNameDict = memberNameTypeNameDict
                    };
                    line += WriteData(element, memberData, FieldType.LineField, fixedWidthConfig);
                }
                resultLines.Add(line);
            }
            return resultLines;
        }

        // DEPRECATED -- // From v1.2.0 Settings args wrapped into object FixedWidthConfig as 2. argument (kept to reduce breakingChange)
        [Obsolete("Use instead Parse(List<string> lines, FixedWidthConfig fixedWidthConfig)")]
        public List<T> Parse(List<string> lines, int structureTypeId)
        {
            return Parse(lines, new FixedWidthConfig() { StructureTypeId = structureTypeId });
        }

        [Obsolete("Use instead Parse(List<string> lines, FixedWidthConfig fixedWidthConfig)")]
        public List<string> Write(List<T> data, int structureTypeId)
        {
            FixedWidthConfig fixedWidthConfig = new FixedWidthConfig() { StructureTypeId = structureTypeId };
            return Write(data, fixedWidthConfig);
        }
        // DEPRECATED SEGMENT END --
    }
}
