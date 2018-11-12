using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FixedWidthParserWriter
{
    public class FixedWidthData
    {
        public List<string> Content { get; set; }
        public DefaultFormat Format { get; set; } = new DefaultFormat();
        public DefaultPad Pad { get; set; } = new DefaultPad();

        public virtual void SetFormatAndPad() { } // to be overriden for custom format

        protected T ParseData<T>(List<string> lines, FieldType fieldType, int structureTypeId = 0) where T : class, new()
        {
            var data = new T();
            var orderProperties = data.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(FixedWidthAttribute))).ToList();

            foreach (var property in orderProperties)
            {
                if (!property.CanWrite)
                    continue;

                FixedWidthAttribute field = null;
                int lineIndex = 0;
                if (fieldType == FieldType.LineField)
                {
                    field = property.GetCustomAttribute<FixedWidthLineFieldAttribute>();
                }
                else if (fieldType == FieldType.FileField)
                {
                    field = property.GetCustomAttributes<FixedWidthFileFieldAttribute>().Where(a => a.StructureTypeId <= structureTypeId).OrderByDescending(a => a.StructureTypeId).FirstOrDefault();
                    lineIndex = ((FixedWidthFileFieldAttribute)field).LineIndex;
                    if (lineIndex < 0) // line is counted from bottom
                    {
                        lineIndex += lines.Count;
                    }
                }
                string valueString = lines[lineIndex];

                int startIndex = field.StartIndex;
                int length = field.Length;
                if (startIndex > 0 || length > 0) // Length = 0; means value is entire line
                {
                    if (valueString.Length < startIndex + length)
                    {
                        throw new InvalidOperationException($"Property {property.Name}='{valueString}' with Length={valueString.Length} not enough for Substring(Start={startIndex + 1}, Length={length})");
                    }
                    valueString = (length == 0) ? valueString.Substring(startIndex) : valueString.Substring(startIndex, length);
                }

                if (fieldType == FieldType.FileField)
                {
                    string separator = ((FixedWidthFileFieldAttribute)field).SplitSeparator;
                    if (separator != null && valueString.Contains(separator))
                        valueString = valueString.Split(new string[] { separator }, StringSplitOptions.None)[0];
                }
                valueString = valueString.Trim();

                object value = null;
                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                string valueTypeName = underlyingType != null ? underlyingType.Name : property.PropertyType.Name;

                if (valueTypeName == nameof(String))
                {
                    value = valueString;
                }
                else
                {
                    string format = field.Format;
                    //format = format ?? DefaultFormat.GetFormat(valueTypeName); // using Dict created by Reflection to only one line: format = format ?? ...
                    switch (valueTypeName)
                    {
                        case nameof(DateTime):
                            format = format ?? Format.DateTimeFormat;
                            value = DateTime.ParseExact(valueString, format, CultureInfo.InvariantCulture);
                            break;
                        case nameof(Int32):
                            format = format ?? Format.Int32Format;
                            value = Int32.Parse(valueString);
                            break;
                        case nameof(Decimal):
                            format = format ?? Format.DecimalFormat;
                            value = Decimal.Parse(valueString, CultureInfo.InvariantCulture);
                            if (format.Contains(";"))
                                value = (decimal)value / (decimal)Math.Pow(10, format.Length - 2); // "0;00".Length == 4 - 2 = 2 (10^2 = 100)
                            break;
                        case nameof(Boolean):
                            format = format ?? Format.BooleanFormat;
                            if (format.Contains(";"))
                                value = valueString == format.Split(';')[0];
                            else
                                value = Boolean.Parse(valueString);
                            break;
                    }
                }
                property.SetValue(data, value);
                // ALTERNATIVE using FastMember library - faster then Reflection
                //accessor[data, property.Name] = value;
            }
            return data;
        }

        protected string ToFormatedString(PropertyInfo property, FieldType fieldType, int structureTypeId = 0)
        {
            FixedWidthAttribute field = null;
            if (fieldType == FieldType.LineField)
            {
                field = property.GetCustomAttributes<FixedWidthLineFieldAttribute>().Where(a => a.StructureTypeId <= structureTypeId).OrderByDescending(a => a.StructureTypeId).FirstOrDefault();
            }
            else if (fieldType == FieldType.FileField)
            {
                field = property.GetCustomAttributes<FixedWidthFileFieldAttribute>().Where(a => a.StructureTypeId <= structureTypeId).OrderByDescending(a => a.StructureTypeId).FirstOrDefault();
            }
            var value = property.GetValue(this);
            //var accessor = TypeAccessor.Create(this.GetType()); // move before in caller method before loop
            //var value = accessor[this, property.Name];
            
            string valueTypeName = value?.GetType().Name ?? nameof(String);

            if (valueTypeName == nameof(Decimal) || valueTypeName == nameof(Int32))
                field.PadSide = field.PadSide != PadSide.Default ? field.PadSide : PadSide.Left; // Numeric types have default Left pad
            else
                field.PadSide = field.PadSide != PadSide.Default ? field.PadSide : PadSide.Right; // others types have default Right pad

            char pad = ' ';

            string result = String.Empty;
            if (valueTypeName == nameof(String))
            {
                result = value?.ToString() ?? "";
            }
            else
            {
                string format = field.Format;
                //format = format ?? DefaultFormat.GetFormat(valueTypeName);
                switch (valueTypeName)
                {
                    case nameof(DateTime):
                        format = format ?? Format.DateTimeFormat;
                        pad = field.Pad ?? Pad.CharacterSeparator;
                        break;
                    case nameof(Int32):
                        format = format ?? Format.Int32Format;
                        pad = field.Pad ?? Pad.NumericSeparator;
                        break;
                    case nameof(Decimal):
                        format = format ?? Format.DecimalFormat;
                        pad = field.Pad ?? Pad.NumericSeparator;
                        if (format.Contains(";"))
                            value = (decimal)value * (decimal)Math.Pow(10, format.Length - 2); // "0;00".Length == 4 - 2 = 2 (10^2 = 100)
                        break;
                    case nameof(Boolean):
                        format = format ?? Format.BooleanFormat;
                        pad = field.Pad ?? Pad.CharacterSeparator;
                        value = value.GetHashCode();
                        break;
                }
                result = format != null ? String.Format(CultureInfo.InvariantCulture, $"{{0:{format}}}", value) : value.ToString();
            }

            if (result.Length > field.Length && field.Length > 0) // if too long cut it
            {
                result = result.Substring(0, field.Length);
            }
            else if (result.Length < field.Length) // if too short pad from one side
            {
                if (field.PadSide == PadSide.Right)
                    result = result.PadRight(field.Length, pad);
                else if (field.PadSide == PadSide.Left)
                    result = result.PadLeft(field.Length, pad);
            }
            if (fieldType == FieldType.FileField)
            {
                int lineIndex = ((FixedWidthFileFieldAttribute)field).LineIndex;
                var content = this.Content;
                if (lineIndex < 0)
                {
                    lineIndex += content.Count;
                }
                string clearString = String.Empty;
                clearString = field.Length > 0 ? content[lineIndex].Remove(field.StartIndex, field.Length) : content[lineIndex].Remove(field.StartIndex);
                content[lineIndex] = clearString.Insert(field.StartIndex, result);
            }
            return result;
        }
    }
}
