using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;

namespace WpfUI.Views
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfUI.Views"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfUI.Views;assembly=WpfUI.Views"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:PreviewerCustomControl/>
    ///
    /// </summary>
    public class PreviewerCustomControl : Control
    {
        private System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

        // Create a bindable property, so the name MenuId can be used as a property name in XAML and it can be bound.
        public static readonly DependencyProperty MenuIdProperty =
            DependencyProperty.Register(
                "MenuId", typeof(int), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(int), // default value is required
                    FrameworkPropertyMetadataOptions.AffectsRender, // automatic repaint
                    new PropertyChangedCallback(MenuIdPropertyChanged))
                );

        public int MenuId
        {
            get 
            { 
                return (int)GetValue(MenuIdProperty); 
            }
            set 
            { 
                SetValue(MenuIdProperty, value);
            }
        }

        private void LoadMenu()
        {
            try
            {
                DataAccess da = new DataAccess();
                currentMenu = da.GetMenuById(MenuId);
                dishes = da.GetDishesInCategory(0, MenuId);
            }
            catch (Exception)
            {

            }
        }

        private WpfUI.MenuLibrary.Menu currentMenu;
        private List<Dish> dishes;

        static void MenuIdPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PreviewerCustomControl control = sender as PreviewerCustomControl;
            if (control != null)
            {
                control.LoadMenu();
            }
        }

        static PreviewerCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewerCustomControl), 
                new FrameworkPropertyMetadata(typeof(PreviewerCustomControl)));
        }

        private void DrawText(DrawingContext dc, string text, Typeface font, double fontSize,
            Brush color, double x, double y)
        {
            var formattedText = new FormattedText(text,
                    cultureInfo, FlowDirection.LeftToRight, font, fontSize, color, 1.0);
            dc.DrawText(formattedText, new Point(x, y));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth, height = ActualHeight;
            const double widthA4 = 595, heightA4 = 842;
            double scaleX = width / widthA4;
            double scaleY = height / heightA4;
            double scale = Math.Min(scaleX, scaleY);
            double translateX = (width - scale * widthA4) / 2;
            double translateY = (height - scale * heightA4) / 2;

            FontFamily fontFamily = new FontFamily("Times New Roman");
            Typeface largeFont = new Typeface(fontFamily, FontStyles.Italic, FontWeights.Bold, FontStretches.Normal);
            Typeface textFont = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            Typeface descrFont = new Typeface(fontFamily, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);

            dc.PushTransform(new MatrixTransform(scale, 0, 0, scale, translateX, translateY));

            dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 2.0), new Rect(0, 0, widthA4, heightA4));

            if (currentMenu == null)
            {
                DrawText(dc, $"{MenuId}", largeFont, 32.0, Brushes.Maroon, 100.0, 80.0);
            }
            else
            {
                DrawText(dc, $"{currentMenu.Name} (Id {currentMenu.Id})", largeFont, 32.0, Brushes.Maroon, 50.0, 80.0);
                DrawText(dc, currentMenu.Description, textFont, 15.0, Brushes.Black, 50.0, 120.0);
            }

            if (dishes != null && dishes.Count > 0)
            {
                double y = 150;
                foreach (var dish in dishes)
                {
                    DrawText(dc, $"{dish.Name}   {dish.Price} €", textFont, 16.0, Brushes.Black, 60.0, y);
                    DrawText(dc, dish.Description, descrFont, 14.0, Brushes.Gray, 60.0, y + 20);
                    y += 40;
                }
            }


            dc.Pop(); // pop transform
        }
        
    }
}
