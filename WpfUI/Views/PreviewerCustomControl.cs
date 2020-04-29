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


        static PreviewerCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewerCustomControl), 
                new FrameworkPropertyMetadata(typeof(PreviewerCustomControl)));
        }
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth, height = ActualHeight;
            const double widthA4 = 595, heightA4 = 842;
            double scaleX = width / widthA4;
            double scaleY = height / heightA4;
            double scale = Math.Min(scaleX, scaleY);
            //double scaledWidth = scale * widthA4;
            //double scaledHeight = scale * heightA4;
            double translateX = (width - scale * widthA4) / 2;
            double translateY = (height - scale * heightA4) / 2;

            dc.PushTransform(new MatrixTransform(scale, 0, 0, scale, translateX, translateY));

            dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 2.0), 
                new Rect(0, 0, widthA4, heightA4));

            var text = new FormattedText("Menu",
                    cultureInfo,
                    FlowDirection.LeftToRight, 
                    new Typeface(new FontFamily("Times New Roman"), FontStyles.Italic, FontWeights.Bold, FontStretches.Normal), 
                    35.0, Brushes.Maroon, 1.0);

            dc.DrawText(text, new Point(200.0, 80.0));


            dc.Pop(); // pop transform
        }
        
    }
}
