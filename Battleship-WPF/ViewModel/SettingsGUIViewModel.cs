using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Media;
using Battleship_WPF.Utils;
using Battleship_WPF.ViewModel.Base;

namespace Battleship_WPF.ViewModel
{
    public class SettingsGUIViewModel : BaseViewModel
    {
        public RelayCommand ModifyPortCommand {get;}

        private string portText;
        public string PortText { get => portText; set { portText = value; portText = AllowOnlyNumerics(portText); } }

        private string responseText;
        public string ResponseText { get => responseText; set { responseText = value; OnPropertyChanged(); } }

        private bool visibility;
        public bool Visibility 
        { 
            get => visibility; 
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        private List<Color> colors = new List<Color>()
        {
            Color.FromRgb(50, 105, 168),
            Color.FromRgb(37, 57, 66),
            Color.FromRgb(186, 217, 232)
        };

        public List<Color> Colors { get => colors; }

        public SettingsGUIViewModel()
        {
            PortText = Settings.getPort().ToString();
            ModifyPortCommand = new RelayCommand(ModifyPort);
        }

        private void ModifyPort()
        {
            if (int.TryParse(PortText, out int port))
            {
                if (port >= 0 && port <= 65535)
                {
                    Settings.setPort(port);
                    ResponseText = "Port set to: " + Settings.getPort().ToString();
                    Settings.Save();
                }
                else
                {
                    ResponseText = "Port must be between 0 and 65535";
                }
            }
            
                     
            Visibility = true;
            Timer timer = new Timer();
            timer.Interval = 1500;
            timer.Elapsed += (source, args) =>
            {
                timer.Stop();
                Visibility = false;
            };
            timer.Start();
        }

        private string AllowOnlyNumerics(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(text) || text.Length > 5)
                text = text.Remove(text.Length - 1);
            if (regex.IsMatch(text) || text.Length > 5)
                text = text.Remove(0);
            return text;
        }
    }
}
