using System;
using Battleship_WPF.Logic;

namespace Battleship_WPF.EventArguments.Client
{
    public class MyHitArgs : EventArgs
    {
        public int I { get; set; }
        public int J { get; set; }
        public CellStatus Status { get; set; }

        public MyHitArgs(int i, int j, CellStatus status)
        {
            I = i;
            J = j;
            Status = status;
        }
    }
}
