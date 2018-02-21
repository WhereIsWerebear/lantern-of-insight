using Microsoft.Win32.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Scanner
    {
        const uint PAGE_SIZE = 4 * 1024;
        static IntPtr ProcessHandle = IntPtr.Zero;

        public static void RunScan()
        {
            IntPtr hWnd = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Magic: The Gathering Online");
            Console.WriteLine($"MTGO window handle: {hWnd}");

            uint processId = 0;
            Win32.GetWindowThreadProcessId(hWnd, out processId);

            Console.WriteLine($"MTGO process ID: {processId}");

            ProcessHandle = Win32.OpenProcess(Win32.ProcessAccessFlags.All, false, Convert.ToInt32(processId));
            Console.WriteLine($"MTGO process handle: {ProcessHandle}");

            // MTGO seems to only use 2 GB of memory, so we can stop at address
            // 0x80000000.
            const uint ENDING_ADDRESS = 0x80000000;
            const uint ENDING_PAGE_NUM = ENDING_ADDRESS / PAGE_SIZE;

            byte[] buffer = new byte[PAGE_SIZE];
            uint startingPageNum = 0;
            for (uint pageNum = 0; pageNum < ENDING_PAGE_NUM; ++pageNum)
            {
                if (0 < ReadPageIntoBuffer(pageNum, ref buffer))
                {
                    startingPageNum = pageNum;
                    break;
                }
            }

            for (uint pageNum = startingPageNum; pageNum < ENDING_PAGE_NUM; ++pageNum)
            {
                int numBytesRead = ReadPageIntoBuffer(pageNum, ref buffer);

                if (0 == numBytesRead)
                {
                    Console.WriteLine($"Reached page {pageNum} which could not be read.  Stopping reading from memory.");
                    break;
                }

                Console.WriteLine($"Read {numBytesRead} bytes at page {pageNum}.");
            }
        }

        static int ReadPageIntoBuffer(uint pageNum, ref byte[] buffer)
        {
#if DEBUG
            // Sanity check that we are calculating only 32-bit addresses.
            Int64 i64BaseAddress =
                Convert.ToInt64(pageNum)
                * Convert.ToInt64(PAGE_SIZE);

            Debug.Assert(i64BaseAddress == Convert.ToInt32(i64BaseAddress));
#endif

            IntPtr ipBaseAddress = new IntPtr(pageNum * PAGE_SIZE);
            int numBytesRead = 0;

            int returnCode = Win32.ReadProcessMemory(
                ProcessHandle,
                ipBaseAddress,
                buffer,
                Convert.ToUInt32(buffer.Length),
                out numBytesRead);

            if (0 == returnCode)
            {
                return 0;
            }
            else
            {
                return numBytesRead;
            }
        }
    }
}