using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared
{
    public class StockCharts
    {
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Volume { get; set; }
    }

    public class StockChartsGet
    {
        public int ResultsCount { get; set; }
        public List<StockCharts> StockCharts { get; set; }
    }
}
