// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Reflection;
using NLog;
using SharpRaven;

namespace TEMS_Inventory
{
    public partial class AppErrorTracking
    {
        public AppErrorTracking() : this(false) { }
        public AppErrorTracking(bool enableErrorTracking)
        {
            InitializeLogging(enableErrorTracking);
        }

        public /* static */ Logger logger = null;

        // Obtain from Sentry by navigating to [Project Name] -> Project Settings -> Client Keys (DSN).
        // see AppErrorTracking.private.cs for implementation of private string publicKey, secretKey, projectId;
        // basic usage: ravenClient.Capture(new SentryEvent(exception));
        public /* static */ RavenClient ravenClient = null;  // see below, only activated if configured to enable


        /// <summary>
        /// initialize and enable error reporting
        /// currently done via Sentry
        /// </summary>
        private void InitializeErrorTracking()
        {
#if !DEBUG      // only create for release builds to avoid polluting logs with errors during development
            logger?.Info($"Sentry url:https://{publicKey}@sentry.io/{projectId}");  // $"Sentry url:https://{publicKey}:{secretKey}@sentry.io/{projectId}"
            ravenClient = new RavenClient($"https://{publicKey}@sentry.io/{projectId}");
#else
            logger?.Info("Sentry error tracking is not enabled.");
#endif
            if (ravenClient != null)
            {
                ravenClient.Release = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ravenClient.Logger = "Raven-csharp";
#if DEBUG
                ravenClient.Environment = "Debug";
#else
                    ravenClient.Environment = "Release";
#endif
                ravenClient.Tags.Add("OS", Environment.OSVersion.VersionString);
                ravenClient.Tags.Add("Device", "WPF");
                ravenClient.Tags.Add("User", "None");
            }
        }

        /// <summary>
        /// initialize our logging support
        /// Currently implemented via NLog, optionally enabling Sentry error logging
        /// </summary>
        private void InitializeLogging(bool enableErrorTracking)
        {
            logger = LogManager.GetLogger("TEMS_Inventory");
            // Log.Init, note NLog initializes from configuration file
            logger?.Info("Application Startup.");

            if (enableErrorTracking) InitializeErrorTracking();
        }

    }
}
