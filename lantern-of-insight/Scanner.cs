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

            IntPtr pSidOwner = new IntPtr();
            IntPtr pSidGroup = new IntPtr();
            IntPtr pDacl = new IntPtr();
            IntPtr pSacl = new IntPtr();
            IntPtr pSecurityDescriptor = new IntPtr();

            Win32Safe.GetSecurityInfo(
                hProcess,
                Win32.SE_OBJECT_TYPE.SE_KERNEL_OBJECT,
                Win32.SECURITY_INFORMATION.DACL_SECURITY_INFORMATION,
                out pSidOwner,
                out pSidGroup,
                out pDacl,
                out pSacl,
                out pSecurityDescriptor);

#if false
            const uint BUFFER_SIZE = 128;
            byte[] buffer = new byte[BUFFER_SIZE];
            IntPtr numBytesRead = new IntPtr();

            Win32Safe.ReadProcessMemory(
                hProcess,
                new IntPtr(0x101038b0),
                buffer,
                Convert.ToUInt32(buffer.Length),
                out numBytesRead);

#endif        }
        }
}
