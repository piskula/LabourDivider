﻿using LabourDivider.Entities;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EmployeeWorkDetails.xaml
    /// </summary>
    public partial class EmployeeWorkDetails : Window, INotifyPropertyChanged
    {
        private BackgroundWorker bw;
        private SortedSet<WorkEntity> mWorks;
        public SortedSet<WorkEntity> Works
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

        private EmployeeEntity mCurrentEmployee;
        public EmployeeEntity CurrentEmployee
        {
            get
            {
                return mCurrentEmployee;
            }
            set
            {
                mCurrentEmployee = value;
                RaisePropertyChangedEvent("CurrentEmployee");
            }
        }

        private int mCurrentEmployeeId;
        public int CurrentEmployeeId
        {
            get
            {
                return mCurrentEmployeeId;
            }
            set
            {
                mCurrentEmployeeId = value;
                RaisePropertyChangedEvent("CurrentEmployeeId");
            }
        }

        private Work mWorkToDelete;
        public Work WorkToDelete
        {
            get
            {
                return mWorkToDelete;
            }
            set
            {
                mWorkToDelete = value;
                RaisePropertyChangedEvent("WorkToDelete");
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

        private DateTime mFromDate;
        public DateTime FromDate
        {
            get
            {
                return mFromDate;
            }
            set
            {
                mFromDate = value;
                RaisePropertyChangedEvent("FromDate");
                if (ToDate != null)
                {
                    if (FromDate.Date <= ToDate.Date)
                    {
                        Works = new SortedSet<WorkEntity>(CurrentEmployee.WorksEntity.Where(e =>
                            (e.From.Date <= ToDate.Date && e.From.Date >= FromDate.Date)));
                    }
                    else
                    {
                        Works = new SortedSet<WorkEntity>();
                    }
                }
            }
        }

        private DateTime mToDate;
        public DateTime ToDate
        {
            get
            {
                return mToDate;
            }
            set
            {
                mToDate = value;
                RaisePropertyChangedEvent("ToDate");
                if (FromDate != null)
                {
                    if (FromDate.Date <= ToDate.Date)
                    {
                        Works = new SortedSet<WorkEntity>(CurrentEmployee.WorksEntity.Where(e =>
                      (e.From.Date <= ToDate.Date && e.From.Date >= FromDate.Date)));
                    }
                    else
                    {
                        Works = new SortedSet<WorkEntity>();
                    }
                }
            }
        }

        public EmployeeWorkDetails(EmployeeEntity EditingEmployee, DateTime from, DateTime to)
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentEmployee = EditingEmployee;
            CurrentEmployeeId = EditingEmployee.Id;
            Titlee = EditingEmployee.FirstName + " " + EditingEmployee.LastName;
            FromDate = from;
            ToDate = to;

            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            ReloadEmployeeProgressBar.Value = 0;
            pictureBoxLoading.Image = LabourDivider.Properties.Resources.yes17;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (WorkToDelete != null)
            {
                using (var db = new ModelContainer())
                {
                    Work workToDelete = db.Works.FirstOrDefault(elem => elem.Id.Equals(WorkToDelete.Id));
                    db.Works.Attach(workToDelete);
                    db.Works.Remove(workToDelete);
                    db.SaveChanges();

                    WorkEntity workToDeleteInList = this.Works.Where(elem => elem.Id.Equals(workToDelete.Id)).First();
                    if (workToDeleteInList != null)
                    {
                        this.Works.Remove(workToDeleteInList);
                    }
                    CurrentEmployeeWork.Items.Refresh();
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (CurrentEmployee != null)
            {
                if (CurrentEmployee.Id != 0)
                {
                    EditWorkDetails EditWork = new EditWorkDetails(WorkToDelete);
                    EditWork.Closing += Window_Closing;
                    EditWork.ShowDialog();
                }
            }
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            pictureBoxLoading.Image = LabourDivider.Properties.Resources.loader32;
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var db = new ModelContainer())
            {
                CurrentEmployee = new EmployeeEntity(db.Employees.FirstOrDefault(elm => elm.Id.Equals(CurrentEmployeeId)));
                int all = db.Works.Where(elm => elm.Employee.Equals(CurrentEmployeeId)).Count();
                int i = 0;
                foreach (Work currentWork in db.Works.Where(elm => elm.EmployeeId.Equals(CurrentEmployeeId)))
                {
                    foreach (var currentJob in db.Jobs.Where(elm => elm.Id.Equals(currentWork.JobId)))
                    {
                        currentWork.Job = currentJob;
                    }
                    CurrentEmployee.WorksEntity.Add(new WorkEntity(currentWork));
                    i++;
                    bw.ReportProgress((i * 100) / all);
                }
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReloadEmployeeProgressBar.Value = e.ProgressPercentage;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Works = new SortedSet<WorkEntity>(CurrentEmployee.WorksEntity.Where(elm =>
                     (elm.From.Date <= ToDate.Date && elm.From.Date >= FromDate.Date)));
            CurrentEmployeeWork.Items.Refresh();

            this.Show();

            pictureBoxLoading.Image = LabourDivider.Properties.Resources.yes17;
            ReloadEmployeeProgressBar.Value = 0;
            bw.CancelAsync();
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
