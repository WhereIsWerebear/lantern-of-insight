using Microsoft.Win32.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Scanner
    {
        const uint PAGE_SIZE = 4 * 1024;
        static byte[] Buffer = new byte[PAGE_SIZE];
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
            const uint STARTING_ADDRESS = 0x0;
            const uint ENDING_ADDRESS = 0x80000000;
            const uint STARTING_PAGE_NUM = STARTING_ADDRESS / PAGE_SIZE;
            const uint ENDING_PAGE_NUM = ENDING_ADDRESS / PAGE_SIZE;

            {
                uint leftPageNum = STARTING_PAGE_NUM;
                uint rightPageNum = ENDING_PAGE_NUM;

                while (true)
                {
                    uint pivotPageNum = (rightPageNum - leftPageNum) / 2;
                    uint prevPageNum = pivotPageNum - 1;

                    bool pivotPageIsReadable = IsPageReadable(pivotPageNum);
                    bool prevPageIsReadable = IsPageReadable(prevPageNum);

                    if (pivotPageIsReadable && !prevPageIsReadable)
                    {
                        // Success
                        return pivotPageNum;
                    }
                    else if (pivotPageIsReadable && prevPageIsReadable)
                    {
                        rightPageNum = pivotPageNum;
                    }
                    else if (!pivotPageIsReadable)
                }
            }

            for (uint pageNum = STARTING_PAGE; pageNum <= ENDING_PAGE; ++pageNum)
            {

                Console.WriteLine("Read {numBytesRead} bytes at address {ipBaseAddress}");
            }
        }

        static bool IsPageReadable(uint pageNum)
        {
#if DEBUG
            // Sanity check that we are calculating only 32-bit addresses.
            UInt64 u64BaseAddress =
                Convert.ToUInt64(pageNum)
                * Convert.ToUInt64(PAGE_SIZE);

            Debug.Assert(u64BaseAddress == Convert.ToUInt32(u64BaseAddress));
#endif

            try
            {
                IntPtr ipBaseAddress = new IntPtr(pageNum * PAGE_SIZE);
                int numBytesRead = 0;

                Win32Safe.ReadProcessMemory(
                    ProcessHandle,
                    ipBaseAddress,
                    Buffer,
                    Convert.ToUInt32(Buffer.Length),
                    out numBytesRead);

                return true;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                return false;
            }
        }
    }
}