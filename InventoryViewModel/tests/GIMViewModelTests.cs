﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;

namespace Tems_Inventory.InventoryViewModel.Tests
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    using TEMS_Inventory.views;
    using TEMS.InventoryModel.command.action;
    using TEMS.InventoryModel.entity.db;
    using TEMS.InventoryModel.entity.db.query;
    using TEMS.InventoryModel.entity.db.user;
    using TEMS.InventoryModel.userManager;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    [TestFixture]
    public sealed class GIMViewModelTests
    {
        private DataRepository DataRepositoryRef = null;

        private string SampleItemNumber = "D600-1CFD";

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
            Assert.NotNull(DataRepositoryRef);
            var db = DataRepository.GetDataRepository;
            Assert.NotNull(db);
            var user = db.Load<UserDetail>("steph");
            Assert.NotNull(user);
            var userManager = new UserManager(db);
            Assert.NotNull(userManager);
            UserManager.GetUserManager.LoginUser(user);
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
            DataRepositoryRef?.Dispose();
            DataRepositoryRef = null;
            System.GC.WaitForPendingFinalizers();
        }

        [Test]
        public void GeneralInventoryManagementViewModel01_Basic()
        {
            var searchFilter = new SearchFilterOptions();
            searchFilter.Initialize();
            Assert.NotNull(searchFilter, nameof(SearchFilterOptions));
            var vm = new GeneralInventoryManagementViewModel();
            Assert.NotNull(vm, nameof(GeneralInventoryManagementViewModel));
            var onSelectionChangedCommand = new UpdateDetailsGeneralInventoryManagementCommand(vm);
            Assert.NotNull(onSelectionChangedCommand, nameof(OnSelectionChangedCommand));
            var searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
            Assert.NotNull(searchResultViewModel, nameof(SearchResultViewModel));
            var searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
            Assert.NotNull(searchFilterOptionsViewModel);

            searchFilter.SearchFilterEnabled = true;
            searchFilterOptionsViewModel.SearchTextCommand.Execute(null);
            searchFilterOptionsViewModel.SearchFilterCommand.WaitForSearchToComplete();
            Assert.NotNull(searchResultViewModel.Items);
            Assert.GreaterOrEqual(searchResultViewModel.Items.Count, 1);

            searchFilter.SearchText = SampleItemNumber;
            searchFilterOptionsViewModel.SearchTextCommand.Execute(null);
            Assert.NotNull(searchResultViewModel.Items);
            Assert.AreEqual(searchResultViewModel.Items.Count, 1);
        }

        private SearchDetailWindowViewModel GetSearchDetailWindowViewModelForGIM()
        {
            var searchFilter = new SearchFilterOptions();
            searchFilter.Initialize();
            Assert.NotNull(searchFilter, nameof(SearchFilterOptions));

            var vm = new GeneralInventoryManagementViewModel();
            Assert.NotNull(vm, nameof(GeneralInventoryManagementViewModel));

            var onSelectionChangedCommand = new UpdateDetailsGeneralInventoryManagementCommand(vm);
            Assert.NotNull(onSelectionChangedCommand, nameof(OnSelectionChangedCommand));

            var searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
            Assert.NotNull(searchResultViewModel, nameof(SearchResultViewModel));

            var searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
            Assert.NotNull(searchFilterOptionsViewModel);

            var GIM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, vm);
            return GIM;
        }

        [Test]
        public void GeneralInventoryManagementViewModel02_Save()
        {
            var searchWin = GetSearchDetailWindowViewModelForGIM();

            searchWin.SearchFilterOptions.SearchFilter.SearchText = SampleItemNumber;
            searchWin.SearchFilterOptions.SearchFilter.SearchFilterEnabled = true;
            searchWin.SearchFilterOptions.SearchTextCommand.Execute(null);
            searchWin.SearchFilterOptions.SearchFilterCommand.WaitForSearchToComplete();
            Assert.NotNull(searchWin.SearchResult.Items);
            Assert.AreEqual(searchWin.SearchResult.Items.Count, 1);


            Assert.NotNull(searchWin.SearchResult.SelectedItem);
            Assert.NotNull(searchWin.Details.CurrentItem);
            /* TODO implement save command, it should map vm to ItemInstance and call DB.save; also add properties to vm for mapping
            Assert.False(vm.SaveCommand.CanExecute(null), "Can Save");  // nothing changed so no changes to save

            if (vm.notes == null)
                vm.notes = "Testing";
            else
                vm.notes = null;
            Assert.True(vm.SaveCommand.CanExecute(null), "Can Save");
            vm.SaveCommand.Execute(null);
            */
        }

        /*
        [Test]
        public void GeneralInventoryManagementViewModel03_ChangeSelectedAndCurrentItem()
        {
            var vm = new GeneralInventoryManagementViewModel();
            Assert.NotNull(vm);
            Assert.NotNull(vm.SearchFilter);
            vm.SearchFilter.SearchText = SampleItemNumber;
            vm.SearchFilter.SearchFilterEnabled = true;
            Assert.NotNull(vm.Items);
            Assert.AreEqual(vm.Items.Count, 1);

            Assert.NotNull(vm.SelectedItem);
            Assert.NotNull(vm.CurrentItem);
            Assert.False(vm.SaveCommand.CanExecute(null), "Can Save");  // nothing changed so no changes to save

            var item = vm.CurrentItem as ItemInstance;
            Assert.NotNull(item);

            vm.SelectedItem = null;
            Assert.IsNull(vm.SelectedItem);
            Assert.IsNull(vm.CurrentItem);

            // set to item we know is in Items
            vm.CurrentItem = item;
            Assert.NotNull(vm.CurrentItem);
            Assert.NotNull(vm.SelectedItem);

            // set to item we know is not in Items, but ensure no item is selected first to avoid check to replace changed item
            vm.SelectedItem = null;
            Assert.IsNull(vm.SelectedItem);
            var newItem = item.GetClonedItem() as ItemInstance;
            newItem.id = Guid.NewGuid();
            newItem.AcceptChanges();
            Assert.NotNull(newItem);
            Assert.Throws(typeof(NotImplementedException), () => { vm.CurrentItem = newItem; });

            // now force check to replace currently changed selection
            Mediator.Register(nameof(YesNoDialogMessage), (o) => { System.Diagnostics.Debug.Print($"YesNoDialog called so allow chance to save current item before it is changed."); });
            vm.CurrentItem = item;
            Assert.NotNull(vm.SelectedItem);
            item.notes = "1";
            Assert.Throws(typeof(NotImplementedException), () => { vm.CurrentItem = newItem; });
        }


        [Test]
        public void GeneralInventoryManagementViewModel04_OpenEditItemWindowCommand()
        {
            var vm = new GeneralInventoryManagementViewModel();
            Assert.NotNull(vm);
            Assert.NotNull(vm.SearchFilter);
            vm.SearchFilter.SearchText = SampleItemNumber;
            vm.SearchFilter.SearchFilterEnabled = true;
            Assert.NotNull(vm.Items);
            Assert.AreEqual(vm.Items.Count, 1);

            Assert.NotNull(vm.SelectedItem);
            Assert.NotNull(vm.CurrentItem);

            var item = vm.CurrentItem as ItemInstance;
            Assert.NotNull(item);

            // attempt to edit item
            Mediator.Register(nameof(ShowWindowMessage), (o) => { System.Diagnostics.Debug.Print($"Opening a new window - {o.ToString()}"); });
            Assert.True(vm.IsAdmin);
            Assert.True(vm.OpenEditItemWindowCommand.CanExecute(null));
            vm.OpenEditItemWindowCommand.Execute(null);
        }
        */
    }
}
