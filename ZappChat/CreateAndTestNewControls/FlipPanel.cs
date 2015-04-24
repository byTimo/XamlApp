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
    [TemplateVisualState(Name = "FrontView", GroupName = "ViewContent"),
    TemplateVisualState(Name = "BackView", GroupName = "ViewContent")]
    public class FlipPanel : Control
    {
        static FlipPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipPanel), new FrameworkPropertyMetadata(typeof(FlipPanel)));
        }

        public static readonly DependencyProperty FrontContentProperty =
            DependencyProperty.Register("FrontContent", typeof (object), typeof (FlipPanel));

        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register("BackContent", typeof (object), typeof (FlipPanel));

        public static readonly DependencyProperty IsFlipedProperty =
            DependencyProperty.Register("IsFliped", typeof (bool), typeof (FlipPanel),
                new FrameworkPropertyMetadata(false));

        public object FrontContent
        {
            get { return GetValue(FrontContentProperty); }
            set { SetValue(FrontContentProperty, value); }
        }
        public object BackContent
        {
            get { return GetValue(BackContentProperty); }
            set { SetValue(BackContentProperty, value); }
        }
        public bool IsFliped
        {
            get { return (bool)GetValue(IsFlipedProperty); }
            set
            {
                SetValue(IsFlipedProperty, value);
                ChangeVisualState();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, "FrontContent", false);
        }

        private void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, IsFliped ? "BackView" : "FrontView", true);
        }
    }
}
