using LinkedInIntegration.Service;
using LinkedInIntegration.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinkedInIntegration
{
    public partial class CheckForUserSession : System.Web.UI.Page
    {
        private oAuthLinkedIn _oauth = new oAuthLinkedIn();

        protected void Page_Load(object sender, EventArgs e)
        {
            var profileDetails = Session["oAuthProfileDetails"] != null ? Session["oAuthProfileDetails"].ToString() : "";
            if (!string.IsNullOrEmpty(profileDetails))
            {
                lblYourDetails.Text = profileDetails;
            }
            else
            {
                btnReturn.Visible = true;
                btnReturn.Text = "Sorry your details not found, click to return to home page.";
            }
        }

        public void HomeBtn_Click(Object sender,
                           EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        public void SearchBtn_Click(Object sender,
                          EventArgs e)
        {
            try
            {
                var searchParameter = txtProfileUrl.Text.Trim();
                var profileResults = (LinkedInProfiles)Session["LinkedInProfiles"];
                if (!string.IsNullOrEmpty(searchParameter))
                {
                    var searchResult = profileResults.ProfileList.Where(x => x.ProfileUrl == searchParameter).FirstOrDefault();

                    if (searchResult != null)
                    {
                        lblYourDetails.Text = @"Here are your search results:
First Name : " + searchResult.FirstName + " | Last name : " + searchResult.LastName + " | Email: " + searchResult.Email + " | Most Recent title: " + searchResult.FirstCompany;
                    }
                    else
                    {
                        lblYourDetails.Text = "";
                    }
                }
            }
            catch { Response.Redirect("Default.aspx"); }
        }
    }
}