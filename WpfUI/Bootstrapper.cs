using Caliburn.Micro;
using System.Windows;
using WpfUI.ViewModels; 

namespace WpfUI
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>(); 
        }
    }
}
