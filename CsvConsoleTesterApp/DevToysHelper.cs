using ConsoleApp1.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class DevToysHelper
    {
        public IEnumerable<CsvObject> LargeData()
        {
            for (int ii = 0; ii < 10000000; ii++)
            {
                CsvObject _line = new()
                {
                    AfBij = "bij",
                    Bedrag = "100",
                    Code = "test",
                    Datum = "2020/02/03",
                    Mededelingen = $"test {ii}",
                    Rekening = "3434",
                    Tegenrekening = "3423424",
                    NaamOmschrijving = $"bla, bla {ii}",
                    MutatieSoort = "Bij"

                };
                yield return _line;
            }
        }
    }
}
