using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace lastfm.Services
{
    public static class Auth
    {
        static Auth()
        {
            MobileAuth = true;
            //Username = "dummyAcc";
            //Password = "swordfish";
        }

        const string UserAuthUri = "http://www.last.fm/api/auth";


        public static bool MobileAuth { get; set; }

        /// <summary>
        /// Log in to Last.fm services
        /// </summary>
        /// <returns>Session key</returns>
        public static IAsyncResult Authenticate(string username, string password, Action<string> callback)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Username and password must not be empty");

            //if (MobileAuth)
                return GetMobileSession(username, password, callback);
            //else
            //    return GetToken(username, password);
        }

        [RequestParameters(HttpMethod.POST, IsSsl = true)]
        static IAsyncResult GetMobileSession(string username, string password, Action<string> callback)
        {
            var p = new Dictionary<string, string>();
            p.Add("method", "auth.getMobileSession");
            p.Add("username", username.ToLower());
            p.Add("authToken", Util.GetHash(username.ToLower() + Util.GetHash(password)));

            return LfmServiceProxy.GetResponseAsync(p, (doc) =>
                {
                    if (doc.Element("lfm").Attribute("status").Value == "ok")
                    {
                        var sessionKey = doc.Descendants("key").First().Value;
                        callback(sessionKey);
                    }
                });
        }

        static string GetToken(string username, string password)
        {
            throw new NotImplementedException();
        }
    }

}
