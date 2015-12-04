using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace LabourDivider.UserControls
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        private CollectionView mHoursFrom;
        public CollectionView HoursFrom
        {
            get
            {
                return mHoursFrom;
            }
            set
            {
                mHoursFrom = value;
            }
        }

        private CollectionView mMinutesFrom;
        public CollectionView MinutesFrom
        {
            get
            {
                return mMinutesFrom;
            }
            set
            {
                mMinutesFrom = value;
            }
        }

        private CollectionView mHoursTo;
        public CollectionView HoursTo
        {
            get
            {
                return mHoursTo;
            }
            set
            {
                mHoursTo = value;
            }
        }

        private CollectionView mMinutesTo;
        public CollectionView MinutesTo
        {
            get
            {
                return mMinutesTo;
            }
            set
            {
                mMinutesTo = value;
            }
        }

        public TimePicker()
        {
            InitializeComponent();

            var List = new ObservableCollection<string>();
            for (int i = 0; i < 24; i++)
            {
                List.Add(i.ToString());
            }
            HoursFrom = new CollectionView(List);
            HoursTo = new CollectionView(List);

            var List2 = new ObservableCollection<string>();
            List2.Add("00"); List2.Add("05");
            for (int i = 10; i < 60; i = i + 5)
            {
                List2.Add(i.ToString());
            }
            MinutesFrom = new CollectionView(List2);
            MinutesTo = new CollectionView(List2);

            LayoutRoot.DataContext = this;
        }

        public string HourFrom
        {
            get { return (string)GetValue(HourFromProperty); }
            set { SetValue(HourFromProperty, value); RaisePropertyChangedEvent("HourFrom"); }
        }

        public static readonly DependencyProperty HourFromProperty = DependencyProperty.Register("HourFrom", typeof(string), typeof(TimePicker),
            new PropertyMetadata(""));

        public string HourTo
        {
            get { return (string)GetValue(HourToProperty); }
            set { SetValue(HourToProperty, value); RaisePropertyChangedEvent("HourTo"); }
        }

        public static readonly DependencyProperty HourToProperty = DependencyProperty.Register("HourTo", typeof(string), typeof(TimePicker),
            new PropertyMetadata(""));

        public string MinuteFrom
        {
            get { return (string)GetValue(MinuteFromProperty); }
            set { SetValue(MinuteFromProperty, value); RaisePropertyChangedEvent("MinuteFrom"); }
        }

        public static readonly DependencyProperty MinuteFromProperty = DependencyProperty.Register("MinuteFrom", typeof(string), typeof(TimePicker),
            new PropertyMetadata(""));

        public string MinuteTo
        {
            get { return (string)GetValue(MinuteToProperty); }
            set { SetValue(MinuteToProperty, value); RaisePropertyChangedEvent("MinuteTo"); }
        }

        public static readonly DependencyProperty MinuteToProperty = DependencyProperty.Register("MinuteTo", typeof(string), typeof(TimePicker),
            new PropertyMetadata(""));

        //BINDING
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
