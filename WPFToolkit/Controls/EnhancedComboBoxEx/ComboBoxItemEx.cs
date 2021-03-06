﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Extends the <see cref="ComboBoxItem"/> class.
    /// </summary>
    [Browsable(false)]
    public class ComboBoxItemEx : ComboBoxItem
    {
        /// <summary>
        /// Gets or sets a <see cref="bool"/> value that indicates if this item is highlighted.
        /// </summary>
        public new bool IsHighlighted
        {
            get { return base.IsHighlighted; }
            set { base.IsHighlighted = value; }
        }
    }
}
