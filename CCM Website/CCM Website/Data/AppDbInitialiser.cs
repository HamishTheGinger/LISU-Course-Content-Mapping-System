using CCM_Website.Models;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using NuGet.Protocol.Plugins;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace CCM_Website.Data
{
    public class AppDbInitialiser
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;

                context.Database.EnsureCreated();

                if (!context.GraduateAttributes.Any())
                {
                    var graduateAttributes = new[]
                    {
                        new GraduateAttribute {  AttributeName = "Adaptable" },
                        new GraduateAttribute {  AttributeName = "Confident" },
                        new GraduateAttribute {  AttributeName = "Effective Communication" },
                        new GraduateAttribute {  AttributeName = "Ethically and Socially Aware" },
                        new GraduateAttribute {  AttributeName = "Experienced Collaborators" },
                        new GraduateAttribute {  AttributeName = "Independent and Critical Thinkers" },
                        new GraduateAttribute {  AttributeName = "Investigative" },
                        new GraduateAttribute {  AttributeName = "Reflective Learners" },
                        new GraduateAttribute {  AttributeName = "Resourceful and Responsible" },
                        new GraduateAttribute {  AttributeName = "Subject Specialists" },
                    };

                    context.GraduateAttributes.AddRange(graduateAttributes);
                    context.SaveChanges();
                }

                if (!context.LearningType.Any())
                {
                    var learningTypes = new[]
                    {
                        new LearningType {  LearningTypeName = "Acquisition", LearningTypeColour = "#A1F5ED"},
                        new LearningType {  LearningTypeName = "Collaboration", LearningTypeColour = "#FFD21A"},
                        new LearningType {  LearningTypeName = "Discussion", LearningTypeColour = "#7AAEEA" },
                        new LearningType {  LearningTypeName = "Investigation", LearningTypeColour = "#F8807F" },
                        new LearningType {  LearningTypeName = "Practice", LearningTypeColour = "#BB98DC" },
                        new LearningType {  LearningTypeName = "Production", LearningTypeColour = "#BDEA75" },
                        new LearningType {  LearningTypeName = "Assessment", LearningTypeColour = "#44546A" },
                    };

                    context.LearningType.AddRange(learningTypes);
                    context.SaveChanges();
                }

                if (!context.LearningPlatforms.Any())
                {
                    var platforms = new[]
                    {
                        new LearningPlatform {  PlatformName = "Moodle" },
                        new LearningPlatform {  PlatformName = "Coursera" },
                        new LearningPlatform {  PlatformName = "FutureLearn" },
                        new LearningPlatform {  PlatformName = "xSiTe" },
                        new LearningPlatform {  PlatformName = "COIL" },
                    };

                    context.LearningPlatforms.AddRange(platforms);
                    context.SaveChanges();
                }

                if (!context.Activities.Any())
                {
                    var activities = new[]
                    {
                        // Moodle
                        new Activities { ActivityName = "Assignment" }, // Coursera, xSiTe, COIL
                        new Activities { ActivityName = "Attendance"}, // COIL
                        new Activities { ActivityName = "Board"}, // COIL
                        new Activities { ActivityName = "Book"}, // COIL
                        new Activities { ActivityName = "Chat"}, // COIL
                        new Activities { ActivityName = "Checklist"}, // COIL
                        new Activities { ActivityName = "Choice"}, // COIL
                        new Activities { ActivityName = "CodeGrade"}, // COIL
                        new Activities { ActivityName = "Custom Certificate"}, // COIL
                        new Activities { ActivityName = "Database"}, // COIL
                        new Activities { ActivityName = "Echo360"}, // xSiTe, COIL
                        new Activities { ActivityName = "EON-XR"}, // COIL
                        new Activities { ActivityName = "ErysNotes"}, // COIL
                        new Activities { ActivityName = "File"}, // xSiTe, COIL
                        new Activities { ActivityName = "Feedback"}, // COIL
                        new Activities { ActivityName = "Folder"}, // COIL
                        new Activities { ActivityName = "Forum"}, // COIL
                        new Activities { ActivityName = "Game"}, // COIL
                        new Activities { ActivityName = "Glossary"}, // xSiTe, COIL
                        new Activities { ActivityName = "Group Choice"}, // COIL
                        new Activities { ActivityName = "H5P"}, // COIL
                        new Activities { ActivityName = "H5P Interactive Content"}, // COIL
                        new Activities { ActivityName = "Harvard Business Publishing"}, // COIL
                        new Activities { ActivityName = "IMS Content Package"}, // COIL
                        new Activities { ActivityName = "Kaltura Media Assignment"}, // COIL
                        new Activities { ActivityName = "Kaltura Video Quiz"}, // COIL
                        new Activities { ActivityName = "Kaltura Video Resource"}, // COIL
                        new Activities { ActivityName = "Labster"}, // COIL
                        new Activities { ActivityName = "LearningScience"}, // COIL
                        new Activities { ActivityName = "Lesson"}, // COIL
                        new Activities { ActivityName = "Mahara"}, // COIL
                        new Activities { ActivityName = "MATLAB Coding Problem"}, // COIL
                        new Activities { ActivityName = "Open Forum"}, // COIL
                        new Activities { ActivityName = "OU Blog"}, // COIL
                        new Activities { ActivityName = "Page"}, // COIL
                        new Activities { ActivityName = "Pearson MyLabs"}, // COIL
                        new Activities { ActivityName = "Peer Assessment"}, // COIL
                        new Activities { ActivityName = "PDF Annotation"}, // COIL
                        new Activities { ActivityName = "Questionnaire"}, // COIL
                        new Activities { ActivityName = "Quiz"}, // FutureLearn, xSiTe, COIL
                        new Activities { ActivityName = "Reading Lists @ Glasgow"}, // COIL
                        new Activities { ActivityName = "Re-engagement"}, // COIL
                        new Activities { ActivityName = "Scheduler"}, // COIL
                        new Activities { ActivityName = "SCORM"}, // xSiTe, COIL
                        new Activities { ActivityName = "Text and Media Area"}, // COIL
                        new Activities { ActivityName = "Turnitin"}, // COIL
                        new Activities { ActivityName = "URL"}, // COIL
                        new Activities { ActivityName = "Wiki"}, // COIL
                        new Activities { ActivityName = "Workshop"}, // COIL
                        new Activities { ActivityName = "Zoom"}, // xSiTe, COIL

                        // Coursera
                        new Activities { ActivityName = "Video"}, // FutureLearn
                        new Activities { ActivityName = "Reading"},
                        new Activities { ActivityName = "Discussion Prompt"},
                        new Activities { ActivityName = "Programming Assignment"},
                        new Activities { ActivityName = "Peer Review"},
                        new Activities { ActivityName = "App Item"},
                        new Activities { ActivityName = "Ungraded Lab"},
                        new Activities { ActivityName = "Ungraded Plugin"},

                        // FutureLearn
                        new Activities { ActivityName = "Article"},
                        new Activities { ActivityName = "Audio"},
                        new Activities { ActivityName = "Discussion"}, // xSiTe
                        new Activities { ActivityName = "Exercise / External Tools"},
                        new Activities { ActivityName = "Peer Graded Assignment"},
                        new Activities { ActivityName = "Poll"},

                        // xSiTe
                        new Activities { ActivityName = "Announcements"},
                        new Activities { ActivityName = "Blog"},
                        new Activities { ActivityName = "Calendar"},
                        new Activities { ActivityName = "Check List"},
                        new Activities { ActivityName = "Class Progress"},
                        new Activities { ActivityName = "Email"},
                        new Activities { ActivityName = "Groups"},
                        new Activities { ActivityName = "HTML Document"},
                        new Activities { ActivityName = "Manage Files"},
                        new Activities { ActivityName = "New Lesson"},
                        new Activities { ActivityName = "Media Library"},
                        new Activities { ActivityName = "Portfolio"},
                        new Activities { ActivityName = "OneDrive"},
                        new Activities { ActivityName = "Self Assessment"},
                        new Activities { ActivityName = "Surveys"},
                        new Activities { ActivityName = "Weblink"},

                        // COIL
                        new Activities { ActivityName = "Blackboard Ally"},
                        new Activities { ActivityName = "Camtasia"},
                        new Activities { ActivityName = "Mentimeter"},
                        new Activities { ActivityName = "Microsoft Excel"},
                        new Activities { ActivityName = "Microsoft OneNote"},
                        new Activities { ActivityName = "Microsoft PowerPoint"},
                        new Activities { ActivityName = "Microsoft Sway"},
                        new Activities { ActivityName = "Microsoft Teams"},
                        new Activities { ActivityName = "Microsoft Word"},
                        new Activities { ActivityName = "Miro"},
                        new Activities { ActivityName = "Qualtrics"},
                    };

                    context.Activities.AddRange(activities);
                    context.SaveChanges();
                }

                if (!context.LearningPlatformActivities.Any())
                {
                    // Learning Platforms
                    var moodle = context.LearningPlatforms.First(lp => lp.PlatformName == "Moodle");
                    var coursera = context.LearningPlatforms.First(lp => lp.PlatformName == "Coursera");
                    var futureLearn = context.LearningPlatforms.First(lp => lp.PlatformName == "FutureLearn");
                    var xSite = context.LearningPlatforms.First(lp => lp.PlatformName == "xSiTe");
                    var COIL = context.LearningPlatforms.First(lp => lp.PlatformName == "COIL");

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

                    var learningPlatformActivities = new[]
                    {
                        // Moodle
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Assignment },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Attendance },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Board },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Book },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Chat },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Checklist },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Choice },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = CodeGrade },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = CustomCertificate },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Database },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Echo360 },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = EONXR },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = ErysNotes },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = File },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Feedback },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Folder },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Forum },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Game },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Glossary },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = GroupChoice },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = H5P },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = H5PInteractiveContent },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = HarvardBusinessPublishing },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = IMSContentPackage },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = KalturaMediaAssignment },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = KalturaVideoQuiz },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = KalturaVideoResource },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Labster },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = LearningScience },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Lesson },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Mahara },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = MATLABCodingProblem },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = OpenForum },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = OUBlog },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Page },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = PearsonMyLabs },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = PeerAssessment },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = PDFAnnotation },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Questionnaire },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Quiz },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = ReadingListsGlasgow },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Reengagement },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Scheduler },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = SCORM },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = TextandMediaArea },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Turnitin },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = URL },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Wiki },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Workshop },
                        new LearningPlatformActivities { LearningPlatform = moodle, Activities = Zoom },

                        // Coursera
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = Video },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = Reading },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = Assignment },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = DiscussionPrompt },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = ProgrammingAssignment },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = PeerReview },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = AppItem },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = UngradedLab },
                        new LearningPlatformActivities { LearningPlatform = coursera, Activities = UngradedPlugin },

                        // FutureLearn
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Article },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Audio },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Discussion },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = ExerciseExternalTools },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = PeerGradedAssignment },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Poll },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Quiz },
                        new LearningPlatformActivities { LearningPlatform = futureLearn, Activities = Video },

                        // xSiTe
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Announcements },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Assignment },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Blog },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Calendar },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = CheckList },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = ClassProgress },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Discussion },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Echo360 },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Email },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = File },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Glossary },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Groups },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = HTMLDocument },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = ManageFiles },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = NewLesson },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = MediaLibrary },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Portfolio },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = OneDrive },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Quiz },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = SCORM },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = SelfAssessment },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Surveys },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Weblink },
                        new LearningPlatformActivities { LearningPlatform = xSite, Activities = Zoom },

                        // COIL
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Assignment },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Attendance },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = BlackboardAlly },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Board },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Book },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Camtasia },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Chat },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Checklist },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Choice },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = CodeGrade },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = CustomCertificate },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Database },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Echo360 },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = EONXR },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = ErysNotes },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = File },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Feedback },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Folder },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Forum },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Game },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Glossary },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = GroupChoice },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = H5P },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = H5PInteractiveContent },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = HarvardBusinessPublishing },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = IMSContentPackage },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = KalturaMediaAssignment },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = KalturaVideoQuiz },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = KalturaVideoResource },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Labster },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = LearningScience },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Lesson },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Mahara },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MATLABCodingProblem },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Mentimeter },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftExcel },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftOneNote },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftPowerPoint },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftSway },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftTeams },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = MicrosoftWord },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Miro },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = OpenForum },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = OUBlog },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Page },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = PearsonMyLabs },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = PeerAssessment },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = PDFAnnotation },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Qualtrics },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Questionnaire },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Quiz },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = ReadingListsGlasgow },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Reengagement },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Scheduler },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = SCORM },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = TextandMediaArea },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Turnitin },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = URL },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Wiki },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Workshop },
                        new LearningPlatformActivities { LearningPlatform = COIL, Activities = Zoom },
                    };

                    context.LearningPlatformActivities.AddRange(learningPlatformActivities);
                    context.SaveChanges();
                }
            }
        }
    }
}
