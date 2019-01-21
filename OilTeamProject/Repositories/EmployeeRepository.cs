using OilTeamProject.Models.Employees;
using OilTeamProject.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace OilTeamProject.Repositories
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Employee> GetEmployeesWhoNotWorking(Shift shift)
        {
            return _context.Employees
                .Where(e => e.DepartmentId == shift.Department.Id &&
                !e.Works.Any(w => w.ShiftId == shift.Id))
                .ToList();
        }

        public ICollection<Employee> GetEmployeesWhoAreWorking(Shift shift)
        {
            return _context.Employees
                .Where(e => e.DepartmentId == shift.Department.Id &&
                e.Works.Any(w => w.ShiftId == shift.Id))
                .ToList();
        }
    }
}