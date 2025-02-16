namespace Battleship_WPF.Network
{
    public class ServerAddress
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public int ID { get; set; }

        public ServerAddress(string name, string ip, int port)
        {
            Name = name;
            IP = ip;
            Port = port;
        }

        public ServerAddress(string name, string ip, int port, int id)
        {
            Name = name;
            IP = ip;
            Port = port;
            ID = id;
        }

        public override string ToString()
        {
            return Name + "$" + IP + "$" + Port;
        }
    }
}
