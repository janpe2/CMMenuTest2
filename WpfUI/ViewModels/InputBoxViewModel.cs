using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.ViewModels
{
    public class InputBoxViewModel : Screen
    {
        public string MessageText { get; set; } = "Text:";

        public string InputText { get; set; } = "";

        public bool DialogResult { get; set; } = false;

        public string DialogTitle { get; set; } = "Input";

        public void Accept()
        {
            DialogResult = true;
            TryCloseAsync();
        }

        public void Cancel()
        {
            DialogResult = false;
            TryCloseAsync();
        }
    }
}
