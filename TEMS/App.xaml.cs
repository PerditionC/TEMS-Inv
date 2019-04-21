// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

using SharpRaven.Data;
using LibZ.Bootstrap;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;

namespace TEMS_Inventory
{
    /// <summary>
    /// helper static class to ensure ability to resolve DLLs in
    /// our containers happen as early as practical without tricks
    /// </summary>
    static class DllResolver
    {
        // Warning, do not create any static scoped variables and delay
        // initialization of instance variables in App class until we 
        // initialize our DLL container support (libz)
        static DllResolver()
        {
            try
            {
                LibZResolver.RegisterFileContainer("TEMS.libz", optional: false);
                /*
                LibZResolver.RegisterFileContainer("Logging.libz", optional: false);
                LibZResolver.RegisterFileContainer("UI.libz", optional: false);
                LibZResolver.RegisterFileContainer("DB.libz", optional: false);
                LibZResolver.RegisterFileContainer("Async.libz", optional: false);
                */
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine("Incomplete installation, missing required files!  Program Aborting!");
                System.Console.Error.WriteLine(e);
                MessageBox.Show("Incomplete installation, missing required files!  Program Aborting!\n\n" + e.ToString(),
                    caption: "Installation corrupt - Aborting!", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                System.Environment.Exit(-1);
            }
        }

        public static bool isLoaded() { return true; }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private static bool DLLsRegistered = DllResolver.isLoaded(); // force static helper class constructor to run
        public App() : base() { }

        // application wide logging and error tracking support, primarily unhandled exceptions/critical startup errors
        public static AppErrorTracking errorTracking = null;

        // application wide data repository and data access
        private static DataRepository dataRepository = null;
        // application wide user manager
        private static IUserManager userManager = null;

        #region OnStartup

        /// <summary>
        /// during development throw exceptions if binding errors occur
        /// </summary>
        private void EnableWpfBindingErrors()
        {
#if DEBUG
            //WpfBindingErrors.BindingExceptionThrower.Attach();
#endif
        }

        /// <summary>
        /// Verifies if DB file exists, if so returns path provided
        /// otherwise asks user for where DB file is and returns either
        /// what user provided or default path provided if user canceled.
        /// </summary>
        /// <param name="defaultDatabasePath">Full path where we expect our SQLite database to be at.</param>
        /// <returns>string with path of SQLite database file to attempt to use</returns>
        private string SelectDbFile(string defaultDatabasePath)
        {
            if (!System.IO.File.Exists(defaultDatabasePath))
            {
                var dlg = new OpenFileDialog
                {
                    Title = "Database not found - Please specify the SQLite DB file to use:",
                    InitialDirectory = defaultDatabasePath,
                    DefaultExt = ".db",
                    Filter = "Database (.db)|*.db",
                    CheckFileExists = true,
                    ShowReadOnly = false,  // must be able to edit DB
                    DereferenceLinks = true,
                    Multiselect = false
                };
                if (dlg.ShowDialog() == true)
                    return dlg.FileName;
            }

            return defaultDatabasePath;
        }

        /// <summary>
        /// open (and initialize) our DB interface
        /// </summary>
        private void IntializeDatabaseRepository()
        {
            try
            {
                // attempt to use stored DBPath, use default otherwise, save updated path, then open DB
                var dbPath = SelectDbFile(GetAppSetting("DatabasePath", @"C:\DB\TEMS_Inv.db"));
                SetAppSetting("DatabasePath", dbPath);
                errorTracking?.logger?.Info("Loading SQLite database: '{0}'", dbPath);
                dataRepository = new DataRepository(dbPath);
                userManager = new UserManager(dataRepository);

                //Resources["DataModel"] = dataRepository.db;
                preloadCache();
            }
            catch (Exception e)
            {
                var msg = $"Critical error initializing database!\n  { e.Message}\n  Program ABORTING!";
                errorTracking?.logger?.Fatal(e, msg);
                errorTracking?.ravenClient?.Capture(new SentryEvent(new AggregateException(msg, e)));
                MessageBox.Show(msg, "Database Error:", MessageBoxButton.OK);
                Application.Current.Shutdown(1);
            }
        }


        /// <summary>
        /// cached data is loaded on demand, but may take a while to load on initial reference
        /// so we force loading early while waiting for user to type in user name and password so they don't notice
        /// </summary>
        private void preloadCache()
        {
            var cache = DataRepository.GetDataRepository.ReferenceData;

            Task t = new Task(() =>
            {
                foreach (var item in ReferenceDataCache.ReferenceDataTypes)
                {
                    var dummy = cache[item.TypeName];
                }
            });
            t.ConfigureAwait(false);
            t.Start();
        }



        /// <summary>
        /// Do any application wide initialization
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            // setup handler for any unexpected exceptions
            // unhandled exceptions on the UI thread
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            // any other unhandled exception
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomainUnhandledException);
#endif

