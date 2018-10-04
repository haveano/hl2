
using Microsoft.Win32;
using System.IO;

public partial class Klasy
{
    //Funkcja sprawdza czy istnieje klucz, jesli nie istnieje to go tworzy,
    //jesli podana 
    public static string ConfigureRegistry(string key_path, string key, string value)
    {
        //here you specify where exactly you want your entry:
        RegistryKey currentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);

        var reg = currentUser.OpenSubKey(key_path, true);
        if (reg == null)
        {
            reg = currentUser.CreateSubKey(key_path);
        }

        if (value == "")
        {
            if (reg.GetValue(key) == null)
            {
                string random = GetRandomString();
                reg.SetValue(key, random);

                return random;
            }
            else
            {
                string klucz = reg.GetValue(key).ToString();
                return klucz;
            }
        }
        else
        {
            reg.SetValue(key, value);

            return value;
        }
    }

    static string GetRandomString()
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", ""); // Remove period.
        return path.Substring(0, 8);  // Return 11 character string
    }
}
