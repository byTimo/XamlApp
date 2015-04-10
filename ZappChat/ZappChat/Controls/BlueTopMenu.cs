﻿using System.Windows;
using System.Windows.Controls;

namespace ZappChat.Controls
{
    [TemplateVisualState(Name = "Normal", GroupName = "ViewStates"),
     TemplateVisualState(Name = "DeleteDialog", GroupName = "ViewStates")]
    public class BlueTopMenu : ContentControl
    {
        public static readonly DependencyProperty IsDeleteDialogProperty =
            DependencyProperty.Register("IsDeleteDialog", typeof (bool), typeof (BlueTopMenu),
                new FrameworkPropertyMetadata(false));
        static BlueTopMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BlueTopMenu),
                new FrameworkPropertyMetadata(typeof (BlueTopMenu)));
        }
        public bool IsDeleteDialog
        {
            get { return (bool) GetValue(IsDeleteDialogProperty); }
            set
            {
                SetValue(IsDeleteDialogProperty, value);
                ChangeVisualState();
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, IsDeleteDialog ? "DeleteDialog" : "Normal", true);
        }
    }
}
