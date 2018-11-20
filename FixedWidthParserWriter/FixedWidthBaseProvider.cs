using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FastMember;

namespace FixedWidthParserWriter
{
    public interface IFixedWidth
    {
        DefaultConfig GetDefaultConfig(int structureTypeId);
    }

    public class FastMemberData
    {
        public Member Member { get; set; }
        public TypeAccessor Accessor { get; set; }
        public Attribute Attribute { get; set; }
        public Dictionary<string, string> MemberNameTypeNameDict { get; set; }
    }

    public class FixedWidthBaseProvider
    {
        public int StructureTypeId { get; set; }

        public List<string> Content { get; set; }

        public virtual DefaultConfig DefaultConfig { get; set; } = new DefaultConfig();

        public virtual void SetDefaultConfig() { } // can be override to change DefaultConfig on entire Provider class

        protected void LoadNewDefaultConfig<T>(T data)
        {
            SetDefaultConfig();
            if (data is IFixedWidth fixedWidth)
            {
                DefaultConfig = fixedWidth.GetDefaultConfig(StructureTypeId);
            }
        }

        protected T ParseData<T>(List<string> lines, FieldType fieldType) where T : class, new()
        {
            var data = new T();
            LoadNewDefaultConfig(data);

            var accessor = TypeAccessor.Create(typeof(T), true);
            var membersData = accessor.GetMembers().Where(a => a.IsDefined(typeof(FixedWidthAttribute))).ToList();

            //var properties = data.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthAttribute))).ToList(); // Reflection - DEPRECATED
            //foreach (var property in properties)
            foreach (var member in membersData)
            {
                if (!member.CanWrite)
                    continue;

                FixedWidthAttribute attribute = null;
                int lineIndex = 0;
                if (fieldType == FieldType.LineField)
                {
                    attribute = member.GetMemberAttributes<FixedWidthLineFieldAttribute>().SingleOrDefault(a => a.StructureTypeId == StructureTypeId);
                }
                else if (fieldType == FieldType.FileField)
                {
                    attribute = member.GetMemberAttributes<FixedWidthFileFieldAttribute>().SingleOrDefault(a => a.StructureTypeId == StructureTypeId);
                    // Deprecated from when every StructureType had all Properties and those that did not had it's specific attribute took the one with higest StructureTypeId
                    //field = property.GetCustomAttributes<FixedWidthFileFieldAttribute>().Where(a => a.StructureTypeId <= structureTypeId).OrderByDescending(a => a.StructureTypeId).FirstOrDefault();

                    var fileField = (FixedWidthFileFieldAttribute)attribute;
                    if (fileField.Line == 0)
                        throw new InvalidOperationException("'Line' parameter of [FixedWidthFileFieldAttribute] can not be zero.");
                    lineIndex = fileField.LineIndex;
                    if (lineIndex < 0) // line is counted from bottom
                    {
                        lineIndex += lines.Count + 1;
                    }
                }
                if (attribute != null)
                {
                    string valueString = lines[lineIndex];

                    int startIndex = attribute.StartIndex;
                    int length = attribute.Length;
                    if (startIndex > 0 || length > 0) // Length = 0; means value is entire line
                    {
                        if (valueString.Length < startIndex + length)
                        {
                            throw new InvalidOperationException($"Property {member.Name}='{valueString}' with Length={valueString.Length}" +
                                                                $"not enough for Substring(Start={startIndex + 1}, Length={length})");
                        }
                        valueString = (length == 0) ? valueString.Substring(startIndex) : valueString.Substring(startIndex, length);
                    }

                    valueString = valueString.Trim();

                    object value = null;
                    var underlyingType = Nullable.GetUnderlyingType(member.Type);
                    string valueTypeName = underlyingType != null ? underlyingType.Name : member.Type.Name;

                    if (valueTypeName == nameof(String)) // string
                    {
                        value = valueString;
                    }
                    else if(valueTypeName == nameof(Char)) // char
                    {
                        value = (char)valueString[0];
                    }
                    else
                    {
                        string format = attribute.Format; //= field.Format ?? DefaultFormat.GetFormat(valueTypeName);

                        if (valueTypeName == nameof(Int32) || valueTypeName == nameof(Int64))
                        {
                            format = format ?? DefaultConfig.FormatNumberInteger;
                            switch (valueTypeName)
                            {
                                case nameof(Int32): // int
                                    value = Int32.Parse(valueString);
                                    break;
                                case nameof(Int64): // long
                                    value = Int64.Parse(valueString);
                                    break;
                            }
                        }
                        else if (valueTypeName == nameof(Decimal) || valueTypeName == nameof(Single) || valueTypeName == nameof(Double))
                        {
                            format = format ?? DefaultConfig.FormatNumberDecimal;
                            switch (valueTypeName)
                            {
                                case nameof(Decimal): // decimal
                                    value = Decimal.Parse(valueString, CultureInfo.InvariantCulture);
                                    if (format.Contains(";")) //';' - Special custom Format that removes decimal separator ("0;00": 123.45 -> 12345)
                                        value = (decimal)value / (decimal)Math.Pow(10, format.Length - 2); // "0;00".Length == 4 - 2 = 2 (10^2 = 100)
                                    break;
                                case nameof(Single): // float
                                    value = Single.Parse(valueString, CultureInfo.InvariantCulture);
                                    if (format.Contains(";"))
                                        value = (float)value / (float)Math.Pow(10, format.Length - 2);
                                    break;
                                case nameof(Double):  // double
                                    value = Double.Parse(valueString, CultureInfo.InvariantCulture);
                                    if (format.Contains(";"))
                                        value = (double)value / (double)Math.Pow(10, format.Length - 2);
                                    break;
                            }
                        }
                        else if(valueTypeName == nameof(Boolean))
                        {
                            format = format ?? DefaultConfig.FormatBoolean;
                        }
                        else if (valueTypeName == nameof(DateTime))
                        {
                            format = format ?? DefaultConfig.FormatDateTime;
                            value = DateTime.ParseExact(valueString, format, CultureInfo.InvariantCulture);
                        }
                    }
                    accessor[data, member.Name] = value;
                    //property.SetValue(data, value); // With Reflection - DEPRECATED
                }
            }
            return data;
        }

