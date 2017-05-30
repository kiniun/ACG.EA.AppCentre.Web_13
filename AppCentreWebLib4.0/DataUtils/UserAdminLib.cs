using ACG.EA.AppCentre.Lib.DAL;
using ACG.EA.AppCentre.Lib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACG.EA.AppCentre.DataUtils
{
        
    public class UserAdminLib
    {
        public UserAdminLib()
        {  }
                
        public List<VW_USER_PERMISSION> GetUserPermissionProfile(string user, string appId)
        {
            using (var db = new AppCentreEntities())
            {
                //disable proxy object creation for to allow serialization
                db.Configuration.ProxyCreationEnabled = false;
                var query = from perms in db.VW_USER_PERMISSION
                            orderby perms.PERMISSION_ID
                            where (perms.USER_NAME == user && perms.APPLICATION_ID == appId)
                            select perms;
                return query.ToList<VW_USER_PERMISSION>();
            }
        }


        public List<User> GetAppAdministrators(string appName, string grp)
        {
            using (var db = new AppCentreEntities())
            {
                var users = new List<User>();
                var query = (from apGrp in db.USER_APPLICATION_GROUP
                            where (apGrp.APPLICATION_ID == appName && apGrp.GROUP_ID == grp)
                            select apGrp.USER).Distinct();
                foreach(var u in query)
                {
                    users.Add(new User
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
                return users; 
            }
        }

        
        public List<User> LoadAllAppUsers()
        {
            var db = new AppCentreEntities();
            var users = new List<User>();
            //disable the creation of proxy objects for EF's POCO objects.
            //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
            db.Configuration.ProxyCreationEnabled = false;
            var query = from aUsers in db.USERs
                        orderby aUsers.USER_NAME
                        select aUsers;

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

            return users.ToList();

        }

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
                EventLog.WriteEntry("AppCentre", ex.InnerException.ToString(), EventLogEntryType.Error);
                return false;
            }
            return true;
        }


        public bool UpdateUser(User usr)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    if (usr.User_Id != null)
                    {
                        var u = db.USERs.Find(usr.User_Id);
                        u.LAST_NAME = u.LAST_NAME != usr.Last_Name ? (usr.Last_Name ?? u.LAST_NAME) : u.LAST_NAME;
                        u.FIRST_NAME = usr.First_Name ?? u.LAST_NAME;
                        u.LAST_MODIFIED = DateTime.Now;
                        u.USER_NAME = usr.User_Name ?? u.USER_NAME;
                        u.EMAIL = usr.Email ?? u.EMAIL;
                        u.PHONE = usr.Phone ?? u.PHONE;
                        u.TITLE = usr.Title ?? u.TITLE; 
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("AppCentre", ex.InnerException.ToString(), EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        public bool RemoveUser(int user_id)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var u = db.USERs.Find(user_id);
                    db.USERs.Remove(u);
                    var up = db.USER_PERMISSION.Find(user_id);
                    db.USER_PERMISSION.Remove(up);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("AppCentre", ex.InnerException.ToString(), EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        public List<User_Permission> LoadUserPermissions(int userId)
        {
            var perms = new List<User_Permission>();
            try
            {
                using(var db = new AppCentreEntities())
                {
                    var query = from p in db.USER_PERMISSION
                                where (p.USER_ID == userId)
                                orderby p.PERMISSION_ID
                                select new User_Permission()
                                {
                                    user_permission_Id = p.USER_PERMISSION_ID, user_Id = p.USER_ID,
                                    permission_Id = p.PERMISSION_ID, application_Id = p.APPLICATION_ID,
                                    target_value_Id = p.TARGET_VALUE_ID, grant = p.GRANT
                                };
                    perms = query.ToList();
                }
            }
            catch(Exception e)
            {
                EventLog.WriteEntry("AppCentre", e.InnerException.ToString(), EventLogEntryType.Error);
            }
            return perms;
        }

        public List<ACTIVITY_LOG> LoadActivityLog(string user)
        {
            var log = new List<ACTIVITY_LOG>();
            try
            {
                using(var db = new AppCentreEntities())
                {
                    var query = from l in db.ACTIVITY_LOG
                                where (l.USER_NAME == user)
                                orderby l.ACTIVITY_DATE descending
                                select l;
                    log = query.ToList();
                }
            }
            catch(Exception ex)
            {
                EventLog.WriteEntry("AppCentre", ex.InnerException.ToString(), EventLogEntryType.Error);
            }
            return log;
        }
        
        public string AddUserApplicationGrp(UserApplicationGroup userGroup)
        {
            var result = string.Empty;
            using (var db = new AppCentreEntities())
            {    
                var grps = db.USER_APPLICATION_GROUP.Where(g => g.APPLICATION_ID == userGroup.application_Id && g.USER_ID == userGroup.user_Id) ?? null;            
                foreach (var gUser in userGroup.groups)
                {
                    if (grps.Any())
                    {
                        foreach(var gDB in grps) {
                            if (gUser != gDB.GROUP_ID)
                            {
                                try
                                {
                                    db.USER_APPLICATION_GROUP.Add(new USER_APPLICATION_GROUP()
                                                                {
                                                                    USER_ID = userGroup.user_Id,
                                                                    GROUP_ID = gUser.ToString(),
                                                                    APPLICATION_ID = userGroup.application_Id
                                                                });
                                    result = "success";
                                }
                                catch (Exception ex)
                                {
                                    EventLog.WriteEntry(userGroup.application_Id, ex.InnerException.Message, EventLogEntryType.Error);
                                    return result = ex.InnerException.Message;
                                };  
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            db.USER_APPLICATION_GROUP.Add(new USER_APPLICATION_GROUP()
                            {
                                USER_ID = userGroup.user_Id,
                                GROUP_ID = gUser.ToString(),
                                APPLICATION_ID = userGroup.application_Id
                            });
                            result = "success";
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry(userGroup.application_Id, ex.InnerException.Message, EventLogEntryType.Error);
                            return result = ex.InnerException.Message;
                        };
                    }
                }
                db.SaveChanges();
            }
            return result;
        }

        public List<UserPermissionProfile> GetApplicationUsers(string appId)
        {
            var uApps = new List<UserPermissionProfile>();
            using (var db = new AppCentreEntities())
            {
                var q = (from apps in db.VW_USER_PERMISSION
                         where (apps.APPLICATION_ID == appId)
                         select new
                         {
                             user = apps.USER_NAME,
                             app = apps.APPLICATION_ID,
                             permission = apps.PERMISSION_ID,
                             target = apps.VALUE
                         }).Distinct();

                var query = (from users in db.USERs
                             orderby users.USER_ID
                             select users).Distinct();

                foreach (var user in query)
                {
                    foreach (var uId in q)
                    {
                        if (user.USER_NAME == uId.user)
                        {
                            uApps.Add(new UserPermissionProfile()
                            {
                                user_Id = user.USER_ID,
                                User_Name = user.USER_NAME,
                                First_Name = user.FIRST_NAME,
                                Last_Name = user.LAST_NAME,
                                permission_Id = uId.permission,
                                application_Id = uId.app,
                                target_value = uId.target,
                                grant = uId.permission != string.Empty
                            });
                        }
                    }

                }
                return uApps;
            }
        }

        public bool UpdateUsersPermissions(UserPermissionProfile usr)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {

                }
            }
            catch(Exception e)
            {

                return false;
            }
            return true;
        }
    }
}
