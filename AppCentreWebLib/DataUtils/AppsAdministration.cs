using System;
using System.Collections.Generic;
using System.Linq;
using ACG.EA.AppCentre.Lib.DAL;
using System.Diagnostics;
using ACG.EA.AppCentre.Utils;
using ACG.EA.AppCentre.Lib.Models;

namespace ACG.EA.AppCentre.DataUtils
{
    public class AppsAdministration
    {

        /// <summary>
        /// search apps
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="app">application id</param>
        /// /// /// <param name="searchBy">search By</param>
        /// <returns>List of Applications</returns>
        public List<Application> SearchAppStores(string searchBy, string app)
        {
            using (var db = new AppCentreEntities())
            {
                //disable the creation of proxy objects for EF's POCO objects.
                //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
                db.Configuration.ProxyCreationEnabled = false;
                if (searchBy == "id")
                {
                    var query = from apps in db.APPLICATIONs
                                where (apps.APPLICATION_ID.Contains(app))
                                orderby apps.APPLICATION_ID
                                select new Application()
                                {
                                    application_id = apps.APPLICATION_ID,
                                    application_name = apps.APPLICATION_NAME,
                                    application_uri = apps.APPLICATION_URI,
                                    application_desc = apps.APPLICATION_DESC,
                                    catalog_id = apps.CATALOG.CATALOG_NAME
                                };
                    return query.ToList<Application>();
                }
                else if (searchBy == "name")
                {
                    var query = from apps in db.APPLICATIONs
                                where (apps.APPLICATION_NAME.Contains(app))
                                orderby apps.APPLICATION_ID
                                select new Application()
                                {
                                    application_id = apps.APPLICATION_ID,
                                    application_name = apps.APPLICATION_NAME,
                                    application_uri = apps.APPLICATION_URI,
                                    application_desc = apps.APPLICATION_DESC,
                                    catalog_id = apps.CATALOG.CATALOG_NAME
                                };
                    return query.ToList<Application>();
                }
                else
                    return null;
            }

        }

        /// <summary>
        /// load application's activity log
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application id</param>
        /// <returns>List of Applications</returns>
        public List<ACTIVITY_LOG> LoadActivityLog(string appId)
        {
            var log = new List<ACTIVITY_LOG>();
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var query = from l in db.ACTIVITY_LOG
                                where (l.APPLICATION_NAME == appId)
                                orderby l.ACTIVITY_DATE descending
                                select l;
                    log = query.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
            }
            return log;
        }

