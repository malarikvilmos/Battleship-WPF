using Newtonsoft.Json;
using System;
using Battleship_WPF.Model.DataPackage;

namespace Battleship_WPF.DataPackage
{
    public class DataConverter
    {
        private static JsonSerializerSettings jsonsettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        public static Data decode(string message)
        {
            Data data;
            try
            {
                data = (Data)JsonConvert.DeserializeObject(message, jsonsettings);
            }
            catch (Exception)
            {
                data = new DummyData(-1);
            }
            return data;            
        }

        public static string encode(Data data)
        {
            string msg;
            try
            {
                msg = JsonConvert.SerializeObject(data, jsonsettings);
            }
            catch (Exception)
            {
                msg = JsonConvert.SerializeObject(new DummyData(-1), jsonsettings);
            }
            return msg;

        }
    }
}
