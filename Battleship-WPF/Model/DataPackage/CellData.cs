using Newtonsoft.Json;
using System;
using Battleship_WPF.Logic;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class CellData : Data
    {
        [JsonProperty("I")]
        public int I { get; set; }
        [JsonProperty("J")]
        public int J { get; set; }

        [JsonProperty("Status")]
        public CellStatus Status { get; set; }

        public CellData() : base() { }

        public CellData(int clientID, int i, int j) : base(clientID)
        {
            I = i;
            J = j;
        }

        public CellData(int clientID, int i, int j, CellStatus status) : base(clientID)
        {
            I = i;
            J = j;
            Status = status;
        }
    }
}
