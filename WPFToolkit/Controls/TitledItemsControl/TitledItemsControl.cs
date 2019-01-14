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
    /// Shows items with its title allined to each other. This can be used for every kind of input masks.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:TitledItemsControl>
    ///     <WPFToolkit:TitledItem Title="Name:">
    ///         <TextBox Text="{Binding Name}" />
    ///     </WPFToolkit:TitledItem>
    ///     <WPFToolkit:TitledItem Title="Age:">
    ///         <TextBox Text="{Binding Age}" />
    ///     </WPFToolkit:TitledItem>
    ///     <WPFToolkit:TitledItem Title="Sex:">
    ///         <TextBox Text="{Binding Sex}" />
    ///     </WPFToolkit:TitledItem>
    /// </WPFToolkit:TitledItemsControl>
    /// ]]>
    /// </code>
    /// </example>
    public class TitledItemsControl : ItemsControl
    {
        static TitledItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitledItemsControl), new FrameworkPropertyMetadata(typeof(TitledItemsControl)));
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.DynamicTabControl" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TitledItem();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.TitledItemsControl" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TitledItem;
        }

        /// <summary>
        /// Gets or sets a value that defines the vertical alignment of all titles in the child elements.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalTitleAlignments
        {
            get { return (VerticalAlignment)GetValue(VerticalTitleAlignmentsProperty); }
            set { SetValue(VerticalTitleAlignmentsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.VerticalTitleAlignments" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalTitleAlignmentsProperty =
            DependencyProperty.Register("VerticalTitleAlignments", typeof(VerticalAlignment), typeof(TitledItemsControl), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets a value that defines the horizontal alignment of all titles in the child elements.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment HorizontalTitleAlignments
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTitleAlignmentsProperty); }
            set { SetValue(HorizontalTitleAlignmentsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.HorizontalTitleAlignments" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalTitleAlignmentsProperty =
            DependencyProperty.Register("HorizontalTitleAlignments", typeof(HorizontalAlignment), typeof(TitledItemsControl), new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets a value that defines the margin of all titles in the child elements
        /// </summary>
        public Thickness TitleMargins
        {
            get { return (Thickness)GetValue(TitleMarginsProperty); }
            set { SetValue(TitleMarginsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.TitleMargins" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleMarginsProperty =
            DependencyProperty.Register("TitleMargins", typeof(Thickness), typeof(TitledItemsControl), new UIPropertyMetadata(new Thickness(5, 0, 5, 0)));

        /// <summary>
        /// Gets or sets a value that defines the horizontal alignments of all contens in the child elements.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Stretch)]
        public HorizontalAlignment HorizontalContentAlignments
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentsProperty); }
            set { SetValue(HorizontalContentAlignmentsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.HorizontalContentAlignments" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentsProperty =
            DependencyProperty.Register("HorizontalContentAlignments", typeof(HorizontalAlignment), typeof(TitledItemsControl), new UIPropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Gets or sets a value that defines the vertical alignments of all contents in the child elements.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalContentAlignments
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentsProperty); }
            set { SetValue(VerticalContentAlignmentsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.VerticalContentAlignments" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentsProperty =
            DependencyProperty.Register("VerticalContentAlignments", typeof(VerticalAlignment), typeof(TitledItemsControl), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets a value that defines the margins of all contents in the child elements.
        /// </summary>
        public Thickness ContentMargins
        {
            get { return (Thickness)GetValue(ContentMarginsProperty); }
            set { SetValue(ContentMarginsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TitledItemsControl.ContentMargins" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentMarginsProperty =
            DependencyProperty.Register("ContentMargins", typeof(Thickness), typeof(TitledItemsControl), new UIPropertyMetadata(new Thickness(0, 2, 0, 2)));
    }
}
