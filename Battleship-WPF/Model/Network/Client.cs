using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Battleship_WPF.DataPackage;
using Battleship_WPF.EventArguments.Client;

namespace Battleship_WPF.Network
{
    public class Client
    {
        public int ID;

        private object messageQueueLock = new object();
        private LinkedList<string> messageQueue = new LinkedList<string>();
        private string ip;
        private int port;
        private bool close = false;
        private bool timedOut = false;
        public event EventHandler<MessageReceivedArgs> OnMessageReceived;
        public event EventHandler<GameEndedArgs> OnGameEnded;
        public event EventHandler<EnemyHitMeArgs> OnEnemyHitMe;
        public event EventHandler<MyHitArgs> OnMyHit;
        public event EventHandler OnYourTurn, OnJoinedEnemy, OnDisconnected, OnRematch;

        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            Thread thread = new Thread(() =>
            {
                run();
            });
            thread.Start();
        }

        public bool isTimeout()
        {
            return timedOut;
        }

        public void sendMessage(Data data)
        {
            string message = DataConverter.encode(data);
            //Console.WriteLine(message);
            lock (messageQueueLock)
            {
                messageQueue.AddLast(message + "<EOF>");
            }
            
        }

        public void Close()
        {
            close = true;
        }

        private void run()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.Connect(serverAddress);

                byte[] bytes = Encoding.UTF8.GetBytes("CLIENT<EOF>");
                socket.Send(bytes);

                Thread thread = new Thread(() =>
                {
                    while (!close)
                    {
                        Thread.Sleep(10);
                        string inMsg = getInMsg(socket);
                        //if (int.TryParse(inMsg, out int ID))
                        //{
                        //    this.ID = ID;
                        //    continue;
                        //}

                        if (inMsg != null)
                        {
                            if (Regex.Matches(inMsg, "}{").Count > 0)
                                processList(inMsg);
                            else
                                process(inMsg);
                        }
                    }
                });
                thread.Start();

                while (!close)
                {
                    Thread.Sleep(10);
                    lock (messageQueueLock)
                    {
                        while (messageQueue.Count != 0)
                        {

                            string message = messageQueue.First.Value;
                            messageQueue.RemoveFirst();
                            socket.Send(Encoding.UTF8.GetBytes(message));
                        }
                    }
                }
                try
                {
                    socket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.StackTrace);
                timedOut = true;
            }
        }

        private string getInMsg(Socket socket)
        {
            byte[] buffer = new byte[10240];
            string inMsg = null;
            try
            {
                while (true)
                {
                    int numByte = socket.Receive(buffer);
                    inMsg += Encoding.UTF8.GetString(buffer, 0, numByte);

                    if (inMsg.IndexOf("<EOF>") > -1)
                    {
                        inMsg = inMsg.Replace("<EOF>", "");
                        break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Szerver elpusztult amíg a kliens rajta volt. Ez nem egy hiba.");
                Close();
            }

            //Console.WriteLine(inMsg);
            return inMsg;
        }

        private void process(string inMsg)
        {
            Data data = DataConverter.decode(inMsg);
            if (data == null)
            {
                Console.WriteLine("Nem tudtam dekódolni a client osztályban:\n" + inMsg);
                return;
            }
            //Console.WriteLine(data.GetType().Name);
            switch (data.GetType().Name)
            {
                case "ChatData":
                    OnMessageReceived?.Invoke(null, new MessageReceivedArgs(data.ClientID, ((ChatData)data).Message));
                    break;
                case "PlaceShipsData":
                    //PlaceShipsData
                    break;
                case "ConnectionData":
                    ID = (data.ClientID == 0) ? 1 : 0;
                    OnJoinedEnemy?.Invoke(null, EventArgs.Empty);
                    break;
                case "ShotData":
                    if (((ShotData)data).RecipientID == ID)
                        OnEnemyHitMe(null, new EnemyHitMeArgs(((ShotData)data).I, ((ShotData)data).J));
                    break;
                case "CellData":
                    OnMyHit(null, new MyHitArgs(((CellData)data).I, ((CellData)data).J, ((CellData)data).Status));
                    break;
                case "TurnData":
                    OnYourTurn(null, EventArgs.Empty);
                    break;
                case "GameEndedData":
                    OnGameEnded(null, new GameEndedArgs(((GameEndedData)data).Status));
                    break;
                case "DisconnectData":
                    OnDisconnected(null, EventArgs.Empty);
                    break;
                case "RematchData":
                    OnRematch(null, EventArgs.Empty);
                    break;
                default:
                    //NOT IMPLEMENTED
                    Console.WriteLine("Nincs implementálva a Client-ben az alábbi osztály: " + data.GetType().Name);
                    break;
            }
        }

        private void processList(string inMsg)
        {
            int charValue = 1;
            char delimeter;
            bool found = true;
            do
            {
                delimeter = (char)charValue;
                if (inMsg.Contains(delimeter + ""))
                    ++charValue;
                else
                    found = false;
            } while (found);
            inMsg = inMsg.Replace("}{", "}" + delimeter + "{");
            string[] inMsgs = inMsg.Split(delimeter);
            foreach (var item in inMsgs)
            {
                process(item);
            }
        }
    }
}
