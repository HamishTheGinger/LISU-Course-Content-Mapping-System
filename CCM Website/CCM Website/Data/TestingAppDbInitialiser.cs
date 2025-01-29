using CCM_Website.Models;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using NuGet.Protocol.Plugins;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;

namespace CCM_Website.Data
{
    public class TestingAppDbInitialiser
    {
        public static void TestingSeed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;

                context.Database.EnsureCreated();

                if (!context.Workbooks.Any())
                {
                    // Learning Platforms
                    var moodle = context.LearningPlatforms.First(lp => lp.PlatformName == "Moodle");
                    var coursera = context.LearningPlatforms.First(lp => lp.PlatformName == "Coursera");
                    var futureLearn = context.LearningPlatforms.First(lp => lp.PlatformName == "FutureLearn");
                    var xSite = context.LearningPlatforms.First(lp => lp.PlatformName == "xSiTe");
                    var COIL = context.LearningPlatforms.First(lp => lp.PlatformName == "COIL");

                    var workbooks = new[]
                    {
                        new Workbook {  CourseName = "Generative AI for Students: Ethics & Academic Integrity",
                                        CourseLead = "Jennifer Boyle",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI5001",
                                        Collaborators = "Caitlin Diver, Scott Ramsay, Andrew Struan",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Algorithmics",
                                        CourseLead = "Gethin Norman",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4009",
                                        Collaborators = "John Smith, Jack Smith",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Systems Programming",
                                        CourseLead = "Phil Trinder",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4081",
                                        Collaborators = "Nikela Papadopoulou",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Data Fundamentals",
                                        CourseLead = "John Williamson",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4073",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Human Centred Systems Design and Evaluation",
                                        CourseLead = "Alistair Morrison",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4014",
                                        Collaborators = "Xianghua (Sharon) Ding",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Introduction to Gnu/Linux",
                                        CourseLead = "Jeremy Singer",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI9001",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Professional Software Development",
                                        CourseLead = "Tim Storer",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4015",
                                        Collaborators = "Peggy Gregory",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Database Systems",
                                        CourseLead = "Dr Chris Anagnostopoulos",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4013",
                                        Collaborators = "Qiyuan Wang",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Cyber Security Fundamentals",
                                        CourseLead = "Thomas Zacharias",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4062",
                                        Collaborators = "Ghadeer Alsharif, Kai Feng",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Robotics Foundations",
                                        CourseLead = "Dr Gerardo Aragon-Camarasa",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4076",
                                        Collaborators = "Dr J. Paul Siebert",
                                        LearningPlatform = moodle},
                        new Workbook {  CourseName = "Text as Data",
                                        CourseLead = "Dr Jake Lever",
                                        CourseLength = 11,
                                        LastEdited = DateTime.Now,
                                        CourseCode = "COMPSCI4074",
                                        LearningPlatform = moodle},
                    };

                    context.Workbooks.AddRange(workbooks);
                    context.SaveChanges();
                }

                if (!context.Weeks.Any())
                {
                    // Workbooks
                    var Algorithmics = context.Workbooks.First(wb => wb.CourseName == "Algorithmics");
                    var genAI = context.Workbooks.First(wb => wb.CourseName == "Generative AI for Students: Ethics & Academic Integrity");

                    var weeks = new[]
                    {
                        // Generative AI for Students: Ethics & Academic Integrity
                        new Week { WeekNumber = 1, Workbook = genAI },
                        new Week { WeekNumber = 2, Workbook = genAI },
                        new Week { WeekNumber = 3, Workbook = genAI },
                    };

                    context.Weeks.AddRange(weeks);
                    context.SaveChanges();
                }

