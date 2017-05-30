using System;

namespace ACG.EA.AppCentre.Web.Models
{
    public class Application
    {
        public string application_id { get; set; }
        public string application_name { get; set; }
        public string application_uri { get; set; }
        public string application_desc { get; set; }
        public string catalog { get; set; }
        public string group_Id { get; set; }
        public string group_Name { get; set; }

    }

    public class Application_Group
    {
        public string Group_Id { get; set; }
        public string Application_Id { get; set; }
        public string Group_Name { get; set; }
    }

    public class AppGroup
    {
        public string applicationId { get; set; }
        public string groupId { get; set; }
    }

}