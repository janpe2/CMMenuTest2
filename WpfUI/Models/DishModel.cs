using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;

namespace WpfUI.Models
{
    public class DishModel
    {
        private BindableCollection<Dish> dishes = new BindableCollection<Dish>();

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public DishModel(List<Dish> d)
        {
            dishes.AddRange(d);
        }
    }
}
