// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace Tems_Inventory.Tests
{
    using NUnit.Framework;

    using TEMS.InventoryModel.util;

    [TestFixture]
    public class ItemNumberParserTests
    {
        private ItemNumberParser parser = new ItemNumberParser();

        [Test]
        public void TestNullItemNumber()
        {
            string itemNumber = null;
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestNotItemNumber()
        {
            string itemNumber = "D-12345-ABC";
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestItemNumberBadSeparator()
        {
            string itemNumber = "D123.12345.ABC";
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestDoubleDash()
        {
            string itemNumber = "D--12345-ABC";
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestNoNumbers()
        {
            string itemNumber = "D--ABC";
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestLongStringWithEmbeddedItemNumber()
        {
            string itemNumber = "This is a long string that with D123-12345-ABC in it which is a valid # but this isn't.";
            Assert.IsFalse(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.Empty);
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestFullItemNumber()
        {
            string itemNumber = "D123-12345-ABC";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.EqualTo("D"));
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.EqualTo("12345"));
            Assert.That(parser.siteCode, Is.EqualTo("ABC"));
        }

        [Test]
        public void TestItemTypeOnly()
        {
            string itemNumber = "123";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.Empty);
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestItemTypeAndItemId()
        {
            string itemNumber = "123-12345";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.EqualTo("12345"));
            Assert.That(parser.siteCode, Is.Empty);
        }

        [Test]
        public void TestMissingOnlyEquip()
        {
            string itemNumber = "123-12345-ABC";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.EqualTo("12345"));
            Assert.That(parser.siteCode, Is.EqualTo("ABC"));
        }

        [Test]
        public void TestMissingOnlyEquipNoSecondDash()
        {
            string itemNumber = "123-12345ABC";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.Empty);
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.EqualTo("12345"));
            Assert.That(parser.siteCode, Is.EqualTo("ABC"));
        }

        [Test]
        public void TestMissingOnlySite()
        {
            string itemNumber = "D123-12345";
            Assert.IsTrue(parser.IsItemNumber(itemNumber));
            Assert.That(parser.equipCode, Is.EqualTo("D"));
            Assert.That(parser.itemTypeId, Is.EqualTo("123"));
            Assert.That(parser.itemId, Is.EqualTo("12345"));
            Assert.That(parser.siteCode, Is.Empty);
        }
    }
}