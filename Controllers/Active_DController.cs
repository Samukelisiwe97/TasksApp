using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Models;
using TasksApp.Services;

namespace TasksApp.Controllers
{
    public class Active_DController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public Active_DController(ApplicationDbContext context , UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Active_D
        public async Task<IActionResult> Index()
        {
            return View(await _context.Active_D.ToListAsync());
        }

        // GET: Active_D/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_D = await _context.Active_D
                .FirstOrDefaultAsync(m => m.Id == id);
            if (active_D == null)
            {
                return NotFound();
            }

            return View(active_D);
        }

        // GET: Active_D/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Active_D/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Active_D active_D)
        {
            if (ModelState.IsValid)
            {
                _context.Add(active_D);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(active_D);
        }

        // GET: Active_D/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_D = await _context.Active_D.FindAsync(id);
            if (active_D == null)
            {
                return NotFound();
            }
            return View(active_D);
        }

        // POST: Active_D/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Active_D active_D)
        {
            if (id != active_D.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(active_D);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Active_DExists(active_D.Id))
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
            return View(active_D);
        }

        // GET: Active_D/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_D = await _context.Active_D
                .FirstOrDefaultAsync(m => m.Id == id);
            if (active_D == null)
            {
                return NotFound();
            }

            return View(active_D);
        }

        // POST: Active_D/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var active_D = await _context.Active_D.FindAsync(id);
            _context.Active_D.Remove(active_D);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Active_DExists(int id)
        {
            return _context.Active_D.Any(e => e.Id == id);
        }

        #region API Calls


        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "Active_D").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Active_D").ToList();

                foreach (var task in Main_Task)
                {

                    var Task = new Active_D
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskCategory = task.TaskCategory
                    };

                    _context.Active_D.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Active_D").ToList();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskCategory == "Active_D");

                    foreach (var item in result)
                    {
                        var Task = new Active_D
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskCategory = item.TaskCategory
                        };

                        _context.Active_D.Add(Task);
                    }

                }
            }

            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Active_D") });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksWeekly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var Day = oDate.DayOfWeek.ToString();

            var TasksToday = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();

                foreach (var task in Main_Task)
                {

                    var Task = new Active_D
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskCategory = task.TaskCategory

                    };

                    _context.Active_D.Add(Task);


                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                    foreach (var item in result)
                    {
                        var Task = new Active_D
                        {
                            Description = item.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = item.Schedule,
                            TaskCategory = item.TaskCategory
                        };

                        _context.Active_D.Add(Task);
                    }

                }

            }

            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksMonthly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksQuarterly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            //var quarter = new DateTime(oDate.Year, oDate.Month, 1);
            var firstDayOfQuarter = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);

            if (firstDayOfQuarter == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Quarterly == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Quarterly == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfQuarter == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Quarterly == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Quarterly == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksBi(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(6).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Bi_Annual == "1st").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Bi_Annual == "1st").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Bi_Annual == "2nd").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Bi_Annual == "2nd").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksAnnually(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var BYear = new DateTime(oDate.Year, oDate.Month, 1);
            var MidYear = BYear.AddMonths(6).AddDays(-1);
            var YearEnd = BYear.AddYears(1).AddDays(-1);


            if (BYear == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (MidYear == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "Mid").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "Mid").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (YearEnd == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskCategory = task.TaskCategory
                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Annual == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskCategory = item.TaskCategory
                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }
            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {


            var task = _context.Active_D.Find(id);
            task.IsDone = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.Identity.Name;

            var date = task.DateCreated;


            var tasks = _context.Tasks.Where(d => d.DateCreated == date).ToList();

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
                {
                    await _context.SaveChangesAsync(_userService.GetUser());

                    return Json(new { success = true, message = "Task Completed!" });
                }

                //else
                //{

                //    task.DateAllTaskCompleted = DateTime.Now;
                //    task.TasksCompleted = true;
                //}

            }



            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { success = true, message = "Task Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.Active_D.Find(id);

            task.Comments = comment;


            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.Active_D.Find(id);
            return Json(task);

        }

        #endregion
    }

}

