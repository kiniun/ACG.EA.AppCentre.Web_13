using System;
using System.Web.Http;

namespace ACG.EA.AppCentre.Web.Utils
{
    public class AuthorizationRules: AuthorizeAttribute
    {
        protected string _authorize;
        public string ACAdmin
        {
            get {
                return _authorize;
            }
        }

        public string AuthBuilder()
        {
            try
            {
                //_authorize = Authorization.GetACAdministrators();
            }
            catch(Exception ex)
            {

            }
            return _authorize;
        }
    }
}