using System;

namespace Battleship_WPF.EventArguments.ShipSelecter
{
    public class SelectShipArgs : EventArgs
    {
        public int ShipSize { get; set; }

        public SelectShipArgs(int shipSize)
        {
            ShipSize = shipSize;
        }
    }
}
