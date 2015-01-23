using SubSonic.SqlGeneration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        public bool IsDefaultEmployee { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public double HourlyRate { get; set; }

        public int TaxExemptions { get; set; }

        public bool IsMarried { get; set; }

        [SubSonicIgnore]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        
    }
}
