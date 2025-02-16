using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Battleship_WPF.DataPackage;
using Battleship_WPF.Logic;
using Battleship_WPF.Utils;

namespace Battleship_WPF.Network
{
    public class Server
    {
        private object queueArrayLock = new object();
        private int clientID = 0;
        private LinkedList<string>[] queueArray = new LinkedList<string>[2];
        private bool close = false;
        private GameLogic gameLogic = null;
        private Socket sSocket = null;

        private List<Socket> socketsList = new List<Socket>(2);

        public void addMessageToQueue(string message, int ID)
        {
            lock (queueArrayLock)
            {
                if (ID != -1)
                    queueArray[ID].AddLast(message + "<EOF>");
            }
        }

        public void Close()
        {
            string finalMessage = DataConverter.encode(new ChatData { RecipientID = 1, ClientID = -1, Message = "Enemy left the game." });
            byte[] message = Encoding.UTF8.GetBytes(finalMessage + "<EOF>");

            try
            {
                if (socketsList.Count > 1)
                    socketsList[1].Send(message);
            }
            catch (Exception) { }
                

            close = true;
            if (sSocket != null)
                sSocket.Close();
        }

        public Server(int port)
        {
            Console.WriteLine("Opening server on port " + port);
            for (int i = 0; i < 2; ++i)
            {
                queueArray[i] = new LinkedList<string>();
            }
            gameLogic = new GameLogic();
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(localEndPoint);
            sSocket.Listen(4);

            Thread messageDistributorThread = new Thread(() =>
            {
                while (!close)
                {
                    Thread.Sleep(10);
                    lock (GameLogic.queueLock)
                    {
                        while (gameLogic.messageQueue.Count != 0)
                        {
                            string messageToClient = gameLogic.messageQueue.First.Value;
                            gameLogic.messageQueue.RemoveFirst();
                            if (messageToClient != null)
                            {
                                Data decoded = DataConverter.decode(messageToClient);
                                int recipient = decoded.RecipientID;
                                addMessageToQueue(messageToClient, recipient);
                            }
                        }
                    }
                }
            });
            messageDistributorThread.Start();

            for (int i = 0; i < 4; ++i)
                ServeClient();
        }

        private void ServeClient()
        {
            Thread thread = new Thread(() =>
            {
                Socket socket = null;
                try
                {
                    while (!close)
                    {
                        socket = sSocket.Accept();
                        socketsList.Add(socket);

                        byte[] buffer = new byte[10240];
                        string inMsg = null;
                        bool isClient = false;

                        if (!isClient)
                        {
                            if (getInMsg(socket) != "CLIENT")
                            {
                                socket.Close();
                                Console.WriteLine("PING received");
                                continue;
                            }
                            else isClient = true;
                        }

                        int ID = clientID++;
                        int otherQueueID = (ID == 0) ? 1 : 0;
                        int ownQueueID = (ID == 0) ? 0 : 1;
                        Console.WriteLine("Client " + ID + " joined the server.");

                        ConnectionData cData = new ConnectionData(ID);

                        addMessageToQueue(DataConverter.encode(cData), otherQueueID);

                        Thread messageProcessingThread = new Thread(() =>
                        {
                            while (!close)
                            {
                                Thread.Sleep(10);
                                try
                                {
                                    inMsg = getInMsg(socket);
                                    gameLogic.processMessage(DataConverter.decode(inMsg));
                                }
                                catch (Exception) { }
                            }
                        });
                        messageProcessingThread.Start();

                        while (!close)
                        {
                            Thread.Sleep(10);
                            lock (queueArrayLock)
                            {
                                while (queueArray[ownQueueID].Count != 0)
                                {
                                    string queueMsg = queueArray[ownQueueID].First.Value;
                                    queueArray[ownQueueID].RemoveFirst();
                                    byte[] bytes = Encoding.UTF8.GetBytes(queueMsg);
                                    socket.Send(bytes);
                                }
                            }
                        }
                    }
                }
                catch (Exception) { }
            });
            thread.Start();
        }

        public static bool isServerAvailable(string ip, int port)
        {
            bool isAvailable = false;

            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = socket.BeginConnect(IPAddress.Parse(ip), port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(1000, true);

                if (socket.Connected)
                {
                    socket.EndConnect(result);
                    socket.Close();
                    isAvailable = true;
                }
                else
                {
                    socket.Close();
                    isAvailable = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.StackTrace);
                isAvailable = false;
            }
            return isAvailable;
        }

        public static string getLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "NO IP FOUND";
        }

        public static List<string> getLocalIPs()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ipList = new List<string>();
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ipList.Add(ip.ToString() + ":" + Settings.getPort());

            return ipList;
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

                    if (inMsg.Contains("<EOF>"))
                    {
                        inMsg = inMsg.Replace("<EOF>", "");
                        break;
                    }
                }
            }
            catch { Close(); }

            return inMsg;
        }
    }
}
