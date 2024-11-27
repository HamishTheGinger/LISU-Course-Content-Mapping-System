using CCM_Website.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using NuGet.Versioning;
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

                if (!context.LearningPlatforms.Any())
                {
                    var platforms = new[]
                    {
                        new LearningPlatform {  PlatformName = "Moodle" },
                        new LearningPlatform {  PlatformName = "Coursera" },
                        new LearningPlatform {  PlatformName = "FutureLearn" },
                        new LearningPlatform {  PlatformName = "xSite" },
                        new LearningPlatform {  PlatformName = "COIL" },
                    };

                    context.LearningPlatforms.AddRange(platforms);
                    context.SaveChanges();
                }

                if (!context.Activities.Any())
                {
                    var activities = new[]
                    {
                        new Activities { ActivityName = "Assignment" },
                        new Activities { ActivityName = "Attendance"},
                        new Activities { ActivityName = "Board"},
                        new Activities { ActivityName = "Book"},
                        new Activities { ActivityName = "Chat"},
                        new Activities { ActivityName = "Checklist"},
                        new Activities { ActivityName = "Choice"},
                        new Activities { ActivityName = "CodeGrade"},
                        new Activities { ActivityName = "Custom Certificate"},
                        new Activities { ActivityName = "Database"},
                        new Activities { ActivityName = "Echo360"},
                        new Activities { ActivityName = "EON-XR"},
                        new Activities { ActivityName = "ErysNotes"},
                        new Activities { ActivityName = "File"},
                        new Activities { ActivityName = "Feedback"},
                        new Activities { ActivityName = "Folder"},
                        new Activities { ActivityName = "Forum"},
                        new Activities { ActivityName = "Game"},
                        new Activities { ActivityName = "Glossary"},
                        new Activities { ActivityName = "Group Choice"},
                        new Activities { ActivityName = "H5P"},
                        new Activities { ActivityName = "H5P Interactive Content"},
                        new Activities { ActivityName = "Harvard Business Publishing"},
                        new Activities { ActivityName = "IMS Content Package"},
                        new Activities { ActivityName = "Kaltura Media Assignment"},
                        new Activities { ActivityName = "Kaltura Video Quiz"},
                        new Activities { ActivityName = "Kaltura Video Resource"},
                        new Activities { ActivityName = "Labster"},
                        new Activities { ActivityName = "LearningScience"},
                        new Activities { ActivityName = "Lesson"},
                        new Activities { ActivityName = "Mahara"},
                        new Activities { ActivityName = "MATLAB Coding Problem"},
                        new Activities { ActivityName = "Open Forum"},
                        new Activities { ActivityName = "OU Blog"},
                        new Activities { ActivityName = "Page"},
                        new Activities { ActivityName = "Pearson MyLabs"},
                        new Activities { ActivityName = "Peer Assessment"},
                        new Activities { ActivityName = "PDF Annotation"},
                        new Activities { ActivityName = "Questionnaire"},
                        new Activities { ActivityName = "Quiz"},
                        new Activities { ActivityName = "Reading Lists @ Glasgow"},
                        new Activities { ActivityName = "Re-engagement"},
                        new Activities { ActivityName = "Scheduler"},
                        new Activities { ActivityName = "SCORM"},
                        new Activities { ActivityName = "Text and Media Area"},
                        new Activities { ActivityName = "Turnitin"},
                        new Activities { ActivityName = "URL"},
                        new Activities { ActivityName = "Wiki"},
                        new Activities { ActivityName = "Workshop"},
                        new Activities { ActivityName = "Zoom"},
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
                    var xSite = context.LearningPlatforms.First(lp => lp.PlatformName == "xSite");
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

                    var learningPlatformActivities = new[]
                    {
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
                    };

                    context.LearningPlatformActivities.AddRange(learningPlatformActivities);
                    context.SaveChanges();
                }
            }
        }
    }
}
