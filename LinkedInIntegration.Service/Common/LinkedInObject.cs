using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInIntegration.Service.Common
{
    public class LinkedInObject
    {
        public string _consumerKey { get; set; }
        public string _consumerSecret { get; set; }
        public string _token { get; set; }
        public string _tokenSecret { get; set; }

        public LinkedInObject()
        {
            _consumerKey = "75uoy4wwrukxis";
            _consumerSecret = "7M2aNia4ZCvntoay";
        }
    }
}
