//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace braingainspa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Percentage
    {
        public int PercentID { get; set; }
        public string PercentName { get; set; }
        public string PValue { get; set; }
        public string PType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<double> Percentage1 { get; set; }
        public Nullable<double> Registration { get; set; }
        public Nullable<double> Subscription { get; set; }
        public Nullable<decimal> PAmount { get; set; }
        public Nullable<bool> PLevel { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
