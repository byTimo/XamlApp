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

namespace ZappChat.Controls
{
	/// <summary>
	/// Логика взаимодействия для QueryControl.xaml
	/// </summary>
	public partial class QueryControl : UserControl
	{
	    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (QueryControl));
	    public string Text
	    {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
	    }

	    public static readonly DependencyProperty IsVinProperty = DependencyProperty.Register("IsVin", typeof (bool),
	        typeof (QueryControl), new FrameworkPropertyMetadata(false));
	    public bool IsVin
	    {
            get { return (bool)GetValue(IsVinProperty); }
	        set
	        {
	            SetValue(IsVinProperty,value);
	            VinBorder.Width = value ? 40 : 0;
	        }
	    }

	    public static readonly DependencyProperty IsYearProperty = DependencyProperty.Register("IsYear", typeof (bool),
	        typeof (QueryControl), new FrameworkPropertyMetadata(false));

	    public bool IsYear
	    {
            get { return (bool)GetValue(IsYearProperty); }
	        set
	        {
	            SetValue(IsYearProperty, value);
	            YearBorder.Width = value ? 40 : 0;
	        }
	    }
        //TODO Время, прошедшее с момента прихода!
		public QueryControl()
		{
			InitializeComponent();
		}
	}
}