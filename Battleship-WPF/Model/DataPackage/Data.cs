using Newtonsoft.Json;
using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public abstract class Data
    {
        [JsonProperty("ClientID")]
        public int ClientID { get; set; }
        [JsonProperty("RecipientID")]
        public int RecipientID { get; set; }

        public Data() { }

        public Data(int clientID)
        {
            this.ClientID = clientID;
            this.RecipientID = -1;
        }
    }
}
