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
    
    public partial class BankPayment
    {
        public int BankPaymentID { get; set; }
        public Nullable<long> PersonID { get; set; }
        public string FullName { get; set; }
        public string GSM { get; set; }
        public string TellerNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Branch { get; set; }
        public Nullable<bool> Confirmed { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
