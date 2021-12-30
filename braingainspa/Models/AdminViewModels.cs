using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using braingainspa.Models;

namespace braingainspa.Models
{
    public class AdminViewModels
    {
    }

    public class AppSettingsViewModel
    {
        public int ID { get; set; }
        //public decimal CourseFees { get; set; }
        public decimal RegistrationFee { get; set; }
        //public decimal RegistrationFee2 { get; set; }
        public string DefaultReferralCode { get; set; }
        //public decimal ReferralCodeEarning { get; set; }
        public int MaxUnderline { get; set; }
        public int MaxFirstLevel { get; set; }
        public int MaximumOptions { get; set; }
        public int QuestionsBatches { get; set; }
        public string SupportPhone { get; set; }
        public double QuizPeriod { get; set; }
        public double MockPeriod { get; set; }
        public double PRYQuizPeriod { get; set; }
        public double JSSQuizPeriod { get; set; }
        public double SSSQuizPeriod { get; set; }
    }

    public class AdminAccountViewModel
    {
        public int AccountID { get; set; }
        public long PersonID { get; set; }
        public decimal Company { get; set; }
        public decimal Rewards { get; set; }
        public decimal Admin1 { get; set; }
        public decimal Admin2 { get; set; }
    }

    public class AdminTransactionViewModel
    {
        public long PaymentTransactionID { get; set; }
        public long PersonID { get; set; }
        public string TransactionName { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; } 
        public DateTime TransDate { get; set; }
    }
}