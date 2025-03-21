using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions; //need regex for filtering
using System.Threading.Tasks;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using CCM_Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CCM_Website.Controllers
{
    public class WorkbooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public WorkbooksController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService
        )
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // GET: Courses
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var dbContext = _context
                .Workbooks.Where(w => w.OwnerId == userId)
                .Include(w => w.UniversityArea)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                dbContext = dbContext.Where(w =>
                    w.CourseName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedWorkbooks = dbContext.ToPagedList(pageNumber, pageSize);
            return Task.FromResult<IActionResult>(View(pagedWorkbooks));
        }

        // POST: Coursess/Search
        public Task<IActionResult> Search(
            string searchPhrase,
            string courseCodePrefix,
            string courseLead,
            int? page
        )
        {
            var query = _context.Workbooks.Include(w => w.UniversityArea).AsQueryable();

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(c =>
                    c.CourseName.ToLower().Contains(searchPhrase.ToLower())
                    || c.CourseLead.ToLower().Contains(searchPhrase.ToLower())
                );
            }

            if (!string.IsNullOrEmpty(courseCodePrefix))
            {
                query = query.Where(c =>
                    c.CourseCode != null && c.CourseCode.StartsWith(courseCodePrefix)
                );
            }

            if (!string.IsNullOrEmpty(courseLead))
            {
                query = query.Where(c => c.CourseLead == courseLead);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedResults = query.ToPagedList(pageNumber, pageSize);

            ViewData["CourseCodePrefixes"] = _context
                .Workbooks.AsEnumerable()
                .Select(c =>
                    c.CourseCode != null
                        ? Regex.Match(c.CourseCode, @"^[A-Za-z]+").Value
                        : string.Empty
                )
                .Where(prefix => !string.IsNullOrEmpty(prefix))
                .Distinct()
                .OrderBy(prefix => prefix)
                .ToList();

            ViewData["CourseLeads"] = _context
                .Workbooks.Select(c => c.CourseLead)
                .Distinct()
                .ToList();

            ViewData["SearchPhrase"] = searchPhrase;
            ViewData["CourseCodePrefix"] = courseCodePrefix;
            ViewData["CourseLead"] = courseLead;

            return Task.FromResult<IActionResult>(View("Search", pagedResults));
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context
                .Workbooks.Include(w => w.Weeks!)
                .ThenInclude(week => week.WeekGraduateAttributes!)
                .ThenInclude(wga => wga.GraduateAttribute!)
                .Include(w => w.Weeks!)
                .ThenInclude(week => week.WeekActivities!)
                .ThenInclude(wa => wa.LearningType!)
                .Include(w => w.Weeks!)
                .ThenInclude(week => week.WeekActivities!)
                .ThenInclude(wa => wa.Activities!)
                .Include(w => w.LearningPlatform)
                .FirstOrDefaultAsync(m => m.WorkbookId == id);

            if (workbook == null)
            {
                return NotFound();
            }

            var allLearningTypes = _context.LearningType.ToList();

            var timeBreakdown = new Dictionary<int, Dictionary<string, TimeSpan>>();

            foreach (var week in workbook.Weeks ?? Enumerable.Empty<Week>())
            {
                var weekData = allLearningTypes.ToDictionary(
                    type => type.LearningTypeName,
                    type => TimeSpan.Zero
                );

                foreach (var activity in week.WeekActivities ?? Enumerable.Empty<WeekActivities>())
                {
                    if (
                        activity.LearningType != null
                        && weekData.ContainsKey(activity.LearningType.LearningTypeName)
                    )
                    {
                        weekData[activity.LearningType.LearningTypeName] += activity.TaskTime;
                    }
                }

                timeBreakdown[week.WeekNumber] = weekData;
            }

            ViewBag.TimeBreakdown = timeBreakdown;
            ViewBag.LearningTypes = allLearningTypes;

            ViewBag.TimeBreakdown = timeBreakdown;
            ViewData["GraduateAttributes"] = await _context.GraduateAttributes.ToListAsync();
            ViewData["LearningTypes"] = await _context.LearningType.ToListAsync();

            return View(workbook);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName"
            );
            ViewBag.UniversityAreas = new SelectList(_context.UniversityArea, "AreaId", "AreaName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,UniversityAreaId,Collaborators"
            )]
                Workbook workbook
        )
        {
            try
            {
                var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(lp =>
                    lp.PlatformId == workbook.LearningPlatformId
                );

                var uniArea = await _context.UniversityArea.FirstOrDefaultAsync(ua =>
                    ua.AreaId == workbook.UniversityAreaId
                );

                if (learningPlatform == null)
                {
                    Console.WriteLine("ERROR: Failed to link Workbook to Learning Platform");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );

                    ViewBag.LearningPlatforms = new SelectList(
                        _context.LearningPlatforms,
                        "PlatformId",
                        "PlatformName"
                    );
                    ViewBag.UniversityAreas = new SelectList(
                        _context.UniversityArea,
                        "AreaId",
                        "AreaName"
                    );
                    return View(workbook);
                }

                if (uniArea == null)
                {
                    Console.WriteLine("ERROR: Failed to link Workbook to University Area");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );

                    ViewBag.LearningPlatforms = new SelectList(
                        _context.LearningPlatforms,
                        "PlatformId",
                        "PlatformName"
                    );
                    ViewBag.UniversityAreas = new SelectList(
                        _context.UniversityArea,
                        "AreaId",
                        "AreaName"
                    );
                    return View(workbook);
                }

                workbook.LearningPlatform = learningPlatform;
                workbook.UniversityArea = uniArea;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the workbook. Please try again later."
                );

                ViewBag.LearningPlatforms = new SelectList(
                    _context.LearningPlatforms,
                    "PlatformId",
                    "PlatformName"
                );
                ViewBag.UniversityAreas = new SelectList(
                    _context.UniversityArea,
                    "AreaId",
                    "AreaName"
                );
                return View(workbook);
            }

            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.UniversityArea));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId == null)
                        return Unauthorized();
                    workbook.OwnerId = userId;

                    workbook.LastEdited = DateTime.Now;
                    _context.Add(workbook);
                    await _context.SaveChangesAsync();

                    int numberOfWeeks = workbook.CourseLength;
                    var weeks = new List<Week>();

                    for (int i = 1; i <= numberOfWeeks; i++)
                    {
                        weeks.Add(
                            new Week
                            {
                                WeekNumber = i,
                                WorkbookId = workbook.WorkbookId,
                                Workbook = workbook,
                                WeekActivities = new List<WeekActivities>(),
                                WeekGraduateAttributes = new List<WeekGraduateAttributes>(),
                            }
                        );
                    }

                    workbook.Weeks = weeks;
                    _context.AddRange(weeks);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB Error occurred: {ex.Message}");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );

                    ViewBag.LearningPlatforms = new SelectList(
                        _context.LearningPlatforms,
                        "PlatformId",
                        "PlatformName"
                    );
                    ViewBag.UniversityAreas = new SelectList(
                        _context.UniversityArea,
                        "AreaId",
                        "AreaName"
                    );

                    return View(workbook);
                }
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
            );

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName"
            );

            return View(workbook);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Workbooks.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var workbook = await _context
                .Workbooks.Include(w => w.LearningPlatform)
                .Include(w => w.UniversityArea)
                .FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName",
                workbook.LearningPlatformId
            );
            ViewBag.UniversityAreas = new SelectList(
                _context.UniversityArea,
                "AreaId",
                "AreaName",
                workbook.UniversityAreaId
            );
            return View(workbook);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,UniversityAreaId,Collaborators"
            )]
                Workbook workbook
        )
        {
            if (id != workbook.WorkbookId)
            {
                return NotFound();
            }

            var entity = await _context
                .Workbooks.AsNoTracking()
                .FirstOrDefaultAsync(w => w.WorkbookId == id);
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));
            ModelState.Remove(nameof(workbook.UniversityArea));

            if (ModelState.IsValid)
            {
                try
                {
                    var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(
                        lp => lp.PlatformId == workbook.LearningPlatformId
                    );

                    var uniArea = await _context.UniversityArea.FirstOrDefaultAsync(ua =>
                        ua.AreaId == workbook.UniversityAreaId
                    );

                    if (learningPlatform == null)
                    {
                        ModelState.AddModelError(
                            "LearningPlatformId",
                            "Invalid Learning Platform selected."
                        );
                        ViewBag.LearningPlatforms = new SelectList(
                            _context.LearningPlatforms,
                            "PlatformId",
                            "PlatformName",
                            workbook.LearningPlatformId
                        );
                        ViewBag.UniversityAreas = new SelectList(
                            _context.UniversityArea,
                            "AreaId",
                            "AreaName",
                            workbook.UniversityAreaId
                        );
                        return View(workbook);
                    }
                    else if (uniArea == null)
                    {
                        ModelState.AddModelError("UniversityAreaId", "Invalid Area selected.");
                        ViewBag.LearningPlatforms = new SelectList(
                            _context.LearningPlatforms,
                            "PlatformId",
                            "PlatformName",
                            workbook.LearningPlatformId
                        );
                        ViewBag.UniversityAreas = new SelectList(
                            _context.UniversityArea,
                            "AreaId",
                            "AreaName",
                            workbook.UniversityAreaId
                        );
                        return View(workbook);
                    }

                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId == null)
                        return Unauthorized();
                    workbook.OwnerId = userId;
                    workbook.LearningPlatform = learningPlatform;
                    workbook.UniversityArea = uniArea;
                    workbook.LastEdited = DateTime.Now;
                    _context.Update(workbook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkbookExists(workbook.WorkbookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), new { id = workbook.WorkbookId });
            }

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName",
                workbook.LearningPlatformId
            );

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
            );
            return View(workbook);
        }

        //-----------------------------------------------------------
        //                        WEEKS
        //-----------------------------------------------------------


        // GET: Workbooks/Week/1
        public async Task<IActionResult> Week(int id)
        {
            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .ThenInclude(wb => wb.Weeks)
                .Include(w => w.Workbook)
                .ThenInclude(wb => wb.LearningPlatform)
                .Include(w => w.WeekActivities)
                .Include(w => w.WeekGraduateAttributes)!
                .ThenInclude(wga => wga.GraduateAttribute)
                .FirstOrDefaultAsync(w => w.WeekId == id);

            if (week == null)
            {
                return NotFound();
            }

            var activities = await _context
                .WeekActivities.Include(a => a.Week)
                .ThenInclude(wk => wk.Workbook)
                .Include(a => a.Activities)
                .Include(a => a.LearningType)
                .Include(a => a.TaskApproach)
                .Include(a => a.TaskLocation)
                .Include(a => a.TasksStatus)
                .Where(a => a.WeekId == id)
                .OrderBy(w => w.TaskOrder)
                .ToListAsync();

            var learningTypeCounts = activities
                .GroupBy(a => a.LearningType.LearningTypeName)
                .ToDictionary(g => g.Key, g => g.Count());

            var locationCount = activities
                .GroupBy(a => a.TaskLocation.LocationName)
                .ToDictionary(g => g.Key, g => g.Count());

            var progressCount = activities
                .GroupBy(a => a.TasksStatus.StatusName)
                .ToDictionary(g => g.Key, g => g.Count());

            var taskStatus = await _context.TaskProgressStatus.ToListAsync();
            var taskApproach = await _context.TaskApproach.ToListAsync();
            var taskLocation = await _context.TaskLocation.ToListAsync();

            ViewBag.learningTypeCounts = learningTypeCounts;
            ViewBag.locationCount = locationCount;
            ViewBag.progressCount = progressCount;
            ViewData["LearningTypes"] = await _context.LearningType.ToListAsync();

            var viewModel = new WeekDetailsViewModel
            {
                Week = week,
                ActivitiesList = activities,
                StatusList = taskStatus,
                ApproachList = taskApproach,
                LocationList = taskLocation,
            };
            return View(viewModel);
        }

        // GET: WeekActivities/Edit/5
        public async Task<IActionResult> EditActivity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context
                .WeekActivities.Where(w => w.WeekActivityId == id)
                .Select(w => w.Week.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var weekActivity = await _context
                .WeekActivities.Include(w => w.Week)
                .ThenInclude(week => week.Workbook)
                .FirstOrDefaultAsync(w => w.WeekActivityId == id);

            if (weekActivity == null)
            {
                return NotFound();
            }

            var learningPlatformId = weekActivity.Week.Workbook.LearningPlatformId;

            var allowedActivities = _context
                .LearningPlatformActivities.Where(lpa =>
                    lpa.LearningPlatformId == learningPlatformId
                )
                .Select(lpa => lpa.Activities)
                .ToList();

            ViewBag.WorkbookId = weekActivity.Week.WorkbookId;
            ViewBag.CrumbWeekId = weekActivity.Week.WeekId;

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivity.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                allowedActivities,
                "ActivityId",
                "ActivityName",
                weekActivity.ActivitiesId
            );
            ViewData["LearningApproach"] = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );

            ViewData["TaskApproach"] = new SelectList(
                _context.TaskApproach,
                "ApproachId",
                "ApproachName"
            );
            ViewData["TaskLocation"] = new SelectList(
                _context.TaskLocation,
                "LocationId",
                "LocationName"
            );
            ViewData["TaskStatus"] = new SelectList(
                _context.TaskProgressStatus,
                "StatusId",
                "StatusName"
            );

            return View(weekActivity);
        }

        // POST: WeekActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActivity(
            int id,
            [Bind(
                "WeekActivityId, TaskOrder,WeekId,ActivitiesId,TaskTitle,TaskStaff,TaskTime,TasksStatusId,LearningTypeId,TaskLocationId,TaskApproachId"
            )]
                WeekActivities weekActivities
        )
        {
            if (id != weekActivities.WeekActivityId)
            {
                return NotFound();
            }

            var entity = await _context
                .WeekActivities.Where(w => w.WeekActivityId == id)
                .Select(w => w.Week.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            ModelState.Remove(nameof(weekActivities.Week));
            ModelState.Remove(nameof(weekActivities.Activities));
            ModelState.Remove(nameof(weekActivities.LearningType));
            ModelState.Remove(nameof(weekActivities.TaskLocation));
            ModelState.Remove(nameof(weekActivities.TaskApproach));
            ModelState.Remove(nameof(weekActivities.TasksStatus));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weekActivities);
                    await _context.SaveChangesAsync();
                    var week = await _context
                        .Weeks.Include(w => w.Workbook)
                        .FirstOrDefaultAsync(w => w.WeekId == weekActivities.WeekId);

                    if (week != null && week.Workbook != null)
                    {
                        week.Workbook.LastEdited = DateTime.Now;
                        _context.Update(week.Workbook);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Week), new { id = weekActivities.WeekId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeekActivitiesExists(weekActivities.WeekActivityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }

                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
                );
            }

            // Assigning FK links
            try
            {
                var week = await _context.Weeks.FirstOrDefaultAsync(wk =>
                    wk.WeekId == weekActivities.WeekId
                );
                var activity = await _context.Activities.FirstOrDefaultAsync(a =>
                    a.ActivityId == weekActivities.ActivitiesId
                );
                var learingType = await _context.LearningType.FirstOrDefaultAsync(lt =>
                    lt.LearningTypeId == weekActivities.LearningTypeId
                );
                var taskStatus = await _context.TaskProgressStatus.FirstOrDefaultAsync(ts =>
                    ts.StatusId == weekActivities.TasksStatusId
                );
                var taskApproach = await _context.TaskApproach.FirstOrDefaultAsync(t =>
                    t.ApproachId == weekActivities.TaskApproachId
                );
                var taskLocation = await _context.TaskLocation.FirstOrDefaultAsync(t =>
                    t.LocationId == weekActivities.TaskLocationId
                );

                if (
                    week == null
                    || activity == null
                    || learingType == null
                    || taskStatus == null
                    || taskApproach == null
                    || taskLocation == null
                )
                {
                    Console.WriteLine($"ERROR: Link Fail");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the weekly activity. Please try again later."
                    );
                    return View(weekActivities);
                }

                weekActivities.Week = week;
                weekActivities.Activities = activity;
                weekActivities.LearningType = learingType;
                weekActivities.TasksStatus = taskStatus;
                weekActivities.TaskApproach = taskApproach;
                weekActivities.TaskLocation = taskLocation;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the weekly activity. Please try again later."
                );
                return View(weekActivities);
            }

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivities.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName",
                weekActivities.ActivitiesId
            );
            ViewData["LearningApproach"] = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );
            ViewData["TaskApproach"] = new SelectList(
                _context.TaskApproach,
                "ApproachId",
                "ApproachName"
            );
            ViewData["TaskLocation"] = new SelectList(
                _context.TaskLocation,
                "LocationId",
                "LocationName"
            );
            ViewData["TaskStatus"] = new SelectList(
                _context.TaskProgressStatus,
                "StatusId",
                "StatusName"
            );

            return View(weekActivities);
        }

        // GET: WeekActivities/Create
        public async Task<IActionResult> CreateActivity(int id, int weekId)
        {
            var entity = await _context
                .Weeks.Where(w => w.WeekId == weekId)
                .Select(w => w.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var filteredWeeks = _context.Weeks.Where(w => w.WorkbookId == id).ToList();

            Console.WriteLine($"Filtered Weeks: {filteredWeeks.Count}: {id}");

            var crumbWeek = _context
                .Weeks.Include(week => week.Workbook)
                .FirstOrDefault(w => w.WeekId == weekId);

            if (crumbWeek == null)
            {
                return NotFound();
            }

            ViewBag.CrumbWorkbookId = id;
            ViewBag.CrumbWeekId = crumbWeek.WeekId;

            var learningPlatformId = crumbWeek.Workbook.LearningPlatformId;

            var allowedActivities = _context
                .LearningPlatformActivities.Where(lpa =>
                    lpa.LearningPlatformId == learningPlatformId
                )
                .Select(lpa => lpa.Activities)
                .ToList();

            ViewBag.WeekId = new SelectList(filteredWeeks, "WeekId", "WeekNumber");
            ViewBag.ActivitiesId = new SelectList(allowedActivities, "ActivityId", "ActivityName");
            ViewBag.LearningApproach = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );
            ViewBag.TaskApproach = new SelectList(
                _context.TaskApproach,
                "ApproachId",
                "ApproachName"
            );
            ViewBag.TaskLocation = new SelectList(
                _context.TaskLocation,
                "LocationId",
                "LocationName"
            );
            ViewBag.TaskStatus = new SelectList(
                _context.TaskProgressStatus,
                "StatusId",
                "StatusName"
            );

            int maxTaskOrder =
                await _context
                    .WeekActivities.Where(w => w.WeekId == weekId)
                    .Select(w => (int?)w.TaskOrder)
                    .MaxAsync() ?? 0;

            ViewBag.NextTaskOrder = maxTaskOrder + 1;

            return View();
        }

        // POST: WeekActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateActivity(
            [Bind(
                "WeekActivityId, TaskOrder,WeekId,ActivitiesId,TaskTitle,TaskStaff,TaskTime,TasksStatusId,LearningTypeId,TaskLocationId,TaskApproachId"
            )]
                WeekActivities weekActivities
        )
        {
            var entity = await _context
                .Weeks.Where(w => w.WeekId == weekActivities.WeekId)
                .Select(w => w.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            ModelState.Remove(nameof(weekActivities.Week));
            ModelState.Remove(nameof(weekActivities.Activities));
            ModelState.Remove(nameof(weekActivities.LearningType));
            ModelState.Remove(nameof(weekActivities.TaskLocation));
            ModelState.Remove(nameof(weekActivities.TaskApproach));
            ModelState.Remove(nameof(weekActivities.TasksStatus));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(weekActivities);
                    await _context.SaveChangesAsync();
                    var week = await _context
                        .Weeks.Include(w => w.Workbook)
                        .FirstOrDefaultAsync(w => w.WeekId == weekActivities.WeekId);

                    if (week != null && week.Workbook != null)
                    {
                        week.Workbook.LastEdited = DateTime.Now;
                        _context.Update(week.Workbook);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Week), new { id = weekActivities.WeekId });
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    ModelState.AddModelError("", $"Error creating Weekly Activity: {exp.Message}");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        // Log each error (you could also store or display them if needed)
                        Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                    }
                }

                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the Weekly Activity. Please contact an administrator if this problem persists."
                );
            }

            try
            {
                var week = await _context.Weeks.FirstOrDefaultAsync(wk =>
                    wk.WeekId == weekActivities.WeekId
                );
                var activity = await _context.Activities.FirstOrDefaultAsync(a =>
                    a.ActivityId == weekActivities.ActivitiesId
                );
                var learingType = await _context.LearningType.FirstOrDefaultAsync(lt =>
                    lt.LearningTypeId == weekActivities.LearningTypeId
                );
                var taskStatus = await _context.TaskProgressStatus.FirstOrDefaultAsync(ts =>
                    ts.StatusId == weekActivities.TasksStatusId
                );
                var taskApproach = await _context.TaskApproach.FirstOrDefaultAsync(t =>
                    t.ApproachId == weekActivities.TaskApproachId
                );
                var taskLocation = await _context.TaskLocation.FirstOrDefaultAsync(t =>
                    t.LocationId == weekActivities.TaskLocationId
                );

                if (
                    week == null
                    || activity == null
                    || learingType == null
                    || taskStatus == null
                    || taskApproach == null
                    || taskLocation == null
                )
                {
                    Console.WriteLine($"ERROR: Link Fail");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the weekly activity. Please try again later."
                    );
                    return View(weekActivities);
                }

                weekActivities.Week = week;
                weekActivities.Activities = activity;
                weekActivities.LearningType = learingType;
                weekActivities.TasksStatus = taskStatus;
                weekActivities.TaskApproach = taskApproach;
                weekActivities.TaskLocation = taskLocation;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the weekly activity. Please try again later."
                );
                return View(weekActivities);
            }

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivities.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName",
                weekActivities.ActivitiesId
            );
            ViewData["LearningApproach"] = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );
            ViewData["TaskApproach"] = new SelectList(
                _context.TaskApproach,
                "ApproachId",
                "ApproachName"
            );
            ViewData["TaskLocation"] = new SelectList(
                _context.TaskLocation,
                "LocationId",
                "LocationName"
            );
            ViewData["TaskStatus"] = new SelectList(
                _context.TaskProgressStatus,
                "StatusId",
                "StatusName"
            );

            return View(weekActivities);
        }

        private bool WorkbookExists(int id)
        {
            return _context.Workbooks.Any(e => e.WorkbookId == id);
        }

        private bool WeekActivitiesExists(int id)
        {
            return _context.WeekActivities.Any(e => e.WeekActivityId == id);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeActivityOrder(
            [FromBody] Dictionary<string, int> activityUpdates
        )
        {
            if (activityUpdates == null || !activityUpdates.Any())
            {
                return BadRequest("No activity data provided.");
            }

            if (!activityUpdates.TryGetValue("workbookId", out int workbookId))
            {
                return BadRequest("workbookId is missing");
            }

            var entity = await _context.Workbooks.FirstOrDefaultAsync(w =>
                w.WorkbookId == workbookId
            );
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            activityUpdates.Remove("workbookId");
            var activityIds = activityUpdates.Keys.Select(int.Parse).ToList();
            var existingActivities = await _context
                .WeekActivities.Where(a => activityIds.Contains(a.WeekActivityId))
                .ToListAsync();

            if (existingActivities.Count != activityUpdates.Count)
            {
                return NotFound("One or more activities not found.");
            }

            foreach (var activity in existingActivities)
            {
                activity.TaskOrder = activityUpdates[activity.WeekActivityId.ToString()];
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: Workbook/AssignAttributes
        public async Task<IActionResult> AssignAttributes(int weekId)
        {
            if (weekId == 0)
            {
                return BadRequest("Invalid weekId received.");
            }

            var entity = await _context
                .Weeks.Where(w => w.WeekId == weekId)
                .Select(w => w.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var week = _context
                .Weeks.Include(w => w.WeekGraduateAttributes)!
                .ThenInclude(wga => wga.GraduateAttribute)
                .FirstOrDefault(w => w.WeekId == weekId);

            if (week == null)
            {
                return NotFound();
            }

            var allAttributes =
                _context.GraduateAttributes?.ToList() ?? new List<GraduateAttribute>();
            if (week.WeekGraduateAttributes != null)
            {
                var assignedAttributeIds = week
                    .WeekGraduateAttributes.Select(wga => wga.GraduateAttribute.AttributeId)
                    .ToList();
                var attributeSelectList = allAttributes
                    .Select(attr => new SelectListItem
                    {
                        Value = attr.AttributeId.ToString(),
                        Text = attr.AttributeName,
                        Selected = assignedAttributeIds.Contains(attr.AttributeId),
                    })
                    .ToList();

                ViewBag.WorkbookId = entity.WorkbookId;
                ViewBag.WeekId = weekId;
                ViewBag.GraduateAttributes = attributeSelectList;
            }

            return View("~/Views/Workbooks/AssignAttributes.cshtml");
        }

        // POST: Workbook/AssignAttributes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAttributes(int weekId, List<int> attributeIds)
        {
            if (weekId == 0)
            {
                return BadRequest("Invalid weekId received.");
            }

            var entity = await _context
                .Weeks.Where(w => w.WeekId == weekId)
                .Select(w => w.Workbook)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User,
                entity,
                "CanAccessResource"
            );
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .Include(w => w.WeekGraduateAttributes)
                .FirstOrDefaultAsync(w => w.WeekId == weekId);

            if (week == null)
            {
                return NotFound();
            }

            if (week.WeekGraduateAttributes != null)
            {
                var existingAttributes = week
                    .WeekGraduateAttributes.Select(wga => wga.GraduateAttributeId)
                    .ToList();
                var attributesToAdd = attributeIds.Except(existingAttributes).ToList();
                var attributesToRemove = existingAttributes.Except(attributeIds).ToList();

                foreach (var attrId in attributesToAdd)
                {
                    var attribute = await _context.GraduateAttributes.FindAsync(attrId);
                    if (attribute != null)
                    {
                        _context.WeekGraduateAttributes.Add(
                            new WeekGraduateAttributes
                            {
                                WeekId = weekId,
                                GraduateAttributeId = attrId,
                                Week = week,
                                GraduateAttribute = attribute,
                            }
                        );
                    }
                }

                var attributesToDelete = _context.WeekGraduateAttributes.Where(wga =>
                    wga.WeekId == weekId && attributesToRemove.Contains(wga.GraduateAttributeId)
                );
                _context.WeekGraduateAttributes.RemoveRange(attributesToDelete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Workbooks", new { id = week.Workbook.WorkbookId });
        }
    }
}
