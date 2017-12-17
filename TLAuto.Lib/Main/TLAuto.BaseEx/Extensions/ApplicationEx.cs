// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

using GalaSoft.MvvmLight.Threading;

using TLAuto.Log;
#endregion

namespace TLAuto.BaseEx.Extensions
{
    public abstract class ApplicationEx : Application
    {
        private static Mutex _mutex;
        private static string _projectName;
        private static readonly string _logSystemPath = @"Logs\";

        protected ApplicationEx()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            _projectName = Assembly.GetCallingAssembly().GetName().Name;
            InitLogServer();

            DispatcherHelper.Initialize();
        }

        protected virtual bool IsCheckTwice { get; } = true;

        private static void InitLogServer()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logSystemPath);
            Logger.Init(logPath);
            LoggerManager.DeleteLogFileForNumberWhile(40, 24);
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                var orginalEx = GetOraginalException(ex);
                Logger.Critical("Critical", GetMemInfo(), ex);
                MessageBox.Show(orginalEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Logger.Critical("Critical", GetMemInfo() + e.ExceptionObject);
                MessageBox.Show("Error:" + e.ExceptionObject, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsCheckTwice)
            {
                CheckTwice();
            }
            base.OnStartup(e);
        }

        private void Close()
        {
            var processList = Process.GetProcessesByName(_projectName);
            foreach (var process in processList)
            {
                process.Kill();
            }
        }

        private static string GetMemInfo()
        {
            var p = Process.GetCurrentProcess();
            var s = $"Mem: {p.PrivateMemorySize64 / 1024} KB, TC: {p.Threads.Count} ";
            return s;
        }

        private static Exception GetOraginalException(Exception ex)
        {
            var ret = ex;
            if (ex.InnerException != null)
            {
                ret = GetOraginalException(ex.InnerException);
            }
            return ret;
        }

        private static void CheckTwice()
        {
            bool createdNew;
            _mutex = new Mutex(true, _projectName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("只能运行一个程序。");
                Environment.Exit(-1);
            }
        }
    }
}