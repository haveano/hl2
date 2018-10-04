using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;


public partial class Klasy
{
    public static string GetFunkcjaHTTPSoverSSL(string front_domain, string real_domain, string url)
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
                //Invalid IP
                IPHostEntry hostEntry1 = Dns.GetHostEntry(front_domain);
            }

            if (front_domain.Length == 0)
            {
                front_domain = real_domain;
            }
            //jesli sie uda rozwiazac nazwe, to znaczy ze polaczenie dziala,
            //jesli sie nie powiedzie, to socketexception obsluzy blad:
            Task<string> task = GetRequestHTTPSoverSSL(front_domain, url, real_domain);
            task.Wait();
#if DEBUG
            Console.WriteLine("### DEBUG (GetFunkcja) 010: wyrzucenie na ekran task.Result");
            Console.WriteLine(task.Result);
            Console.WriteLine("### end DEBUG (GetFunkcja) 010\n");
#endif
            return (task.Result);
        }
        catch (SocketException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFunkcja) 020");
            Console.WriteLine("[ERR0r] SocketException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFunkcja) 020\n");
#endif
            return null;
        }
        catch (ArgumentNullException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFunkcja) 030");
            Console.WriteLine("[ERR0r] ArgumentNullException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFunkcja) 030\n");
#endif
            return null;
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFunkcja) 040");
            Console.WriteLine("[ERR0r] Exception caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFunkcja) 040\n");
#endif
            return null;
        }
    }

    static async Task<string> GetRequestHTTPSoverSSL(string front_domain, string url, string real_domain)
    {
        var tcs = new TaskCompletionSource<string>();
        try
        {
            ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe");
            cmd.RedirectStandardOutput = true;
            cmd.RedirectStandardError = true;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            string command_arg = "/C socat.exe TCP-LISTEN:23899 OPENSSL:" + front_domain + ":443,verify=0";
            //string command_arg = "/C socat.exe TCP-LISTEN:23899,fork,reuseaddr TCP:" + front_domain + ":443";
            cmd.Arguments = command_arg;

            Process proc_cmd = Process.Start(cmd);


            using (HttpClient client = new HttpClient())
            {
                
                if (front_domain != real_domain)
                {
                    client.DefaultRequestHeaders.Host = real_domain;
                }
                
                using (HttpResponseMessage response = await client.GetAsync("http://localhost:23899/" + url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
#if DEBUG
                        //wyrzuca na ekran zawartosc pliku, który czyta za pomoca GET:
                        Console.WriteLine("### DEBUG (GetRequest) 010: wyrzucenie na ekran \"mycontent\"");
                        Console.WriteLine(mycontent);
                        Console.WriteLine("### end DEBUG (GetRequest) 010\n");
#endif
                        tcs.SetResult(mycontent);
                    }
                }
            }
            /*
            foreach (var process in Process.GetProcessesByName("socat"))
            {
                process.Kill();
            }
            */
        }
        catch (Exception ex)
        {
            tcs.SetResult(ex.Message);
        }
        return await tcs.Task;
    }
}