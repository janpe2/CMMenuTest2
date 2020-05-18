using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class CategoryViewModel : Screen
    {
        public BindableCollection<Dish> DishesInCategory { get; } = new BindableCollection<Dish>();

        public List<string> CategoryNames { get; set; }

        public BindableCollection<Menu> Menus { get; } = new BindableCollection<Menu>();

        private string _selectedCategoryName = "Starter";
        public string SelectedCategoryName 
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
                _selectedCategoryId = (int)Enum.Parse(typeof(Menu.Category), value);
                LoadCategory();
            }
        }

        private int _selectedCategoryId;
        public int SelectedCategoryId
        {
            get
            {
                return _selectedCategoryId;
            }
            set
            {
                if (value != -1)
                {
                    _selectedCategoryId = value;
                    SelectedCategoryName = ((Menu.Category)value).ToString();
                    LoadCategory();
                }
            }
        }

        public string SelectedMenuDescription
        {
            get
            {
                return SelectedMenu.Description;
            }
            set
            {
                SelectedMenu.Description = value;
                NotifyOfPropertyChange(() => SelectedMenuDescription);
            }
        }

        private Menu _selectedMenu;
        public Menu SelectedMenu
        {
            get
            {
                return _selectedMenu;
            }
            set
            {
                if (value != null)
                {
                    _selectedMenu = value;
                    LoadCategory();
                    SelectedMenuDescription = value.Description;
                    NotifyOfPropertyChange(() => SelectedCategoryId);
                    NotifyOfPropertyChange(() => SelectedMenu);
                }
            }
        }

        public List<Dish> AllDishes { get; set; }

        private Dish _selectedDishInCategory;
        public Dish SelectedDishInCategory
        {
            get { return _selectedDishInCategory; }
            set
            {
                _selectedDishInCategory = value;
                NotifyOfPropertyChange(() => SelectedDishInCategory);
            }
        }

        public Dish SelectedDishInAllDishes { get; set; }

        private void LoadCategory()
        {
            DataAccess da = new DataAccess();
            List<Dish> dishes = da.GetDishesInCategory(SelectedCategoryId, SelectedMenu.Id);

            DishesInCategory.Clear();
            DishesInCategory.AddRange(dishes);

            if (DishesInCategory.Count == 0)
            {
                SelectedDishInCategory = null;
            }
            else
            {
                SelectedDishInCategory = DishesInCategory[0];
            }

            NotifyOfPropertyChange(() => DishesInCategory);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manager">menu manager</param>
        public CategoryViewModel()
        {
            DataAccess da = new DataAccess();
            List<Menu> allMenus = da.GetAllMenus();
            SelectedMenu = allMenus[0];

            CategoryNames = new List<string>();
            CategoryNames.AddRange(Enum.GetNames(typeof(Menu.Category)));

            Menus.AddRange(allMenus); 
            AllDishes = da.GetAllDishes();
        }

        public void AddDishToCategory()
        {
            if (SelectedCategoryId == -1)
            {
                MessageBox.Show("No category has been selected");
                return;
            }
            if (SelectedDishInAllDishes == null)
            {
                MessageBox.Show("No dish has been selected");
                return;
            }

            Dish dish = SelectedDishInAllDishes;
            if (DishesInCategory.Contains(dish))
            {
                MessageBox.Show($"The dish {dish.Name} is already in category {SelectedCategoryId}");
                return;
            }

            DataAccess da = new DataAccess();
            da.AddDishToCategory(dish.Id, SelectedCategoryId, SelectedMenu.Id);

            DishesInCategory.Add(dish);
            SelectedDishInCategory = dish;
            NotifyOfPropertyChange(() => DishesInCategory);
        }

        public void RemoveDishFromCategory()
        {
            if (SelectedDishInCategory == null)
            {
                MessageBox.Show("No dish has been selected in category");
                return;
            }

            Dish dish = SelectedDishInCategory;

            DataAccess da = new DataAccess();
            da.RemoveDishFromCategory(dish.Id, SelectedCategoryId, SelectedMenu.Id);

            DishesInCategory.Remove(dish);
            SelectedDishInCategory = null;
            NotifyOfPropertyChange(() => DishesInCategory);
        }
        public void AddMenu()
        {
            string name = AskMenuNameOrDescr("New Menu", "Name for new menu:", "");
            if (name == null)
            {
                return;
            }

            Menu menu = new Menu(name, "");

            DataAccess da = new DataAccess();
            da.AddMenu(menu);
            menu.Id = GetMaxMenuIdFromDB(da);

            Menus.Add(menu);
            NotifyOfPropertyChange(() => Menus);
            SelectedMenu = menu;
        }

        private int GetIndexOfMenuById(int menuId)
        {
            int i = 0;
            foreach (var m in Menus)
            {
                if (m.Id == menuId)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public void DeleteMenu()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Are you sure you want delete menu {SelectedMenu.Name}?", "Delete Menu",
                MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //int index = AllMenus.IndexOf(SelectedMenu);
                int index = GetIndexOfMenuById(SelectedMenu.Id);
                Menu newSelection;

                if (index == -1)
                {
                    MessageBox.Show("Menu does not exist.");
                    return;
                }
                else if (Menus.Count == 1)
                {
                    // TODO
                    return; // newSelection = null;
                }
                else if (index == 0)
                {
                    newSelection = Menus[1];
                }
                else
                {
                    newSelection = Menus[index - 1];
                }

                Menus.RemoveAt(index);

                DataAccess da = new DataAccess();
                da.DeleteMenu(SelectedMenu);

                Menus.Clear();
                Menus.AddRange(da.GetAllMenus());
                NotifyOfPropertyChange(() => Menus);
                SelectedMenu = newSelection;
            }
        }

        private string AskMenuNameOrDescr(string title, string message, string oldName)
        {
            var viewModel = IoC.Get<InputBoxViewModel>();
            viewModel.DialogTitle = title;
            viewModel.MessageText = message;
            viewModel.InputText = oldName;
            new WindowManager().ShowDialogAsync(viewModel);

            if (!viewModel.DialogResult || string.IsNullOrWhiteSpace(viewModel.InputText) ||
                viewModel.InputText == oldName)
            {
                return null;
            }
            return viewModel.InputText;
        }

        public void RenameMenu()
        {
            if (SelectedMenu == null)
            {
                return;
            }

            string newName = AskMenuNameOrDescr("Rename Menu", "Rename menu to:", SelectedMenu.Name);
            if (newName == null)
            {
                return;
            }

            new DataAccess().ModifyMenu(SelectedMenu, newName, SelectedMenu.Description);
            SelectedMenu.Name = newName;

            // Reload the list to force items update in ComboBox
            List<Menu> tempList = new List<Menu>(Menus);
            Menus.Clear();
            Menus.AddRange(tempList);

            // TODO Selected item disappears in combo box
            NotifyOfPropertyChange(() => Menus);
            NotifyOfPropertyChange(() => SelectedMenu);
        }

        private int GetMaxMenuIdFromDB(DataAccess da)
        {
            List<Menu> menus = da.GetAllMenus();
            int id = 1;
            foreach (var d in menus)
            {
                if (d.Id > id)
                {
                    id = d.Id;
                }
            }
            return id;
        }

        public void SetMenuDescription()
        {
            if (SelectedMenu == null)
            {
                return;
            }

            string newDescr = AskMenuNameOrDescr("Menu Description", "Description:", SelectedMenu.Description);
            if (newDescr == null)
            {
                return;
            }

            new DataAccess().ModifyMenu(SelectedMenu, SelectedMenu.Name, newDescr);
            SelectedMenu.Description = newDescr;
            NotifyOfPropertyChange(() => SelectedMenuDescription);
        }

    }
}