            // let frame do anything it needs to do
            base.OnStartup(e);

            // Initialize embedded assembly loading and complete startup initialization.
            // Make deploying and sharing easier by supporting the embedding of most 
            // resources/assemblies instead of having many small assemblies without 
            // requiring building as 1 giant assembly.
            LibZResolver.Startup(() =>
            {
                // enable Sentry error reporting only when enabled in config file
                errorTracking = new AppErrorTracking("TRUE".Equals(GetAppSetting("ReportErrors", "True"), StringComparison.InvariantCultureIgnoreCase));

                EnableWpfBindingErrors();

                IntializeDatabaseRepository();
            });
        }

        #endregion // OnStartup

        #region OnExit
        /// <summary>
        /// ensure db and related are cleaned up
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            // automatically log user out
            UserManager.GetUserManager?.LogoutUser("Program exit.");

            base.OnExit(e);

            errorTracking?.logger?.Info("Application Exit.");
            NLog.LogManager.Shutdown();
        }
        #endregion // OnExit

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    dataRepository?.Dispose();
                    dataRepository = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.

                // flag that disposition has been completed, any additional calls ignored
                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AppDataRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // Implement the disposable pattern, i.e. allow both manual and automatic [redundant] calls to Dispose()
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Support

        #region Exception handling

        static private readonly string ErrorMessage = "An application error occurred.  It is recommended to restart the application as soon as possible." +
                "If the error occurs again then please report the issue along with steps to reproduce.\n\n" +
                "Error:{0}\n\n";

        /// <summary>
        /// Handles any unhandled exceptions on main UI thread.
        /// Logs and displays unhandled exception to user.
        /// Prompts user if they wish to attempt to continue processing.  Warning: this may not be possible!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">the unhandled Exception information</param>
        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            if (args != null)
            {
                Exception e = args.Exception;
                string errorMessage = string.Format(ErrorMessage, e?.Message ?? "Unhandled null Exception thrown!");
                App.errorTracking?.logger?.Error(e, "AppDispatcher: " + errorMessage); // + e?.StackTrace.ToString());                
                App.errorTracking?.ravenClient?.Capture(new SentryEvent(new AggregateException("AppDispatcher: " + errorMessage, e)));


                errorMessage += "Do you wish to try and continue running the application?\n" +
                                "If you choose Yes the application will try to continue, otherwise the application will close.\n\n" +
                                "WARNING: Any changes not saved may be lost!";

                // mark exception as handled so no further exception processing occcurs, may allow user
                // to continue running application, otherwise we explicitly quit the application to avoid further issues
                args.Handled = true;
                if (MessageBox.Show(
                        errorMessage,
                        "AppDispatcher: Internal Application Error",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error,
                        MessageBoxResult.Yes
                       ) != MessageBoxResult.Yes)
                {
                    Application.Current?.Shutdown(1);
                }

            }
        }

        /// <summary>
        /// Handles any unhandled exceptions other than on main UI thread, currently logs for further investigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args != null)
            {
                Exception e = args.ExceptionObject as Exception;
                string errorMessage = string.Format(ErrorMessage, e?.Message ?? "Unhandled null Exception thrown!");
                App.errorTracking?.logger?.Error(e, "AppDomain: " + errorMessage /* + e?.StackTrace.ToString() */ + $"\nApp is terminating:{args.IsTerminating}\n");
                App.errorTracking?.ravenClient?.Capture(new SentryEvent(new AggregateException("AppDomain: " + errorMessage + $"\nApp is terminating:{args.IsTerminating}\n", e)));

                // mark exception as handled so no further exception processing occcurs, may allow user
                // to continue running application, otherwise we explicitly quit the application to avoid further issues
                MessageBox.Show(
                        errorMessage,
                        "AppDomain: Internal Application Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.None);

            }
        }

        #endregion // Exception handling

        #region Application Settings

        static string GetAppSetting(string key, string defaultValue)
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch (ConfigurationErrorsException e)
            {
                App.errorTracking?.logger.Info(e, "Using default AppSetting[{0}]={1}", key, defaultValue);
                return defaultValue;
            }
        }

        static void SetAppSetting(string key, string newValue)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, newValue);
                }
                else
                {
                    settings[key].Value = newValue;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                App.errorTracking?.logger.Info(e, $"Failed to SetAppSetting with key={key.ToString()}.");
            }
        }

        #endregion // Application Settings
    }
}
