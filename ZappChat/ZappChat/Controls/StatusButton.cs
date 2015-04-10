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
using ZappChat.Core;

namespace ZappChat.Controls
{
    public class StatusButton : Button
    {
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof (AppStatus), typeof (StatusButton),
            new FrameworkPropertyMetadata(AppStatus.Disconnect));
        public AppStatus Status
        {
            get { return (AppStatus)GetValue(StatusProperty); }
            set
            {
                SetValue(StatusProperty, value);
                ChangeStatusOnButton(Status);
            }
        }
        public static readonly DependencyProperty IndicaterColorProperty =
            DependencyProperty.Register("IndicaterColor", typeof(SolidColorBrush), typeof(StatusButton));

        public SolidColorBrush IndicaterColor
        {
            get { return (SolidColorBrush)GetValue(IndicaterColorProperty); }
            set { SetValue(IndicaterColorProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof (string), typeof (StatusButton));

        public string StatusText
        {
            get { return GetValue(StatusTextProperty) as string; }
            set { SetValue(StatusTextProperty, value); }
        }
        static StatusButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusButton), new FrameworkPropertyMetadata(typeof(StatusButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Status = AppStatus.Disconnect;
        }

        private void ChangeStatusOnButton(AppStatus nowStatus)
        {
            switch (nowStatus)
            {
                case AppStatus.Connect:
                    IndicaterColor = Brushes.Green;
                    StatusText = "В сети";
                    break;
                case AppStatus.Disconnect:
                    IndicaterColor = Brushes.Red;
                    StatusText = "Не в сети";
                    break;
            }
        }
    }
}
