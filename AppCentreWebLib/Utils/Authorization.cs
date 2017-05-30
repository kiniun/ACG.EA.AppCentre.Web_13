using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACG.EA.AppCentre.Lib.DAL;
using ACG.EA.AppCentre.Lib.Models;

namespace ACG.EA.AppCentre.Utils
{
    public class Authorization
    {
        //protected const string ApplicationName = "AppCentre";
        //protected const string acgControl = "Admin";

        public User GetUserProfile(string username)
        {

            User _UserProfile = new User();

            using (var db = new AppCentreEntities())
            {

                var user = (from li in db.USERs
                            where li.USER_NAME.ToLower() == username
                            select li).FirstOrDefault();


                if (user != null)
                {
                    _UserProfile.First_Name = user.FIRST_NAME;
                    _UserProfile.Last_Name = user.LAST_NAME;
                    _UserProfile.Email = user.EMAIL;
                    _UserProfile.Phone = user.PHONE;
                    _UserProfile.Title = user.TITLE;
                    _UserProfile.User_Name = user.USER_NAME;
                    //  _UserProfile.ModifiedBy = user.MODIFIED_BY;
                    // _UserProfile.LastModified = (System.DateTime)user.LAST_MODIFIED;

                }

                return _UserProfile;

            }
        }

        public List<string> GetUserAccessProfile(string userId, string app)
        {
            var prof = new List<string>();

            using(var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from perms in db.VW_USER_PERMISSION.Where(p => p.USER_NAME == userId && p.APPLICATION_ID == app)
                            orderby perms.PERMISSION_ID
                            select perms.PERMISSION_ID;
                prof = query.ToList<string>();
            }
            return prof;
        }


        public static string GetACAdministrators(string acgControl, string app)
        {
            var admins = new StringBuilder();
            IQueryable<string> query;
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                query = from perms in db.VW_USER_PERMISSION.Where(p => p.APPLICATION_ID == app && p.PERMISSION_ID == acgControl)
                            orderby perms.USER_NAME
                            select perms.USER_NAME;
            }
            
            foreach(var user in query)
            {
                if (admins.Length == 0)
                {
                    admins.Append(user);
                }
                else
                    admins.Append(", " + user);
            }
            return admins.ToString().Trim(new char[] {',', ' '});
        }

        public bool UserIsACAdmin(string app, string uName, string acgControl)
        {
            var isAdmin = false;
            try
            {
                using(var db = new AppCentreEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    //var qry = from usrs in db.USER_PERMISSION
                    //          where (usrs.USER.USER_NAME == uName && usrs.APPLICATION_ID == app && usrs.PERMISSION_ID == acgControl)
                    //          select usrs.PERMISSION_ID;
                    //var aGrp = (from uGrp in db.USER_APPLICATION_GROUP
                    //            where (uGrp.APPLICATION_ID == app && uGrp.GROUP_ID == acgControl && uGrp.USER.USER_NAME == uName)
                    //            select uGrp.USER.USER_NAME).ToList();

                    var perms = from perm in db.VW_USER_PERMISSION
                                where (perm.USER_NAME == uName && perm.APPLICATION_ID == app && perm.PERMISSION_ID == acgControl)
                                select perm;
                    if (perms.Any())
                    {
                        isAdmin = true; 
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
            }
            return isAdmin;
        }

        public bool UserIsAdmin(string app, string uName, string acgControl)
        {
            bool isAdmin = false;
            try
            {
                using (var db = new AppCentreEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var qry = from usrs in db.USER_PERMISSION
                              //join u in db.USERs
                              //on usrs.USER_ID equals u.USER_ID
                              where (usrs.USER.USER_NAME == uName && usrs.PERMISSION_ID.Contains(acgControl))
                              select usrs;
                    //isAdmin = qry != null;
                    var qGrp = from grp in db.USER_APPLICATION_GROUP
                               where (grp.USER.USER_NAME == uName && grp.GROUP_ID.Contains(acgControl))
                               select grp;
                    //isAdmin = qry != null || qGrp != null;
                    if (qry.Any() || qGrp.Any())
                        isAdmin = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
            }
            return isAdmin;
        }
    }
}
