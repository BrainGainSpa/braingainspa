using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace braingainspa.Models
{
    public class QuizViewModels
    {

    }

    //public class BankInfoViewModels
    //{

    //    public int BankInfoID { get; set; }
    //    public int PersonID { get; set; }

    //    [Required]
    //    public Nullable<short> BANK_ID { get; set; }

    //    public string BankName { get; set; }

    //    public string BranchName { get; set; }

    //    [Required]
    //    //[StringLength(50, ErrorMessage = "The {0} must be at least {10} characters long.", MinimumLength = 50)]
    //    [StringLength(50, ErrorMessage = "You must enter Account Name", MinimumLength = 2)]
    //    public string AccountName { get; set; }

    //    [Required]
    //    [StringLength(10, ErrorMessage = "You must enter Account Number", MinimumLength = 10)]
    //    public string AccountNumber { get; set; }

    //    public bool IsDeleted { get; set; }

    //    public Nullable<DateTime> Created { get; set; }
    //    public Nullable<DateTime> Updated { get; set; }

    //}


    public class SchoolViewModels
    {
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }

    public class SchoolListViewModels
    {
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
    }

    public class FacultyViewModels
    {
        public int FacultyID { get; set; }
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string FacultyName { get; set; }
        public int ResourceTypeID { get; set; }
        public string ResourceTypeName { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        //public Nullable<System.DateTime> Created { get; set; }
        //public Nullable<System.DateTime> Updated { get; set; }

        public virtual School School { get; set; }
    }

    public class DepartmentViewModels
    {
        public int DepartmentID { get; set; }
        public Nullable<int> FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public List<Faculty> Faculty { get; set; }
    }
    public class CourseViewModels
    {
        public int CourseID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> SemesterID { get; set; }
        public Nullable<int> CYear { get; set; }
        public Nullable<int> MonthID { get; set; }
        public Nullable<int> YearID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public Nullable<int> CreditUnits { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }


    }

    public class CurrentClassViewModels
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public List<ClassTopics> CurrentTopics { get; set; }

    }

    public class GradeCourses
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }

    }

    public class FreeTrialViewModels
    {
        public int FreeTrialID { get; set; }
        public int FacultyID { get; set; }
        public string FacultyName { get; set; }
        public List<GradeCourses> FreeCourses { get; set; }

    }

    public class ProfileViewModel
    {
        public long PersonID { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public DateTime DOB { get; set; }
        public string ReferralCode { get; set; }
        public string RefereeCode { get; set; }
        public string ParentCode { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }

    public class ClassTopics
    {
        public int TopicID { get; set; }
        public string TopicName { get; set; }
    }

    public class FreeQuizInfoViewModels
    {
        //public long QuizID { get; set; }
        //public string Token { get; set; }
        //public long PersonID { get; set; }
        //public int TopicID { get; set; }

        public long FreeTrialQuizID { get; set; }
        public long FreeTrialID { get; set; }
        public int SubjectID { get; set; }
        public string QuizName { get; set; }
        public int QuestionsCount { get; set; }
        public int QuizDuration { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Phone is Required.")]
        //[DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        //public int TotalMarks { get; set; }
        //public int StartSN { get; set; }
        //public int StopSN { get; set; }
    }

    public class QuizInfoViewModels
    {
        public long QuizID { get; set; }
        public string Token { get; set; }
        public long PersonID { get; set; }
        public int TopicID { get; set; }
        public string QuizName { get; set; }
        public int QuestionsCount { get; set; }
        public int QuizDuration { get; set; }
        public int TotalMarks { get; set; }
        public int StartSN { get; set; }
        public int StopSN { get; set; }
    }

    public class TopicViewModels
    {
        public int TopicID { get; set; }
        public Nullable<int> CourseID { get; set; }
        public string TopicName { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public virtual Cours Cours { get; set; }
    }

    public class PastQuestionViewModels
    {
        public int PastQuestionID { get; set; }
        public int CourseID { get; set; }
        public int YearID { get; set; }
        public int YearName { get; set; }
        public int linktoYearId { get; set; }
        public string CourseName { get; set; }
        public string FacultyName { get; set; }
        public string Description { get; set; }
        //public Nullable<System.DateTime> Created { get; set; }
        //public Nullable<System.DateTime> Updated { get; set; }
        //public Nullable<bool> IsDeleted { get; set; }

        public virtual Cours Cours { get; set; }
    }

    public class QuestionVModels
    {
        public int QuestionID { get; set; } = 0;
        public int CourseID { get; set; }
        public int TopicID { get; set; }
        public int QuestionTypeID { get; set; }
        public int ResourceTypeID { get; set; }
        public int SectionID { get; set; }
        public int QuestionNumber { get; set; }
        public string Number { get; set; }
        public string QuestionString { get; set; }
        public string Answer { get; set; }
        public int QuestionMarks { get; set; }
        public string CorrectAnswer { get; set; }
        public string AnswerExplanation { get; set; }
        public int QuestionDuration { get; set; }
        public int Year { get; set; }
        public bool IsVerified { get; set; }
        public string Instruction { get; set; }
        public bool IsMock { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileWrapper Image_File { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Topic Topic { get; set; }
        public string TopicName { get; set; }
    }

    public class QuestionViewModels
    {
        public int QuestionID { get; set; }
        public int CourseID { get; set; }
        public int TopicID { get; set; }
        public int QuestionTypeID { get; set; }
        public int ResourceTypeID { get; set; }
        public int SectionID { get; set; }
        public int QuestionNumber { get; set; }
        public string Number { get; set; }
        public string QuestionString { get; set; }
        public string Answer { get; set; }
        public int QuestionMarks { get; set; }
        public string CorrectAnswer { get; set; }
        public string AnswerExplanation { get; set; }
        public int QuestionDuration { get; set; }
        public int Year { get; set; }
        public bool IsVerified { get; set; }
        public string Instruction { get; set; }
        public bool IsMock { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Topic Topic { get; set; }
        public string TopicName { get; set; }
    }

    public class AnswerViewModels
    {
        [Key]
        public int AnswerID { get; set; }
        public int QuestionID { get; set; }
        public string QuestionString { get; set; }
        public int QuestionNumber { get; set; }
        public string OptionLetter { get; set; }
        public string AnswerString { get; set; }
        public bool IsCorrect { get; set; }

        //public virtual Question Question { get; set; }
    }

    public class ClientQuizViewModel
    {
        public long ClientQuizID { get; set; }
        public string Token { get; set; }
        public long PersonID { get; set; }
        public int TopicID { get; set; }
        public string TopicName { get; set; }
        public string ResourceTypeName { get; set; }
        public int QuestionCount { get; set; }
        public int LastQuestionID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    }

    public class ClientQuestionViewModel
    {
        public long ClientQuestionID { get; set; }
        public long PersonID { get; set; }
        public int QuizID { get; set; }
        public string Token { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public int QuestionMarks { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionNumber { get; set; }
        public string Direction { get; set; }
        public bool Answered { get; set; }
        //public int RemainingTimeMin { get; set; }
        //public int RemainingTimeSec { get; set; }
        //public TimeSpan ReaminTime { get; set; }

        public List<ClientAnswerViewModel> ClientAnswer { get; set; }
    }

    public class ClientAnswerViewModel
    {
        public Nullable<int> AnswerID { get; set; }
        public string AnswerText { get; set; }
    }

    public class QuestionXModels
    {
        public long PersonID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public int TopicID { get; set; }
        public int CourseID { get; set; }
        public int QuestionTypeID { get; set; }
        public string Number { get; set; }
        public string QuestionString { get; set; }
        public string QLine1 { get; set; }
        public string QLine2 { get; set; }
        public string QLine3 { get; set; }
        public string QLine4 { get; set; }
        public string QLine5 { get; set; }
        public string QLine6 { get; set; }
        public string QLine7 { get; set; }
        public string QLine8 { get; set; }
        public string QLine9 { get; set; }
        public int QuestionNumber { get; set; }
        public int QuestionMarks { get; set; }
        public string AnswerExplanation { get; set; }
        public string Answer { get; set; }
        public int FIGSlot { get; set; }
        public string FIGAnswer1 { get; set; }
        public string FIGAnswer2 { get; set; }
        public string FIGAnswer3 { get; set; }
        public string FIGAnswer4 { get; set; }
        public string Direction { get; set; }
        public string Instruction { get; set; }
        public bool Iscorrect { get; set; }
        public bool Answered { get; set; }
        public string ImagePath { get; set; }
        public string Token { get; set; }

        //------------------------------SESSION VARIABLES--------------------------------------------------//
        public int SMarks { get; set; }
        public string SExplitive { get; set; }
        public string SCorrectAnswer { get; set; }
        public string SInCorrectAnswer { get; set; }
        public string SAnswerExplanation { get; set; }
        public int SPosQue { get; set; }
        public int SNegQue { get; set; }
        public string ELine1 { get; set; }
        public string ELine2 { get; set; }
        public string ELine3 { get; set; }
        public string ELine4 { get; set; }
        public string ELine5 { get; set; }
        public string ELine6 { get; set; }
        public string ELine7 { get; set; }
        public string ELine8 { get; set; }
        public string ELine9 { get; set; }
        public bool Submit { get; set; }

        //--------------------------------------------------------------------------------//

        public List<AnswerXModels> Options { get; set; }

    }

    public class AnswerXModels
    {
        //[Key]
        public int AnswerID { get; set; }
        public string OptionLetter { get; set; }
        public string AnswerString { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionNumber { get; set; }
        public string Token { get; set; }
    }

    public class QuizQuestionModels
    {
        public int QuestionID { get; set; }
    }

    public class ViewModel
    {
        public ClientQuestion ClientQuestions { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }

    public class CascadingModel
    {
        public CascadingModel()
        {
            this.Faculties = new List<SelectListItem>();
            this.Courses = new List<SelectListItem>();
            //this.Cities = new List<SelectListItem>();
        }

        public List<SelectListItem> Faculties { get; set; }
        public List<SelectListItem> Courses { get; set; }
        //public List<SelectListItem> Cities { get; set; }

        public int FacultyId { get; set; }
        public int CourseId { get; set; }
        //public int CityId { get; set; }
    }
    public class BankInfoViewModels
    {

        public int BankInfoID { get; set; }
        public int PersonID { get; set; }

        [Required]
        public Nullable<short> BANK_ID { get; set; }

        public string BankName { get; set; }

        public string BranchName { get; set; }

        [Required]
        //[StringLength(50, ErrorMessage = "The {0} must be at least {10} characters long.", MinimumLength = 50)]
        //[StringLength(50, ErrorMessage = "You must enter Account Name", MinimumLength = 2)]
        public string AccountName { get; set; }

        [Required]
        //[StringLength(10, ErrorMessage = "You must enter Account Number", MinimumLength = 10)]
        public string AccountNumber { get; set; }

        public bool IsDeleted { get; set; }

        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }

    }

    public class PackageViewModel
    {

        public int MemberNetworkID { get; set; }

        [Required]
        public int PersonID { get; set; }

        [Required]
        public int NetworkID { get; set; }

        public string NetworkName { get; set; }

        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }

    }

    public class ManagePackageViewModel
    {

        public int NetworkID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> StartupAmount { get; set; }
        public Nullable<int> MaxCycle { get; set; }
        public Nullable<int> MaxUnderlines { get; set; }
        public Nullable<decimal> Level1 { get; set; }
        public Nullable<decimal> Level2 { get; set; }
        public Nullable<decimal> Level3 { get; set; }
        public Nullable<decimal> CompanyAmount { get; set; }
        public Nullable<decimal> BonusAmount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        //public int NetworkID { get; set; }

        //[Required]
        //public string PackageName { get; set; }

        //[Required]
        //public decimal StartupAmount { get; set; }

        //public int MaxCycle { get; set; }

        //public int MaxUnderlines { get; set; }

        //public Nullable<DateTime> Created { get; set; }
        //public Nullable<DateTime> Updated { get; set; }

    }

    public class DownlineReportViewModel
    {

        public int MemberNetworkID { get; set; }

        [Required]
        public int PersonID { get; set; }

        [Required]
        public int NetworkID { get; set; }

        public string NetworkName { get; set; }

        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }

    }

    public class ReferralBoardViewModel
    {
        public Nullable<int> ReferralBoardID { get; set; }
        public long PersonID { get; set; }
        public string FullName { get; set; }
        public long ReferralID { get; set; }
        public long DirectReferralID { get; set; }
        public int NetworkID { get; set; }
        public string PackageName { get; set; }
        public DateTime Date { get; set; }
        public int BoardNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //public virtual Network Network { get; set; }
        public virtual Person Person { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int PercentageID { get; set; }
        public Nullable<int> NetworkID { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        public string PercentName { get; set; }
        public Nullable<double> Percents { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }

        //public virtual PayType PayType { get; set; }
    }

    public class PercentViewModel
    {
        public int PercentageID { get; set; }
        public Nullable<int> NetworkID { get; set; }
        public string PackageName { get; set; }
        public Nullable<int> PayTypeID { get; set; }
        public string PayTypeName { get; set; }
        public string PercentName { get; set; }
        public Nullable<double> Percents { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        //public virtual PayType PayType { get; set; }
        //public virtual Network Network { get; set; }
    }

    public class SignupViewModel
    {
        [Key]
        public int PersonID { get; set; }
        public int UserID { get; set; }

        [Required(ErrorMessage = "First Name is Required.")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required.")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is Required.")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
        public string EntityType { get; set; }
        public string ReferralCode { get; set; }
        public string ReferreeCode { get; set; }
        public string ParentCode { get; set; }
        public int MaxFirstLevel { get; set; }
    }

    public class SigninViewModel
    {
        [Required(ErrorMessage = "Login ID is Required.")]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Terms of Service is Required.")]
        public bool TOS { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyPhoneEmailViewModel
    {

        [Required(ErrorMessage = "Phone Verification Code is Required.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneCode { get; set; }

        //[Required(ErrorMessage = "Email Code is Required.")]
        public string EmailCode { get; set; }
        public string PhoneDetails { get; set; }

    }

    public class PricingViewModel
    {
        public int PricingID { get; set; }
        public int SchoolID { get; set; }
        public int FacultyID { get; set; }
        public string ClassName { get; set; }
        public int DurationID { get; set; }

        public string Duration { get; set; }
        public decimal Amount { get; set; }

    }

    public partial class PreSubscriptionViewModel
    {
        public long SubscriptionID { get; set; }
        public long PersonID { get; set; }
        public int QuizPricingID { get; set; }
        public int PastQuestionPricingID { get; set; }
        public int LibraryPricingID { get; set; }
        //[DisplayFormat(NullDisplayText = "N/A")]
        //public string Grade { get; set; }
        //[DisplayFormat(NullDisplayText = "N/A")]
        //public string Period { get; set; }
        //[DisplayFormat(NullDisplayText = "0.00")]
        //public decimal Amount { get; set; }
        //[DisplayFormat(NullDisplayText = "N/A")]
        //public System.DateTime StartDate { get; set; }
        //[DisplayFormat(NullDisplayText = "N/A")]
        //public System.DateTime EndDate { get; set; }
        //[DisplayFormat(NullDisplayText = "False")]
        //public System.DateTime IsActive { get; set; }

    }

    public partial class SubscriptionViewModel
    {
        public long SubscriptionID { get; set; }
        public long PersonID { get; set; }
        public int PricingID { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public string Grade { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public string Period { get; set; }
        [DisplayFormat(NullDisplayText = "0.00")]
        public decimal Amount { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public System.DateTime StartDate { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public System.DateTime EndDate { get; set; }
        [DisplayFormat(NullDisplayText = "False")]
        public System.DateTime IsActive { get; set; }

    }

    public partial class SubscriptionPreviewViewModel
    {
        public long SubscriptionID { get; set; }
        public long PersonID { get; set; }
        public int PricingID { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public string ResourceType { get; set; }
        public string Class { get; set; }
        [DisplayFormat(NullDisplayText = "N/A")]
        public string Duration { get; set; }
        [DisplayFormat(NullDisplayText = "0")]
        public Nullable<decimal> Amount { get; set; }
        //[DisplayFormat(NullDisplayText = "0.00")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }

        public System.Boolean IsActive { get; set; }

    }

    public class TimerViewModel
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "New Password is Required.")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required.")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

    }

    public class MyCodesViewModel
    {
        public string ReferreeCode { get; set; }
        public string ReferralCode { get; set; }
        public string ParentCode { get; set; }
    }

    public class ResultViewModel
    {
        public string Token { get; set; }
        public string Quiz { get; set; }
        public int TopicID { get; set; }
        public string Topic { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double TotalScores { get; set; }
        public double Scores { get; set; }
        public double TotalPoints { get; set; }
        public double Points { get; set; }
        public double Percentage { get; set; }
        public string Qualification { get; set; }
    }

    public class QResultViewModel
    {
        public string Client { get; set; }
        public string Token { get; set; }
        public string Quiz { get; set; }
        public int TopicID { get; set; }
        public string Topic { get; set; }
        public string QuizDuration { get; set; }
        public int QuizScores { get; set; }
        public string TimeSpent { get; set; }        
        public double TotalScores { get; set; }       
        public double TotalPoints { get; set; }
        public double Points { get; set; }
        public int Percentage { get; set; }
        public int TotalQuestions { get; set; }
        public int SuccessQuestion { get; set; }
        public int FailedQuestions { get; set; }
        public int Attemped { get; set; }

        public string Qualification { get; set; }
    }

    public partial class ResourceTypeViewModel
    {
        public int ResourceTypeID { get; set; }
        public string ResourceTypeName { get; set; }
        public string Description { get; set; }

    }

    public partial class IncomeViewModel
    {
        public long PaymentTransactionID { get; set; }
        public string Payer { get; set; }
        public string TransactionTypeName { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }

    }

    public partial class ClientPaymentViewModel
    {
        public int ClientPaymentID { get; set; }
        public string PayMode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
    }

    #region Paystack

    public class PayStackRequestModel
    {
        public string reference { get; set; }
        public string email { get; set; }
        public string amount { get; set; }
        public string secretKey { get; set; }
        public string callback_url { get; set; }
    }

    public class PayStackResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
        public string amount { get; set; }
        public string transaction_date { get; set; }
        public string status { get; set; }
        public string gateway_response { get; set; }
    }

    public class VerifyPayStackResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }


    #endregion
}