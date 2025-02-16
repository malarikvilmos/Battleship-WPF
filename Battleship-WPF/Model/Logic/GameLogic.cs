using System;
using System.Collections.Generic;
using System.Threading;
using Battleship_WPF.DataPackage;

namespace Battleship_WPF.Logic
{
    public class GameLogic
    {
        public static readonly object queueLock = new object();
        public LinkedList<string> messageQueue = new LinkedList<string>();
        private Random rnd = new Random();
        private Player[] players;

        public GameLogic()
        {
            players = new Player[2];
            players[0] = new Player();
            players[1] = new Player();
        }

        private void Reset()
        {
            foreach (Player player in players)
                player.Reset();
        }

        private void addMessage(Data data)
        {
            lock (queueLock)
            {
                messageQueue.AddLast(DataConverter.encode(data));
            }
        }

        private void addMessageToAll(Data data)
        {
            lock (queueLock)
            {
                data.RecipientID = 0;
                messageQueue.AddLast(DataConverter.encode(data));
                data.RecipientID = 1;
                messageQueue.AddLast(DataConverter.encode(data));
            }
        }

        public void processMessage(Data data)
        {
            if (data == null)
            {
                throw new Exception("Data is null");
            }
            switch (data.GetType().Name)
            {
                case "ChatData":
                    processChatData((ChatData)data);
                    break;
                case "PlaceShipsData":
                    setPlayerBoard((PlaceShipsData)data);
                    break;
                case "ConnectionData":
                    break;
                case "ShotData":
                    calcShot((ShotData)data);
                    break;
                case "DisconnectData":
                    data.RecipientID = data.ClientID == 1 ? 0 : 1;
                    addMessage((DisconnectData)data);
                    break;
                default:
                    Console.WriteLine("########## ISMERETLEN OSZTÁLY #########");
                    Console.WriteLine("Nincs implementálva a GameLogicban az alábbi osztály: " + data.GetType().Name);
                    throw new Exception("Not implemented");
            }
        }

        private void processChatData(ChatData data)
        {
            if (data.Message[0] == '/')//ha parancs
            {
                data.Message = data.Message.Substring(1);
                ChatData msg = new ChatData();
                msg.ClientID = -1;
                switch (data.Message)
                {
                    case "help":
                        msg.RecipientID = data.ClientID;
                        msg.Message = "Available commands:" +
                            "\n/help\t  Show all commands." +
                            "\n/rematch\t  Request rematch." +
                            "\n/score\t  Show score.";
                        addMessage(msg);
                        break;
                    case "rematch":
                        players[data.ClientID].wantRematch = true;

                        msg.Message = "The enemy wants to start a new game.";
                        msg.RecipientID = data.ClientID == 0 ? 1 : 0;
                        addMessage(msg);
                        msg.Message = "You want to start a new game.";
                        msg.RecipientID = data.ClientID;
                        addMessage(msg);

                        if (players[0].wantRematch == true && players[1].wantRematch == true)
                        {
                            msg.Message = "Rematch begins.";
                            addMessageToAll(msg);

                            Thread.Sleep(200);
                            RematchData rd = new RematchData();
                            rd.ClientID = -1;
                            addMessageToAll(rd);

                            Reset();
                        }
                        break;
                    case "score":
                        msg.RecipientID = data.ClientID;
                        msg.Message = String.Format("You won {0} times, and lost {1} times.", players[data.ClientID].Win, players[data.ClientID].Lose);
                        addMessage(msg);
                        break;
                    default:
                        msg.RecipientID = data.ClientID;
                        msg.Message = "Unknown command. Use /help to show available commands.";
                        addMessage(msg);
                        break;
                }
            }
            else //ha sima üzenet
            {
                addMessageToAll(data);
            }
        }

        private void calcShot(ShotData data)
        {

            int egyik = data.ClientID;
            int masik = egyik == 1 ? 0 : 1;

            ShotData sd = new ShotData(data.ClientID, data.I, data.J);
            sd.RecipientID = masik;
            addMessage(sd);

            CellData cd = new CellData(-1, data.I, data.J, players[masik].Board.cellstatus[data.I, data.J]);
            cd.RecipientID = egyik;
            addMessage(cd);

            if (players[masik].Board.cellstatus[data.I, data.J] == CellStatus.Ship)
            {
                players[masik].Board.cellstatus[data.I, data.J] = CellStatus.ShipHit;
                if (players[masik].Board.isSunk(data.I, data.J))
                {
                    hitNear(egyik, masik, data.I, data.J);
                }
                if (isWin(players[masik]))
                {
                    ++players[egyik].Win;
                    ++players[masik].Lose;
                    addMessage(new GameEndedData(GameEndedStatus.Win, egyik));
                    addMessage(new GameEndedData(GameEndedStatus.Defeat, masik));
                    ChatData msg = new ChatData();
                    msg.ClientID = -1;
                    msg.Message = "Use /rematch to play again.";
                    addMessageToAll(msg);
                }
                else
                {
                    addMessage(new TurnData(egyik));
                }
            }
            else
            {
                addMessage(new TurnData(masik));
            }
        }

        private bool isWin(Player player)
        {
            if (player.Board.hasCellStatus(CellStatus.Ship))
                return false;

            return true;
        }

        private void hitNear(int egyik, int masik, int i, int j)
        {
            foreach (Coordinate nearShipPoint in players[masik].Board.nearShipPoints(i, j))
            {
                CellData cd = new CellData(-1, nearShipPoint.X, nearShipPoint.Y, players[masik].Board.cellstatus[nearShipPoint.X, nearShipPoint.Y]);
                cd.RecipientID = egyik;
                addMessage(cd);
                ShotData sd = new ShotData(egyik, nearShipPoint.X, nearShipPoint.Y);
                sd.RecipientID = masik;
                addMessage(sd);
            }
        }

        private void setPlayerBoard(PlaceShipsData data)
        {
            if (data.ClientID == 0)
            {
                players[0].Identifier = data.ClientID;
                players[0].isReady = true;
                players[0].Board = data.Board;
            }
            else
            {
                players[1].Identifier = data.ClientID;
                players[1].isReady = true;
                players[1].Board = data.Board;
            }

            if (players[0].isReady == true && players[1].isReady == true)
            {
                addMessage(new TurnData(rnd.Next(2)));
            }
        }
    }
}
