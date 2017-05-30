using ACG.EA.AppCentre.Lib.DAL;
using ACG.EA.AppCentre.Lib.Models;
using ACG.EA.AppCentre.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ACG.EA.AppCentre.DataUtils
{
        
    public class UserAdminLib
    {
        public UserAdminLib()
        {  }

        /// <summary>
        /// retrieves all users profile matching the request
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="user">search string</param>
        /// /// /// <param name="searchBy">search by</param>
        /// <returns>bool: success/failure</returns>
        public List<User> LoadSearchedUserProfile(string searchBy, string user)
        {
            var users = new List<User>();
            IEnumerable<USER> query = new List<USER>();
            //disable the creation of proxy objects for EF's POCO objects.
            //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (searchBy == "User_Name")
                {
                    query = from aUsers in db.USERs
                            where (aUsers.USER_NAME.StartsWith(user))
                            orderby aUsers.USER_NAME
                            select aUsers;
                }
                else if (searchBy == "First_Name")
                {
                    query = from aUsers in db.USERs
                            where (aUsers.FIRST_NAME.Contains(user))
                            orderby aUsers.USER_NAME
                            select aUsers;
                }
                else if (searchBy == "Last_Name")
                {
                    query = from aUsers in db.USERs
                            where (aUsers.LAST_NAME.Contains(user))
                            orderby aUsers.USER_NAME
                            select aUsers;
                }
                else if (searchBy == "All")
                {
                    query = from aUsers in db.USERs
                            orderby aUsers.USER_NAME
                            select aUsers;
                }

                if (query.Any())
                {
                    foreach (var u in query)
                    {
                        users.Add(new User()
                        {
                            User_Id = u.USER_ID,
                            User_Name = u.USER_NAME,
                            First_Name = u.FIRST_NAME,
                            Last_Name = u.LAST_NAME,
                            Title = u.TITLE,
                            Email = u.EMAIL,
                            Phone = u.PHONE,
                            Create_Date = u.CREATE_DATE.Date.ToString(),
                            Last_Modified = u.LAST_MODIFIED.ToString(),
                            Modified_By = u.MODIFIED_BY
                        });
                    }
                }
            }
            return users;

        }

        /// <summary>
        /// adds new user to the dB
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="value">User object</param>
        /// <returns>bool: success/failure</returns>
        public bool AddNewUser(User value)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var user = new USER()
                    {
                        USER_NAME = value.User_Name,
                        LAST_NAME = value.Last_Name,
                        FIRST_NAME = value.First_Name,
                        EMAIL = value.Email,
                        LAST_MODIFIED = DateTime.Now,
                        PHONE = value.Phone,
                        TITLE = value.Title,
                        MODIFIED_BY = value.Modified_By,
                        CREATE_DATE = DateTime.Now
                    };
                    //var current = db.USERs.Find(value.User_Id);
                    //db.USERs.Attach(user);
                    ////if (value.User_Id == null)
                    ////{
                    ////    db.USERs.Add(user); 
                    ////}
                    db.USERs.Add(user); 
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                return false;
            }
            return true;
        }

        /// <summary>
        /// update this user's details
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="usr">User object</param>
        /// <returns>bool: success/failure</returns>
        public bool UpdateUser(User usr)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    if (usr.User_Id != 0)
                    {
                        var u = db.USERs.Find(usr.User_Id);
                        u.LAST_NAME = usr.Last_Name ?? u.LAST_NAME;
                        u.FIRST_NAME = usr.First_Name ?? u.LAST_NAME;
                        u.MODIFIED_BY = usr.Modified_By;
                        u.LAST_MODIFIED = DateTime.Now;
                        u.USER_NAME = usr.User_Name ?? u.USER_NAME;
                        u.EMAIL = usr.Email ?? "";
                        u.PHONE = usr.Phone ?? "";
                        u.TITLE = usr.Title ?? ""; 
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                return false;
            }
            return true;
        }

        /// <summary>
        /// check if the entered user id exists.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="appId">application Id<
        /// /// <param name="userName">user id</param>
        /// <returns>bool: yse/no</returns>
        public bool CheckUserExists(string userName)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var usr = db.USERs.Where(u => u.USER_NAME == userName).FirstOrDefault();
                    if (usr != null)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                return false;
            }
        }

        /// <summary>
        /// deletes the user from the dB along with related objects
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="user_id">user id</param>
        /// <returns>bool: success/failure</returns>
        public bool RemoveUser(int user_id)
        {
            using (var db = new AppCentreEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try 
	                {	        
		                int modified = 0;
                        var user = db.USERs.Find(user_id);
                        var uPermissions = db.USER_PERMISSION.Where(u => u.USER_ID == user_id);
                        var uGroups = db.USER_APPLICATION_GROUP.Where(u => u.USER_ID == user_id);

                        if (uPermissions.Any())
                        {
                            foreach (var usr in uPermissions)
                            {
                                db.USER_PERMISSION.Remove(usr);
                                modified++;
                            }
                        }
                        if (uGroups.Any())
                        {
                            foreach (var usr in uGroups)
                            {
                                db.USER_APPLICATION_GROUP.Remove(usr);
                                modified++;
                            }
                        }
                        db.SaveChanges();

                        if (!uGroups.Any() && !uPermissions.Any())
                        {
                            db.USERs.Remove(user);
                            modified++;
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
	                }
	                catch (Exception ex)
	                {
                        dbContextTransaction.Rollback();
                        ExceptionHandler.LogException(ex, "AppCentre");
                        return false;
	                } 
                }
            }
        }

        /// <summary>
        /// retrieves the groups for the provided user and application.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="appId">application Id</param>
        /// /// <param name="user">user id</param>
        /// <returns>List of User_GroupPT objects</returns>
        public List<User_GroupPT> LoadSelectedUserGroups(int user, string appId)
        {
            var userGrpView = new List<User_GroupPT>();
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {

                    var groups = from p in db.APPLICATION_GROUP
                                      where (p.APPLICAITON_ID == appId)
                                      select p;
                    var userGroups = from g in db.USER_APPLICATION_GROUP
                                     where (g.APPLICATION_ID == appId && g.USER_ID == user)
                                     select g;

                    var multi = from p in groups
                                join u in userGroups
                                on new { app = p.APPLICAITON_ID, groupId = p.GROUP_ID } equals new { app = u.APPLICATION_ID, groupId = u.GROUP_ID }
                                into subGroups
                                from sub in subGroups.DefaultIfEmpty()
                                select new User_GroupPT()
                                {
                                    USER_APPLICATION_GROUP_ID = sub.USER_APPLICATION_GROUP_ID,
                                    USER_ID = sub.USER_ID,
                                    GROUP_ID = p.GROUP_ID,
                                    GROUP_NAME = p.GROUP_NAME,
                                    APPLICATION_ID = p.APPLICAITON_ID,
                                    isMember = sub.GROUP_ID != null
                                };

                    var permissionView = new List<PermissionId>();

                    foreach (var usrGroup in multi)
                    {
                        permissionView = (from perms in db.GROUP_PERMISSION
                                          join p in db.PERMISSIONs
                                          on new { perms.PERMISSION_ID, perms.APPLICATION_ID } equals new { p.PERMISSION_ID, p.APPLICATION_ID }
                                          where (perms.GROUP_ID == usrGroup.GROUP_ID && perms.APPLICATION_ID == appId && (!(bool)p.GROUP_ENABLED || p.GROUP_ENABLED == null))
                                          orderby perms.GROUP_PERMISSSION_ID
                                          select new PermissionId
                                          {
                                              PERMISSION_ID = perms.PERMISSION_ID,
                                              TARGET_ID = perms.TARGET_VALUE_ID
                                          }).ToList<PermissionId>();
                        
                        var permissions = string.Empty;
                        var targets = string.Empty;

                        foreach(var perm in permissionView)
                        {
                            if (permissions == "")
                                permissions = perm.PERMISSION_ID;
                            else
                                permissions += "," + perm.PERMISSION_ID;
                            if (targets == "")
                                targets = perm.TARGET_ID.ToString();
                            else
                                targets += "," + perm.TARGET_ID.ToString();
                        }
                        usrGroup.Permissions = permissions;
                        usrGroup.TargetValues = targets;

                        userGrpView.Add(usrGroup);
                    }

                    //userGrpView = multi.ToList<User_Groups>();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "AppCentre");
                }

                return userGrpView;
            }
        }

        /// <summary>
        /// retrieves users of the application.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="appId">application Id</param>
        /// <returns>List of UserPermissionProfile objects</returns>
        public bool SetUserGroups(UserGroupSet grpSet)
        {
            var modified = 0;
            var userId = Convert.ToInt16(grpSet.userId);
            using (var db = new AppCentreEntities())
            {
                for (var i = 0; i < grpSet.groups.Count(); i++)
                {
                    try
                    {
                        int g = grpSet.usrAppGrpIds[i];
                        if (g == 0)
                        {
                            if ((bool)grpSet.isMember[i])
                            {
                                db.USER_APPLICATION_GROUP.Add(new USER_APPLICATION_GROUP
                                                    {
                                                        USER_ID = userId,
                                                        APPLICATION_ID = grpSet.applicationId,
                                                        GROUP_ID = grpSet.groups[i]
                                                    });
                                modified++;
                            }
                        }
                        else
                        {
                            if(!(bool)grpSet.isMember[i])
                            {
                                var uGrp = (from u in db.USER_APPLICATION_GROUP
                                           where (u.USER_APPLICATION_GROUP_ID == g)
                                           select u).SingleOrDefault();
                                db.USER_APPLICATION_GROUP.Remove(uGrp);
                                modified++;
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex, "AppCentre");
                        return false;
                    }
                }
                if (modified > 0)
                {
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// retrieves users of the application.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="appId">application Id</param>
        /// <returns>List of UserPermissionProfile objects</returns>
        public List<UserPermissionProfile> GetApplicationUsers(string appId)
        {
            var uApps = new List<UserPermissionProfile>();
            using (var db = new AppCentreEntities())
            {

                var query = (from users in db.USERs
                             join p in db.USER_PERMISSION
                             on users.USER_ID equals p.USER_ID
                             where (p.APPLICATION_ID == appId)
                             orderby users.USER_ID
                             select new UserPermissionProfile()
                             {
                                 user_permissionId = p.USER_PERMISSION_ID,
                                 User_Name = users.USER_NAME,
                                 First_Name = users.FIRST_NAME,
                                 Last_Name = users.LAST_NAME,
                                 grant = p.GRANT,
                                 permission_Id = p.PERMISSION_ID,
                                 target_value = p.TARGET_VALUE.NAME,
                                 application_Id = p.APPLICATION_ID
                             }).Distinct();
                uApps = query.ToList<UserPermissionProfile>();

                return uApps;
            }
        }

        /// <summary>
        /// update user's acitivty log.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="atL">ActivityLog object</param>
        /// <returns>void</returns>
        public void UpdateUserActLog(ActivityLog atL)
        {
            try
            {
                using(var db = new AppCentreEntities())
                {
                    var actLog = (from aL in db.ACTIVITY_LOG
                                 where (atL.applicationId == aL.APPLICATION_NAME && atL.uName == aL.USER_NAME && (atL.target == null ? aL.TARGET_VALUE == null : atL.target == aL.TARGET_VALUE))
                                 select aL).FirstOrDefault();
                    if(!(actLog == null))
                    {
                        db.ACTIVITY_LOG.Add(new ACTIVITY_LOG()
                        {
                            USER_NAME = atL.uName,
                            APPLICATION_NAME = atL.applicationId,
                            TARGET_VALUE = atL.target ?? null,
                            ACTIVITY_DATE = DateTime.Now
                        });
                    }
                    else
                    {
                        actLog.ACTIVITY_DATE = DateTime.Now;
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
            }
        }

    }
}
