using LinkedInIntegration.Service;
using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

namespace LinkedInIntegration
{
    public partial class _Default : Page
    {
        private oAuthLinkedIn _oauth = new oAuthLinkedIn();

        protected void Page_Load(object sender, EventArgs e)
        {
            string oauth_token = Request.QueryString["oauth_token"];
            string oauth_verifier = Request.QueryString["oauth_verifier"];
            

            if (oauth_token != null && oauth_verifier != null)
            {
                //txtRequestToken.Text = Application["reuqestToken"].ToString();
                //txtTokenSecret.Text = Application["reuqestTokenSecret"].ToString();
                hypAuthToken.NavigateUrl = Application["oauthLink"].ToString();
                hypAuthToken.Text = Application["oauthLink"].ToString();

                Application["oauth_token"] = oauth_token;
                Application["oauth_verifier"] = oauth_verifier;

                //txtoAuth_token.Text = oauth_token;
                //txtoAuth_verifier.Text = oauth_verifier;

                _oauth._LinkedInObject._token = oauth_token;
                _oauth._LinkedInObject._tokenSecret = Application["reuqestTokenSecret"].ToString();
                _oauth.Verifier = oauth_verifier;

                _oauth.AccessTokenGet(_oauth._LinkedInObject._token);
                //txtAccessToken.Text = _oauth._LinkedInObject._token;
                //txtAccessTokenSecret.Text = _oauth._LinkedInObject._tokenSecret;

                string response = _oauth.APIWebRequest("GET", "https://api.linkedin.com/v1/people/~:(first-name,last-name,email-address,public-profile-url,positions:(company:(name)))", null);
          

                lblYourDetails.Text = ProcessXMLResponseForYourProfile(response);

                lblYourDetails.Focus();
                connectionId.Visible = false;
            }
            else
            {
                GenerateLinkedInConnectionURL();
            }
        }

        private void GenerateLinkedInConnectionURL()
        {
            string authLink = _oauth.AuthorizationLinkGet();
            Application["reuqestToken"] = _oauth._LinkedInObject._token;
            Application["reuqestTokenSecret"] = _oauth._LinkedInObject._tokenSecret;
            Application["oauthLink"] = authLink;

            hypAuthToken.NavigateUrl = authLink;
            hypAuthToken.Text = "Click here to load your profile details";
        }

    



        private string ProcessXMLResponseForYourProfile(string inputXMLResponse)
        {
            var output = new StringBuilder();
            XmlDocument doc = new XmlDocument();
            var companyFirstIsNotComplete = true;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(inputXMLResponse); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.SelectNodes("/person");
                foreach (XmlNode node in xnList)
                {
                    output.AppendLine("Your first name is: " + node["first-name"].InnerText );
                    output.AppendLine("Your last name is: " + node["last-name"].InnerText );
                    output.AppendLine("Your email address is: " + node["email-address"].InnerText );
                    output.AppendLine("Your public profile is: " + node["public-profile-url"].InnerText );
                }
     
                

                xml.LoadXml(inputXMLResponse);
                XmlNodeList xnCompany = xml.SelectNodes("/person/positions/position/company");
                foreach (XmlNode firstCompany in xnCompany)
                {
                    if (companyFirstIsNotComplete)
                    {
                        output.AppendLine("Your first company is: " + firstCompany["name"].InnerText );
                        companyFirstIsNotComplete = false;
                    }
                }
         
                


                return output.ToString();
            }
            catch { }
            return "";
        }

        
    }
}