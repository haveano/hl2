using System;

public partial class Klasy
{
    public static void ZapiszDoPliku(string linia, string nazwa_pliku)
    {
        try
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@nazwa_pliku, true))
            {
#if DEBUG
                Console.WriteLine("### DEBUG (ZapiszDoPliku) 010");
                Console.WriteLine(string.Format("Zapisuje do pliku \"{0}\" linie: \"{1}\"", nazwa_pliku, linia));
                Console.WriteLine("### end DEBUG (ZapiszDoPliku) 010\n");
#endif
                file.WriteLine(linia);
            }
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine("### DEBUG (ZapiszDoPliku) 020");
            Console.WriteLine("[ERR0r] Exception caught!!!");
            Console.WriteLine("[ERR0r] Source : " + e.Source);
            Console.WriteLine("[ERR0r] Message : " + e.Message);
            Console.WriteLine("### end DEBUG (ZapiszDoPliku) 020\n");
#endif
        }
    }
}
