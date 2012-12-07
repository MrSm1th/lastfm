using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;

namespace lastfm.Services
{
    public static class LfmServiceProxy
    {
        class RequestState
        {
            public HttpWebRequest Request { get; set; }

            public RequestState(HttpWebRequest r)
            {
                Request = r;
            }
        }

        class SendRequestState
        {
            public SendRequestDelegate Delegate { get; private set; }
            public Action<XDocument> Callback { get; private set; }

            public SendRequestState(SendRequestDelegate d, Action<XDocument> callback)
            {
                Delegate = d;
                Callback = callback;
            }
        }

        public delegate XDocument SendRequestDelegate(Dictionary<string, string> parameters, string httpMethod, bool authNeeded, bool isSsl);

        public delegate void RequestErrorEventHandler(object sender, RequestErrorEventArgs e);

        public class RequestErrorEventArgs : EventArgs
        {
            public int? LastfmErrorCode { get; private set; }
            public string Message { get; private set; }
            public Dictionary<string, string> RequestParameters { get; private set; }

            public RequestErrorEventArgs(string message)
            {
                Message = message;
            }

            public RequestErrorEventArgs(string message, int errorCode, Dictionary<string, string> requestParameters)
            {
                Message = message;
                LastfmErrorCode = errorCode;
                RequestParameters = requestParameters;
            }
        }

        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        public class ErrorEventArgs : EventArgs
        {
            public Exception Error { get; set; }

            public ErrorEventArgs(Exception ex)
            {
                Error = ex;
            }
        }



        public static event RequestErrorEventHandler NetworkErrorOccured;
        static void OnNetworkErrorOccured(RequestErrorEventArgs e)
        {
            if (NetworkErrorOccured != null)
                NetworkErrorOccured(null, e);
        }

        public static event RequestErrorEventHandler LastfmErrorOccured;
        static void OnLastfmErrorOccured(RequestErrorEventArgs e)
        {
            if (LastfmErrorOccured != null)
                LastfmErrorOccured(null, e);
        }

        public static event ErrorEventHandler ErrorOccured;
        static void OnErrorOccured(ErrorEventArgs e)
        {
            if (ErrorOccured != null)
                ErrorOccured(null, e);
        }


        const string Uri = "http://ws.audioscrobbler.com/2.0/";
        const string SslUri = "https://ws.audioscrobbler.com/2.0/";
        const string ApiKey = "{api key here}";
        const string SharedSecret = "{shared secret here}";

        public static string SessionKey { get; set; }

        static LfmServiceProxy()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// Begins an asynchronous request and executes supplied callback on the request results
        /// </summary>
        /// <param name="parameters">Request parameters</param>
        /// <param name="callback">Callback to execute when a response is received. Null allowed</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IAsyncResult GetResponseAsync(Dictionary<string, string> parameters, Action<XDocument> callback)
        {
            var httpMethod = "GET";
            var authNeeded = false;
            var isSsl = false;

            RequestParametersAttribute attr = null;
            var a = new StackFrame(1, false).GetMethod().GetCustomAttributes(typeof(RequestParametersAttribute), false);
            if (a.Length > 0) attr = a[0] as RequestParametersAttribute;
            if (attr != null)
            {
                httpMethod = attr.HttpMethod.ToString();
                authNeeded = attr.AuthNeeded;
                isSsl = attr.IsSsl;
            }

            Logger.WriteEmptyLine();
            Logger.LogMessage(parameters["method"], "Request");

            var d = new SendRequestDelegate(SendRequest);
            return d.BeginInvoke(
                parameters,
                httpMethod,
                authNeeded,
                isSsl,
                new AsyncCallback((res) =>
                {
                    var s = (SendRequestState)res.AsyncState;
                    var doc = s.Delegate.EndInvoke(res);
                    if (doc != null) // no exceptions occured
                    {
                        Logger.LogMessage("Last.fm response OK");
                        //Logger.LogMessage(Environment.NewLine + doc.Document.ToString(), "Last.fm response");

                        if (s.Callback != null)
                            s.Callback(doc);
                    }
                }),
                new SendRequestState(d, callback));
        }

        /*
        static IAsyncResult GetResponseAsync(HttpWebRequest r, Action<XDocument> callback)
        {
            return r.BeginGetResponse(new AsyncCallback((res) =>
                {
                    var st = (RequestState)res.AsyncState;
                    var resp = st.Request.EndGetResponse(res);
                    var xDoc = XDocument.Load(new StreamReader(resp.GetResponseStream()));

                    callback(xDoc);
                }),
                new RequestState(r));
        }
        */


