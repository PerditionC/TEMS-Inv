// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace Tems_Inventory.Tests
{
    using NUnit.Framework;

    using TEMS.InventoryModel.userManager;
    using TEMS.InventoryModel.userManager.extension;

    [TestFixture]
    public class PasswordHashingTests
    {
        private const string KnownPassword = "HACK";
        private const string KnownSalt = "W4HzBdwizUNGfhTQJnPwEQ ==";
        private const string KnownHashAndSalt = "W4HzBdwizUNGfhTQJnPwEeLEqCQcEqY2c1IPAW02wV6XkpHC"; // salt + HACK

        [Test]
        public void TestExtractSalt()
        {
            byte[] expectedSalt = Convert.FromBase64String(KnownSalt);
            byte[] storedHash = Convert.FromBase64String(KnownHashAndSalt);
            byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);

            Assert.That(salt, Is.EqualTo(expectedSalt));
        }

        [Test]
        public void TestExtractBadSalt()
        {
            byte[] expectedSalt = Convert.FromBase64String(KnownSalt);
            byte[] storedHash = Convert.FromBase64String("W4HzBdwizUNGfhTQJnP=");

            Assert.That(storedHash.Length, Is.LessThan(expectedSalt.Length));

            byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);

            Assert.That(salt, Is.Not.EqualTo(expectedSalt));
        }

        [Test]
        public void TestExtractNoSaltProvided()
        {
            byte[] expectedSalt = Convert.FromBase64String(KnownSalt);
            byte[] storedHash = Convert.FromBase64String("");

            Assert.That(storedHash.Length, Is.LessThan(expectedSalt.Length));

            byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);

            Assert.That(salt, Is.Not.EqualTo(expectedSalt));
        }

        [Test]
        public void TestGenerateSalt()
        {
            byte[] expectedSalt = Convert.FromBase64String(KnownSalt);
            byte[] salt = PasswordHashing.GenerateSalt();

            Assert.That(salt, Is.Not.EqualTo(expectedSalt));
        }

        [Test]
        public void TestEncode()
        {
            byte[] storedHash = Convert.FromBase64String(KnownHashAndSalt);
            byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);
            byte[] curHash = Convert.FromBase64String(PasswordHashing.EncodePassword(KnownPassword.ToSecureString(), salt));

            Assert.That(storedHash, Is.EqualTo(curHash), "Hashed Passphrase 'HACK' should be the same");
        }

        [Test]
        public void TestHashMatches()
        {
            byte[] storedHash = Convert.FromBase64String(KnownHashAndSalt);
            byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);
            byte[] curHash = Convert.FromBase64String(PasswordHashing.EncodePassword(KnownPassword.ToSecureString(), salt));

            Assert.That(PasswordHashing.HashesMatch(storedHash, curHash), Is.True);
        }

        [Test]
        public void TestHashDoesntMatch()
        {
            byte[] storedHash = Convert.FromBase64String(KnownHashAndSalt);
            byte[] salt = PasswordHashing.GenerateSalt();
            byte[] curHash = Convert.FromBase64String(PasswordHashing.EncodePassword(KnownPassword.ToSecureString(), salt));

            Assert.That(PasswordHashing.HashesMatch(storedHash, curHash), Is.False);
        }

        [Test]
        public void TestGenerateAndCompareHashMatch()
        {
            byte[] saltAndHash = Convert.FromBase64String(PasswordHashing.EncodePassword(KnownPassword.ToSecureString(), null));
            byte[] salt = PasswordHashing.ExtractStoredSalt(saltAndHash);

            Assert.That(salt, Is.Not.Null);
            Assert.That(PasswordHashing.HashesMatch(Convert.FromBase64String(PasswordHashing.EncodePassword(KnownPassword.ToSecureString(), salt)), saltAndHash));
        }
    }
}