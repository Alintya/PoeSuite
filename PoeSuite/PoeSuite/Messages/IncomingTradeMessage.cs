using PoeSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Messages
{
    class IncomingTradeMessage
    {
        public TradeRequest Request { get; private set; }

        public IncomingTradeMessage(TradeRequest request)
        {
            Request = request;
        }
    }
}
