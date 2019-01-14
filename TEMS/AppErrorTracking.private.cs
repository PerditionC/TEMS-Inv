// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory
{
    /// <summary>
    /// private constants used by connecting to error tracking service
    /// </summary>
    public partial class AppErrorTracking
    {
        #pragma warning disable 0414 // ignore warning about unused fields value set
        // Obtain from Sentry by navigating to [Project Name] -> Project Settings -> Client Keys (DSN).
        private static readonly string publicKey = "12f1f2e9f52347cb8c0139125d64d553";
        private static readonly string secretKey = "";  // deprecated & no longer needed
        private static readonly string projectId = "278259";
    }
}