                if (!context.WeekGraduateAttributes.Any())
                {
                    // Workbooks
                    var genAI = context.Workbooks.First(wb => wb.CourseName == "Generative AI for Students: Ethics & Academic Integrity");

                    // GraduateAttributes
                    var Adaptable = context.GraduateAttributes.First(ga => ga.AttributeName == "Adaptable");
                    var Confident = context.GraduateAttributes.First(ga => ga.AttributeName == "Confident");
                    var EffectiveCommunication = context.GraduateAttributes.First(ga => ga.AttributeName == "Effective Communication");
                    var EthicallyandSociallyAware = context.GraduateAttributes.First(ga => ga.AttributeName == "Ethically and Socially Aware");
                    var ExperiencedCollaborators = context.GraduateAttributes.First(ga => ga.AttributeName == "Experienced Collaborators");
                    var IndependentandCriticalThinkers = context.GraduateAttributes.First(ga => ga.AttributeName == "Independent and Critical Thinkers");
                    var Investigative = context.GraduateAttributes.First(ga => ga.AttributeName == "Investigative");
                    var ReflectiveLearners = context.GraduateAttributes.First(ga => ga.AttributeName == "Reflective Learners");
                    var ResourcefulandResponsible = context.GraduateAttributes.First(ga => ga.AttributeName == "Resourceful and Responsible");
                    var SubjectSpecialists = context.GraduateAttributes.First(ga => ga.AttributeName == "Subject Specialists");

                    // Weeks
                        // Generative AI for Students: Ethics & Academic Integrity
                    var genAI_week1 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 1);
                    var genAI_week2 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 2);
                    var genAI_week3 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 3);

                    var weekGraduateAttributes = new[]
                    {
                        // Generative AI for Students: Ethics & Academic Integrity
                        new WeekGraduateAttributes { Week = genAI_week1, GraduateAttribute = Adaptable },
                        new WeekGraduateAttributes { Week = genAI_week1, GraduateAttribute = EffectiveCommunication },
                        new WeekGraduateAttributes { Week = genAI_week2, GraduateAttribute = Investigative },
                        new WeekGraduateAttributes { Week = genAI_week2, GraduateAttribute = EthicallyandSociallyAware },
                        new WeekGraduateAttributes { Week = genAI_week3, GraduateAttribute = ExperiencedCollaborators },
                        new WeekGraduateAttributes { Week = genAI_week3, GraduateAttribute = Confident },
                    };

                    context.WeekGraduateAttributes.AddRange(weekGraduateAttributes);
                    context.SaveChanges();
                }

                if (!context.WeekActivities.Any())
                {
                    // Workbooks
                    var genAI = context.Workbooks.First(wb => wb.CourseName == "Generative AI for Students: Ethics & Academic Integrity");

                    // Weeks
                        // Generative AI for Students: Ethics & Academic Integrity
                    var genAI_week1 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 1);
                    var genAI_week2 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 2);
                    var genAI_week3 = context.Weeks.First(w => w.Workbook == genAI && w.WeekNumber == 3);

                    // Activities
                        // Moodle
                    var Assignment = context.Activities.First(act => act.ActivityName == "Assignment");
                    var Attendance = context.Activities.First(act => act.ActivityName == "Attendance");
                    var Board = context.Activities.First(act => act.ActivityName == "Board");
                    var Book = context.Activities.First(act => act.ActivityName == "Book");
                    var Chat = context.Activities.First(act => act.ActivityName == "Chat");
                    var Checklist = context.Activities.First(act => act.ActivityName == "Checklist");
                    var Choice = context.Activities.First(act => act.ActivityName == "Choice");
                    var CodeGrade = context.Activities.First(act => act.ActivityName == "CodeGrade");
                    var CustomCertificate = context.Activities.First(act => act.ActivityName == "Custom Certificate");
                    var Database = context.Activities.First(act => act.ActivityName == "Database");
                    var Echo360 = context.Activities.First(act => act.ActivityName == "Echo360");
                    var EONXR = context.Activities.First(act => act.ActivityName == "EON-XR");
                    var ErysNotes = context.Activities.First(act => act.ActivityName == "ErysNotes");
                    var File = context.Activities.First(act => act.ActivityName == "File");
                    var Feedback = context.Activities.First(act => act.ActivityName == "Feedback");
                    var Folder = context.Activities.First(act => act.ActivityName == "Folder");
                    var Forum = context.Activities.First(act => act.ActivityName == "Forum");
                    var Game = context.Activities.First(act => act.ActivityName == "Game");
                    var Glossary = context.Activities.First(act => act.ActivityName == "Glossary");
                    var GroupChoice = context.Activities.First(act => act.ActivityName == "Group Choice");
                    var H5P = context.Activities.First(act => act.ActivityName == "H5P");
                    var H5PInteractiveContent = context.Activities.First(act => act.ActivityName == "H5P Interactive Content");
                    var HarvardBusinessPublishing = context.Activities.First(act => act.ActivityName == "Harvard Business Publishing");
                    var IMSContentPackage = context.Activities.First(act => act.ActivityName == "IMS Content Package");
                    var KalturaMediaAssignment = context.Activities.First(act => act.ActivityName == "Kaltura Media Assignment");
                    var KalturaVideoQuiz = context.Activities.First(act => act.ActivityName == "Kaltura Video Quiz");
                    var KalturaVideoResource = context.Activities.First(act => act.ActivityName == "Kaltura Video Resource");
                    var Labster = context.Activities.First(act => act.ActivityName == "Labster");
                    var LearningScience = context.Activities.First(act => act.ActivityName == "LearningScience");
                    var Lesson = context.Activities.First(act => act.ActivityName == "Lesson");
                    var Mahara = context.Activities.First(act => act.ActivityName == "Mahara");
                    var MATLABCodingProblem = context.Activities.First(act => act.ActivityName == "MATLAB Coding Problem");
                    var OpenForum = context.Activities.First(act => act.ActivityName == "Open Forum");
                    var OUBlog = context.Activities.First(act => act.ActivityName == "OU Blog");
                    var Page = context.Activities.First(act => act.ActivityName == "Page");
                    var PearsonMyLabs = context.Activities.First(act => act.ActivityName == "Pearson MyLabs");
                    var PeerAssessment = context.Activities.First(act => act.ActivityName == "Peer Assessment");
                    var PDFAnnotation = context.Activities.First(act => act.ActivityName == "PDF Annotation");
                    var Questionnaire = context.Activities.First(act => act.ActivityName == "Questionnaire");
                    var Quiz = context.Activities.First(act => act.ActivityName == "Quiz");
                    var ReadingListsGlasgow = context.Activities.First(act => act.ActivityName == "Reading Lists @ Glasgow");
                    var Reengagement = context.Activities.First(act => act.ActivityName == "Re-engagement");
                    var Scheduler = context.Activities.First(act => act.ActivityName == "Scheduler");
                    var SCORM = context.Activities.First(act => act.ActivityName == "SCORM");
                    var TextandMediaArea = context.Activities.First(act => act.ActivityName == "Text and Media Area");
                    var Turnitin = context.Activities.First(act => act.ActivityName == "Turnitin");
                    var URL = context.Activities.First(act => act.ActivityName == "URL");
                    var Wiki = context.Activities.First(act => act.ActivityName == "Wiki");
                    var Workshop = context.Activities.First(act => act.ActivityName == "Workshop");
                    var Zoom = context.Activities.First(act => act.ActivityName == "Zoom");

                        // Coursera
                    var Video = context.Activities.First(act => act.ActivityName == "Video");
                    var Reading = context.Activities.First(act => act.ActivityName == "Reading");
                    var DiscussionPrompt = context.Activities.First(act => act.ActivityName == "Discussion Prompt");
                    var ProgrammingAssignment = context.Activities.First(act => act.ActivityName == "Programming Assignment");
                    var PeerReview = context.Activities.First(act => act.ActivityName == "Peer Review");
                    var AppItem = context.Activities.First(act => act.ActivityName == "App Item");
                    var UngradedLab = context.Activities.First(act => act.ActivityName == "Ungraded Lab");
                    var UngradedPlugin = context.Activities.First(act => act.ActivityName == "Ungraded Plugin");

                        // FutureLearn
                    var Article = context.Activities.First(act => act.ActivityName == "Article");
                    var Audio = context.Activities.First(act => act.ActivityName == "Audio");
                    var Discussion = context.Activities.First(act => act.ActivityName == "Discussion");
                    var ExerciseExternalTools = context.Activities.First(act => act.ActivityName == "Exercise / External Tools");
                    var PeerGradedAssignment = context.Activities.First(act => act.ActivityName == "Peer Graded Assignment");
                    var Poll = context.Activities.First(act => act.ActivityName == "Poll");

                        // xSiTe
                    var Announcements = context.Activities.First(act => act.ActivityName == "Announcements");
                    var Blog = context.Activities.First(act => act.ActivityName == "Blog");
                    var Calendar = context.Activities.First(act => act.ActivityName == "Calendar");
                    var CheckList = context.Activities.First(act => act.ActivityName == "Check List");
                    var ClassProgress = context.Activities.First(act => act.ActivityName == "Class Progress");
                    var Email = context.Activities.First(act => act.ActivityName == "Email");
                    var Groups = context.Activities.First(act => act.ActivityName == "Groups");
                    var HTMLDocument = context.Activities.First(act => act.ActivityName == "HTML Document");
                    var ManageFiles = context.Activities.First(act => act.ActivityName == "Manage Files");
                    var NewLesson = context.Activities.First(act => act.ActivityName == "New Lesson");
                    var MediaLibrary = context.Activities.First(act => act.ActivityName == "Media Library");
                    var Portfolio = context.Activities.First(act => act.ActivityName == "Portfolio");
                    var OneDrive = context.Activities.First(act => act.ActivityName == "OneDrive");
                    var SelfAssessment = context.Activities.First(act => act.ActivityName == "Self Assessment");
                    var Surveys = context.Activities.First(act => act.ActivityName == "Surveys");
                    var Weblink = context.Activities.First(act => act.ActivityName == "Weblink");

                        // COIL
                    var BlackboardAlly = context.Activities.First(act => act.ActivityName == "Blackboard Ally");
                    var Camtasia = context.Activities.First(act => act.ActivityName == "Camtasia");
                    var Mentimeter = context.Activities.First(act => act.ActivityName == "Mentimeter");
                    var MicrosoftExcel = context.Activities.First(act => act.ActivityName == "Microsoft Excel");
                    var MicrosoftOneNote = context.Activities.First(act => act.ActivityName == "Microsoft OneNote");
                    var MicrosoftPowerPoint = context.Activities.First(act => act.ActivityName == "Microsoft PowerPoint");
                    var MicrosoftSway = context.Activities.First(act => act.ActivityName == "Microsoft Sway");
                    var MicrosoftTeams = context.Activities.First(act => act.ActivityName == "Microsoft Teams");
                    var MicrosoftWord = context.Activities.First(act => act.ActivityName == "Microsoft Word");
                    var Miro = context.Activities.First(act => act.ActivityName == "Miro");
                    var Qualtrics = context.Activities.First(act => act.ActivityName == "Qualtrics");

                    // Learning Types
                    var Acquisition = context.LearningType.First(lt => lt.LearningTypeName == "Acquisition");
                    var Collaboration = context.LearningType.First(lt => lt.LearningTypeName == "Collaboration");
                    var Discussion_LT = context.LearningType.First(lt => lt.LearningTypeName == "Discussion");
                    var Investigation = context.LearningType.First(lt => lt.LearningTypeName == "Investigation");
                    var Practice = context.LearningType.First(lt => lt.LearningTypeName == "Practice");
                    var Production = context.LearningType.First(lt => lt.LearningTypeName == "Production");
                    var Assessment = context.LearningType.First(lt => lt.LearningTypeName == "Assessment");

                    var weekActivities = new[]
                    {
                        // Generative AI for Students: Ethics & Academic Integrity
                            // Week 1
                        new WeekActivities { TaskTitle = "Lesson 1 - Course Intro",
                                             TaskStaff = "Scott Ramsay",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 1,
                                             Week = genAI_week1,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Introduction to AI and Digital Tools",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,20,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 2,
                                             Week = genAI_week1,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Explore Your Own Knowledge / Views on AI",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 3,
                                             Week = genAI_week1,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 1 - Practice Assignment (AI, GenAI and Digital Literacies)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 4,
                                             Week = genAI_week1,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 1 - Lesson 1 Recap",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 5,
                                             Week = genAI_week1,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - Different Types of AI and Gen AI",
                                             TaskStaff = "Jennifer Boyle",
                                             TaskTime = new TimeSpan(0,20,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 6,
                                             Week = genAI_week1,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - What Tools Do You Use & What Are Your Key Drivers?",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 7,
                                             Week = genAI_week1,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 2 - Practice Assignment (Types of AI and Uses of AI)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 8,
                                             Week = genAI_week1,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 3 - Tool Limitations Based on Ethics",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 9,
                                             Week = genAI_week1,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 3 - Over to You - Exploring GenAI Tools",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 9,
                                             Week = genAI_week1,
                                             Activities = Reading,
                                             LearningType = Investigation},
                        new WeekActivities { TaskTitle = "Lesson 3 - Practice Assignment (Using AI with Ethics in Mind)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 10,
                                             Week = genAI_week1,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 3 - Module 1 Complete",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 11,
                                             Week = genAI_week1,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Graded Assignment (End of Module Assignment)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 12,
                                             Week = genAI_week1,
                                             Activities = Assignment,
                                             LearningType = Assessment},
                            // Week 2
                        new WeekActivities { TaskTitle = "Lesson 1 - Welcome to Module 2",
                                             TaskStaff = "Scott Ramsay",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 1,
                                             Week = genAI_week2,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Our and AI Responsibilities in Study and Research",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 2,
                                             Week = genAI_week2,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Your Role in Your Work",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 3,
                                             Week = genAI_week2,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 1 - Your Responsibilities vs the AI's",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 4,
                                             Week = genAI_week2,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Practice Assignment (Your Role and AI's Role)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 5,
                                             Week = genAI_week2,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 2 - GenAI Tools for Research and Study",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 6,
                                             Week = genAI_week2,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - Over to You - Finding Tools that Work",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 7,
                                             Week = genAI_week2,
                                             Activities = Reading,
                                             LearningType = Investigation},
                        new WeekActivities { TaskTitle = "Lesson 2 - GenAI and You: What Do You Do?",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,15,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 8,
                                             Week = genAI_week2,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 2 - AI Tools for Research: Paper-Finding Tools",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 9,
                                             Week = genAI_week2,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - AI Tools for Research: Summarising Tools",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 10,
                                             Week = genAI_week2,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - Practice Assignment (AI Tools for Research and Study)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 11,
                                             Week = genAI_week2,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 3 - The Costs of AI: Environmental, Human and Social",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,15,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 12,
                                             Week = genAI_week2,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 3 - The Biases of AI: Understanding How AI's Biases Work",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,15,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 13,
                                             Week = genAI_week2,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 3 - Practice Assignment (AI's Limitations)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 14,
                                             Week = genAI_week2,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 3 - Module 2 Complete",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 15,
                                             Week = genAI_week2,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Graded Assignment (End of Module Assignment)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 16,
                                             Week = genAI_week2,
                                             Activities = Assignment,
                                             LearningType = Assessment},
                            // Week 3
                        new WeekActivities { TaskTitle = "Lesson 1 - The Key Principles of Academic Integrity in an AI World",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,20,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 1,
                                             Week = genAI_week3,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Ownership, Plagiarism, and Academic Integrity",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 2,
                                             Week = genAI_week3,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - AI and AI: Academic Integrity and Artificial Intelligence",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 3,
                                             Week = genAI_week3,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - The Changing Face of Academic Integrity in Light of GenAI",
                                             TaskStaff = "Jennifer Boyle",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 4,
                                             Week = genAI_week3,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 1 - Practice Assignment (Academic Integrity and AI)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 5,
                                             Week = genAI_week3,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 2 - Journals, Funding, and GenAI",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,5,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 6,
                                             Week = genAI_week3,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - Revisited: Explore Your Own Knowledge / Views on AI",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 7,
                                             Week = genAI_week3,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 2 - AI and Your Subject",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 8,
                                             Week = genAI_week3,
                                             Activities = Reading,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 2 - Practice Assignment (The Rules of AI in Your Study)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 9,
                                             Week = genAI_week3,
                                             Activities = Quiz,
                                             LearningType = Practice},
                        new WeekActivities { TaskTitle = "Lesson 3 - Reflection: Considering Data Info and Sensitivity in Study",
                                             TaskStaff = "Caitlin Diver",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 10,
                                             Week = genAI_week3,
                                             Activities = DiscussionPrompt,
                                             LearningType = Discussion_LT},
                        new WeekActivities { TaskTitle = "Lesson 3 - Data and Rights to Confidentiality",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 11,
                                             Week = genAI_week3,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Lesson 3 - Copyright and Intellectual Property",
                                             TaskStaff = "Scott Ramsey",
                                             TaskTime = new TimeSpan(0,10,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 12,
                                             Week = genAI_week3,
                                             Activities = Video,
                                             LearningType = Acquisition},
                        new WeekActivities { TaskTitle = "Graded Assignment (End of Module Assignment)",
                                             TaskStaff = "Andrew Struan",
                                             TaskTime = new TimeSpan(0,30,0),
                                             TasksStatus = "Completed",
                                             TaskLocation = "Remote (Home)",
                                             TaskApproach = "Synchronous",
                                             TaskOrder = 13,
                                             Week = genAI_week3,
                                             Activities = Assignment,
                                             LearningType = Assessment},
                    };

                    context.WeekActivities.AddRange(weekActivities);
                    context.SaveChanges();
                }
            }
        }
    }
}