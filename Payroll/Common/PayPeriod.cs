using SubSonic.SqlGeneration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common
{
    public class PayPeriod
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public double FederalWithholding { get; set; }
        public double SocialSecurityWithholding { get; set; }
        public double IncomeTax { get; set; }
        public double Hours { get; set; }
        public double MedicareWithholding { get; set; }


        [SubSonicIgnore]
        public double TotalWitholdings 
        {
            get
            {
                return this.FederalWithholding + this.SocialSecurityWithholding + this.MedicareWithholding;
            }
        }
    }
}
