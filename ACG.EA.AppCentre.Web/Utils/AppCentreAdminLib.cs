using System;
using System.Collections.Generic;
using System.Linq;
using ACG.EA.AppCentre.Utils;
using LibModel = ACG.EA.AppCentre.Lib.Models;
using ACG.EA.AppCentre.DataUtils;

namespace ACG.EA.AppCentre.Web.Utils
{
    public class AppCentreAdminLib: AC_Base
    {
        public AppCentreAdminLib(string app)
        {
            if (authClient == null)
                authClient = new Authorization();
        }

        public AppCentreAdminLib()
        {
            if (authClient == null)
                authClient = new Authorization();
        }


        protected static Authorization authClient = new Authorization();

        public bool IsUserInRole(string username)
        {
            var perms = authClient.GetUserAccessProfile(username, ApplicationName);
            return perms.Any();

        }

        public static string GetUserProfile(string userName)
        {
            LibModel.User user2;
            try
            {
                user2 = authClient.GetUserProfile(userName);
                if (user2 != null)
                    return user2.First_Name + " " + user2.Last_Name;
                else
                    return "";
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "ACG.EA.AppCentre.Web");
                return "";
            }
        }

        public bool IsAppCentreAdmin(string uName)
        {
            var admin = false;
            try
            {
                admin = authClient.UserIsACAdmin(ApplicationName, uName, permType);
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return admin;
        }

        public bool IsAppAdmin(string uName)
        {
            var admin = false;
            try
            {
                admin = authClient.UserIsAdmin(ApplicationName, uName, permType);
                UpdateUserActivity(ApplicationName, uName, null);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return admin;
        }

        public void UpdateUserActivity(string appId, string user, string target)
        {
            UserAdminLib lib = new UserAdminLib();
            LibModel.ActivityLog log = new LibModel.ActivityLog()
            {
                applicationId = appId,
                uName = user,
                target = target
            };

            try
            {
                lib.UpdateUserActLog(log);
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, ApplicationName);
            }
        }
    }
}