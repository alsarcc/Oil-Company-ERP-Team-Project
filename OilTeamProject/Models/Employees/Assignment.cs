using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilTeamProject.Models.Employees
{
    public class Assignment
    {
        [Key]
        [Column(Order = 1)]
        public int EmployeeId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ProjectId { get; set; }

        public Employee Employee { get; set; }

        public Project Project { get; set; }
    }
}