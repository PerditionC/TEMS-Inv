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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the features to text boxes to define its selection or bound the selection part.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <TextBox Text="{Binding TheText}"
    ///          Interactivity:TextBoxBehavior.SelectAllOnFocus="True"
    ///          Interactivity:TextBoxBehavior.SelectedText="{Binding SelectedText}" />
    /// ]]>
    /// </code>
    /// </example>
    public class TextBoxBehavior : DependencyObject
    {
        /// <summary>
        /// Gets the selected text in a text box.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.TextBoxBehavior.SelectedText property value for the element.</returns>
        public static string GetSelectedText(DependencyObject obj)
        {
            return (string)obj.GetValue(SelectedTextProperty);
        }

        /// <summary>
        /// Attaches the information which text has to be selected in a text box.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.TextBoxBehavior.SelectedText value.</param>
        public static void SetSelectedText(DependencyObject obj, string value)
        {
            obj.SetValue(SelectedTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.GetSelectedText(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.SetSelectedText(DependencyObject, string)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached("SelectedText", typeof(string), typeof(TextBoxBehavior), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTextChanged));

        /// <summary>
        /// Gets a value that indicates if everything has to be selected automatically when the text box got the focus.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.TextBoxBehavior.SelectAllOnFocus property value for the element.</returns>
        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        /// <summary>
        /// Attaches a value that indicates if everything has to be selected automatically when the text box got the focus.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.TextBoxBehavior.SelectedText value.</param>
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.GetSelectAllOnFocus(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.SetSelectAllOnFocus(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextBoxBehavior), new UIPropertyMetadata(OnSelectAllOnFocusChanged));

        private static TextBoxBehavior GetTextTextBoxBehavior(DependencyObject obj)
        {
            return (TextBoxBehavior)obj.GetValue(TextTextBoxBehaviorProperty);
        }

        private static void SetTextTextBoxBehavior(DependencyObject obj, TextBoxBehavior value)
        {
            obj.SetValue(TextTextBoxBehaviorProperty, value);
        }

        private static readonly DependencyProperty TextTextBoxBehaviorProperty =
            DependencyProperty.RegisterAttached("TextTextBoxBehavior", typeof(TextBoxBehavior), typeof(TextBoxBehavior), new UIPropertyMetadata(null));

        private static TextBoxBehavior GetAllTextBoxBehavior(DependencyObject obj)
        {
            return (TextBoxBehavior)obj.GetValue(AllTextBoxBehaviorProperty);
        }

        private static void SetAllTextBoxBehavior(DependencyObject obj, TextBoxBehavior value)
        {
            obj.SetValue(AllTextBoxBehaviorProperty, value);
        }

        private static readonly DependencyProperty AllTextBoxBehaviorProperty =
            DependencyProperty.RegisterAttached("AllTextBoxBehavior", typeof(TextBoxBehavior), typeof(TextBoxBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets a value that indicates on which key the text binding has to be refreshed in a text box.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.TextBoxBehavior.RefreshBindingOnKey property value for the element.</returns>
        public static Key GetRefreshBindingOnKey(DependencyObject obj)
        {
            return (Key)obj.GetValue(RefreshBindingOnKeyProperty);
        }

        /// <summary>
        /// Attaches a value that indicates on which key the text binding has to be refreshed in a text box.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.TextBoxBehavior.RefreshBindingOnKey value.</param>
        public static void SetRefreshBindingOnKey(DependencyObject obj, Key value)
        {
            obj.SetValue(RefreshBindingOnKeyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.GetRefreshBindingOnKey(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.TextBoxBehavior.SetRefreshBindingOnKey(DependencyObject, Key)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty RefreshBindingOnKeyProperty =
            DependencyProperty.RegisterAttached("RefreshBindingOnKey", typeof(Key), typeof(TextBoxBehavior), new UIPropertyMetadata(OnRefreshBindingOnKeyChanged));

        private static void OnRefreshBindingOnKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = GetTextBox(sender);
            textBox.KeyUp += TextBox_KeyUp;
        }

        private static void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var box = (TextBox)sender;
            if (e.Key == GetRefreshBindingOnKey(box))
            {
                var expression = box.GetBindingExpression(TextBox.TextProperty);
                expression.UpdateSource();
            }
        }

        private static void OnSelectedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = GetTextBox(sender);
            var behavior = GetTextTextBoxBehavior(sender);
            if (behavior == null)
            {
                behavior = new TextBoxBehavior();
                SetTextTextBoxBehavior(sender, behavior);
                textBox.SelectionChanged += new RoutedEventHandler(behavior.TextBox_SelectionChanged);
            }
            behavior.RunSetSelectedText(textBox, e.NewValue);
        }

        private bool _selfChange;
        private void RunSetSelectedText(TextBox textbox, object param)
        {
            if (!_selfChange &&
                param != null)
                SelectText(textbox, false, param.ToString());
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _selfChange = true;
            var textBox = (TextBox)sender;
            SetSelectedText(textBox, textBox.SelectedText);
            _selfChange = false;
        }

        private static void OnSelectAllOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = GetTextBox(sender);
            var behavior = GetAllTextBoxBehavior(sender);
            if (behavior == null)
            {
                behavior = new TextBoxBehavior();
                SetAllTextBoxBehavior(sender, behavior);
                textBox.GotFocus += new RoutedEventHandler(behavior.TextBox_GotFocus);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            SelectText(textBox, true, null);
        }

        private static TextBox GetTextBox(DependencyObject sender)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                throw new InvalidOperationException("The TextBoxBehavior.SelectionStart only can be attached on a TextBox control");
            return textBox;
        }

        private void SelectText(TextBox box, bool selectAll, string text)
        {
            ControlFocus.GiveFocus(box, delegate
                {
                    if (selectAll)
                        box.SelectAll();
                    else if (!string.IsNullOrEmpty(text))
                    {
                        int pos = box.Text.IndexOf(text);
                        if (pos > 0 &&
                            box.Text.Contains(text))
                            box.Select(pos, text.Length);
                    }
                });
        }
    }
}
