using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using CCM_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Controllers
{
    public class WorkbooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkbooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workbooks.ToListAsync());
        }

        // POST: Courses/Search
        public async Task<IActionResult> Search(string SearchPhrase)
        {
            // Check if the SearchPhrase is not null or empty
            if (string.IsNullOrEmpty(SearchPhrase))
            {
                // If no search phrase, return an empty list or all courses
                return View("Search", await _context.Workbooks.ToListAsync());
            }

            // Perform search filtering across multiple fields
            var searchResults = await _context
                .Workbooks.Where(c =>
                    c.CourseName.Contains(SearchPhrase) || c.CourseLead.Contains(SearchPhrase)
                )
                .ToListAsync();

            // Return the search form view with search results and the search phrase
            ViewData["SearchPhrase"] = SearchPhrase;
            return View("Search", searchResults);
        }

        //GET: Courses/Filter
        [HttpGet]
        public async Task<IActionResult> Filter(string searchPhrase, string filterPhrase)
        {
            var workbooks = _context.Workbooks.AsQueryable();

            // First, apply the search term (if provided)
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                workbooks = workbooks.Where(w =>
                    EF.Functions.Like(w.CourseName, $"%{searchPhrase}%")
                    || EF.Functions.Like(w.CourseLead, $"%{searchPhrase}%")
                );
            }

            // Then, apply the filter to the search results (if provided)
            if (!string.IsNullOrEmpty(filterPhrase))
            {
                workbooks = workbooks.Where(w =>
                    EF.Functions.Like(w.CourseName, $"%{filterPhrase}%")
                    || EF.Functions.Like(w.CourseLead, $"%{filterPhrase}%")
                );
            }

            // Preserve search and filter values in ViewData
            ViewData["SearchPhrase"] = searchPhrase;
            ViewData["FilterPhrase"] = filterPhrase;

            return View("Search", await workbooks.ToListAsync()); // Return filtered results to Search.cshtml
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
                .ThenInclude(wa => wa.LearningType!) // Include LearningType
                .Include(w => w.Weeks!)
                .ThenInclude(week => week.WeekActivities!)
                .ThenInclude(wa => wa.Activities!) // Include Activities
                .Include(w => w.LearningPlatform)
                .FirstOrDefaultAsync(m => m.WorkbookId == id);

            if (workbook == null)
            {
                return NotFound();
            }

            // Create a dictionary to store total time spent per learning type per week
            var timeBreakdown = new Dictionary<int, Dictionary<string, TimeSpan>>();

            foreach (var week in workbook.Weeks)
            {
                var weekData = new Dictionary<string, TimeSpan>
                {
                    { "Acquisition", TimeSpan.Zero },
                    { "Collaboration", TimeSpan.Zero },
                    { "Discussion", TimeSpan.Zero },
                    { "Investigation", TimeSpan.Zero },
                    { "Practice", TimeSpan.Zero },
                    { "Production", TimeSpan.Zero },
                    { "Assessment", TimeSpan.Zero },
                };

                foreach (var activity in week.WeekActivities)
                {
                    if (weekData.ContainsKey(activity.LearningType.LearningTypeName))
                    {
                        weekData[activity.LearningType.LearningTypeName] += activity.TaskTime;
                    }
                }

                timeBreakdown[week.WeekNumber] = weekData;
            }

            ViewBag.TimeBreakdown = timeBreakdown;

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
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,Collaborators"
            )]
                Workbook workbook
        )
        {
            try
            {
                var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(lp =>
                    lp.PlatformId == workbook.LearningPlatformId
                );

                workbook.LearningPlatform = learningPlatform;

                if (workbook.LearningPlatform == null)
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

                    return View(workbook);
                }
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

                return View(workbook);
            }

            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid)
            {
                try
                {
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

            var workbook = await _context
                .Workbooks.Include(w => w.LearningPlatform)
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
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,Collaborators"
            )]
                Workbook workbook
        )
        {
            if (id != workbook.WorkbookId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid)
            {
                try
                {
                    var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(
                        lp => lp.PlatformId == workbook.LearningPlatformId
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
                        return View(workbook);
                    }

                    workbook.LearningPlatform = learningPlatform;
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
                return RedirectToAction(nameof(Index));
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
                .Include(w => w.WeekGraduateAttributes)
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

            ViewBag.learningTypeCounts = learningTypeCounts;
            ViewBag.locationCount = locationCount;
            ViewBag.progressCount = progressCount;
            ViewData["LearningTypes"] = await _context.LearningType.ToListAsync();

            var viewModel = new WeekDetailsViewModel { Week = week, ActivitiesList = activities };
            return View(viewModel);
        }

        // GET: WeekActivities/Edit/5
        public async Task<IActionResult> EditActivity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weekActivity = await _context
                .WeekActivities.Include(w => w.Week)
                .FirstOrDefaultAsync(w => w.WeekActivityId == id);

            if (weekActivity == null)
            {
                return NotFound();
            }

            ViewBag.WorkbookId = weekActivity.Week.WorkbookId;
            ViewBag.CrumbWeekId = weekActivity.Week.WeekId;

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivity.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                _context.Activities,
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
        public IActionResult CreateActivity(int id, int weekId)
        {
            var filteredWeeks = _context.Weeks.Where(w => w.WorkbookId == id).ToList();

            Console.WriteLine($"Filtered Weeks: {filteredWeeks.Count}: {id}");

            var crumbWeek = _context.Weeks.FirstOrDefault(w => w.WeekId == weekId);

            if (crumbWeek == null)
            {
                return NotFound();
            }

            ViewBag.CrumbWorkbookId = id;
            ViewBag.CrumbWeekId = crumbWeek.WeekId;

            ViewBag.WeekId = new SelectList(filteredWeeks, "WeekId", "WeekNumber");
            ViewBag.ActivitiesId = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName"
            );
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
                    return RedirectToAction(nameof(Week), new { id = weekActivities.WeekId });
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    ModelState.AddModelError("", $"Error creating WeekActivity: {exp.Message}");
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
                    "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
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
        public async Task<IActionResult> ActivityUp([FromBody] int id)
        {
            var activity = await _context.WeekActivities.FirstOrDefaultAsync(a =>
                a.WeekActivityId == id
            );

            if (activity == null)
            {
                return NotFound();
            }

            var taskToSwap = await _context.WeekActivities.FirstOrDefaultAsync(a =>
                a.TaskOrder == activity.TaskOrder - 1
            );

            if (taskToSwap == null)
            {
                return NoContent();
            }

            int tempOrder = activity.TaskOrder;
            activity.TaskOrder = taskToSwap.TaskOrder;
            taskToSwap.TaskOrder = tempOrder;

            _context.Update(activity);
            _context.Update(taskToSwap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ActivityDown([FromBody] int id)
        {
            var activity = await _context.WeekActivities.FirstOrDefaultAsync(a =>
                a.WeekActivityId == id
            );

            if (activity == null)
            {
                return NotFound();
            }

            var taskToSwap = await _context.WeekActivities.FirstOrDefaultAsync(a =>
                a.TaskOrder == activity.TaskOrder + 1
            );

            if (taskToSwap == null)
            {
                return NoContent();
            }

            int tempOrder = activity.TaskOrder;
            activity.TaskOrder = taskToSwap.TaskOrder;
            taskToSwap.TaskOrder = tempOrder;

            _context.Update(activity);
            _context.Update(taskToSwap);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
