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

            if (!Page.IsPostBack)
            {

                if (oauth_token != null && oauth_verifier != null)
                {
                    hypAuthToken.NavigateUrl = Application["oauthLink"].ToString();
                    hypAuthToken.Text = Application["oauthLink"].ToString();

                    Application["oauth_token"] = oauth_token;
                    Application["oauth_verifier"] = oauth_verifier;


                    _oauth._LinkedInObject._token = oauth_token;
                    _oauth._LinkedInObject._tokenSecret = Application["reuqestTokenSecret"].ToString();
                    _oauth.Verifier = oauth_verifier;

                    _oauth.AccessTokenGet(_oauth._LinkedInObject._token);

                    string response = _oauth.APIWebRequest("GET", "https://api.linkedin.com/v1/people/~:(first-name,last-name,email-address,public-profile-url,positions:(title))", null);

                    lblYourDetails.Text = _oauth.ProcessXMLResponseForYourProfile(response);
                    Session["oAuthProfileDetails"] = lblYourDetails.Text;
                    Session["oAuthLinkedIn"] = _oauth;

                    lblYourDetails.Focus();
                    connectionId.Visible = false;
                    btnPersist.Visible = true;
                }
                else
                {
                    GenerateLinkedInConnectionURL();
                    _oauth = (oAuthLinkedIn)Session["oAuthLinkedIn"];
                    btnPersist.Visible = false;
                }
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



        public void HomeBtn_Click(Object sender,
                          EventArgs e)
        {
            Response.Redirect("CheckForUserSession.aspx");
        }

        

        
    }
}