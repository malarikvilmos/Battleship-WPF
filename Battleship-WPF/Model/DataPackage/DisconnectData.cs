using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class DisconnectData : Data
    {
        public DisconnectData() : base() { }
        public DisconnectData(int clientID) : base(clientID) { }
    }
}
