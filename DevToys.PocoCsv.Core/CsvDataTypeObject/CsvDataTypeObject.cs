using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    /// <summary>
    /// Convenience class to read up to 50 CsvColumns from a Csv document
    /// </summary>
    public sealed class CsvDataTypeObject : IEquatable<CsvDataTypeObject>, IComparable<CsvDataTypeObject>, ICloneable, ISerializable
    {
        private string[] _Backing = new string[50];
        private int _Count = 0;

        public CsvDataTypeObject() 
        { }

        public CsvDataTypeObject(string csv)
        {
            string[] _items = CsvUtils.SplitCsvLine(csv, ',');
            for (int ii = 0; ii < _items.Length; ii++)
            {
                _Backing[ii] = _items[ii];
                _Count = ii;
            }
            _Count++;
        }

        #region Field Properties

        /// <summary>Column 1</summary>
        [Column(Index = 0)]
        public string Field01 { get => GetBacking(0); set => SetBacking(0, value); }

        /// <summary>Column 2</summary>
        [Column(Index = 1)]
        public string Field02 { get => GetBacking(1); set => SetBacking(1, value); }

        /// <summary>Column 3</summary>
        [Column(Index = 2)]
        public string Field03 { get => GetBacking(2); set => SetBacking(2, value); }

        /// <summary>Column 4</summary>
        [Column(Index = 3)]
        public string Field04 { get => GetBacking(3); set => SetBacking(3, value); }

        /// <summary>Column 5</summary>
        [Column(Index = 4)]
        public string Field05 { get => GetBacking(4); set => SetBacking(4, value); }

        /// <summary>Column 6</summary>
        [Column(Index = 5)]
        public string Field06 { get => GetBacking(5); set => SetBacking(5, value); }

        /// <summary>Column 7</summary>
        [Column(Index = 6)]
        public string Field07 { get => GetBacking(6); set => SetBacking(6, value); }

        /// <summary>Column 8</summary>
        [Column(Index = 7)]
        public string Field08 { get => GetBacking(7); set => SetBacking(7, value); }

        /// <summary>Column 9</summary>
        [Column(Index = 8)]
        public string Field09 { get => GetBacking(8); set => SetBacking(8, value); }

        /// <summary>Column 10</summary>
        [Column(Index = 9)]
        public string Field10 { get => GetBacking(9); set => SetBacking(9, value); }

        /// <summary>Column 11</summary>
        [Column(Index = 10)]
        public string Field11 { get => GetBacking(10); set => SetBacking(10, value); }

        /// <summary>Column 12</summary>
        [Column(Index = 11)]
        public string Field12 { get => GetBacking(11); set => SetBacking(11, value); }

        /// <summary>Column 13</summary>
        [Column(Index = 12)]
        public string Field13 { get => GetBacking(12); set => SetBacking(12, value); }

        /// <summary>Column 14</summary>
        [Column(Index = 13)]
        public string Field14 { get => GetBacking(13); set => SetBacking(13, value); }

        /// <summary>Column 15</summary>
        [Column(Index = 14)]
        public string Field15 { get => GetBacking(14); set => SetBacking(14, value); }

        /// <summary>Column 16</summary>
        [Column(Index = 15)]
        public string Field16 { get => GetBacking(15); set => SetBacking(15, value); }

        /// <summary>Column 17</summary>
        [Column(Index = 16)]
        public string Field17 { get => GetBacking(16); set => SetBacking(16, value); }

        /// <summary>Column 18</summary>
        [Column(Index = 17)]
        public string Field18 { get => GetBacking(17); set => SetBacking(17, value); }

        /// <summary>Column 19</summary>
        [Column(Index = 18)]
        public string Field19 { get => GetBacking(18); set => SetBacking(18, value); }

        /// <summary>Column 20</summary>
        [Column(Index = 19)]
        public string Field20 { get => GetBacking(19); set => SetBacking(19, value); }

        /// <summary>Column 21</summary>
        [Column(Index = 20)]
        public string Field21 { get => GetBacking(20); set => SetBacking(20, value); }

        /// <summary>Column 22</summary>
        [Column(Index = 21)]
        public string Field22 { get => GetBacking(21); set => SetBacking(21, value); }

        /// <summary>Column 23</summary>
        [Column(Index = 22)]
        public string Field23 { get => GetBacking(22); set => SetBacking(22, value); }

        /// <summary>Column 24</summary>
        [Column(Index = 23)]
        public string Field24 { get => GetBacking(23); set => SetBacking(23, value); }

        /// <summary>Column 25</summary>
        [Column(Index = 24)]
        public string Field25 { get => GetBacking(24); set => SetBacking(24, value); }

        /// <summary>Column 26</summary>
        [Column(Index = 25)]
        public string Field26 { get => GetBacking(25); set => SetBacking(25, value); }

        /// <summary>Column 27</summary>
        [Column(Index = 26)]
        public string Field27 { get => GetBacking(26); set => SetBacking(26, value); }

        /// <summary>Column 28</summary>
        [Column(Index = 27)]
        public string Field28 { get => GetBacking(27); set => SetBacking(27, value); }

        /// <summary>Column 29</summary>
        [Column(Index = 28)]
        public string Field29 { get => GetBacking(28); set => SetBacking(28, value); }

        /// <summary>Column 30</summary>
        [Column(Index = 29)]
        public string Field30 { get => GetBacking(29); set => SetBacking(29, value); }

        /// <summary>Column 31</summary>
        [Column(Index = 30)]
        public string Field31 { get => GetBacking(30); set => SetBacking(30, value); }

        /// <summary>Column 32</summary>
        [Column(Index = 31)]
        public string Field32 { get => GetBacking(31); set => SetBacking(31, value); }

        /// <summary>Column 33</summary>
        [Column(Index = 32)]
        public string Field33 { get => GetBacking(32); set => SetBacking(32, value); }

        /// <summary>Column 34</summary>
        [Column(Index = 33)]
        public string Field34 { get => GetBacking(33); set => SetBacking(33, value); }

        /// <summary>Column 35</summary>
        [Column(Index = 34)]
        public string Field35 { get => GetBacking(34); set => SetBacking(34, value); }

        /// <summary>Column 36</summary>
        [Column(Index = 35)]
        public string Field36 { get => GetBacking(35); set => SetBacking(35, value); }

        /// <summary>Column 37</summary>
        [Column(Index = 36)]
        public string Field37 { get => GetBacking(36); set => SetBacking(36, value); }

        /// <summary>Column 38</summary>
        [Column(Index = 37)]
        public string Field38 { get => GetBacking(37); set => SetBacking(37, value); }

        /// <summary>Column 39</summary>
        [Column(Index = 38)]
        public string Field39 { get => GetBacking(38); set => SetBacking(38, value); }

        /// <summary>Column 40</summary>
        [Column(Index = 39)]
        public string Field40 { get => GetBacking(39); set => SetBacking(39, value); }

        /// <summary>Column 41</summary>
        [Column(Index = 40)]
        public string Field41 { get => GetBacking(40); set => SetBacking(40, value); }

        /// <summary>Column 42</summary>
        [Column(Index = 41)]
        public string Field42 { get => GetBacking(41); set => SetBacking(41, value); }

        /// <summary>Column 43</summary>
        [Column(Index = 42)]
        public string Field43 { get => GetBacking(42); set => SetBacking(42, value); }

        /// <summary>Column 44</summary>
        [Column(Index = 43)]
        public string Field44 { get => GetBacking(43); set => SetBacking(43, value); }

        /// <summary>Column 45</summary>
        [Column(Index = 44)]
        public string Field45 { get => GetBacking(44); set => SetBacking(44, value); }

        /// <summary>Column 46</summary>
        [Column(Index = 45)]
        public string Field46 { get => GetBacking(45); set => SetBacking(45, value); }

        /// <summary>Column 47</summary>
        [Column(Index = 46)]
        public string Field47 { get => GetBacking(46); set => SetBacking(46, value); }

        /// <summary>Column 48</summary>
        [Column(Index = 47)]
        public string Field48 { get => GetBacking(47); set => SetBacking(47, value); }

        /// <summary>Column 49</summary>
        [Column(Index = 48)]
        public string Field49 { get => GetBacking(48); set => SetBacking(48, value); }

        /// <summary>Column 50</summary>
        [Column(Index = 49)]
        public string Field50 { get => GetBacking(49); set => SetBacking(49, value); }

        #endregion

        private string GetBacking(int index)
        {
            if (index > _Count)
            {
                return null;
            }
            return _Backing[index];
        }

        private void SetBacking(int index, string value)
        {
            if (index > _Count - 1)
            {
                _Count = index + 1;
            }
            _Backing[index] = value;
        }

        #region Decontructs

        /// <summary>Deconstruct 50</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45,
            out string field46, out string field47, out string field48, out string field49, out string field50
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
            field46 = Field46;
            field47 = Field47;
            field48 = Field48;
            field49 = Field49;
            field50 = Field50;
        }

        /// <summary>Deconstruct 49</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45,
            out string field46, out string field47, out string field48, out string field49
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
            field46 = Field46;
            field47 = Field47;
            field48 = Field48;
            field49 = Field49;
        }

        /// <summary>Deconstruct 48</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45,
            out string field46, out string field47, out string field48
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
            field46 = Field46;
            field47 = Field47;
            field48 = Field48;
        }

        /// <summary>Deconstruct 47</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45,
            out string field46, out string field47
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
            field46 = Field46;
            field47 = Field47;
        }

        /// <summary>Deconstruct 46</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45,
            out string field46
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
            field46 = Field46;
        }

        /// <summary>Deconstruct 45</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44, out string field45
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
            field45 = Field45;
        }

        /// <summary>Deconstruct 44</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43, out string field44
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
            field44 = Field44;
        }

        /// <summary>Deconstruct 43</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42, out string field43
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
            field43 = Field43;
        }

        /// <summary>Deconstruct 42</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41, out string field42
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
            field42 = Field42;
        }

        /// <summary>Deconstruct 41</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40,
            out string field41
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
            field41 = Field41;
        }

        /// <summary>Deconstruct 40</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39, out string field40
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
            field40 = Field40;
        }

        /// <summary>Deconstruct 39</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38, out string field39
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
            field39 = Field39;
        }

        /// <summary>Deconstruct 38</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37, out string field38
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
            field38 = Field38;
        }

        /// <summary>Deconstruct 37</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36, out string field37
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
            field37 = Field37;
        }

        /// <summary>Deconstruct 36</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35,
            out string field36
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
            field36 = Field36;
        }

        /// <summary>Deconstruct 35</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34, out string field35
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
            field35 = Field35;
        }

        /// <summary>Deconstruct 34</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33, out string field34
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
            field34 = Field34;
        }

        /// <summary>Deconstruct 33</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32, out string field33
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
            field33 = Field33;
        }

        /// <summary>Deconstruct 32</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31, out string field32
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
            field32 = Field32;
        }

        /// <summary>Deconstruct 31</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30,
            out string field31
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
            field31 = Field31;
        }

        /// <summary>Deconstruct 30</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29, out string field30
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
            field30 = Field30;
        }

        /// <summary>Deconstruct 29</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28, out string field29
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
            field29 = Field29;
        }

        /// <summary>Deconstruct 28</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27, out string field28
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
            field26 = Field26;
            field27 = Field27;
            field28 = Field28;
        }

        /// <summary>Deconstruct 27</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26, out string field27
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
            field26 = Field26;
            field27 = Field27;
        }

        /// <summary>Deconstruct 26</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05,
            out string field06, out string field07, out string field08, out string field09, out string field10,
            out string field11, out string field12, out string field13, out string field14, out string field15,
            out string field16, out string field17, out string field18, out string field19, out string field20,
            out string field21, out string field22, out string field23, out string field24, out string field25,
            out string field26
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
            field26 = Field26;
        }

        /// <summary>Deconstruct 25</summary>
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

        /// <summary>Deconstruct 24</summary>
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

        /// <summary>Deconstruct 23</summary>
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

        /// <summary>Deconstruct 22</summary>
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

        /// <summary>Deconstruct 21</summary>
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

        /// <summary>Deconstruct 20</summary>
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

        /// <summary>Deconstruct 19</summary>
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

        /// <summary>Deconstruct 18</summary>
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

        /// <summary>Deconstruct 17</summary>
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

        /// <summary>Deconstruct 16</summary>
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

        /// <summary>Deconstruct 15</summary>
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

        /// <summary>Deconstruct 14</summary>
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

        /// <summary>Deconstruct 13</summary>
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

        /// <summary>Deconstruct 12</summary>
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

        /// <summary>Deconstruct 11</summary>
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

        /// <summary>Deconstruct 10</summary>
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

        /// <summary>Deconstruct 9</summary>
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

        /// <summary>Deconstruct 8</summary>
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

        /// <summary>Deconstruct 7</summary>
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

        /// <summary>Deconstruct 6</summary>
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

        /// <summary>Deconstruct 5</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04, out string field05)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
            field05 = Field05;
        }

        /// <summary>Deconstruct 4</summary>
        public void Deconstruct(out string field01, out string field02, out string field03, out string field04)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
            field04 = Field04;
        }

        /// <summary>Deconstruct 3</summary>
        public void Deconstruct(out string field01, out string field02, out string field03)
        {
            field01 = Field01;
            field02 = Field02;
            field03 = Field03;
        }

        /// <summary>Deconstruct 2</summary>
        public void Deconstruct(out string field01, out string field02)
        {
            field01 = Field01;
            field02 = Field02;
        }

        /// <summary>Deconstruct 1</summary>
        public void Deconstruct(out string field01)
        {
            field01 = Field01;
        }

        #endregion

        #region Operators

        /// <summary>Equal operator ==</summary>
        public static bool operator ==(CsvDataTypeObject left, CsvDataTypeObject right) => Equals(left, right);

        /// <summary>Not equal operator !=</summary>
        public static bool operator !=(CsvDataTypeObject left, CsvDataTypeObject right) => !Equals(left, right);

        /// <summary>Smaller then</summary>
        public static bool operator <(CsvDataTypeObject csv1, CsvDataTypeObject csv2) => CompareTo(csv1, csv2) < 0;

        /// <summary>Larger then</summary>
        public static bool operator >(CsvDataTypeObject csv1, CsvDataTypeObject csv2) => CompareTo(csv1, csv2) > 0;

        /// <summary>Implicit cast from string to CsvDataTypeObject</summary>
        public static implicit operator CsvDataTypeObject(string csv) => Deserialize(csv);

        /// <summary>Implicit cast from CsvDataTypeObject to string</summary>
        public static implicit operator string(CsvDataTypeObject csv) => csv.Serialize();

        private static bool Equals(CsvDataTypeObject left, CsvDataTypeObject right)
        {
            if (object.ReferenceEquals(left , null) && object.ReferenceEquals(right, null))
            {
                return true;
            }
            if (!object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
            {
                return false;
            }
            if (object.ReferenceEquals(left, null) && !object.ReferenceEquals(right, null))
            {
                return false;
            }
            return left.Equals(right);
        }

        private static int CompareTo(CsvDataTypeObject left, CsvDataTypeObject right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
            {
                return 0;
            }
            if (!object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
            {
                return 1;
            }
            if (object.ReferenceEquals(left, null) && !object.ReferenceEquals(right, null))
            {
                return -1;
            }
            return left.CompareTo(right);
        }

        #endregion

        /// <summary>
        /// Convert to Csv String
        /// </summary>
        public string Serialize() => CsvUtils.JoinCsvLine(_Count, _Backing);
 

        /// <summary>
        /// Convert csv string to Object.
        /// </summary>
        public static CsvDataTypeObject Deserialize(string csv)
        {
            CsvDataTypeObject _new = new CsvDataTypeObject();
            string[] _items = CsvUtils.SplitCsvLine(csv, ',');
            int _top = CsvUtils.Lowest(_items.Length, _new._Backing.Length);
            for (int ii = 0; ii < _top; ii++)
            {
                _new._Backing[ii] = _items[ii];
                _new._Count = ii + 1;
            }
            return _new;
        }

        #region Interface Implementations

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public bool Equals(CsvDataTypeObject other)
        {
            if (other == null)
            {
                return false;
            }
            for (int ii = 0; ii < _Backing.Length; ii++)
            {
                if (_Backing[ii] != other._Backing[ii])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public override bool Equals(object obj) => Equals(obj as CsvDataTypeObject);


        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => Serialize().GetHashCode();


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public object Clone()
        {
            var _new = new CsvDataTypeObject();
            for (int ii = 0; ii < _Backing.Length; ii++)
            {
                _new._Backing[ii] = this._Backing[ii];                
            }
            _new._Count = _Backing.Length;
            return _new;
        }

        /// <summary>
        /// ISerializable implementation.
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            for (int ii = 0; ii < _Count; ii++)
            {
                info.AddValue($"Field{string.Format("{0:00}", ii + 1)}", _Backing[ii]);
            }
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The
        /// return value has these meanings: Value Meaning Less than zero This instance precedes
        /// other in the sort order. Zero This instance occurs in the same position in the
        /// sort order as other. Greater than zero This instance follows other in the sort
        /// order.
        /// </returns>
        public int CompareTo([AllowNull]CsvDataTypeObject other)
        {
            if (other == null)
            {
                return 1;
            }
            for (int ii = 0; ii < _Backing.Length; ii++)
            {
                if (_Backing[ii] == null && other._Backing[ii] == null)
                {
                    continue;
                }
                if (_Backing[ii] == null && other._Backing[ii] != null)
                {
                    return -1;
                }
                if (_Backing[ii] != null && other._Backing[ii] == null)
                {
                    return 1;
                }
                if (_Backing[ii] == other._Backing[ii])
                {
                    continue;
                }
                return _Backing[ii].CompareTo(other._Backing[ii]);
            }
            return 0;
        }

        #endregion
    }
}
