using LinkedInIntegration.Service.Common;
using LinkedInIntegration.Service.DTO;
using LinkedInIntegration.Service.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace LinkedInIntegration.Service
{
    [System.Runtime.InteropServices.GuidAttribute("B0410499-E988-4370-8BAD-EB2A9E1EED2F")]
    public class oAuthLinkedIn : oAuthBase2
    {
        

        public const string REQUEST_TOKEN = "https://api.linkedin.com/uas/oauth/requestToken";
        public const string AUTHORIZE = "https://api.linkedin.com/uas/oauth/authorize";
        public const string ACCESS_TOKEN = "https://api.linkedin.com/uas/oauth/accessToken";

        public readonly LinkedInObject _LinkedInObject;

        public oAuthLinkedIn()
        {
            _LinkedInObject = new LinkedInObject();
        }

        public string ProcessXMLResponseForYourProfile(string inputXMLResponse, bool InTest = false)
        {
            var output = new StringBuilder();
            XmlDocument doc = new XmlDocument();
            var companyFirstTitleIsNotComplete = true;
            var profileList = new LinkedInProfiles();
            profileList.ProfileList = new List<Profile>();
            var profileItem = new Profile();
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(inputXMLResponse);
                XmlNodeList xnList = xml.SelectNodes("/person");
                foreach (XmlNode node in xnList)
                {
                    AppendOutputString(output, node);

                    SetLinkedInProfiles(profileItem, node);
                }

                xml.LoadXml(inputXMLResponse);
                XmlNodeList xnCompany = xml.SelectNodes("/person/positions/position");
                foreach (XmlNode firstCompany in xnCompany)
                {
                    companyFirstTitleIsNotComplete = GetCompanyCurrentTitle(output, companyFirstTitleIsNotComplete, profileItem, firstCompany);
                }

                profileList.ProfileList.Add(profileItem);

                if (!InTest)
                {
                    HttpContext.Current.Session["LinkedInProfiles"] = profileList;
                }

                return output.ToString();
            }
            catch { }
            return "";
        }

        private static bool GetCompanyCurrentTitle(StringBuilder output, bool companyFirstTitleIsNotComplete, Profile profileItem, XmlNode firstCompany)
        {
            if (companyFirstTitleIsNotComplete)
            {
                output.AppendLine("Your more recent title is: " + firstCompany["title"].InnerText);
                profileItem.FirstCompany = firstCompany["title"].InnerText;
                companyFirstTitleIsNotComplete = false;
            }
            return companyFirstTitleIsNotComplete;
        }

        private static void AppendOutputString(StringBuilder output, XmlNode node)
        {
            output.AppendLine("Your first name is: " + node["first-name"].InnerText);

            output.AppendLine("Your last name is: " + node["last-name"].InnerText);

            output.AppendLine("Your email address is: " + node["email-address"].InnerText);

            output.AppendLine("Your public profile is: " + node["public-profile-url"].InnerText);
        }

        private static void SetLinkedInProfiles(Profile profileItem, XmlNode node)
        {
            profileItem.Email = node["email-address"].InnerText;
            profileItem.FirstName = node["first-name"].InnerText;
            profileItem.LastName = node["last-name"].InnerText;
            profileItem.ProfileUrl = node["public-profile-url"].InnerText;
        }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            string ret = null;

            string response = oAuthWebRequest(MethodEnums.Method.POST, REQUEST_TOKEN, String.Empty);
            if (response.Length > 0)
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    _LinkedInObject._token = qs["oauth_token"];
                    _LinkedInObject._tokenSecret = qs["oauth_token_secret"];
                    ret = AUTHORIZE + "?oauth_token=" + _LinkedInObject._token;
                }
            }
            return ret;
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken)
        {
            _LinkedInObject._token = authToken;

            string response = oAuthWebRequest(MethodEnums.Method.POST, ACCESS_TOKEN, string.Empty);

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    _LinkedInObject._token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    _LinkedInObject._tokenSecret = qs["oauth_token_secret"];
                }
            }
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string oAuthWebRequest(MethodEnums.Method method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";

            if (method == MethodEnums.Method.POST)
            {
                if (postData.Length > 0)
                {
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];
                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                _LinkedInObject._consumerKey,
                _LinkedInObject._consumerSecret,
                _LinkedInObject._token,
                _LinkedInObject._tokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            //Convert the querystring to postData
            if (method == MethodEnums.Method.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            if (method == MethodEnums.Method.POST || method == MethodEnums.Method.GET)
                ret = WebRequest(method, outUrl + querystring, postData);
            //else if (method == Method.PUT)
            //ret = WebRequestWithPut(outUrl + querystring, postData);
            return ret;
        }

        /// <summary>
        /// WebRequestWithPut
        /// </summary>
        /// <param name="method">WebRequestWithPut</param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string APIWebRequest(string method, string url, string postData)
        {
            Uri uri = new Uri(url);
            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            string outUrl, querystring;

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                _LinkedInObject._consumerKey,
                _LinkedInObject._consumerSecret,
                _LinkedInObject._token,
                _LinkedInObject._tokenSecret,
                method,
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            HttpWebRequest webRequest = null;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;

            webRequest.Method = method;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.AllowWriteStreamBuffering = true;

            webRequest.PreAuthenticate = true;
            webRequest.ServicePoint.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            webRequest.Headers.Add("Authorization", "OAuth realm=\"http://api.linkedin.com/\",oauth_consumer_key=\"" + _LinkedInObject._consumerKey + "\",oauth_token=\"" + _LinkedInObject._token + "\",oauth_signature_method=\"HMAC-SHA1\",oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\",oauth_timestamp=\"" + timeStamp + "\",oauth_nonce=\"" + nonce + "\",oauth_verifier=\"" + this.Verifier + "\", oauth_version=\"1.0\"");
            if (postData != null)
            {
                byte[] fileToSend = Encoding.UTF8.GetBytes(postData);
                webRequest.ContentLength = fileToSend.Length;

                Stream reqStream = webRequest.GetRequestStream();

                reqStream.Write(fileToSend, 0, fileToSend.Length);
                reqStream.Close();
            }

            string returned = WebResponseGet(webRequest);

            return returned;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(MethodEnums.Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;

            if (method == MethodEnums.Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}