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
    
    public partial class Student_Details
    {
        public long StudentDetailID { get; set; }
        public Nullable<long> PersonID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> FacultyID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> LevelID { get; set; }
        public Nullable<int> SemesterID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
