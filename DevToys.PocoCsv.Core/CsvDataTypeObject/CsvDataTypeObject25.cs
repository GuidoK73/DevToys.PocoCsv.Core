using System;
using System.Collections.Generic;
using System.Text;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    /// <summary>
    /// Class to read 25 CsvColumns from a Csv document
    /// </summary>
    public sealed class CsvDataTypeObject25 : IEquatable<CsvDataTypeObject25>, ICloneable
    {
        private string[] _Backing = new string[25];

        [Column(Index = 0)]
        public string Field01 { get => _Backing[0]; set => _Backing[0] = value; }

        [Column(Index = 1)]
        public string Field02 { get => _Backing[1]; set => _Backing[1] = value; }

        [Column(Index = 2)]
        public string Field03 { get => _Backing[2]; set => _Backing[2] = value; }

        [Column(Index = 3)]
        public string Field04 { get => _Backing[3]; set => _Backing[3] = value; }

        [Column(Index = 4)]
        public string Field05 { get => _Backing[4]; set => _Backing[4] = value; }

        [Column(Index = 5)]
        public string Field06 { get => _Backing[5]; set => _Backing[5] = value; }

        [Column(Index = 6)]
        public string Field07 { get => _Backing[6]; set => _Backing[6] = value; }

        [Column(Index = 7)]
        public string Field08 { get => _Backing[7]; set => _Backing[7] = value; }

        [Column(Index = 8)]
        public string Field09 { get => _Backing[8]; set => _Backing[8] = value; }

        [Column(Index = 9)]
        public string Field10 { get => _Backing[9]; set => _Backing[9] = value; }

        [Column(Index = 10)]
        public string Field11 { get => _Backing[10]; set => _Backing[10] = value; }

        [Column(Index = 11)]
        public string Field12 { get => _Backing[11]; set => _Backing[11] = value; }

        [Column(Index = 12)]
        public string Field13 { get => _Backing[12]; set => _Backing[12] = value; }

        [Column(Index = 13)]
        public string Field14 { get => _Backing[13]; set => _Backing[13] = value; }

        [Column(Index = 14)]
        public string Field15 { get => _Backing[14]; set => _Backing[14] = value; }

        [Column(Index = 15)]
        public string Field16 { get => _Backing[15]; set => _Backing[15] = value; }

        [Column(Index = 16)]
        public string Field17 { get => _Backing[16]; set => _Backing[16] = value; }

        [Column(Index = 17)]
        public string Field18 { get => _Backing[17]; set => _Backing[17] = value; }

        [Column(Index = 18)]
        public string Field19 { get => _Backing[18]; set => _Backing[18] = value; }

        [Column(Index = 19)]
        public string Field20 { get => _Backing[19]; set => _Backing[19] = value; }

        [Column(Index = 20)]
        public string Field21 { get => _Backing[20]; set => _Backing[20] = value; }

        [Column(Index = 21)]
        public string Field22 { get => _Backing[21]; set => _Backing[21] = value; }

        [Column(Index = 22)]
        public string Field23 { get => _Backing[22]; set => _Backing[22] = value; }

        [Column(Index = 23)]
        public string Field24 { get => _Backing[23]; set => _Backing[23] = value; }

        [Column(Index = 24)]
        public string Field25 { get => _Backing[24]; set => _Backing[24] = value; }

        #region Decontructs

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25
            )
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
            field21 = Field21;
            field22 = Field22;
            field23 = Field23;
            field24 = Field24;
            field25 = Field25;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24
            )
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
            field21 = Field21;
            field22 = Field22;
            field23 = Field23;
            field24 = Field24;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23
            )
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
            field21 = Field21;
            field22 = Field22;
            field23 = Field23;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22
    )
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
            field21 = Field21;
            field22 = Field22;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21
)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
            field21 = Field21;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
            field20 = Field20;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19
)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
            field19 = Field19;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18
)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
            field18 = Field18;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
    out string field06, out string field07, out string field08, out string field09, out string field10,
    out string field11, out string field12, out string field13, out string field14, out string field15,
    out string field16, out string field17
)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
            field17 = Field17;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16
)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
            field16 = Field16;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
            field15 = Field15;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
            field14 = Field14;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
            field13 = Field13;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
            field12 = Field12;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
            field11 = Field11;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
            field10 = Field10;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
             out string field06, out string field07, out string field08, out string field09)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
            field09 = Field09;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
            field08 = Field08;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
            field07 = Field07;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
            field06 = Field06;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
        }

        public void Deconstruct(out string field01, out string field02, out string field03, out string field04)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
        }

        public void Deconstruct(out string field01, out string field02, out string field03)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
        }

        public void Deconstruct(out string field01, out string field02)
        {
            field01 = Field01;
            field02 = Field02;
        }

        public void Deconstruct(out string field01)
        {
            field01 = Field01;
        }

        #endregion

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public bool Equals(CsvDataTypeObject25 other)
        {
            if (Field01 == other.Field01 &&
                Field02 == other.Field02 &&
                Field03 == other.Field03 &&
                Field04 == other.Field04 &&
                Field05 == other.Field05 &&
                Field06 == other.Field06 &&
                Field07 == other.Field07 &&
                Field08 == other.Field08 &&
                Field09 == other.Field09 &&
                Field10 == other.Field10 &&
                Field11 == other.Field11 &&
                Field12 == other.Field12 &&
                Field13 == other.Field13 &&
                Field14 == other.Field14 &&
                Field15 == other.Field15 &&
                Field16 == other.Field16 &&
                Field17 == other.Field17 &&
                Field18 == other.Field18 &&
                Field19 == other.Field19 &&
                Field20 == other.Field20 &&
                Field21 == other.Field21 &&
                Field22 == other.Field22 &&
                Field23 == other.Field23 &&
                Field24 == other.Field24 &&
                Field25 == other.Field25)
            {
                return true;
            }
            return false;
        }

        public static bool operator ==(CsvDataTypeObject25 left, CsvDataTypeObject25 right) => left.Equals(right);
        public static bool operator !=(CsvDataTypeObject25 left, CsvDataTypeObject25 right) => !left.Equals(right);

        public static implicit operator CsvDataTypeObject25(string csv) => Deserialize(csv);

        public static implicit operator string(CsvDataTypeObject25 csv) => csv.Serialize();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CsvDataTypeObject25);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            return Serialize().GetHashCode();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public object Clone()
        {
            return new CsvDataTypeObject25()
            {
                Field01 = this.Field01,
                Field02 = this.Field02,
                Field03 = this.Field03,
                Field04 = this.Field04,
                Field05 = this.Field05,
                Field06 = this.Field06,
                Field07 = this.Field07,
                Field08 = this.Field08,
                Field09 = this.Field09,
                Field10 = this.Field10,
                Field11 = this.Field11,
                Field12 = this.Field12,
                Field13 = this.Field13,
                Field14 = this.Field14,
                Field15 = this.Field15,
                Field16 = this.Field16,
                Field17 = this.Field17,
                Field18 = this.Field18,
                Field19 = this.Field19,
                Field20 = this.Field20,
                Field21 = this.Field21,
                Field22 = this.Field22,
                Field23 = this.Field23,
                Field24 = this.Field24,
                Field25 = this.Field25
            };
        }

        /// <summary>
        /// Convert to Csv String
        /// </summary>
        public string Serialize()
        {
            return CsvUtils.ToCsvString(Field01, Field02, Field03, Field04, Field05, Field06, Field07, Field08, Field09, Field10, Field11, Field12, Field13, Field14, Field15, Field16, Field17, Field18, Field19, Field20, Field21, Field22, Field23, Field24, Field25);
        }

        /// <summary>
        /// Convert csv string to Object.
        /// </summary>
        public static CsvDataTypeObject25 Deserialize(string csv)
        {
            CsvDataTypeObject25 _new = new CsvDataTypeObject25();
            string[] _items = CsvUtils.SplitCsvString(csv, ',');
            int _top = CsvUtils.Lowest(_items.Length, _new._Backing.Length);
            for (int ii = 0; ii < _top; ii++)
            {
                _new._Backing[ii] = _items[ii];
            }
            return _new;
        }
    }
}
