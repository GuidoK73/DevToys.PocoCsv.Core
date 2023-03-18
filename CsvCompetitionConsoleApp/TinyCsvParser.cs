using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Objects;
using TinyCsvParser.Mapping;

namespace ConsoleApp1
{
    public class CsvPersonMapping : CsvMapping<CsvObject>
    {
        public CsvPersonMapping()
            : base()
        {
            MapProperty(5, x => x.AfBij);
            MapProperty(6, x => x.Bedrag);
            MapProperty(4, x => x.Code);
            MapProperty(0, x => x.Datum);
            MapProperty(8, x => x.Mededelingen);
            MapProperty(7, x => x.MutatieSoort);
            MapProperty(1, x => x.NaamOmschrijving);
            MapProperty(2, x => x.Rekening);
            MapProperty(3, x => x.Tegenrekening);

        }
    }
}
