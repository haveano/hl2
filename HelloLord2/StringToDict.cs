using System;
using System.Collections.Generic;
using System.Linq;

public partial class Klasy
{
    public static Dictionary<string, string> StringToDict(string mycontent)
    {
        mycontent = mycontent.Replace("\n", String.Empty);
        mycontent = mycontent.Replace("\r", String.Empty);
        mycontent = mycontent.Replace("\t", String.Empty);
        mycontent = mycontent.Replace(" ", String.Empty);

        var dict = mycontent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(part => part.Split('='))
        .ToDictionary(split => split[0], split => split[1]);
#if DEBUG
        Console.WriteLine("### DEBUG (StringToDict) 010: wyrzucenie na ekran slownika");
        //wyrzuca na ekran zawartosc pliku, który czyta za pomoca GET:
        foreach (KeyValuePair<string, string> pair in dict)
        {
            Console.WriteLine(string.Format("Key: {0} Values: {1}", pair.Key, pair.Value));
        }
        Console.WriteLine("### end DEBUG (StringToDict) 010\n");
#endif
        return dict;
    }
}
