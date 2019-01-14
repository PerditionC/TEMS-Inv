// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/**
 * WPF Binding Error Testing
 * Copyright 2013 Benoit Blanchon
 * 
 * This has been inpired by  
 * http://tech.pro/tutorial/940/wpf-snippet-detecting-binding-errors
 */

using System;
using System.Diagnostics;
using System.Text;

namespace WpfBindingErrors
{
    /// <summary>
    /// A TraceListener that raise an event each time a trace is written
    /// </summary>
    sealed class ObservableTraceListener : TraceListener
    {
        StringBuilder buffer = new StringBuilder();

        public override void Write(string message)
        {
            buffer.Append(message);
        }

        [DebuggerStepThrough]
        public override void WriteLine(string message)
        {
            buffer.Append(message);

            TraceCatched?.Invoke(buffer.ToString());

            buffer.Clear();
        }

        public event Action<string> TraceCatched;
    }
}
