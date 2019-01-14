// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// based on https://github.com/RSuter/MyToolkit
// license: MS Public License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventoryViewWPF
{
    /// <summary>
    /// Interaction logic for FilePickerBase.xaml
    /// </summary>
    public abstract partial class FilePickerBase : UserControl
    {
        protected FilePickerBase()
        {
            InitializeComponent();
            ((FrameworkElement)Content).DataContext = this;
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath", typeof(string), typeof(FilePickerBase), new PropertyMetadata(default(string)));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty DefaultExtensionProperty = DependencyProperty.Register(
            "DefaultExtension", typeof(string), typeof(FilePickerBase), new PropertyMetadata(".*"));

        public string DefaultExtension
        {
            get { return (string)GetValue(DefaultExtensionProperty); }
            set { SetValue(DefaultExtensionProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", typeof(string), typeof(FilePickerBase), new PropertyMetadata("All Files (.*)|*.*"));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        protected abstract void SelectFile();

        private void OnSelectFile(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }
    }
}