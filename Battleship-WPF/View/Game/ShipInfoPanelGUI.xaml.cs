using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleship_WPF.Utils;

namespace Battleship_WPF.View.Game
{
    /// <summary>
    /// Interaction logic for ShipInfoPanelGUI.xaml
    /// </summary>
    public partial class ShipInfoPanelGUI : UserControl
    {
        private SolidColorBrush UnSelectedColor, SelectedColor;
        private TextBlock felirat;
        public readonly int ShipSize;
        private int piece;
        Style niceTextStyle = Application.Current.TryFindResource("NiceText") as Style;
        public ShipInfoPanelGUI(int shipSize, int piece)
        {
            InitializeComponent();
            UnSelectedColor = new SolidColorBrush(ColorChanger.DarkeningColor(Settings.getBackgroundColor(), 16));
            SelectedColor = new SolidColorBrush(ColorChanger.DarkeningColor(UnSelectedColor.Color, -32));
            Background = UnSelectedColor;
            this.ShipSize = shipSize;
            this.piece = piece;
            felirat = new TextBlock();
            felirat.Text = "1x" + shipSize + ": " + piece + "db";
            felirat.HorizontalAlignment = HorizontalAlignment.Center;
            felirat.Style = niceTextStyle;
            grid.Children.Add(felirat);
        }

        public void SetPiece(int piece)
        {
            this.piece = piece;
            felirat.Text = "1x" + ShipSize + ": " + piece + "db";
            if (piece > 0)
            {
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
                UnSelect();
            }
        }

        public int getPiece() => this.piece;

        public void Select() => Background = SelectedColor;

        public void UnSelect() => Background = UnSelectedColor;

        public void decrease()
        {
            piece--;
            felirat.Text = "1x" + ShipSize + ": " + piece + "db";
            if (piece == 0)
            {
                IsEnabled = false;
                UnSelect();
            }
        }

        public void increase()
        {
            piece++;
            felirat.Text = "1x" + ShipSize + ": " + piece + "db";
            IsEnabled = true;
        }
    }
}
