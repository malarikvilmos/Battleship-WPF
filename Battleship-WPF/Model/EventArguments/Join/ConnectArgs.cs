using System;
using Battleship_WPF.Network;

namespace Battleship_WPF.EventArguments.Join
{
    public class ConnectArgs : EventArgs
    {
        public ServerAddress ServerAddress { get; set; }

        public ConnectArgs(ServerAddress serverAddress)
        {
            ServerAddress = serverAddress;
        }
    }
}
