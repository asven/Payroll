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
            SelectedEmployee = this.AllEmployees.Where(ae => ae.IsDefaultEmployee).First();
        }


        public Employee SelectedEmployee { get; set; }

        public List<Employee> AllEmployees { get; set; }

        private double hoursWorked;
        public double HoursWorked 
        {
            get
            {
                return hoursWorked;
            }
            set
            {
                hoursWorked = value;
                RaisePropertyChanged("HoursWorked");
            }
        }

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
                RaisePropertyChanged("PayInfoVisibility");
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
        }

        internal void SavePayPeriod()
        {
            repo.Add<PayPeriod>(this.PayPeriod);

            this.PayPeriod = null;

            this.HoursWorked = 0;
        }
    }
}
