//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACG.EA.AppCentre.Lib.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class GROUP_PERMISSION
    {
        public int GROUP_PERMISSSION_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public string PERMISSION_ID { get; set; }
        public Nullable<int> TARGET_VALUE_ID { get; set; }
    
        public virtual APPLICATION_GROUP APPLICATION_GROUP { get; set; }
        public virtual PERMISSION PERMISSION { get; set; }
        public virtual TARGET_VALUE TARGET_VALUE { get; set; }
    }
}