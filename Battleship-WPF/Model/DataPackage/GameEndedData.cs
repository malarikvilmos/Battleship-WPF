using Newtonsoft.Json;
using System;

namespace Battleship_WPF.DataPackage
{
    [Serializable]
    public class GameEndedData : Data
    {
        [JsonProperty("Status")]
        public GameEndedStatus Status { get; set; }

        public GameEndedData() : base() { }
        public GameEndedData(GameEndedStatus status, int recipientID) : base(-1)
        {
            Status = status;
            RecipientID = recipientID;
        }
    }

    [Serializable]
    public enum GameEndedStatus
    {
        Unknown,
        Defeat,
        Win
    }
}
