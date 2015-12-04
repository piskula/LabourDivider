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
    /// Interaction logic for ManageJobs.xaml
    /// </summary>
    public partial class ManageJobs : Window, INotifyPropertyChanged
    {
        public const string NAME_HINT = "Názov pozície";

        private CollectionView mJobTypes;
        public CollectionView JobTypes
        {
            get
            {
                return mJobTypes;
            }
            set
            {
                mJobTypes = value;
                RaisePropertyChangedEvent("JobTypes");
            }
        }

        private bool mIsAddingChecked;
        public bool IsAddingChecked
        {
            get
            {
                return mIsAddingChecked;
            }
            set
            {
                mIsAddingChecked = value;
                if (IsDeletingChecked == IsAddingChecked)
                    IsDeletingChecked = !IsAddingChecked;
                RaisePropertyChangedEvent("IsAddingChecked");
            }
        }

        private bool mIsDeletingChecked;
        public bool IsDeletingChecked
        {
            get
            {
                return mIsDeletingChecked;
            }
            set
            {
                mIsDeletingChecked = value;
                if (IsAddingChecked == IsDeletingChecked)
                    IsAddingChecked = !IsDeletingChecked;
                RaisePropertyChangedEvent("IsDeletingChecked");
            }
        }

        private string mJobName;
        public string JobName
        {
            get
            {
                return mJobName;
            }
            set
            {
                mJobName = value;
                RaisePropertyChangedEvent("JobName");
            }
        }

        private int _JobId;
        public int JobId
        {
            get { return _JobId; }
            set
            {
                _JobId = value;
                RaisePropertyChangedEvent("JobId");
            }
        }

        public ManageJobs(ObservableCollection<JobEntity> jobs)
        {
            IsAddingChecked = true;

            var List = new ObservableCollection<JobEntity>();
            foreach (var current in jobs.OrderBy(e => e.Name))
            {
                List.Add(current);
            }
            JobTypes = new CollectionView(List);
            InitializeComponent();
            this.DataContext = this;

            JobName = NAME_HINT;
            NameTextBox.Foreground = Brushes.Gray;
            NameTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_GotKeyboardFocus);
            NameTextBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_LostKeyboardFocus);
        }

        private void DeleteJob(object sender, RoutedEventArgs e)
        {
            if (JobId != 0 && IsDeletingChecked)
            {
                using (var db = new ModelContainer())
                {
                    var JobToDelete = db.Jobs.Where(elm => elm.Id.Equals(JobId)).FirstOrDefault();
                    if (JobToDelete != null)
                    {
                        var WorksToDelete = db.Works.Where(elm => elm.JobId.Equals(JobId));
                        MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(
                                GenerateWarningMessage(JobToDelete, WorksToDelete.Count()),
                                "Varovanie", System.Windows.MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            int count = 0;
                            SortedSet<Work> _works = new SortedSet<Work>(WorksToDelete);
                            foreach (var currentWork in _works)
                            {
                                var workToDelete = db.Works.Where(elm => elm.Id.Equals(currentWork.Id)).FirstOrDefault();
                                db.Works.Attach(workToDelete);
                                db.Works.Remove(workToDelete);
                                db.SaveChanges();
                                count++;
                            }
                            db.Jobs.Attach(JobToDelete);
                            db.Jobs.Remove(JobToDelete);
                            db.SaveChanges();
                            MessageBox.Show(GenerateConfirmationMessage(count));
                            Close();
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("Neurobili ste žiadnu zmenu");
            Close();
        }

        private void AddJob(object sender, RoutedEventArgs e)
        {
            if (JobName != null)
            {
                if (JobName != "" && JobName != NAME_HINT)
                {
                    var createdPosition = new Job();
                    createdPosition.Name = JobName;
                    //createdPosition.Works = new ObservableCollection<Work>();
                    //createdPosition.SetOfWorkingPeopleAsString = "";

                    using (var db = new ModelContainer())
                    {
                        db.Jobs.Add(createdPosition);
                        db.SaveChanges();
                    }
                    MessageBox.Show("Úspešne ste vytvorili pozíciu s názvom \"" +
                        createdPosition.Name + "\"");
                    Close();
                    return;
                }
            }
        }

        private string GenerateConfirmationMessage(int works)
        {
            var sb = new StringBuilder();
            sb.Append("Úspešne ste odstránili pozíciu ");
            if (works == 0)
                return sb.ToString();
            if (works > 4)
            {
                sb.Append(", aj jej ");
                sb.Append(works);
                sb.Append(" pracovných zaradení");
            }
            else if (works == 1)
            {
                sb.Append(", aj jej ");
                sb.Append(works);
                sb.Append(" pracovné zaradenie");
            }
            else
            {
                sb.Append(", aj jej ");
                sb.Append(works);
                sb.Append(" pracovné zaradenia");
            }
            return sb.ToString();
        }

        private string GenerateWarningMessage(Job job, int works)
        {
            var sb = new StringBuilder();
            sb.Append("Chcete naozaj nenávratne odstrániť pozíciu \"");
            sb.Append(job.Name);
            sb.Append("\" zo systému?");
            if (works > 0)
            {
                sb.AppendLine();
                sb.Append("Táto pozícia má v systéme ešte ");
                sb.Append(works);
                if (works == 1)
                {
                    sb.Append(" pracovné zaradenie, ktoré sa tiež vymaže!!!");
                }
                else if (works > 4)
                {
                    sb.Append(" pracovných zaradení, ktoré sa tiež vymažú!!!");
                }
                else
                {
                    sb.Append(" pracovné zaradenia, ktoré sa tiež vymažú!!!");
                }
            }
            return sb.ToString();
        }

        #region TextBoxHinting
        private void tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                //If nothing has been entered yet.
                if (((TextBox)sender).Foreground == Brushes.Gray)
                {
                    ((TextBox)sender).Text = "";
                    ((TextBox)sender).Foreground = Brushes.Black;
                }
            }
        }

        private void tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is TextBox)
            {
                //If nothing was entered, reset default text.
                if (((TextBox)sender).Text.Trim().Equals(""))
                {
                    ((TextBox)sender).Foreground = Brushes.Gray;
                    ((TextBox)sender).Text = NAME_HINT;
                }
            }
        }
        #endregion TextBoxHinting

        #region RaiseProperty
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion RaiseProperty
    }
}
