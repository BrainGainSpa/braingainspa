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
    
    public partial class CoursePlu
    {
        public int CoursePlusID { get; set; }
        public Nullable<int> CourseID { get; set; }
        public Nullable<int> SemesterID { get; set; }
        public Nullable<int> MthID { get; set; }
        public Nullable<int> MonthID { get; set; }
        public Nullable<int> YearID { get; set; }
        public string imgpath { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
