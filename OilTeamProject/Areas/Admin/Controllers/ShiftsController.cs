using OilTeamProject.Models.Employees;
using OilTeamProject.Persistence;
using OilTeamProject.Repositories;
using OilTeamProject.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OilTeamProject.Areas.Admin.Controllers
{
    public class ShiftsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ShiftRepository _shiftRepository;

        private readonly EmployeeRepository _employeeRepository;

        private readonly DepartmentRepository _departmentRepository;

        public ShiftsController()
        {
            _context = new ApplicationDbContext();
            _shiftRepository = new ShiftRepository(_context);
            _employeeRepository = new EmployeeRepository(_context);
            _departmentRepository = new DepartmentRepository(_context);
        }

        //Action To find Shift for New Work
        public ActionResult FindShiftForNewWork(AssignShiftEmployeesViewModel viewModel)
        {
            var datetime = viewModel.Datetime;
            var departmentId = _departmentRepository.GetDepartmentId(viewModel);
            var shiftTypeId = viewModel.ShiftTypeId;

            var shiftId = _shiftRepository.GetShiftIdByDayTimeAndDepartmentId(datetime, departmentId, shiftTypeId);

            TempData["ID"] = shiftId;
            //TempData["DepartmentID"] = departmentId;
            return RedirectToAction("NewWork");
        }

        //get
        public ActionResult NewWork(int? id)
        {
            var newId = Convert.ToInt32(TempData["ID"]);
            if (newId != 0)
            {
                id = newId;
            }

            //var depapartmentID = Convert.ToInt32(TempData["DepartmentID"]);
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var shift = _shiftRepository.GetShift(id);

            if (shift == null)
            {
                return HttpNotFound();
            }

            var employees = _employeeRepository.GetEmployeesWhoNotWorking(shift);

            var workingEmployees = _employeeRepository.GetEmployeesWhoAreWorking(shift);

            var viewModel = new AssignShiftEmployeesViewModel
            {
                Shift = shift,
                Employees = employees,
                WorkingEmployees = workingEmployees,
                Department = shift.Department,
            };

            return View("NewWork", viewModel);
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewWork(AssignShiftEmployeesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Employees = _context.Employees
                    .Where(e => e.DepartmentId == viewModel.Shift.DepartmentId)
                    .ToList();
                viewModel.Shift = _context.Shifts
                    .Include(s => s.Department)
                    .Include(s => s.ShiftType)
                    .Single();

                return View("NewWork", viewModel);
            }

            var employee = viewModel.EmployeeId;
            var shift = viewModel.Shift.Id;

            var exist = _context.Works.Any(w => w.EmployeeID == employee && w.ShiftId == shift);
            //var existandiscanceled = _context.Works.Any(w => w.EmployeeID == employee && w.ShiftId == shift && w.IsCanceled);

            //if (existandiscanceled)
            //{
            //    var updatedWork = _context.Works.Where(w => w.EmployeeID == employee && w.ShiftId == shift).Single();
            //    updatedWork.IsCanceled = false;
            //    _context.SaveChanges();
            //    return RedirectToAction("NewWork", viewModel.Shift.Id);
            //}
            if (exist)
            {
                return RedirectToAction("NewWork");
            }

            var work = new Work(viewModel.EmployeeId, viewModel.Shift.Id);
            _context.Works.Add(work);
            _context.SaveChanges();

            var shiftId = viewModel.Shift.Id;
            TempData["ID"] = shiftId;
            return RedirectToAction("NewWork", shiftId);
        }

        //Get
        public ActionResult AddAWorkWeek()
        {
            var viewModel = new WorkDayViewModel
            {
                Departments = _context.Departments
            };

            return View(viewModel);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAWorkWeek(WorkDayViewModel viewModel)
        {
            var shift = _context.Shifts
                .Include(s => s.Department)
                .Include(s => s.ShiftType)
                .Any(s => s.DateTime == viewModel.WorkDate && s.DepartmentId == viewModel.DepartmentId);

            viewModel.Departments = _context.Departments.ToList();

            if (viewModel.DepartmentId == 0 || viewModel.NumberOfWorkDays == 0 || viewModel.NumbersOfShifts == 0 || viewModel.WorkDate == null)
            {
                var returnViewModel = new WorkDayViewModel
                {
                    Departments = viewModel.Departments,
                    WorkDate = viewModel.WorkDate,
                    NumberOfWorkDays = viewModel.NumberOfWorkDays,
                    NumbersOfShifts = viewModel.NumbersOfShifts
                };
                return RedirectToAction("AddAWorkWeek", returnViewModel);
            }

            if (!shift)
            {
                for (int j = 0; j < viewModel.NumberOfWorkDays; j++)
                {
                    for (byte i = 1; i <= viewModel.NumbersOfShifts; i++)
                    {
                        var newShift = new Shift(viewModel.WorkDate.AddDays(j), i, viewModel.DepartmentId);
                        _context.Shifts.Add(newShift);
                    }
                }
                _context.SaveChanges();

                viewModel.Department = _context.Departments.Where(d => d.Id == viewModel.DepartmentId).Single();
                var assigneViewModel = new AssignShiftEmployeesViewModel
                {
                    Department = viewModel.Department,
                    ShiftTypes = _context.ShiftTypes.ToList()
                };

                return View("ChooseDateAssigment", assigneViewModel);
            }

            return View(viewModel);
        }

        public ActionResult Index(DateTime? dateTime, int? departmentId)
        {
            var departments = new GetWorkDayViewModel
            {
                Departments = _context.Departments.ToList()
            };
            if (dateTime == null && departmentId == null)
            {
                return View(departments);
            }
            else
            {
                var shiftIds = _context.Shifts.Where(s => s.DateTime == dateTime && s.DepartmentId == departmentId).Select(s => s.ShiftTypeId).ToList();
                var shifttypes = _context.ShiftTypes.Where(t => shiftIds.Contains(t.Id)).ToList();
                return RedirectToAction("GetWorkDay", new { currentDatetime = dateTime, currnetDpertmentId = departmentId, departmentsToList = departments, shiftToList = shifttypes });
            }
        }

        //Index (works)
        public ActionResult GetWorkDay(DateTime dateTime, int departmentId)
        {
            if (departmentId == 0)
            {
                return RedirectToAction("Index", null, null);
            }
            var shifts = _context.Shifts
                                .Include(s => s.Department)
                                .Include(s => s.ShiftType)
                                .Include(s => s.Works)
                                .Include(s => s.Works.Select(w => w.Employee))
                                .Where(s => s.DateTime == dateTime && s.DepartmentId == departmentId)
                                .ToList();
            if (shifts.Count == 0)
                return RedirectToAction("Index", null, null);

            var ShiftResults = shifts.First();

            var shiftIds = _context.Shifts.Where(s => s.DateTime == dateTime && s.DepartmentId == departmentId).Select(s => s.ShiftTypeId).ToList();
            var shifttypes = _context.ShiftTypes.Where(t => shiftIds.Contains(t.Id)).ToList();

            var employees = _context.Employees
                 .Where(e => e.DepartmentId == departmentId);

            var workDay = new GetWorkDayViewModel
            {
                Datetime = ShiftResults.DateTime,
                DepartmentId = departmentId,
                DepartemtName = ShiftResults.Department.Name,
                Shifts = shifts,
                Departments = _context.Departments.ToList(),
                ShiftTypes = shifttypes,
                Employees = employees,
            };

            return View("GetWorkDay", workDay);
        }

        //Create Get (works)
        public ActionResult Create()
        {
            var viewModel = new ShiftFormViewModel
            {
                Departments = _context.Departments.ToList(),
                ShiftTypes = _context.ShiftTypes.ToList(),
            };

            return View(viewModel);
        }

        //Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ShiftFormViewModel viewModel)
        //{
        //    if ((!ModelState.IsValid || !viewModel.IsValidDayComparedToDate())
        //        || (ModelState.IsValid && !viewModel.IsValidDayComparedToDate()))
        //    {
        //        viewModel.Departments = _context.Departments.ToList();
        //        viewModel.ShiftTypes = _context.ShiftTypes.ToList();

        //        return View("Create", viewModel);
        //    };

        //    var shift = new Shift(viewModel.GetDateTime(), viewModel.ShiftTypeId, viewModel.DepartmentId);
        //    _context.Shifts.Add(shift);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}
    }
}