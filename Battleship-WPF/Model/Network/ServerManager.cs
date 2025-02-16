using System;
using System.Collections.Generic;
using System.IO;

namespace Battleship_WPF.Network
{
    public static class ServerManager
    {
        private static List<ServerAddress> serverList = new List<ServerAddress>();
        private static readonly char sep = Path.DirectorySeparatorChar;

        static ServerManager()
        {
            ReadServersFromFile();
        }

        private static void WriteSavedServersToFile()
        {
            try
            {
                string dir = Directory.GetCurrentDirectory();
                StreamWriter file = new StreamWriter(dir + $"{sep}servers.dat");
                file.WriteLine("Saved_Servers {");
                foreach (ServerAddress item in serverList)
                {
                    file.WriteLine("\t" + item);
                }
                file.WriteLine("}");
                file.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error writing file: " + ex.Message);
            }
        }

        private static void ReadServersFromFile()
        {
            try
            {
                string dir = Directory.GetCurrentDirectory();
                StreamReader file = new StreamReader(dir + $"{sep}servers.dat");

                while (file.Peek() > -1)
                {
                    string data = file.ReadLine();
                    if (data.Contains("Saved_Servers {"))
                    {
                        while (true)
                        {
                            string line = file.ReadLine();
                            if (line.Contains("}"))
                                break;
                            string[] strArr = line.Trim().Split('$');
                            if (strArr.Length != 3)
                                continue;
                            ServerAddress sAddress = new ServerAddress(strArr[0], strArr[1], int.Parse(strArr[2]));
                            serverList.Add(sAddress);
                        }
                    }
                }
                file.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("servers.dat file not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                WriteSavedServersToFile();
            }
        }

        public static List<ServerAddress> GetServers()
        {
            return serverList;
        }

        public static void AddServer(ServerAddress sAddress)
        {
            serverList.Add(sAddress);
            WriteSavedServersToFile();
        }

        public static void EditServer(string name, ServerAddress newAddress)
        {
            if(name != newAddress.Name)
            {
                for (int i = 0; i < serverList.Count; ++i)
                {
                    if(newAddress.Name == serverList[i].Name)
                    {
                        throw new Exception("SZERVER NÉV MÁR LÉTEZIK!");
                    }
                }
            }

            for (int i = 0; i < serverList.Count; ++i)
            {
                if(name == serverList[i].Name)
                {
                    serverList[i].Name = newAddress.Name;
                    serverList[i].IP = newAddress.IP;
                    serverList[i].Port = newAddress.Port;
                    WriteSavedServersToFile();
                    break;
                }
            }
        }

        public static void DeleteServer(ServerAddress sAddress)
        {
            for (int i = 0; i < serverList.Count; ++i)
            {
                if (serverList[i].Name == sAddress.Name)
                {
                    serverList.RemoveAt(i);
                    WriteSavedServersToFile();
                    break;
                }
            }
        }
    }
}
