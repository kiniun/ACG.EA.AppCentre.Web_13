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
    
    public partial class APPLICATION
    {
        public APPLICATION()
        {
            this.APPLICATION_GROUP = new HashSet<APPLICATION_GROUP>();
            this.PERMISSIONs = new HashSet<PERMISSION>();
            this.TARGETs = new HashSet<TARGET>();
        }
    
        public string APPLICATION_ID { get; set; }
        public string CATALOG_ID { get; set; }
        public string APPLICATION_NAME { get; set; }
        public string APPLICATION_URI { get; set; }
        public string APPLICATION_DESC { get; set; }
    
        public virtual CATALOG CATALOG { get; set; }
        public virtual ICollection<APPLICATION_GROUP> APPLICATION_GROUP { get; set; }
        public virtual ICollection<PERMISSION> PERMISSIONs { get; set; }
        public virtual ICollection<TARGET> TARGETs { get; set; }
    }
}
