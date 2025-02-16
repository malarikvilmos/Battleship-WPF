using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Battleship_WPF.DataPackage;
using Battleship_WPF.EventArguments.Board;
using Battleship_WPF.EventArguments.Chat;
using Battleship_WPF.EventArguments.Client;
using Battleship_WPF.EventArguments.ShipSelecter;
using Battleship_WPF.Interfaces;
using Battleship_WPF.Logic;
using Battleship_WPF.Network;
using Battleship_WPF.Utils;
using Battleship_WPF.View.Game;
using Battleship_WPF.ViewModel;

namespace Battleship_WPF.View
{
    /// <summary>
    /// Interaction logic for GameGUI.xaml
    /// </summary>
    public partial class GameGUI : UserControl, IExitableGUI
    {
        PlayerBoardGUI playerBoardGUI;
        EnemyBoardGUI enemyBoardGUI;
        private ShipSelecterGUI selecter;
        private ChatGUI chatGUI;
        private InfoPanelGUI infoPanel;
        private Client Client;
        private Server Server;
        private bool exitable = true;

        public GameGUI(int port) : this(Settings.getIP(), port)
        {
            Server = new Server(port);
            ipListGrid.Visibility = Visibility.Visible;
            IPListItemsControl.DataContext = Server.getLocalIPs();
        }

        public GameGUI(string ip, int port)
        {
            InitializeComponent();

            playerBoardGUI = new PlayerBoardGUI();
            enemyBoardGUI = new EnemyBoardGUI();
            selecter = new ShipSelecterGUI();
            chatGUI = new ChatGUI();

            Client = new Client(ip, port);
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnYourTurn += Client_OnYourTurn;
            Client.OnGameEnded += Client_OnGameEnded;
            Client.OnEnemyHitMe += Client_OnEnemyHitMe;
            Client.OnMyHit += Client_OnMyHit;
            Client.OnJoinedEnemy += Client_OnJoinedEnemy;
            Client.OnDisconnected += Client_OnDisconnected;
            Client.OnRematch += Client_OnRematch;

            playerBoardGUI.Visibility = Visibility.Hidden;
            playerBoardGUI.OnPlace += PlayerBoardGUI_OnPlace;
            playerBoardGUI.OnPickUp += PlayerBoardGUI_OnPickUp;
            grid.Children.Add(playerBoardGUI);
            Grid.SetRow(playerBoardGUI, 3);
            Grid.SetColumn(playerBoardGUI, 1);

            enemyBoardGUI.Visibility = Visibility.Hidden;
            enemyBoardGUI.OnShot += EnemyBoardGUI_OnShot;
            grid.Children.Add(enemyBoardGUI);
            Grid.SetRow(enemyBoardGUI, 3);
            Grid.SetColumn(enemyBoardGUI, 5);

            selecter.Visibility = Visibility.Hidden;
            selecter.OnSelectShip += Selecter_OnSelectShip;
            selecter.OnSelectDirection += Selecter_OnSelectDirection;
            selecter.OnClearBoard += Selecter_OnClearBoard;
            selecter.OnPlaceRandomShips += Selecter_OnPlaceRandomShips;
            selecter.OnRanOutOfShips += Selecter_OnRanOutOfShips;
            selecter.OnDone += Selecter_OnDone;
            grid.Children.Add(selecter);
            Grid.SetRow(selecter, 1);
            Grid.SetRowSpan(selecter, 3);
            Grid.SetColumn(selecter, 1);
            Grid.SetColumnSpan(selecter, 5);

            chatGUI.Visibility = Visibility.Hidden;
            ((ChatViewModel)chatGUI.DataContext).OnSendMessage += ChatGUI_OnSendMessage;
            grid.Children.Add(chatGUI);
            Grid.SetRow(chatGUI, 1);
            Grid.SetRowSpan(chatGUI, 1);
            Grid.SetColumn(chatGUI, 1);
            Grid.SetColumnSpan(chatGUI, 5);

            infoPanel = new InfoPanelGUI();
            infoPanel.Visibility = Visibility.Hidden;
            grid.Children.Add(infoPanel);
            Grid.SetRow(infoPanel, 3);
            Grid.SetColumn(infoPanel, 3);
            ((InfoPanelGUIViewModel)infoPanel.DataContext).changeVisibility(false);
        }

        private void ReInit()
        {
            ((InfoPanelGUIViewModel)infoPanel.DataContext).changeVisibility(false);

            playerBoardGUI.IsEnabled = true;
            chatGUI.Visibility = Visibility.Hidden;
            infoPanel.Visibility = Visibility.Hidden;

            selecter.Visibility = Visibility.Visible;
            enemyBoardGUI.ReInit();
            playerBoardGUI.ReInit();
            selecter.ReInit();
            Selecter_OnClearBoard(null, null);
            Selecter_OnPlaceRandomShips(null, null);
        }

