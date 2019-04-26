// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#region License
/*
The MIT License (MIT)

Copyright (c) 2009-2016 David Wendland

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
*/
#endregion License

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a pane where items can be collapsed into and expanded back. A single item is visible only at one time.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <DockPanel>
    /// 
    ///     <WPFToolkit:DockingPane DockPanel.Dock="Right" ExpandDirection="RightToLeft" AreaWidth="250" AreaMinWidth="150">
    ///         <WPFToolkit:DockingPaneItem Header="Title 1">
    ///             <ListBox />
    ///         </WPFToolkit:DockingPaneItem>
    ///         <WPFToolkit:DockingPaneItem Header="Title 2">
    ///             <ListBox />
    ///         </WPFToolkit:DockingPaneItem>
    ///     </WPFToolkit:DockingPane>
    /// 
    ///     <Grid />
    /// </DockPanel>
    /// ]]>
    /// </code>
    /// </example>
    public class DockingPane : TabControl
    {
        static DockingPane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingPane), new FrameworkPropertyMetadata(typeof(DockingPane)));
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.DockingPane.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.DockingPane" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DockingPaneItem;
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.DockingPane" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DockingPaneItem();
        }

        /// <summary>
        /// Gets or sets the button pane position in the control.
        /// </summary>
        [DefaultValue(ButtonPanePosition.Outher)]
        public ButtonPanePosition ButtonsPosition
        {
            get { return (ButtonPanePosition)GetValue(ButtonsPositionProperty); }
            set { SetValue(ButtonsPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.ExpandDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsPositionProperty =
            DependencyProperty.Register("ButtonsPosition", typeof(ButtonPanePosition), typeof(DockingPane), new PropertyMetadata(ButtonPanePosition.Outher));

        /// <summary>
        /// Gets or sets in which direction the selected content should be expanded to.
        /// </summary>
        [DefaultValue(ExpandDirection.LeftToRight)]
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.ExpandDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register("ExpandDirection", typeof(ExpandDirection), typeof(DockingPane), new PropertyMetadata(ExpandDirection.LeftToRight));

        /// <summary>
        /// Gets or sets the width of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaWidth
        {
            get { return (double)GetValue(AreaWidthProperty); }
            set { SetValue(AreaWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaWidthProperty =
            DependencyProperty.Register("AreaWidth", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the minimum width of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaMinWidth
        {
            get { return (double)GetValue(AreaMinWidthProperty); }
            set { SetValue(AreaMinWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaMinWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaMinWidthProperty =
            DependencyProperty.Register("AreaMinWidth", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the maximum width of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaMaxWidth
        {
            get { return (double)GetValue(AreaMaxWidthProperty); }
            set { SetValue(AreaMaxWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaMaxWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaMaxWidthProperty =
            DependencyProperty.Register("AreaMaxWidth", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the height of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaHeight
        {
            get { return (double)GetValue(AreaHeightProperty); }
            set { SetValue(AreaHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaHeightProperty =
            DependencyProperty.Register("AreaHeight", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the minimum height of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaMinHeight
        {
            get { return (double)GetValue(AreaMinHeightProperty); }
            set { SetValue(AreaMinHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaMinHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaMinHeightProperty =
            DependencyProperty.Register("AreaMinHeight", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the maximum height of the area visible if an item gets expanded.
        /// </summary>
        [DefaultValue(double.NaN)]
        public double AreaMaxHeight
        {
            get { return (double)GetValue(AreaMaxHeightProperty); }
            set { SetValue(AreaMaxHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DockingPane.AreaMaxHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaMaxHeightProperty =
            DependencyProperty.Register("AreaMaxHeight", typeof(double), typeof(DockingPane), new PropertyMetadata(double.NaN));
    }
}
