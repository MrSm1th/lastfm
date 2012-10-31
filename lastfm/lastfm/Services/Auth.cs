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

        //public static string Username { get; set; }
        //public static string Password { get; set; }

        //static string _sessionKey;
        //public static string SessionKey
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_sessionKey))
        //            if (Util.GetSessionKey() != null)
        //                _sessionKey = Util.GetSessionKey();
        //            else
        //                Authenticate();
        //        return _sessionKey;
        //    }
        //    private set
        //    {
        //        if (value != Util.GetSessionKey())
        //        {
        //            _sessionKey = value;

        //            Util.SaveConfig();
        //        }
        //    }
        //}

        public static bool MobileAuth { get; set; }

        //public static bool IsAuthorized
        //{
        //    get
        //    {
        //        return Util.GetSessionKey() != null;
        //    }
        //}

        /// <summary>
        /// Log in to Last.fm services
        /// </summary>
        /// <returns>Session key</returns>
        public static string Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Username and password must not be empty");

            if (MobileAuth)
                return GetMobileSession(username, password);
            else
                return GetToken(username, password);
        }

        [RequestParameters(HttpMethod.POST, IsSsl = true)]
        static string GetMobileSession(string username, string password)
        {
            var p = new Dictionary<string, string>();
            p.Add("method", "auth.getMobileSession");
            p.Add("username", username.ToLower());
            p.Add("authToken", Util.GetHash(username.ToLower() + Util.GetHash(password)));

            XDocument resp = LfmServiceProxy.GetResponse(p);

            if (resp.Element("lfm").Attribute("status").Value == "ok")
            {
                return resp.Descendants("key").First().Value;
            }
            return string.Empty;
        }

        static string GetToken(string username, string password)
        {
            return "";
        }
    }

}
