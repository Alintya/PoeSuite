using PoeSuite.DataTypes.Enums;

namespace PoeSuite.DataTypes
{
    internal class ChatMessage
    {
        private string _raw;
        private ChatMessageChannel _channel;

        public string Sender { get; set; }
        public string Message { get; set; }

        public ChatMessageChannel Channel
        {
            set
            {
                if (value == _channel)
                    return;

                _channel = value;
            }
            get
            {
                if (_channel == ChatMessageChannel.None && !string.IsNullOrEmpty(_raw))
                    _channel = GetMessageChannel(this.Message);

                return _channel;
            }
        }

        public ChatMessage()
        {

        }

        public ChatMessage(string raw)
        {
            // TODO: apply regex?
            _raw = raw;
        }

        public ChatMessage(ChatMessageChannel channel, string sender, string msg)
        {
            Channel = channel;
            Sender = sender;
            Message = msg;
        }

        public string GetPrefix => ChannelToPrefix(_channel);

        public static ChatMessageChannel GetMessageChannel(string message)
        {
            ChatMessageChannel ch = ChatMessageChannel.None;

            switch (message[0])
            {
                case '@':
                    ch = ChatMessageChannel.Private;
                    break;
                case '#':
                    ch = ChatMessageChannel.Global;
                    break;
                case '%':
                    ch = ChatMessageChannel.Party;
                    break;
                case '$':
                    ch = ChatMessageChannel.Trade;
                    break;
                case '&':
                    ch = ChatMessageChannel.Guild;
                    break;
                default:
                    // TODO: system or local
                    break;
            }

            return ch;
        }

        public static string ChannelToPrefix(ChatMessageChannel ch)
        {
            string prefix = string.Empty;

            switch (ch)
            {
                case ChatMessageChannel.Private:
                    prefix = "@";
                    break;
                case ChatMessageChannel.Global:
                    prefix = "#";
                    break;
                case ChatMessageChannel.Party:
                    prefix = "%";
                    break;
                case ChatMessageChannel.Trade:
                    prefix = "$";
                    break;
                case ChatMessageChannel.Guild:
                    prefix = "&";
                    break;
                case ChatMessageChannel.ChatCommand:
                    prefix = "/";
                    break;
                default:

                    break;
            }

            return prefix;
        }
    }
}
