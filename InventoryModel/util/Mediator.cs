// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NLog;

namespace TEMS.InventoryModel.util
{
    /// <summary>
    /// singleton instance in our project of Mediator
    /// actions are registered (and unregistered) and then
    /// notification can then be done without a direct reference
    /// to the handler
    /// </summary>
    public static class Mediator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// structure to keep track of notification events and who to be notified when raised
        /// </summary>
        private static readonly IDictionary<object, List<Action<object>>> callbacks = new Dictionary<object, List<Action<object>>>();

        /// <summary>
        /// associates callback with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void Register(object key, Action<object> callback)
        {
            logger.Log(LogLevel.Debug, "Register callback for " + key.ToString());

            // create a new list if doesn't already exist
            if (!callbacks.ContainsKey(key))
            {
                callbacks.Add(key, new List<Action<object>>());
            }

            // add the callback to the list, but ignore duplicate
            var callbackList = callbacks[key];
            bool alreadyRegistered = false;
            foreach (var x in callbackList)
            {
                if (x == callback)
                {
                    alreadyRegistered = true;
                    break;
                }
            }
            if (!alreadyRegistered)
            {
                callbackList.Add(callback);
            }
        }

        /// <summary>
        /// removes associated callback from key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void Unregister(object key, Action<object> callback)
        {
            logger.Log(LogLevel.Debug, "Unregister callback for " + key.ToString());

            // if list exists, then attempt to remove callback
            if (callbacks.ContainsKey(key))
            {
                callbacks[key].Remove(callback);
            }
        }

        /// <summary>
        /// invokes (synchronously) registered callbacks for given key with provided args
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        public static void InvokeCallback(object key, object args)
        {
            logger.Log(LogLevel.Debug, "Invoking callbacks for " + key.ToString() + ((args == null) ? " with null arguments" : string.Empty));

            try
            {

                // if key is valid, then for each registered callback invoke it
                if (callbacks.ContainsKey(key))
                {
                    var callbackList = callbacks[key];
                    foreach (var callback in callbackList)
                    {
                        callback(args);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to invoke callback {key.ToString()}");
                throw;
            }
        }
    }
}