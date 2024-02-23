using System;
using System.Collections.Generic;
using System.Text;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    /// <summary>
    /// Class to read 10 CsvColumns from a Csv document
    /// </summary>
    public sealed class CsvDataTypeObject10 : IEquatable<CsvDataTypeObject10>, ICloneable 
    {
        private string[] _Backing = new string[10];

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

        #region Decontructs

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
        public bool Equals(CsvDataTypeObject10 other)
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
                Field10 == other.Field10)
            {
                return true;
            }
            return false;
        }

        public static bool operator ==(CsvDataTypeObject10 left, CsvDataTypeObject10 right) => left.Equals(right);
        public static bool operator !=(CsvDataTypeObject10 left, CsvDataTypeObject10 right) => !left.Equals(right);

        public static implicit operator CsvDataTypeObject10(string csv) => Deserialize(csv);

        public static implicit operator string(CsvDataTypeObject10 csv) => csv.Serialize();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CsvDataTypeObject10);
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
            return new CsvDataTypeObject10()
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
                Field10 = this.Field10
            };
        }

        /// <summary>
        /// Convert to Csv String
        /// </summary>
        public string Serialize()
        {
            return CsvUtils.ToCsvString(Field01, Field02, Field03, Field04, Field05, Field06, Field07, Field08, Field09, Field10);
        }

        /// <summary>
        /// Convert csv string to Object.
        /// </summary>
        public static CsvDataTypeObject10 Deserialize(string csv)
        {
            CsvDataTypeObject10 _new = new CsvDataTypeObject10();
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
