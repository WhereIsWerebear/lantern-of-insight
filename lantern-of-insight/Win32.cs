using Microsoft.Win32.Interop;
using System;
using System.Runtime.InteropServices;

namespace lantern_of_insight
{
    class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,[In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("Oleacc.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        public enum SE_OBJECT_TYPE
        {
            SE_UNKNOWN_OBJECT_TYPE = 0,
            SE_FILE_OBJECT,
            SE_SERVICE,
            SE_PRINTER,
            SE_REGISTRY_KEY,
            SE_LMSHARE,
            SE_KERNEL_OBJECT,
            SE_WINDOW_OBJECT,
            SE_DS_OBJECT,
            SE_DS_OBJECT_ALL,
            SE_PROVIDER_DEFINED_OBJECT,
            SE_WMIGUID_OBJECT,
            SE_REGISTRY_WOW64_32KEY
        }

        [Flags]
        public enum SECURITY_INFORMATION : uint
        {
            OWNER_SECURITY_INFORMATION = 0x00000001,
            GROUP_SECURITY_INFORMATION = 0x00000002,
            DACL_SECURITY_INFORMATION = 0x00000004,
            SACL_SECURITY_INFORMATION = 0x00000008,
            UNPROTECTED_SACL_SECURITY_INFORMATION = 0x10000000,
            UNPROTECTED_DACL_SECURITY_INFORMATION = 0x20000000,
            PROTECTED_SACL_SECURITY_INFORMATION = 0x40000000,
            PROTECTED_DACL_SECURITY_INFORMATION = 0x80000000
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern uint GetSecurityInfo(IntPtr handle, SE_OBJECT_TYPE ObjectType, SECURITY_INFORMATION SecurityInfo, out IntPtr pSidOwner, out IntPtr pSidGroup, out IntPtr pDacl, out IntPtr pSacl, out IntPtr pSecurityDescriptor);
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

        public static void GetSecurityInfo(IntPtr handle, Win32.SE_OBJECT_TYPE ObjectType, Win32.SECURITY_INFORMATION SecurityInfo, out IntPtr pSidOwner, out IntPtr pSidGroup, out IntPtr pDacl, out IntPtr pSacl, out IntPtr pSecurityDescriptor)
        {
            uint getSecurityInfoReturn = Win32.GetSecurityInfo(
                handle,
                ObjectType,
                SecurityInfo,
                out pSidOwner,
                out pSidGroup,
                out pDacl,
                out pSacl,
                out pSecurityDescriptor);

            if (ResultWin32.ERROR_SUCCESS != getSecurityInfoReturn)
            {
                throw new System.ComponentModel.Win32Exception(ResultWin32.GetErrorName(Convert.ToInt32(getSecurityInfoReturn)));
            }
        }
    }
}
