// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class ItemTypeManagementViewModel : MasterDetailItemWindowViewModelBase
    {
        // anything that needs initializing for MSVC designer
        public ItemTypeManagementViewModel() : base(QueryResultEntitySelector.ItemType)
        {
            // site & status are specific to an instance so don't use
            SearchFilter.SiteLocationVisible = false;
            SearchFilter.SiteLocationEnabled = false;
            SearchFilter.SelectItemStatusValuesVisible = false;
            SearchFilter.SelectItemStatusValuesEnabled = false;

            // item types are not limited to specific equipment
            SearchFilter.SelectEquipmentUnitsVisible = false;
            SearchFilter.SelectEquipmentUnitsEnabled = false;

            PropertyChanged += ItemTypeManagementViewModel_PropertyChanged;
        }

        private void ItemTypeManagementViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((CurrentItem != null) && nameof(CurrentItem).Equals(e.PropertyName,StringComparison.InvariantCulture))
            {
                CurrentItem.PropertyChanged += CurrentItem_PropertyChanged;
                // flag other items that may change when selected item changes
                RaisePropertyChanged(nameof(HasMultipleImages));

                // select first image (or clear)
                CurrentImageIndex = 0;
            }
        }

        private void CurrentItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "batteryType")
            {
                RaisePropertyChanged(nameof(RequiresBatteryCount));
                // TODO raise nested properties as well, at least name
            }
            RaisePropertyChanged(nameof(HasMultipleImages));
        }

        public bool RequiresBatteryCount
        {
            get
            {
                try
                {
                    if (CurrentItem is ItemType itemType)
                    {
                        if (itemType.batteryType != null)
                        {
                            return !string.Equals("None", itemType.batteryType.name, StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Warn(e, $"Failed to determine if current item requires batteries - {e.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// Called to convert a CurrentItem into a value to insert into Items collection.
        /// Only called if CurrentItem is set to a value that does not correspond to any
        /// existing SearchResult items in the Items collection; i.e. a newly created CurrentItem
        /// </summary>
        /// <param name="value">the current ItemBase object to convert from</param>
        /// <returns>the GenericItemResult that represents the ItemBase object</returns>
        protected override GenericItemResult ConvertItemsValueFromCurrent(ItemBase value)
        {
            var itemType = value as ItemType;
            if (itemType != null)
            {
                var listItem = new GenericItemResult()
                {
                    id = itemType.id,
                    description = itemType.name,
                    entity = itemType,
                    entityType = nameof(ItemType),
                    isBin = itemType.isBin,
                    isModule = itemType.isModule,
                    itemNumber = itemType.itemTypeId.ToString(),
                    quantity = 0,
                    /*
                    siteLocationId = Guid.Empty,
                    statusId = Guid.Empty,
                    unitTypeName = null,
                    parentId = Guid.Empty,
                    */
                };
                return listItem;
            }
            throw new ArgumentException("Unable to convert value, expected an ItemType!");
        }


        #region OpenManageVendorsWindow Command

        /// <summary>
        /// Command to open the vendor window of the selected item's vendor so can be modified/viewed
        /// </summary>
        public ICommand OpenManageVendorsWindowCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoOpenManageVendorsWindowCommand(), param => IsSelectedItem); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        /// <summary>
        /// Performs the action to open and view vendors window
        /// </summary>
        private void DoOpenManageVendorsWindowCommand()
        {
            try
            {
                var viewModel = new ManageVendorsViewModel();
                ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in DoOpenManageVendorsWindowCommand - {e.Message}");
                throw;
            }

        }

        #endregion // OpenManageVendorsWindow Command


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return InitializeCommand(
                    ref _AddCommand, 
                    param => { CurrentItem = DataRepository.GetDataRepository.GetInitializedItemType(); }, 
                    param => { return true; }
                );
            }
        }
        private ICommand _AddCommand;


        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand CloneCommand
        {
            get
            {
                return InitializeCommand(
                    ref _CloneCommand, 
                    param => 
                    {
                        var clonedItem = DataRepository.GetDataRepository.GetInitializedItemType();
                        var itemType = CurrentItem as ItemType;
                        clonedItem.associatedItems = itemType.associatedItems;
                        clonedItem.batteryCount = itemType.batteryCount;
                        clonedItem.batteryType = itemType.batteryType;
                        clonedItem.category = itemType.category;
                        clonedItem.cost = itemType.cost;
                        clonedItem.isBin = itemType.isBin;
                        clonedItem.isModule = itemType.isModule;
                        clonedItem.make = itemType.make;
                        clonedItem.model = itemType.model;
                        clonedItem.name = itemType.name;
                        clonedItem.notes = itemType.notes;
                        clonedItem.unitOfMeasure = itemType.unitOfMeasure;
                        clonedItem.vendor = itemType.vendor;
                        clonedItem.weight = itemType.weight;
                        clonedItem.documents = new ObservableCollection<Document>(itemType.documents);
                        clonedItem.images = new ObservableCollection<Image>(itemType.images);
                        CurrentItem = clonedItem;
                    }, 
                    param => { return (CurrentItem != null); }
                );
            }
        }
        private ICommand _CloneCommand;


        #region DeleteCommand

        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in backend DB
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return InitializeCommand(ref _DeleteCommand, param => this.DoDelete(), param => IsSelectedItem); }
        }
        private ICommand _DeleteCommand;

        /// <summary>
        /// default implementation just deletes from database if db is open
        /// Allows subclasses to alter how deletion occurs, e.g. remove additional tables or files
        /// </summary>
        /// <param name="item"></param>
        protected virtual void DoDeleteItem(ItemBase item)
        {
            DataRepository.GetDataRepository.Delete(item);
        }

        /// <summary>
        /// Removes the selected item from list of items including marking deleted in database
        /// </summary>
        private void DoDelete()
        {
            Mediator.InvokeCallback(nameof(YesNoDialogMessage),
                new YesNoDialogMessage
                {
                    caption = "Delete",
                    message = "Do you want to delete this item?",
                    NoAction = (x) => { /* do nothing */ },
                    YesAction = (x) =>
                    {
                        try
                        {
                            // actually remove from DB
                            if (CurrentItem != null) DoDeleteItem(CurrentItem);
                            // update display
                            Items.Remove(SelectedItem);
                            //RaisePropertyChanged(nameof(items));
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, "Failed to delete selected item!");
                            //throw; swallow error, todo - show an error!
                        }
                    },

                });


            // either item removed (no item) or delete canceled/failed - either way don't leave item selected
            SelectedItem = null;
        }

        #endregion DeleteCommand



        #region documents

        public Document CurrentDocument
        {
            get { return _currentDocument; }
            set { SetProperty<Document>(ref _currentDocument, value, nameof(CurrentDocument)); }
        }
        private Document _currentDocument = null;

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand DocumentViewCommand
        {
            get { return InitializeCommand(ref _DocumentViewCommand, param => DoDocumentViewCommand(), param => CurrentDocument != null); }
        }
        private ICommand _DocumentViewCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoDocumentViewCommand()
        {
            string tempfile = string.Empty;
            try
            {
                // get temp file name, but need proper extension so opens in correct application
                // if file exists then overwrite it, create subdir to help ensure we don't destroy user data
                tempfile = Path.GetTempPath() + @"TEMS\"; // + currentDocument.name;
                try { Directory.CreateDirectory(tempfile); } catch { /* ignore all errors creating subdirectory */ }
                tempfile += CurrentDocument.name;
                logger.Debug($"DoDocumentViewCommand - temp file is \"{tempfile}\"");
                using (File.Create(tempfile)) { }
                // mark file as temporary
                FileInfo fileInfo = new FileInfo(tempfile)
                {
                    Attributes = FileAttributes.Temporary
                };
                // save DB contents to file
                File.WriteAllBytes(tempfile, CurrentDocument.data);

                // spawn viewer program and track usage to attempt file deletion on program exit
                Process.Start(tempfile);

                // TODO - either make this class disposable and delete temp TEMS directory on close
                // or create and remove temp directory at startup and exit of program <-- preferred
            }
            catch (IOException eIO)
            {
                logger.Warn(eIO, $"Unable to generate and view temp file '{tempfile}' from DB contents - {eIO.Message}");
                Mediator.InvokeCallback(nameof(MessageDialogMessage),
                    new MessageDialogMessage
                    {
                        caption = "Document view failed",
                        message = $"Unable to create temp file and open document for viewing! - {eIO.Message}"
                    });
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in DoDocumentViewCommand - {e.Message}");
                throw;
            }
        }


        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand DocumentAddCommand
        {
            get { return InitializeCommand(ref _DocumentAddCommand, param => DoDocumentAddCommand(), null); }
        }
        private ICommand _DocumentAddCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoDocumentAddCommand()
        {
            try
            {
                try
                {
                    if (CurrentItem is ItemType itemType)
                    {
                        Document doc = new Document
                        {
                            data = SelectDataFile(out string filename, "Document files(*.doc, *.txt) | *.doc; *.txt | All files(*.*) | *.*")
                        };
                        if (doc.data.Length > 0)
                        {
                            doc.name = filename;
                            doc.description = filename; // for now so filename shows in tooltip
                            itemType.documents.Add(doc);
                            // force change so SaveUser knows currentItem has changed   TODO implement better
                            itemType.documents = itemType.documents; // forces IsChanged == true & Raising documents changed
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Exception in Do...Command - {e.Message}");
                    throw;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }

        }

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand DocumentDeleteCommand
        {
            get { return InitializeCommand(ref _DocumentDeleteCommand, param => DoDocumentDeleteCommand(), param => CurrentDocument != null); }
        }
        private ICommand _DocumentDeleteCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoDocumentDeleteCommand()
        {
            try
            {
                if (CurrentItem is ItemType itemType /* && currentDocument != null */)
                {
                    // TODO actually delete from DB

                    itemType.documents.Remove(CurrentDocument);
                    // force IsChanged and raising properties changed
                    itemType.documents = itemType.documents;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }

        }

        #endregion // documents


        /// <summary>
        /// Verifies if file exists, if so returns data as byte[]
        /// </summary>
        /// <param name="filter">default filter name, e.g. "Image files (*.bmp, *jpg)|*.bmp;*.jpg|All files (*.*)|*.*"</param>
        /// <returns>string with path of SQLite database file to attempt to use</returns>
        private byte[] SelectDataFile(out string filename, string filter = "All files (*.*)|*.*")
        {
            string selectedFilename = string.Empty; // so can show if there are errors!
            filename = string.Empty;

            try
            {
                Mediator.InvokeCallback(nameof(OpenFileDialogMessage),
                    new OpenFileDialogMessage
                    {
                        //caption = "Open file",
                        message = "Please select file to insert:",
                        //InitialDirectory = defaultPath,
                        //DefaultExt = ext,
                        Filter = filter,
                        CheckFileExists = true,
                        ShowReadOnly = true,
                        DereferenceLinks = true,
                        Multiselect = false,
                        CanceledAction = (x) => { /* do nothing */ },
                        SelectedAction = (x) => { selectedFilename = x as string; },
                    });
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to load data file {selectedFilename}!");
                // not considered a fatal error!
                Mediator.InvokeCallback(nameof(MessageDialogMessage),
                    new MessageDialogMessage
                    {
                        caption = "File load failed",
                        message = $"Unable to retrieve contents for {selectedFilename} - {e.Message}"
                    });
            }

            if ((selectedFilename != string.Empty) && System.IO.File.Exists(selectedFilename))
            {
                filename = Path.GetFileName(selectedFilename);
                return File.ReadAllBytes(selectedFilename);
            }

            // on any errors return an empty byte array
            return new byte[0];
        }


        #region images

        public string ImageHeader
        {
            get
            {
                var imageXofN = (CurrentImage == null) ? "no images" : $"{CurrentImageIndex + 1} of {((ItemType)CurrentItem).images.Count}";
                return $"Image: {imageXofN}";
            }
        }

        public Image CurrentImage
        {
            get { return _currentImage; }
            set
            {
                SetProperty<Image>(ref _currentImage, value, nameof(CurrentImage));
                RaisePropertyChanged(nameof(ImageHeader));
                logger.Debug($"Image name={CurrentImage?.name} with description=[{CurrentImage?.description}]");
            }
        }
        private Image _currentImage = null;

        public int CurrentImageIndex
        {
            get { return _currentImageIndex; }
            set
            {
                // select first image
                var itemType = CurrentItem as ItemType;
                if ((value >= 0) && (value < itemType?.images.Count))
                {
                    SetProperty<int>(ref _currentImageIndex, value, nameof(CurrentImageIndex));
                    //currentImage = itemType.images.First<Image>();
                    CurrentImage = itemType.images[_currentImageIndex];
                }
                else
                {
                    SetProperty<int>(ref _currentImageIndex, -1, nameof(CurrentImageIndex));
                    CurrentImage = null;
                }
            }
        }
        private int _currentImageIndex = -1;

        /// <summary>
        /// property that indicates multiple images associated with currently selected item
        /// </summary>
        public bool HasMultipleImages
        {
            get
            {
                if ((CurrentItem as ItemType)?.images?.Count > 1)
                {
                    System.Diagnostics.Debug.WriteLine("HasMultipleImages == true");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand ImageLeftCommand
        {
            get { return InitializeCommand(ref _ImageLeftCommand, param => DoImageLeftCommand(), param => HasMultipleImages && (CurrentImageIndex > 0)); }
        }
        private ICommand _ImageLeftCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageLeftCommand()
        {
            try
            {
                CurrentImageIndex--;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }

        }

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand ImageRightCommand
        {
            get { return InitializeCommand(ref _ImageRightCommand, param => DoImageRightCommand(), param => HasMultipleImages && (CurrentImageIndex < (((ItemType)CurrentItem)?.images?.Count - 1))); }
        }
        private ICommand _ImageRightCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageRightCommand()
        {
            try
            {
                CurrentImageIndex++;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }

        }

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand ImageAddCommand
        {
            get { return InitializeCommand(ref _ImageAddCommand, param => DoImageAddCommand(), null); }
        }
        private ICommand _ImageAddCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageAddCommand()
        {
            try
            {
                if (CurrentItem is ItemType itemType)
                {
                    Image image = new Image
                    {
                        data = SelectDataFile(out string filename, "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif | All files(*.*) | *.*")
                    };
                    if (image.data.Length > 0)
                    {
                        image.name = filename;
                        image.description = filename; // for now so filename shows in tooltip
                        itemType.images.Add(image);
                        // force IsChanged and raising properties changed
                        itemType.images = itemType.images;
                        // raise so left and right arrows visibility update correctly
                        RaisePropertyChanged(nameof(HasMultipleImages));
                        // assume added as last image in index
                        CurrentImageIndex = itemType.images.Count - 1;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand ImageDeleteCommand
        {
            get { return InitializeCommand(ref _ImageDeleteCommand, param => DoImageDeleteCommand(), param => CurrentImage != null); }
        }
        private ICommand _ImageDeleteCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageDeleteCommand()
        {
            try
            {
                if (CurrentItem is ItemType itemType /* && currentImage != null */)
                {
                    // TODO actually delete from DB

                    itemType.images.RemoveAt(CurrentImageIndex);
                    // force IsChanged and raising properties changed
                    itemType.images = itemType.images;
                    // raise so left and right arrows visibility update correctly
                    RaisePropertyChanged(nameof(HasMultipleImages));
                    // update displayed image
                    if (CurrentImageIndex >= itemType.images.Count)
                        CurrentImageIndex = CurrentImageIndex - 1;
                    else
                        CurrentImageIndex = CurrentImageIndex;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in Do...Command - {e.Message}");
                throw;
            }

        }

        #endregion // images
    }
}
