// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// Simple class to implement basics for password hashing

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace TEMS.InventoryModel.userManager
{
    public static class PasswordHashing
    {
        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 20;
        private const int HASH_ITERATIONS = 1000;

        /// <summary>
        /// Generate salt value for password hashing
        /// </summary>
        /// <returns>a byte array to use as salt value</returns>
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SALT_SIZE];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Wraps Rfc2898DeriveBytes for obtaining hashed passphrase to allow use of a SecureString.
        /// i.e. byte[] hash = Rfc2898DeriveBytes(passphrase, salt, iterations).GetBytes(keyByteLenth);
        /// See https://stackoverflow.com/questions/9734043/rfc2898derivebytes-pbkdf2-securestring-is-it-possible-to-use-a-secure-string
        /// </summary>
        /// <param name="passphrase"></param>
        /// <param name="salt"></param>
        /// <param name="iterations"></param>
        /// <param name="keyByteLength"></param>
        /// <returns></returns>
        private static byte[] DeriveKey(SecureString passphrase, byte[] salt, int iterations, int keyByteLength)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(passphrase);
            byte[] passwordByteArray = null;
            try
            {
                int length = Marshal.ReadInt32(ptr, -4);
                passwordByteArray = new byte[length];
                GCHandle handle = GCHandle.Alloc(passwordByteArray, GCHandleType.Pinned);
                try
                {
                    for (int i = 0; i < length; i++)
                    {
                        passwordByteArray[i] = Marshal.ReadByte(ptr, i);
                    }

                    using (var rfc2898 = new Rfc2898DeriveBytes(passwordByteArray, salt, iterations))
                    {
                        return rfc2898.GetBytes(keyByteLength);
                    }
                }
                finally
                {
                    Array.Clear(passwordByteArray, 0, passwordByteArray.Length);
                    handle.Free();
                }
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }

        /// <summary>
        /// Given a password and salt returnes hashed (encrypted) password
        /// See http://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        /// </summary>
        /// <param name="passphrase">the password or phrase to encrypt/hash</param>
        /// <param name="salt">null to generate or stored/known salt to use; byte[SALT_SIZE]</param>
        /// <returns></returns>
        public static string EncodePassword(SecureString passphrase, byte[] salt)
        {
            // generate the salt if not provided by user
            if (salt == null) salt = GenerateSalt();

            // compute the hash value
            var hash = DeriveKey(passphrase, salt, HASH_ITERATIONS, HASH_SIZE);

            // combine (copy bytes into single buffer) and convert that to a Base64 encoded string
            byte[] combined = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

            return Convert.ToBase64String(combined);
        }

        /// <summary>
        /// Extracts the salt portion from a stored passphrase and salt
        /// that have been combined and stored together.
        /// If string is too short, returns a random salt value to force
        /// corresponding password to always fail validation - i.e. stored
        /// passphrase is invalid so should always fail validation
        /// </summary>
        /// <param name="hashedPassphraseAndSalt">byte[] with salt immediately followed
        /// by hashed passphrase.  Should be raw bytes not encoded (e.g. not base64 text).
        /// </param>
        /// <returns>the salt as a byte[]</returns>
        public static byte[] ExtractStoredSalt(byte[] hashedPassphraseAndSalt)
        {
            // split apart stored hash so we can hash attempted password & compare
            byte[] salt = new byte[SALT_SIZE];
            // if invalid password stored generate random salt [should force validation to fail]
            if (hashedPassphraseAndSalt.Length < SALT_SIZE)
                salt = GenerateSalt();
            else
                Buffer.BlockCopy(hashedPassphraseAndSalt, 0, salt, 0, SALT_SIZE);

            return salt;
        }

        /// <summary>
        /// Does a [constant time] comparison of stored and current hashed passphrase
        /// to determine if they match.
        /// </summary>
        /// <param name="hash1">hashed passphrase</param>
        /// <param name="hash2">other hashed passphrase</param>
        /// <returns></returns>
        public static bool HashesMatch(byte[] hash1, byte[] hash2)
        {
            // determine if match; always loop max length times; if not same size then not a match
            // use xor and or instead of == to avoid any time differences due to compiler using branches for ==
            bool isValid = (hash1.Length ^ hash2.Length) == 0;
            //System.Diagnostics.Trace.WriteLine($"HashesMatch() {hash1.Length}=?={hash2.Length} Max={Math.Max(hash1.Length, hash2.Length)}");
            for (int i = Math.Max(hash1.Length, hash2.Length) - 1; i >= 0; i--)
            {
                // if either array is shorter, nothing to compare so obviously not a match
                if ((hash1.Length > i) && (hash2.Length > i))
                    isValid = isValid && ((hash1[i] ^ hash2[i]) == 0);
                else
                    isValid = false;
                //System.Diagnostics.Trace.WriteLine($"HashesMatch() i={i}, isValid={isValid}");
            }

            return isValid;
        }
    }
}