﻿using System.Windows;
using System.Windows.Controls;
using Szofttech_WPF.Interfaces;
using Szofttech_WPF.Logic;
using Szofttech_WPF.View.Game;

namespace Szofttech_WPF.View
{
    /// <summary>
    /// Interaction logic for GameGUI.xaml
    /// </summary>
    public partial class GameGUI : UserControl, IExitableGUI
    {
        private ShipSelecterGUI selecter;
        private InfoPanelGUI infoPanel;

        public GameGUI()
        {
            InitializeComponent();

            PlayerBoardGUI playerBoardGUI = new PlayerBoardGUI();
            window.Children.Add(playerBoardGUI);
            Grid.SetRow(playerBoardGUI, 3);
            Grid.SetColumn(playerBoardGUI, 1);
            EnemyBoardGUI enemyBoardGUI = new EnemyBoardGUI();
            window.Children.Add(enemyBoardGUI);
            Grid.SetRow(enemyBoardGUI, 3);
            Grid.SetColumn(enemyBoardGUI, 5);

        }

        public void CloseGUI()
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
