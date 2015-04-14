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

	    public static readonly DependencyProperty VinProperty = DependencyProperty.Register("Vin", typeof (string), typeof (QueryControl));
	    public string Vin
	    {
            get { return GetValue(VinProperty) as string; }
            set { SetValue(VinProperty, value); }
	    }

	    public static readonly DependencyProperty YearProperty = DependencyProperty.Register("Year", typeof (string),
	        typeof (QueryControl));

	    public string Year
	    {
            get { return GetValue(YearProperty) as string; }
            set { SetValue(YearProperty, value); }
	    }
        //TODO Время, прошедшее с момента прихода!
		public QueryControl()
		{
			InitializeComponent();
		}
	}
}