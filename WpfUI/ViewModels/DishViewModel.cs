using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class DishViewModel : Screen
    {
        public MenuManager TheMenuManager { get; set; }

        private BindableCollection<Dish> _dishes = new BindableCollection<Dish>();
        public BindableCollection<Dish> Dishes 
        { 
            get
            {
                return _dishes;
            }
        }

        private Dish _selectedDish;
        public Dish SelectedDish 
        { 
            get
            {
                return _selectedDish;
            }
            set
            {
                _selectedDish = value;
                NotifyOfPropertyChange(() => NameOfSelectedDish);
                NotifyOfPropertyChange(() => PriceOfSelectedDish);
                NotifyOfPropertyChange(() => DescriptionOfSelectedDish);
            }
        }

        public string NameOfSelectedDish
        {
            get { return SelectedDish.Name; }
        }

        public string PriceOfSelectedDish
        {
            get { return $"{SelectedDish.Price} euroa"; }
        }

        public string DescriptionOfSelectedDish
        {
            get { return SelectedDish.Description; }
        }

        public DishViewModel(MenuManager m)
        {
            TheMenuManager = m;
            _dishes.AddRange(m.AllDishes);
            if (m.AllDishes.Count > 0)
            {
                SelectedDish = m.AllDishes[0];
            }
        }
    }
}
