using System.Windows;
using System.Windows.Controls;
using ZappChat.Core;

namespace ZappChat.Controls
{
    [TemplateVisualState(Name = "Normal", GroupName = "ViewStates"),
     TemplateVisualState(Name = "DeleteDialog", GroupName = "ViewStates"),
     TemplatePart(Name = "Yes", Type = typeof(Button)),
     TemplatePart(Name = "No", Type = typeof(Button))]
    public class BlueTopMenu : ContentControl
    {
        public Dialogue DeleteDialogue { get; set; }

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
            AppEventManager.DeleteConfirmationDialogue += (s, e) =>
            {
                DeleteDialogue = e.DeletedDialogue;
                SwapState();
            };
            var yesButton = GetTemplateChild("Yes") as Button;
            yesButton.Click += (s, e) =>
            {
                if(DeleteDialogue != null)
                    AppEventManager.DeleteDialogueEvent(this, DeleteDialogue, true);
                SwapState();
                DeleteDialogue = null;
            };
            var noButton = GetTemplateChild("No") as Button;
            noButton.Click += (s, e) =>
            {
                if (DeleteDialogue != null)
                    AppEventManager.DeleteDialogueEvent(this, DeleteDialogue, false);
                SwapState();
                DeleteDialogue = null;
            };
        }

        public void SwapState()
        {
            IsDeleteDialog = !IsDeleteDialog;
        }
        private void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, IsDeleteDialog ? "DeleteDialog" : "Normal", true);
        }
    }
}
