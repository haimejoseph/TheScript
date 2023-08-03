using MI24_TheScriptApp.Listener;
using MI24_TheScriptApp.Service;
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

namespace MI24_TheScriptApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        private string compareA;
        private string compareB;
        public MainWin()
        {
            InitializeComponent();
            btnCompareAStart.IsEnabled = true;
            btnCompareBStart.IsEnabled = false;

            btnCompareAStart.IsEnabled = true;
            btnCompareAStop.IsEnabled = false;
            btnCompareAClear.IsEnabled = false;

            btnCompareBStart.IsEnabled = false;
            btnCompareBStop.IsEnabled = false;
            btnCompareBClear.IsEnabled = false;
        }


        //CompareA-Start
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlListener.StartEventListener();
            btnCompareAStart.IsEnabled = false;
            btnCompareAStop.IsEnabled = true;
        }
       
        //CompareA-Stop
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            compareA = SqlListener.StopEventListener();
            btnCompareBStart.IsEnabled = true;
            btnCompareBStop.IsEnabled = false;
            btnCompareBClear.IsEnabled = false;
        }

        //CompareA-Clear
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            compareA = string.Empty;
            btnCompareAStart.IsEnabled = true;
            btnCompareAStop.IsEnabled = false;
            btnCompareAClear.IsEnabled = false;
        }


        //CompareB-Start
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SqlListener.StartEventListener();
        }


        //CompareB-Stop
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            compareB = SqlListener.StopEventListener();
            Processor.ProcessCompare(compareA, compareB);
        }

        //CompareB-Clear
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }
    }
}
