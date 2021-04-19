using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands.Extensions
{
    public static class ProcessExtension
    {
        /// <summary>
        /// Returns the account that owns the process
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetProcessOwnerString(this Process p)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + p.Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the account that owns the process
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static WindowsIdentity GetProcessOwner(this Process p)
        {

            string query = "Select * From Win32_Process Where ProcessID = " + p.Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();
            string name = null;
            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    name = argList[1] + "\\" + argList[0];
                }
            }

            var u1 = UserPrincipal.FindByIdentity(UserPrincipal.Current.Context, IdentityType.SamAccountName, name);
            var u2 = UserPrincipal.FindByIdentity(UserPrincipal.Current.Context, IdentityType.UserPrincipalName, name);
            using (var user = u1 ?? u2)
            {
                return user == null
                  ? null
                  : new WindowsIdentity(user.UserPrincipalName);
            }
        }

        /// <summary>
        /// Gets the path of the executable
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetProcessPath(this Process p)
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                string query = "SELECT ExecutablePath, ProcessID FROM Win32_Process";
                ManagementObjectSearcher psearcher = new ManagementObjectSearcher(query);

                foreach (ManagementObject item in psearcher.Get())
                {
                    object id = item["ProcessID"];
                    object path = item["ExecutablePath"];
                    //TODO: check if == works lol
                    if (path != null && id.ToString() == p.Id.ToString())
                    {
                        return path.ToString();
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// Gets all the modules (DLLs) the process is using.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string[] GetAllModules(this Process p)
        {
            try
            {
                List<string> s = new List<string>();
                foreach (ProcessModule m in p.Modules)
                {
                    s.Add(m.ModuleName);
                }
                return s.ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the amount of memory a process is using
        /// </summary>
        /// <returns></returns>
        public static ulong GetPhysicalMemory()
        {
            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.length = Marshal.SizeOf(status);
            if (!GlobalMemoryStatusEx(ref status))
            {
                int err = Marshal.GetLastWin32Error();
                throw new Win32Exception(err);
            }
            return status.totalPhys;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public int length;
            public int memoryLoad;
            public ulong totalPhys;
            public ulong availPhys;
            public ulong totalPageFile;
            public ulong availPageFile;
            public ulong totalVirtual;
            public ulong availVirtual;
            public ulong availExtendedVirtual;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX buffer);

        /// <summary>
        /// Kills a process and its children processes.
        /// </summary>
        /// <param name="continueOnError">If true, the method will try to kill all the subprocesses before throwing the excptions</param>
        public static void KillProcessAndChildren(this Process p, bool continueOnError)
        {
            int pid = p.Id;
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])), continueOnError);
            }
            List<Exception> exceptions = new List<Exception>();
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (Exception ex)
            {
                if (continueOnError)
                {
                    exceptions.Add(ex);
                }
                else
                {
                    throw ex;
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions.ToArray());
            }
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Suspends a process
        /// </summary>
        /// <param name="p"></param>
        public static void Suspend(this Process p)
        {
            try
            {
                int pid = p.Id;

                foreach (ProcessThread pT in p.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    SuspendThread(pOpenThread);

                    CloseHandle(pOpenThread);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Resumes a process
        /// </summary>
        /// <param name="p"></param>
        public static void Resume(this Process p)
        {
            try
            {

                if (p.ProcessName == string.Empty)
                    return;

                foreach (ProcessThread pT in p.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    var suspendCount = 0;
                    do
                    {
                        suspendCount = ResumeThread(pOpenThread);
                    } while (suspendCount > 0);

                    CloseHandle(pOpenThread);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
