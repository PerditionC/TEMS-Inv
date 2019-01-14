// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Interaction logic for BarCodeLabel.xaml
    /// </summary>
    public partial class BarCodeLabel : UserControl
    {
        public BarCodeLabel()
        {
            InitializeComponent();
        }

        public string BarCode
        {
            get { return (string)GetValue(BarcodeProperty); }
            set { SetValue(BarcodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Barcode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarcodeProperty =
            DependencyProperty.Register(nameof(BarCode), typeof(string), typeof(BarCodeLabel), new PropertyMetadata("00012345689-2"));
    }
}
