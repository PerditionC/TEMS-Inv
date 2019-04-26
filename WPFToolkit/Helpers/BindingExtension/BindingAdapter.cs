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

using System.Windows;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings the possibility to modify existing bindings wo be able to bind the parameters in the Binding like Converter, ConverterParameter and so on.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="{Binding Demo}" ToolTip="{Binding AnyTag}">
    ///     <Helpers:BindingAdapter.BindingExtensions>
    ///         <Helpers:BindingExtensionCollection>
    ///             <Helpers:BindingExtension Property="TextBlock.Text"
    ///                                       Converter="{Binding DemoConverter}"
    ///                                       ConverterParameter="{Binding DemoConverterParameter}" />
    ///             <Helpers:BindingExtension Property="TextBlock.ToolTip"
    ///                                       FallbackValue="{Binding BindingFallbackValue}" />
    ///         </Helpers:BindingExtensionCollection>
    ///     </Helpers:BindingAdapter.BindingExtensions>
    /// </TextBlock>
    /// ]]>
    /// </code>
    /// </example>
    public class BindingAdapter : FrameworkElement
    {
        private readonly FrameworkElement _owner;

        private BindingAdapter(FrameworkElement owner)
        {
            _owner = owner;

            _owner.DataContextChanged += HandleDataContextChanged;
            _owner.Loaded += HandleLoaded;
        }

        #region BindingAdapter

        private static BindingAdapter GetBindingAdapter(DependencyObject obj)
        {
            return (BindingAdapter)obj.GetValue(BindingAdapterProperty);
        }

        private static void SetBindingAdapter(DependencyObject obj, BindingAdapter value)
        {
            obj.SetValue(BindingAdapterProperty, value);
        }

        private static readonly DependencyProperty BindingAdapterProperty =
            DependencyProperty.RegisterAttached("BindingAdapter", typeof(BindingAdapter), typeof(BindingAdapter), new PropertyMetadata(null));

        #endregion BindingAdapter

        #region BindingExtensions

        /// <summary>
        /// Gets the extension collection which modifies a binding.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Helpers.BindingAdapter.BindingExtensions property value for the element.</returns>
        public static BindingExtensionCollection GetBindingExtensions(DependencyObject obj)
        {
            var extension = (BindingExtensionCollection)obj.GetValue(BindingExtensionsProperty);
            if (extension == null)
            {
                var extensionsCollection = new BindingExtensionCollection();
                obj.SetValue(BindingExtensionsProperty, extensionsCollection);
            }
            return extension;
        }

        /// <summary>
        /// Sets the extensions collection which modifies a binding.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Helpers.BindingAdapter.BindingExtensions value.</param>
        public static void SetBindingExtensions(DependencyObject obj, BindingExtensionCollection value)
        {
            obj.SetValue(BindingExtensionsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingAdapter.GetBindingExtensions(DependencyObject)" /> <see cref="DW.WPFToolkit.Helpers.BindingAdapter.SetBindingExtensions(DependencyObject, BindingExtensionCollection)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty BindingExtensionsProperty =
            DependencyProperty.RegisterAttached("BindingExtensions", typeof(BindingExtensionCollection), typeof(BindingAdapter), new PropertyMetadata(OnBindingExtensionsChanged));

        private static void OnBindingExtensionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = GetBindingAdapter(d);
            if (container != null)
                return;

            container = new BindingAdapter((FrameworkElement)d);
            SetBindingAdapter(d, container);
        }

        private void UpdateExtensions()
        {
            var extensions = GetBindingExtensions(_owner);
            if (extensions == null)
                return;

            foreach (var extension in extensions)
            {
                extension.DataContext = _owner.DataContext;
                extension.Owner = _owner;
            }
        }

        #endregion BindingExtensions

        #region BindingExtension

        /// <summary>
        /// Gets the binding extension which modifies a binding.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Helpers.BindingAdapter.BindingExtension property value for the element.</returns>
        public static BindingExtension GetBindingExtension(DependencyObject obj)
        {
            return (BindingExtension)obj.GetValue(BindingExtensionProperty);
        }

        /// <summary>
        /// Sets the binding extension which modifies a binding.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Helpers.BindingAdapter.BindingExtension value.</param>
        public static void SetBindingExtension(DependencyObject obj, BindingExtension value)
        {
            obj.SetValue(BindingExtensionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingAdapter.GetBindingExtension(DependencyObject)" /> <see cref="DW.WPFToolkit.Helpers.BindingAdapter.SetBindingExtension(DependencyObject, BindingExtension)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty BindingExtensionProperty =
            DependencyProperty.RegisterAttached("BindingExtension", typeof(BindingExtension), typeof(BindingAdapter), new PropertyMetadata(OnBindingExtensionChanged));

        private static void OnBindingExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = GetBindingAdapter(d);
            if (container != null)
                return;

            container = new BindingAdapter((FrameworkElement)d);
            SetBindingAdapter(d, container);
        }

        private void UpdateExtension()
        {
            var extension = GetBindingExtension(_owner);
            if (extension == null)
                return;

            extension.DataContext = _owner.DataContext;
            extension.Owner = _owner;
        }

        #endregion BindingExtension

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateExtension();
            UpdateExtensions();
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            UpdateExtension();
            UpdateExtensions();
        }
    }
}