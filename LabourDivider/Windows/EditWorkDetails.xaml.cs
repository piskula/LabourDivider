using LabourDivider.Entities;
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
using System.Windows.Shapes;

namespace LabourDivider.Windows
{
    /// <summary>
    /// Interaction logic for EditWorkDetails.xaml
    /// </summary>
    public partial class EditWorkDetails : Window, INotifyPropertyChanged
    {
        public const string PIC_YES = "../Resources/yes17.gif";
        public const string PIC_ERROR = "../Resources/nok17.gif";
        public const string TIME_ERROR = "Chybný čas";

        private Work mWorkToUpdate;
        public Work WorkToUpdate
        {
            get
            {
                return mWorkToUpdate;
            }
            set
            {
                mWorkToUpdate = value;
                RaisePropertyChangedEvent("WorkToUpdate");
            }
        }

        private string mTitle;
        public string Titlee
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                RaisePropertyChangedEvent("Titlee");
            }
        }

        private CollectionView mJobTypes1;
        public CollectionView JobTypes1
        {
            get
            {
                return mJobTypes1;
            }
            set
            {
                mJobTypes1 = value;
                RaisePropertyChangedEvent("JobTypes1");
            }
        }

        private int _Job1Id;
        public int Job1Id
        {
            get { return _Job1Id; }
            set
            {
                _Job1Id = value;
                RaisePropertyChangedEvent("Job1Id");
            }
        }

        private string mHourFrom1;
        public string HourFrom1
        {
            get
            {
                return mHourFrom1;
            }
            set
            {
                mHourFrom1 = value;
                IsOkDay1 = ValidateTime(HourFrom1, MinuteFrom1, HourTo1, MinuteTo1);
                RaisePropertyChangedEvent("HourFrom1");
            }
        }

        private string mMinuteFrom1;
        public string MinuteFrom1
        {
            get
            {
                return mMinuteFrom1;
            }
            set
            {
                mMinuteFrom1 = value;
                IsOkDay1 = ValidateTime(HourFrom1, MinuteFrom1, HourTo1, MinuteTo1);
                RaisePropertyChangedEvent("MinuteFrom1");
            }
        }

        private string mHourTo1;
        public string HourTo1
        {
            get
            {
                return mHourTo1;
            }
            set
            {
                mHourTo1 = value;
                IsOkDay1 = ValidateTime(HourFrom1, MinuteFrom1, HourTo1, MinuteTo1);
                RaisePropertyChangedEvent("HourTo1");
            }
        }

        private string mMinuteTo1;
        public string MinuteTo1
        {
            get
            {
                return mMinuteTo1;
            }
            set
            {
                mMinuteTo1 = value;
                IsOkDay1 = ValidateTime(HourFrom1, MinuteFrom1, HourTo1, MinuteTo1);
                RaisePropertyChangedEvent("MinuteTo1");
            }
        }

        private string mIsOkDay1;
        public string IsOkDay1
        {
            get
            {
                return mIsOkDay1;
            }
            set
            {
                mIsOkDay1 = value;
                RaisePropertyChangedEvent("IsOkDay1");
                DayPic1 = ValidateRow(Job1Id, IsOkDay1);
            }
        }

        private ObservableCollection<Job> mJobs;
        public ObservableCollection<Job> Jobs
        {
            get
            {
                return mJobs;
            }
            set
            {
                mJobs = value;
                RaisePropertyChangedEvent("Jobs");
            }
        }

        private string mDayPic1;
        public string DayPic1
        {
            get
            {
                return mDayPic1;
            }
            set
            {
                mDayPic1 = value;
                RaisePropertyChangedEvent("DayPic1");
            }
        }

        public EditWorkDetails(Work workToUpdate)
        {
            InitializeComponent();
            this.DataContext = this;
            WorkToUpdate = workToUpdate;
            Titlee = WorkToUpdate.From.ToString("dd.MM.yyyy") + " " + WorkToUpdate.Employee.FirstName + " " + WorkToUpdate.Employee.LastName;
            using (var db = new ModelContainer())
            {
                Jobs = new ObservableCollection<Job>(db.Jobs);
            }

            var List2 = new ObservableCollection<Job>();
            foreach (var current in Jobs.OrderBy(e => e.Name))
            {
                List2.Add(new JobEntity(current));
            }
            JobTypes1 = new CollectionView(List2);

            HourFrom1 = workToUpdate.From.Hour.ToString();
            int help = workToUpdate.From.Minute;
            if (help == 0)
            {
                MinuteFrom1 = "00";
            }
            else if (help == 5)
            {
                MinuteFrom1 = "05";
            }
            else
            {
                MinuteFrom1 = help.ToString();
            }

            HourTo1 = workToUpdate.To.Hour.ToString();
            help = workToUpdate.To.Minute;
            if (help == 0)
            {
                MinuteTo1 = "00";
            }
            else if (help == 5)
            {
                MinuteTo1 = "05";
            }
            else
            {
                MinuteTo1 = help.ToString();
            }

            Job1Id = workToUpdate.JobId;
            DayPic1 = ValidateRow(Job1Id, IsOkDay1);
        }

        private string ValidateRow(int JobId, string IsOkDayX)
        {

            if (IsOkDayX == TIME_ERROR)
            {
                return PIC_ERROR;
            }
            else
            {
                if (JobId == 0)
                {
                    return PIC_ERROR;
                }
                else
                {
                    return PIC_YES;
                }

            }

        }

        private string ValidateTime(string HourFrom, string MinuteFrom, string HourTo, string MinuteTo)
        {
            if (HourFrom == null || MinuteFrom == null || HourTo == null || MinuteTo == null)
            {
                return "";
            }
            if (HourFrom == "" || MinuteFrom == "" || HourTo == "" || MinuteTo == "")
            {
                return "";
            }
            var dateFrom = new DateTime(2011, 1, 1, Convert.ToInt32(HourFrom), Convert.ToInt32(MinuteFrom), 0);
            var dateTo = new DateTime(2011, 1, 1, Convert.ToInt32(HourTo), Convert.ToInt32(MinuteTo), 0);

            if (dateFrom <= dateTo)
            {
                return "";
            }
            return TIME_ERROR;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (!(Job1Id == WorkToUpdate.JobId
                && HourFrom1.Equals(WorkToUpdate.From.Hour)
                && MinuteFrom1.Equals(WorkToUpdate.From.Minute)
                && HourTo1.Equals(WorkToUpdate.To.Hour)
                && MinuteTo1.Equals(WorkToUpdate.To.Minute)))
            {
                if (DayPic1.Equals(PIC_YES))
                {
                    Work myWork = new Work();
                    myWork.EmployeeId = WorkToUpdate.EmployeeId;
                    myWork.JobId = Job1Id;
                    var DateFrom = new DateTime(WorkToUpdate.From.Year, WorkToUpdate.From.Month, WorkToUpdate.From.Day,
                           Convert.ToInt32(HourFrom1), Convert.ToInt32(MinuteFrom1), 0);
                    var DateTo = new DateTime(WorkToUpdate.From.Year, WorkToUpdate.From.Month, WorkToUpdate.From.Day,
                           Convert.ToInt32(HourTo1), Convert.ToInt32(MinuteTo1), 0);
                    myWork.From = DateFrom;
                    myWork.To = DateTo;
                    using (var db = new ModelContainer())
                    {
                        var Works = db.Works;
                        Work workToDelete = Works.FirstOrDefault(elem => elem.Id.Equals(WorkToUpdate.Id));
                        db.Works.Attach(workToDelete);
                        db.Works.Remove(workToDelete);
                        db.SaveChanges();
                    }
                    using (var db = new ModelContainer())
                    {
                        db.Works.Add(myWork);
                        db.SaveChanges();
                    }
                }
                else
                {
                    MessageBox.Show("Zle zadaná pracovná doba!");
                    return;
                }
            }

            this.Close();
        }

        #region Notification
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Notofication
    }
}
