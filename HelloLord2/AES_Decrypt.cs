using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public partial class Klasy
{
    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, byte[] saltBytes)
    {
        byte[] decryptedBytes = null;

        //byte[] bytesToBeDecrypted;
        //byte[] passwordBytes;
        //byte[] saltBytes;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        //byte[] saltBytes = new byte[] { 1, 3, 2, 4, 5, 7, 6, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }

    //public static string DecryptFile(string fileEncrypted, string password, string salt)
    public static string DecryptFile(string fileEncrypted, byte[] passwordBytes, string salt)
    {
        try
        {
            string ToBeDecrypted = "";
            using (System.IO.StreamReader file = new StreamReader(@fileEncrypted))
            {
                ToBeDecrypted = file.ReadLine();
            }
            byte[] bytesToBeDecrypted = Convert.FromBase64String(ToBeDecrypted);
            //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes, saltBytes);

            //string file = "C:\\SampleFile.DLL";
            //File.WriteAllBytes(file, bytesDecrypted);
            string content = Encoding.UTF8.GetString(bytesDecrypted);
            return content;
        }
        catch (Exception e)
        {
            return "";
        }
        
    }
}

