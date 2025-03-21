using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.ContentModel;
using X.PagedList;
using Xunit;

namespace CCM_Website.Test
{
    [Collection("DatabaseCollection")]
    public class WorkbookTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public WorkbookTests(DatabaseFixture fixture)
        {
            _context = fixture.Context;
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockAuthorizationService
                .Setup(a =>
                    a.AuthorizeAsync(
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync(AuthorizationResult.Success);
        }

        [Fact]
        public async Task Workbook_Index_TestingWBData()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

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

            var result = await controller.Index("", 1) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IPagedList<Workbook>>(result.Model);

            Assert.Equal(2, model.Count);

            Assert.Contains(model, w => w.CourseName == "Network and Operating System Essentials");
            Assert.Contains(model, w => w.CourseName == "Data Fundamentals");

            Assert.Contains(model, w => w.CourseCode == "COMPSCI2024");
            Assert.Contains(model, w => w.CourseCode == "COMPSCI4073");
        }

        [Fact]
        public async Task Workbook_Create_TestCreationAllFieldsPass()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var lp = _context.LearningPlatforms.FirstOrDefault(lp => lp.PlatformId == 1);
            Assert.NotNull(lp);

            var cose = _context.UniversityArea.FirstOrDefault(ua =>
                ua.AreaName == "College of Science & Engineering"
            );
            Assert.NotNull(cose);

            var newWorkbook = new Workbook
            {
                CourseName = "Professional Software Engineering",
                CourseCode = "COMPSCI303",
                CourseLength = 2,
                PipReference = "CSP272",
                CourseLead = "Tim Storer",
                LearningPlatformId = 1,
                LearningPlatform = lp,
                Collaborators = "Dave Smith",
                UniversityArea = cose,
                UniversityAreaId = 1,
                OwnerId = "test-user-123",
            };

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

