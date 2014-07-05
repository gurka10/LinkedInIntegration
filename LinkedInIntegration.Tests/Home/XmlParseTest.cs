using LinkedInIntegration.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Web;
using System.Web.Routing;

namespace LinkedInIntegration.Tests
{
    [TestClass]
    public class XmlParseTest
    {
        private string validXML { get; set; }

        private string InValidXML { get; set; }

        private oAuthLinkedIn _oauth = new oAuthLinkedIn();

        [TestMethod]
        public void ProcessXMLResponseForYourProfile_WithValidXML_ReturnsCorrectOutput()
        {
            //Arrange
            var expectedResult = @"Your first name is: Jon
Your last name is: Bock
Your email address is: gurka10@hotmail.com
Your public profile is: http://www.linkedin.com/pub/jon-bock/9/79/a36
Your more recent title is: Creator LTd
";
            validXML = "<?xml version='1.0' encoding='us-ascii'?> <person><first-name>Jon</first-name><last-name>Bock</last-name><email-address>gurka10@hotmail.com</email-address><public-profile-url>http://www.linkedin.com/pub/jon-bock/9/79/a36</public-profile-url><positions><position><title>Creator LTd</title></position></positions></person>";

            //Act
            var result = _oauth.ProcessXMLResponseForYourProfile(validXML, true);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ProcessXMLResponseForYourProfile_WithXMLWitoutCompany_ReturnsCorrectOutput()
        {
            //Arrange
            var expectedResult = @"Your first name is: Jon
Your last name is: Bock
Your email address is: gurka10@hotmail.com
Your public profile is: http://www.linkedin.com/pub/jon-bock/9/79/a36
";
            validXML = "<?xml version='1.0' encoding='us-ascii'?> <person><first-name>Jon</first-name><last-name>Bock</last-name><email-address>gurka10@hotmail.com</email-address><public-profile-url>http://www.linkedin.com/pub/jon-bock/9/79/a36</public-profile-url></person>";

            //Act
            var result = _oauth.ProcessXMLResponseForYourProfile(validXML, true);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result);
        }

    
        
    }
}