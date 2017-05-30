using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACG.EA.AppCentre.DataUtils;
using System.Diagnostics;
using LibModel = ACG.EA.AppCentre.Lib.Models;
using WebModel = ACG.EA.AppCentre.Web.Models;
using ACG.EA.AppCentre.Web.Utils;

namespace ACG.EA.AppCentre.Web.Controllers
{
    [System.Serializable()]
    [RoutePrefix("AppcentreApi")]
    public class AppCentreController : ACG.EA.AppCentre.Web.Utils.AC_Controllers
    {
        public AppCentreController()
        {
            if (lib == null)
                lib = new AppsAdministration();
            if (libPer == null)
                libPer = new Permissions();
        }
        
        private static AppsAdministration lib;
        private static UserAdminLib libUser = new UserAdminLib();
        private static Permissions libPer;
        private static ACG.EA.AppCentre.Utils.Authorization auth = new ACG.EA.AppCentre.Utils.Authorization();

        [Route("getApplicationGroups/{appId}")]
        public List<LibModel.Groups> GetApplicationGroups(string appId)
        {
            List<LibModel.Groups> groups = null;
            try
            {
                groups = lib.LoadApplicationGroups(appId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return groups;
        }

        [Route("GetPermissions/{uName}")]
        public bool GetPermissions(string uName)
        {
            bool admin = false;
            admin = auth.UserIsAdmin(ApplicationName, uName, permType);
            return admin;
        }

        [Route("getAppsByCatalog/{user}")]
        public List<LibModel.ApplicationsByCatalog> GetAppsByCatalog(string user)
        {
            List<LibModel.ApplicationsByCatalog> apps = null;
            Utils.AppCentreAdminLib userProfile = new Utils.AppCentreAdminLib();
            bool IsAdmin;
            IsAdmin = userProfile.IsAppCentreAdmin(user);
            try
            {
                apps = lib.GetUserApplicationsByCatalog(user, IsAdmin);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return apps;
        }

        [Route("loadAppUsersActivityLog/{appId}")]
        [HttpGet]
        public List<ACG.EA.AppCentre.Lib.DAL.ACTIVITY_LOG> LoadAppUsersActivityLog(string appId)
        {
            List<ACG.EA.AppCentre.Lib.DAL.ACTIVITY_LOG> logs = null;

            try
            {
                logs = lib.LoadActivityLog(appId);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return logs;
        }

        [Route("getAllAppUsers/{searchBy}/{user}")]
        public List<LibModel.User> GetAllAppUsers(string searchBy, string user)
        {
            var appUsers = libUser.LoadSearchedUserProfile(searchBy, user);
            return appUsers.ToList();

        }

        [Route("getadminApps/{user}")]
        public List<LibModel.Application> GetAdminApplications(string user)
        {
            List<LibModel.Application> apps = null;
            //var app = System.Configuration.ConfigurationManager.AppSettings["application"].ToString();
            var perm = System.Configuration.ConfigurationManager.AppSettings["permissionTyp"].ToString();
            Utils.AppCentreAdminLib userProfile = new Utils.AppCentreAdminLib();
            bool IsAdmin;
            IsAdmin = userProfile.IsAppCentreAdmin(user);
            try
            {
                if (IsAdmin)
                {
                    apps = lib.GetAdminApplications(user, IsAdmin, perm); 
                }
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return apps;
        }

        [Route("getUserAppGrp/{user}/{appId}")]
        [HttpGet]
        public List<LibModel.User_GroupPT> GetUserAppGrp(int user, string appId)
        {
            var result = new List<LibModel.User_GroupPT>();
            try
            {
                if (ModelState.IsValid)
                {
                    result = libUser.LoadSelectedUserGroups(user, appId);
                }
                else
                    result = null;
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
                result = null;
            }
            return result;
        }

        [Route("getUserPermissions/{user}/{appId}")]
        public List<LibModel.UserGroup_Permission> GetUserPermissions_V1(int user, string appId)
        {
            var userPermProfile = new List<LibModel.UserGroup_Permission>();
            try
            {
                //userPermProfile = lib.LoadSelectedUserPermissions(user, appId);
                userPermProfile = libPer.LoadPermsForUser_V1(user, appId);
            }
            catch (Exception ex)
            {
                ;
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return userPermProfile.ToList();
        }

        [Route("syncUserGroups")]
        [HttpGet]
        public HttpResponseMessage SyncUserGroups([FromUri] LibModel.UserGroupSet uGroups)
        {
            try
            {
                if (ModelState.IsValid && uGroups != null)
                {
                    if (libUser.SetUserGroups(uGroups))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                ;
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        [Route("syncUserPermissions")]
        [HttpGet]
        public HttpResponseMessage SyncUserPermissions([FromUri] LibModel.UserGroupPermissionSet pSet)
        {
            try
            {
                if (ModelState.IsValid && pSet != null)
                {
                    if (libPer.UpdateUserPermissions_V1(pSet))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ApplicationName, ex.Message, EventLogEntryType.Error);
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }


        [Route("getApplicationTargets{appId}")]
        public List<LibModel.Target> GetApplicationTargets(string appId)
        {
            List<LibModel.Target> targets = null;
            try
            {
                targets = lib.LoadApplicationTarget(appId);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return targets;
        }

        [Route("getAppTargetValues{appId}")]
        public List<LibModel.Target> GetAppTargetValues(string appId)
        {
            List<LibModel.Target> targets = null;
            try
            {
                targets = lib.LoadAppTargetValues(appId);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return targets;
        }

        [Route("getGrpPermissionsV1")]
        public List<LibModel.Group_Permission> GetGrpPermissionsV1([FromUri] Models.AppGroup appGroup)
        {
            var userPermProfile = new List<LibModel.Group_Permission>();
            try
            {
                userPermProfile = libPer.LoadAppGroupPermissionNTargets(WebUtility.HtmlDecode(appGroup.groupId), WebUtility.HtmlDecode(appGroup.applicationId));
            }
            catch (Exception ex)
            {
                ;
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return userPermProfile.ToList();

        }

        [Route("searchUsersApplications/{user}/{searchBy}/{keyword}")]
        [HttpGet]
        public List<LibModel.Application> SearchUsersApplications(string user, string searchBy, string keyword)
        {
            List<LibModel.Application> apps = null;
            try
            {
                apps = lib.SearchUserApplications(user, searchBy, keyword);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return apps;
        }

        [Route("filterApplications/{searchBy}/{query}")]
        [HttpGet]
        public List<LibModel.Application> FilterApplications(string searchBy, string query)
        {
            List<LibModel.Application> apps = null;
            try
            {
                apps = lib.SearchAppStores(searchBy, query);
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
            }
            return apps;
        }

        [Route("getApplicationUsers")]
        public List<LibModel.AppUsersGroupProfile> GetApplicationUsers([FromUri] Models.AppGroup appGroup, [FromUri] LibModel.GridSortParams gSort)
        {
            List<LibModel.AppUsersGroupProfile> usrsProfile;
            if (appGroup.applicationId != string.Empty)
            {
                usrsProfile = libPer.GetApplicationUsersAndGroups_VW(appGroup.applicationId, appGroup.groupId, gSort);
                return usrsProfile.ToList();
            }
            else
                return null;
        }

        [Route("loadAppGroupPermissions")]
        [HttpGet]
        public List<LibModel.Group_Permission> LoadAppGroupPermissions([FromUri] Models.AppGroup appGroup, [FromUri] LibModel.GridSortParams gSort)
        {
            List<LibModel.Group_Permission> usrsProfile;
            if (appGroup.applicationId != string.Empty)
            {
                usrsProfile = libPer.GetApplicationGroupPermissions(appGroup.applicationId, appGroup.groupId);
                return usrsProfile.ToList();
            }
            else
                return null;
        }

        [Route("addUser")]
        [HttpGet]
        public HttpResponseMessage AddUser([FromUri] LibModel.User user)
        {
            try
            {
                if (user != null)
                {
                    if (user.User_Id == 0)
                    {
                        if (libUser.AddNewUser(user))
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        else
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        if (libUser.UpdateUser(user))
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        else
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        [Route("removeUser/{userId}")]
        [HttpGet]
        public HttpResponseMessage RemoveUser(string userId)
        {
            if (userId != null)
            {
                if (libUser.RemoveUser(Convert.ToInt32(userId)))
                    return new HttpResponseMessage(HttpStatusCode.OK);
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else
                return new HttpResponseMessage(HttpStatusCode.NotModified);
        }

        [Route("checkIfUserExists/{userName}")]
        [HttpGet]
        public int checkIfUserExists(string userName)
        {
            var result = string.Empty;
            try
            {
                if (libUser.CheckUserExists(userName))
                    return 1;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
                return -1;
            }
        }

        [Route("checkIfAppExists/{appId}")]
        [HttpGet]
        public int checkIfAppExists(string appId)
        {
            var result = string.Empty;
            try
            {
                if (lib.CheckAppExists(appId))
                    return 1;
                else
                    return 0;
            }
            catch (Exception ex)
            {

                ExceptionHandler.LogException(ex, ApplicationName);
                return -1;
            }
        }

        [Route("updateApplication")]
        [HttpGet]
        public HttpResponseMessage UpdateApplication([FromUri] Lib.Models.Application app)
        {
            try
            {
                if (ModelState.IsValid && app != null)
                {
                    if (lib.UpdateApplication(app))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        [Route("addNewApplication")]
        [HttpGet]
        public HttpResponseMessage SaveNewApplication([FromUri] Lib.Models.Application app)
        {
            try
            {
                if (ModelState.IsValid && app != null)
                {
                    if (app.catalog_id == null)
                    {
                        var catalog = System.Configuration.ConfigurationManager.AppSettings["catalog"].ToString();
                        app.catalog_id = catalog;
                    }
                    if (lib.AddApplication(app))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        //adds and updates multiple permissions for a group
        [Route("updateGroupPermissions")]
        [HttpGet]
        public HttpResponseMessage UpdateGroupPermissions([FromUri] LibModel.GroupPermissionSet pSet)
        {
            try
            {
                if (ModelState.IsValid && pSet != null)
                {
                    var newSet = new LibModel.GroupPermissionSet();
                    newSet.groupId = WebUtility.HtmlDecode(pSet.groupId);
                    newSet.applicationId = WebUtility.HtmlDecode(pSet.applicationId);
                    newSet.permissions = new List<string>();
                    newSet.grpPermId = new List<int>();
                    newSet.isGranted = new List<bool>();
                    newSet.targets = new List<int?>();
                    foreach (var p in pSet.permissions)
                    {
                        newSet.permissions.Add(WebUtility.HtmlDecode(p));
                    }
                    foreach (var granted in pSet.isGranted)
                    {
                        newSet.isGranted.Add(granted);
                    }
                    foreach (var p in pSet.grpPermId)
                    {
                        newSet.grpPermId.Add(p);
                    }
                    foreach (var p in pSet.targets)
                    {
                        newSet.targets.Add(p);
                    }

                    if (libPer.UpdateGroupPermissions(newSet))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                ;
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [Route("createApplicationGroup")]
        [HttpGet]
        public HttpResponseMessage CreateApplicationGroup([FromUri] LibModel.Application_Group appGrp)
        {
            try
            {
                if (ModelState.IsValid && appGrp != null)
                {
                    if(lib.AddApplicationGroup(appGrp))
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    else
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                }
                else
                    return new HttpResponseMessage(HttpStatusCode.NotModified);

            }
            catch (Exception ex)
            {
                ;
                ExceptionHandler.LogException(ex, ApplicationName);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}