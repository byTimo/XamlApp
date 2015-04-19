using System.Windows;
using System.Windows.Controls;
using ZappChat.Core;

namespace ZappChat.Controls
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

        public void Flip()
        {
            IsFliped = !IsFliped;
        }
        private void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, IsFliped ? "BackView" : "FrontView", true);
        }
    }
}
