using System;

namespace FixedWidthParserWriter
{
    public class FixedWidthAttribute : Attribute
    {
        public virtual int Start { get; set; }
        public virtual int Length { get; set; }
        public virtual string Format { get; set; }

        public virtual char Pad { get; set; } = '\0';
        public virtual PadSide PadSide { get; set; }

        public virtual int StartIndex => Start - 1;

        public int StructureTypeId { get; set; }

        public bool ExceptionOnOverflow { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthLineFieldAttribute : FixedWidthAttribute
    {
        public override object TypeId { get { return this; } } // overriding done because of AllowMultiple == true
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FixedWidthFileFieldAttribute : FixedWidthAttribute
    {
        public override int Start { get; set; } = 1; // overriding to set initial value

        public override object TypeId { get { return this; } }

        public int Line { get; set; } = 1;

        public int LineIndex => Line - 1;
    }
}
