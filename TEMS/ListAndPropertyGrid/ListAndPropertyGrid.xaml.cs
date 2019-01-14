// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS_Inventory.views;

namespace TEMS_Inventory.UserControls
{
    /// <summary>
    /// Interaction logic for ListAndPropertyGrid.xaml
    /// </summary>
    public partial class ListAndPropertyGrid : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private PropertyDetails propertyDetails;

        public ListAndPropertyGrid() : base()
        {
            InitializeComponent();
            Initialize(null, null);
        }

        public void Initialize(Func<ItemBase> GetNewItem, ObservableCollection<ItemBase> items)
        {
            ViewModel.Initialize(GetNewItem, items);  // e.g. delegate () { return (ItemBase)Activator.CreateInstance(objType); }

            // create an instance of our property details creator and pass our container object
            propertyDetails = new PropertyDetails(DetailView);
            propertyDetails.AddDetails(GetNewItem);
        }

        public ItemListToAddEditDeleteViewModel ViewModel { get; } = new ItemListToAddEditDeleteViewModel();
    }
}
