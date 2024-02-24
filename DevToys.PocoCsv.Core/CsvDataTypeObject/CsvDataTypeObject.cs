using System;
using System.Collections.Generic;
using System.Text;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    /// <summary>
    /// Class to read 50 CsvColumns from a Csv document
    /// </summary>
    public sealed class CsvDataTypeObject : IEquatable<CsvDataTypeObject>, ICloneable
    {
        private string[] _Backing = new string[50];

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

        [Column(Index = 25)]
        public string Field26 { get => _Backing[25]; set => _Backing[25] = value; }

        [Column(Index = 26)]
        public string Field27 { get => _Backing[26]; set => _Backing[26] = value; }

        [Column(Index = 27)]
        public string Field28 { get => _Backing[27]; set => _Backing[27] = value; }

        [Column(Index = 28)]
        public string Field29 { get => _Backing[28]; set => _Backing[28] = value; }

        [Column(Index = 29)]
        public string Field30 { get => _Backing[29]; set => _Backing[29] = value; }

        [Column(Index = 30)]
        public string Field31 { get => _Backing[30]; set => _Backing[30] = value; }

        [Column(Index = 31)]
        public string Field32 { get => _Backing[31]; set => _Backing[31] = value; }

        [Column(Index = 32)]
        public string Field33 { get => _Backing[32]; set => _Backing[32] = value; }

        [Column(Index = 33)]
        public string Field34 { get => _Backing[33]; set => _Backing[33] = value; }

        [Column(Index = 34)]
        public string Field35 { get => _Backing[34]; set => _Backing[34] = value; }

        [Column(Index = 35)]
        public string Field36 { get => _Backing[35]; set => _Backing[35] = value; }

        [Column(Index = 36)]
        public string Field37 { get => _Backing[36]; set => _Backing[36] = value; }

        [Column(Index = 37)]
        public string Field38 { get => _Backing[37]; set => _Backing[37] = value; }

        [Column(Index = 38)]
        public string Field39 { get => _Backing[38]; set => _Backing[38] = value; }

        [Column(Index = 39)]
        public string Field40 { get => _Backing[39]; set => _Backing[39] = value; }

        [Column(Index = 40)]
        public string Field41 { get => _Backing[40]; set => _Backing[40] = value; }

        [Column(Index = 41)]
        public string Field42 { get => _Backing[41]; set => _Backing[41] = value; }

        [Column(Index = 42)]
        public string Field43 { get => _Backing[42]; set => _Backing[42] = value; }

        [Column(Index = 43)]
        public string Field44 { get => _Backing[43]; set => _Backing[43] = value; }

        [Column(Index = 44)]
        public string Field45 { get => _Backing[44]; set => _Backing[44] = value; }

        [Column(Index = 45)]
        public string Field46 { get => _Backing[45]; set => _Backing[45] = value; }

        [Column(Index = 46)]
        public string Field47 { get => _Backing[46]; set => _Backing[46] = value; }

        [Column(Index = 47)]
        public string Field48 { get => _Backing[47]; set => _Backing[47] = value; }

        [Column(Index = 48)]
        public string Field49 { get => _Backing[48]; set => _Backing[48] = value; }

        [Column(Index = 49)]
        public string Field50 { get => _Backing[49]; set => _Backing[49] = value; }

        #region Decontructs

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
        public bool Equals(CsvDataTypeObject other)
        {
            for (int ii = 0; ii < _Backing.Length; ii++)
            {
                if (_Backing[ii] != other._Backing[ii])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator ==(CsvDataTypeObject left, CsvDataTypeObject right) => left.Equals(right);
        public static bool operator !=(CsvDataTypeObject left, CsvDataTypeObject right) => !left.Equals(right);

        public static implicit operator CsvDataTypeObject(string csv) => Deserialize(csv);

        public static implicit operator string(CsvDataTypeObject csv) => csv.Serialize();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CsvDataTypeObject);
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
            var _new = new CsvDataTypeObject();
            for (int ii = 0; ii < _Backing.Length; ii++)
            {
                _new._Backing[ii] = this._Backing[ii];
            }
            return _new;
        }

        /// <summary>
        /// Convert to Csv String
        /// </summary>
        public string Serialize()
        {
            return CsvUtils.ToCsvString(_Backing);
        }

        /// <summary>
        /// Convert csv string to Object.
        /// </summary>
        public static CsvDataTypeObject Deserialize(string csv)
        {
            CsvDataTypeObject _new = new CsvDataTypeObject();
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
