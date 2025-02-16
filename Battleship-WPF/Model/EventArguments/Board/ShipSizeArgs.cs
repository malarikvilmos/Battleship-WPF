using System;

namespace Battleship_WPF.EventArguments.Board
{
    public class ShipSizeArgs : EventArgs
    {
        public int ShipSize { get; set; }

        public ShipSizeArgs(int shipSize)
        {
            ShipSize = shipSize;
        }
    }
}
