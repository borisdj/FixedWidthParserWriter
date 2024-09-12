using System.Collections.Generic;
using System.Linq;
using FastMember;

namespace FixedWidthParserWriter
{
    public class FixedWidthFileProvider<T> : FixedWidthBaseProvider where T : class, new()
    {
        public T Parse(List<string> lines, int structureTypeId = 0, Dictionary<string, FixedWidthAttribute> dynamicSettings = null, List<string> errorLog = null)
        {
            StructureTypeId = structureTypeId;
            ErrorLog = errorLog;
            return ParseData<T>(lines, FieldType.FileField, dynamicSettings);
        }

        public void UpdateContent(T data, int structureTypeId = 0, Dictionary<string, FixedWidthAttribute> dynamicSettings = null, List<string> errorLog = null)
        {
            StructureTypeId = structureTypeId;
            ErrorLog = errorLog;
            LoadNewDefaultConfig(data);

            var accessor = TypeAccessor.Create(typeof(T), true);
            var memberSet = accessor.GetMembers().Where(a => a.IsDefined(typeof(FixedWidthFileFieldAttribute)));
            var membersData = new List<Member>();
            var attributesDict = new Dictionary<string, FixedWidthAttribute>();
            var memberNameTypeNameDict = new Dictionary<string, string>();
            foreach (var member in memberSet)
            {
                FixedWidthAttribute attributeData = null;
                if (dynamicSettings != null && dynamicSettings.ContainsKey(member.Name))
                {
                    attributeData = dynamicSettings[member.Name];
                }
                else
                {
                    attributeData = member.GetMemberAttributes<FixedWidthFileFieldAttribute>().SingleOrDefault(a => a.StructureTypeId == StructureTypeId);
                }
                if (attributeData != null)
                {
                    membersData.Add(member);
                    attributesDict.Add(member.Name, attributeData);
                    memberNameTypeNameDict.Add(member.Name, member.Type.Name);
                }
            }

            foreach (var propertyMember in membersData)
            {
                var attribute = attributesDict[propertyMember.Name];
                var memberData = new FastMemberData()
                {
                    Member = propertyMember,
                    Accessor = accessor,
                    Attribute = attribute,
                    MemberNameTypeNameDict = memberNameTypeNameDict
                };
                WriteData(data, memberData, FieldType.FileField);
            }
        }
    }
}
