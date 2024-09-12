using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;

namespace FixedWidthParserWriter
{
    public class FixedWidthLinesProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public List<T> Parse(List<string> lines, int structureTypeId = 0, Dictionary<string, FixedWidthAttribute> dynamicSettings = null)
        {
            StructureTypeId = structureTypeId;
            List<T> result = new List<T>();
            foreach (var line in lines)
            {
                result.Add(ParseData<T>(new List<string> { line }, FieldType.LineField, dynamicSettings));
            }
            return result;
        }

        public List<string> Write(List<T> data, int structureTypeId = 0)
        {
            StructureTypeId = structureTypeId;
            LoadNewDefaultConfig(new T());

            var accessor = TypeAccessor.Create(typeof(T), true);
            var memberSet = accessor.GetMembers().Where(a => a.IsDefined(typeof(FixedWidthLineFieldAttribute)));
            var membersDict = new Dictionary<int, Member>();
            var attributesDict = new Dictionary<string, FixedWidthLineFieldAttribute>();
            var memberNameTypeNameDict = new Dictionary<string, string>();
            foreach (var member in memberSet)
            {
                var attribute = member.GetMemberAttributes<FixedWidthLineFieldAttribute>().SingleOrDefault(a => a.StructureTypeId == StructureTypeId);
                if (attribute != null)
                {
                    membersDict.Add(attribute.Start, member);
                    attributesDict.Add(member.Name, attribute);
                    memberNameTypeNameDict.Add(member.Name, member.Type.Name);
                }
            }
            var membersData = membersDict.OrderBy(a => a.Key).Select(a => a.Value);

            List<string> resultLines = new List<string>();
            foreach (T element in data)
            {
                string line = String.Empty;

                int startPrev = 1;
                int lengthPrev = 0;
                foreach (var propertyMember in membersData)
                {
                    var attribute = attributesDict[propertyMember.Name];
                    if (startPrev + lengthPrev != attribute.Start)
                        throw new InvalidOperationException($"Invalid Start or Length parameter, {attribute.Start} !=  {startPrev + lengthPrev}" +
                                                            $", on FixedLineFieldAttribute (property {propertyMember.Name}) for StructureTypeId {StructureTypeId}");
                    startPrev = attribute.Start;
                    lengthPrev = attribute.Length;

                    var memberData = new FastMemberData()
                    {
                        Member = propertyMember,
                        Accessor = accessor,
                        Attribute = attribute,
                        MemberNameTypeNameDict = memberNameTypeNameDict
                    };
                    line += WriteData(element, memberData, FieldType.LineField);
                }
                resultLines.Add(line);
            }
            return resultLines;
        }
    }
}
