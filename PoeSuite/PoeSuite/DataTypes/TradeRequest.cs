using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.DataTypes
{
    public class TradeRequest
    {
        public string PlayerName { get; set; }
        public string ItemName { get; set; }
        public string StashTabName { get; set; }
        public System.Numerics.Vector<int> ItemPosition { get; set; }
        public int Price { get; set; }
        public string CurrencyName { get; set; }

    }
}
