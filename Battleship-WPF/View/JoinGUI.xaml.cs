using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleship_WPF.Interfaces;
using Battleship_WPF.Utils;
using Battleship_WPF.ViewModel;

namespace Battleship_WPF.View
{
    /// <summary>
    /// Interaction logic for JoinGUI.xaml
    /// </summary>
    public partial class JoinGUI : UserControl, IExitableGUI
    {
        private JoinGameGUIViewModel joinGameGUIViewModel;
        public JoinGUI()
        {
            InitializeComponent();
            joinGameGUIViewModel = new JoinGameGUIViewModel();
            DataContext = joinGameGUIViewModel;
        }

        public void CloseGUI()
        {
            this.Visibility = Visibility.Hidden;
            JoinGameGUIViewModel.SelectedServerAddress = null;
            joinGameGUIViewModel.CancelCommand.Execute(null);

            SolidColorBrush backColor = new SolidColorBrush(Settings.getBackgroundColor());
            for (int i = 0; i < ServerListItemViewModel.slistItems.Count; ++i)
            {
                ServerListItemViewModel.slistItems[i].SelectedColor = backColor;
            }
        }

        public bool ExitApplication()
        {
            return true;
        }
    }
}
