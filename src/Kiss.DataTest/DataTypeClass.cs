using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.DataTest
{
    public class DataBaseTypeClass
    {
        public Boolean CBool { get; set; }
        public Int32 CInt { get; set; }
        public Double CNumeric { get; set; }
        public String CString { get; set; }
        public DateTime CDatetime { get; set; }
     }

    public class DataTypeClass
    {
        public Boolean P_Boolean { get; set; }
        public Char P_Char { get; set; }
        public SByte P_SByte { get; set; }
        public Byte P_Byte { get; set; }
        public Int16 P_Int16 { get; set; }
        public UInt16 P_UInt16 { get; set; }
        public Int32 P_Int32 { get; set; }
        public UInt32 P_UInt32 { get; set; }
        public Int64 P_Int64 { get; set; }
        public UInt64 P_UInt64 { get; set; }
        public Single P_Single { get; set; }
        public Double P_Double { get; set; }
        public Decimal P_Decimal { get; set; }
        public DateTime P_DateTime { get; set; }
        public String P_String { get; set; }
        public Guid P_Guid { get; set; }

        public Boolean? NP_Boolean { get; set; }
        public Char? NP_Char { get; set; }
        public SByte? NP_SByte { get; set; }
        public Byte? NP_Byte { get; set; }
        public Int16? NP_Int16 { get; set; }
        public UInt16? NP_UInt16 { get; set; }
        public Int32? NP_Int32 { get; set; }
        public UInt32? NP_UInt32 { get; set; }
        public Int64? NP_Int64 { get; set; }
        public UInt64? NP_UInt64 { get; set; }
        public Single? NP_Single { get; set; }
        public Double? NP_Double { get; set; }
        public Decimal? NP_Decimal { get; set; }
        public DateTime? NP_DateTime { get; set; }
        public Guid? NP_Guid { get; set; }

        public DemoEnum P_Enum { get; set; }
        public DemoEnum? NP_Enum { get; set; }
    }

    public class DataTypeStruct
    {
        public Boolean F_Boolean;
        public Char F_Char;
        public SByte F_SByte;
        public Byte F_Byte;
        public Int16 F_Int16;
        public UInt16 F_UInt16;
        public Int32 F_Int32;
        public UInt32 F_UInt32;
        public Int64 F_Int64;
        public UInt64 F_UInt64;
        public Single F_Single;
        public Double F_Double;
        public Decimal F_Decimal;
        public DateTime F_DateTime;
        public String F_String;
        public Guid F_Guid;

        public Boolean? NF_Boolean;
        public Char? NF_Char;
        public SByte? NF_SByte;
        public Byte? NF_Byte;
        public Int16? NF_Int16;
        public UInt16? NF_UInt16;
        public Int32? NF_Int32;
        public UInt32? NF_UInt32;
        public Int64? NF_Int64;
        public UInt64? NF_UInt64;
        public Single? NF_Single;
        public Double? NF_Double;
        public Decimal? NF_Decimal;
        public DateTime? NF_DateTime;
        public Guid? NF_Guid;

    }

}
