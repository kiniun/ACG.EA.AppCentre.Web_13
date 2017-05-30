using System;

namespace ACG.EA.AppCentre.Web.Models
{
    public class QueryModels
    {
    }

    public sealed class AppUser
    {
        public string user { get; set; }
        public string app { get; set; }
        public string appId { get; set; }
    }

    public class RequestFilter
    {
        public string MyProperty { get; set; }
    }
}