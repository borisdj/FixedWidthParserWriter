using System.Collections.Generic;

namespace FixedWidthParserWriter
{
    /// <summary>
    ///     Provides configuration for FixedWidth Provider
    /// </summary>
    public class FixedWidthConfig
    {
        /// <summary>
        ///     When need more then 1 file structure/format we can put multiple Attributes per Property with different StructureTypeId.
        /// </summary>
        /// <value>
        ///     Default value is 0, and other types can be set with incremental integer numbers.
        /// </value>
        public int StructureTypeId { get; set; }

        /// <summary>
        ///     Enables Attributes to be defined at runtime, for all usage types. Dict with PropertyName and independent Attribute with parameter values.
        /// </summary>
        public Dictionary<string, FixedWidthAttribute> DynamicSettings { get; set; }

        /// <summary>
        ///     When set parsing error are skipped, no exception, and ErrorMessage logged into list ErrorsLog.
        /// </summary>
        public bool LogAndSkipErrors { get; set; }

        /// <summary>
        ///     List for Error Messages when being logged.
        /// </summary>
        internal List<string> ErrorsLog { get; set; } = new List<string>();

        /// <summary>
        ///     List for Warnings, like when Writing string is cut to fit into defined position.
        /// </summary>
        internal List<string> WarningsLog { get; set; } = new List<string>();
    };

    public class DefaultConfig
    {
        // Formats
        public virtual string FormatNumberInteger { get; set; } = "0";
        public virtual string FormatNumberDecimal { get; set; } = "0.00";
        public virtual string FormatBoolean { get; set; } = "1; ;0";
        public virtual string FormatDateTime { get; set; } = "yyyyMMdd";

        //public virtual string FormatInt32 { get; set; }   // FormatNumberInteger
        //public virtual string FormatInt64 { get; set; }   // FormatNumberInteger
        //public virtual string FormatDecimal { get; set; } // FormatNumberDecimal
        //public virtual string FormatSingle { get; set; }  // FormatNumberDecimal
        //public virtual string FormatDouble { get; set; }  // FormatNumberDecimal

        // Pads
        public virtual char PadNumeric { get; set; } = ' ';
        public virtual char PadNonNumeric { get; set; } = ' ';

        public virtual PadSide PadSideNumeric { get; set; } = PadSide.Left;
        public virtual PadSide PadSideNonNumeric { get; set; } = PadSide.Right;
    }

    public enum PadSide
    {
        Default,
        Right,
        Left
    }

    public enum FieldType
    {
        LineField,
        FileField,
        CustomFileField
    }
}
