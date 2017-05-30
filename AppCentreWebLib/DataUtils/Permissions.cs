using System;
using System.Collections.Generic;
using System.Linq;
using ACG.EA.AppCentre.Lib.DAL;
using System.Diagnostics;
using ACG.EA.AppCentre.Utils;
using ACG.EA.AppCentre.Lib.Models;
using System.Data.Entity.SqlServer;

namespace ACG.EA.AppCentre.DataUtils
{
    public sealed class Permissions
    {
        /// <summary>
        /// permissions and targets list for the requested group.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="groupId">group id</param>
        /// /// <param name="appId">application id</param>
        /// <returns>a list of Group_Permission objects</returns>
        public List<Group_Permission> LoadAppGroupPermissionNTargets(string groupId, string appId)
        {
            var permissionView = new List<Group_Permission>();
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {
                    var permTargets = (from p in db.PERMISSIONs
                                       join t in db.TARGET_VALUE
                                       on p.APPLICATION_ID equals t.APPLICATION_ID into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId && p.TARGET_ID != null)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.TARGET_VALUE_ID,
                                           targetValueName = sub.NAME,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();
                    var permissions = (from p in db.PERMISSIONs
                                       join t in permTargets
                                           on new { applicationId = p.APPLICATION_ID, permissionId = p.PERMISSION_ID } equals new { t.applicationId, t.permissionId } into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.targetValueId,
                                           targetValueName = sub.targetValueName,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();
                    var grpPermissions = from g in db.GROUP_PERMISSION
                                         where (g.APPLICATION_ID == appId && g.GROUP_ID == groupId)
                                         select g;
                    var multi = from p in permissions
                                join g in grpPermissions
                                on new { p.applicationId, p.permissionId, p.targetValueId } equals new { applicationId = g.APPLICATION_ID, permissionId = g.PERMISSION_ID, targetValueId = g.TARGET_VALUE_ID }
                                into gp
                                from sub in gp.DefaultIfEmpty()
                                select new Group_Permission
                                {
                                    GROUP_PERMISSSION_ID = sub.GROUP_PERMISSSION_ID,
                                    GROUP_ID = sub.GROUP_ID,
                                    APPLICATION_ID = p.applicationId,
                                    PERMISSION_ID = p.permissionId,
                                    PERMISSION_NAME = p.permissionName,
                                    TARGET_VALUE = p.targetValueName,
                                    TARGET_VALUE_ID = p.targetValueId,
                                    isGranted = sub.GROUP_PERMISSSION_ID != null
                                };
                    var nullTargets = multi.Where(p => p.TARGET_VALUE_ID == null).OrderBy(p => p.PERMISSION_NAME).ToList<Group_Permission>();
                    var nonnullTargets = multi.Where(p => p.TARGET_VALUE_ID != null).OrderBy(p => p.PERMISSION_NAME).ToList<Group_Permission>();
                    permissionView = nullTargets.Union(nonnullTargets).ToList<Group_Permission>();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "AppCentre");
                }
                return permissionView;
            }
        }

        public List<Group_Permission> LoadAppGroupPermissions(string groupId, string appId)
        {
            var permissionView = new List<Group_Permission>();
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {
                    var permissions = from p in db.PERMISSIONs
                                      where (p.APPLICATION_ID == appId)
                                      select p;
                    var grpPermissions = from g in db.GROUP_PERMISSION
                                         where (g.APPLICATION_ID == appId && g.GROUP_ID == groupId)
                                         select g;
                    var multi = from p in permissions
                                join g in grpPermissions
                                on new { p.APPLICATION_ID, p.PERMISSION_ID } equals new { g.APPLICATION_ID, g.PERMISSION_ID } into gp
                                from sub in gp.DefaultIfEmpty()
                                select new Group_Permission
                                {
                                    GROUP_PERMISSSION_ID = sub.GROUP_PERMISSSION_ID,
                                    GROUP_ID = sub.GROUP_ID,
                                    APPLICATION_ID = p.APPLICATION_ID,
                                    PERMISSION_ID = p.PERMISSION_ID,
                                    TARGET_VALUE = sub.TARGET_VALUE.NAME,
                                    TARGET_VALUE_ID = sub.TARGET_VALUE_ID,
                                    isGranted = sub.PERMISSION_ID != null
                                };

                    permissionView = multi.ToList<Group_Permission>();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "AppCentre");
                }

