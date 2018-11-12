using System;

namespace FixedWidthParserWriter
{
    public class FixedWidthAttribute : Attribute
    {
        public virtual int Start { get; set; }
        public virtual int Length { get; set; }
        public virtual string Format { get; set; }

        public virtual char? Pad { get; set; }
        public virtual PadSide PadSide { get; set; }

        public virtual int StartIndex => Start - 1;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthLineFieldAttribute : FixedWidthAttribute
    {
        public override object TypeId { get { return this; } } // overriding done because of AllowMultiple == true

        public int StructureTypeId { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthFileFieldAttribute : FixedWidthAttribute
    {
        public override object TypeId { get { return this; } }

        public override int Start { get; set; } = 1; // overriding to set initial value

        public int Line { get; set; } = 1;
        public string SplitSeparator { get; set; }

        public int StructureTypeId { get; set; }

        public int LineIndex => Line - 1;
    }
}
