
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/11/2021 16:54:00
-- Generated from EDMX file: C:\Apps\Web\braingainspa\braingainspa\Models\bgsModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [bgspaQuiz];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Answers_Questions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Questions];
GO
IF OBJECT_ID(N'[dbo].[FK_Courses_Departments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Courses] DROP CONSTRAINT [FK_Courses_Departments];
GO
IF OBJECT_ID(N'[dbo].[FK_Courses_Faculties]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Courses] DROP CONSTRAINT [FK_Courses_Faculties];
GO
IF OBJECT_ID(N'[dbo].[FK_PastQuestions_Courses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PastQuestions] DROP CONSTRAINT [FK_PastQuestions_Courses];
GO
IF OBJECT_ID(N'[dbo].[FK_Topics_Courses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Topics] DROP CONSTRAINT [FK_Topics_Courses];
GO
IF OBJECT_ID(N'[dbo].[FK_Faculties_Schools]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Faculties] DROP CONSTRAINT [FK_Faculties_Schools];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[Addresses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Addresses];
GO
IF OBJECT_ID(N'[dbo].[Answers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Answers];
GO
IF OBJECT_ID(N'[dbo].[ClientPayments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientPayments];
GO
IF OBJECT_ID(N'[dbo].[ClientQuestions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientQuestions];
GO
IF OBJECT_ID(N'[dbo].[ClientQuizs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientQuizs];
GO
IF OBJECT_ID(N'[dbo].[Contacts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Contacts];
GO
IF OBJECT_ID(N'[dbo].[Courses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Courses];
GO
IF OBJECT_ID(N'[dbo].[Departments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departments];
GO
IF OBJECT_ID(N'[dbo].[Downlines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Downlines];
GO
IF OBJECT_ID(N'[dbo].[DurationInMonths]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DurationInMonths];
GO
IF OBJECT_ID(N'[dbo].[EntityTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityTypes];
GO
IF OBJECT_ID(N'[dbo].[Faculties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Faculties];
GO
IF OBJECT_ID(N'[dbo].[PastQuestions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PastQuestions];
GO
IF OBJECT_ID(N'[dbo].[PaymentModes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentModes];
GO
IF OBJECT_ID(N'[dbo].[PaymentTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentTransactions];
GO
IF OBJECT_ID(N'[dbo].[Percentages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Percentages];
GO
IF OBJECT_ID(N'[dbo].[Persons]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Persons];
GO
IF OBJECT_ID(N'[dbo].[Pricings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Pricings];
GO
IF OBJECT_ID(N'[dbo].[QuestionMarks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionMarks];
GO
IF OBJECT_ID(N'[dbo].[Questions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Questions];
GO
IF OBJECT_ID(N'[dbo].[ResourceTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ResourceTypes];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Schools]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schools];
GO
IF OBJECT_ID(N'[dbo].[SiteMenus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SiteMenus];
GO
IF OBJECT_ID(N'[dbo].[Subscriptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subscriptions];
GO
IF OBJECT_ID(N'[dbo].[Topics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Topics];
GO
IF OBJECT_ID(N'[dbo].[TransactionTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionTypes];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Years]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Years];
GO
IF OBJECT_ID(N'[dbo].[AcademicClasses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AcademicClasses];
GO
IF OBJECT_ID(N'[dbo].[AppSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppSettings];
GO
IF OBJECT_ID(N'[dbo].[BankPayments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BankPayments];
GO
IF OBJECT_ID(N'[dbo].[BANKS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BANKS];
GO
IF OBJECT_ID(N'[dbo].[Commissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Commissions];
GO
IF OBJECT_ID(N'[dbo].[Countries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Countries];
GO
IF OBJECT_ID(N'[dbo].[CourseDepartments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourseDepartments];
GO
IF OBJECT_ID(N'[dbo].[CoursePlus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CoursePlus];
GO
IF OBJECT_ID(N'[dbo].[FailedRegistrations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FailedRegistrations];
GO
IF OBJECT_ID(N'[dbo].[Genders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Genders];
GO
IF OBJECT_ID(N'[dbo].[Items]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Items];
GO
IF OBJECT_ID(N'[dbo].[MDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MDetails];
GO
IF OBJECT_ID(N'[dbo].[Modules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Modules];
GO
IF OBJECT_ID(N'[dbo].[Months]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Months];
GO
IF OBJECT_ID(N'[dbo].[Payments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Payments];
GO
IF OBJECT_ID(N'[dbo].[Purchases]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Purchases];
GO
IF OBJECT_ID(N'[dbo].[QuestionTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionTypes];
GO
IF OBJECT_ID(N'[dbo].[Sales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sales];
GO
IF OBJECT_ID(N'[dbo].[Sections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sections];
GO
IF OBJECT_ID(N'[dbo].[Semesters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Semesters];
GO
IF OBJECT_ID(N'[dbo].[States]', 'U') IS NOT NULL
    DROP TABLE [dbo].[States];
GO
IF OBJECT_ID(N'[dbo].[Student_Details]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Student_Details];
GO
IF OBJECT_ID(N'[dbo].[StudentCourses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudentCourses];
GO
IF OBJECT_ID(N'[dbo].[StudentPayments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudentPayments];
GO
IF OBJECT_ID(N'[dbo].[Titles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Titles];
GO
IF OBJECT_ID(N'[dbo].[Transactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Transactions];
GO
IF OBJECT_ID(N'[dbo].[Units]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Units];
GO
IF OBJECT_ID(N'[dbo].[ClientSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientSettings];
GO
IF OBJECT_ID(N'[dbo].[BankInfos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BankInfos];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [AccountID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [Purse] decimal(18,2)  NULL,
    [TempPurse] decimal(18,2)  NULL,
    [Withdrawals] decimal(18,2)  NULL,
    [IsCompany] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [Admin2] decimal(18,2)  NULL
);
GO

-- Creating table 'Addresses'
CREATE TABLE [dbo].[Addresses] (
    [AddressID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NOT NULL,
    [Address1] varchar(255)  NULL,
    [Description] varchar(255)  NULL,
    [City] varchar(50)  NULL,
    [StateID] smallint  NULL,
    [LGAID] smallint  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Answers'
CREATE TABLE [dbo].[Answers] (
    [AnswerID] int IDENTITY(1,1) NOT NULL,
    [QuestionID] int  NULL,
    [OptionLetter] nchar(1)  NULL,
    [AnswerString] nvarchar(max)  NULL,
    [IsCorrect] bit  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'ClientPayments'
CREATE TABLE [dbo].[ClientPayments] (
    [ClientPaymentID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [Mode] nvarchar(50)  NULL,
    [PaymentModeID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [Status] bit  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [Description] nvarchar(500)  NULL
);
GO

-- Creating table 'ClientQuestions'
CREATE TABLE [dbo].[ClientQuestions] (
    [ClientQuestionID] bigint IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [ClientQuizID] bigint  NULL,
    [Token] nvarchar(max)  NULL,
    [QuestionID] int  NULL,
    [QuestionNumber] int  NULL,
    [QuestionMarks] int  NULL,
    [AnswerID] int  NULL,
    [IsCorrect] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [Answered] bit  NULL,
    [ImagePath] nvarchar(max)  NULL
);
GO

-- Creating table 'ClientQuizs'
CREATE TABLE [dbo].[ClientQuizs] (
    [ClientQuizID] bigint IDENTITY(1,1) NOT NULL,
    [Token] nvarchar(max)  NULL,
    [PersonID] bigint  NULL,
    [TopicID] int  NULL,
    [ResourceTypeID] int  NULL,
    [QuestionCount] int  NULL,
    [Duration] int  NULL,
    [LastQuestionID] int  NULL,
    [StartTime] nvarchar(50)  NULL,
    [EndTime] nvarchar(50)  NULL,
    [TotalScore] float  NULL,
    [Score] float  NULL,
    [Percentage] float  NULL,
    [Qualification] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Contacts'
CREATE TABLE [dbo].[Contacts] (
    [ContactID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NOT NULL,
    [Phone] varchar(50)  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [AlternateEmail] nvarchar(max)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Courses'
CREATE TABLE [dbo].[Courses] (
    [CourseID] int IDENTITY(1,1) NOT NULL,
    [FacultyID] int  NULL,
    [DepartmentID] int  NULL,
    [SemesterID] int  NULL,
    [CYear] int  NULL,
    [MonthID] int  NULL,
    [YearID] int  NULL,
    [CourseCode] nvarchar(50)  NULL,
    [CourseName] nvarchar(1000)  NULL,
    [CreditUnits] int  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [DepartmentID] int IDENTITY(1,1) NOT NULL,
    [FacultyID] int  NULL,
    [DepartmentName] nvarchar(300)  NULL,
    [Description] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Downlines'
CREATE TABLE [dbo].[Downlines] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [Name] nvarchar(50)  NULL,
    [Pid] int  NULL,
    [PPid] nvarchar(50)  NULL,
    [Description] nvarchar(50)  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'DurationInMonths'
CREATE TABLE [dbo].[DurationInMonths] (
    [DurationID] int IDENTITY(1,1) NOT NULL,
    [Duration] int  NULL,
    [Description] nvarchar(100)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'EntityTypes'
CREATE TABLE [dbo].[EntityTypes] (
    [EntityTypeID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Description] varchar(50)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Faculties'
CREATE TABLE [dbo].[Faculties] (
    [FacultyID] int IDENTITY(1,1) NOT NULL,
    [SchoolID] int  NULL,
    [ResourceTypeID] int  NULL,
    [FacultyName] nvarchar(50)  NULL,
    [Description] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'PastQuestions'
CREATE TABLE [dbo].[PastQuestions] (
    [PastQuestionID] int IDENTITY(1,1) NOT NULL,
    [CourseID] int  NULL,
    [Year] int  NULL,
    [linktoYearId] int  NULL,
    [YearID] int  NULL,
    [DurationInMinutes] int  NULL,
    [Description] nvarchar(1000)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'PaymentModes'
CREATE TABLE [dbo].[PaymentModes] (
    [PaymentModeID] int IDENTITY(1,1) NOT NULL,
    [PaymentModeName] nvarchar(50)  NULL,
    [Description] nvarchar(500)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'PaymentTransactions'
CREATE TABLE [dbo].[PaymentTransactions] (
    [PaymentTransactionID] bigint IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [RefereeCode] char(5)  NULL,
    [ReferralCode] char(5)  NULL,
    [TransactionTypeID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [Level] int  NULL,
    [NLevel] int  NULL,
    [Comment] nvarchar(100)  NULL,
    [TempFlag] bit  NULL,
    [AcctFlag] bit  NULL,
    [ToCompanyFlag] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Percentages'
CREATE TABLE [dbo].[Percentages] (
    [PercentID] int IDENTITY(1,1) NOT NULL,
    [PercentName] nvarchar(50)  NULL,
    [PValue] nvarchar(50)  NULL,
    [PType] nvarchar(50)  NULL,
    [Amount] decimal(18,2)  NULL,
    [Percentage1] float  NULL,
    [Registration] float  NULL,
    [Subscription] float  NULL,
    [PAmount] decimal(18,2)  NULL,
    [PLevel] bit  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Persons'
CREATE TABLE [dbo].[Persons] (
    [PersonID] bigint IDENTITY(1,1) NOT NULL,
    [EntityTypeID] int  NULL,
    [TitleID] int  NULL,
    [LastName] nvarchar(50)  NULL,
    [MiddleName] nvarchar(50)  NULL,
    [FirstName] nvarchar(50)  NULL,
    [GenderID] int  NULL,
    [DOB] datetime  NULL,
    [ReferralCode] char(5)  NULL,
    [AcademicClassID] int  NULL,
    [SemesterID] int  NULL,
    [PayMode] int  NULL,
    [RefereeCode] char(5)  NULL,
    [ParentCode] nchar(6)  NULL,
    [ActiveSubscriptionID] bigint  NULL,
    [IsCompany] bit  NULL,
    [MaxFirstLevel] int  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [ImagePath] nvarchar(max)  NULL
);
GO

-- Creating table 'Pricings'
CREATE TABLE [dbo].[Pricings] (
    [PricingID] int IDENTITY(1,1) NOT NULL,
    [SchoolID] int  NULL,
    [FacultyID] int  NULL,
    [DurationID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [HasMock] bit  NULL
);
GO

-- Creating table 'QuestionMarks'
CREATE TABLE [dbo].[QuestionMarks] (
    [QuestionMarkID] int IDENTITY(1,1) NOT NULL,
    [QuestionMark1] int  NULL,
    [Description] nvarchar(100)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [QuestionID] int IDENTITY(1,1) NOT NULL,
    [CourseID] int  NULL,
    [TopicID] int  NULL,
    [QuestionTypeID] int  NULL,
    [ResourceTypeID] int  NULL,
    [CoursePlusID] int  NULL,
    [SectionID] int  NULL,
    [QuestionNumber] int  NULL,
    [Number] nvarchar(50)  NULL,
    [QuestionString] nvarchar(max)  NULL,
    [Answer] nvarchar(250)  NULL,
    [QuestionMarks] int  NULL,
    [CorrectAnswer] nvarchar(50)  NULL,
    [AnswerExplanation] nvarchar(max)  NULL,
    [QuestionDuration] int  NULL,
    [Year] int  NULL,
    [IsVerified] bit  NULL,
    [Instruction] nvarchar(1000)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [IsMock] bit  NULL,
    [ImagePath] nvarchar(max)  NULL,
    [TopicName] nvarchar(50)  NULL,
    [InserterID] bigint  NULL,
    [UpdaterID] bigint  NULL
);
GO

-- Creating table 'ResourceTypes'
CREATE TABLE [dbo].[ResourceTypes] (
    [ResourceTypeID] int IDENTITY(1,1) NOT NULL,
    [ResourceTypeName] nvarchar(50)  NULL,
    [Description] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [RoleID] int IDENTITY(1,1) NOT NULL,
    [RoleName] varchar(50)  NULL,
    [Description] varchar(200)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Schools'
CREATE TABLE [dbo].[Schools] (
    [SchoolID] int IDENTITY(1,1) NOT NULL,
    [SchoolName] nvarchar(100)  NULL,
    [Address] nvarchar(max)  NULL,
    [Phone] nvarchar(50)  NULL,
    [Email] nvarchar(255)  NULL,
    [Website] nvarchar(50)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'SiteMenus'
CREATE TABLE [dbo].[SiteMenus] (
    [MenuID] int IDENTITY(1,1) NOT NULL,
    [MenuName] nvarchar(100)  NULL,
    [NavURL] nvarchar(200)  NULL,
    [ParentMenuID] int  NULL
);
GO

-- Creating table 'Subscriptions'
CREATE TABLE [dbo].[Subscriptions] (
    [SubscriptionID] bigint IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [ResourceTypeID] int  NULL,
    [PricingID] int  NULL,
    [Duration] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [IsActive] bit  NULL,
    [Status] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Topics'
CREATE TABLE [dbo].[Topics] (
    [TopicID] int IDENTITY(1,1) NOT NULL,
    [CourseID] int  NULL,
    [TopicName] nvarchar(1000)  NULL,
    [DurationInMinutes] int  NULL,
    [Description] nvarchar(1000)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'TransactionTypes'
CREATE TABLE [dbo].[TransactionTypes] (
    [TransactionTypeID] int IDENTITY(1,1) NOT NULL,
    [TransactionTypeName] varchar(50)  NULL,
    [Description] varchar(50)  NULL,
    [Investment] bit  NULL,
    [Subscription] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [UserID] bigint IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [RoleID] int  NULL,
    [UserName] nvarchar(850)  NOT NULL,
    [PhoneNumber] nvarchar(11)  NULL,
    [Password] nvarchar(80)  NOT NULL,
    [StatusTypeID] tinyint  NULL,
    [FirstTimeLogin] tinyint  NULL,
    [IsActive] bit  NULL,
    [ConfirmEmail] bit  NULL,
    [ConfirmPhone] bit  NULL,
    [PhoneCode] nvarchar(10)  NULL,
    [EmailCode] nvarchar(10)  NULL,
    [IsRegistered] bit  NULL,
    [Deactivated] bit  NULL,
    [LoginCode] nvarchar(50)  NULL,
    [Used] bit  NULL,
    [LastLoginDate] datetime  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Years'
CREATE TABLE [dbo].[Years] (
    [YearID] int IDENTITY(1,1) NOT NULL,
    [YearName] int  NULL,
    [IsVisible] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'AcademicClasses'
CREATE TABLE [dbo].[AcademicClasses] (
    [AcademicClassID] int  NOT NULL,
    [AcademicClassName] varchar(50)  NULL,
    [Description] varchar(50)  NULL,
    [Amount] decimal(18,2)  NULL,
    [IsVisible] bit  NULL,
    [IsDelete] bit  NOT NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'AppSettings'
CREATE TABLE [dbo].[AppSettings] (
    [ID] int  NOT NULL,
    [CourseFees] decimal(18,2)  NULL,
    [RegistrationFee] decimal(18,2)  NULL,
    [RegistrationFee2] decimal(18,2)  NULL,
    [DefaultReferralCode] char(5)  NULL,
    [ReferralCodeEarning] decimal(18,2)  NULL,
    [MaxUnderline] int  NULL,
    [MaxFirstLevel] int  NULL,
    [CompRegPercent] float  NULL,
    [CompSubPercent] float  NULL,
    [CurrentSemester] int  NULL,
    [CurrentSession] int  NULL,
    [NoReferralAmount] decimal(18,2)  NULL,
    [MaximumOptions] int  NULL,
    [QuestionsBatches] int  NULL,
    [SupportPhone] nvarchar(20)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [MinSubscriptionAmt] decimal(18,2)  NULL,
    [QuizPeriod] float  NULL,
    [MockPeriod] float  NULL,
    [PRYQuizPeriod] float  NULL,
    [JSSQuizPeriod] float  NULL,
    [SSSQuizPeriod] float  NULL
);
GO

-- Creating table 'BankPayments'
CREATE TABLE [dbo].[BankPayments] (
    [BankPaymentID] int  NOT NULL,
    [PersonID] bigint  NULL,
    [FullName] nvarchar(100)  NULL,
    [GSM] nvarchar(15)  NULL,
    [TellerNumber] nvarchar(50)  NULL,
    [Amount] decimal(18,2)  NULL,
    [Branch] nvarchar(50)  NULL,
    [Confirmed] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'BANKS'
CREATE TABLE [dbo].[BANKS] (
    [BANK1] varchar(100)  NOT NULL,
    [BANK_ID] smallint  NOT NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'Commissions'
CREATE TABLE [dbo].[Commissions] (
    [CommissionID] int  NOT NULL,
    [TranssactionID] bigint  NULL,
    [Amount] decimal(18,2)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Countries'
CREATE TABLE [dbo].[Countries] (
    [CountryID] int  NOT NULL,
    [Country1] nvarchar(50)  NULL
);
GO

-- Creating table 'CourseDepartments'
CREATE TABLE [dbo].[CourseDepartments] (
    [CourseDepartmentID] int  NOT NULL,
    [CourseID] int  NULL,
    [DepartmentID] int  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'CoursePlus'
CREATE TABLE [dbo].[CoursePlus] (
    [CoursePlusID] int  NOT NULL,
    [CourseID] int  NULL,
    [SemesterID] int  NULL,
    [MthID] int  NULL,
    [MonthID] int  NULL,
    [YearID] int  NULL,
    [imgpath] nvarchar(max)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'FailedRegistrations'
CREATE TABLE [dbo].[FailedRegistrations] (
    [ID] int  NOT NULL,
    [FirstName] nvarchar(50)  NULL,
    [LastName] nvarchar(50)  NULL,
    [Phone] nvarchar(50)  NULL,
    [Email] nvarchar(250)  NULL,
    [Excluded] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Genders'
CREATE TABLE [dbo].[Genders] (
    [GenderID] int  NOT NULL,
    [Gender1] nvarchar(6)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Items'
CREATE TABLE [dbo].[Items] (
    [ItemID] int  NOT NULL,
    [ItemName] nvarchar(50)  NULL,
    [Cost] decimal(18,2)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'MDetails'
CREATE TABLE [dbo].[MDetails] (
    [MDetailID] int  NOT NULL,
    [PersonID] bigint  NULL,
    [AccountTypeID] tinyint  NULL,
    [AccountName] nvarchar(100)  NULL,
    [BankID] int  NULL,
    [AccountNumber] nchar(10)  NULL,
    [BankName] nvarchar(50)  NULL,
    [CardNumber] nvarchar(50)  NULL,
    [ExpiryDate] nvarchar(50)  NULL,
    [CVV] nchar(3)  NULL,
    [DefaultAccount] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Modules'
CREATE TABLE [dbo].[Modules] (
    [ModuleID] int  NOT NULL,
    [CourseID] int  NULL,
    [MContent] nvarchar(50)  NULL,
    [Description] nvarchar(100)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Months'
CREATE TABLE [dbo].[Months] (
    [MonthID] int  NOT NULL,
    [MonthName] nvarchar(50)  NULL,
    [IsVisible] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Payments'
CREATE TABLE [dbo].[Payments] (
    [PaymentID] int  NOT NULL,
    [PersonID] bigint  NULL,
    [NetworkID] int  NULL,
    [ReferralID] bigint  NULL,
    [DirectReferralID] bigint  NULL,
    [BankID] int  NULL,
    [Date] datetime  NULL,
    [Mode] nchar(10)  NULL,
    [ChequeNumber] varchar(50)  NULL,
    [Amount] decimal(18,2)  NULL,
    [Paid] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'Purchases'
CREATE TABLE [dbo].[Purchases] (
    [PurchaseID] int  NOT NULL,
    [ItemID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'QuestionTypes'
CREATE TABLE [dbo].[QuestionTypes] (
    [QuestionTypeID] int  NOT NULL,
    [QuestionType1] nvarchar(50)  NULL,
    [IsVisible] bit  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Sales'
CREATE TABLE [dbo].[Sales] (
    [SalesID] bigint  NOT NULL,
    [ItemID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [Craeted] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Sections'
CREATE TABLE [dbo].[Sections] (
    [SectionID] int  NOT NULL,
    [UnitID] int  NULL,
    [SN] nvarchar(5)  NULL,
    [Title] nvarchar(max)  NULL,
    [SContent] nvarchar(max)  NULL,
    [Name] nvarchar(200)  NULL,
    [ContentType] nvarchar(500)  NULL,
    [Data] varbinary(max)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Semesters'
CREATE TABLE [dbo].[Semesters] (
    [SemesterID] int  NOT NULL,
    [Semester1] nvarchar(50)  NULL,
    [IsVisible] bit  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'States'
CREATE TABLE [dbo].[States] (
    [StateID] int  NOT NULL,
    [State1] nvarchar(50)  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [CountryID] int  NOT NULL
);
GO

-- Creating table 'Student_Details'
CREATE TABLE [dbo].[Student_Details] (
    [StudentDetailID] bigint  NOT NULL,
    [PersonID] bigint  NULL,
    [SchoolID] int  NULL,
    [FacultyID] int  NULL,
    [DepartmentID] int  NULL,
    [LevelID] int  NULL,
    [SemesterID] int  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'StudentCourses'
CREATE TABLE [dbo].[StudentCourses] (
    [StudentCourseID] bigint  NOT NULL,
    [StudentDetailID] bigint  NULL,
    [CourseID] int  NULL,
    [Year] int  NULL,
    [TutorialScore] float  NULL,
    [ExamScore] float  NULL,
    [IsDeleted] bit  NULL,
    [IsActive] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'StudentPayments'
CREATE TABLE [dbo].[StudentPayments] (
    [StudentPaymentID] bigint  NOT NULL,
    [PersonID] bigint  NULL,
    [LevelID] int  NULL,
    [SemesterID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Titles'
CREATE TABLE [dbo].[Titles] (
    [TitleID] tinyint  NOT NULL,
    [Title1] nvarchar(255)  NOT NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Transactions'
CREATE TABLE [dbo].[Transactions] (
    [TransactionID] bigint  NOT NULL,
    [PersonID] bigint  NULL,
    [CommitID] int  NULL,
    [TransactionTypeID] int  NULL,
    [Amount] decimal(18,2)  NULL,
    [Credit] decimal(18,2)  NULL,
    [Debit] decimal(18,2)  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'Units'
CREATE TABLE [dbo].[Units] (
    [UnitID] int  NOT NULL,
    [CourseID] int  NULL,
    [ModuleID] int  NULL,
    [UnitDetails] nvarchar(max)  NULL,
    [Description] nvarchar(100)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'ClientSettings'
CREATE TABLE [dbo].[ClientSettings] (
    [UserSettingID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NULL,
    [WithdrawalPercent] int  NULL,
    [Craeted] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- Creating table 'BankInfos'
CREATE TABLE [dbo].[BankInfos] (
    [BankInfoID] int IDENTITY(1,1) NOT NULL,
    [PersonID] bigint  NOT NULL,
    [BankID] smallint  NULL,
    [BranchName] nvarchar(100)  NULL,
    [AccountName] nvarchar(100)  NULL,
    [AccountNumber] nchar(10)  NULL,
    [IsDeleted] bit  NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AccountID] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([AccountID] ASC);
GO

-- Creating primary key on [AddressID] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [PK_Addresses]
    PRIMARY KEY CLUSTERED ([AddressID] ASC);
GO

-- Creating primary key on [AnswerID] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [PK_Answers]
    PRIMARY KEY CLUSTERED ([AnswerID] ASC);
GO

-- Creating primary key on [ClientPaymentID] in table 'ClientPayments'
ALTER TABLE [dbo].[ClientPayments]
ADD CONSTRAINT [PK_ClientPayments]
    PRIMARY KEY CLUSTERED ([ClientPaymentID] ASC);
GO

-- Creating primary key on [ClientQuestionID] in table 'ClientQuestions'
ALTER TABLE [dbo].[ClientQuestions]
ADD CONSTRAINT [PK_ClientQuestions]
    PRIMARY KEY CLUSTERED ([ClientQuestionID] ASC);
GO

-- Creating primary key on [ClientQuizID] in table 'ClientQuizs'
ALTER TABLE [dbo].[ClientQuizs]
ADD CONSTRAINT [PK_ClientQuizs]
    PRIMARY KEY CLUSTERED ([ClientQuizID] ASC);
GO

-- Creating primary key on [ContactID] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [PK_Contacts]
    PRIMARY KEY CLUSTERED ([ContactID] ASC);
GO

-- Creating primary key on [CourseID] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [PK_Courses]
    PRIMARY KEY CLUSTERED ([CourseID] ASC);
GO

-- Creating primary key on [DepartmentID] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([DepartmentID] ASC);
GO

-- Creating primary key on [ID] in table 'Downlines'
ALTER TABLE [dbo].[Downlines]
ADD CONSTRAINT [PK_Downlines]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [DurationID] in table 'DurationInMonths'
ALTER TABLE [dbo].[DurationInMonths]
ADD CONSTRAINT [PK_DurationInMonths]
    PRIMARY KEY CLUSTERED ([DurationID] ASC);
GO

-- Creating primary key on [EntityTypeID] in table 'EntityTypes'
ALTER TABLE [dbo].[EntityTypes]
ADD CONSTRAINT [PK_EntityTypes]
    PRIMARY KEY CLUSTERED ([EntityTypeID] ASC);
GO

-- Creating primary key on [FacultyID] in table 'Faculties'
ALTER TABLE [dbo].[Faculties]
ADD CONSTRAINT [PK_Faculties]
    PRIMARY KEY CLUSTERED ([FacultyID] ASC);
GO

-- Creating primary key on [PastQuestionID] in table 'PastQuestions'
ALTER TABLE [dbo].[PastQuestions]
ADD CONSTRAINT [PK_PastQuestions]
    PRIMARY KEY CLUSTERED ([PastQuestionID] ASC);
GO

-- Creating primary key on [PaymentModeID] in table 'PaymentModes'
ALTER TABLE [dbo].[PaymentModes]
ADD CONSTRAINT [PK_PaymentModes]
    PRIMARY KEY CLUSTERED ([PaymentModeID] ASC);
GO

-- Creating primary key on [PaymentTransactionID] in table 'PaymentTransactions'
ALTER TABLE [dbo].[PaymentTransactions]
ADD CONSTRAINT [PK_PaymentTransactions]
    PRIMARY KEY CLUSTERED ([PaymentTransactionID] ASC);
GO

-- Creating primary key on [PercentID] in table 'Percentages'
ALTER TABLE [dbo].[Percentages]
ADD CONSTRAINT [PK_Percentages]
    PRIMARY KEY CLUSTERED ([PercentID] ASC);
GO

-- Creating primary key on [PersonID] in table 'Persons'
ALTER TABLE [dbo].[Persons]
ADD CONSTRAINT [PK_Persons]
    PRIMARY KEY CLUSTERED ([PersonID] ASC);
GO

-- Creating primary key on [PricingID] in table 'Pricings'
ALTER TABLE [dbo].[Pricings]
ADD CONSTRAINT [PK_Pricings]
    PRIMARY KEY CLUSTERED ([PricingID] ASC);
GO

-- Creating primary key on [QuestionMarkID] in table 'QuestionMarks'
ALTER TABLE [dbo].[QuestionMarks]
ADD CONSTRAINT [PK_QuestionMarks]
    PRIMARY KEY CLUSTERED ([QuestionMarkID] ASC);
GO

-- Creating primary key on [QuestionID] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([QuestionID] ASC);
GO

-- Creating primary key on [ResourceTypeID] in table 'ResourceTypes'
ALTER TABLE [dbo].[ResourceTypes]
ADD CONSTRAINT [PK_ResourceTypes]
    PRIMARY KEY CLUSTERED ([ResourceTypeID] ASC);
GO

-- Creating primary key on [RoleID] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([RoleID] ASC);
GO

-- Creating primary key on [SchoolID] in table 'Schools'
ALTER TABLE [dbo].[Schools]
ADD CONSTRAINT [PK_Schools]
    PRIMARY KEY CLUSTERED ([SchoolID] ASC);
GO

-- Creating primary key on [MenuID] in table 'SiteMenus'
ALTER TABLE [dbo].[SiteMenus]
ADD CONSTRAINT [PK_SiteMenus]
    PRIMARY KEY CLUSTERED ([MenuID] ASC);
GO

-- Creating primary key on [SubscriptionID] in table 'Subscriptions'
ALTER TABLE [dbo].[Subscriptions]
ADD CONSTRAINT [PK_Subscriptions]
    PRIMARY KEY CLUSTERED ([SubscriptionID] ASC);
GO

-- Creating primary key on [TopicID] in table 'Topics'
ALTER TABLE [dbo].[Topics]
ADD CONSTRAINT [PK_Topics]
    PRIMARY KEY CLUSTERED ([TopicID] ASC);
GO

-- Creating primary key on [TransactionTypeID] in table 'TransactionTypes'
ALTER TABLE [dbo].[TransactionTypes]
ADD CONSTRAINT [PK_TransactionTypes]
    PRIMARY KEY CLUSTERED ([TransactionTypeID] ASC);
GO

-- Creating primary key on [UserID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UserID] ASC);
GO

-- Creating primary key on [YearID] in table 'Years'
ALTER TABLE [dbo].[Years]
ADD CONSTRAINT [PK_Years]
    PRIMARY KEY CLUSTERED ([YearID] ASC);
GO

-- Creating primary key on [AcademicClassID], [IsDelete] in table 'AcademicClasses'
ALTER TABLE [dbo].[AcademicClasses]
ADD CONSTRAINT [PK_AcademicClasses]
    PRIMARY KEY CLUSTERED ([AcademicClassID], [IsDelete] ASC);
GO

-- Creating primary key on [ID] in table 'AppSettings'
ALTER TABLE [dbo].[AppSettings]
ADD CONSTRAINT [PK_AppSettings]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [BankPaymentID] in table 'BankPayments'
ALTER TABLE [dbo].[BankPayments]
ADD CONSTRAINT [PK_BankPayments]
    PRIMARY KEY CLUSTERED ([BankPaymentID] ASC);
GO

-- Creating primary key on [BANK1], [BANK_ID] in table 'BANKS'
ALTER TABLE [dbo].[BANKS]
ADD CONSTRAINT [PK_BANKS]
    PRIMARY KEY CLUSTERED ([BANK1], [BANK_ID] ASC);
GO

-- Creating primary key on [CommissionID] in table 'Commissions'
ALTER TABLE [dbo].[Commissions]
ADD CONSTRAINT [PK_Commissions]
    PRIMARY KEY CLUSTERED ([CommissionID] ASC);
GO

-- Creating primary key on [CountryID] in table 'Countries'
ALTER TABLE [dbo].[Countries]
ADD CONSTRAINT [PK_Countries]
    PRIMARY KEY CLUSTERED ([CountryID] ASC);
GO

-- Creating primary key on [CourseDepartmentID] in table 'CourseDepartments'
ALTER TABLE [dbo].[CourseDepartments]
ADD CONSTRAINT [PK_CourseDepartments]
    PRIMARY KEY CLUSTERED ([CourseDepartmentID] ASC);
GO

-- Creating primary key on [CoursePlusID] in table 'CoursePlus'
ALTER TABLE [dbo].[CoursePlus]
ADD CONSTRAINT [PK_CoursePlus]
    PRIMARY KEY CLUSTERED ([CoursePlusID] ASC);
GO

-- Creating primary key on [ID] in table 'FailedRegistrations'
ALTER TABLE [dbo].[FailedRegistrations]
ADD CONSTRAINT [PK_FailedRegistrations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [GenderID] in table 'Genders'
ALTER TABLE [dbo].[Genders]
ADD CONSTRAINT [PK_Genders]
    PRIMARY KEY CLUSTERED ([GenderID] ASC);
GO

-- Creating primary key on [ItemID] in table 'Items'
ALTER TABLE [dbo].[Items]
ADD CONSTRAINT [PK_Items]
    PRIMARY KEY CLUSTERED ([ItemID] ASC);
GO

-- Creating primary key on [MDetailID] in table 'MDetails'
ALTER TABLE [dbo].[MDetails]
ADD CONSTRAINT [PK_MDetails]
    PRIMARY KEY CLUSTERED ([MDetailID] ASC);
GO

-- Creating primary key on [ModuleID] in table 'Modules'
ALTER TABLE [dbo].[Modules]
ADD CONSTRAINT [PK_Modules]
    PRIMARY KEY CLUSTERED ([ModuleID] ASC);
GO

-- Creating primary key on [MonthID] in table 'Months'
ALTER TABLE [dbo].[Months]
ADD CONSTRAINT [PK_Months]
    PRIMARY KEY CLUSTERED ([MonthID] ASC);
GO

-- Creating primary key on [PaymentID] in table 'Payments'
ALTER TABLE [dbo].[Payments]
ADD CONSTRAINT [PK_Payments]
    PRIMARY KEY CLUSTERED ([PaymentID] ASC);
GO

-- Creating primary key on [PurchaseID] in table 'Purchases'
ALTER TABLE [dbo].[Purchases]
ADD CONSTRAINT [PK_Purchases]
    PRIMARY KEY CLUSTERED ([PurchaseID] ASC);
GO

-- Creating primary key on [QuestionTypeID] in table 'QuestionTypes'
ALTER TABLE [dbo].[QuestionTypes]
ADD CONSTRAINT [PK_QuestionTypes]
    PRIMARY KEY CLUSTERED ([QuestionTypeID] ASC);
GO

-- Creating primary key on [SalesID] in table 'Sales'
ALTER TABLE [dbo].[Sales]
ADD CONSTRAINT [PK_Sales]
    PRIMARY KEY CLUSTERED ([SalesID] ASC);
GO

-- Creating primary key on [SectionID] in table 'Sections'
ALTER TABLE [dbo].[Sections]
ADD CONSTRAINT [PK_Sections]
    PRIMARY KEY CLUSTERED ([SectionID] ASC);
GO

-- Creating primary key on [SemesterID] in table 'Semesters'
ALTER TABLE [dbo].[Semesters]
ADD CONSTRAINT [PK_Semesters]
    PRIMARY KEY CLUSTERED ([SemesterID] ASC);
GO

-- Creating primary key on [StateID], [State1], [Code], [CountryID] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [PK_States]
    PRIMARY KEY CLUSTERED ([StateID], [State1], [Code], [CountryID] ASC);
GO

-- Creating primary key on [StudentDetailID] in table 'Student_Details'
ALTER TABLE [dbo].[Student_Details]
ADD CONSTRAINT [PK_Student_Details]
    PRIMARY KEY CLUSTERED ([StudentDetailID] ASC);
GO

-- Creating primary key on [StudentCourseID] in table 'StudentCourses'
ALTER TABLE [dbo].[StudentCourses]
ADD CONSTRAINT [PK_StudentCourses]
    PRIMARY KEY CLUSTERED ([StudentCourseID] ASC);
GO

-- Creating primary key on [StudentPaymentID] in table 'StudentPayments'
ALTER TABLE [dbo].[StudentPayments]
ADD CONSTRAINT [PK_StudentPayments]
    PRIMARY KEY CLUSTERED ([StudentPaymentID] ASC);
GO

-- Creating primary key on [TitleID], [Title1] in table 'Titles'
ALTER TABLE [dbo].[Titles]
ADD CONSTRAINT [PK_Titles]
    PRIMARY KEY CLUSTERED ([TitleID], [Title1] ASC);
GO

-- Creating primary key on [TransactionID] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [PK_Transactions]
    PRIMARY KEY CLUSTERED ([TransactionID] ASC);
GO

-- Creating primary key on [UnitID] in table 'Units'
ALTER TABLE [dbo].[Units]
ADD CONSTRAINT [PK_Units]
    PRIMARY KEY CLUSTERED ([UnitID] ASC);
GO

-- Creating primary key on [UserSettingID] in table 'ClientSettings'
ALTER TABLE [dbo].[ClientSettings]
ADD CONSTRAINT [PK_ClientSettings]
    PRIMARY KEY CLUSTERED ([UserSettingID] ASC);
GO

-- Creating primary key on [BankInfoID] in table 'BankInfos'
ALTER TABLE [dbo].[BankInfos]
ADD CONSTRAINT [PK_BankInfos]
    PRIMARY KEY CLUSTERED ([BankInfoID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [QuestionID] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [FK_Answers_Questions]
    FOREIGN KEY ([QuestionID])
    REFERENCES [dbo].[Questions]
        ([QuestionID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answers_Questions'
CREATE INDEX [IX_FK_Answers_Questions]
ON [dbo].[Answers]
    ([QuestionID]);
GO

-- Creating foreign key on [DepartmentID] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [FK_Courses_Departments]
    FOREIGN KEY ([DepartmentID])
    REFERENCES [dbo].[Departments]
        ([DepartmentID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Courses_Departments'
CREATE INDEX [IX_FK_Courses_Departments]
ON [dbo].[Courses]
    ([DepartmentID]);
GO

-- Creating foreign key on [FacultyID] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [FK_Courses_Faculties]
    FOREIGN KEY ([FacultyID])
    REFERENCES [dbo].[Faculties]
        ([FacultyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Courses_Faculties'
CREATE INDEX [IX_FK_Courses_Faculties]
ON [dbo].[Courses]
    ([FacultyID]);
GO

-- Creating foreign key on [CourseID] in table 'PastQuestions'
ALTER TABLE [dbo].[PastQuestions]
ADD CONSTRAINT [FK_PastQuestions_Courses]
    FOREIGN KEY ([CourseID])
    REFERENCES [dbo].[Courses]
        ([CourseID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PastQuestions_Courses'
CREATE INDEX [IX_FK_PastQuestions_Courses]
ON [dbo].[PastQuestions]
    ([CourseID]);
GO

-- Creating foreign key on [CourseID] in table 'Topics'
ALTER TABLE [dbo].[Topics]
ADD CONSTRAINT [FK_Topics_Courses]
    FOREIGN KEY ([CourseID])
    REFERENCES [dbo].[Courses]
        ([CourseID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Topics_Courses'
CREATE INDEX [IX_FK_Topics_Courses]
ON [dbo].[Topics]
    ([CourseID]);
GO

-- Creating foreign key on [SchoolID] in table 'Faculties'
ALTER TABLE [dbo].[Faculties]
ADD CONSTRAINT [FK_Faculties_Schools]
    FOREIGN KEY ([SchoolID])
    REFERENCES [dbo].[Schools]
        ([SchoolID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Faculties_Schools'
CREATE INDEX [IX_FK_Faculties_Schools]
ON [dbo].[Faculties]
    ([SchoolID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------