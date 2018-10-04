
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

public partial class Klasy
{
    public static void GetFileFunkcjaSSL(string front_domain, string real_domain, string filename, string filename_dest)
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
            //zwrocic uwage, ze w GetFile filename jest jako 2-gi parametr, a w GetFileFunkcja jako trzeci
            Task<string> task_get = GetFileSSL(front_domain, filename, real_domain, filename_dest);
            task_get.Wait();
#if DEBUG
            Console.WriteLine("### DEBUG (GetFileFunkcja) 010: wyrzucenie na ekran task_get.Result");
            //wyrzuca na konsole "Success" czyli wynik z funkcjo GetFile:
            Console.WriteLine(task_get.Result);
            Console.WriteLine("### end DEBUG (GetFileFunkcja) 010\n");
#endif
        }
        catch (SocketException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFileFunkcja) 020");
            Console.WriteLine("[ERR0r] SocketException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFileFunkcja) 020\n");
#endif
        }
        catch (ArgumentNullException e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFileFunkcja) 030");
            Console.WriteLine("[ERR0r] ArgumentNullException caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFileFunkcja) 030\n");
#endif
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFileFunkcja) 040");
            Console.WriteLine("[ERR0r] Exception caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (GetFileFunkcja) 040\n");
#endif
        }
    }


    static async Task<string> GetFileSSL(string front_domain, string filename, string real_domain, string filename_dest)
    {
        
        if (File.Exists(filename_dest))
        {
#if DEBUG
            Console.WriteLine("### DEBUG (GetFile) 002");
            Console.WriteLine("sprawdzic czy jest plik");
            Console.ReadKey();
            Console.WriteLine("### end DEBUG (GetFile) 002");
#endif

            File.Delete(filename_dest);

#if DEBUG
            Console.WriteLine("### DEBUG (GetFile) 004");
            Console.WriteLine("sprawdzi czy juz nie ma pliku");
            Console.ReadKey();
            Console.WriteLine("### end DEBUG (GetFile) 004");
#endif
        }

        var tcs = new TaskCompletionSource<string>();
        try
        {
            using (var client = new HttpClient())
            {
                if (front_domain != real_domain)
                {
                    client.DefaultRequestHeaders.Host = real_domain;
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, "https://" + front_domain + "/" + filename))
                {
                    using (
                        Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename_dest, FileMode.Create, FileAccess.Write, FileShare.None, 1, true))
                    {
                        await contentStream.CopyToAsync(stream);
#if DEBUG
                        Console.WriteLine("### DEBUG (GetFile) 010");
                        //wielkosc pliku:
                        Console.WriteLine("wielkosc pliku" + stream.Length);
                        Console.WriteLine("### endif DEBUG (GetFile) 010");
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
