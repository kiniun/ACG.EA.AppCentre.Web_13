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
    
    public partial class TARGET
    {
        public TARGET()
        {
            this.PERMISSIONs = new HashSet<PERMISSION>();
            this.TARGET_VALUE = new HashSet<TARGET_VALUE>();
        }
    
        public int TARGET_ID { get; set; }
        public string APPLICATION_ID { get; set; }
        public string TARGET_NAME { get; set; }
    
        public virtual APPLICATION APPLICATION { get; set; }
        public virtual ICollection<PERMISSION> PERMISSIONs { get; set; }
        public virtual ICollection<TARGET_VALUE> TARGET_VALUE { get; set; }
    }
}
