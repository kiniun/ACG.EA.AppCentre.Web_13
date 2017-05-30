using System;
using System.Web.Http;

namespace ACG.EA.AppCentre.Web.Utils
{
    public class AC_Base
    {
        public AC_Base()
        {

        }

        protected readonly string ApplicationName = System.Configuration.ConfigurationManager.AppSettings["application"].ToString();
        protected readonly string permType = System.Configuration.ConfigurationManager.AppSettings["permissionTyp"].ToString();
    }

    public class AC_Controllers: ApiController
    {
        public AC_Controllers()
        {

        }

        protected readonly string ApplicationName = System.Configuration.ConfigurationManager.AppSettings["application"].ToString();
        protected readonly string permType = System.Configuration.ConfigurationManager.AppSettings["permissionTyp"].ToString();
    }
}