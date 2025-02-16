using System;
using System.Collections.Generic;

namespace Battleship_WPF.Logic
{
    public class Board
    {
        private static Coordinate[] relativeCoords = {
        new Coordinate(-1, -1),
        new Coordinate(-1, 0),
        new Coordinate(-1, 1),
        new Coordinate(0, -1),
        new Coordinate(0, 0),
        new Coordinate(0, 1),
        new Coordinate(1, -1),
        new Coordinate(1, 0),
        new Coordinate(1, 1)
        };

        private static Coordinate[] relativeCoordsVertical = {
            new Coordinate(-1, 0),
            new Coordinate(1, 0)
        };
        private static Coordinate[] relativeCoordsHorizontal = {
            new Coordinate(0, -1),
            new Coordinate(0, 1)
        };

        public CellStatus[,] cellstatus = new CellStatus[10, 10];

        /// <summary>
        /// Üres táblát hoz létre.
        /// </summary>
        public Board()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellstatus[i, j] = CellStatus.Empty;
                }
            }
        }
        /// <summary>
        /// Létrehoz egy táblát string típusból.
        /// </summary>
        /// <param name="a"></param>
        public Board(string a)
        {
            string[] row = a.Split(';');
            for (int i = 0; i < 10; i++)
            {
                string[] column = row[i].Split(';');
                for (int j = 0; j < 10; j++)
                {
                    Enum.TryParse(column[j], out cellstatus[i, j]);
                }
            }
        }

        /// <summary>
        /// Beállítja a táblán az adott koordinátájú cellát egy értékre.
        /// </summary>
        /// <param name="i">Sor.</param>
        /// <param name="j">Oszlop.</param>
        /// <param name="status">Cellstatus típusú objektum.</param>
        public void setCell(int i, int j, CellStatus status)
        {
            cellstatus[i, j] = status;
        }

        public int getNLength()
        {
            return cellstatus.GetLength(0);
        }
        public CellStatus[,] getCellstatus()
        {
            return cellstatus;
        }
        /// <summary>
        /// Átalakítja stringgé a táblát.
        /// </summary>
        /// <returns></returns>
        public string convertToString()
        {
            string re = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    re += cellstatus[i, j] + ":";
                }
                re += ";";
            }
            return re;
        }

        public override string ToString()
        {
            string a = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    a += cellstatus[j, i].ToString()[0] == 'S' ? "▓ " : "░ ";
                }
                a += "\n";
            }
            return "Board:\n" + a;
        }

        public static Board RandomBoard()
        {
            Random rnd = new Random();
            Board board = new Board();
            int[] shipSizes = { 4, 3, 2, 1 };
            int[] shipPieces = { 1, 2, 3, 4 };
            List<Coordinate> availableCells = new List<Coordinate>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    availableCells.Add(new Coordinate(i, j));
                }
            }

            for (int i = 0; i < shipSizes.Length; i++)
            {
                for (int dbI = 0; dbI < shipPieces[i]; dbI++)
                {
                    List<int> availableCellsIntegers = new List<int>();
                    for (int s = 0; s < availableCells.Count; s++)
                    {
                        availableCellsIntegers.Add(s);
                    }

                    bool probalkozz = true, horizontal = rnd.Next(0, 2) == 1 ? true : false;
                    int cellI, cellJ;

                    do
                    {
                        int rndnum = rnd.Next(availableCellsIntegers.Count);
                        int sgd = availableCellsIntegers[rndnum];

                        cellI = availableCells[sgd].X;
                        cellJ = availableCells[sgd].Y;

                        if (Board.isEmptyPlace(board.cellstatus, cellI, cellJ, shipSizes[i], horizontal))
                        {
                            if (horizontal)
                            {
                                for (int s = 0; s < shipSizes[i]; s++)
                                {
                                    board.setCell(cellI + s, cellJ, CellStatus.Ship);
                                }
                            }
                            else
                            {
                                for (int s = 0; s < shipSizes[i]; s++)
                                {
                                    board.setCell(cellI, cellJ + s, CellStatus.Ship);
                                }
                            }
                            probalkozz = false;
                        }
                        availableCellsIntegers.Remove(rndnum);
                    } while (probalkozz && availableCellsIntegers.Count > 0);
                }
            }
            return board;
        }

        private static bool isEmptyPlace(CellStatus[,] cell, int ccelli, int ccellj, int selectedShipSize, bool shipPlaceHorizontal)
        {
            if (shipPlaceHorizontal)
            {
                if (ccelli + selectedShipSize - 1 <= 9)
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        foreach (var relativeCoord in relativeCoords)
                        {
                            int cellI = ccelli + relativeCoord.Y;
                            int cellJ = ccellj + relativeCoord.X;

                            cellI += i;
                            if (cellI >= 0 && cellI <= 9 && cellJ >= 0 && cellJ <= 9)
                            {
                                if (cell[cellI, cellJ] == CellStatus.Ship)
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
            {
                if (ccellj + selectedShipSize - 1 <= 9)
                {
                    for (int i = 0; i < selectedShipSize; i++)
                    {
                        foreach (Coordinate relativeCoord in relativeCoords)
                        {
                            int cellI = ccelli + relativeCoord.Y;
                            int cellJ = ccellj + relativeCoord.X;

                            cellJ += i;
                            if (cellI >= 0 && cellI <= 9 && cellJ >= 0 && cellJ <= 9)
                            {
                                if (cell[cellI, cellJ] != CellStatus.Empty)
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

        public bool hasCellStatus(CellStatus status)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (cellstatus[i, j] == status)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Coordinate> ShipCoords(int i, int j)
        {
            List<Coordinate> shipsCoords = new List<Coordinate>();            
            bool horizontal = false;
            foreach (Coordinate point in relativeCoordsHorizontal)
            {
                int x = i + point.X;
                int y = j + point.Y;
                if (x >= 0 && x < 10 && y >= 0 && y < 10)
                {
                    CellStatus cs = cellstatus[x, y];
                    if (cs == CellStatus.Ship || cs == CellStatus.ShipHit || cs == CellStatus.ShipSunk)
                    {
                        horizontal = true;
                    }
                }
            }

            shipsCoords.Add(new Coordinate(i, j));

            if (horizontal)
            {
                CellStatus cs;
                int x, y;
                bool existShip;

                foreach (Coordinate point in relativeCoordsHorizontal)
                {
                    x = i;
                    y = j;
                    existShip = true;
                    do
                    {
                        x += point.X;
                        y += point.Y;
                        if (x >= 0 && x < 10 && y >= 0 && y < 10)
                        {
                            cs = cellstatus[x, y];
                            if (cs == CellStatus.Ship || cs == CellStatus.ShipHit || cs == CellStatus.ShipSunk)
                            {
                                Coordinate p = new Coordinate(x, y);
                                if (!shipsCoords.Contains(p))
                                {
                                    shipsCoords.Add(p);
                                }
                            }
                            else
                            {
                                existShip = false;
                            }
                        }
                        else
                        {
                            existShip = false;
                        }
                    } while (existShip);
                }
            }
            else
            {
                CellStatus cs;
                int x, y;
                bool existShip;

                foreach (Coordinate point in relativeCoordsVertical)
                {
                    x = i;
                    y = j;
                    existShip = true;
                    do
                    {
                        x += point.X;
                        y += point.Y;
                        if (x >= 0 && x < 10 && y >= 0 && y < 10)
                        {
                            cs = cellstatus[x, y];
                            if (cs == CellStatus.Ship || cs == CellStatus.ShipHit || cs == CellStatus.ShipSunk)
                            {
                                Coordinate p = new Coordinate(x, y);
                                if (!shipsCoords.Contains(p))
                                {
                                    shipsCoords.Add(p);
                                }
                            }
                            else
                            {
                                existShip = false;
                            }
                        }
                        else
                        {
                            existShip = false;
                        }
                    } while (existShip);
                }
            }
            return shipsCoords;
        }

        public bool isSunk(int i, int j)
        {
            List<Coordinate> shipCoords = ShipCoords(i, j);

            foreach (Coordinate shipCoord in shipCoords)
            {
                if (cellstatus[shipCoord.X, shipCoord.Y] == CellStatus.Ship)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Coordinate> nearShipPoints(int i, int j)
        {
            List<Coordinate> nearShipPoints = new List<Coordinate>();

            foreach (Coordinate shipCoord in ShipCoords(i, j))
            {
                foreach (Coordinate relativeCoord in relativeCoords)
                {
                    int x = shipCoord.X + relativeCoord.X;
                    int y = shipCoord.Y + relativeCoord.Y;
                    if (x >= 0 && x < 10 && y >= 0 && y < 10)
                    {
                        if (cellstatus[x, y] == CellStatus.Empty)
                        {
                            Coordinate p = new Coordinate(x, y);
                            if (!nearShipPoints.Contains(p))
                            {
                                nearShipPoints.Add(p);
                            }
                        }
                    }
                }
            }
            return nearShipPoints;
        }
    }
}
