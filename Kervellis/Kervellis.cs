using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace Kervellis
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }


    public partial class Service1 : ServiceBase
    {
        string kervellis_installation_folder;
        string dllName;
        string dllFullPath;
        Thread work;

        public Service1()
        {
            InitializeComponent();
            kervellis_installation_folder = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;
            dllName = Process.GetCurrentProcess().ProcessName + ".dll";
            
            dllFullPath = Path.Combine(kervellis_installation_folder, dllName);
        }

        public void DoWork()
        {
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
            work = new Thread(DoWork);
            work.Start();
        }

        protected override void OnStop()
        {
            work.Abort();
        }
    }
}