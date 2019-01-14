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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// A control which shows a bunch of items which can be expanded and collapsed. All expanded items shares the available space left.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:NavigationBar AllowMultiExpandings="True">
    /// 
    ///     <WPFToolkit:NavigationBarItem IsExpanded="True" Header="First">
    ///         <ListBox />
    ///     </WPFToolkit:NavigationBarItem>
    /// 
    ///     <WPFToolkit:NavigationBarItem Header="Second">
    ///         <ListBox />
    ///     </WPFToolkit:NavigationBarItem>
    /// 
    ///     <WPFToolkit:NavigationBarItem Header="Third">
    ///         <ListBox />
    ///     </WPFToolkit:NavigationBarItem>
    /// 
    /// </WPFToolkit:NavigationBar>
    /// ]]>
    /// </code>
    /// </example>
    public class NavigationBar : ItemsControl
    {
        static NavigationBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBar), new FrameworkPropertyMetadata(typeof(NavigationBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.NavigationBar" /> class.
        /// </summary>
        public NavigationBar()
        {
            AddHandler(NavigationBarItem.ExpandedEvent, (RoutedEventHandler)OnExpanded);

            Loaded += (sender, args) => OnExpandedItemIndexesChanged();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.NavigationBar.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.NavigationBarItem" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NavigationBarItem;
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.NavigationBar" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NavigationBarItem();
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            if (ExpansionMode == ExpansionMode.Multiple)
                return;

            var currentItem = e.OriginalSource as NavigationBarItem;
            if (currentItem == null)
                return;

            foreach (var item in Items)
            {
                var itemContainer = ItemContainerGenerator.ContainerFromItem(item) as NavigationBarItem;
                if (itemContainer == null ||
                    Equals(itemContainer, currentItem))
                    continue;

                if (itemContainer.IsExpanded)
                    itemContainer.IsExpanded = false;
            }
        }

        /// <summary>
        /// Gets or sets the content string format.
        /// </summary>
        [DefaultValue(null)]
        public string ContentStringFormat
        {
            get { return (string)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.ContentStringFormat" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty =
            DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(NavigationBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content template
        /// </summary>
        [DefaultValue(null)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.ContentTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(NavigationBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content template selector.
        /// </summary>
        [DefaultValue(null)]
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.ContentTemplateSelector" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value which indicates how items can be expanded.
        /// </summary>
        [DefaultValue(ExpansionMode.Multiple)]
        public ExpansionMode ExpansionMode
        {
            get { return (ExpansionMode)GetValue(ExpansionModeProperty); }
            set { SetValue(ExpansionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.ExpansionMode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpansionModeProperty =
            DependencyProperty.Register("ExpansionMode", typeof(ExpansionMode), typeof(NavigationBar), new PropertyMetadata(ExpansionMode.Multiple));

        /// <summary>
        /// Gets or sets a comma separated value of item indexes which are expanded on default.
        /// </summary>
        public string ExpandedItemIndexes
        {
            get { return (string)GetValue(ExpandedItemIndexesProperty); }
            set { SetValue(ExpandedItemIndexesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.ExpandedItemIndexes" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedItemIndexesProperty =
            DependencyProperty.Register("ExpandedItemIndexes", typeof(string), typeof(NavigationBar), new PropertyMetadata(OnExpandedItemIndexesChanged));

        /// <summary>
        /// Gets or sets a value which indicates the orientation of the NavigationBar.
        /// </summary>
        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBar.Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(NavigationBar), new PropertyMetadata(Orientation.Vertical));

        private static void OnExpandedItemIndexesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBar)d;
            control.OnExpandedItemIndexesChanged();
        }

        private void OnExpandedItemIndexesChanged()
        {
            var indexes = ExtraxtIndexes();
            for (var currentIndex = 0; currentIndex < Items.Count; ++currentIndex)
            {
                var itemContainer = ItemContainerGenerator.ContainerFromItem(Items[currentIndex]) as NavigationBarItem;
                if (itemContainer == null)
                    continue;

                if (indexes.Contains(currentIndex))
                {
                    if (!itemContainer.IsExpanded)
                        itemContainer.IsExpanded = true;
                }
                else
                {
                    if (itemContainer.IsExpanded)
                        itemContainer.IsExpanded = false;
                }
            }
        }

        private List<int> ExtraxtIndexes()
        {
            var indexes = new List<int>();

            if (string.IsNullOrWhiteSpace(ExpandedItemIndexes))
                return indexes;

            var itemIndexStrings = ExpandedItemIndexes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var itemIndexString in itemIndexStrings)
            {
                var itemIndex = 0;
                if (int.TryParse(itemIndexString, out itemIndex))
                    indexes.Add(itemIndex);

                if (ExpansionMode == ExpansionMode.Single)
                    return indexes;
            }
            return indexes;
        }
    }
}
