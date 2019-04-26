// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using NLog;
using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ReferenceDataWindow.xaml
    /// </summary>
    public partial class ReferenceDataWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ReferenceDataWindow()
        {
            InitializeComponent();

            // initialize list of reference items
            referenceDataType.ItemsSource = ReferenceDataCache.ReferenceDataTypes;

            // pick first item and show
            referenceDataType.SelectedIndex = 0;
        }

        private void referenceDataType_SelectionChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.AddedItems.Count >= 1)
                {
                    // selection is one of referenceDataType items added to combobox in
                    // constructor, should match name of DB table and corresponding DataModel class
                    dynamic selection = eventArgs.AddedItems[0];
                    logger.Debug(selection);

                    // get reference to data repository from main Application 
                    var db = DataRepository.GetDataRepository;
                    try
                    {
                        // convert class name to Type object, note since not in same DLL, need to use full specifier
                        var objType = ReferenceDataCache.GetReferenceType(selection.TypeName);

                        // initialize our UserControl with reference to how to create new objects and list of existing ones
#pragma warning disable IDE0039 // Use local function
                        Func<ItemBase> newItemFn = delegate () { return (ItemBase)Activator.CreateInstance(objType); };
#pragma warning restore IDE0039 // Use local function
                        ItemList.Initialize(newItemFn, db.ReferenceData[selection.TypeName]);
                    }
                    catch (Exception e)
                    {
                        // verify able to obtain type, in case typo or other error ...
                        logger.Warn(e, $"ReferenceData.SelectionChanged({selection}) - failed to obtain Type, list not updated.");

                        ItemList.Initialize(null, null);
                    }
                }
                else
                {
                    // if nothing selected, just reset
                    ItemList.Initialize(null, null);
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }
    }
}
