using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    /// <summary>
    /// Class to read 5 CsvColumns from a Csv document
    /// </summary>
    public sealed class CsvDataTypeObject5 : IEquatable<CsvDataTypeObject5>, ICloneable
    {
        private string[] _Backing =new string[5];

        [Column(Index = 0)]
        public string Field1 { get => _Backing[0]; set => _Backing[0] = value; }

        [Column(Index = 1)]
        public string Field2 { get => _Backing[1]; set => _Backing[1] = value; }

        [Column(Index = 2)]
        public string Field3 { get => _Backing[2]; set => _Backing[2] = value; }

        [Column(Index = 3)]
        public string Field4 { get => _Backing[3]; set => _Backing[3] = value; }

        [Column(Index = 4)]
        public string Field5 { get => _Backing[4]; set => _Backing[4] = value; }



        #region Decontructs

        public void Deconstruct(out string field1, out string field2, out string field3, out string field4, out string field5)
        {
            field1 = Field1;
            field2 = Field2;
            field3 = Field3;
            field4 = Field4;
            field5 = Field5;
        }

        public void Deconstruct(out string field1, out string field2, out string field3, out string field4)
        {
            field1 = Field1;
            field2 = Field2;
            field3 = Field3;
            field4 = Field4;
        }

        public void Deconstruct(out string field1, out string field2, out string field3)
        {
            field1 = Field1;
            field2 = Field2;
            field3 = Field3;
        }

        public void Deconstruct(out string field1, out string field2)
        {
            field1 = Field1;
            field2 = Field2;
        }

        public void Deconstruct(out string field1)
        {
            field1 = Field1;
        }


        #endregion

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public bool Equals(CsvDataTypeObject5 other)
        {
            if (Field1 == other.Field1 &&
                Field2 == other.Field2 &&
                Field3 == other.Field3 &&
                Field4 == other.Field4 &&
                Field5 == other.Field5)
            {
                return true;
            }
            return false;
        }
   
        public static bool operator == (CsvDataTypeObject5 left, CsvDataTypeObject5 right) => left.Equals(right);
        public static bool operator != (CsvDataTypeObject5 left, CsvDataTypeObject5 right) => !left.Equals(right);

        public static implicit operator CsvDataTypeObject5(string csv) => Deserialize(csv);

        public static implicit operator string(CsvDataTypeObject5 csv) => csv.Serialize();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CsvDataTypeObject5);
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
            return new CsvDataTypeObject5()
            {
                Field1 = this.Field1,
                Field2 = this.Field2,
                Field3 = this.Field3,
                Field4 = this.Field4,
                Field5 = this.Field5,
            };
        }

        /// <summary>
        /// Convert to Csv String
        /// </summary>
        public string Serialize()
        {
            return CsvUtils.ToCsvString(Field1, Field2, Field3, Field4, Field5);
        }

        /// <summary>
        /// Convert csv string to Object.
        /// </summary>
        public static CsvDataTypeObject5 Deserialize(string csv)
        {
            CsvDataTypeObject5 _new = new CsvDataTypeObject5();
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
