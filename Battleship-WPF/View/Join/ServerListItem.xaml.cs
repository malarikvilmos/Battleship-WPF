#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Battleship_WPF.Network;

namespace Battleship_WPF.View.Join
{
    public partial class ServerListItem : UserControl, INotifyPropertyChanged
    {
        public static int Global_ID = 0;

        public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(ServerListItem));
        public static readonly DependencyProperty ServerIPProperty = DependencyProperty.Register("ServerIP", typeof(string), typeof(ServerListItem));
        public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(ServerListItem));

        public string ServerName
        {
            get => (string)GetValue(ServerNameProperty);
            set { SetValue(ServerNameProperty, value); ServerAddress.Name = ServerName; ServerAddress.IP = ServerIP; ServerAddress.Port = ServerPort; }
        }

        public string ServerIP
        {
            get => (string)GetValue(ServerIPProperty);
            set { SetValue(ServerIPProperty, value); ServerAddress.Name = ServerName; ServerAddress.IP = ServerIP; ServerAddress.Port = ServerPort; }
        }

        public int ServerPort
        {
            get => (int)GetValue(ServerPortProperty);
            set { SetValue(ServerPortProperty, value); ServerAddress.Name = ServerName; ServerAddress.IP = ServerIP; ServerAddress.Port = ServerPort; }
        }

        private ServerAddress serverAddress;

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerAddress ServerAddress { get => serverAddress; set { serverAddress = value; OnPropertyChanged("ServerAddress"); } }

        public ServerListItem()
        {
            InitializeComponent();
            SetServerAddress(Global_ID++);
        }
        
        private async Task SetServerAddress(int id)
        {
            await Task.Delay(100);
            ServerAddress = new ServerAddress(ServerName, ServerIP, ServerPort, id);
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
