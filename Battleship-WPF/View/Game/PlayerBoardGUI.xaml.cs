using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Battleship_WPF.EventArguments.Board;
using Battleship_WPF.Logic;

namespace Battleship_WPF.View.Game
{
    /// <summary>
    /// Interaction logic for PlayerBoardGUI.xaml
    /// </summary>
    public partial class PlayerBoardGUI : BoardGUI
    {
        public bool shipPlaceHorizontal;
        public bool canPlace;
        public int selectedShipSize;
        private List<CellGUI> selectedCells;
        private readonly Point[] relativeCoords = {
            new Point(-1, -1),
            new Point(-1, 0),
            new Point(-1, 1),
            new Point(0, -1),
            new Point(0, 0),
            new Point(0, 1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1)
        };
        public event EventHandler<ShipSizeArgs> OnPlace, OnPickUp;

        public PlayerBoardGUI()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            canPlace = true;
            shipPlaceHorizontal = true;
            selectedShipSize = 4;
            selectedCells = new List<CellGUI>();
            cells = new CellGUI[board.getNLength(), board.getNLength()];
            
            int szelesseg = int.Parse("" + Math.Floor(Width / board.getNLength()));
            for (int i = 0; i < board.getNLength(); i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                for (int j = 0; j < board.getNLength(); j++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    CellGUI seged = new CellGUI(i, j);
                    seged.Width = seged.Height = szelesseg;
                    seged.PreviewMouseLeftButtonDown += (send, args) =>
                    {
                        if (IsEnabled)
                            cellClick(seged);
                    };
                    seged.MouseEnter += (send, args) =>
                    {
                        if (IsEnabled)
                            if (canPlace)
                                cellEntered(seged);
                    };
                    seged.MouseLeave += (send, args) =>
                    {
                        if (IsEnabled)
                            cellExited(seged);
                    };
                    cells[i, j] = seged;
                    seged.setCell(board.getCellstatus()[i, j]);
                    grid.Children.Add(seged);
                    Grid.SetRow(seged, i);
                    Grid.SetColumn(seged, j);
                }
            }
        }

        public void ReInit()
        {
            canPlace = true;
            shipPlaceHorizontal = true;
            selectedShipSize = 4;
            selectedCells.Clear();
            for (int i = 0; i < board.getNLength(); i++)
            {
                for (int j = 0; j < board.getNLength(); j++)
                {
                    cells[i, j].setCell(CellStatus.Empty);
                }
            }
        }

        private void cellExited(CellGUI cel)
        {
            foreach (var selectedCell in selectedCells)
            {
                selectedCell.unSelect();
            }
            selectedCells.Clear();
        }

        private void cellEntered(CellGUI cell)
        {
            selectedCells.Clear();
            if (isEmptyPlace(cell))
            {
                if (shipPlaceHorizontal)
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        cells[cell.I, cell.J + i].select();
                        selectedCells.Add(cells[cell.I, cell.J + i]);
                    }
                }
                else
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        cells[cell.I + i, cell.J].select();
                        selectedCells.Add(cells[cell.I + i, cell.J]);
                    }
                }
            }
        }

        private bool isEmptyPlace(CellGUI cell)
        {
            if (shipPlaceHorizontal)
            {
                if (cell.J + selectedShipSize - 1 <= 9)
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        foreach (Point relativeCoord in relativeCoords)
                        {
                            int cellI = cell.I + (int)relativeCoord.Y;
                            int cellJ = cell.J + (int)relativeCoord.X;

                            cellJ += i;
                            if (cellI >= 0 && cellI <= 9 && cellJ >= 0 && cellJ <= 9)
                            {
                                if (cells[cellI, cellJ].CellStatus != CellStatus.Empty)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {//Vertical
                if (cell.I + selectedShipSize - 1 <= 9)
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        foreach (Point relativeCoord in relativeCoords)
                        {
                            int cellI = cell.I + (int)relativeCoord.Y;
                            int cellJ = cell.J + (int)relativeCoord.X;

                            cellI += i;
                            if (cellI >= 0 && cellI <= 9 && cellJ >= 0 && cellJ <= 9)
                            {
                                if (cells[cellI, cellJ].CellStatus != CellStatus.Empty)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void cellClick(CellGUI cell)
        {
            if (cell.CellStatus != CellStatus.Ship && canPlace)
            {
                if (selectedCells.Count > 0)
                {
                    foreach (var selectedCell in selectedCells)
                    {
                        selectedCell.setCell(CellStatus.Ship);
                        board.setCell(selectedCell.I, selectedCell.J, selectedCell.CellStatus);
                    }
                    OnPlace?.Invoke(null, new ShipSizeArgs(selectedShipSize));
                }
            }
            else if (cell.CellStatus == CellStatus.Ship)
            { //Ha felszedi
                int cellI = cell.I, cellJ = cell.J;
                int i = 1;
                int pickupShipSize = 0;
                //LE
                while (cellI >= 0 && cellI <= 9 && cellJ + i >= 0 && cellJ + i <= 9)
                {
                    if (cells[cellI, cellJ + i].CellStatus == CellStatus.Ship)
                    {
                        cells[cellI, cellJ + i].setCell(CellStatus.Empty);
                        ++pickupShipSize;
                    }
                    else
                    {
                        break;
                    }
                    ++i;
                }
                i = 0;
                //FEL
                while (cellI >= 0 && cellI <= 9 && cellJ + i >= 0 && cellJ + i <= 9)
                {
                    if (cells[cellI, cellJ + i].CellStatus == CellStatus.Ship)
                    {
                        cells[cellI, cellJ + i].setCell(CellStatus.Empty);
                        ++pickupShipSize;
                    }
                    else
                    {
                        break;
                    }
                    --i;
                }
                i = 1;
                //JOBBRA
                while (cellI + i >= 0 && cellI + i <= 9 && cellJ >= 0 && cellJ <= 9)
                {
                    if (cells[cellI + i, cellJ].CellStatus == CellStatus.Ship)
                    {
                        cells[cellI + i, cellJ].setCell(CellStatus.Empty);
                        ++pickupShipSize;
                    }
                    else
                    {
                        break;
                    }
                    ++i;
                }
                i = -1;
                //BALRA
                while (cellI + i >= 0 && cellI + i <= 9 && cellJ >= 0 && cellJ <= 9)
                {
                    if (cells[cellI + i, cellJ].CellStatus == CellStatus.Ship)
                    {
                        cells[cellI + i, cellJ].setCell(CellStatus.Empty);
                        ++pickupShipSize;
                    }
                    else
                    {
                        break;
                    }
                    --i;
                }
                OnPickUp?.Invoke(null, new ShipSizeArgs(pickupShipSize));
                cellEntered(cell); //Kijelölve legyen, amit levettünk
            }
        }

        public void ClearBoard()
        {
            for (int i = 0; i < board.getNLength(); i++)
            {
                for (int j = 0; j < board.getNLength(); j++)
                {
                    cells[i, j].setCell(CellStatus.Empty);
                    board.cellstatus[i, j] = CellStatus.Empty;
                }
            }
        }

        public void RandomPlace()
        {
            board = Board.RandomBoard();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cells[i, j].setCell(board.getCellstatus()[i, j]);
                }
            }
            selectedCells.Clear();
        }

    }
}
