using System;
using System.Collections.Generic;
using System.Text;

namespace ACG.EA.AppCentre.Lib.Models
{
    public class Application
    {
        public string application_id { get; set; }
        public string application_name { get; set; }
        public string application_uri { get; set; }
        public string application_desc { get; set; }
        public string catalog_id { get; set; }
    }

    public class User_Application
    {
        public int userId { get; set; }
        public string groupId { get; set; }
        public Application App { get; set; }
    }

    public class ApplicationProfile
    {
        public string group_Id { get; set; }
        public Application ApplicationGrp { get; set; }
        public string group_Name { get; set; }

    }

    public class Application_Group
    {
        public string Group_Id { get; set; }
        public string Application_Id { get; set; }
        public string Group_Name { get; set; }
        public Nullable<bool> setPermission { get; set; }
    }

    public class Permission
    {
        public string PERMISSION_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public Nullable<int> TARGET_ID { get; set; }
        public string PERMISSION_NAME { get; set; }
    }

    public class Group_Permission
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int GROUP_PERMISSSION_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public string PERMISSION_ID { get; set; }
        public Nullable<int> TARGET_VALUE_ID { get; set; }
    }

    public class Target
    {
        public int target_Id { get; set; }
        public string target_name { get; set; }
    }

    public class UserApplicationGroup
    {
        //[System.ComponentModel.DataAnnotations.Key]
        //public int USER_APPLICATION_GROUP_ID { get; set; }
        public int user_Id { get; set; }
        public List<string> groups { get; set; }
        public string application_Id { get; set; }
    }


    public class PermissionSet
    {
        public string applicationId { get; set; }
        public string groupId { get; set; }
        public List<string> permissions { get; set; }
        public string target { get; set; }
    }

    public class UserPermission
    {
        public string applicationId { get; set; }
        public string userId { get; set; }
        public string targetId { get; set; }
        public List<string> permissions { get; set; }
        public bool grant { get; set; }
    }
}
