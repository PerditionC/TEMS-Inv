// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Diagnostics;
using System.Linq;

namespace Tems_Inventory.Tests
{
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    using TEMS.InventoryModel.entity;
    using TEMS.InventoryModel.entity.db;
    using TEMS.InventoryModel.entity.db.user;
    using TEMS.InventoryModel.userManager;
    using TEMS.InventoryModel.userManager.extension;

    [TestFixture]
    public sealed class UserManagerTests
    {
        private const string TESTUSERID = "MyTestUserId";
        private const string UNKNOWN_NAME = "<unnamed>";

        private DataRepository DataRepositoryRef = null;
        private IUserManager userManager = null;

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
            userManager = new UserManager(DataRepositoryRef);
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
            userManager = null;
            DataRepositoryRef.Dispose();
            DataRepositoryRef = null;
            System.GC.WaitForPendingFinalizers();
        }

        [Test]
        public void UserManagerTest01_CreateUser()
        {
            UserDetail user = userManager.CreateUser(TESTUSERID, "MyPwHash");
            Assert.NotNull(user);
            Assert.That(user.userId, Is.EqualTo(TESTUSERID.Trim().ToLowerInvariant()));
            Assert.That(user.displayName, Is.EqualTo(TESTUSERID.Trim().ToLowerInvariant()));
            Assert.That(user.hashedPassphrase, Is.EqualTo("MyPwHash"));
            Assert.That(user.isPasswordExpired, nameof(user.isPasswordExpired), Is.True);
            Assert.IsFalse(user.isActive);
            Assert.IsFalse(user.isAdmin);
            Assert.That(user.role, Is.EqualTo(UserDetail.USER));
            Assert.IsNull(user.firstName);
            Assert.IsNull(user.lastName);
            Assert.IsNull(user.email);
            Assert.NotNull(user.availableSites);
            Assert.IsEmpty(user.availableSites);
            Assert.IsNull(user.currentSite);

            Assert.That(user.IsChanged, nameof(user.IsChanged), Is.True);
            user.AcceptChanges();
            Assert.IsFalse(user.IsChanged);
        }

        [Test]
        public void UserManagerTest02_CreateUser()
        {
            UserDetail user = userManager.CreateUser(userId: null, hashedPassphrase: null);
            Assert.NotNull(user);
            Assert.IsNull(user.userId);
            Assert.That(user.displayName, Is.EqualTo(UNKNOWN_NAME.Trim().ToLowerInvariant()));
            Assert.IsNull(user.hashedPassphrase);
            Assert.IsTrue(user.isPasswordExpired, nameof(user.isPasswordExpired));
            Assert.IsFalse(user.isActive, nameof(user.isActive));
            Assert.IsFalse(user.isAdmin, nameof(user.isAdmin));
            Assert.That(user.role, Is.EqualTo(UserDetail.USER));
            Assert.IsNull(user.firstName);
            Assert.IsNull(user.lastName);
            Assert.IsNull(user.email);
            Assert.NotNull(user.availableSites);
            Assert.IsEmpty(user.availableSites);
            Assert.IsNull(user.currentSite);

            Assert.IsTrue(user.IsChanged, nameof(user.IsChanged));
            user.AcceptChanges();
            Assert.IsFalse(user.IsChanged, nameof(user.IsChanged));
        }

        [Test]
        public void UserManagerTest03_CloneUser()
        {
            var oldUser = userManager.CreateUser(TESTUSERID, "MyPwHash");
            var user = userManager.CloneUser("NewUser", oldUser);

            Assert.NotNull(user);
            Assert.That(user.userId, Is.EqualTo("NewUser"));
            Assert.That(user.displayName, Is.EqualTo("NewUser"));
            Assert.IsNull(user.hashedPassphrase, nameof(user.hashedPassphrase));
            Assert.IsTrue(user.isPasswordExpired, nameof(user.isPasswordExpired));
            Assert.IsFalse(user.isActive, nameof(user.isActive));
            Assert.IsFalse(user.isAdmin, nameof(user.isAdmin));
            Assert.That(user.role, Is.EqualTo(UserDetail.USER));
            Assert.IsNull(user.firstName);
            Assert.IsNull(user.lastName);
            Assert.IsNull(user.email);
            Assert.NotNull(user.availableSites);
            Assert.IsEmpty(user.availableSites);
            Assert.IsNull(user.currentSite);

            Assert.IsTrue(user.IsChanged, nameof(user.IsChanged));
            user.AcceptChanges();
            Assert.IsFalse(user.IsChanged, nameof(user.IsChanged));

            user.hashedPassphrase = "MyPwHash";
            user.isActive = true;
            Assert.That(user.hashedPassphrase, Is.EqualTo("MyPwHash"));
            Assert.IsTrue(user.isActive, nameof(user.isActive));
        }

