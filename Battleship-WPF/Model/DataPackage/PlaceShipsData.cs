using Newtonsoft.Json;
using System;
using Battleship_WPF.Logic;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class PlaceShipsData : Data
    {
        [JsonProperty("Board")]
        public Board Board;

        public PlaceShipsData() : base() { }
        public PlaceShipsData(int clientID, Board board) : base(clientID)
        {
            Board = board;
        }
    }
}
