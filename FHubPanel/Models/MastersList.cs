//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FHubPanel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MastersList
    {
        public MastersList()
        {
            this.MasterValues = new HashSet<MasterValue>();
            this.ParameterMappings = new HashSet<ParameterMapping>();
        }
    
        public int Id { get; set; }
        public string MasterName { get; set; }
        public Nullable<decimal> OrdNo { get; set; }
        public bool IsSystem { get; set; }
    
        public virtual ICollection<MasterValue> MasterValues { get; set; }
        public virtual ICollection<ParameterMapping> ParameterMappings { get; set; }
    }
}
