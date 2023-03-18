using ConsoleApp1.Objects;
using Csv;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class LumenWorksCsvReaderHelper
    {
        public IEnumerable<CsvObject> ReadCsvItems(CachedCsvReader csv)
        {
            foreach (string[] line  in csv)
            {
                // Header is handled, each line will contain the actual row data
                CsvObject _item = new CsvObject();
                _item.AfBij = line[5];
                _item.Bedrag = line[6];
                _item.Code = line[4];
                _item.Datum = line[0];
                _item.Mededelingen = line[8];
                _item.MutatieSoort = line[7];
                _item.NaamOmschrijving = line[1];
                _item.Rekening = line[2];
                _item.Tegenrekening = line[3];
                yield return _item;
            }
  
        }
    }
}
