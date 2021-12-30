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
    
    public partial class FreeTrialQuestion
    {
        public long FreeTrialQuestionID { get; set; }
        public Nullable<long> PersonID { get; set; }
        public Nullable<long> FreeTrialQuizID { get; set; }
        public string Token { get; set; }
        public string ImagePath { get; set; }
        public Nullable<int> QuestionID { get; set; }
        public Nullable<int> QuestionNumber { get; set; }
        public Nullable<int> QuestionMarks { get; set; }
        public Nullable<int> AnswerID { get; set; }
        public Nullable<bool> IsCorrect { get; set; }
        public Nullable<bool> Answered { get; set; }
        public string FIGAnswer1 { get; set; }
        public string FIGAnswer2 { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> TopicID { get; set; }
        public Nullable<int> CourseID { get; set; }
    }
}
