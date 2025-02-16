using Newtonsoft.Json;
using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class ShotData : Data
    {
        [JsonProperty("I")]
        public int I { get; private set; }

        [JsonProperty("J")]
        public int J { get; private set; }

        public ShotData() : base() { }
        public ShotData(int clientID, int i, int j) : base(clientID)
        {
            I = i;
            J = j;
        }
    }
}
