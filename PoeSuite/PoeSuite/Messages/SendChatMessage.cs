using PoeSuite.DataTypes;

namespace PoeSuite.Messages
{
    internal class SendChatMessage
    {
        public ChatMessage Message { get; set; }

        public SendChatMessage(ChatMessage msg)
        {
            Message = msg;
        }
    }
}
