//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace  ACG.EA.AppCentre.Lib.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class USER
    {
        public USER()
        {
            this.USER_APPLICATION_GROUP = new HashSet<USER_APPLICATION_GROUP>();
            this.USER_PERMISSION = new HashSet<USER_PERMISSION>();
        }
    
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string TITLE { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED { get; set; }
        public string MODIFIED_BY { get; set; }
    
        public virtual ICollection<USER_APPLICATION_GROUP> USER_APPLICATION_GROUP { get; set; }
        public virtual ICollection<USER_PERMISSION> USER_PERMISSION { get; set; }
    }
}
