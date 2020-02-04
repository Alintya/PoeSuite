using PoeSuite.DataTypes;
using PoeSuite.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Messages
{
    class SendChatMessage
    {
        public ChatMessage Message { get; set; }

        public SendChatMessage(ChatMessage msg)
        {
            Message = msg;
        }
    }
}
