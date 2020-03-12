using PoeSuite.Models;

namespace PoeSuite.Messages
{
    internal class IncomingTradeMessage
    {
        public TradeRequest Request { get; }

        public IncomingTradeMessage(TradeRequest request)
        {
            Request = request;
        }
    }
}
