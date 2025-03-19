using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Http;
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
        public void HomePage_Index_BlankPage()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BlankTestDatabase")
                .Options;

            var localContext = new ApplicationDbContext(options);

            var mockLogger = new Mock<ILogger<HomeController>>();

            var controller = new HomeController(mockLogger.Object, localContext);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "test-user-123"),
                        new Claim(ClaimTypes.Name, "Test User"),
                    ],
                    "mock"
                )
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user },
            };

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);

            var workbookList = Assert.IsType<List<Workbook>>(result.ViewData["MyWorkbooks"]);

            Assert.Empty(workbookList);
        }

        [Fact]
        public void HomePage_Index_MultipleWorkbooks()
        {
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object, _context);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "test-user-123"),
                        new Claim(ClaimTypes.Name, "Test User"),
                    ],
                    "mock"
                )
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user },
            };

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);

            var workbookList = Assert.IsType<List<Workbook>>(result.ViewData["MyWorkbooks"]);

            Assert.Equal(2, workbookList.Count);
            Assert.Equal(2, workbookList[0].WorkbookId);
            Assert.Equal("Network and Operating System Essentials", workbookList[1].CourseName);
        }

        [Fact]
        public void HomePage_Index_MultipleWorkbooksSorting()
        {
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object, _context);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "test-user-123"),
                        new Claim(ClaimTypes.Name, "Test User"),
                    ],
                    "mock"
                )
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user },
            };

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);

            var workbookList = Assert.IsType<List<Workbook>>(result.ViewData["MyWorkbooks"]);

            Assert.Equal(2, workbookList[0].WorkbookId);
            Assert.Equal(1, workbookList[1].WorkbookId);
            Assert.True(workbookList[0].LastEdited > workbookList[1].LastEdited);
        }
    }
}