        [Test]
        public void UserManagerTest04_LoadUser()
        {
            Stopwatch sw = new Stopwatch();
            UserDetail user = null;

            // reference, we don't want to penalize uncached with cost to force uncache usage
            Debug.Print("Calculating overhead");
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                DataRepository.GetDataRepository.ReferenceData.RefreshData("SiteLocation");
            }
            sw.Stop();
            var overhead = sw.ElapsedMilliseconds;

            Debug.Print("Running uncached version - well load & clear cache");
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                user = DataRepository.GetDataRepository.Load<UserDetail>("steph");
                DataRepository.GetDataRepository.ReferenceData.RefreshData("SiteLocation");
            }
            sw.Stop();
            var elapsed = (sw.ElapsedMilliseconds - overhead) / 10;

            Debug.Print("Running with loads cached");
            //DataRepository.GetDataRepository.Load<UserDetail>("steph"); // preload cache
            sw.Restart();
            for (int i = 0; i < 10; i++)
            {
                user = DataRepository.GetDataRepository.Load<UserDetail>("steph");
            }
            sw.Stop();
            var elapsed2 = sw.ElapsedMilliseconds / 10;
            System.Diagnostics.Debug.Print($"Using cached data = {elapsed2} ms vs uncached data = {elapsed} ms");
            Assert.LessOrEqual(elapsed2, elapsed);

            Assert.NotNull(user);
            Assert.That(user.userId, Is.EqualTo("steph"));
            Assert.That(user.siteId, Is.EqualTo(new Guid("27d62928-2263-4cda-83cf-77031cea1d68")));
            Assert.NotNull(user.currentSite);
            Assert.That(user.currentSite.name, Is.EqualTo("Norfolk"));
            Assert.IsTrue(user.availableSites.Count > 0);
        }

        [Test]
        public void UserManagerTest05_Create_Update_Validate_And_DeleteUser()
        {
            // create a new user and save to DB
            var user = userManager.CreateUser(TESTUSERID, PasswordHashing.EncodePassword("testHash".ToSecureString(), null));
            user.isActive = true;  // ensure we activate account so we can retrieve it later
            user.isPasswordExpired = false;  // and don't want to be flagged as needing pw change
            Assert.Throws<SavedFailedException>(() => userManager.SaveUser(user));
            user.currentSite = DataRepository.GetDataRepository.ReferenceData["SiteLocation"].FirstOrDefault(x => ((SiteLocation)x).name == "Norfolk") as SiteLocation; // avoid FK constraint failure
            Assert.DoesNotThrow(() => userManager.SaveUser(user));

            // see if we can validate user with wrong passphrase
            Assert.IsFalse(userManager.ValidateUser(TESTUSERID, null, out user), "1" + nameof(userManager.ValidateUser));
            Assert.IsNull(user);
            Assert.IsFalse(userManager.ValidateUser(TESTUSERID, "BADHash".ToSecureString(), out user), "2" + nameof(userManager.ValidateUser));
            Assert.IsNull(user);

            // and again but with correct passphrase
            Assert.IsTrue(userManager.ValidateUser(TESTUSERID, "testHash".ToSecureString(), out user), "3" + nameof(userManager.ValidateUser));
            Assert.NotNull(user);

            userManager.RemoveUser(user);
            // verify actually removed
            Assert.IsFalse(userManager.ValidateUser(TESTUSERID, "testHash".ToSecureString(), out user), "4" + nameof(userManager.ValidateUser));
            Assert.IsNull(user);
        }
    }
}