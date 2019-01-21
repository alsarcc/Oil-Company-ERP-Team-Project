using OilTeamProject.Areas.Admin.Controllers;
using OilTeamProject.Models.Employees;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace OilTeamProject.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DepartmentId { get; set; }

        public int RoleId { get; set; }

        public double Salary { get; set; }

        public int RemainingDaysOfLeave { get; set; }

        public PersonalDetails PersonalDetails { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Role> Roles { get; set; }

        public string Heading { get; set; }

        public string Action
        {
            get
            {
                Expression<Func<EmployeesController, ActionResult>> update =
                    (c => c.Update(this));
                Expression<Func<EmployeesController, ActionResult>> create =
                    (c => c.Create(this));

                var action = (Id != 0) ? update : create;
                var actionName = (action.Body as MethodCallExpression).Method.Name;

                return actionName;
            }
        }

        public EmployeeFormViewModel()
        {
        }

        public EmployeeFormViewModel(Employee employee)
        {
            PersonalDetails = employee.PersonalDetails;
            ContactDetails = employee.ContactDetails;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            RoleId = employee.RoleId;
            DepartmentId = employee.DepartmentId;
            Id = employee.Id;
            Heading = "Update Employee";
            Salary = employee.Salary;
            RemainingDaysOfLeave = employee.RemaingDaysOfLeave;
        }
    }
}