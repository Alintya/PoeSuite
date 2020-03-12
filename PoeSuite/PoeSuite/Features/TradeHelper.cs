using PoeSuite.DataTypes;
using PoeSuite.Models;

using GalaSoft.MvvmLight.Messaging;

using System.Text.RegularExpressions;

namespace PoeSuite.Features
{
    internal class TradeHelper
    {
        // regex
        // "@\\w+ (.+) [\\w\\s,]+ your ([0-9]*) *(.+) \\w* [for |my ]*([0-9]+) (\\w+) \\w+ (.+)( \\([\\w\\s]+\"(.+)\"; \\w+: \\w+ ([0-9]+), \\w+ ([0-9]+)\\))*"
        private readonly Regex _tradeRequestRegex = new Regex("[\\w\\s,]+ your ([0-9]*) *(.+) \\w* [for |my ]*([0-9]+) (\\w+) \\w+ (\\w+ \\w+) \\(.*\"(.+)\";\\D*(\\d+)\\D*(\\d+)");
        // matches [amount] > item/currency name > offering amount > offering currency > league > stash tab name > stash x pos > stash y pos

        public TradeHelper()
        {
            //Messenger.Default.
        }

        public void OnChatMessage(ChatMessage msg)
        {
            if (msg.Channel != DataTypes.Enums.ChatMessageChannel.Private)
                return;

            var match = _tradeRequestRegex.Match(msg.Message);
            if (!match.Success)
                return;

            // TODO
            bool isCurrencyTrade = false;

            TradeRequest request = new TradeRequest
            {
                PlayerName = msg.Sender,
                IsOutgoing = msg.Message.StartsWith("@To"),
                IsCurrencyExchange = isCurrencyTrade,

                ItemName = match.Groups[2].Value,
                Price = int.Parse(match.Groups[3].Value),
                CurrencyName = match.Groups[4].Value,
                StashTabName = match.Groups[6].Value,
                ItemPosition = new System.Drawing.Point(int.Parse(match.Groups[7].Value), int.Parse(match.Groups[8].Value))
            };

            Messenger.Default.Send(new Messages.IncomingTradeMessage(request));
        }
    }
}
