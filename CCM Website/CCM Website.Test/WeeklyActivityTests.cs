using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.ContentModel;
using Xunit;

namespace CCM_Website.Test
{
    [Collection("DatabaseCollection")]
    public class WeeklyActivityTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public WeeklyActivityTests(DatabaseFixture fixture)
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
        public async void WeekActivities_CreateActivity_DropdownFilterAsync()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            int wbid = 1;
            int wkid = 1;
            var result = await controller.CreateActivity(wbid, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["ActivitiesId"]);
            var filteredActivities = selectList.Items.Cast<Activities>().ToList();

            Assert.Single(filteredActivities);
            Assert.Equal("Assignment", filteredActivities[0].ActivityName);
        }

        [Fact]
        public async void WeekActivities_CreateActivity_LearningTypeDropdownFilter()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            int wbid = 1;
            int wkid = 1;
            var result = await controller.CreateActivity(wbid, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["LearningApproach"]);
            var filteredLearningType = selectList.Items.Cast<LearningType>().ToList();

            Assert.Equal(2, filteredLearningType.Count);
            Assert.Equal("Acquisition", filteredLearningType[0].LearningTypeName);
            Assert.Equal("#A1F5ED", filteredLearningType[0].LearningTypeColour);
        }

        [Fact]
        public async void WeekActivities_CreateActivity_WeekDropdownFilter()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);
            int wbId = 1;
            int wkid = 1;

            var result = await controller.CreateActivity(wbId, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["WeekId"]);
            var filteredWeeks = selectList.Items.Cast<Week>().ToList();

            Assert.Equal(2, filteredWeeks.Count);
            Assert.All(filteredWeeks, w => Assert.Equal(wbId, w.WorkbookId));
        }

        [Fact]
        public async void WeekActivities_CreateActivity_TestCreationPass()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var week = await _context.Weeks.FirstOrDefaultAsync(w => w.WeekId == 1);
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == 1);
            var lt = await _context.LearningType.FirstOrDefaultAsync(w => w.LearningTypeId == 1);
            var la = await _context.TaskApproach.FirstOrDefaultAsync(w => w.ApproachId == 1);
            var ll = await _context.TaskLocation.FirstOrDefaultAsync(w => w.LocationId == 1);
            var ls = await _context.TaskProgressStatus.FirstOrDefaultAsync(s => s.StatusId == 1);

            Assert.NotNull(week);
            Assert.NotNull(activity);
            Assert.NotNull(lt);
            Assert.NotNull(la);
            Assert.NotNull(ll);
            Assert.NotNull(ls);

            var newActivity = new WeekActivities
            {
                WeekActivityId = 1,
                TaskOrder = 1,
                WeekId = 1,
                ActivitiesId = 1,
                TaskTitle = "Lecture 1",
                TaskStaff = "John Smith",
                TaskTime = new TimeSpan(1, 30, 0),
                TasksStatusId = 1,
                LearningTypeId = 1,
                TaskLocationId = 1,
                TaskApproachId = 1,
                Week = week,
                Activities = activity,
                LearningType = lt,
                TaskApproach = la,
                TaskLocation = ll,
                TasksStatus = ls,
            };

            var result = await controller.CreateActivity(newActivity) as RedirectToActionResult;

            // Result Checks
            Assert.NotNull(result);
            Assert.Equal("Week", result.ActionName);
            // This check ensures that the page correctly redirects to the appropriate week page.
            Assert.Equal(newActivity.WeekId, result.RouteValues?["id"]);

            // Database Checks
            var savedActivity = await _context
                .WeekActivities.Include(weekActivities => weekActivities.TasksStatus)
                .Include(weekActivities => weekActivities.LearningType)
                .FirstOrDefaultAsync(w => w.WeekActivityId == newActivity.WeekActivityId);
            Assert.NotNull(savedActivity);
            Assert.Equal("Lecture 1", savedActivity.TaskTitle);
            Assert.Equal("John Smith", savedActivity.TaskStaff);
            Assert.Equal("Completed", savedActivity.TasksStatus.StatusName);
            Assert.Equal(new TimeSpan(1, 30, 0), savedActivity.TaskTime);
            Assert.Equal("Acquisition", savedActivity.LearningType.LearningTypeName);
        }

        [Fact]
        public async Task WeekActivities_CreateActivity_TestCreationFail()
        {
            var controller = new WorkbooksController(_context, _mockAuthorizationService.Object);

            var week = await _context.Weeks.FirstOrDefaultAsync(w => w.WeekId == 1);
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == 1);
            var lt = await _context.LearningType.FirstOrDefaultAsync(w => w.LearningTypeId == 1);
            var la = await _context.TaskApproach.FirstOrDefaultAsync(w => w.ApproachId == 1);
            var ll = await _context.TaskLocation.FirstOrDefaultAsync(w => w.LocationId == 1);
            var ls = await _context.TaskProgressStatus.FirstOrDefaultAsync(s => s.StatusId == 1);

            Assert.NotNull(week);
            Assert.NotNull(activity);
            Assert.NotNull(lt);
            Assert.NotNull(la);
            Assert.NotNull(ll);
            Assert.NotNull(ls);

            var incompleteActivity = new WeekActivities
            {
                TaskOrder = 1,
                // No WeekId
                ActivitiesId = 1,
                TaskTitle = "New Task",
                TaskStaff = "John Doe",
                TaskTime = new TimeSpan(1, 30, 0),
                TasksStatusId = 1,
                LearningTypeId = 1,
                TaskLocationId = 1,
                TaskApproachId = 1,
                Week = week,
                Activities = activity,
                LearningType = lt,
                TaskApproach = la,
                TaskLocation = ll,
                TasksStatus = ls,
            };

            controller.ModelState.AddModelError("WeekId", "The WeekId field is required.");

            var result = await controller.CreateActivity(incompleteActivity) as ViewResult;

            Assert.Null(result);
            Assert.False(controller.ModelState.IsValid);
            var savedActivity = await _context.WeekActivities.FirstOrDefaultAsync(w =>
                w.TaskTitle == "New Task"
            );
            Assert.Null(savedActivity);
        }
    }
}
