﻿using Payroll.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Payroll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainWindowViewModel;
            }

        }


        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
            this.ViewModel.PayPeriod = new Common.PayPeriod();
            this.ViewModel.PayPeriod.ID = 1015;


            this.ViewModel.GeneratePayStub();
        }

        private void ComputePayButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ComputePay();
        }

        private void SavePayInfoButton_Click(object sender, RoutedEventArgs e)
        {
            string resultText = ViewModel.SavePayPeriod();

            MessageBox.Show(resultText);
        }
    }
}
