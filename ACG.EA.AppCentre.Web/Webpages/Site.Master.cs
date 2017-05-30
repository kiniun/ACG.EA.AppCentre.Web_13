using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACG.EA.AppCentre.Web
{
    public partial class Site : System.Web.UI.MasterPage
    {
        //public string appRoot ;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptPath = "http://" + Request.Url.Host + Request.ApplicationPath + "/Support/Scripts";
            ContentPath = "http://" + Request.Url.Host + Request.ApplicationPath + "/Support/Content";
            StylesPath = "http://" + Request.Url.Host + Request.ApplicationPath + "/Support/Styles";
            appCentrePath = "http://" + Request.Url.Host + Request.ApplicationPath + "/Webpages";
        }

        public string ScriptPath { get; set; }
        public string ContentPath { get; set; }
        public string StylesPath { get; set; }
        public string appCentrePath { get; set; }

    }
}