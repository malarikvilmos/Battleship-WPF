using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Media;

namespace Battleship_WPF.Utils
{
    public static class Settings
    {
        public static int port = 25564;
        public static string BackgroundColor = Color.FromRgb(37, 57, 66).ToString();

        private static readonly FieldInfo[] fields = typeof(Settings).GetFields();
        private static int lineCount = 0;
        private static readonly char sep = Path.DirectorySeparatorChar;

        static Settings()
        {
            Read();
        }

        private static void Read()
        {
            string dir = Directory.GetCurrentDirectory();
            StreamReader file = null;
            try
            {
                file = new StreamReader(dir + $"{sep}settings.cfg");
                while (file.Peek() >= 0)
                {
                    string line = file.ReadLine();
                    foreach (FieldInfo variable in fields)
                    {
                        SetValue(line, variable);
                    }
                }
                file.Close();
                if (lineCount != fields.Length)
                {
                    Console.WriteLine("Potenciális elírás a settings.cfg fájlban...");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(dir + $"{sep}settings.cfg fájl nem létezik");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba lépett fel a settings.cfg fájl olvasása közben...\nhibaüzenet: " + ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
                Save();
            }
        }

        private static void SetValue(string line, FieldInfo variable)
        {
            if (line.Contains(variable.Name))
            {
                if (int.TryParse(line.Split(' ')[1], out int parsed))
                    variable.SetValue(null, parsed);
                else 
                    variable.SetValue(null, line.Split(' ')[1]);
                ++lineCount;
            }
        }

        public static void Save()
        {
            string dir = Directory.GetCurrentDirectory();
            StreamWriter file = new StreamWriter(dir + $"{sep}settings.cfg");
            foreach (FieldInfo item in fields)
            {
                file.WriteLine(item.Name + " " + item.GetValue(null));
            }
            file.Close();
        }

        public static string getIP()
        {
            return IPAddress.Loopback.ToString();
        }

        public static int getPort()
        {
            return port;
        }

        public static void setPort(int _port)
        {
            port = _port;
        }

        public static Color getBackgroundColor()
        {
            Color color;
            try
            {
                color = (Color)ColorConverter.ConvertFromString(BackgroundColor);
            }
            catch (Exception)
            {
                BackgroundColor = Color.FromRgb(37, 57, 66).ToString();
                color = (Color)ColorConverter.ConvertFromString(BackgroundColor);
                Save();
            }
            return color;
        }

        public static void setBackgroundColor(Color color)
        {
            BackgroundColor = color.ToString();
        }
    }
}
