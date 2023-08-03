using MI24_TheScriptApp.API;
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
using System.Windows.Shapes;

namespace MI24_TheScriptApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Start the local API on application startup.
            var localApi = new LocalAPI();
            await localApi.StartAsync();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Your application startup logic goes here.
            MainWin mainWindow = new MainWin();
            mainWindow.Show();
        }

    }
}
