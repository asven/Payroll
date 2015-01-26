﻿using GalaSoft.MvvmLight;
using Microsoft.Reporting.WinForms;
using Payroll.Common;
using SubSonic.Repository;
using SubSonic.Schema;
using System;
using System.Collections.Generic;
using System.IO;
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

        internal void GeneratePayStub()
        {
            LocalReport report = new LocalReport();
            report.ReportPath = @"..\..\Reports\Paystub.rdlc";

            StoredProcedure sp = new StoredProcedure("GetEmployeePayStub"); 
            sp.Command.AddParameter("@PayPeriodId", 5, System.Data.DbType.Int32);
            sp.Command.AddParameter("@EmployeeId", this.SelectedEmployee.EmployeeID.ToString(), System.Data.DbType.Int32);
            var dataset = sp.ExecuteDataSet();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            reportDataSource1.Name = "PayrollDataSet";
            reportDataSource1.Value = dataset.Tables[0];

            ReportParameter[] param = new ReportParameter[2];
            param[0] = new ReportParameter("PayPeriodId", "5", true);
            param[1] = new ReportParameter("EmployeeId", this.SelectedEmployee.EmployeeID.ToString(), true);

            report.DataSources.Add(reportDataSource1);

            byte[] bytes = report.Render("PDF");

            using (FileStream fs = new FileStream(@"C:\PayStubs\paystub" + DateTime.Now.Day +  DateTime.Now.Month + DateTime.Now.Year + ".pdf", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
