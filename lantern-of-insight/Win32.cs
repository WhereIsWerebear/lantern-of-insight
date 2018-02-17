﻿using System;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,[In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    }

    class Win32Safe
    {
        public static void ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,[In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead)
        {
            int readProcessMemoryReturn = Win32.ReadProcessMemory(
                hProcess,
                lpBaseAddress,
                buffer,
                size,
                out lpNumberOfBytesRead);

            if (0 == readProcessMemoryReturn)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}
