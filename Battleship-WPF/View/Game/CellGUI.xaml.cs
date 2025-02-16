using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleship_WPF.Logic;
using Battleship_WPF.Utils;

namespace Battleship_WPF.View.Game
{
    /// <summary>
    /// Interaction logic for CellGUI.xaml
    /// </summary>
    public partial class CellGUI : UserControl
    {
        //private readonly Color BackGroundColor = Color.FromRgb(82, 137, 200);
        //private readonly Color shipColor = Color.FromRgb(18, 73, 136);
        private Color BackGroundColor;
        private Color shipColor;
        public readonly int I;
        public readonly int J;
        public CellStatus CellStatus { get; private set; }

        public CellGUI(int i, int j)
        {
            InitializeComponent();
            BackGroundColor = ColorChanger.DarkeningColor(Settings.getBackgroundColor(), 32);
            if (BackGroundColor.R == 218 && BackGroundColor.G == 249 && BackGroundColor.B == 255)
                shipColor = ColorChanger.DarkeningColor(BackGroundColor, -128);
            else
                shipColor = ColorChanger.DarkeningColor(BackGroundColor, -64);
            this.I = i;
            this.J = j;
            CellStatus = CellStatus.Empty;
            Background = new SolidColorBrush(BackGroundColor);
        }

        public void select()
        {
            if (CellStatus == CellStatus.Empty)
                setColorSelected();
        }

        public void unSelect()
        {
            if (CellStatus == CellStatus.Empty)
                SetColorUnSelected();
        }

        private void setColorSelected() => Background = new SolidColorBrush(ColorChanger.DarkeningColor(shipColor, 32));

        private void SetColorUnSelected() => Background = new SolidColorBrush(BackGroundColor);



        public void setCell(CellStatus cell)
        {
            CellStatus = cell;
            Dispatcher.Invoke(() =>
            {
                switch (cell)
                {
                    case CellStatus.Empty:
                        Background = new SolidColorBrush(BackGroundColor);
                        break;
                    case CellStatus.EmptyHit:
                        break;
                    case CellStatus.NearShip:
                        break;
                    case CellStatus.Ship:
                        Background = new SolidColorBrush(shipColor);
                        break;
                    case CellStatus.ShipHit:
                        break;
                    case CellStatus.ShipSunk:
                        break;
                    default:
                        break;
                }
                InvalidateVisual();
            });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ImageBrush imageBrush = new ImageBrush();
            Pen pen;
            Brush brush;
            Rect rect;
            Point xp;
            Point yp;
            SolidColorBrush solidColorBrush;
            switch (CellStatus)
            {
                case CellStatus.Empty:
                    break;
                case CellStatus.EmptyHit:
                    //háttér
                    solidColorBrush = new SolidColorBrush(BackGroundColor);
                    pen = new Pen(solidColorBrush, 1);
                    rect = new Rect(0, 0, 30, 30);
                    drawingContext.DrawRectangle(solidColorBrush, pen, rect);
                    //hullám
                    pen = new Pen(Brushes.Blue, 1);
                    for (int k = 0; k < 6; k++)
                    {
                        int[] x = { 0, 5, 10, 15, 20, 25, 30 };
                        int[] y = { 0 + k * 5, 5 + k * 5, 0 + k * 5, 5 + k * 5, 0 + k * 5, 5 + k * 5, 0 + k * 5 };
                        for (int i = 1; i < x.Length; i++)
                        {
                            drawingContext.DrawLine(pen, new Point(x[i - 1], y[i - 1]), new Point(x[i], y[i]));
                        }
                    }
                    //hogy látszódjon is...
                    Background = null;
                    break;
                case CellStatus.NearShip:
                    break;
                case CellStatus.Ship:
                    break;
                case CellStatus.ShipHit:
                    //háttér
                    solidColorBrush = new SolidColorBrush(shipColor);
                    pen = new Pen(solidColorBrush, 1);
                    rect = new Rect(0, 0, 30, 30);
                    drawingContext.DrawRectangle(solidColorBrush, pen, rect);
                    //X
                    pen = new Pen(Brushes.Red, 3);
                    xp = new Point(0, 0);
                    yp = new Point(30, 30);
                    drawingContext.DrawLine(pen, xp, yp);
                    xp = new Point(0, 30);
                    yp = new Point(30, 0);
                    drawingContext.DrawLine(pen, xp, yp);
                    //hogy látszódjon is...
                    Background = null;
                    break;
                case CellStatus.ShipSunk:
                    break;
                default:
                    break;
            }
        }
    }
}
