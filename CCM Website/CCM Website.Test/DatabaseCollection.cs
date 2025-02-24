using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        Context = new ApplicationDbContext(options);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var learningPlatform = new LearningPlatform() { PlatformName = "Moodle" };

        var activity = new Activities { ActivityName = "Assignment" };

        var lpActivity = new LearningPlatformActivities
        {
            LearningPlatform = learningPlatform,
            Activities = activity,
        };

        var learningTypes = new[]
        {
            new LearningType
            {
                LearningTypeName = "Acquisition",
                LearningTypeColour = "#A1F5ED",
                LearningTypeTextColour = "#fff",
            },
            new LearningType
            {
                LearningTypeName = "Collaboration",
                LearningTypeColour = "#FFD21A",
                LearningTypeTextColour = "#fff",
            },
        };
        var workbooks = new List<Workbook>
        {
            new Workbook
            {
                WorkbookId = 1,
                CourseName = "Network and Operating System Essentials",
                CourseCode = "COMPSCI2024",
                CourseLead = "Awais Aziz Shah",
                LearningPlatform = learningPlatform,
                LastEdited = new DateTime(2024, 11, 12),
            },
            new Workbook
            {
                WorkbookId = 2,
                CourseName = "Data Fundamentals",
                CourseCode = "COMPSCI4073",
                CourseLead = "John Williamson",
                LearningPlatform = learningPlatform,
                LastEdited = DateTime.Now,
            },
        };

        Context.Workbooks.AddRange(workbooks);
        Context.LearningPlatforms.Add(learningPlatform);
        Context.Activities.Add(activity);
        Context.LearningPlatformActivities.Add(lpActivity);
        Context.LearningType.AddRange(learningTypes);

        Context.Weeks.AddRange(
            new List<Week>
            {
                new Week
                {
                    WeekId = 1,
                    WorkbookId = 1,
                    WeekNumber = 1,
                    Workbook = workbooks[0],
                },
                new Week
                {
                    WeekId = 2,
                    WorkbookId = 1,
                    WeekNumber = 2,
                    Workbook = workbooks[0],
                },
                new Week
                {
                    WeekId = 3,
                    WorkbookId = 2,
                    WeekNumber = 3,
                    Workbook = workbooks[1],
                },
            }
        );

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
