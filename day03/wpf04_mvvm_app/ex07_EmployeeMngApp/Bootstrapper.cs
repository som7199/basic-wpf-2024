using Caliburn.Micro;
using ex07_EmployeeMngApp.ViewModels;
using System.Windows;

namespace ex07_EmployeeMngApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(sender, e);
            DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
