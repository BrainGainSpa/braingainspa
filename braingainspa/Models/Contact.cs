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
    
    public partial class Contact
    {
        public int ContactID { get; set; }
        public long PersonID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AlternateEmail { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
