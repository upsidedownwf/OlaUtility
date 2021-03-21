using System;
using System.Security.Cryptography;
using System.Text;

namespace OlamideIOCContainer
{
    /// <summary>
    /// Class to hold values after hashing
    /// </summary>
    public sealed class HashValues
    {
        /// <summary>
        /// Hashed String
        /// </summary>
        /// <value>Returns the hashed string</value>
        public string HashedString { get; set; }
        /// <summary>
        /// Salt used for hashing
        /// </summary>
        /// <value>Returns the salt used for hashing</value>
        public string Salt { get; set; }
    }
    /// <summary>
    /// Class that handles hashing and verfying a hashed string
    /// </summary>
    public static class HashPassword
    {
        private static string GetSalt()
        {
            byte[] saltArray = new byte[128/2];
            using (var passwordsalt = RandomNumberGenerator.Create())
            {
                passwordsalt.GetBytes(saltArray);
                return BitConverter.ToString(saltArray).Replace("-", "");
            }
        }
        /// <summary>
        /// This method hashes your <paramref name="password"/> string
        /// </summary>
        /// <param name="password">string that is to be hashed</param>
        /// <returns>the hashed string and the salt used to hash it</returns>
        public static HashValues GetHash(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("password cannot be null or empty", nameof(password));
            string salt = GetSalt();
            using (var sha512 = new SHA512Managed())
            {
                var hashedBytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(string.Format("{0}{1}", salt, password)));
                var result = new HashValues()
                {
                    HashedString = BitConverter.ToString(hashedBytes).Replace("-", ""),
                    Salt = salt
                };
                return result;
            }

        }
        /// <summary>
        /// This method tries to match your password and a hashed password 
        /// </summary>
        /// <param name="password">this is the unhashed password</param>
        /// <param name="salt">this is the salt used to hash the password</param>
        /// <param name="hashedPassword">this is the hashed password you want to compare with</param>
        /// <returns>return true if the two password matches with the hashed password else return false </returns>
        /// <exception cref="ArgumentException">Thrown when one or all the parameters are either null or empty</exception>
        public static bool Verify(string password, string salt, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("password cannot be null or empty", nameof(password));
            if (string.IsNullOrEmpty(hashedPassword)) throw new ArgumentException("hashedPassword cannot be null or empty", nameof(hashedPassword));
            using (var sha512 = new SHA512Managed())
            {
                var hashedBytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(string.Format("{0}{1}", salt, password)));
                var hashedASCIIBytes = Encoding.ASCII.GetBytes(BitConverter.ToString(hashedBytes).Replace("-", ""));
                var hashpasswordbytes = Encoding.ASCII.GetBytes(hashedPassword);
                return slowEquals(hashedASCIIBytes, hashpasswordbytes);
            }

        }
        private static bool slowEquals(byte[] a, byte[] b)
        {
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
