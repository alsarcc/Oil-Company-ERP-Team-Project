using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilTeamProject.Models.Employees
{
    public class PersonalDetails
    {
        public enum FamilyStatus
        {
            UnMarried = 1,
            Married,
            Divorced,
            Widowed
        }

        public enum SexStatus
        {
            Male = 1,
            Female,
            Other
        }

        [Key]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public FamilyStatus CurrentFamilyStatus { get; set; }

        public int NumberOfChildren { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public int Age
        {
            get
            {
                return DateTime.Now.Year - DateOfBirth.Year;
            }
        }

        public SexStatus Sex { get; set; }

        public string SSN { get; set; }

        public string IdentityCard { get; set; }
    }
}