using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public void LoadDish()
        {
            MessageBox.Show("Abc");
        }
    }
}
