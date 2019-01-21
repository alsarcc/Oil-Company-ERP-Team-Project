using OilTeamProject.Persistence;
using OilTeamProject.ViewModels;
using System.Linq;

namespace OilTeamProject.Repositories
{
    public class DepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetDepartmentId(AssignShiftEmployeesViewModel viewModel)
        {
            return _context.Departments.Where(d => d.Id == viewModel.Department.Id).Single().Id;
        }
    }
}