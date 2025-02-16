using System;
using System.Windows;
using System.Windows.Controls;
using Battleship_WPF.EventArguments.ShipSelecter;

namespace Battleship_WPF.View.Game
{
    /// <summary>
    /// Interaction logic for ShipSelecterGUI.xaml
    /// </summary>
    public partial class ShipSelecterGUI : UserControl
    {
        private const int shipTypeNumbers = 4;
        private bool shipPlaceHorizontal;
        ShipInfoPanelGUI[] shipInfoPanels;
        public event EventHandler<SelectShipArgs> OnSelectShip;
        public event EventHandler<SelectShipDirectionArgs> OnSelectDirection;
        public event EventHandler OnRanOutOfShips, OnClearBoard, OnPlaceRandomShips, OnDone;

        public ShipSelecterGUI()
        {
            InitializeComponent();
            buttonHorizontal.Click += (send, args) =>
            {
                if (buttonHorizontal.Content.Equals("Horizontal"))
                {
                    buttonHorizontal.Content = "Vertical";
                    shipPlaceHorizontal = false;
                }
                else
                {
                    buttonHorizontal.Content = "Horizontal";
                    shipPlaceHorizontal = true;
                }
                OnSelectDirection?.Invoke(send, new SelectShipDirectionArgs(shipPlaceHorizontal));
            };
            buttonClearBoard.Click += (send, args) =>
            {
                resetShips();
                OnClearBoard?.Invoke(send, null);
            };
            buttonRandomizeShips.Click += (send, args) =>
            {
                setShipsPieceTo(0);
                OnPlaceRandomShips?.Invoke(send, null);
            };
            buttonDone.Click += (send, args) =>
            {
                OnDone?.Invoke(send, null);
                Visibility = Visibility.Hidden;
            };
            Init();
        }

        private void Init()
        {
            shipPlaceHorizontal = true;
            shipInfoPanels = new ShipInfoPanelGUI[shipTypeNumbers];

            int enWidth = (int)(Width / shipTypeNumbers);
            int enHeight = (int)Height;
            for (int i = 0; i < shipTypeNumbers; i++)
            {
                ShipInfoPanelGUI infoPanel = new ShipInfoPanelGUI(i + 1, shipTypeNumbers - i);
                infoPanel.PreviewMouseLeftButtonDown += (send, args) =>
                {
                    if (infoPanel.IsEnabled)
                        SelectShip(infoPanel);
                };
                grid.Children.Add(infoPanel);
                Grid.SetColumn(infoPanel, i);
                Grid.SetRow(infoPanel, 1);
                shipInfoPanels[i] = infoPanel;
            }
        }

        public void ReInit()
        {
            foreach (var item in shipInfoPanels)
            {
                grid.Children.Remove(item);
            }
            Init();
        }


        public void CanDone(bool value)
        {
            buttonDone.IsEnabled = value;
        }

        public void PlaceToTable(int size)
        {
            shipInfoPanels[size - 1].decrease();
            if (shipInfoPanels[size - 1].getPiece() == 0)
            {
                shipInfoPanels[size - 1].IsEnabled = false;
                automaticSelectShip();
            }
        }

        public void PickupFromTable(int size)
        {
            shipInfoPanels[size - 1].increase();
            if (shipInfoPanels[size - 1].getPiece() == shipInfoPanels.Length)
            {
                shipInfoPanels[size - 1].IsEnabled = false;
                SelectShip(shipInfoPanels[size - 1]);
            }
            automaticSelectShip();
        }

        private void automaticSelectShip()
        {
            foreach (var shipInfoPanel in shipInfoPanels)
                if (shipInfoPanel.getPiece() > 0)
                    SelectShip(shipInfoPanel);

            int i = shipInfoPanels.Length - 1;
            while (i >= 0)
            {
                if (shipInfoPanels[i].getPiece() > 0)
                {
                    SelectShip(shipInfoPanels[i]);
                    return;
                }
                --i;
            }
            OnRanOutOfShips?.Invoke(null, null);
        }

        private void setShipsPieceTo(int number)
        {
            foreach (ShipInfoPanelGUI shipInfoPanel in shipInfoPanels)
                shipInfoPanel.SetPiece(number);
        }

        private void resetShips()
        {
            for (int i = 0; i < shipInfoPanels.Length; i++)
                shipInfoPanels[i].SetPiece(shipTypeNumbers - i);
        }

        private void SelectShip(ShipInfoPanelGUI infoPanel)
        {
            foreach (ShipInfoPanelGUI shipInfoPanel in shipInfoPanels)
                shipInfoPanel.UnSelect();

            infoPanel.Select();
            OnSelectShip?.Invoke(infoPanel, new SelectShipArgs(infoPanel.ShipSize));
        }
    }
}