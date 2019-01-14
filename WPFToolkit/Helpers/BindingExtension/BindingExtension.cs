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

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings the possibility to modify existing bindings. See <see cref="DW.WPFToolkit.Helpers.BindingAdapter" />.
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
    public class BindingExtension : FrameworkElement
    {
        private readonly bool _isChangedInternally;

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Helpers.BindingExtension" /> class.
        /// </summary>
        public BindingExtension()
        {
            _isChangedInternally = true;

            new Binding().CopyInto(this);

            _isChangedInternally = false;
        }

        internal FrameworkElement Owner
        {
            get { return (FrameworkElement)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        internal static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(FrameworkElement), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the property which binding have to be modified.
        /// </summary>
        public DependencyProperty Property
        {
            get { return (DependencyProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.Property" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof(DependencyProperty), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the UpdateSourceTrigger in the modified binding.
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return (UpdateSourceTrigger)GetValue(UpdateSourceTriggerProperty); }
            set { SetValue(UpdateSourceTriggerProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.UpdateSourceTrigger" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateSourceTriggerProperty =
            DependencyProperty.Register("UpdateSourceTrigger", typeof(UpdateSourceTrigger), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the ValidatesOnDataErrors in the modified binding.
        /// </summary>
        public bool ValidatesOnDataErrors
        {
            get { return (bool)GetValue(ValidatesOnDataErrorsProperty); }
            set { SetValue(ValidatesOnDataErrorsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.ValidatesOnDataErrors" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidatesOnDataErrorsProperty =
            DependencyProperty.Register("ValidatesOnDataErrors", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the Mode in the modified binding.
        /// </summary>
        public BindingMode Mode
        {
            get { return (BindingMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.Mode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(BindingMode), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the Path in the modified binding.
        /// </summary>
        public PropertyPath Path
        {
            get { return (PropertyPath)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.Path" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(PropertyPath), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the AsyncState in the modified binding.
        /// </summary>
        public object AsyncState
        {
            get { return (object)GetValue(AsyncStateProperty); }
            set { SetValue(AsyncStateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.AsyncState" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AsyncStateProperty =
            DependencyProperty.Register("AsyncState", typeof(object), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the BindingGroupName in the modified binding.
        /// </summary>
        public string BindingGroupName
        {
            get { return (string)GetValue(BindingGroupNameProperty); }
            set { SetValue(BindingGroupNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.BindingGroupName" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingGroupNameProperty =
            DependencyProperty.Register("BindingGroupName", typeof(string), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the BindsDirectlyToSource in the modified binding.
        /// </summary>
        public bool BindsDirectlyToSource
        {
            get { return (bool)GetValue(BindsDirectlyToSourceProperty); }
            set { SetValue(BindsDirectlyToSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.BindsDirectlyToSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindsDirectlyToSourceProperty =
            DependencyProperty.Register("BindsDirectlyToSource", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the Converter in the modified binding.
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.Converter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the ConverterCulture in the modified binding.
        /// </summary>
        public CultureInfo ConverterCulture
        {
            get { return (CultureInfo)GetValue(ConverterCultureProperty); }
            set { SetValue(ConverterCultureProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.ConverterCulture" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterCultureProperty =
            DependencyProperty.Register("ConverterCulture", typeof(CultureInfo), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the ConverterParameter in the modified binding.
        /// </summary>
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.ConverterParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the ElementName in the modified binding.
        /// </summary>
        public string ElementName
        {
            get { return (string)GetValue(ElementNameProperty); }
            set { SetValue(ElementNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.ElementName" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementNameProperty =
            DependencyProperty.Register("ElementName", typeof(string), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the FallbackValue in the modified binding.
        /// </summary>
        public object FallbackValue
        {
            get { return (object)GetValue(FallbackValueProperty); }
            set { SetValue(FallbackValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.FallbackValue" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty FallbackValueProperty =
            DependencyProperty.Register("FallbackValue", typeof(object), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the IsAsync in the modified binding.
        /// </summary>
        public bool IsAsync
        {
            get { return (bool)GetValue(IsAsyncProperty); }
            set { SetValue(IsAsyncProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.IsAsync" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAsyncProperty =
            DependencyProperty.Register("IsAsync", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the NotifyOnSourceUpdated in the modified binding.
        /// </summary>
        public bool NotifyOnSourceUpdated
        {
            get { return (bool)GetValue(NotifyOnSourceUpdatedProperty); }
            set { SetValue(NotifyOnSourceUpdatedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.NotifyOnSourceUpdated" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotifyOnSourceUpdatedProperty =
            DependencyProperty.Register("NotifyOnSourceUpdated", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the NotifyOnTargetUpdated in the modified binding.
        /// </summary>
        public bool NotifyOnTargetUpdated
        {
            get { return (bool)GetValue(NotifyOnTargetUpdatedProperty); }
            set { SetValue(NotifyOnTargetUpdatedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.NotifyOnTargetUpdated" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotifyOnTargetUpdatedProperty =
            DependencyProperty.Register("NotifyOnTargetUpdated", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the NotifyOnValidationError in the modified binding.
        /// </summary>
        public bool NotifyOnValidationError
        {
            get { return (bool)GetValue(NotifyOnValidationErrorProperty); }
            set { SetValue(NotifyOnValidationErrorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.NotifyOnValidationError" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotifyOnValidationErrorProperty =
            DependencyProperty.Register("NotifyOnValidationError", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the StringFormat in the modified binding.
        /// </summary>
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.StringFormat" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the TargetNullValue in the modified binding.
        /// </summary>
        public object TargetNullValue
        {
            get { return (object)GetValue(TargetNullValueProperty); }
            set { SetValue(TargetNullValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.TargetNullValue" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetNullValueProperty =
            DependencyProperty.Register("TargetNullValue", typeof(object), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the UpdateSourceExceptionFilter in the modified binding.
        /// </summary>
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        {
            get { return (UpdateSourceExceptionFilterCallback)GetValue(UpdateSourceExceptionFilterProperty); }
            set { SetValue(UpdateSourceExceptionFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.UpdateSourceExceptionFilter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateSourceExceptionFilterProperty =
            DependencyProperty.Register("UpdateSourceExceptionFilter", typeof(UpdateSourceExceptionFilterCallback), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the ValidatesOnExceptions in the modified binding.
        /// </summary>
        public bool ValidatesOnExceptions
        {
            get { return (bool)GetValue(ValidatesOnExceptionsProperty); }
            set { SetValue(ValidatesOnExceptionsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.ValidatesOnExceptions" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidatesOnExceptionsProperty =
            DependencyProperty.Register("ValidatesOnExceptions", typeof(bool), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        /// <summary>
        /// Gets or sets the XPath in the modified binding.
        /// </summary>
        public string XPath
        {
            get { return (string)GetValue(XPathProperty); }
            set { SetValue(XPathProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.BindingExtension.XPath" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty XPathProperty =
            DependencyProperty.Register("XPath", typeof(string), typeof(BindingExtension), new PropertyMetadata(OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extension = (BindingExtension)d;
            extension.Update();
        }

        private void Update()
        {
            if (_isChangedInternally)
                return;

            if (Owner == null || Property == null)
                return;

            var expression = Owner.GetBindingExpression(Property);
            if (expression == null || expression.ParentBinding == null)
                return;

            var refreshedBinding = expression.ParentBinding.Clone();
            refreshedBinding.Take(this);

            Owner.SetBinding(Property, refreshedBinding);
        }
    }
}
