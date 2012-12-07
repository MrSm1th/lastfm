using System;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;

namespace lastfm.Services
{
    public enum HttpMethod { GET, POST /* [...] */ }


    public class LastfmException : Exception
    {
        public LastfmException(int errorCode, string description) : base(description)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; }
    }


    public static class Util
    {
        /// <summary>
        /// Reads SessionKey setting from the config file
        /// </summary>
        /// <returns>Session key string</returns>
        //public static string GetSessionKey()
        //{
        //    return ConfigurationManager.AppSettings["SessionKey"];
        //}

        /// <summary>
        /// Saves the config file to disk
        /// </summary>
        //public static void SaveConfig()
        //{
        //    var c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    if (ConfigurationManager.AppSettings["SessionKey"] == null)
        //        c.AppSettings.Settings.Add("SessionKey", Auth.SessionKey);
        //    else
        //        ConfigurationManager.AppSettings["SessionKey"] = Auth.SessionKey;


        //    c.Save(ConfigurationSaveMode.Modified);
        //}

        /// <summary>
        /// Gets the Unix timestamp of DateTime.UtcNow
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return GetUnixTimestamp(DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the Unix timestamp of the DateTime <paramref name="t"/>
        /// </summary>
        public static long GetUnixTimestamp(DateTime t)
        {
            return (t.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks) / 10000000;
        }

        /// <summary>
        /// Gets MD5 hash of the input string
        /// </summary>
        public static string GetHash(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);

            MD5CryptoServiceProvider c = new MD5CryptoServiceProvider();
            buffer = c.ComputeHash(buffer);

            StringBuilder builder = new StringBuilder();
            foreach (byte b in buffer)
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
}
