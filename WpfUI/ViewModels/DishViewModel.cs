using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class DishViewModel : Screen
    {
        public DishModel TheDish { get; set; } 

        public string NameOfSelectedDish 
        { 
            get { return TheDish.Name; }
        }

        public string PriceOfSelectedDish
        {
            get { return $"{TheDish.Price} euroa"; }
        }

        public string DescriptionOfSelectedDish
        {
            get { return TheDish.Description; }
        }

        public DishViewModel(DishModel d)
        {
            TheDish = d;
        }
    }
}
