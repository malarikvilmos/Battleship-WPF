using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Battleship_WPF.EventArguments.Join;
using Battleship_WPF.Network;
using Battleship_WPF.Utils;
using Battleship_WPF.ViewModel.Base;

namespace Battleship_WPF.ViewModel
{
    public class JoinGameGUIViewModel : BaseViewModel
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName] string propName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propName));
        }

        private static ObservableCollection<ServerAddress> serverAddresses;
        public static ObservableCollection<ServerAddress> ServerAddresses { get => serverAddresses; set { serverAddresses = value; } }

        private static ServerAddress selectedServerAddress;
        public static ServerAddress SelectedServerAddress { get => selectedServerAddress; set { selectedServerAddress = value; OnStaticPropertyChanged(); } }

        public RelayCommand ConnectCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SaveAddCommand { get; }
        public RelayCommand SaveEditCommand { get; }
        public RelayCommand SaveOnEnterCommand { get; }

        private static bool visibilityAdd;
        public static bool VisibilityAdd { get => visibilityAdd; set { visibilityAdd = value; OnStaticPropertyChanged(); } }

        private static bool visibilityEdit;
        public static bool VisibilityEdit { get => visibilityEdit; set { visibilityEdit = value; OnStaticPropertyChanged(); } }

        private static bool visibilityList = true;
        public static bool VisibilityList { get => visibilityList; set { visibilityList = value; OnStaticPropertyChanged(); } }

        public string textBoxName;
        public string TextBoxName { get => textBoxName; set { textBoxName = value; OnPropertyChanged(); } }

        public string textBoxIP;
        public string TextBoxIP { get => textBoxIP; set { textBoxIP = value; OnPropertyChanged(); } }

        public string textBoxPort;
        public string TextBoxPort { get => textBoxPort; set { textBoxPort = value; textBoxPort = AllowOnlyNumerics(textBoxPort); OnPropertyChanged(); } }

        public event EventHandler<ConnectArgs> OnConnect;
        public JoinGameGUIViewModel()
        {
            ServerAddresses = new ObservableCollection<ServerAddress>();
            ConnectCommand = new RelayCommand(Connect);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            RemoveCommand = new RelayCommand(Remove);
            CancelCommand = new RelayCommand(Cancel);
            SaveAddCommand = new RelayCommand(SaveAdd);
            SaveEditCommand = new RelayCommand(SaveEdit);
            LoadServers();
        }

        private void LoadServers()
        {
            ServerAddresses.Clear();
            foreach (ServerAddress item in ServerManager.GetServers())
            {
                ServerAddresses.Add(item);
            }
        }

        private void Connect()
        {
            if (SelectedServerAddress != null)
                OnConnect?.Invoke(null, new ConnectArgs(SelectedServerAddress));
        }

        private void Add()
        {
            VisibilityList = false;
            VisibilityEdit = false;
            VisibilityAdd = true;
        }

        private void Edit()
        {
            if (SelectedServerAddress == null)
                return;

            TextBoxName = SelectedServerAddress.Name;
            TextBoxIP = SelectedServerAddress.IP;
            TextBoxPort = SelectedServerAddress.Port.ToString();

            VisibilityList = false;
            VisibilityAdd = false;
            VisibilityEdit = true;

            
        }

        private void Remove()
        {
            if (SelectedServerAddress == null)
                return;

            ServerAddresses.Remove(ServerAddresses.FirstOrDefault(i =>
                   i.Name == SelectedServerAddress.Name
                && i.IP == SelectedServerAddress.IP
                && i.Port == SelectedServerAddress.Port));

            ServerManager.DeleteServer(SelectedServerAddress);
            SelectedServerAddress = null;
        }

        private void Cancel()
        {
            VisibilityList = true;
            VisibilityAdd = false;
            VisibilityEdit = false;
            SelectedServerAddress = null;

            SolidColorBrush backColor = new SolidColorBrush(Settings.getBackgroundColor());
            for (int i = 0; i < ServerListItemViewModel.slistItems.Count; ++i)
            {
                ServerListItemViewModel.slistItems[i].SelectedColor = backColor;
            }

            TextBoxName = TextBoxIP = null;
            TextBoxPort = null;
        }

        private void SaveAdd()
        {
            VisibilityList = true;
            VisibilityAdd = false;
            VisibilityEdit = false;

            ServerAddress newAddress = new ServerAddress(TextBoxName, TextBoxIP, VerifyPort(textBoxPort));
            ServerAddresses.Add(newAddress);
            ServerManager.AddServer(newAddress);

            SelectedServerAddress = null;
            SolidColorBrush backColor = new SolidColorBrush(Settings.getBackgroundColor());
            for (int i = 0; i < ServerListItemViewModel.slistItems.Count; ++i)
            {
                ServerListItemViewModel.slistItems[i].SelectedColor = backColor;
            }

            TextBoxName = TextBoxIP = TextBoxPort = null;
        }

        private void SaveEdit()
        {
            VisibilityList = true;
            VisibilityAdd = false;
            VisibilityEdit = false;

            bool result = ServerAddresses.Remove(ServerAddresses.FirstOrDefault(i =>
                   i.Name == SelectedServerAddress.Name
                && i.IP   == SelectedServerAddress.IP
                && i.Port == SelectedServerAddress.Port));
            Console.WriteLine(result);

            ServerAddress newAddress = new ServerAddress(TextBoxName, TextBoxIP, VerifyPort(textBoxPort));
            ServerAddresses.Add(newAddress);
            ServerManager.DeleteServer(SelectedServerAddress);
            ServerManager.AddServer(newAddress);

            SelectedServerAddress = null;
            SolidColorBrush backColor = new SolidColorBrush(Settings.getBackgroundColor());
            for (int i = 0; i < ServerListItemViewModel.slistItems.Count; ++i)
            {
                ServerListItemViewModel.slistItems[i].SelectedColor = backColor;
            }

            TextBoxName = TextBoxIP = TextBoxPort = null;
        }

        private int VerifyPort(string portText)
        {
            if (int.TryParse(portText, out int port))
            {
                if (port >= 0 && port <= 65535)
                {
                    return port;
                }
            }
            return 0;
        }

        private string AllowOnlyNumerics(string text)
        {
            if (text == null)
                return "";

            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(text) || text.Length > 5)
                text = text.Remove(text.Length - 1);

            if (regex.IsMatch(text) || text.Length > 5)
                text = text.Remove(0);

            return text;
        }
    }
}