        /// <summary>
        /// Create and send a request to the server
        /// </summary>
        /// <returns>Response XML</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static XDocument GetResponse(Dictionary<string, string> parameters)
        {
            var httpMethod = "GET";
            var authNeeded = false;
            var isSsl = false;

            RequestParametersAttribute attr = null;
            var a = new StackFrame(1, false).GetMethod().GetCustomAttributes(typeof(RequestParametersAttribute), false);
            if (a.Length > 0) attr = a[0] as RequestParametersAttribute;
            if (attr != null)
            {
                httpMethod = attr.HttpMethod.ToString();
                authNeeded = attr.AuthNeeded;
                isSsl = attr.IsSsl;
            }

            //if (authNeeded && !Auth.IsAuthorized)
            //    Auth.Authenticate();

            XDocument doc = null;
            try
            {
                var req = BuildRequest(parameters, httpMethod, authNeeded, isSsl);
                doc = GetResponse(req);
            }
            catch (WebException ex)
            {
                HandleWebException(ex, parameters);
            }

            return doc;
        }

        /// <summary>
        /// Send a request to the server
        /// </summary>
        /// <returns>Response XML</returns>
        static XDocument GetResponse(HttpWebRequest r)
        {
            r.Timeout = 1 * 60 * 1000;
            var resp = r.GetResponse();
            return XDocument.Load(new StreamReader(resp.GetResponseStream()));
        }

        static XDocument SendRequest(Dictionary<string, string> parameters, string httpMethod, bool authNeeded, bool isSsl)
        {
            XDocument doc = null;
            try
            {
                var req = BuildRequest(parameters, httpMethod, authNeeded, isSsl);
                doc = GetResponse(req);
            }
            catch (WebException ex)
            {
                HandleWebException(ex, parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                OnErrorOccured(new ErrorEventArgs(ex));
            }
            return doc;
        }

        static HttpWebRequest BuildRequest(Dictionary<string, string> parameters, string httpMethod, bool authNeeded, bool isSsl)
        {
            parameters.Add("api_key", ApiKey);
            if (authNeeded)
            {
                if (string.IsNullOrEmpty(SessionKey))
                    throw new InvalidOperationException("Session key must be set before sending write requests to Last.fm API");

                parameters.Add("sk", SessionKey);
                parameters.Add("api_sig", GetSignature(parameters));
            }
            if (parameters["method"] == "auth.getMobileSession")
                parameters.Add("api_sig", GetSignature(parameters));    // TODO: ? which requests require the api_sig parameter ?

            var reqStr = BuildParametersString(parameters, httpMethod == "POST");

            var baseUri = new Uri(isSsl ? SslUri : Uri);

            HttpWebRequest req;

            if (httpMethod == "POST")
            {
                req = WebRequest.Create(baseUri) as HttpWebRequest;
                req.Method = httpMethod;
                req.ContentLength = reqStr.Length;
                req.ContentType = "application/x-www-form-urlencoded";

                using (var sw = new StreamWriter(req.EndGetRequestStream(req.BeginGetRequestStream(null, null))))
                {
                    sw.Write(reqStr);
                }
            }
            else
            {
                req = WebRequest.Create(new Uri(baseUri, reqStr)) as HttpWebRequest;
                req.Method = httpMethod;
            }

            return req;
        }

        static string BuildParametersString(Dictionary<string, string> parameters, bool isPostData)
        {
            var res = string.Join("&", parameters.Select(p => p.Key + '=' + HttpUtility.UrlEncode(p.Value)).ToArray());
            return isPostData ? res : '?' + res;
        }

        static string GetSignature(Dictionary<string, string> parameters)
        {
            var p = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            var str = string.Concat(p.Select(pp => pp.Key + pp.Value).ToArray()) + SharedSecret;

            return Util.GetHash(str);
        }

        static bool IsResponseOK(XDocument doc)
        {
            try
            {
                return doc.Element("lfm").Attribute("status").Value == "ok";
            }
            catch (Exception)
            {
                throw new ArgumentException("Specified XDocument isn't a valid Last.fm response");
            }
        }

        static void HandleWebException(WebException ex, Dictionary<string, string> requestParameters)
        {
            if (ex.Response != null)
            {
                string responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                try
                {
                    var xDoc = XDocument.Parse(responseBody);
                    var error = xDoc.Element("lfm").Element("error");
                    var errorCode = error.Attribute("code").Value;
                    var errorValue = error.Value.Trim();
                    Logger.LogError(string.Format("code {0} '{1}'", errorCode, errorValue), "Last.fm error");
                    requestParameters.Remove("api_key");
                    requestParameters.Remove("sk");
                    OnLastfmErrorOccured(new RequestErrorEventArgs(errorValue, int.Parse(errorCode), requestParameters));
                    return;
                }
                catch
                {
                    Logger.LogMessage("Server response:" + Environment.NewLine + responseBody);
                }
                finally
                {
                    var br = Environment.NewLine;
                    var pStr = requestParameters.Aggregate("", (str, p) => str += string.Format("{0}: {1}{2}", p.Key, p.Value, br));
                    Logger.LogMessage(br + pStr, "Request parameters");
                    Logger.LogError(ex.Message);
                }
            }
            else
            {
                Logger.LogError(ex.Message);
            }

            OnNetworkErrorOccured(new RequestErrorEventArgs(ex.Message));
        }
    }
}
