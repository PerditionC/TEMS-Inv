// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;

namespace Tems_Inventory.Tests
{
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    using TEMS.InventoryModel.entity.db;

    [TestFixture]
    public class InventoryModelTests
    {
        /// <summary>
        /// loads SQLite and creates and populates our test DB
        /// </summary>
        [SetUp]
        public void setupTest()
        {
            #region add support for native DLLs under both Win32 and Win64

            try
            {
                SQLite.SQLiteLoader.SetNativeDllDirectory();
            }
            catch
            {
                Assert.Fail("Critical error initializing dynamic libraries!  ABORTING!");
            }

            #endregion add support for native DLLs under both Win32 and Win64

            // set current directory to expected value, i.e. directory of binaries, so relative paths for databases work as expected
            // Note: otherwise likely to fail as default current directory may be within protected dir, e.g. Visual Studio install directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())?.Location));
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
        }

        [Test]
        public void TestDataModel()
        {
            // WARNING! test.db must be setup already with proper schema
            using (var dataRepo = new DataRepository(@"C:\DB\test.db"))
            {
                var db = dataRepo.db;
                Assert.NotNull(db);

                var uoms = db.LoadRows<UnitOfMeasure>(null);
                Assert.IsTrue(uoms.Count > 0);
                Assert.IsTrue(uoms.Count == 4);

                var vehicles = db.LoadRows<VehicleLocation>(null);
                Assert.IsTrue(vehicles.Count > 0);
                Assert.IsTrue(vehicles.Count == 6);

                var units = db.LoadRows<EquipmentUnitType>(null);
                Assert.IsTrue(units.Count > 0);
                Assert.IsTrue(units.Count >= 3);  // may be more if testing doesn't clean up properly
                foreach (var unit in units)
                {
                    Assert.That(unit.name, Is.Not.Empty);
                    Assert.That(unit.unitCode, Is.Not.Empty);
                    Assert.That(unit.unitCode.Length, Is.EqualTo(1));
                    Assert.That(unit.description, Is.Not.Null);
                }

                var siteLocations = db.LoadRows<SiteLocation>(null);
                Assert.IsTrue(siteLocations.Count > 0);
                Assert.IsTrue(siteLocations.Count >= 16);  // may be more if testing doesn't clean up properly
                foreach (var siteLocation in siteLocations)
                {
                    Assert.That(siteLocation.name, Is.Not.Empty);
                    Assert.That(siteLocation.locSuffix, Is.Not.Empty);
                }

                var vendors = db.LoadRows<VendorDetail>(null);  // this should load vendors, vendorSiteAccounts, and SiteLocations
                Assert.IsTrue(vendors.Count > 0);
                Assert.IsTrue(vendors.Count == 33);
                foreach (var v in vendors)
                {
                    Assert.That(v.name, Is.Not.Empty);
                }

                //var itemTypes = db.LoadRows<ItemType>();
                var itemTypes = dataRepo.ReferenceData[nameof(ItemType)];
                //Assert.IsTrue(itemTypes.Count > 0); // may not have any data in table yet
                foreach (var it in itemTypes)
                {
                    //Console.WriteLine("ItemType: " + serializer.Serialize(it));
                    //Console.WriteLine($"ItemType: id:{it.id}, itemId:{it.itemId}, uom:{it.unitOfMeasure?.id}");
                }
                var i1 = itemTypes.FirstOrDefault();
                if (i1 != null)
                {
                    var itemTypeX = db.Load<ItemType>(i1.PrimaryKey);
                    Assert.That(i1, Is.EqualTo(itemTypeX));
                    Assert.IsTrue(dataRepo.Exists(i1));
                    Assert.IsFalse(i1.IsChanged);
                    var i2 = i1.GetClonedItem();
                    Assert.IsTrue(dataRepo.Exists(i2));
                    Assert.IsFalse(i2.IsChanged);
                    i2 = new ItemInstance();
                    Assert.IsFalse(dataRepo.Exists(i2));
                    Assert.IsTrue(i2.IsChanged);

                    //var itemx = db.Load<Item>(new Guid("3612e75e-1642-4027-bf8c-4f28777e47ca"));
                    /*
                    var items = db.LoadRows<Item>();
                    int ndx = 3;
                    foreach (var item in items)
                    {
                        Console.WriteLine("Item: " + serializer.Serialize(item));
                        ndx--;
                        if (ndx < 1) break;
                    }
                    var items = db.Query<Item>(null, "WHERE hasBarcode = ?", 1);
                    int ndx = 3;
                    foreach (var item in items)
                    {
                        ndx--;
                        if (ndx < 1) break;
                    }
                    */

                    var U1 = uoms.First(x => string.Equals(x.name, "Each", StringComparison.InvariantCultureIgnoreCase));
                    var U2 = itemTypeX.unitOfMeasure;
                    Assert.That(U1, Is.EqualTo(U2));

                    var services = db.LoadRows<ItemService>("WHERE itemInstanceId = ?", new Guid());  // this should return 0 items since we create a random new Guid
                    Assert.IsTrue(services.Count == 0);
                }
            }
        }
    }
}