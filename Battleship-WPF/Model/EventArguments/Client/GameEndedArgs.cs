using System;
using Battleship_WPF.DataPackage;

namespace Battleship_WPF.EventArguments.Client
{
    public class GameEndedArgs : EventArgs
    {
        public GameEndedStatus GameEndedStatus { get; set; }

        public GameEndedArgs(GameEndedStatus gameEndedStatus)
        {
            GameEndedStatus = gameEndedStatus;
        }
    }
}
