using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared
{
    public class StockChartsPost
    {
        public string IdCompany { get; set; }
        public DateTime DateTime { get; set; }
        public string Json { get; set; }
    }
}
