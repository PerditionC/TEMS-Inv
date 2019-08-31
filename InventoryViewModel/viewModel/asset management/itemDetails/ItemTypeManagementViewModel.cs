// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif 

using InventoryViewModel;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating ItemType table
    /// </summary>
    public class ItemTypeManagementViewModel : ItemDetailsViewModel
    {
        public ItemTypeManagementViewModel() : base() { }

        /// <summary>
        /// Initialize to nothing selected to display details of
        /// </summary>
        public override void clear()
        {
            base.clear();

            itemTypeId = 0;
            name = null;
            make = null;
            model = null;
            expirationRestockCategory = ExpirationCategory.None;
            cost = 0;
            weight = 0;
            unitOfMeasure = null;
            category = null;
            batteryCount = 0;
            batteryType = null;
            associatedItems = null;
            isBin = false;
            isModule = false;
            vendor = null;
            notes = null;
            images = new ObservableCollection<Image>();
            documents = new ObservableCollection<Document>();
    }



    public bool RequiresBatteryCount
        {
            get
            {
                try
                {
                    if (batteryType != null)
                    {
                        return !string.Equals("None", batteryType.name, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                catch (Exception e)
                {
                    logger.Warn(e, $"Failed to determine if current item requires batteries - {e.Message}");
                }
                return false;
            }
        }


        #region OpenManageVendorsWindow Command

        /// <summary>
        /// Command to open the vendor window of the selected item's vendor so can be modified/viewed
        /// </summary>
        public ICommand OpenManageVendorsWindowCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoOpenManageVendorsWindowCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        /// <summary>
        /// Performs the action to open and view vendors window
        /// </summary>
        private void DoOpenManageVendorsWindowCommand()
        {
            try
            {
                ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ManageVendors", searchText=vendor?.name });
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
                    param =>
                    {
                        var defaultItem = DataRepository.GetDataRepository.GetInitializedItemType();
                        Mapper.GetMapper().Map(defaultItem, this);
                    },
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
                        // get a new unique id, upon saving this will then map to a new (cloned) record
                        var defaultItem = DataRepository.GetDataRepository.GetInitializedItemType();
                        guid = defaultItem.id;
                        itemTypeId = defaultItem.itemTypeId;
                        CurrentItem.id = guid;
                        CurrentItem.entity = null;

                        // TODO verify if need to fixup documents and images
                        //clonedItem.documents = new ObservableCollection<Document>(itemType.documents);
                        //clonedItem.images = new ObservableCollection<Image>(itemType.images);
                    },
                    param => { return (CurrentItem != null); }
                );
            }
        }
        private ICommand _CloneCommand;


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
                    Document doc = new Document
                    {
                        data = SelectDataFile(out string filename, "Document files(*.doc, *.txt) | *.doc; *.txt | All files(*.*) | *.*")
                    };
                    if (doc.data.Length > 0)
                    {
                        doc.name = filename;
                        doc.description = filename; // for now so filename shows in tooltip
                        documents.Add(doc);
                        // force change so SaveUser knows currentItem has changed   TODO implement better
                        documents = documents; // forces IsChanged == true & Raising documents changed
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
                if (CurrentDocument != null)
                {
                    // TODO actually delete from DB

                    documents.Remove(CurrentDocument);
                    // force IsChanged and raising properties changed
                    documents = documents;
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
                var imageXofN = (CurrentImage == null) ? "no images" : $"{CurrentImageIndex + 1} of {images.Count}";
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
                if ((value >= 0) && (value < images.Count))
                {
                    SetProperty<int>(ref _currentImageIndex, value, nameof(CurrentImageIndex));
                    //currentImage = itemType.images.First<Image>();
                    CurrentImage = images[_currentImageIndex];
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
                if (images?.Count > 1)
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
            get { return InitializeCommand(ref _ImageRightCommand, param => DoImageRightCommand(), param => HasMultipleImages && (CurrentImageIndex < (images?.Count - 1))); }
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
                Image image = new Image
                {
                    data = SelectDataFile(out string filename, "Image files(*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif | All files(*.*) | *.*")
                };
                if (image.data.Length > 0)
                {
                    image.name = filename;
                    image.description = filename; // for now so filename shows in tooltip
                    images.Add(image);
                    // force IsChanged and raising properties changed
                    images = images;
                    // raise so left and right arrows visibility update correctly
                    RaisePropertyChanged(nameof(HasMultipleImages));
                    // assume added as last image in index
                    CurrentImageIndex = images.Count - 1;
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
                if (CurrentImage != null)
                {
                    // TODO actually delete from DB

                    images.RemoveAt(CurrentImageIndex);
                    // force IsChanged and raising properties changed
                    images = images;
                    // raise so left and right arrows visibility update correctly
                    RaisePropertyChanged(nameof(HasMultipleImages));
                    // update displayed image
                    if (CurrentImageIndex >= images.Count)
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


        #region ItemType values
        // mirror of ItemType object, use AutoMapper to convert back & forth between view model and db model

        // external id #, items with same item type id correspond to items of same type
        // Note: identical itemId implies itemNumber differs only by locSuffix
        // WARNING: we currently save via INSERT OR REPLACE, if PK or UNIQUE items conflict the old
        // item is replaced with the new item instead of inserting a new item!
        public int itemTypeId { get { return _itemTypeId; } set { SetProperty(ref _itemTypeId, value, nameof(itemTypeId)); } }
        private int _itemTypeId = 0;

        // what is displayed, description of item
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); } }
        private string _name = null;

        // item make, manufacturer or name brand, e.g. Abbott
        public string make { get { return _make; } set { SetProperty(ref _make, value, nameof(make)); } }
        private string _make = null;

        // item model number, e.g. Precision Xtra
        public string model { get { return _model; } set { SetProperty(ref _model, value, nameof(model)); } }
        private string _model = null;

        // None if does not expire, otherwise is expiration an annual date or date specific restock
        public ExpirationCategory expirationRestockCategory
        {
            get { return _expirationRestockCategory; }
            set { SetProperty(ref _expirationRestockCategory, value, nameof(expirationRestockCategory)); }
        }
        private ExpirationCategory _expirationRestockCategory = ExpirationCategory.None;

        // how much a single unit of item costs in 10th cents
        // *** Assume any price is rounded to the nearest 10th of a cent and displayed in dollars as needed
        // *** Only valid for this item's vendor (i.e. determined by vendor)
        public long cost { get { return _cost; } set { SetProperty(ref _cost, value, nameof(cost)); } }
        private long _cost = 0;

        // how much a single unit of item weighs
        public double weight { get { return _weight; } set { SetProperty(ref _weight, value, nameof(weight)); } }
        private double _weight;

        // unit of measure, what determines a single unit
        public UnitOfMeasure unitOfMeasure
        {
            get { return _unitOfMeasure; }
            set
            {
                SetProperty(ref _unitOfMeasure, value, nameof(unitOfMeasure));
            }
        }
        private UnitOfMeasure _unitOfMeasure = null;

        // item category, purpose of item, e.g. Treatment
        public ItemCategory category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value, nameof(category));
            }
        }
        private ItemCategory _category = null;

        // count of batteries item requires, 0 if it doesn't require any
        public int batteryCount
        {
            get { return _batteryCount; }
            set { SetProperty(ref _batteryCount, value, nameof(batteryCount)); }
        }
        private int _batteryCount = 0;

        // what type of batteries item requires, set to predefined type "None" if does not require any
        public BatteryType batteryType
        {
            get { return _batteryType; }
            set
            {
                SetProperty(ref _batteryType, value, nameof(batteryType));
                if (RequiresBatteryCount)
                {
                    if (batteryCount < 1) batteryCount = 1;
                }
                else
                {
                    batteryCount = 0;
                }
                RaisePropertyChanged(nameof(RequiresBatteryCount));
            }
        }
        private BatteryType _batteryType = null;

        // additional items (not tracked separately) associated with this one
        //[FieldLabel(PrettyName: "Additional Items", ToolTip = "Additional items (not tracked separately) associated with this one.")]
        public string associatedItems { get { return _associatedItems; } set { SetProperty(ref _associatedItems, value, nameof(associatedItems)); } }
        private string _associatedItems = null;

        // is this a bin (can contain other bins, modules, or items), DB should add an index for this value
        public bool isBin
        {
            get { return _isBin; }
            set
            {
                SetProperty(ref _isBin, value, nameof(isBin));
                if (value && _isModule) isModule = false; // can not be both
            }
        }
        private bool _isBin;

        // is this a modules (can contain other modules or items), DB should add an index for this value
        public bool isModule
        {
            get { return _isModule; }
            set
            {
                SetProperty(ref _isModule, value, nameof(isModule));
                if (value && _isBin) isBin = false; // can not be both
            }
        }
        private bool _isModule;

        //SELECT distinct VendorDetail.id, VendorDetail.name, ItemType.name, ItemType.id FROM ItemType JOIN (Item JOIN (ItemInstance JOIN (VendorSiteAccountInfo JOIN VendorDetail ON VendorSiteAccountInfo.vendorDetailId=VendorDetail.id) ON ItemInstance.vendorSiteAccountInfoId=VendorSiteAccountInfo.id) ON ItemInstance.itemId=Item.id) ON Item.itemTypeId=ItemType.id ORDER By ItemType.name, VendorDetail.name;
        // vendor information, currently all items are centrally purchased so
        // may assume same item type has same vendor regardless of where item ultimately goes
        public VendorDetail vendor
        {
            get { return _vendor; }
            set
            {
                SetProperty(ref _vendor, value, nameof(vendor));
            }
        }
        private VendorDetail _vendor = null;

        // additional remarks, applies to all items of this type regardless of unit or site
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }
        private string _notes = null;


        // image data for picture(s) of item (png, gif, jpg, etc. - raw data)
        // images are added to separate table (not usually required so data loaded only when needed)
        // foreign key(s) into Image table, empty set if none specified
        public ObservableCollection<Image> images
        {
            get { return _images; }
            set { SetProperty(ref _images, value, nameof(images)); }
        }
        private ObservableCollection<Image> _images = new ObservableCollection<Image>();

        // documentation such as user manuals for item (txt, pdf, etc. - raw data)
        // documents are added to separate table (not usually required so data loaded only when needed)
        // foreign key(s) into Document table, empty set if none specified
        public ObservableCollection<Document> documents
        {
            get { return _documents; }
            set { SetProperty(ref _documents, value, nameof(documents)); }
        }
        private ObservableCollection<Document> _documents = new ObservableCollection<Document>();
        #endregion // ItemType values
    }
}
