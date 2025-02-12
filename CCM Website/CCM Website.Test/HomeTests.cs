using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CCM_Website.Test
{
    [Collection("DatabaseCollection")]
    public class HomeTests
    {
        private readonly ApplicationDbContext _context;

        public HomeTests(DatabaseFixture fixture)
        {
            _context = fixture.Context;
        }

        [Fact]
        public async Task HomePage_A_BlankPage()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BlankTestDatabase")
                .Options;

            var localContext = new ApplicationDbContext(options);

            var mockLogger = new Mock<ILogger<HomeController>>();

            var controller = new HomeController(mockLogger.Object, localContext);
            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);

            var workbookList = Assert.IsType<List<Workbook>>(result.ViewData["MyWorkbooks"]);

            Assert.Empty(workbookList);
        }

        [Fact]
        public async Task HomePage_MultipleWorkbooks()
        {
            // Arrange: Create in-memory database options


            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object, _context);
            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);

            var workbookList = Assert.IsType<List<Workbook>>(result.ViewData["MyWorkbooks"]);

            Assert.Equal(2, workbookList.Count);
            Assert.Equal(2, workbookList[0].WorkbookId);
        }

        private List<Workbook> GetWorkbooks()
        {
            var learningPlatform = new LearningPlatform() { PlatformName = "Moodle" };

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

            return workbooks;
        }
    }
}
