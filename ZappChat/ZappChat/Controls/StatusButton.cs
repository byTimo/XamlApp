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
            DependencyProperty.Register("Status", typeof (ConnectionStatus), typeof (StatusButton),
            new FrameworkPropertyMetadata(ConnectionStatus.Disconnect));
        public ConnectionStatus Status
        {
            get { return (ConnectionStatus)GetValue(StatusProperty); }
            private set{ SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty IndicaterColorProperty =
            DependencyProperty.Register("IndicaterColor", typeof(LinearGradientBrush), typeof(StatusButton));

        public LinearGradientBrush IndicaterColor
        {
            get { return (LinearGradientBrush)GetValue(IndicaterColorProperty); }
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
            Status = ConnectionStatus.Disconnect;
        }

        public void ChangeStatusOnButton(ConnectionStatus nowStatus)
        {
            switch (nowStatus)
            {
                case ConnectionStatus.Connect:
                    IndicaterColor = new LinearGradientBrush(new GradientStopCollection{
                        new GradientStop(Color.FromArgb(255,173,228,95),0),
                        new GradientStop(Color.FromArgb(255,155,196,60),100.0)
                    });
                    StatusText = "В сети";
                    Status = ConnectionStatus.Connect;
                    break;
                case ConnectionStatus.Disconnect:
                    IndicaterColor = new LinearGradientBrush(new GradientStopCollection{
                        new GradientStop(Color.FromArgb(255,228,63,60),0),
                        new GradientStop(Color.FromArgb(255,225,95,95),1.0)
                    });
                    StatusText = "Не в сети";
                    Status = ConnectionStatus.Disconnect;
                    break;
            }
        }
    }
}
