using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject2
{
    public class ParseDateTimeUTC : ICustomCsvParse<DateTime>
    {
        private CultureInfo _culture;

        public ParseDateTimeUTC()
        {
            _culture = CultureInfo.GetCultureInfo("en-US");
        }

        public DateTime Read(StringBuilder value)
        {
            bool succes = DateTime.TryParse(value.ToString().Replace("UTC", ""), _culture, DateTimeStyles.None, out DateTime _value);
            if (succes)
            {
                return _value;
            }
            return DateTime.MinValue;
        }
        public string Write(DateTime value) => throw new NotImplementedException();
    }

    public class CsvTransactions
    {
        [Column(Index = 0, CustomParserType = typeof(ParseDateTimeUTC))]
        public DateTime Date { get; set; }
        [Column(Index = 1)]
        public string TransactionID { get; set; }
        [Column(Index = 2)]
        public string Type { get; set; }
        [Column(Index = 3)]
        public string ReceivedFrom { get; set; }
        [Column(Index = 4)]
        public Decimal ReceivedAmount { get; set; }
        [Column(Index = 5)]
        public string ReceivedCurrency { get; set; }
        [Column(Index = 6)]
        public Decimal SentAmount { get; set; }
        [Column(Index = 7)]
        public string SentCurrency { get; set; }
        [Column(Index = 8)]
        public Decimal FeeAmount { get; set; }
        [Column(Index = 9)]
        public string FeeCurrency { get; set; }

        //[Column(Index = 10)]
        //public string EmptyColumn { get; set; }

    }


    [TestClass]
    public class TestX
    {
        [TestMethod()]
        public void Testing()
        {
            StringBuilder _sb = new StringBuilder();

            string file = @"C:\Users\guido\Downloads\transactions.csv";

            using (CsvReader<CsvTransactions> _reader = new(file))
            {
                _reader.Culture = CultureInfo.GetCultureInfo("en-us");
                _reader.Open();
                _reader.Separator = ','; // or use _reader.DetectSeparator(); 
                _reader.Skip();
                var _data = _reader.ReadAsEnumerable().Where(p => p.ReceivedCurrency.StartsWith("DRIP")).ToList();

                foreach (var _item in _data)
                {
                    _sb.AppendLine($"{_item.Date} - {_item.ReceivedAmount}");
                }

            }

        }

    }
}
