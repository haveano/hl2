using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace upd
{
    class Program
    {
        static void Main(string[] args)
        {
            //ustawianie "working directory" dla autostartu z rejestru:
            //https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string path = Path.GetDirectoryName(uri.LocalPath);
            Directory.SetCurrentDirectory(path);

            //w args[0] jest nazwa procesu, z którego updater został uruchomiony,
            //updater sprawdza czy ten proces jeszcze istnieje i czeka na jego zamnknięcie
            //potem kopiuje ten proces na ten podeny jako args[1]
            string proces = args[0];
            string fileNameSource = args[1];
            string fileNameDest = args[0] + ".exe";

            for (int n = 1; n <= 60; n = n++)
            {
                if (!File.Exists(fileNameSource))
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    n = 100;
                }
            }

            for (int i = 1; i <= 60; i = i++)
            {
                if (Process.GetProcessesByName(proces).Length > 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    i = 100;
                }
            }

            File.Copy(fileNameSource, fileNameDest, true);
        }
    }
}
