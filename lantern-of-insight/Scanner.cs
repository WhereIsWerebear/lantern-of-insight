using System;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Scanner
    {
        public static void RunScan()
        {
            IntPtr hwnd = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Magic: The Gathering Online");
            Console.WriteLine($"MTGO window handle: {hwnd}");

            const uint FOUR_KILOBYTES = 4 * 1024;
            byte[] buffer = new byte[FOUR_KILOBYTES];
            IntPtr numBytesRead = new IntPtr();

            int readProcessMemoryReturn = Win32.ReadProcessMemory(
                hwnd,
                IntPtr.Zero,
                buffer,
                Convert.ToUInt32(buffer.Length),
                out numBytesRead);

            if (0 == readProcessMemoryReturn)
            {
                string errorMessage = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
                Console.WriteLine($"Error in Win32.ReadProcessMemory(): {errorMessage}");
            }
        }
    }
}
