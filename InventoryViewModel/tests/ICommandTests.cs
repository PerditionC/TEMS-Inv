﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;

namespace Tems_Inventory.Tests
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    using TEMS.InventoryModel.command.action;
    using TEMS.InventoryModel.entity.db;
    using TEMS.InventoryModel.entity.db.query;
    using TEMS.InventoryModel.entity.db.user;
    using TEMS.InventoryModel.userManager;
    using TEMS_Inventory.views;

    [TestFixture]
    public sealed class ICommandTests
    {
        private DataRepository DataRepositoryRef = null;

        private string SampleItemInstance = "b6277b55-7608-4b22-8407-688fca89dbf2";   // ** GUID subject to change, really should save iteminstance then check for that one
        private string SampleItemNumber = "D600-1CFD";


        private sealed class TestDetailsViewModel : DetailsViewModelBase { }


        /// <summary>
        /// loads SQLite and creates and populates our test DB
        /// </summary>
        [SetUp]
        public void setupTest()
        {
            // set current directory to expected value, i.e. directory of binaries, so relative paths for databases work as expected
            // Note: otherwise likely to fail as default current directory may be within protected dir, e.g. Visual Studio install directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())?.Location));

            // WARNING! test.db must be setup already with proper schema
            DataRepositoryRef = new DataRepository(@"C:\DB\test.db");
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
            DataRepositoryRef.Dispose();
            DataRepositoryRef = null;
            System.GC.WaitForPendingFinalizers();
        }

        [Test]
        public void ICommandDataRepositoryAvailable()
        {
            // validate that we can get a valid instance of our DataRepository
            // should succeed as we create our test instance in setupTest()
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db, nameof(DataRepository));
        }

        [Test]
        public void SaveItemCommand_CanSave()
        {
            var fakeBattery = new BatteryType
            {
                name = "Test Battery"
            };
            Assert.That(fakeBattery.IsChanged, nameof(fakeBattery.IsChanged), Is.True);
            Assert.That(fakeBattery.name, Is.EqualTo("Test Battery"));
            Assert.That(fakeBattery.displayName, Is.EqualTo(fakeBattery.name));

            var cmd = new SaveItemCommand();
            Assert.NotNull(cmd, nameof(SaveItemCommand));

            Assert.IsTrue(cmd.CanExecute(fakeBattery));

            fakeBattery.AcceptChanges();
            Assert.IsFalse(cmd.CanExecute(fakeBattery));
        }

        [Test]
        public void SaveItemCommand_CanSave_NullConstraintsNotMet()
        {
            var fakeUnit = new EquipmentUnitType
            {
                name = "Test Trailer"
            };
            Assert.That(fakeUnit.IsChanged, nameof(fakeUnit.IsChanged), Is.True);
            Assert.That(fakeUnit.name, Is.EqualTo("Test Trailer"));
            Assert.That(fakeUnit.displayName, Is.EqualTo(fakeUnit.name));

            var cmd = new SaveItemCommand();
            Assert.NotNull(cmd, nameof(SaveItemCommand));

            // description and unitCode are both null, so can't save yet
            Assert.IsFalse(cmd.CanExecute(fakeUnit));

            fakeUnit.description = "My Description";
            Assert.IsFalse(cmd.CanExecute(fakeUnit));

            fakeUnit.unitCode = "T";
            Assert.IsTrue(cmd.CanExecute(fakeUnit));

            fakeUnit.AcceptChanges();
            Assert.IsFalse(cmd.CanExecute(fakeUnit));
        }

        [Test]
        public void SaveItemCommand_DoSave()
        {
            var fakeBattery = new BatteryType
            {
                name = "Test Battery"
            };
            Assert.That(fakeBattery.IsChanged, nameof(fakeBattery.IsChanged), Is.True);
            Assert.That(fakeBattery.name, Is.EqualTo("Test Battery"));
            //Assert.That(fakeBattery.displayName, Is.EqualTo(fakeBattery.name));

            var cmd = new SaveItemCommand();
            Assert.NotNull(cmd, nameof(SaveItemCommand));

            Assert.IsTrue(cmd.CanExecute(fakeBattery));

            cmd.Execute(fakeBattery);
            Assert.IsFalse(cmd.CanExecute(fakeBattery));

            var db = DataRepository.GetDataRepository;
            var fakeBatteryReplica = db.Load<BatteryType>(fakeBattery.PrimaryKey);
            Assert.NotNull(fakeBatteryReplica, nameof(fakeBatteryReplica));

            db.Delete(fakeBattery);
        }

        [Test]
        public void SaveItemCommand_DoSave_ManyToMany()
        {
            var fakeUnit = new EquipmentUnitType
            {
                name = "Test Trailer",
                description = "My Description",
                unitCode = "T"
            };
            Assert.That(fakeUnit.IsChanged, nameof(fakeUnit.IsChanged), Is.True);
            Assert.That(fakeUnit.name, Is.EqualTo("Test Trailer"));
            Assert.That(fakeUnit.displayName, Is.EqualTo(fakeUnit.name));

            var cmd = new SaveItemCommand();
            Assert.NotNull(cmd, nameof(SaveItemCommand));

            fakeUnit.unitCode = "T";
            Assert.IsTrue(cmd.CanExecute(fakeUnit));

            var fakeSite = new SiteLocation
            {
                name = "My Site",
                locSuffix = "My"
            };
            Assert.That(fakeSite.IsChanged, nameof(fakeSite.IsChanged), Is.True);
            Assert.That(fakeSite.name, Is.EqualTo("My Site"));

            fakeSite.equipmentUnitTypesAvailable.Add(fakeUnit);
            Assert.IsTrue(cmd.CanExecute(fakeSite));

            cmd.Execute(fakeSite);
            Assert.IsFalse(cmd.CanExecute(fakeSite));
            Assert.IsFalse(cmd.CanExecute(fakeUnit));

            var db = DataRepository.GetDataRepository;
            var fakeSiteReplica = db.Load<SiteLocation>(fakeSite.PrimaryKey);
            Assert.NotNull(fakeSiteReplica, nameof(fakeSiteReplica));
            Assert.That(fakeSiteReplica.equipmentUnitTypesAvailable.Count, Is.GreaterThan(0));

            // note that must delete site first to remove mappings otherwise will fail due to mapping fk constraints
            db.Delete(fakeSite);
            db.Delete(fakeUnit);
        }

        [Test]
        public void LoadItemCommand_DoLoad()
        {
            var searchResult = new SearchResult()
            {
                id = new Guid("584da062-0a37-4a6d-9d8b-202e202e202e"),
                entityType = "BatteryType",
            };

            // transparently load entity
            var entity = searchResult.entity;
            Assert.NotNull(entity);

            searchResult.id = Guid.Empty;
            //Assert.Throws(typeof(ArgumentOutOfRangeException), () => entity = searchResult.entity);
            Assert.IsNull(searchResult.entity);

            searchResult.id = new Guid("584da062-0a37-4a6d-9d8b-202e202e202e");
            entity = searchResult.entity;
            Assert.NotNull(entity);

            searchResult.entityType = null;
            //Assert.Throws(typeof(ArgumentOutOfRangeException), () => entity = searchResult.entity);
            Assert.IsNull(searchResult.entity);

            searchResult.entityType = "BatteryType";
            BatteryType noBattery = searchResult.entity as BatteryType;
            Assert.NotNull(noBattery);
            Assert.AreEqual(noBattery.name, "None");
            Assert.AreEqual(noBattery.id, searchResult.id);
        }

        [Test]
        public void DeployRecoverItemCommand()
        {
            var deployCmd = new DeployItemCommand() { Notes = "Unit Test" };
            var recoverCmd = new RecoverItemCommand();

            var db = DataRepository.GetDataRepository;
            var user = db.Load<UserDetail>("steph");
            var userManager = new UserManager(db);
            UserManager.GetUserManager.LoginUser(user);

            var item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(item);
            Assert.That(DeployRecoverItemCommandBase.statusAvailable.id, Is.EqualTo(item.statusId));
            Assert.That(DeployItemCommand.statusAvailable, Is.EqualTo(item.status));

            Assert.IsTrue(deployCmd.CanExecute(item));
            Assert.IsFalse(recoverCmd.CanExecute(item));

            deployCmd.Execute(item);

            Assert.IsFalse(deployCmd.CanExecute(item));
            Assert.IsTrue(recoverCmd.CanExecute(item));

            var deployEvent = db.GetLatestDeploymentEventFor(item.id);
            Assert.NotNull(deployEvent);
            Assert.NotNull(deployEvent.deployBy);
            Assert.That(deployEvent.deployBy, Is.EqualTo(user.userId));
            Assert.NotNull(deployEvent.deployDate);
            Assert.IsNull(deployEvent.recoverDate);
            Assert.IsNull(deployEvent.recoverBy);

            recoverCmd.Execute(item);

            Assert.IsTrue(deployCmd.CanExecute(item));
            Assert.IsFalse(recoverCmd.CanExecute(item));

            deployEvent = db.GetLatestDeploymentEventFor(item.id);
            Assert.NotNull(deployEvent);
            Assert.NotNull(deployEvent.deployBy);
            Assert.That(deployEvent.deployBy, Is.EqualTo(user.userId));
            Assert.NotNull(deployEvent.deployDate);
            Assert.NotNull(deployEvent.recoverDate);
            Assert.NotNull(deployEvent.recoverBy);
            Assert.That(deployEvent.recoverBy, Is.EqualTo(user.userId));
        }

        [Test]
        public void ReturnItemCommand()
        {
            var cmd = new ReturnToInventoryCommand();

            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var available = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Available");
            var missing = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Missing");

            var item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(item);

            Assert.That(item.status, Is.EqualTo(available));
            Assert.IsFalse(cmd.CanExecute(item));

            item.status = missing;
            Assert.IsTrue(cmd.CanExecute(item));
            cmd.Execute(item);
            Assert.That(item.status, Is.EqualTo(available));
        }

        [Test]
        public void AddItemCommand()
        {
            var cmd = new SaveItemCommand();

            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);

            var testInstance = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(testInstance);
            Assert.IsFalse(cmd.CanExecute(testInstance)); // not an Item, can't add ItemInstance directly
            Assert.IsFalse(cmd.CanExecute(testInstance.item)); // because it already exists
            Assert.IsFalse(cmd.CanExecute(testInstance.item.itemType)); // an ItemType, reference data

            var item = db.Load<Item>("f69eb75a-3468-2f78-6b54-2530f95e75e0");
            Assert.NotNull(item);
            Assert.IsFalse(cmd.CanExecute(item));

            // mutate as new item so we can add it
            item.id = Guid.Empty;
            item.itemId = 99999; // way too large to be valid, ideally we'd ask for proper value
            Assert.IsTrue(cmd.CanExecute(item));

            // add the item instances (and item is not already saved)
            cmd.Execute(item);
            var itemInstances = db.db.LoadRows<ItemInstance>("WHERE itemId=?", item.id.ToString());
            Assert.That(itemInstances.Count, Is.EqualTo(13));
            // cleanup, remove the item instances
            foreach (var itemInstance in itemInstances)
            {
                db.Delete(itemInstance);
            }
            db.Delete(item);
        }

        [Test]
        public void DamagedMissingItemCommand()
        {
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var user = db.Load<UserDetail>("steph");
            var userManager = new UserManager(db);
            UserManager.GetUserManager.LoginUser(user);

            // construction expects a valid user
            Assert.NotNull(UserManager.GetUserManager.CurrentUser());
            var damagedCmd = new DamagedMissingItemCommand(DamageMissingEventType.Damage);
            var missingCmd = new DamagedMissingItemCommand(DamageMissingEventType.Missing);

            var item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(item);
            Assert.That(item.status.name, Is.EqualTo("Available"));

            Assert.IsTrue(damagedCmd.CanExecute(item));
            Assert.IsTrue(missingCmd.CanExecute(item));

            damagedCmd.Execute(item);
            Assert.IsFalse(damagedCmd.CanExecute(item));
            Assert.IsTrue(missingCmd.CanExecute(item));

            missingCmd.Execute(item);
            Assert.IsFalse(damagedCmd.CanExecute(item));
            Assert.IsFalse(missingCmd.CanExecute(item));

            // restore status
            var returnToInventoryCmd = new ReturnToInventoryCommand();
            returnToInventoryCmd.Execute(item);
            Assert.That(item.status.name, Is.EqualTo("Available"));
            Assert.IsTrue(damagedCmd.CanExecute(item));
            Assert.IsTrue(missingCmd.CanExecute(item));
        }

        [Test]
        public void SearchItemsCommandTest()
        {
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var user = db.Load<UserDetail>("steph");
            var userManager = new UserManager(db);
            UserManager.GetUserManager.LoginUser(user);


            var searchFilter = new SearchFilterOptions
            {
                User = UserManager.GetUserManager.CurrentUser(),
                SiteLocationEnabled = false,
                SearchFilterEnabled = true,
                SearchText = "Airway Kit",
                SelectEquipmentUnitsEnabled = false,
                SelectItemStatusValuesEnabled = false,
                SelectItemCategoryValuesEnabled = false
            };
            Assert.NotNull(searchFilter, nameof(SearchFilterOptions));
            var resultVM = new TestDetailsViewModel();
            Assert.NotNull(resultVM, nameof(TestDetailsViewModel));
            var onSelectionChangedCommand = new OnSelectionChangedCommand(resultVM);
            Assert.NotNull(onSelectionChangedCommand, nameof(OnSelectionChangedCommand));
            var searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
            Assert.NotNull(searchResultViewModel, nameof(SearchResultViewModel));
            var searchCmd = new SearchItemsCommand(QueryResultEntitySelector.ItemType, searchResultViewModel);
            Assert.NotNull(searchCmd, nameof(SearchItemsCommand));

            Assert.IsTrue(searchCmd.CanExecute(searchFilter));

            Assert.IsNull(resultVM.CurrentItem);  // nothing selected to display details of
            Assert.That(searchResultViewModel.Items.Count, Is.EqualTo(0)); // no search results or empty search results
            searchCmd.Execute(searchFilter);
            searchCmd.WaitForSearchToComplete();
            Assert.That(searchResultViewModel.Items.Count, Is.GreaterThan(0));

            searchResultViewModel.SelectedItem = searchResultViewModel.Items.First();
            Assert.NotNull(searchResultViewModel.SelectedItem);
            Assert.NotNull(resultVM.CurrentItem);

            Assert.AreNotEqual(Guid.Empty, resultVM.CurrentItem.id);
            Assert.NotNull(resultVM.CurrentItem.entityType);
            Assert.NotNull(resultVM.CurrentItem.entity);
        }

        [Test]
        public void ServiceItemCommand()
        {
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var user = db.Load<UserDetail>("steph");
            var userManager = new UserManager(db);
            UserManager.GetUserManager.LoginUser(user);

            var cmd = new ServiceItemCommand();
            Assert.NotNull(cmd, nameof(ServiceItemCommand));

            var item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(item);
            Assert.That(item.status.name, Is.EqualTo("Available"));

            var serviceType = new ItemService()
            {
                name = "Test service",
                itemInstance = item,
                category = db.ReferenceData[nameof(ServiceCategory)].ByName<ServiceCategory>("Clean"),
                reoccurring = true,
                lengthTilNextService = 2,
                serviceFrequency = ServiceFrequency.Years,
            };

            // Note: may be open ItemServiceHistory events if pending reoccurring ItemService types.
            var serviceEvent = new ItemServiceHistory
            {
                service = serviceType,
                serviceDue = DateTime.Now.Date,
                notes = "This is a test service event and is still pending."
            };

            Assert.IsTrue(cmd.CanExecute(serviceEvent));

            var searchResult = new GenericItemResult() { id = item.id, entity = item, entityType = nameof(ItemInstance) };
            var currentServiceEvents = db.GetItemServiceEvents(searchResult);

            cmd.Execute(serviceEvent);
            Assert.That(item.status.name, Is.EqualTo("Out for Service"));

            var updatedServiceEvents = db.GetItemServiceEvents(searchResult);
            Assert.That(updatedServiceEvents.Count, Is.GreaterThan(currentServiceEvents.Count));

            // restore status assuming service complete
            // note there can be a pending service event with item available, i.e. reoccurring event still pending
            var returnToInventoryCmd = new ServiceItemCompleteCommand();
            returnToInventoryCmd.Execute(serviceEvent);
            Assert.That(item.status.name, Is.EqualTo("Available"));
        }

        [Test]
        public void ReplaceItemCommand()
        {
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var user = db.Load<UserDetail>("steph");
            var userManager = new UserManager(db);
            UserManager.GetUserManager.LoginUser(user);

            var cmd = new ReplaceItemCommand();
            Assert.NotNull(cmd, nameof(ReplaceItemCommand));

            var item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.NotNull(item);
            // force known value
            item.status = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Available");
            db.Save(item);
            item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.That(item.status.name, Is.EqualTo("Available"));

            Assert.IsTrue(cmd.CanExecute(item));
            cmd.Execute(item);
            Assert.That(item.status.name, Is.EqualTo("Out for Service"));

            // verify a new item created
            var searchFilter = new SearchFilterOptions
            {
                User = UserManager.GetUserManager.CurrentUser(),
                SiteLocationEnabled = false,
                SearchFilterEnabled = true,
                SearchText = SampleItemNumber,
                ItemTypeMatching = SearchFilterItemMatching.OnlyExact,
                //AllowItemsRemovedFromService = false, // required or query below may return old item instead of new one
                SelectEquipmentUnitsEnabled = false,
                SelectItemStatusValuesEnabled = false,
                SelectItemCategoryValuesEnabled = false
            };
            Assert.NotNull(searchFilter, nameof(SearchFilterOptions));
            var resultVM = new TestDetailsViewModel();
            Assert.NotNull(resultVM, nameof(TestDetailsViewModel));
            var onSelectionChangedCommand = new OnSelectionChangedCommand(resultVM);
            Assert.NotNull(onSelectionChangedCommand, nameof(OnSelectionChangedCommand));
            var searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
            Assert.NotNull(searchResultViewModel, nameof(SearchResultViewModel));
            var searchCmd = new SearchItemsCommand(QueryResultEntitySelector.ItemType, searchResultViewModel);
            Assert.NotNull(searchCmd, nameof(SearchItemsCommand));
            //var searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
            //Assert.NotNull(searchFilterOptionsViewModel);

            Assert.IsTrue(searchCmd.CanExecute(searchFilter));

            Assert.IsNull(resultVM.CurrentItem);  // nothing selected to display details of
            Assert.That(searchResultViewModel.Items.Count, Is.EqualTo(0)); // no search results or empty search results
            searchCmd.Execute(searchFilter);
            searchCmd.WaitForSearchToComplete();
            Assert.That(searchResultViewModel.Items.Count, Is.GreaterThan(0));

            searchResultViewModel.SelectedItem = searchResultViewModel.Items.First();
            Assert.NotNull(searchResultViewModel.SelectedItem);
            Assert.NotNull(resultVM.CurrentItem);
            Assert.That(resultVM.CurrentItem.id, Is.Not.EqualTo(item.id));

            // delete new item and restore old item to Available
            var newItem = db.Load<ItemInstance>(resultVM.CurrentItem.id);
            Assert.That(newItem.status.name, Is.EqualTo("Available"));
            item.status = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Available");
            item.removedServiceDate = null;
            db.Delete(newItem);
            Assert.IsFalse(db.Exists(newItem));
            db.Save(item);
            item = db.Load<ItemInstance>(SampleItemInstance);
            Assert.That(item.status.name, Is.EqualTo("Available"));
        }
    }
}