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

namespace TEMS_Inventory.views
{
    public class ItemTypeManagementViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public ItemTypeManagementViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            try
            {
                base.Initialize(db, GetNewItem);
                // site & status are specific to an instance so don't use
                SearchFilter.SiteLocationVisible = false;
                SearchFilter.SiteLocationEnabled = false;
                SearchFilter.SelectItemStatusValuesVisible = false;
                SearchFilter.SelectItemStatusValuesEnabled = false;

#if false       // allow for now to limit query to results in specific equipment
                // and these are specific to item not item type
                SearchFilter.SelectEquipmentUnitsVisible = false;
                SearchFilter.SelectEquipmentUnitsEnabled = false;
#endif
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to Initialize() item type viewmodel.");
                throw;
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
                    var itemType = currentItem as ItemType;
                    if (itemType != null && itemType.batteryType != null)
                    {
                        return !string.Equals("None", itemType.batteryType.name, StringComparison.InvariantCultureIgnoreCase);
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
        /// returns filled in result to use with item list
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public override QueryResultsBase ItemResult(object primaryKey)
        {
            return db.QueryItemTypeResult((Guid)primaryKey);
        }

        /// <summary>
        /// load ItemResult based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            try
            {
                if (currentItem != null) currentItem.PropertyChanged -= CurrentItem_PropertyChanged;

                if ((selListItem?.pk != null) && (selListItem.pk != Guid.Empty))
                {
                    selectedItem = db.db.Load<ItemType>(selListItem.pk);

#if false
                    // if not currently editing anything then we match current item, but
                    // we don't update currentItem otherwise as may be a clone, etc.
                    if (isDetailViewInActive && EditCommand.CanExecute(null)) EditCommand.Execute(null);
#else
                    // update detail view immediately when a new item is selected
                    // note this will loose changes for a clone
                    if (currentItem != null && currentItem.IsChanged)
                    {
                        var x = MessageBox.Show("Current item has been modified, do you wish to save changes?", $"Changes to {currentItem.displayName} will be lost!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                        if (x == MessageBoxResult.Yes)
                        {
                            if (SaveCommand.CanExecute(null)) SaveCommand.Execute(null);
                        }
                    }
                    if (EditCommand.CanExecute(null)) EditCommand.Execute(null);
                    /*
                    currentItem = selectedItem;  //DoEdit();
                                                 // Note: need same object so changes triggered via items bound to selectedListItem will show in detail view
                    selListItem.entity = currentItem;
                    */

#endif
                }
                else
                {
                    selectedItem = null;
                }

                if (currentItem != null) currentItem.PropertyChanged += CurrentItem_PropertyChanged;
                // flag other items that may change when selected item changes
                RaisePropertyChanged(nameof(HasMultipleImages));

                // select first image (or clear)
                currentImageIndex = 0;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in loadSelectedItem - {e.Message}");
                throw;
            }
        }


        protected override void DoSearch()
        {
            try
            {
                logger.Debug("Loading item types - DoSearch:\n" + SearchFilter.ToString());

                items = db.GetItemTypeList(SearchFilter);
                // autoselect if only 1 item type returned
                if (items.Count == 1) selectedListItem = items[0];
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in DoSearch - {e.Message}");
                throw;
            }
        }


        #region OpenManageVendorsWindow Command

        /// <summary>
        /// Command to open the vendor window of the selected item's vendor so can be modified/viewed
        /// </summary>
        public ICommand OpenManageVendorsWindowCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoOpenManageVendorsWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        /// <summary>
        /// Performs the action to open and view vendors window
        /// </summary>
        private void DoOpenManageVendorsWindowCommand()
        {
            try
            {
                var newWin = new ManageVendorsWindow();
                ShowChildWindow(newWin);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in DoOpenManageVendorsWindowCommand - {e.Message}");
                throw;
            }

        }

        #endregion // OpenManageVendorsWindow Command

        #region documents

        public Document currentDocument
        {
            get { return _currentDocument; }
            set { SetProperty<Document>(ref _currentDocument, value, nameof(currentDocument)); }
        }
        private Document _currentDocument = null;

        /// <summary>
        /// Command to ...
        /// </summary>
        public ICommand DocumentViewCommand
        {
            get { return InitializeCommand(ref _DocumentViewCommand, param => DoDocumentViewCommand(), param => currentDocument != null); }
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
                tempfile += currentDocument.name;
                logger.Debug($"DoDocumentViewCommand - temp file is \"{tempfile}\"");
                using (File.Create(tempfile)) { }
                // mark file as temporary
                FileInfo fileInfo = new FileInfo(tempfile)
                {
                    Attributes = FileAttributes.Temporary
                };
                // save DB contents to file
                File.WriteAllBytes(tempfile, currentDocument.data);

                // spawn viewer program and track usage to attempt file deletion on program exit
                Process.Start(tempfile);

                // TODO - either make this class disposable and delete temp TEMS directory on close
                // or create and remove temp directory at startup and exit of program <-- preferred
            }
            catch (IOException eIO)
            {
                logger.Warn(eIO, $"Unable to generate and view temp file '{tempfile}' from DB contents - {eIO.Message}");
                MessageBox.Show($"Unable to create temp file and open document for viewing! - {eIO.Message}", "Document view failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    var itemType = currentItem as ItemType;
                    if (itemType != null)
                    {
                        Document doc = new Document();
                        string filename;
                        doc.data = SelectDataFile(out filename, "Document files(*.doc, *.txt) | *.doc; *.txt | All files(*.*) | *.*");
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
            get { return InitializeCommand(ref _DocumentDeleteCommand, param => DoDocumentDeleteCommand(), param => currentDocument != null); }
        }
        private ICommand _DocumentDeleteCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoDocumentDeleteCommand()
        {
            try
            {
                var itemType = currentItem as ItemType;
                if (itemType != null /* && currentDocument != null */)
                {
                    // TODO actually delete from DB

                    itemType.documents.Remove(currentDocument);
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
                var dlg = new OpenFileDialog
                {
                    Title = "Please select file to insert:",
                    //InitialDirectory = defaultPath,
                    //DefaultExt = ext,
                    Filter = filter,
                    CheckFileExists = true,
                    ShowReadOnly = true,
                    DereferenceLinks = true,
                    Multiselect = false
                };
                if (dlg.ShowDialog() == true)
                {
                    selectedFilename = dlg.FileName;
                    if (System.IO.File.Exists(selectedFilename))
                    {
                        filename = Path.GetFileName(selectedFilename);
                        return File.ReadAllBytes(selectedFilename);
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to load data file {selectedFilename}!");
                // not considered a fatal error!
                MessageBox.Show($"Unable to retrieve contents for {selectedFilename} - {e.Message}");
            }

            // on any errors return an empty byte array
            return new byte[0];
        }


        #region images

        public string ImageHeader
        {
            get
            {
                var imageXofN = (currentImage == null) ? "no images" : $"{currentImageIndex + 1} of {((ItemType)selectedItem).images.Count}";
                return $"Image: {imageXofN}";
            }
        }

        public Image currentImage
        {
            get { return _currentImage; }
            set
            {
                SetProperty<Image>(ref _currentImage, value, nameof(currentImage));
                RaisePropertyChanged(nameof(ImageHeader));
                logger.Debug($"Image name={currentImage?.name} with description=[{currentImage?.description}]");
            }
        }
        private Image _currentImage = null;

        public int currentImageIndex
        {
            get { return _currentImageIndex; }
            set
            {
                // select first image
                var itemType = selectedItem as ItemType;
                if ((value >= 0) && (value < itemType?.images.Count))
                {
                    SetProperty<int>(ref _currentImageIndex, value, nameof(currentImageIndex));
                    //currentImage = itemType.images.First<Image>();
                    currentImage = itemType.images[_currentImageIndex];
                }
                else
                {
                    SetProperty<int>(ref _currentImageIndex, -1, nameof(currentImageIndex));
                    currentImage = null;
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
                if ((currentItem as ItemType)?.images?.Count > 1)
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
            get { return InitializeCommand(ref _ImageLeftCommand, param => DoImageLeftCommand(), param => HasMultipleImages && (currentImageIndex > 0)); }
        }
        private ICommand _ImageLeftCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageLeftCommand()
        {
            try
            {
                currentImageIndex--;
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
            get { return InitializeCommand(ref _ImageRightCommand, param => DoImageRightCommand(), param => HasMultipleImages && (currentImageIndex < (((ItemType)currentItem)?.images?.Count - 1))); }
        }
        private ICommand _ImageRightCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageRightCommand()
        {
            try
            {
                currentImageIndex++;
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
                var itemType = currentItem as ItemType;
                if (itemType != null)
                {
                    Image image = new Image();
                    string filename;
                    image.data = SelectDataFile(out filename, "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif | All files(*.*) | *.*");
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
                        currentImageIndex = itemType.images.Count - 1;
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
            get { return InitializeCommand(ref _ImageDeleteCommand, param => DoImageDeleteCommand(), param => currentImage != null); }
        }
        private ICommand _ImageDeleteCommand;

        /// <summary>
        /// Performs the action to ...
        /// </summary>
        private void DoImageDeleteCommand()
        {
            try
            {
                var itemType = currentItem as ItemType;
                if (itemType != null /* && currentImage != null */)
                {
                    // TODO actually delete from DB

                    itemType.images.RemoveAt(currentImageIndex);
                    // force IsChanged and raising properties changed
                    itemType.images = itemType.images;
                    // raise so left and right arrows visibility update correctly
                    RaisePropertyChanged(nameof(HasMultipleImages));
                    // update displayed image
                    if (currentImageIndex >= itemType.images.Count)
                        currentImageIndex = currentImageIndex - 1;
                    else
                        currentImageIndex = currentImageIndex;
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
