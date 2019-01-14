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

namespace WpfBindingErrors
{
    /// <summary>
    /// Exception thrown by the BindingExceptionThrower each time a WPF binding error occurs
    /// </summary>
    [Serializable]
    public class BindingException : Exception
    {
        public BindingException(string message) 
            : base(message)
        {

        }
    }
}
