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

            uint processId = 0;
            Win32.GetWindowThreadProcessId(hWnd, out processId);

            Console.WriteLine($"MTGO process ID: {processId}");

            IntPtr hProcess = Win32.OpenProcess(Win32.ProcessAccessFlags.All, false, Convert.ToInt32(processId));
            Console.WriteLine($"MTGO process handle: {hProcess}");

            // STARTING_ADDRESS and ENDING_ADDRESS were determined by scanning
            // the Windows 64-bit 8-terabyte address range, [0x000'00000000,
            // 0x7FF'FFFFFFFF] (' is the 4-byte separator) [1]. STARTING_ADDRESS
            // and ENDING_ADDRESS make up the only readable range of memory.
            //
            // [1] https://docs.microsoft.com/en-us/windows-hardware/drivers/gettingstarted/virtual-address-spaces
            const uint PAGE_SIZE        = 4 * 1024;
            const uint STARTING_ADDRESS = 0x240000;
            const uint ENDING_ADDRESS   = 0x7ffe0000;
            const uint STARTING_PAGE    = STARTING_ADDRESS / PAGE_SIZE;
            const uint ENDING_PAGE      = ENDING_ADDRESS / PAGE_SIZE;


            byte[] buffer       = new byte[PAGE_SIZE];
            IntPtr numBytesRead = new IntPtr();

            for (uint pageNum = STARTING_PAGE; pageNum <= ENDING_PAGE; ++pageNum)
            {
                Int64 i64BaseAddress = pageNum * PAGE_SIZE;

                Win32Safe.ReadProcessMemory(
                    hProcess,
                    i64BaseAddress,
                    buffer,
                    Convert.ToUInt32(buffer.Length),
                    out numBytesRead);

                if (0 < numBytesRead.ToInt32())
                {
                    Console.WriteLine("Read {numBytesRead}  bytes at address {i64BaseAddress}");
                }
            }

        }
    }
}
