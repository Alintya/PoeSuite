using PoeSuite.Imports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Utility
{
    public static class PrivilegeHelper
    {
        public static bool EnableDebugPrivileges(IntPtr procHandle)
        {
            IntPtr tokenHandle = default;

            try
            {
                if (!Advapi32.OpenProcessToken(procHandle, (uint)TokenAccessLevels.AdjustPrivileges | (uint)TokenAccessLevels.Query, out tokenHandle))
                    return false;

                return SetPrivilege(tokenHandle, "SeDebugPrivilege", true);
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                    Kernel32.CloseHandle(tokenHandle);
            }
        }

        internal static bool EnableDebugPrivileges(Process proc)
        {
            if (proc?.HasExited != false)
                return false;

            return EnableDebugPrivileges(proc.Handle);
        }

        public static bool SetPrivilege(IntPtr tokenHandle, string privilegeName, bool enable)
        {
            var privLuid = new Advapi32.LUID();

            if (!Advapi32.LookupPrivilegeValue(null, privilegeName, ref privLuid))
                return false;

            var privs = new Advapi32.LUID_AND_ATTRIBUTES
                {
                    Luid = privLuid,
                    Attributes = enable ? 0x00000002u/*SE_PRIVILEGE_ENABLED*/ : 0x00000004u/*SE_PRIVILEGE_REMOVED*/
                };

            var newPrivs = new Advapi32.TOKEN_PRIVILEGES()
            {
                PrivilegeCount = 1,
                Privileges = new Advapi32.LUID_AND_ATTRIBUTES[] { privs }
            };

            return Advapi32.AdjustTokenPrivileges(tokenHandle, false, ref newPrivs, 0, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
