// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Linq;
using System.Security;

using TEMS.InventoryModel.entity.db.user;

namespace TEMS.InventoryModel.userManager.extension
{
    public static class UserDetailHelper
    {
        /// <summary>
        /// Allows [re]setting stored password hash
        /// </summary>
        /// <param name="passphrase">the password/phrase to hash</param>
        /// <param name="mustChange">defaults true, user must change on next log in; use false if user is setting/changing their own passphrase</param>
        public static void SetPasswordHash(this UserDetail user, SecureString passphrase, bool mustChange = true)
        {
            user.isPasswordExpired = mustChange;
            user.hashedPassphrase = PasswordHashing.EncodePassword(passphrase, null);
        }

        /// <summary>
        /// Helper method to convert an insecure string into a SecureString (for testing)
        /// Note: returns null if provided string is blank or null
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string passPhrase)
        {
            SecureString secureString = null;

            if (!string.IsNullOrWhiteSpace(passPhrase))
            {
                secureString = new SecureString();
                passPhrase.ToList().ForEach(secureString.AppendChar);
            }

            return secureString;
        }
    }
}