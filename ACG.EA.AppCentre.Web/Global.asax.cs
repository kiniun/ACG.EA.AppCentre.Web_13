using ACG.EA.AppCentre.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace ACG.EA.AppCentre.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected HttpRequest request;
        public static string masterPage = "";

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var name = User.Identity.Name;
            var i = name.IndexOf("\\") + 1;
            Session["UserName"] = name.Substring(i, name.Length - i);

            Session["InRole"] = false;
            Session["IsAppCentreAdmin"] = false;
            Session["IsAppAdmin"] = false;
            Session["User"] = "";
            Utils.AppCentreAdminLib userProfile = new Utils.AppCentreAdminLib();

            Session["InRole"] = userProfile.IsUserInRole(Session["UserName"].ToString());
            //if (Session["InRole"] && Session["Username"] != null)
            if ((bool)Session["InRole"] && Session["Username"] != null)
            {
                //Session["InRole"] = true;
                Session["User"] = Utils.AppCentreAdminLib.GetUserProfile(Session["UserName"].ToString());
                Session["IsAppCentreAdmin"] = userProfile.IsAppCentreAdmin(Session["UserName"].ToString());
                Session["IsAppAdmin"] = userProfile.IsAppAdmin(Session["UserName"].ToString());
                //Session["IsAppAdmin"] = true;
            }
            else
            {
                Session["InRole"] = false;
            }
            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //masterPage = "http://" + Context.Request.Url.Host;
            //masterPage += Context.Request.Url.Port.ToString() != "" ? ":" + Context.Request.Url.Port + Context.Request.ApplicationPath : "";
            //masterPage += "/Support/Site.Master";
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception _ex = Server.GetLastError();

            // Handle HTTP errors
            if (_ex.GetType() == typeof(HttpException))
            {

                if (_ex.Message.Contains("NoCatch") || _ex.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page
                //Server.Transfer(Request.ApplicationPath + "WebPages/ErrorPage.aspx");
                Server.Transfer("~/WebPages/ErrorPage.aspx");
            }

            // For other kinds of errors give the user some information
            // but stay on the default page
            Response.Write("<h2>Unspecific application Error</h2>\n");
            Response.Write(
                "<p>" + _ex.Message + "</p>\n");
            Response.Write("Return to the <a href='Default.aspx'>" +
                "Home Page</a>\n");

            // Log the _exeption and notify system operators
            ExceptionHandler.LogException(_ex, "/WebPagesDefaultPage");
            ExceptionHandler.NotifySystemOps(_ex);

            // Clear the error from the server
            Server.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}