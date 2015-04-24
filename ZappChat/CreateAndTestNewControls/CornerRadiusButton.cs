using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CreateAndTestNewControls
{
    public class CornerRadiusButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof (CornerRadius), typeof (CornerRadiusButton));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        static CornerRadiusButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CornerRadiusButton), new FrameworkPropertyMetadata(typeof(CornerRadiusButton)));
        }
    }
}
