using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.DataTypes.Interfaces
{
    interface IGame
    {
        event EventHandler GameProcessExited;

        bool IsForegroundWindow { get; }
        string LogFile { get; }
        bool IsValid { get; }

        void DisplayWhoIs(string player);
        void EnterHideout(string player);
        void KickPlayer(string player);
        void LeaveParty();
        void TradeWith(string player);
        void InvitePlayer(string player);
        void SendWhisper(string recipientName);

        void ChatLogout();
        bool CloseTcpConnections(Imports.Iphlpapi.IpVersion ipVersion);

        IGame Launch(string filepath);
    }
}
