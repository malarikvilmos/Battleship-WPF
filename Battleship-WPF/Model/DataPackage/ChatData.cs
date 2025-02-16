using Newtonsoft.Json;
using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class ChatData : Data
    {
        [JsonProperty("")]
        public string Message { get; set; }

        public ChatData() : base() { }

        public ChatData(int clientID, string message) : base(clientID)
        {
            Message = message;
        }
    }
}
