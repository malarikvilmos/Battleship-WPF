using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class TurnData : Data
    {
        public TurnData() : base() { }
        public TurnData(int recipientID) : base(-1)
        {
            RecipientID = recipientID;
        }
    }
}
