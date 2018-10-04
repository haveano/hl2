using System.Security;
using System.Runtime.InteropServices;
using System;

public partial class Klasy
{
    public static byte[] SecureString()
    {
        SecureString secureString = new SecureString();
        secureString.AppendChar('J');   //1
        secureString.AppendChar('a');   //2
        secureString.AppendChar('k');   //3
        secureString.AppendChar('i');
        secureString.AppendChar('e');   //5
        secureString.AppendChar('s');
        secureString.AppendChar('B');
        secureString.AppendChar('a');
        secureString.AppendChar('r');
        secureString.AppendChar('d');   //10
        secureString.AppendChar('z');
        secureString.AppendChar('o');
        secureString.AppendChar('T');
        secureString.AppendChar('r');
        secureString.AppendChar('u');   //15
        secureString.AppendChar('d');
        secureString.AppendChar('n');
        secureString.AppendChar('e');
        secureString.AppendChar('H');
        secureString.AppendChar('a');
        secureString.AppendChar('s');   //20
        secureString.AppendChar('l');
        secureString.AppendChar('o');
        secureString.AppendChar('!');   //24

        byte[] secureStringBytes = null;
        // Convert System.SecureString to Pointer
        IntPtr unmanagedBytes = Marshal.SecureStringToGlobalAllocAnsi(secureString);
        try
        {
            unsafe
            {
                byte* byteArray = (byte*)unmanagedBytes.ToPointer();
                // Find the end of the string
                byte* pEnd = byteArray;
                while (*pEnd++ != 0) { }
                // Length is effectively the difference here (note we're 1 past end) 
                int length = (int)((pEnd - byteArray) - 1);
                secureStringBytes = new byte[length];
                for (int i = 0; i < length; ++i)
                {
                    // Work with data in byte array as necessary, via pointers, here
                    secureStringBytes[i] = *(byteArray + i);
                }
            }
        }
        finally
        {
            // This will completely remove the data from memory
            Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
        }
        return secureStringBytes;
    }
}
