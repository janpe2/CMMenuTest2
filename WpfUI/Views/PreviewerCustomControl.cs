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
using WpfUI.MenuLibrary.Graphics;

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
        // Create a bindable property, so the name MenuId can be used as a property name in XAML and it can be bound.
        public static readonly DependencyProperty MenuIdProperty =
            DependencyProperty.Register(
                "MenuId", typeof(int), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(int), // default value is required
                    FrameworkPropertyMetadataOptions.AffectsRender, // automatic repaint
                    new PropertyChangedCallback(MenuIdPropertyChanged))
                );

        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.Register(
                "ShowBorder", typeof(bool), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public static readonly DependencyProperty ShowOrnamentsProperty =
            DependencyProperty.Register(
                "ShowOrnaments", typeof(bool), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(bool),
                    FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public static readonly DependencyProperty ThemeColorProperty =
            DependencyProperty.Register(
                "ThemeColor", typeof(Color), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(Color),
                    FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public static readonly DependencyProperty MenuBackgroundColorProperty =
            DependencyProperty.Register(
                "MenuBackgroundColor", typeof(Color), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(Color),
                    FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public static readonly DependencyProperty MenuFontFamilyProperty =
            DependencyProperty.Register(
                "MenuFontFamily", typeof(FontFamily), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(FontFamily),
                    FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public static readonly DependencyProperty CurrentPageIndexProperty =
            DependencyProperty.Register(
                "CurrentPageIndex", typeof(int), typeof(PreviewerCustomControl),
                new FrameworkPropertyMetadata(
                    default(int), 
                    FrameworkPropertyMetadataOptions.AffectsRender, 
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

        public int CurrentPageIndex
        {
            get 
            { 
                return (int)GetValue(CurrentPageIndexProperty); 
            }
            set 
            { 
                SetValue(CurrentPageIndexProperty, value);
            }
        }

        public bool ShowBorder
        {
            get
            {
                return (bool)GetValue(ShowBorderProperty);
            }
            set
            {
                SetValue(ShowBorderProperty, value);
            }
        }

        public bool ShowOrnaments
        {
            get
            {
                return (bool)GetValue(ShowOrnamentsProperty);
            }
            set
            {
                SetValue(ShowOrnamentsProperty, value);
            }
        }

        public Color ThemeColor
        {
            get
            {
                return (Color)GetValue(ThemeColorProperty);
            }
            set
            {
                SetValue(ThemeColorProperty, value);
            }
        }

        public Color MenuBackgroundColor
        {
            get
            {
                return (Color)GetValue(MenuBackgroundColorProperty);
            }
            set
            {
                SetValue(MenuBackgroundColorProperty, value);
            }
        }

        public FontFamily MenuFontFamily
        {
            get
            {
                return (FontFamily)GetValue(MenuFontFamilyProperty);
            }
            set
            {
                SetValue(MenuFontFamilyProperty, value);
            }
        }

        private MenuGraphicsCreator graphicsCreator = new MenuGraphicsCreator();

        private void LoadMenu()
        {
            graphicsCreator.LoadMenu(MenuId);
        }

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

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth;
            double height = ActualHeight;
            double pageWidth = MenuGraphicsCreator.PageWidth;
            double pageHeight = MenuGraphicsCreator.PageHeight;
            double scaleX = width / pageWidth;
            double scaleY = height / pageHeight;
            double scale = Math.Min(scaleX, scaleY);
            double translateX = (width - scale * pageWidth) / 2;
            double translateY = (height - scale * pageHeight) / 2;

            dc.PushTransform(new MatrixTransform(scale, 0, 0, scale, translateX, translateY));

            dc.DrawRectangle(new SolidColorBrush(MenuBackgroundColor), new Pen(Brushes.Black, 1.0 / scale), 
                new Rect(0, 0, pageWidth, pageHeight));
                
            graphicsCreator.Start(new ScreenGraphicsContext(dc), ThemeColor, null, MenuFontFamily);
            graphicsCreator.DrawMenuPage(CurrentPageIndex, ShowBorder, ShowOrnaments);
            graphicsCreator.End();

            dc.Pop(); // pop transform
        }
        
    }
}
