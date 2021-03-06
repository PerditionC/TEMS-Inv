﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.userManager;


namespace TEMS_Inventory.views
{
    public class ViewPrintLabelsViewModel : DetailsViewModelBase
    {
        public ViewPrintLabelsViewModel() : base()
        {
            try
            {
                /*
                // site & status are specific to an instance so don't use
                SearchFilter.SiteLocationVisible = false;
                SearchFilter.SiteLocationEnabled = false;
                SearchFilter.SelectItemStatusValuesVisible = false;
                SearchFilter.SelectItemStatusValuesEnabled = false;
                */

#if false       // allow for now to limit query to results in specific equipment
                // and these are specific to item not item type
                SearchFilter.SelectEquipmentUnitsVisible = false;
                SearchFilter.SelectEquipmentUnitsEnabled = false;
#endif
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to Initialize() item type {nameof(ViewPrintLabelsViewModel)}.");
                throw;
            }
        }

        // multiple selections? or simply everything below current selected
        // for each selected item, barcode label with its item number

        // label paper type
        // skip N (previously used) labels
    }
}
