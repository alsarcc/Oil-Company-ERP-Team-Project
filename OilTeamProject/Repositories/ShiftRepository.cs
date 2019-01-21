using OilTeamProject.Models.Employees;
using OilTeamProject.Persistence;
using System;
using System.Data.Entity;
using System.Linq;

namespace OilTeamProject.Repositories
{
    public class ShiftRepository
    {
        private ApplicationDbContext _context;

        public ShiftRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Shift GetShift(int? id)
        {
            return _context.Shifts
                .Include(s => s.Department)
                .Include(s => s.ShiftType)
                .SingleOrDefault(s => s.Id == id);
        }

        public int GetShiftIdByDayTimeAndDepartmentId(DateTime datetime, int departmentId, int shiftTypeId)
        {
            return _context.Shifts.Where(s => s.DateTime == datetime && s.DepartmentId == departmentId && s.ShiftTypeId == shiftTypeId).Single().Id;
        }
    }
}