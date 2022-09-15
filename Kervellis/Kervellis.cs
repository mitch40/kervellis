using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kervellis
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
    }


    public partial class Service1 : ServiceBase
    {
        ServiceStatus serviceStatus;
        string kervellis_installation_folder;
        string dllName;
        string dllFullPath;
        Thread work;

        public Service1()
        {
            InitializeComponent();
            serviceStatus = new ServiceStatus();
            kervellis_installation_folder = @"C:\kervellis\kervellis-1.0";
            dllName = "kervellis.dll";
            dllFullPath = Path.Combine(kervellis_installation_folder, dllName);
        }

        public void DoWork()
        {
            if (!Directory.Exists(kervellis_installation_folder))
            {
                Directory.CreateDirectory(kervellis_installation_folder);
            }

            IntPtr pDll;
            do
            {

                pDll = NativeMethods.LoadLibrary(dllFullPath);
                Thread.Sleep(5000);


            } while (pDll == IntPtr.Zero);

            NativeMethods.FreeLibrary(pDll);
        }

        protected override void OnStart(string[] args)
        {
            /*
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            NativeMethods.SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            */

            work = new Thread(DoWork);
            work.Start();

            /*
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            NativeMethods.SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            */
        }

        protected override void OnStop()
        {
            work.Abort();
        }
    }
}
