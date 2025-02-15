using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Controllers;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CCM_Website.Test
{
    [Collection("DatabaseCollection")]
    public class WeeklyActivityTests
    {
        private readonly ApplicationDbContext _context;

        public WeeklyActivityTests(DatabaseFixture fixture)
        {
            _context = fixture.Context;
        }

        [Fact]
        public async Task WeekActivities_CreateActivityDropdownFilter()
        {
            var controller = new WorkbooksController(_context);

            int wbid = 1;
            int wkid = 1;
            var result = controller.CreateActivity(wbid, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["ActivitiesId"]);
            var filteredActivities = selectList.Items.Cast<Activities>().ToList();

            Assert.Single(filteredActivities);
            Assert.Equal("Assignment", filteredActivities[0].ActivityName);
        }

        [Fact]
        public async Task WeekActivities_CreateLearningTypeDropdownFilter()
        {
            var controller = new WorkbooksController(_context);

            int wbid = 1;
            int wkid = 1;
            var result = controller.CreateActivity(wbid, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["LearningApproach"]);
            var filteredLearningType = selectList.Items.Cast<LearningType>().ToList();

            Assert.Equal(2, filteredLearningType.Count);
            Assert.Equal("Acquisition", filteredLearningType[0].LearningTypeName);
            Assert.Equal("#A1F5ED", filteredLearningType[0].LearningTypeColour);
        }

        [Fact]
        public async Task WeekActivities_CreateWeekDropdownFilter()
        {
            var controller = new WorkbooksController(_context);
            int wbId = 1;
            int wkid = 1;

            var result = controller.CreateActivity(wbId, wkid) as ViewResult;

            Assert.NotNull(result);
            var selectList = Assert.IsType<SelectList>(result.ViewData["WeekId"]);
            var filteredWeeks = selectList.Items.Cast<Week>().ToList();

            Assert.Equal(2, filteredWeeks.Count);
            Assert.All(filteredWeeks, w => Assert.Equal(wbId, w.WorkbookId));
        }
    }
}
