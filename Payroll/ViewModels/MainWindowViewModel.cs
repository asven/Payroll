using Payroll.Common;
using SubSonic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            SimpleRepository repo = new SimpleRepository("Payroll", SimpleRepositoryOptions.RunMigrations);
            this.AllEmployees = repo.All<Employee>().ToList();
        }




        public List<Employee> AllEmployees { get; set; }
    }
}
