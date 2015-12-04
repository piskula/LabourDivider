using LabourDivider.Entities;
using LabourDivider.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

namespace LabourDivider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public const string WEEK_PRINT_BUTTON = "Tlačiť VÝKAZ týždeň ";
        public const string DAY_PRINT_BUTTON = "Tlačiť ROZPIS na dnes ";

        private BackgroundWorker bw;

        private string mDayButton;
        public string DayButton
        {
            get
            {
                return mDayButton;
            }
            set
            {
                mDayButton = value;
                RaisePropertyChangedEvent("DayButton");
            }
        }

        private string mDaysButton;
        public string DaysButton
        {
            get
            {
                return mDaysButton;
            }
            set
            {
                mDaysButton = value;
                RaisePropertyChangedEvent("DaysButton");
            }
        }

        private DateTime mFirstDate;
        public DateTime FirstDate
        {
            get
            {
                return mFirstDate;
            }
            set
            {
                mFirstDate = value;
                SecondDateTime = value;
                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                WeekNumber = cal.GetWeekOfYear(FirstDate, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);
                RaisePropertyChangedEvent("FirstDate");
            }
        }

        private DateTime mSecondDateTime;
        public DateTime SecondDateTime
        {
            get
            {
                return mSecondDateTime;
            }
            set
            {
                mSecondDateTime = value;
                if (SecondDateTime.Date.Equals(FirstDate.Date))
                {
                    foreach (var currentJob in Jobs)
                    {
                        currentJob.filter(FirstDate);
                    }
                    foreach (var currentEmployee in Employees)
                    {
                        currentEmployee.filter(FirstDate);
                    }
                }
                else
                {
                    foreach (var currentJob in Jobs)
                    {
                        currentJob.filter(FirstDate, SecondDateTime);
                    }
                    foreach (var currentEmployee in Employees)
                    {
                        currentEmployee.filter(FirstDate, SecondDateTime);
                    }
                }

                RaisePropertyChangedEvent("SecondDateTime");
                int days = (SecondDateTime - FirstDate).Days + 1;
                if (days >= 1)
                {
                    NumberOfDays = days;
                }
                else
                {
                    NumberOfDays = 1;
                }
                MyEmployees.Items.Refresh();
                MyJobs.Items.Refresh();
                DaysButton = GenerateDaysButtonString();
                DayButton = DAY_PRINT_BUTTON + DateTime.Now.ToString("dd.MM.");
            }
        }

        private ObservableCollection<EmployeeEntity> mEmployees;
        public ObservableCollection<EmployeeEntity> Employees
        {
            get
            {
                return mEmployees;
            }
            set
            {
                mEmployees = value;
                EmployeesDisplayed = new SortedSet<EmployeeEntity>(Employees);
                RaisePropertyChangedEvent("Employees");
            }
        }

        private ObservableCollection<JobEntity> mJobs;
        public ObservableCollection<JobEntity> Jobs
        {
            get
            {
                return mJobs;
            }
            set
            {
                mJobs = value;
                JobsDisplayed = new SortedSet<JobEntity>(Jobs);
                RaisePropertyChangedEvent("Jobs");
            }
        }

        private ObservableCollection<WorkEntity> mWorks;
        public ObservableCollection<WorkEntity> Works
        {
            get
            {
                return mWorks;
            }
            set
            {
                mWorks = value;
                RaisePropertyChangedEvent("Works");
            }
        }

        private SortedSet<JobEntity> mJobsDisplayed;
        public SortedSet<JobEntity> JobsDisplayed
        {
            get
            {
                return mJobsDisplayed;
            }
            set
            {
                mJobsDisplayed = value;
                RaisePropertyChangedEvent("JobsDisplayed");
            }
        }

        private SortedSet<EmployeeEntity> mEmployeesDisplayed;
        public SortedSet<EmployeeEntity> EmployeesDisplayed
        {
            get
            {
                return mEmployeesDisplayed;
            }
            set
            {
                mEmployeesDisplayed = value;
                RaisePropertyChangedEvent("EmployeesDisplayed");
            }
        }

        private int mWeekNumber;
        public int WeekNumber
        {
            get
            {
                return mWeekNumber;
            }
            set
            {
                mWeekNumber = value;
                RaisePropertyChangedEvent("WeekNumber");
            }
        }

        private int mNumberOfDays;
        public int NumberOfDays
        {
            get
            {
                return mNumberOfDays;
            }
            set
            {
                mNumberOfDays = value;
                RaisePropertyChangedEvent("NumberOfDays");
            }
        }

        #region SelectedItems
        private JobEntity mSelectedJob;
        public JobEntity SelectedJob
        {
            get
            {
                return mSelectedJob;
            }
            set
            {
                mSelectedJob = value;
                RaisePropertyChangedEvent("SelectedJob");
            }
        }

        private EmployeeEntity mSelectedEmployee;
        public EmployeeEntity SelectedEmployee
        {
            get
            {
                return mSelectedEmployee;
            }
            set
            {
                mSelectedEmployee = value;
                RaisePropertyChangedEvent("SelectedEmployee");
            }
        }
        #endregion SelectedItems

        #region Buttons
        private void AddNewWork(object sender, RoutedEventArgs e)
        {
            AssignWork AddNewWork = new AssignWork(FirstDate, Employees, Jobs, Works);
            AddNewWork.Closing += Window_Closing;
            AddNewWork.ShowDialog();
        }

        private void PrintThisDateJobs(object sender, RoutedEventArgs e)
        {/*
            var PrntDlg = new System.Windows.Controls.PrintDialog();
            if (PrntDlg.ShowDialog() == true)
            {
                var doc = new PrintDocument(Employees, Jobs, Works);
                var output = doc.GenerateJobToEmployeesDay(DateTime.Now);
                output.Name = "OneDayJobToEmployeesPrint";

                IDocumentPaginatorSource idpsource = output;
                try
                {
                    PrntDlg.PrintDocument(idpsource.DocumentPaginator, "Configuration Print");
                }
                catch (Exception)
                {
                    return;
                }
            }*/
        }

        private void PrintTheseDatesJobs(object sender, RoutedEventArgs e)
        {/*
            var PrntDlg = new System.Windows.Controls.PrintDialog();
            if (PrntDlg.ShowDialog() == true)
            {
                var doc = new PrintDocument(Employees, Jobs, Works);
                var output = doc.GenerateEmployeeToJobsDays(FirstDate, SecondDateTime);
                output.Name = "MoreDaysEmployeeToJobsPrint";

                IDocumentPaginatorSource idpsource = output;
                try
                {
                    PrntDlg.PrintDocument(idpsource.DocumentPaginator, "Configuration Print");
                }
                catch (Exception)
                {
                    return;
                }
            }*/
        }

        private void PrintTheseDatesEmployees(object sender, RoutedEventArgs e)
        {/*
            var PrntDlg = new System.Windows.Controls.PrintDialog();
            if (PrntDlg.ShowDialog() == true)
            {
                var doc = new PrintDocument(Employees, Jobs, Works);
                var output = doc.GenerateJobToEmployeesDays(FirstDate, SecondDateTime);
                output.Name = "MoreDaysJobToEmployeesPrint";

                IDocumentPaginatorSource idpsource = output;
                try
                {
                    PrntDlg.PrintDocument(idpsource.DocumentPaginator, "Configuration Print");
                }
                catch (Exception)
                {
                    return;
                }
            }*/
        }

        private void PrintThisDateEmployees(object sender, RoutedEventArgs e)
        {/*
            var PrntDlg = new System.Windows.Controls.PrintDialog();
            if (PrntDlg.ShowDialog() == true)
            {
                var doc = new PrintDocument(Employees, Jobs, Works);
                var output = doc.GenerateEmployeeToJobsDay(FirstDate);
                output.Name = "OneDayEmployeeToJobsPrint";

                IDocumentPaginatorSource idpsource = output;
                try
                {
                    PrntDlg.PrintDocument(idpsource.DocumentPaginator, "Configuration Print");
                }
                catch (Exception)
                {
                    return;
                }
            }*/
        }

        private void PrintOutEmployee(object sender, RoutedEventArgs e)
        {/*
            if (SelectedEmployee != null)
            {
                if (SelectedEmployee.Id != 0)
                {
                    PrintOut printer = new PrintOut(SelectedEmployee, FirstDate, SecondDateTime);
                    printer.Closing += Window_Closing_Without_Refresh;
                    printer.ShowDialog();
                }
            }*/
        }

        private void PrintOutJob(object sender, RoutedEventArgs e)
        {/*
            if (SelectedJob != null)
            {
                if (SelectedJob.Id != 0)
                {
                    PrintOutJob printer = new PrintOutJob(SelectedJob, FirstDate, SecondDateTime);
                    printer.Closing += Window_Closing_Without_Refresh;
                    printer.ShowDialog();
                }
            }*/
        }

        private void EditJob(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {
                if (SelectedJob.Id != 0)
                {
                    JobWorkDetails Edit = new JobWorkDetails(SelectedJob, FirstDate, SecondDateTime);
                    Edit.Closing += Window_Closing;
                    Edit.ShowDialog();
                }
            }
        }

        private void EditEmployee(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee != null)
            {
                if (SelectedEmployee.Id != 0)
                {
                    EmployeeWorkDetails Edit = new EmployeeWorkDetails(SelectedEmployee, FirstDate, SecondDateTime);
                    Edit.Closing += Window_Closing;
                    Edit.ShowDialog();
                }
            }
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            //pictureBoxLoading.Image = System.Drawing.Image.FromFile(LOADER);
            pictureBoxLoading.Image = LabourDivider.Properties.Resources.loader32;
            bw.RunWorkerAsync();
        }
        void Window_Closing_Without_Refresh(object sender, CancelEventArgs e)
        {
            this.Show();
        }

        private void Refresh()
        {
            bw_DoWork(this, null);
            
            log("bw_done");
            int fuckinNumberDays = 0;
            if (NumberOfDays >= 1)
            {
                fuckinNumberDays = NumberOfDays - 1;
            }
            SecondDateTime = FirstDate.AddDays(fuckinNumberDays);
        }

        private void EndWork(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion Buttons

        public MainWindow()
        {
            var directInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directInfo != null)
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", directInfo.FullName);
                log(directInfo.FullName);
            }
            //log("001");
            InitializeComponent();
            //log("002");
            this.DataContext = this;

            //log("003");
            bw = new BackgroundWorker();
            //log("004");
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            //log("005");
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            //log("006");
            bw.WorkerReportsProgress = true;
            //log("007");
            bw.WorkerSupportsCancellation = true;

            //log("008");
            pictureBoxLoading.Image = LabourDivider.Properties.Resources.yes17;
            //log("009");
            Refresh();
            //log("refreshed");
            FirstDate = DateTime.Today;
        }

        public void log(string msg)
        {
            StreamWriter file2 = new StreamWriter("labourdividerlogfile.txt", true);
            file2.WriteLine(DateTime.Now + " : " + msg);
            file2.Close();
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

        private string GenerateDaysButtonString()
        {
            if (NumberOfDays == 1)
            {
                return "Tlačiť len " + FirstDate.ToString("dd.MM.");
            }
            else
            {
                var str = new StringBuilder();
                str.Append("Tlačiť ");
                str.Append(NumberOfDays);
                if (NumberOfDays > 4)
                {
                    str.Append(" vybraných dní (");
                }
                else
                {
                    str.Append(" vybrané dni (");
                }
                str.Append(FirstDate.ToString("dd.MM."));
                str.Append(" až ");
                str.Append(SecondDateTime.ToString("dd.MM."));
                str.Append(")");
                return str.ToString();
            }
        }

        private void SetWholeWeek(object sender, RoutedEventArgs e)
        {
            int diff = FirstDate.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
                diff += 7;
            FirstDate = FirstDate.AddDays(-diff);
            SecondDateTime = FirstDate.AddDays(5);
        }

        private void SetWholeMonth(object sender, RoutedEventArgs e)
        {
            FirstDate = new DateTime(FirstDate.Year, FirstDate.Month, 1);
            SecondDateTime = new DateTime(FirstDate.Year, FirstDate.Month, DateTime.DaysInMonth(FirstDate.Year, FirstDate.Month));
        }

        private void ManageJobs(object sender, RoutedEventArgs e)
        {
            ManageJobs ManageJobsWindow = new ManageJobs(Jobs);
            ManageJobsWindow.Closing += Window_Closing;
            ManageJobsWindow.ShowDialog();

            //TOTO OSTAVA ZAKOMENTOVANE
            /*using (var db = new employeesContainer())
            {
                int i = 0;
                foreach (var cur in Employees)
                {
                    var curDb = db.Employees1.Where(elm => elm.Id.Equals(cur.Id)).FirstOrDefault();
                    if (curDb != null)
                    {
                        db.Employees1.Attach(curDb);
                        db.Employees1.Remove(curDb);
                        db.SaveChanges();
                        i++;
                        if (i > 100)
                            break;
                    }
                }
                System.Windows.MessageBox.Show("Úspešne ste odstránili " +
                        i + " zamestnancov");
            }*/

            /*using (var db = new employeesContainer())
            {
                int i = 0;
                foreach (var cur in Jobs)
                {
                    var curDb = db.Jobs.Where(elm => elm.Id.Equals(cur.Id)).FirstOrDefault();
                    if (curDb != null)
                    {
                        db.Jobs.Attach(curDb);
                        db.Jobs.Remove(curDb);
                        db.SaveChanges();
                        i++;
                    }
                }
                System.Windows.MessageBox.Show("Úspešne ste odstránili " +
                        i + " pozícií");
            }*/

            /*using (var db = new employeesContainer())
            {
                int i = 0;
                foreach (var cur in Works)
                {
                    var curDb = db.Works.Where(elm => elm.Id.Equals(cur.Id)).FirstOrDefault();
                    if (curDb != null)
                    {
                        db.Works.Attach(curDb);
                        db.Works.Remove(curDb);
                        db.SaveChanges();
                        i++;
                        if (i > 100)
                            break;
                    }
                }
                System.Windows.MessageBox.Show("Úspešne ste odstránili " +
                        i + " zaradení");
            }*/

            /*Random rnd = new Random();
            int i;
            for(i = 0; i < 1000; i++)
            {
                var work = new Work();
                work.EmployeesId = Employees.ElementAt(rnd.Next(Employees.Count())).Id;
                work.JobId = Jobs.ElementAt(rnd.Next(Jobs.Count())).Id;
                work.From = new DateTime(2015, 8, rnd.Next(1, 30), rnd.Next(1, 12), (rnd.Next(11) * 5), 0);
                work.To = new DateTime(work.From.Year, work.From.Month, work.From.Day, rnd.Next(13, 22), (rnd.Next(11) * 5), 0);
                using (var db = new employeesContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                }
            }
            System.Windows.MessageBox.Show("Úspešne ste vytvorili " +
                        i + " zaradení");*/
        }

        private void ManageEmployees(object sender, RoutedEventArgs e)
        {
            ManageEmployees ManageEmployeesWindow = new ManageEmployees(Employees);
            ManageEmployeesWindow.Closing += Window_Closing;
            ManageEmployeesWindow.ShowDialog();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {

            using (var db = new ModelContainer())
            {
                log("dbcontext created");
                var EmployeesTemp = new ObservableCollection<Employee>(db.Employees);
                var JobsTemp = new ObservableCollection<Job>(db.Jobs);
                var WorksTemp = new ObservableCollection<Work>(db.Works);
                log("Employees, Jobs, Works = set");

                var WorksEntityTemp = new ObservableCollection<WorkEntity>();
                var EmployeesEntityTemp = new ObservableCollection<EmployeeEntity>();
                var JobsEntityTemp = new ObservableCollection<JobEntity>();

                foreach(var current in EmployeesTemp)
                    EmployeesEntityTemp.Add(new EmployeeEntity(current));
                foreach (var current in WorksTemp)
                    WorksEntityTemp.Add(new WorkEntity(current));
                foreach (var current in JobsTemp)
                    JobsEntityTemp.Add(new JobEntity(current));

                Works = WorksEntityTemp;
                Employees = EmployeesEntityTemp;
                Jobs = JobsEntityTemp;

                int all = Works.Count();
                int i = 0;
                foreach (var currentWork in Works)
                {
                    foreach (var currentJob in Jobs)
                    {
                        if (currentJob.Id.Equals(currentWork.JobId))
                        {
                            currentJob.WorksEntity.Add(currentWork);
                        }
                    }
                    foreach (var currentEmployee in Employees)
                    {
                        if (currentEmployee.Id.Equals(currentWork.EmployeeId))
                        {
                            currentEmployee.WorksEntity.Add(currentWork);
                        }
                    }
                    i++;
                }
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int fuckinNumberDays = 0;
            if (NumberOfDays >= 1)
            {
                fuckinNumberDays = NumberOfDays - 1;
            }
            SecondDateTime = FirstDate.AddDays(fuckinNumberDays);

            this.Show();

            pictureBoxLoading.Image = LabourDivider.Properties.Resources.yes17;
            bw.CancelAsync();
        }
    }
}
