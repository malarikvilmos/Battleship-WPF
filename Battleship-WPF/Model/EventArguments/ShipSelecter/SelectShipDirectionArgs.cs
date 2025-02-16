using System;

namespace Battleship_WPF.EventArguments.ShipSelecter
{
    public class SelectShipDirectionArgs : EventArgs
    {
        public bool ShipPlaceHorizontal { get; set; }

        public SelectShipDirectionArgs(bool shipPlaceHorizontal)
        {
            ShipPlaceHorizontal = shipPlaceHorizontal;
        }
    }
}