        /// <summary>
        /// add new application details
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="newApp">application object</param>
        /// <returns>bool</returns>
        public bool AddApplication(Application newApp)
        {
            try
            {
                using(var db = new AppCentreEntities())
                {
                    db.APPLICATIONs.Add(new APPLICATION()
                    {
                        APPLICATION_ID = newApp.application_id,
                        APPLICATION_NAME = newApp.application_name,
                        APPLICATION_URI = newApp.application_uri,
                        APPLICATION_DESC = newApp.application_desc,
                        CATALOG_ID = newApp.catalog_id
                    });
                    db.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                return false;
            }
            //return true;
        }

        public bool CheckAppExists(string appId)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var usr = db.APPLICATIONs.Where(a => a.APPLICATION_ID == appId).FirstOrDefault();
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
        /// update application's details
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="App">application object</param>
        /// <returns>bool</returns>
        public bool UpdateApplication(Application App)
        {
            try
            {
                using (var db = new AppCentreEntities())
                {                    
                    //var currentApp = db.APPLICATIONs.Find(App.application_id);
                    APPLICATION currentApp = db.APPLICATIONs.First(a => a.APPLICATION_ID == App.application_id);
                    if (currentApp != null)
                    {
                        currentApp.APPLICATION_NAME = App.application_name;
                        currentApp.APPLICATION_URI = App.application_uri;
                        currentApp.APPLICATION_DESC = App.application_desc; 
                    }
                    //currentApp.CATALOG_ID = App.catalog_id;
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
        /// removed application
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application Id</param>
        /// <returns>bool</returns>
        public bool RemoveApplication(string appId)
        {
            try
            {
                using(var db = new AppCentreEntities())
                {
                    var app = db.APPLICATIONs.Find(appId);
                    db.APPLICATIONs.Remove(app);
                    db.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                return false;
            }
        }

        /// <summary>
        /// retrieves list of applications for supplied user. Gets all applications if IsAdmin is true.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="userName">requeseting user (logged on user)</param>
        /// /// /// <param name="IsAdmin">is an administratorof appcentre</param>
        /// <returns>List of Applications</returns>
        public List<ApplicationsByCatalog> GetUserApplicationsByCatalog(string userName, bool IsAdmin)
        {
            var uApps = new List<ApplicationsByCatalog>();
            IQueryable<APPLICATION> sApps;
            using (var db = new AppCentreEntities())
            {
                //if (!IsAdmin)
                //{
                //    sApps = (from v in db.VW_USER_PERMISSION
                //                 join a in db.APPLICATIONs
                //                 on v.APPLICATION_ID equals a.APPLICATION_ID
                //                 where (v.USER_NAME == userName)
                //                 orderby a.CATALOG_ID ascending
                //                 select a).Distinct().OrderBy(a => a.CATALOG_ID); 
                //}
                //else
                //{
                //    sApps = (from v in db.APPLICATIONs
                //             orderby v.CATALOG_ID ascending
                //             select v).Distinct().OrderBy(a => a.CATALOG_ID);
                //}

                sApps = (from v in db.VW_USER_PERMISSION
                         join a in db.APPLICATIONs
                         on v.APPLICATION_ID equals a.APPLICATION_ID
                         where (v.USER_NAME == userName)
                         orderby a.CATALOG_ID ascending
                         select a).Distinct().OrderBy(a => a.CATALOG_ID); 

                var cats = (from c in db.CATALOGs
                            orderby c.CATALOG_ID ascending
                            select c).Distinct().OrderBy(a => a.CATALOG_ID);

                if (sApps.Any())
                {
                    string cat = string.Empty;
                    string catName = string.Empty;
                    List<Application> myApps = new List<Application>();
                    int catCount = 0;
                    int appsCount = sApps.Count();
                    foreach (var c in cats)
                    {
                        catCount = sApps.Where(p => c.CATALOG_ID == p.CATALOG_ID).Count();
                        foreach (var item in sApps)
                        {
                            if (item.CATALOG_ID == null)
                                item.CATALOG_ID = "Default";
                            if (c.CATALOG_ID == item.CATALOG_ID)
                            {
                                if (myApps == null)
                                    myApps = new List<Application>();
                                myApps.Add(new Application
                                {
                                    application_id = item.APPLICATION_ID,
                                    application_name = item.APPLICATION_NAME,
                                    application_uri = item.APPLICATION_URI,
                                    application_desc = item.APPLICATION_DESC
                                });
                                catCount--;
                                if(catCount == 0)
                                {
                                    uApps.Add(new ApplicationsByCatalog
                                    {
                                        catalog_name = c.CATALOG_ID ?? item.CATALOG_ID,
                                        catalog_id = c.CATALOG_NAME ?? item.CATALOG.CATALOG_NAME,
                                        apps = myApps.OrderBy(ap => ap.application_id).ToList<Application>()
                                    });
                                    myApps = null;
                                    break;
                                }
                            }
                        }
                    } 
                }
            }
            return uApps;
        }

        /// <summary>
        /// retrieves list of applications that user is an administrator
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="userName">requesting user (logged on user)</param>
        /// /// /// <param name="permId">permission set in the AppCentre web.config. This must match with the administrator type in dB (Admin or Administrator)</param>
        /// /// /// <param name="IsAdmin">is an administratorof appcentre</param>
        /// <returns>List of Applications</returns>
        public List<Application> GetAdminApplications(string userName, bool IsAdmin, string permId)
        {
            var aApps = new List<Application>();
            using (var db = new AppCentreEntities())
            {
                if (IsAdmin)
                {
                    var q = (from apps in db.APPLICATIONs
                             orderby apps.APPLICATION_ID
                             select new Application()
                             {
                                 application_id = apps.APPLICATION_ID,
                                 application_name = apps.APPLICATION_NAME,
                                 application_uri = apps.APPLICATION_URI,
                                 application_desc = apps.APPLICATION_DESC
                             }).Distinct();
                    aApps = q.ToList<Application>();
                    
                }
                else
                {
                    var apps = (from uGroup in db.USER_APPLICATION_GROUP
                                where (uGroup.USER.USER_NAME == userName && uGroup.GROUP_ID == "Administrator")
                                select new Application()
                                 {
                                     application_id = uGroup.APPLICATION_ID,
                                     application_name = uGroup.APPLICATION_GROUP.APPLICATION.APPLICATION_NAME,
                                     application_uri = uGroup.APPLICATION_GROUP.APPLICATION.APPLICATION_URI,
                                     application_desc = uGroup.APPLICATION_GROUP.APPLICATION.APPLICATION_DESC
                                 }
                                ).Union(
                                from perm in db.USER_PERMISSION
                                where (perm.USER.USER_NAME == userName && perm.PERMISSION_ID == permId)
                                select new Application()
                                 {
                                     application_id = perm.APPLICATION_ID,
                                     application_name = perm.PERMISSION.APPLICATION.APPLICATION_NAME,
                                     application_uri = perm.PERMISSION.APPLICATION.APPLICATION_URI,
                                     application_desc = perm.PERMISSION.APPLICATION.APPLICATION_DESC
                                 }).Distinct();
                    aApps = apps.ToList<Application>();
                }
            }
            return aApps;
        }

        /// <summary>
        /// add new application group
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appGrp">Application_Group object</param>
        /// <returns>bool</returns>
        public bool AddApplicationGroup(Application_Group appGrp)
        {
            var result = false;
            try
            {
                using (var db = new AppCentreEntities())
                {
                    var groups = db.APPLICATION_GROUP.Where(g => g.APPLICAITON_ID == appGrp.Application_Id && g.GROUP_ID == appGrp.Group_Id) ?? null;
                    if (!groups.Any())
                    {
                        db.APPLICATION_GROUP.Add(new APPLICATION_GROUP()
                                    {
                                        GROUP_ID = appGrp.Group_Id,
                                        APPLICAITON_ID = appGrp.Application_Id,
                                        GROUP_NAME = appGrp.Group_Name
                                    });
                        db.SaveChanges();                        
                        result = true;
                    }
                }

            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, "AppCentre");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// retrieves list of target related to the supplied application
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application id</param>
        /// <returns>List of Target</returns>
        public List<Target> LoadApplicationTarget(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from tv in db.TARGETs
                            where (tv.APPLICATION_ID == appId)
                            orderby tv.APPLICATION_ID
                            select new Target()
                            {
                                target_Id = tv.TARGET_ID,
                                target_name = tv.TARGET_NAME
                            };
                return query.ToList<Target>();
            }

        }

        /// <summary>
        /// retrieves list of target values related to the supplied application
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application id</param>
        /// <returns>List of Target values</returns>
        public List<Target> LoadAppTargetValues(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from tv in db.TARGET_VALUE
                            where (tv.APPLICATION_ID == appId)
                            orderby tv.APPLICATION_ID
                            select new Target()
                            {
                                target_Id = tv.TARGET_VALUE_ID,
                                target_name = tv.NAME
                            };
                return query.ToList<Target>();
            }
        }

        /// <summary>
        /// retrieves list of groups for the requested applicaiton
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application id</param>
        /// <returns>List of Groups</returns>
        public List<Groups> LoadApplicationGroups(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = (from groups in db.APPLICATION_GROUP
                             where (groups.APPLICAITON_ID == appId)
                             orderby groups.GROUP_ID
                             select new Groups()
                             {
                                 group_Id = groups.GROUP_ID,
                                 groupDescription = groups.GROUP_NAME
                             }).Distinct();
                return query.ToList<Groups>();
            }

        }

        /// <summary>
        /// removes provided group from application groups
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="appId">application id</param>
        /// /// /// <param name="groupId">group id</param>
        /// <returns>bool</returns>
        public bool RemoveApplicationGroup(string appId, string groupId)
        {
            using (var db = new AppCentreEntities())
            {
                int modified = 0;
                using(var dbContextTransaction = db.Database.BeginTransaction()) {
                    try
                    {
                        var toDelte = db.APPLICATION_GROUP.FirstOrDefault(g => g.APPLICAITON_ID == appId && g.GROUP_ID == groupId);
                        var grpPermissions = from gp in db.GROUP_PERMISSION
                                             where (gp.APPLICATION_ID == appId && gp.GROUP_ID == groupId)
                                             select gp;
                        if (grpPermissions != null)
                        {
                            foreach (var permission in grpPermissions)
                            {
                                db.GROUP_PERMISSION.Remove(permission);
                                modified++;
                            }
                        }
                        var usrGroups = from up in db.USER_APPLICATION_GROUP
                                        where (up.APPLICATION_ID == appId && up.GROUP_ID == groupId)
                                        select up;
                        if (usrGroups != null)
                        {
                            foreach (var usrGroup in usrGroups)
                            {
                                db.USER_APPLICATION_GROUP.Remove(usrGroup);
                                modified++;
                            }
                        }
                        if (modified > 0)
                        {
                            if (toDelte != null)
                            {
                                db.APPLICATION_GROUP.Remove(toDelte);
                                modified++;
                            }
                        }
                        else if (grpPermissions == null && usrGroups == null && modified == 0)
                        {
                            if (toDelte != null)
                            {
                                db.APPLICATION_GROUP.Remove(toDelte);
                                modified++;
                            }
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        ExceptionHandler.LogException(ex, "AppCentre");
                        return false;
                    }
                    if (modified > 0)
                    {
                        return true;
                    }
                    else
                        return false;
                }                
            }
        }

        /// <summary>
        /// retrieves filtered list of applications for this user based on search
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// /// <param name="userName">current user logged in</param>
        /// /// /// <param name="searchBy">search by</param>
        /// /// /// <param name="txt">search text</param>
        /// <returns>List of Applications</returns>
        public List<Application> SearchUserApplications(string userName, string searchBy, string txt)
        {
            var uApps = new List<Application>();
            using (var db = new AppCentreEntities())
            {
                if (searchBy == "Keyword")
                {
                    var q = (from v in db.VW_USER_PERMISSION
                             join a in db.APPLICATIONs
                             on v.APPLICATION_ID equals a.APPLICATION_ID
                             where (v.USER_NAME == userName && a.APPLICATION_DESC.Contains(txt))
                             select new Application()
                             {
                                 application_id = a.APPLICATION_ID,
                                 application_name = a.APPLICATION_NAME,
                                 application_uri = a.APPLICATION_URI,
                                 application_desc = a.APPLICATION_DESC
                             }).Distinct();
                    uApps = q.ToList<Application>();
                }
                else if (searchBy == "Name")
                {
                    var q = (from v in db.VW_USER_PERMISSION
                             join a in db.APPLICATIONs
                             on v.APPLICATION_ID equals a.APPLICATION_ID
                             where (v.USER_NAME == userName && a.APPLICATION_NAME.Contains(txt))
                             select new Application()
                             {
                                 application_id = a.APPLICATION_ID,
                                 application_name = a.APPLICATION_NAME,
                                 application_uri = a.APPLICATION_URI,
                                 application_desc = a.APPLICATION_DESC
                             }).Distinct();
                    uApps = q.ToList<Application>();
                }

            }
            return uApps;
        }
        
    }
}
