using PoeSuite.DataTypes.Enums;
using PoeSuite.DataTypes;

using System;

namespace PoeSuite.Features
{
    internal class ChatScanner
    {
        private ChatMessageChannel _channelFilter = ChatMessageChannel.None;
        private IObservable<string> _searchWords;
        private IObservable<string> _badWords;

        public string FilterString
        {
            get
            {
                return FilterString;
            }
            set
            {
                ParseFilterString(value);
                FilterString = value;
            }
        }

        public ChatScanner()
        {

        }

        public void OnChatMessage(ChatMessage message)
        {
            if (!_channelFilter.HasFlag(message.Channel))
                return;

            // TODO
        }

        private void ParseFilterString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