        protected string WriteData<T>(T element, FastMemberData memberData, FieldType fieldType)
        {
            string memberName = memberData.Member.Name;
            var attribute = (FixedWidthAttribute)memberData.Attribute;

            //var value = property.GetValue(element); // With Reflection - DEPRECATED
            var value = memberData.Accessor[element, memberName];

            string valueTypeName = memberData.MemberNameTypeNameDict[memberName];

            if (attribute.PadSide == PadSide.Default)
            {
                bool isNumbericType = (valueTypeName == nameof(Int32) || valueTypeName == nameof(Int64) || // IntegerNumbers
                                       valueTypeName == nameof(Decimal) || valueTypeName == nameof(Single) || valueTypeName == nameof(Double)); // or DecimalNumbers
                attribute.PadSide = isNumbericType ? DefaultConfig.PadSideNumeric : DefaultConfig.PadSideNonNumeric; // Initial default Left pad: Numeric-Left, NonNumeric-Right
            }
            char pad = ' ';

            string result = String.Empty;
            if (valueTypeName == nameof(String) || valueTypeName == nameof(Char))
            {
                result = value?.ToString() ?? "";
            }
            else
            {
                string format = attribute.Format;
                //format = format ?? DefaultFormat.GetFormat(valueTypeName);
                switch (valueTypeName)
                {
                    case nameof(Int32):
                    case nameof(Int64):
                        format = format ?? DefaultConfig.FormatNumberInteger;
                        pad = attribute.Pad != '\0' ? attribute.Pad : DefaultConfig.PadSeparatorNumeric;
                        break;
                    case nameof(Decimal):
                    case nameof(Single):
                    case nameof(Double):
                        format = format ?? DefaultConfig.FormatNumberDecimal;
                        pad = attribute.Pad != '\0' ? attribute.Pad : DefaultConfig.PadSeparatorNumeric;
                        if (format.Contains(";")) //';' - Special custom Format that removes decimal separator ("0;00": 123.45 -> 12345)
                        {
                            double decimalFactor = Math.Pow(10, format.Length - 2); // "0;00".Length == 4 - 2 = 2 (10^2 = 100)
                            switch (valueTypeName)
                            {
                                case nameof(Decimal):
                                    value = (decimal)value * (decimal)decimalFactor;
                                    break;
                                case nameof(Single):
                                    value = (float)value * (float)decimalFactor;
                                    break;
                                case nameof(Double):
                                    value = (double)value * decimalFactor;
                                    break;
                            }
                        }
                        break;
                    case nameof(Boolean):
                        format = format ?? DefaultConfig.FormatBoolean;
                        pad = attribute.Pad != '\0' ? attribute.Pad : DefaultConfig.PadSeparatorNonNumeric;
                        value = value.GetHashCode();
                        break;
                    case nameof(DateTime):
                        format = format ?? DefaultConfig.FormatDateTime;
                        pad = attribute.Pad != '\0' ? attribute.Pad : DefaultConfig.PadSeparatorNonNumeric;
                        break;
                }
                result = format != null ? String.Format(CultureInfo.InvariantCulture, $"{{0:{format}}}", value) : value.ToString();
            }

            if (result.Length > attribute.Length && attribute.Length > 0) // if too long cut it
            {
                result = result.Substring(0, attribute.Length);
            }
            else if (result.Length < attribute.Length) // if too short pad from one side
            {
                if (attribute.PadSide == PadSide.Right)
                    result = result.PadRight(attribute.Length, pad);
                else if (attribute.PadSide == PadSide.Left)
                    result = result.PadLeft(attribute.Length, pad);
            }

            if (fieldType == FieldType.FileField)
            {
                int lineIndex = ((FixedWidthFileFieldAttribute)attribute).LineIndex;
                var content = this.Content;
                if (lineIndex < 0)
                {
                    lineIndex += content.Count + 1;
                }
                string clearString = String.Empty;
                clearString = attribute.Length > 0 ? content[lineIndex].Remove(attribute.StartIndex, attribute.Length) : content[lineIndex].Remove(attribute.StartIndex);
                content[lineIndex] = clearString.Insert(attribute.StartIndex, result);
            }
            return result;
        }
    }
}
