using System;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Scanner
    {
        public static void RunScan()
        {
            IntPtr hWnd = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Magic: The Gathering Online");
            Console.WriteLine($"MTGO window handle: {hWnd}");

            IntPtr hProcess = Win32.GetProcessHandleFromHwnd(hWnd);
            Console.WriteLine($"MTGO process handle: {hProcess}");

            const uint BUFFER_SIZE = 128;
            byte[] buffer = new byte[BUFFER_SIZE];
            IntPtr numBytesRead = new IntPtr();

            Win32Safe.ReadProcessMemory(
                hProcess,
                new IntPtr(0x101038b0),
                buffer,
                Convert.ToUInt32(buffer.Length),
                out numBytesRead);
        }
    }
}