            var result = await controller.Create(newWorkbook) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            var savedWorkbook = await _context
                .Workbooks.Include(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(wb => wb.CourseName == "Professional Software Engineering");
            Assert.NotNull(savedWorkbook);

            Assert.Equal("Professional Software Engineering", savedWorkbook.CourseName);
            Assert.Equal("CSP272", savedWorkbook.PipReference);
            Assert.Equal(lp, savedWorkbook.LearningPlatform);
            Assert.Equal("Tim Storer", savedWorkbook.CourseLead);

            _context.Workbooks.Remove(savedWorkbook);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Workbook_Create_TestCreationRequiredFieldsPass()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var lp = _context.LearningPlatforms.FirstOrDefault(lp => lp.PlatformId == 1);
            Assert.NotNull(lp);

            var cose = _context.UniversityArea.FirstOrDefault(ua =>
                ua.AreaName == "College of Science & Engineering"
            );
            Assert.NotNull(cose);

            var newWorkbook = new Workbook
            {
                CourseName = "Network Systems",
                CourseCode = "COMPSCI4603", //Remove when bug fixed
                CourseLength = 2,
                CourseLead = "Colin",
                LearningPlatformId = 1,
                LearningPlatform = lp,
                UniversityArea = cose,
                UniversityAreaId = 1,
            };

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

            var result = await controller.Create(newWorkbook) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            var savedWorkbook = await _context
                .Workbooks.Include(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(wb => wb.CourseName == "Network Systems");
            Assert.NotNull(savedWorkbook);

            Assert.Equal("Network Systems", savedWorkbook.CourseName);
            Assert.Equal(lp, savedWorkbook.LearningPlatform);
            Assert.Equal("Colin", savedWorkbook.CourseLead);
            Assert.Equal(2, savedWorkbook.CourseLength);

            Assert.Null(savedWorkbook.PipReference);
            Assert.Null(savedWorkbook.Collaborators);

            _context.Workbooks.Remove(savedWorkbook);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Workbook_Create_TestCreationFail()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var lp = _context.LearningPlatforms.FirstOrDefault(lp => lp.PlatformId == 1);
            Assert.NotNull(lp);

            var cose = _context.UniversityArea.FirstOrDefault(ua =>
                ua.AreaName == "College of Science & Engineering"
            );
            Assert.NotNull(cose);

            var incompleteWorkbook = new Workbook
            {
                CourseName = "IOOP",
                CourseCode = "COMPSCI303",
                CourseLead = "PLACEHOLDER",
                CourseLength = 2,
                PipReference = "CSP272",
                LearningPlatformId = 1,
                LearningPlatform = lp,
                Collaborators = "Dave Smith",
                UniversityArea = cose,
                UniversityAreaId = 1,
            };

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

            // Manually add the error, as to not break the initialisation above
            controller.ModelState.AddModelError("CourseLead", "The CourseLead field is required.");

            var result = await controller.Create(incompleteWorkbook) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            var savedActivity = await _context.Workbooks.FirstOrDefaultAsync(w =>
                w.CourseName == "IOOP"
            );
            Assert.Null(savedActivity);
        }

        [Fact]
        public async Task Workbook_Edit_TestEdit()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var lp = _context.LearningPlatforms.FirstOrDefault(lp => lp.PlatformId == 1);
            Assert.NotNull(lp);

            var cose = _context.UniversityArea.FirstOrDefault(ua =>
                ua.AreaName == "College of Science & Engineering"
            );
            Assert.NotNull(cose);

            var newWorkbook = new Workbook
            {
                CourseName = "Operating Systems",
                CourseCode = "COMPSCI4503",
                CourseLength = 2,
                CourseLead = "John Smith",
                LearningPlatformId = 1,
                LearningPlatform = lp,
                UniversityArea = cose,
                UniversityAreaId = 1,
            };

            _context.Workbooks.Add(newWorkbook);
            await _context.SaveChangesAsync();

            var newWorkbookContext = await _context
                .Workbooks.Include(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(wb => wb.CourseName == "Operating Systems");
            Assert.NotNull(newWorkbookContext);

            var editedWorkbook = newWorkbook;
            editedWorkbook.CourseName = "Operating Systems 101";
            editedWorkbook.CourseCode = "COMPSCI4903";

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

            var result =
                await controller.Edit(newWorkbook.WorkbookId, editedWorkbook)
                as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);

            var savedWorkbook = await _context
                .Workbooks.Include(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(wb => wb.CourseName == "Operating Systems 101");
            Assert.NotNull(savedWorkbook);

            Assert.Equal(newWorkbook.WorkbookId, savedWorkbook.WorkbookId);
            Assert.Equal("Operating Systems 101", savedWorkbook.CourseName);
            Assert.Equal("COMPSCI4903", savedWorkbook.CourseCode);
            Assert.Equal(lp, savedWorkbook.LearningPlatform);
            Assert.Equal("John Smith", savedWorkbook.CourseLead);

            _context.Workbooks.Remove(savedWorkbook);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Workbook_Edit_TestEditFail()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var lp = _context.LearningPlatforms.FirstOrDefault(lp => lp.PlatformId == 1);
            Assert.NotNull(lp);

            var cose = _context.UniversityArea.FirstOrDefault(ua =>
                ua.AreaName == "College of Science & Engineering"
            );
            Assert.NotNull(cose);

            var newWorkbook = new Workbook
            {
                CourseName = "Operating Systems",
                CourseCode = "COMPSCI4503",
                CourseLength = 2,
                CourseLead = "John Smith",
                LearningPlatformId = 1,
                LearningPlatform = lp,
                UniversityArea = cose,
                UniversityAreaId = 1,
            };

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

            var result = await controller.Edit(1100, newWorkbook);

            Assert.IsType<NotFoundResult>(result);
            //Assert.Equal("Index", result.ActionName);

            var savedWorkbook = await _context
                .Workbooks.Include(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(wb => wb.CourseName == "Operating Systems");
            Assert.Null(savedWorkbook);
        }
    }
}
