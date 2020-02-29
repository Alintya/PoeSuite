using PoeSuite.DataTypes;
using PoeSuite.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Features
{


    class ChatScanner
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


        }

        private void ParseFilterString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
