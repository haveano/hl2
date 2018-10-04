using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Klasy
{
    public static void PostFunkcjaHTTPSoverSSL(string front_domain, string real_domain, string url,
    Dictionary<string, string> myDictionary)
    {
        try
        {
            IPAddress ip_address;
            if (IPAddress.TryParse(front_domain, out ip_address))
            {
                //Valid IP, with address containing the IP
                front_domain = ip_address.ToString();
            }
            else
            {
                //Invalid IP - not IP address
                IPHostEntry hostEntry1 = Dns.GetHostEntry(front_domain);
            }

            if (front_domain.Length == 0)
            {
                front_domain = real_domain;
            }
            //jesli sie uda rozwiazac nazwe, to znaczy ze polaczenie dziala,
            //jesli sie nie powiedzie, to socketexception obsluzy blad:
            Task<string> task = PostRequestHTTPSoverSSL(front_domain, url, real_domain, myDictionary);
            task.Wait();
#if DEBUG
            Console.WriteLine("### DEBUG (PostFunkcja) 010: wyrzucenie na ekran task.Result");
            //wyrzuca na konsole to co zwraca funkcja PostRequest,
            //czyli wyrzuca to co serwer POST zwróci:
            Console.WriteLine(task.Result);             //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ZapiszDoPliku(task.Result, "log.txt");
            Console.WriteLine("### end DEBUG (PostFunkcja) 010\n");
#endif
        }
        catch (SocketException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (PostFunkcja) 020");
            Console.WriteLine("[ERR0r] SocketException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            ZapiszDoPliku(e.Source + " " + e.Message, "log.txt"); //!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("### end DEBUG (PostFunkcja) 020\n");
#endif
        }
        catch (ArgumentNullException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (Postunkcja) 030");
            Console.WriteLine("[ERR0r] ArgumentNullException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            ZapiszDoPliku(e.Source + " " + e.Message, "log.txt");   //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("### end DEBUG (Postunkcja) 030\n");
#endif
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (PostFunkcja) 040");
            Console.WriteLine("[ERR0r] Exception caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            ZapiszDoPliku(e.Source + " " + e.Message, "log.txt");   //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("### end DEBUG (Postunkcja) 040\n");
#endif
        }
    }

    static async Task<string> PostRequestHTTPSoverSSL(string front_domain, string url, string real_domain,
        Dictionary<string, string> myDictionary)
    {
        var tcs = new TaskCompletionSource<string>();
        try
        {
            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
            cmd.RedirectStandardOutput = true;
            cmd.RedirectStandardError = true;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            //string command_arg = "/C socat.exe TCP-LISTEN:28899 OPENSSL:" + front_domain + ":443,verify=0";
            //string command_arg = "/C socat.exe TCP-LISTEN:25899,fork,reuseaddr TCP:" + front_domain + ":443";
            string command_arg = "/C socat.exe TCP-LISTEN:25899 OPENSSL:" + front_domain + ":443,verify=0";
            cmd.Arguments = command_arg;

            Process proc_cmd = Process.Start(cmd);


            HttpContent qqq = new FormUrlEncodedContent(myDictionary);
            using (HttpClient client = new HttpClient())
            {
                if (front_domain != real_domain)
                {
                    client.DefaultRequestHeaders.Host = real_domain;
                }
                using (HttpResponseMessage response = await client.PostAsync("http://localhost:25899/" + url, qqq))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
#if DEBUG
                        Console.WriteLine("### DEBUG (PostRequest) 010");
                        Console.WriteLine(mycontent);
                        //zwraca to co wyrzuca serwer POST, na który się robi POST:
                        Console.WriteLine("### end DEBUG (PostRequest) 010\n");
                        //return mycontent;
                        ZapiszDoPliku(mycontent, "log.txt");   //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
#endif
                        tcs.SetResult("Success");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            tcs.SetResult(ex.Message);
        }
        return await tcs.Task;
    }
}
