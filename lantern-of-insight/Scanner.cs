using Microsoft.Win32.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Scanner
    {
        public static void RunScan()
        {
            IntPtr hWnd = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Magic: The Gathering Online");
            Console.WriteLine($"MTGO window handle: {hWnd}");

            uint processId = 0;
            Win32.GetWindowThreadProcessId(hWnd, out processId);

            Console.WriteLine($"MTGO process ID: {processId}");

            IntPtr hProcess = Win32.OpenProcess(Win32.ProcessAccessFlags.All, false, Convert.ToInt32(processId));
            Console.WriteLine($"MTGO process handle: {hProcess}");

            // MTGO seems to only use 2 GB of memory, so we can stop at address
            // 0x80000000.
            const uint PAGE_SIZE        = 4 * 1024;
            const uint STARTING_ADDRESS = 0x0;
            const uint ENDING_ADDRESS   = 0x80000000;
            const uint STARTING_PAGE    = STARTING_ADDRESS / PAGE_SIZE;
            const uint ENDING_PAGE      = ENDING_ADDRESS   / PAGE_SIZE;

            byte[] buffer = new byte[PAGE_SIZE];

            for (uint pageNum = STARTING_PAGE; pageNum <= ENDING_PAGE; ++pageNum)
            {
#if DEBUG
                // Sanity check that we are calculating only 32-bit addresses.
                UInt64 u64PageNum     = pageNum;
                UInt64 u64PageSize    = PAGE_SIZE;
                UInt64 u64BaseAddress = u64PageNum * u64PageSize;
                Debug.Assert(u64BaseAddress == Convert.ToUInt32(u64BaseAddress));
#endif

                try
                {
                    IntPtr ipBaseAddress = new IntPtr(pageNum * PAGE_SIZE);
                    int    numBytesRead  = 0;

                    Win32Safe.ReadProcessMemory(
                        hProcess,
                        ipBaseAddress,
                        buffer,
                        Convert.ToUInt32(buffer.Length),
                        out numBytesRead);

                    Console.WriteLine("Read {numBytesRead} bytes at address {ipBaseAddress}");
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    // We expect to read restricted areas of memory so just
                    // ignore those errors.
                    if (ResultWin32.ERROR_PARTIAL_COPY != e.NativeErrorCode)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
