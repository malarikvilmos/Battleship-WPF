using System;

namespace Battleship_WPF.EventArguments.Client
{
    public class MessageReceivedArgs : EventArgs
    {
        public int SenderID { get; set; }
        public string Message { get; set; }
        public MessageReceivedArgs(int senderID, string message)
        {
            SenderID = senderID;
            Message = message;
        }
    }
}
