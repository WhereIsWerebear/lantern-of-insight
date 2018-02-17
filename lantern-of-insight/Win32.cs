using System;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,[In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
    }
}
