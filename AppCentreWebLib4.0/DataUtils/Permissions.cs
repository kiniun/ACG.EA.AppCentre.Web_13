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
    public sealed class Permissions
    {
        public List<PERMISSION> LoadAllPermissions()
        {
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from perms in db.PERMISSIONs
                            orderby perms.PERMISSION_ID
                            select perms;
                return query.ToList<PERMISSION>();
            }
        }

        public List<string> LoadApplicationPermissions(string appId)
        {
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from perms in db.PERMISSIONs
                            where (perms.APPLICATION_ID == appId)
                            orderby perms.PERMISSION_ID
                            select perms.PERMISSION_ID;
                return query.ToList<string>();
            }
        }

        public bool AddPermissions(Permission perm)
        {
            var done = true;
            try
            {
                using(var db = new AppCentreEntities())
                {
                    db.PERMISSIONs.Add(new PERMISSION()
                    {
                        PERMISSION_ID = perm.PERMISSION_ID,
                        APPLICATION_ID = perm.APPLICATION_ID,
                        TARGET_ID = perm.TARGET_ID,
                        PERMISSION_NAME = perm.PERMISSION_NAME
                    });
                    db.SaveChanges();
                    return done;
                }
            }
            catch(Exception e)
            {
                EventLog.WriteEntry(perm.APPLICATION_ID, e.InnerException.Message, EventLogEntryType.Error);
                done = false;
            }
            return done;
        }

        public bool AddPermissions(Group_Permission perm)
        {
            var done = true;
            try
            {
                using (var db = new AppCentreEntities())
                {
                    db.GROUP_PERMISSION.Add(new GROUP_PERMISSION()
                    {
                        GROUP_ID = perm.GROUP_ID,
                        APPLICATION_ID = perm.APPLICATION_ID,
                        PERMISSION_ID = perm.PERMISSION_ID,
                        TARGET_VALUE_ID = perm.TARGET_VALUE_ID
                    });
                    db.SaveChanges();
                    return done;
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(perm.APPLICATION_ID, e.InnerException.Message, EventLogEntryType.Error);
                done = false;
            }
            return done;
        }

        public string AddUserPermissions(UserPermission perm)
        {
            var done = string.Empty;
                using (var db = new AppCentreEntities())
                {
                    if (perm != null)
                    {
                        foreach (var p in perm.permissions)
                        {
                            try
                            {
                                //TARGET_VALUE tgt = new TARGET_VALUE()
                                //{
                                //    TARGET_ID = perm.targetId,
                                //    TARGET_VALUE_ID = null;
                                //};
                                db.USER_PERMISSION.Add(new USER_PERMISSION()
                                                        {
                                                            USER_ID = Convert.ToInt32(perm.userId),
                                                            PERMISSION_ID = p.ToString(),
                                                            APPLICATION_ID = perm.applicationId,
                                                            GRANT = perm.grant
                                                        });
                            }
                            catch (Exception e)
                            {
                                EventLog.WriteEntry(perm.applicationId, e.InnerException.Message, EventLogEntryType.Error);
                                done = "i am the problem";
                            }
                        } 
                    }
                    else
                        done = "Empty model";
                    db.SaveChanges();
                    done = "User Permissions set!";
                }
            return done;
        }

        public static bool SetAppGroupPermission(Application_Group appGrp, AppCentreEntities db)
        {
            try
            {
                using (db)
                {
                    var permissions = db.PERMISSIONs.Where(p => p.APPLICATION_ID == appGrp.Application_Id);
                    var grpPermissions = db.GROUP_PERMISSION.Where(g => g.APPLICATION_ID == appGrp.Application_Id);
                    if (permissions.Any())
                    {
                        foreach (var g in grpPermissions)
                        {
                            foreach (var p in permissions)
                            {
                                if (g.PERMISSION_ID != p.PERMISSION_ID)
                                {
                                    db.GROUP_PERMISSION.Add(new GROUP_PERMISSION()
                                    {
                                        GROUP_ID = appGrp.Group_Id,
                                        APPLICATION_ID = appGrp.Application_Id,
                                        PERMISSION_ID = p.PERMISSION_ID,
                                        TARGET_VALUE_ID = p.TARGET_ID
                                    });
                                }
                            }
                        }
                        db.SaveChanges();
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddApplicationGroupPermissionSet(PermissionSet grpPermissions)
        {
            //var permissions = db.PERMISSIONs.Where(p => p.APPLICATION_ID == appGrp.Application_Id);
            using (var db = new AppCentreEntities())
            {
                foreach (var permId in grpPermissions.permissions)
                {
                    db.GROUP_PERMISSION.Add(new GROUP_PERMISSION()
                    {
                        GROUP_ID = grpPermissions.groupId,
                        APPLICATION_ID = grpPermissions.applicationId,
                        PERMISSION_ID = permId.ToString()
                    });
                }
                db.SaveChanges();
            }
            return true;
        }
    }
}
