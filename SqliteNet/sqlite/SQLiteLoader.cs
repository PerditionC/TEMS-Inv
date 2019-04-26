// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NLog;

namespace SQLite
{
    public class SQLiteLoader
    {
        public static Logger logger = LogManager.GetLogger("TEMS_Inventory.SQLiteLoader");

        #region Native Dll support

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectoryW(string path);

        /// <summary>
        /// Updates where Windows attempts to load DLLs from to include subdirectory 
        /// named based on currently executing architecture; e.g. x86 or x64
        /// </summary>
        public static void SetNativeDllDirectory()
        {
            // get location of .exe, append x86 or x64 depending on if 64bit process or not
            // Note if GetEntryAssembly() returns null then use GetExecutingAssembly() [which
            // may be a DLL] for cases where called from unit tests or other cases where current 
            // AppDomains did not set  AppDomainManager.EntryAssembly property 
            // then update search path for DLLs to this subdirectory
            // See for alternate solution: https://stackoverflow.com/a/21888521
            // Note: we assume running on Intel compatible x86, TODO for other architectures, e.g. ARM
            string basePath = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())?.Location);
            if (basePath != null)
            {
                string dllPath = Path.Combine(basePath, Environment.Is64BitProcess ? "x64" : "x86");
                logger.Info("Native DLL path set to '{0}'", dllPath);
                if (!SetDllDirectoryW(dllPath)) throw new System.ComponentModel.Win32Exception();
            }
        }

        #endregion // Native Dll support

    }
}
