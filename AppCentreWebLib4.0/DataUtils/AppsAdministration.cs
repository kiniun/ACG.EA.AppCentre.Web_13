using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACG.EA.AppCentre.Lib.DAL;
using System.Diagnostics;
using ACG.EA.AppCentre.Utils;
using ACG.EA.AppCentre.Lib.Models;

namespace ACG.EA.AppCentre.DataUtils
{
    public class AppsAdministration
    {

        public List<Application> LoadAllApplications()
        {
            using (var db = new AppCentreEntities())
            {                
                //disable the creation of proxy objects for EF's POCO objects.
                //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
                db.Configuration.ProxyCreationEnabled = false;
                var query = from apps in db.APPLICATIONs
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

        }

        public List<ApplicationProfile> LoadApplicationProfile(string app)
        {
            using (var db = new AppCentreEntities())
            {
                //disable the creation of proxy objects for EF's POCO objects.
                //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
                db.Configuration.ProxyCreationEnabled = false;
                var query = from apps in db.APPLICATION_GROUP
                            where (apps.APPLICAITON_ID == app)
                            orderby apps.APPLICAITON_ID
                            select new ApplicationProfile()
                            {
                                group_Id = apps.GROUP_ID,
                                group_Name = apps.GROUP_NAME,
                                ApplicationGrp = new Application()
                                {
                                    application_id = apps.APPLICATION.APPLICATION_ID,
                                    application_name = apps.APPLICATION.APPLICATION_NAME,
                                    application_desc = apps.APPLICATION.APPLICATION_DESC,
                                    application_uri = apps.APPLICATION.APPLICATION_URI,
                                    catalog_id = apps.APPLICATION.CATALOG.CATALOG_NAME,
                                }
                            };
                return query.ToList<ApplicationProfile>();
            }

        }

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
                EventLog.WriteEntry(newApp.application_id, ex.InnerException.Message, EventLogEntryType.Error);
                return false;
            }
            //return true;
        }

        public bool UpdateApplications(Application app)
        {
            try
            {
                using(var db = new AppCentreEntities())
                {
                    var getApp = db.APPLICATIONs.Find(app.application_id);
                    getApp.APPLICATION_NAME = app.application_name;
                    getApp.APPLICATION_URI = app.application_uri;
                    getApp.APPLICATION_DESC = app.application_desc;
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                EventLog.WriteEntry(app.application_id, e.InnerException.Message, EventLogEntryType.Error);
                return false;
            }
            return true;
        }

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
            catch(Exception e)
            {
                EventLog.WriteEntry(appId, e.InnerException.Message, EventLogEntryType.Error);
                return false;
            }
        }

        public List<Application> GetUserApplications(string userName)
        {
            var uApps = new List<Application>();
            using (var db = new AppCentreEntities())
            {
                var q = (from apps in db.USER_APPLICATION_GROUP
                         where (apps.USER.USER_NAME == userName)
                         select new Application()
                         {
                             application_id = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_ID,
                             application_name = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_NAME,
                             application_uri = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_URI,
                             application_desc = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_DESC
                         }).Distinct();

                uApps = q.ToList<Application>();
            }
            return uApps;
        }

        public List<User_Application> GetUsersApplicationsAlternative(string userName)
        {
            var uApps = new List<User_Application>();
            using(var db = new AppCentreEntities())
            {
                var q = (from apps in db.USER_APPLICATION_GROUP
                         where (apps.USER.USER_NAME == userName)
                         select new User_Application() {
                                groupId = apps.GROUP_ID,
                                App = new Application() {
                                    application_id = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_ID,
                                    application_desc =  apps.APPLICATION_GROUP.APPLICATION.APPLICATION_DESC,
                                    application_name = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_NAME,
                                    application_uri = apps.APPLICATION_GROUP.APPLICATION.APPLICATION_URI,
                                    catalog_id = apps.APPLICATION_GROUP.APPLICATION.CATALOG.CATALOG_NAME
                                },
                                userId = apps.USER_ID
                            }).Distinct();
                
                uApps = q.ToList<User_Application>();
            }
            return uApps;
        }

        public string AddApplicationGroup(Application_Group appGrp)
        {
            string result = string.Empty;
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
                        //if ((bool)appGrp.setPermission)
                        //{
                        //    if (Permissions.SetAppGroupPermission(appGrp, db))
                        //    {
                        //        result = "Application group and group permissions added!";
                        //    }
                        //    else
                        //    {
                        //        result = "There are no permissions set for this application. Add permissions then set group permissions";
                        //    }
                        //}
                        //else
                        //    result = "Group added, go to permissions tab to add permissions for group!";
                        result = "Application Group added";
                    }
                    else
                        result = "Group already exists for this App";
                }

            }
            catch(Exception e)
            {
                EventLog.WriteEntry(appGrp.Application_Id, e.InnerException.Message, EventLogEntryType.Error);
                result = "Exception occured in Admin library";
            }
            return result;
        }


        public List<Target> LoadApplicationTarget(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                //disable the creation of proxy objects for EF's POCO objects.
                //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
                db.Configuration.ProxyCreationEnabled = false;
                var query = from tv in db.TARGET_VALUE
                            where (tv.APPLICATION_ID == appId)
                            orderby tv.APPLICATION_ID
                            select new Target()
                            {
                                target_Id = tv.TARGET_ID,
                                target_name = tv.NAME
                            };
                return query.ToList<Target>();
            }

        }

        public List<string> LoadApplicationGroups(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                //disable the creation of proxy objects for EF's POCO objects.
                //if not disabled, EF creates proxies for the POCO objects. these proxy objects are not serializable.
                db.Configuration.ProxyCreationEnabled = false;
                var query = (from groups in db.APPLICATION_GROUP
                             where (groups.APPLICAITON_ID == appId)
                             orderby groups.GROUP_ID
                             select groups.GROUP_ID).Distinct();
                return query.ToList<string>();
            }

        }


        //application patch routine
        //nice to have
        //public bool PatchApp(string id, Delta<APPLICATION> newApp)
        //{
        //    using (AppCentreEntities db = new AppCentreEntities())
        //    {
        //        APPLICATION app = db.APPLICATIONs.SingleOrDefault(p => p.APPLICATION_ID == id);
        //        if (app == null)
        //        {
        //            return false;
        //        }
        //        newApp.Patch(app);
        //        db.SaveChanges();
        //    }
        //    return true;
        //}
    }
}
