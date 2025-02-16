using System;

namespace Battleship_WPF.EventArguments.Board
{
    public class ShotArgs : EventArgs
    {
        public int I { get; set; }
        public int J { get; set; }

        public ShotArgs(int i, int j)
        {
            I = i;
            J = j;
        }
    }
}