                return permissionView;
            }
        }

        /// <summary>
        /// Updates a particular application's group's permissions.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="groupSet">check GroupPermissionSet object definition for members</param>
        /// <returns>bool: success/failure</returns>
        public bool UpdateGroupPermissions(GroupPermissionSet groupSet)
        {
            var modified = 0;
            using(var db = new AppCentreEntities())
            {
                for (var i = 0; i < groupSet.permissions.Count(); i++ )
                {
                    try
                    {
                        int g = groupSet.grpPermId[i];
                        if (g != 0)
                        {
                            if (!groupSet.isGranted[i])
                            {
                                var grp = db.GROUP_PERMISSION.FirstOrDefault(p => p.GROUP_PERMISSSION_ID == g);
                                db.GROUP_PERMISSION.Remove(grp);
                                modified++;
                            }
                        }
                        else if (g == 0)
                        {
                            if (groupSet.isGranted[i])
                            {                           
                                int tg = Convert.ToInt32(groupSet.targets[i]);
                                if (tg != 0)
                                {
                                    TARGET_VALUE tar = db.TARGET_VALUE.Find(tg);
                                    if (tar != null)
                                    {
                                        db.GROUP_PERMISSION.Add(new GROUP_PERMISSION
                                        {
                                            GROUP_ID = groupSet.groupId,
                                            APPLICATION_ID = groupSet.applicationId,
                                            PERMISSION_ID = Convert.ToString(groupSet.permissions[i]),
                                            TARGET_VALUE_ID = groupSet.targets[i]
                                        });
                                    }
                                    else
                                    {
                                        db.GROUP_PERMISSION.Add(new GROUP_PERMISSION
                                        {
                                            GROUP_ID = groupSet.groupId,
                                            APPLICATION_ID = groupSet.applicationId,
                                            PERMISSION_ID = Convert.ToString(groupSet.permissions[i])
                                        });
                                    }
                                    modified++;
                                }
                                else
                                {
                                    db.GROUP_PERMISSION.Add(new GROUP_PERMISSION
                                    {
                                        GROUP_ID = groupSet.groupId,
                                        APPLICATION_ID = groupSet.applicationId,
                                        PERMISSION_ID = Convert.ToString(groupSet.permissions[i])
                                    });
                                    modified++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex, "AppCentre");
                        return false;
                    }
                }
                db.SaveChanges();
            }
            if (modified > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// User's permission update.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="permSet">check UserGroupPermissionSet object definition for members</param>
        /// <returns>bool: success/failure</returns>
        public bool UpdateUserPermissions_V1(UserGroupPermissionSet permSet)
        {
            var modified = 0;
            var userId = Convert.ToInt16(permSet.userId);
            using (var db = new AppCentreEntities())
            {
                for (var i = 0; i < permSet.permissions.Count(); i++)
                {
                    try
                    {
                        int g = Convert.ToInt32(permSet.usrPermId[i]);
                        int grP = Convert.ToInt32(permSet.usrGrpPermId[i]);
                        var perm = Convert.ToString(permSet.permissions[i]);
                        var pState = permSet.permState[i].ToString();

                        if(pState == "explicit")
                        {
                            if(g == 0)
                            {
                                TARGET_VALUE tar = null;
                                if (permSet.targets[i] != 0)
                                {
                                    tar = db.TARGET_VALUE.Find(permSet.targets[i]);
                                }
                                if (tar != null)
                                {
                                    db.USER_PERMISSION.Add(new USER_PERMISSION
                                    {
                                        USER_ID = userId,
                                        APPLICATION_ID = permSet.applicationId,
                                        PERMISSION_ID = perm,
                                        TARGET_VALUE_ID = permSet.targets[i],
                                        GRANT = true
                                    });
                                    modified++;
                                }
                                else
                                {
                                    db.USER_PERMISSION.Add(new USER_PERMISSION
                                    {
                                        USER_ID = userId,
                                        APPLICATION_ID = permSet.applicationId,
                                        PERMISSION_ID = perm,
                                        GRANT = true
                                    });
                                    modified++;
                                }
                            }
                            else
                            {
                                if (grP != 0)
                                {
                                    var usrP = db.USER_PERMISSION.Find(g);
                                    if (usrP != null)
                                    {
                                        usrP.GRANT = true;
                                        modified++;
                                    }
                                }
                            }
                        }
                        else if (pState == "inherited")
                        {
                            if (g != 0)
                            {
                                var usrP = db.USER_PERMISSION.Find(g);
                                if (usrP != null)
                                {
                                    db.USER_PERMISSION.Remove(usrP);
                                    modified++;
                                }
                            }
                        }
                        else if (pState == "none")
                        {
                            if (grP != 0)
                            {
                                if (g == 0)
                                {
                                    TARGET_VALUE tar = null;
                                    if (permSet.targets[i] != 0)
                                    {
                                        tar = db.TARGET_VALUE.Find(permSet.targets[i]);
                                    }
                                    if (tar != null)
                                    {
                                        db.USER_PERMISSION.Add(new USER_PERMISSION
                                        {
                                            USER_ID = userId,
                                            APPLICATION_ID = permSet.applicationId,
                                            PERMISSION_ID = perm,
                                            TARGET_VALUE_ID = permSet.targets[i],
                                            GRANT = false
                                        });
                                        modified++;
                                    }
                                    else
                                    {
                                        db.USER_PERMISSION.Add(new USER_PERMISSION
                                        {
                                            USER_ID = userId,
                                            APPLICATION_ID = permSet.applicationId,
                                            PERMISSION_ID = perm,
                                            GRANT = false
                                        });
                                        modified++;
                                    } 
                                }
                                else
                                {
                                    var usrP = db.USER_PERMISSION.Find(g);
                                    if (usrP != null)
                                    {
                                        usrP.GRANT = false;
                                        modified++;
                                    }
                                }
                            }
                            else
                            {
                                if (g != 0)
                                {
                                    var usrP = db.USER_PERMISSION.Find(g);
                                    if (usrP != null && grP == 0)
                                    {
                                        db.USER_PERMISSION.Remove(usrP);
                                        modified++;
                                    }
                                }
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
        /// User's permission list.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="user">user id</param>
        /// /// <param name="appId">application id</param>
        /// <returns>List of users group and each group</returns>
        public List<UserGroup_Permission> LoadPermsForUser_V1(int user, string appId)
        {
            var usrPermissionView = new List<UserGroup_Permission>();
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {
                    var permTargets = (from p in db.PERMISSIONs
                                       join t in db.TARGET_VALUE
                                       on p.APPLICATION_ID equals t.APPLICATION_ID into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId && p.TARGET_ID != null)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.TARGET_VALUE_ID,
                                           targetValueName = sub.NAME,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();
                    var permissions = (from p in db.PERMISSIONs
                                       join t in permTargets
                                           on new { applicationId = p.APPLICATION_ID, permissionId = p.PERMISSION_ID } equals new { t.applicationId, t.permissionId } into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.targetValueId,
                                           targetValueName = sub.targetValueName,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();
                    //var permissions = (from p in db.PERMISSIONs
                    //                   join t in db.TARGET_VALUE
                    //                   on p.APPLICATION_ID equals t.APPLICATION_ID into gp
                    //                   from sub in gp.DefaultIfEmpty()
                    //                   where (p.APPLICATION_ID == appId)
                    //                   select new AllPermissions
                    //                   {
                    //                       permissionId = p.PERMISSION_ID,
                    //                       permissionName = p.PERMISSION_NAME,
                    //                       applicationId = p.APPLICATION_ID,
                    //                       targetId = p.TARGET_ID,
                    //                       target = p.TARGET.TARGET_NAME,
                    //                       targetValueId = sub.TARGET_VALUE_ID,
                    //                       targetValueName = sub.NAME,
                    //                       groupEnabled = p.GROUP_ENABLED
                    //                   });

                    var usrPermissions = (from g in db.USER_PERMISSION
                                          where (g.APPLICATION_ID == appId && g.USER_ID == user)
                                          select new
                                          {
                                              application_Id = g.APPLICATION_ID,
                                              user_Id = g.USER_ID,
                                              user_permission_Id = g.USER_PERMISSION_ID,
                                              groupEnabled = false,
                                              grant = g.GRANT ? "Explicit" : "None",
                                              permission_Id = g.PERMISSION_ID,
                                              permission_Name = string.Empty,
                                              targetValueId = g.TARGET_VALUE_ID,
                                              group_permission_Id = 0
                                          }).Union
                                          (
                                         from uG in db.USER_APPLICATION_GROUP
                                         join gp in db.GROUP_PERMISSION
                                         on new { uG.GROUP_ID, uG.APPLICATION_ID } equals new { gp.GROUP_ID, gp.APPLICATION_ID }
                                         where (uG.APPLICATION_ID == appId && uG.USER_ID == user)
                                         select new
                                         {
                                             application_Id = uG.APPLICATION_ID,
                                             user_Id = uG.USER_ID,
                                             user_permission_Id = 0,
                                             groupEnabled = false,
                                             grant = "Group",
                                             permission_Id = gp.PERMISSION_ID,
                                             permission_Name = string.Empty,
                                             targetValueId = gp.TARGET_VALUE_ID,
                                             group_permission_Id = gp.GROUP_PERMISSSION_ID
                                         }
                                         );

                    var multi = (from p in permissions
                                 join g in usrPermissions
                                 on new { applicationId = p.applicationId, permissionId = p.permissionId, p.targetValueId } equals new { applicationId = g.application_Id, permissionId = g.permission_Id, g.targetValueId } into gp
                                 from sub in gp.DefaultIfEmpty()
                                 select new UserGroup_Permission
                                 {
                                     application_Id = p.applicationId,
                                     user_Id = sub.user_Id,
                                     user_permission_Id = sub.user_permission_Id,
                                     groupEnabled = p.groupEnabled ?? false,
                                     grant = sub.permission_Id != null ? sub.grant : "None",
                                     targetId = p.targetId,
                                     target = p.target,
                                     targetValueId = p.targetValueId,
                                     targetValue = p.targetValueName,
                                     permission_Id = p.permissionId,
                                     //permission_Name = sub.permission_Name ?? p.permissionName,
                                     permission_Name = p.permissionName,
                                     group_permission_Id = sub.group_permission_Id
                                 }).Where(p => !p.groupEnabled).OrderBy(p => p.permission_Id);
                    
                    if (multi.Any())
                    {
                        var multiList = multi.ToList();
                        foreach (var perm in multi)
                        {
                            for (int i = 0; i < multiList.Count(); i++)
                            {
                                if (perm.permission_Id == multiList[i].permission_Id)
                                {
                                    if (perm.grant == "Explicit")
                                    {
                                        if (usrPermissionView.Any())
                                        {
                                            UserGroup_Permission u;
                                            if (perm.targetValue != null)
                                            {
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id && p.targetValue == perm.targetValue).SingleOrDefault();
                                            }
                                            else
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id).SingleOrDefault();
                                            if (u == null)
                                            {
                                                usrPermissionView.Add(perm);
                                                break;
                                            }
                                            else
                                            {
                                                usrPermissionView.Remove(u);
                                                perm.group_permission_Id = u.group_permission_Id;
                                                usrPermissionView.Add(perm);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            usrPermissionView.Add(perm);
                                            break;
                                        }
                                    }
                                    else if (perm.grant == "Group")
                                    {
                                        if (usrPermissionView.Any())
                                        {
                                            IEnumerable<UserGroup_Permission> u;
                                            if (perm.targetValue != null)
                                            {
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id && p.targetValueId == perm.targetValueId);
                                            }
                                            else
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id);
                                            if (!u.Any())
                                            {
                                                usrPermissionView.Add(perm);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            usrPermissionView.Add(perm);
                                            break;
                                        }
                                    }
                                    else if (perm.grant == "None")
                                    {
                                        if (usrPermissionView.Any())
                                        {
                                            UserGroup_Permission uP = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id && p.grant == "Group" && p.targetValue == perm.targetValue).SingleOrDefault();
                                            
                                            if (uP == null)
                                            {
                                                usrPermissionView.Add(perm);
                                                break;
                                            }
                                            else
                                            {
                                                usrPermissionView.Remove(uP);
                                                perm.group_permission_Id = uP.group_permission_Id;
                                                usrPermissionView.Add(perm);
                                               
                                            IEnumerable<UserGroup_Permission> u;
                                            if (perm.targetValue != null)
                                            {
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id && p.targetValueId == perm.targetValueId);
                                            }
                                            else
                                                u = usrPermissionView.Where(p => p.permission_Id == perm.permission_Id);
                                            if (!u.Any())
                                            {
                                                usrPermissionView.Add(perm);
                                                break;
                                            } break;
                                            }
                                        }
                                        else
                                        {
                                            usrPermissionView.Add(perm);
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    //usrPermissionView = null;
                    //usrPermissionView = multi.ToList<UserGroup_Permission>();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "AppCentre");
                }

                return usrPermissionView;
            }
        }

        /// <summary>
        /// Users permission gridview report logic.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="groupId">group id</param>
        /// /// <param name="appId">application id</param>
        /// <param name="appId">group id</param>
        /// <returns>list of users that to this application group</returns>
        public List<AppUsersGroupProfile> GetApplicationUsersAndGroups_VW(string appId, string groupId, GridSortParams gSort)
        {
            var uApps = new List<AppUsersProfile>();
            using (var db = new AppCentreEntities())
            {
                var vW = (from apps in db.VW_USER_PERMISSION
                          join users in db.USERs
                          on apps.USER_NAME equals users.USER_NAME
                          where (apps.APPLICATION_ID == appId)
                          select new AppUsersGroupProfile
                         {
                             User_Name = apps.USER_NAME,
                             User_Id = users.USER_ID,
                             First_Name = users.FIRST_NAME,
                             Last_Name = users.LAST_NAME,
                             application_Id = apps.APPLICATION_ID,
                             groupName = string.Empty
                         }).Distinct();

                IEnumerable<AppUsersGroupProfile> uGrps;

                if (groupId == "All Groups")
                {
                    uGrps = (from apps in db.USER_APPLICATION_GROUP
                             join p in vW
                             on apps.USER_ID equals p.User_Id
                             into gp
                             from sub in gp.DefaultIfEmpty()
                             where (apps.APPLICATION_ID == appId)
                             select new AppUsersGroupProfile
                             {
                                 User_Name = sub.User_Name,
                                 User_Id = sub.User_Id,
                                 First_Name = sub.First_Name,
                                 Last_Name = sub.Last_Name,
                                 application_Id = sub.application_Id,
                                 groupName = apps.GROUP_ID
                             }
                            ).Distinct(); 
                    //uGrps = vW;
                }
                else
                {
                    uGrps = (from apps in db.USER_APPLICATION_GROUP
                             join p in vW
                             on apps.USER_ID equals p.User_Id
                             into gp
                             from sub in gp.DefaultIfEmpty()
                             where (apps.APPLICATION_ID == appId && apps.GROUP_ID == groupId)
                             select new AppUsersGroupProfile
                             {
                                 User_Name = sub.User_Name,
                                 User_Id = sub.User_Id,
                                 First_Name = sub.First_Name,
                                 Last_Name = sub.Last_Name,
                                 application_Id = sub.application_Id,
                                 groupName = apps.GROUP_ID
                             }
                            ).Distinct(); 
                }

                if (gSort == null)
                {
                    if (gSort.sidx == "First_Name")
                        uGrps.OrderBy(p => p.First_Name);
                    if (gSort.sidx == "User_Name")
                        uGrps.OrderBy(p => p.User_Name);
                    if (gSort.sidx == "Last_Name")
                        uGrps.OrderBy(p => p.Last_Name);
                    if (gSort.sidx == "groupName")
                        uGrps.OrderBy(p => p.groupName);
                }
                else
                {

                }
                return uGrps.ToList<AppUsersGroupProfile>();
            }
        }

        /// <summary>
        /// permissions list for the requested group.
        /// Note in case of error, it will pass through the ExceptionHandler
        /// </summary>
        /// <param name="groupId">group id</param>
        /// /// <param name="appId">application id</param>
        /// <returns>permissions list for the requested group</returns>
        public List<Group_Permission> GetApplicationGroupPermissions(string appId, string groupId)
        {
            var uApps = new List<Group_Permission>();
            
            using (var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {
                    var permTargets = (from p in db.PERMISSIONs
                                       join t in db.TARGET_VALUE
                                       on p.APPLICATION_ID equals t.APPLICATION_ID into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId && p.TARGET_ID != null)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.TARGET_VALUE_ID,
                                           targetValueName = sub.NAME,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();
                    var permissions = (from p in db.PERMISSIONs
                                       join t in permTargets
                                           on new { applicationId = p.APPLICATION_ID, permissionId = p.PERMISSION_ID } equals new { t.applicationId, t.permissionId } into gp
                                       from sub in gp.DefaultIfEmpty()
                                       where (p.APPLICATION_ID == appId)
                                       select new AllPermissions
                                       {
                                           permissionId = p.PERMISSION_ID,
                                           permissionName = p.PERMISSION_NAME,
                                           applicationId = p.APPLICATION_ID,
                                           targetId = p.TARGET_ID,
                                           target = p.TARGET.TARGET_NAME,
                                           targetValueId = sub.targetValueId,
                                           targetValueName = sub.targetValueName,
                                           groupEnabled = p.GROUP_ENABLED
                                       }).Distinct();

                    IEnumerable<GROUP_PERMISSION> grpPermissions;
                    if (groupId == "All Groups")
                    {
                        grpPermissions = from g in db.GROUP_PERMISSION
                                         where (g.APPLICATION_ID == appId)
                                         select g;
                    }
                    else
                    {
                        grpPermissions = from g in db.GROUP_PERMISSION
                                         where (g.APPLICATION_ID == appId && g.GROUP_ID == groupId)
                                         select g;
                    }
                    
                    var grpAndTargets = from g in grpPermissions
                                        join p in permissions
                                        on new { applicationId = g.APPLICATION_ID, permissionId = g.PERMISSION_ID, targetValueId = g.TARGET_VALUE_ID } equals new { p.applicationId, p.permissionId, p.targetValueId }
                                        into gp
                                        from sub in gp.DefaultIfEmpty()
                                        select new Group_Permission()
                                        {
                                            APPLICATION_ID = g.APPLICATION_ID,
                                            GROUP_ID = g.GROUP_ID,
                                            GROUP_PERMISSSION_ID = g.GROUP_PERMISSSION_ID,
                                            PERMISSION_ID = sub.permissionId ?? "",
                                            PERMISSION_NAME = sub.permissionName ?? "",
                                            TARGET_VALUE = sub.targetValueName ?? "",
                                            TARGET_VALUE_ID = sub.targetValueId
                                        };

                    if (grpAndTargets.Any())
                    {
                        //uApps = grpAndTargets.OrderBy(p => p.PERMISSION_ID).Distinct().ToList<Group_Permission>(); 
                        foreach(var perm in grpAndTargets)
                        {
                            if (uApps.Any())
                            {
                                var isPresent = uApps.Find(p => p.PERMISSION_ID == perm.PERMISSION_ID && p.GROUP_ID == perm.GROUP_ID);
                                if (isPresent == null)
                                {
                                    uApps.Add(perm);
                                }
                            }
                            else
                                uApps.Add(perm);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, "AppCentre");
                }
            }

            return uApps;
        }
    }
}
