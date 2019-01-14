// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Linq;

namespace Tems_Inventory.Tests
{
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    using TEMS.InventoryModel.entity.db;

    [TestFixture]
    public class ReferenceDataCacheTests
    {
        private Database dbRef = null;
        private ReferenceDataCache cache;

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
            dbRef = new Database(@"C:\DB\test.db");
            cache = new ReferenceDataCache(dbRef);
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
            //cache.Dispose();
            cache = null;
            dbRef.Dispose();
            dbRef = null;
            System.GC.WaitForPendingFinalizers();
        }

        [Test]
        public void ReferenceDataCacheTest01()
        {
            Assert.NotNull(cache);
        }

        [Test]
        public void ReferenceDataCacheTest02_metadata()
        {
            var meta = ReferenceDataCache.ReferenceDataTypes;
            Assert.NotNull(meta);
            Assert.NotZero(meta.Count);
            foreach (var refDataType in meta)
            {
                Assert.NotNull(refDataType);
                Assert.NotNull(refDataType.Text);
                Assert.NotZero(refDataType.Text.Length);
                Assert.NotNull(refDataType.TypeName);
                Assert.NotZero(refDataType.TypeName.Length);
            }
        }

        [Test]
        public void ReferenceDataCacheTest03_getBatteryList()
        {
            // pk is id
            var cachedList = cache[nameof(BatteryType)];
            Assert.NotNull(cachedList, "list");
            Assert.NotZero(cachedList.Count);
            var x = cachedList.FirstOrDefault();
            Assert.NotNull(x, "first item");
            Assert.NotNull(x.displayName, "item name");
            var genRefData = x as ReferenceData;
            Assert.NotNull(genRefData, "as reference data");
            Assert.NotNull(genRefData.name, "reference data name");
        }

        [Test]
        public void ReferenceDataCacheTest04_getUnitList()
        {
            // pk is name not id
            var cachedList = cache[nameof(EquipmentUnitType)];
            Assert.NotNull(cachedList, "list");
            Assert.NotZero(cachedList.Count);
            var x = cachedList.FirstOrDefault();
            Assert.NotNull(x, "first item");
            Assert.NotNull(x.displayName, "item name");
            var equip = x as EquipmentUnitType;
            Assert.NotNull(equip, "as equip");
            Assert.NotNull(equip.name, "equip name");
        }

        [Test]
        public void ReferenceDataCacheTest05_getById()
        {
            var cachedList = cache[nameof(EquipmentUnitType)];
            Assert.NotNull(cachedList);
            //var item = cachedList.ById<ItemBase>("MMRS");
            var item = cache[nameof(EquipmentUnitType), "MMRS"];
            Assert.NotNull(item);
            Assert.IsFalse(item.IsChanged, "IsChanged");
            //item = cachedList.ById<ItemBase>(null);
            item = cache[nameof(EquipmentUnitType), null];
            Assert.IsNull(item);
            //item = cachedList.ById<ItemBase>("BadId");
            item = cache[nameof(EquipmentUnitType), "BadId"];
            Assert.IsNull(item);
            //item = cachedList.ById<ItemBase>(840); // BAD
            item = cache[nameof(EquipmentUnitType), 840];
            Assert.IsNull(item);
        }

        [Test]
        public void ReferenceDataCacheTest06_refresh()
        {
            var cachedList = cache[nameof(EquipmentUnitType)];
            Assert.NotNull(cachedList);
            cache.RefreshData(nameof(EquipmentUnitType));
            cachedList = cache[nameof(EquipmentUnitType)];
            Assert.NotNull(cachedList);
            Assert.DoesNotThrow(() => cache.RefreshData("BadItem"));
            Assert.DoesNotThrow(() => cache.RefreshData(null));
        }
    }
}