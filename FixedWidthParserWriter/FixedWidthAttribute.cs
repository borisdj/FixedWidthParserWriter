using System;

namespace FixedWidthParserWriter
{
    public class FixedWidthAttribute : Attribute
    {
        public char RecordType { get; set; }
        public virtual int Start { get; set; }
        public virtual int Length { get; set; }
        public virtual string Format { get; set; }

        public virtual char Pad { get; set; } = '\0';
        public virtual PadSide PadSide { get; set; }

        public virtual bool DoTrim { get; set; } = true;

        public virtual int StartIndex => Start - 1;

        public int StructureTypeId { get; set; }

        /// <summary>
        /// Pattern used to represent a null value.
        /// </summary>
        public virtual string NullPattern { get; set; } = string.Empty;

        public bool ExceptionOnOverflow { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthLineFieldAttribute : FixedWidthAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthFileFieldAttribute : FixedWidthAttribute
    {
        public override int Start { get; set; } = 1; // overriding to set initial value

        public int Line { get; set; } = 1;

        public int LineIndex => Line - 1;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomFileFieldAttribute : FixedWidthAttribute
    {
        public override int Start { get; set; } = 1; // overriding to set initial value
        public virtual string StartsWith { get; set; }

        public virtual string EndsWith { get; set; }

        public virtual string Contains { get; set; }

        public virtual int Offset { get; set; }

        public virtual string RemoveText { get; set; }

        public virtual bool RemoveStartsWith { get; set; } = true;
        public virtual bool RemoveEndsWith { get; set; } = true;
        public virtual bool RemoveContains { get; set; } = true;
    }
}
