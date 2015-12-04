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
    /// Interaction logic for ManageEmployees.xaml
    /// </summary>
    public partial class ManageEmployees : Window, INotifyPropertyChanged
    {
        public const string FIRST_NAME_HINT = "Meno";
        public const string LAST_NAME_HINT = "Priezvisko";

        private CollectionView mPeople;
        public CollectionView People
        {
            get
            {
                return mPeople;
            }
            set
            {
                mPeople = value;
                RaisePropertyChangedEvent("People");
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

        private string mFirstName;
        public string FirstName
        {
            get
            {
                return mFirstName;
            }
            set
            {
                mFirstName = value;
                RaisePropertyChangedEvent("FirstName");
            }
        }

        private string mLastName;
        public string LastName
        {
            get
            {
                return mLastName;
            }
            set
            {
                mLastName = value;
                RaisePropertyChangedEvent("LastName");
            }
        }

        private int _EmployeeId;
        public int EmployeeId
        {
            get { return _EmployeeId; }
            set
            {
                _EmployeeId = value;
                RaisePropertyChangedEvent("EmployeeId");
            }
        }

        public ManageEmployees(ObservableCollection<EmployeeEntity> employees)
        {
            IsAddingChecked = true;

            var List = new ObservableCollection<EmployeeEntity>();
            foreach (var current in employees.OrderBy(e => e.LastName))
            {
                List.Add(current);
            }
            People = new CollectionView(List);
            InitializeComponent();
            this.DataContext = this;

            FirstName = FIRST_NAME_HINT;
            FirstNameTextBox.Foreground = Brushes.Gray;
            FirstNameTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_GotKeyboardFocus);
            FirstNameTextBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_LostKeyboardFocusFirstName);

            LastName = LAST_NAME_HINT;
            LastNameTextBox.Foreground = Brushes.Gray;
            LastNameTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_GotKeyboardFocus);
            LastNameTextBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(tb_LostKeyboardFocusLastName);
        }

        private void DeleteEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeeId != 0 && IsDeletingChecked)
            {
                using (var db = new ModelContainer())
                {
                    var EmployeeToDelete = db.Employees.Where(elm => elm.Id.Equals(EmployeeId)).FirstOrDefault();
                    if (EmployeeToDelete != null)
                    {
                        var WorksToDelete = db.Works.Where(elm => elm.EmployeeId.Equals(EmployeeId));
                        MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(
                                GenerateWarningMessage(EmployeeToDelete, WorksToDelete.Count()),
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
                            db.Employees.Attach(EmployeeToDelete);
                            db.Employees.Remove(EmployeeToDelete);
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

        private void AddEmployee(object sender, RoutedEventArgs e)
        {
            if (FirstName != null && LastName != null)
            {
                if (FirstName != "" && FirstName != FIRST_NAME_HINT
                    && LastName != "" && LastName != LAST_NAME_HINT)
                {
                    var createdEmployee = new Employee();
                    createdEmployee.FirstName = FirstName;
                    createdEmployee.LastName = LastName;
                    //createdEmployee.Works = new ObservableCollection<Work>();
                    //createdEmployee.SetOfWorkAsString = "";

                    using (var db = new ModelContainer())
                    {
                        db.Employees.Add(createdEmployee);
                        db.SaveChanges();
                    }
                    MessageBox.Show("Úspešne ste zaviedli zamestnanca menom \"" +
                        createdEmployee.FirstName + " " + createdEmployee.LastName + "\"");
                    Close();
                    return;
                }
            }
        }

        private string GenerateWarningMessage(Employee empl, int works)
        {
            var sb = new StringBuilder();
            sb.Append("Chcete naozaj nenávratne odstrániť zamestnanca \"");
            sb.Append(empl.FirstName);
            sb.Append(" ");
            sb.Append(empl.LastName);
            sb.Append("\" zo systému?");
            if (works > 0)
            {
                sb.AppendLine();
                sb.Append("Tento zamestnanec má v systéme ešte ");
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

        private string GenerateConfirmationMessage(int works)
        {
            var sb = new StringBuilder();
            sb.Append("Úspešne ste odstránili zamestnanca");
            if (works > 4)
            {
                sb.Append(", aj jeho ");
                sb.Append(works);
                sb.Append(" pracovných zaradení");
            }
            else if (works == 1)
            {
                sb.Append(", aj jeho ");
                sb.Append(works);
                sb.Append(" pracovné zaradenie");
            }
            else
            {
                sb.Append(", aj jeho ");
                sb.Append(works);
                sb.Append(" pracovné zaradenia");
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

        private void tb_LostKeyboardFocusFirstName(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is TextBox)
            {
                //If nothing was entered, reset default text.
                if (((TextBox)sender).Text.Trim().Equals(""))
                {
                    ((TextBox)sender).Foreground = Brushes.Gray;
                    ((TextBox)sender).Text = FIRST_NAME_HINT;
                }
            }
        }

        private void tb_LostKeyboardFocusLastName(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is TextBox)
            {
                //If nothing was entered, reset default text.
                if (((TextBox)sender).Text.Trim().Equals(""))
                {
                    ((TextBox)sender).Foreground = Brushes.Gray;
                    ((TextBox)sender).Text = LAST_NAME_HINT;
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
