using System;
using System.Collections.Generic;
using System.Linq;

namespace ACG.EA.AppCentre.Lib.Models
{

    public class Group_Permission
    {
        public int? GROUP_PERMISSSION_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public string PERMISSION_ID { get; set; }
        public string PERMISSION_NAME { get; set; }
        public Nullable<int> TARGET_VALUE_ID { get; set; }
        public string TARGET_VALUE { get; set; }
        public bool isGranted { get; set; }
    }

    public class CompareStrings : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            return string.Compare(s1, s2, true);
        }
    }
    
    public class GroupPermissionSet
    {
        //{ applicationId: "", groupId: "", permissions: [], grpPermId: [], target: null, isGranted: [] };
        public string applicationId { get; set; }
        public string groupId { get; set; }
        public List<string> permissions { get; set; }
        public List<int> grpPermId { get; set; }
        public List<bool> isGranted { get; set; }
        public List<int?> targets { get; set; }
    }

    public class AllPermissions
    {
        public string permissionId { get; set; }
        public string permissionName { get; set; }
        public string applicationId { get; set; }
        public int? targetId { get; set; }
        public string target { get; set; }
        public int? targetValueId { get; set; }
        public string targetValueName { get; set; }
        public bool? groupEnabled { get; set; }
    }

    public class User_Permission
    {
        public User_Permission()
        { }

        public int? user_permission_Id { get; set; }
        public int? user_Id { get; set; }
        public string permission_Id { get; set; }
        public string permission_Name { get; set; }
        public string application_Id { get; set; }
        public int? targetId { get; set; }
        public string target { get; set; }
        public string targetValue { get; set; }
        public int? targetValueId { get; set; }
        public bool groupEnabled { get; set; }
        public string grant { get; set; }
    }

    public class UserGroup_Permission
    {
        public UserGroup_Permission()
        { }

        public int? user_permission_Id { get; set; }
        public int? group_permission_Id { get; set; }
        public int? user_Id { get; set; }
        public string permission_Id { get; set; }
        public string permission_Name { get; set; }
        public string application_Id { get; set; }
        public int? targetId { get; set; }
        public string target { get; set; }
        public string targetValue { get; set; }
        public int? targetValueId { get; set; }
        public bool groupEnabled { get; set; }
        public string grant { get; set; }
    }

    public class User_PermissionTargets
    {
        public int? user_permission_Id { get; set; }
        public int? user_Id { get; set; }
        public string permission_Id { get; set; }
        public string permission_Name { get; set; }
        public string application_Id { get; set; }
        public int? targetId { get; set; }
        public List<Target> target { get; set; }
        public bool? groupEnabled { get; set; }
        public string grant { get; set; }
        public int? group_permission_id { get; set; }
    }
    
    public class UserPermissionSet
    {

        public string applicationId { get; set; }
        public string userId { get; set; }
        public List<int> usrPermId { get; set; }
        public List<bool> isGranted { get; set; }
        public List<string> permissions { get; set; }
        public List<int?> targets { get; set; }
    }

    public class UserGroupPermissionSet
    {

        public string applicationId { get; set; }
        public string userId { get; set; }
        public List<int> usrPermId { get; set; }
        public List<int?> usrGrpPermId { get; set; }
        //public List<bool> isGranted { get; set; }
        public List<string> permState { get; set; }
        public List<string> permissions { get; set; }
        public List<int?> targets { get; set; }
    }

    public class UserPermissionTargetSet
    {
        public string applicationId { get; set; }
        public string userId { get; set; }
        public List<int> usrPermId { get; set; }
        public List<bool> isGranted { get; set; }
        public List<string> permissions { get; set; }
        //public List<string> targets { get; set; }
        public List<List<int>> targets { get; set; }
    }

    public class AppPermissionSet
    {
        public string applicationId { get; set; }
        public List<string> permissions { get; set; }
        public List<bool> persist { get; set; }
    }
}
