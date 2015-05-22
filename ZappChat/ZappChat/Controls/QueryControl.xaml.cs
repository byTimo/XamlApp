using System;
using System.Collections.Generic;
using System.Globalization;
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
using ZappChat.Core;

namespace ZappChat.Controls
{
	/// <summary>
	/// Логика взаимодействия для QueryControl.xaml
	/// </summary>
	public partial class QueryControl : UserControl
	{
        public Dialogue Dialogue { get; private set; }
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

	    public static readonly DependencyProperty PastTimeProperty = DependencyProperty.Register("PastTime",
	        typeof (string), typeof (QueryControl));

	    public string PastTime
	    {
            get { return GetValue(PastTimeProperty) as string; }
            set { SetValue(PastTimeProperty, value); }
	    }

	    public static readonly DependencyProperty YearProperty = DependencyProperty.Register("Year", typeof (string),
	        typeof (QueryControl), new FrameworkPropertyMetadata(""));

	    public string Year
	    {
            get { return GetValue(YearProperty) as string; }
            set { SetValue(YearProperty, value); }
	    }

	    public string PastTimeCalculate(DateTime time)
	    {
            if (DateTime.Now.Year - time.Year != 0)
                return time.Year + " год";
            if (DateTime.Now.DayOfYear - time.DayOfYear != 0)
                return time.ToString("M", new CultureInfo("Ru-ru"));

            var deltaTime = DateTime.Now.Subtract(time);
            if (deltaTime.Hours != 0)
            {
                var number = deltaTime.Hours;
                var end = number % 20 == 1 ? " час" : number % 20 != 0 && number % 20 < 5 ? " часа" : " часов";
                return number + end + " назад";
            }
            if (deltaTime.Minutes != 0)
            {
                var number = deltaTime.Minutes;
                var end = (number == 1 || number > 11) && number%20 == 1
                    ? " минуту"
                    : number > 20 && number%20 != 0 && number%20 < 5 ? " минуты" : " минут";
                return number + end + " назад";
            }
            return "0 минут назад";
	    }
		public QueryControl()
		{
			InitializeComponent();
		}

	    public QueryControl(Dialogue dialogue) : this()
	    {
	        Dialogue = dialogue;
	        Text = dialogue.Query;
	        PastTime = PastTimeCalculate(dialogue.LastDateTime);
	        AppEventManager.UpdateCounter += () => PastTime = PastTimeCalculate(dialogue.LastDateTime);
	    }

	    public void SetCarInfoAdapter(string brand, string model, string vin, string year)
	    {
	        Dialogue.SetCarInformation(brand, model, vin, year);
	        if (vin != null) IsVin = true;
	        if (year != null)
	        {
	            IsYear = true;
	            Year = year;
	        }
	    }
	}
}