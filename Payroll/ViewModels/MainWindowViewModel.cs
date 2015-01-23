using GalaSoft.MvvmLight;
using Payroll.Common;
using SubSonic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Payroll.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private SimpleRepository repo = new SimpleRepository("Payroll", SimpleRepositoryOptions.RunMigrations);

        public MainWindowViewModel()
        {
            this.AllEmployees = repo.All<Employee>().ToList();
        }


        public Employee SelectedEmployee { get; set; }

        public List<Employee> AllEmployees { get; set; }

        public double HoursWorked { get; set; }

        private PayPeriod payPeriod;
        public PayPeriod PayPeriod
        {
            get
            {
                return payPeriod;
            }
            set
            {
                payPeriod = value;
                RaisePropertyChanged("PayPeriod");
                
            }
        }

        public Visibility PayInfoVisibility
        {
            get
            {
                if(this.PayPeriod != null)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public void ComputePay()
        {
            PayPeriod payPeriod = new PayPeriod();
            payPeriod.GrossAmount = HoursWorked * SelectedEmployee.HourlyRate;
            payPeriod.MedicareWithholding = payPeriod.GrossAmount * Constants.MedicarePercentage;
            payPeriod.SocialSecurityWithholding = payPeriod.GrossAmount * Constants.SocialSecurityPercentage;

            var taxBracket = repo.All<FederalWithholdingTaxBracket>().Where(fwtb => fwtb.MoreThan < payPeriod.GrossAmount && fwtb.LessThan > payPeriod.GrossAmount && fwtb.IsMarried == SelectedEmployee.IsMarried).First();

            payPeriod.FederalWithholding = ((payPeriod.GrossAmount - taxBracket.ExcessOver) * taxBracket.Percentage) + taxBracket.AmountToAdd;

            payPeriod.NetAmount = payPeriod.GrossAmount - payPeriod.MedicareWithholding - payPeriod.SocialSecurityWithholding - payPeriod.FederalWithholding;

            payPeriod.Date = DateTime.Now;
            payPeriod.EmployeeID = SelectedEmployee.EmployeeID;
            payPeriod.Hours = this.HoursWorked;

            this.PayPeriod = payPeriod;
            RaisePropertyChanged("PayInfoVisibility");
        }

        internal void SavePayPeriod()
        {
            repo.Add<PayPeriod>(this.PayPeriod);
        }
    }
}
