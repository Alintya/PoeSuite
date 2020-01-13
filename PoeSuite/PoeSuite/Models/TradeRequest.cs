using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Models
{
    class TradeRequest
    {
        string PlayerName;
        string ItemName;
        string StashTabName;
        System.Numerics.Vector<int> ItemPosition;
        int Price;
        string CurrencyName;

    }
}