        private void Client_OnRematch(object sender, EventArgs e)
        {
            Console.WriteLine("REMATCH");
            Dispatcher.Invoke(() => ReInit());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void EnemyBoardGUI_OnShot(object sender, ShotArgs e)
        {
            Dispatcher.Invoke(() => ((InfoPanelGUIViewModel)infoPanel.DataContext).changeVisibility(false));
            Client.sendMessage(new ShotData(Client.ID, e.I, e.J));
        }

        private void ChatGUI_OnSendMessage(object sender, SendMessageEventArgs e)
        {
            Client.sendMessage(new ChatData(Client.ID, e.Message));
        }

        private void Client_OnDisconnected(object sender, EventArgs e)
        {
            Client_OnMessageReceived(null, new MessageReceivedArgs(-1, "Enemy left the game."));
            exitable = true;
        }

        private void Client_OnJoinedEnemy(object sender, EventArgs e)
        {
            exitable = false;
            Dispatcher.Invoke(() =>
            {
                waitingTitle.Visibility = Visibility.Hidden;
                ipListGrid.Visibility = Visibility.Hidden;
                playerBoardGUI.Visibility = Visibility.Visible;
                enemyBoardGUI.Visibility = Visibility.Visible;
                selecter.Visibility = Visibility.Visible;
            });
        }

        private void Client_OnMyHit(object sender, MyHitArgs e)
        {
            enemyBoardGUI.Hit(e.I, e.J, e.Status);
        }

        private void Client_OnEnemyHitMe(object sender, EnemyHitMeArgs e)
        {
            playerBoardGUI.Hit(e.I, e.J);
        }

        private void Client_OnGameEnded(object sender, GameEndedArgs e)
        {
            enemyBoardGUI.setTurnEnabled(false);
            string endMessage = "";
            switch (e.GameEndedStatus)
            {
                case GameEndedStatus.Unknown:
                    endMessage = "Unknown game ended status.";
                    break;
                case GameEndedStatus.Defeat:
                    endMessage = "Defeat!";
                    break;
                case GameEndedStatus.Win:
                    endMessage = "You win!";
                    break;
                default:
                    break;
            }
            Dispatcher.Invoke(() =>
            {
                ((ChatViewModel)chatGUI.DataContext).addMessage("System", endMessage);
                infoPanel.Visibility = Visibility.Hidden;
            });
            exitable = true;
        }

        private void Client_OnYourTurn(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => ((InfoPanelGUIViewModel)infoPanel.DataContext).changeVisibility(true));
            enemyBoardGUI.setTurnEnabled(true);
        }

        private void Client_OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.SenderID == -1)
                    ((ChatViewModel)chatGUI.DataContext).addMessage("System", e.Message);
                else if (e.SenderID == Client.ID)
                    ((ChatViewModel)chatGUI.DataContext).addMessage("You", e.Message);
                else
                    ((ChatViewModel)chatGUI.DataContext).addMessage("Opponent", e.Message);
            });
        }

        private void PlayerBoardGUI_OnPickUp(object sender, ShipSizeArgs e)
        {
            selecter.PickupFromTable(e.ShipSize);
            playerBoardGUI.canPlace = true;
            selecter.CanDone(false);
        }

        private void PlayerBoardGUI_OnPlace(object sender, ShipSizeArgs e)
        {
            selecter.PlaceToTable(e.ShipSize);
        }

        private void Selecter_OnDone(object sender, EventArgs e)
        {
            playerBoardGUI.IsEnabled = false;
            chatGUI.Visibility = Visibility.Visible;
            infoPanel.Visibility = Visibility.Visible;
            Client.sendMessage(new PlaceShipsData(Client.ID, playerBoardGUI.board));
        }

        private void Selecter_OnRanOutOfShips(object sender, EventArgs e)
        {
            playerBoardGUI.canPlace = false;
            selecter.CanDone(true);
        }

        private void Selecter_OnPlaceRandomShips(object sender, EventArgs e)
        {
            playerBoardGUI.canPlace = false;
            playerBoardGUI.RandomPlace();
            selecter.CanDone(true);
        }

        private void Selecter_OnClearBoard(object sender, EventArgs e)
        {
            playerBoardGUI.ClearBoard();
            playerBoardGUI.canPlace = true;
            selecter.CanDone(false);
        }

        private void Selecter_OnSelectDirection(object sender, SelectShipDirectionArgs e)
        {
            playerBoardGUI.shipPlaceHorizontal = e.ShipPlaceHorizontal;
        }

        private void Selecter_OnSelectShip(object sender, SelectShipArgs e)
        {
            playerBoardGUI.selectedShipSize = e.ShipSize;
        }

        public void CloseGUI()
        {
            if (exitable)
            {
                this.Visibility = Visibility.Hidden;
                Server?.Close();
                Client?.Close();
            }
            else
            {
                var Res = MessageBox.Show("Do you want to return to the menu?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.Yes == Res)
                {
                    exitable = true;
                    Client.sendMessage(new DisconnectData(Client.ID));
                    CloseGUI();
                }
            }
        }

        public bool ExitApplication()
        {
            if (exitable) return true;

            var Res = MessageBox.Show("Do you want to exit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == Res)
            {
                Client.sendMessage(new DisconnectData(Client.ID));
                return true;
            }

            return false;
        }

        private void ClickIPTextBlock(object sender, System.Windows.Input.MouseButtonEventArgs e) => Clipboard.SetText((sender as TextBlock).Text);
    }
}
