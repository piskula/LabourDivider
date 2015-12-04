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
    /// Interaction logic for AssignWork.xaml
    /// </summary>
    public partial class AssignWork : Window, INotifyPropertyChanged
    {
        public const string PARSE_DATE = "dd.MM.yyyy";
        public const string TIME_ERROR = "Chybný čas";
        public const string PIC_YES = "..\\Resources\\yes17.gif";
        public const string PIC_DISABLE = "..\\Resources\\no17.gif";
        public const string PIC_ERROR = "..\\Resources\\nok17.gif";
        public const string NO_MSG = "nie";
        public const string COLOR_PASSIVE = "#d1d1d1";
        public const string COLOR_ACTIVE = "#ffffff";

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

        private DateTime mDate;
        public DateTime Date
        {
            get
            {
                return mDate;
            }
            set
            {
                mDate = value;
                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                WeekNumber = cal.GetWeekOfYear(Date, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);
                RaisePropertyChangedEvent("Date");
                SecondDateTime = value;
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
                if (value >= Date)
                {
                    mSecondDateTime = value;
                    RaisePropertyChangedEvent("SecondDateTime");
                    int days = (SecondDateTime - Date).Days + 1;
                    if (days >= 1 && days <= 7)
                    {
                        NumberOfDays = days;
                    }
                    else
                    {
                        NumberOfDays = 1;
                    }

                    var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                    NullActiveDays();
                    for (var day = Date; day <= SecondDateTime; day = day.AddDays(1))
                    {
                        if (cal.GetWeekOfYear(day, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday) == WeekNumber)
                            switch (day.DayOfWeek)
                            {
                                case DayOfWeek.Monday:
                                    DateOrNo1 = day.ToString(PARSE_DATE);
                                    IsActiveDay1 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee1 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee1();
                                    break;
                                case DayOfWeek.Tuesday:
                                    DateOrNo2 = day.ToString(PARSE_DATE);
                                    IsActiveDay2 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee2 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee2();
                                    break;
                                case DayOfWeek.Wednesday:
                                    DateOrNo3 = day.ToString(PARSE_DATE);
                                    IsActiveDay3 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee3 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee3();
                                    break;
                                case DayOfWeek.Thursday:
                                    DateOrNo4 = day.ToString(PARSE_DATE);
                                    IsActiveDay4 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee4 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee4();
                                    break;
                                case DayOfWeek.Friday:
                                    DateOrNo5 = day.ToString(PARSE_DATE);
                                    IsActiveDay5 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee5 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee5();
                                    break;
                                case DayOfWeek.Saturday:
                                    DateOrNo6 = day.ToString(PARSE_DATE);
                                    IsActiveDay6 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee6 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee6();
                                    break;
                                case DayOfWeek.Sunday:
                                    DateOrNo7 = day.ToString(PARSE_DATE);
                                    IsActiveDay7 = true;
                                    if (Empl != null)
                                    {
                                        WorkHasAlreadyEmployee7 = GenerateHasAlreadyWorkString(Empl, day);
                                    }
                                    ValidateEmployee7();
                                    break;
                            }
                        else break;
                    }
                    ValidateAllRows();
                }
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

        #region ComboBoxCollections
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

        private CollectionView mJobTypes2;
        public CollectionView JobTypes2
        {
            get
            {
                return mJobTypes2;
            }
            set
            {
                mJobTypes2 = value;
                RaisePropertyChangedEvent("JobTypes2");
            }
        }

        private CollectionView mJobTypes3;
        public CollectionView JobTypes3
        {
            get
            {
                return mJobTypes3;
            }
            set
            {
                mJobTypes3 = value;
                RaisePropertyChangedEvent("JobTypes3");
            }
        }

        private CollectionView mJobTypes4;
        public CollectionView JobTypes4
        {
            get
            {
                return mJobTypes4;
            }
            set
            {
                mJobTypes4 = value;
                RaisePropertyChangedEvent("JobTypes4");
            }
        }

        private CollectionView mJobTypes5;
        public CollectionView JobTypes5
        {
            get
            {
                return mJobTypes5;
            }
            set
            {
                mJobTypes5 = value;
                RaisePropertyChangedEvent("JobTypes5");
            }
        }

        private CollectionView mJobTypes6;
        public CollectionView JobTypes6
        {
            get
            {
                return mJobTypes6;
            }
            set
            {
                mJobTypes6 = value;
                RaisePropertyChangedEvent("JobTypes6");
            }
        }

        private CollectionView mJobTypes7;
        public CollectionView JobTypes7
        {
            get
            {
                return mJobTypes7;
            }
            set
            {
                mJobTypes7 = value;
                RaisePropertyChangedEvent("JobTypes7");
            }
        }
        #endregion ComboBoxCollections

        #region IsActiveDayX
        private Boolean mIsActiveDay1;
        public Boolean IsActiveDay1
        {
            get
            {
                return mIsActiveDay1;
            }
            set
            {
                mIsActiveDay1 = value;
                RaisePropertyChangedEvent("IsActiveDay1");
            }
        }
        private Boolean mIsActiveDay2;
        public Boolean IsActiveDay2
        {
            get
            {
                return mIsActiveDay2;
            }
            set
            {
                mIsActiveDay2 = value;
                RaisePropertyChangedEvent("IsActiveDay2");
            }
        }
        private Boolean mIsActiveDay3;
        public Boolean IsActiveDay3
        {
            get
            {
                return mIsActiveDay3;
            }
            set
            {
                mIsActiveDay3 = value;
                RaisePropertyChangedEvent("IsActiveDay3");
            }
        }
        private Boolean mIsActiveDay4;
        public Boolean IsActiveDay4
        {
            get
            {
                return mIsActiveDay4;
            }
            set
            {
                mIsActiveDay4 = value;
                RaisePropertyChangedEvent("IsActiveDay4");
            }
        }
        private Boolean mIsActiveDay5;
        public Boolean IsActiveDay5
        {
            get
            {
                return mIsActiveDay5;
            }
            set
            {
                mIsActiveDay5 = value;
                RaisePropertyChangedEvent("IsActiveDay5");
            }
        }
        private Boolean mIsActiveDay6;
        public Boolean IsActiveDay6
        {
            get
            {
                return mIsActiveDay6;
            }
            set
            {
                mIsActiveDay6 = value;
                RaisePropertyChangedEvent("IsActiveDay6");
            }
        }
        private Boolean mIsActiveDay7;
        public Boolean IsActiveDay7
        {
            get
            {
                return mIsActiveDay7;
            }
            set
            {
                mIsActiveDay7 = value;
                RaisePropertyChangedEvent("IsActiveDay7");
            }
        }
        #endregion IsActiveDayX

        #region Colors
        private string mColorDay1;
        public string ColorDay1
        {
            get
            {
                return mColorDay1;
            }
            set
            {
                mColorDay1 = value;
                RaisePropertyChangedEvent("ColorDay1");
            }
        }

        private string mColorDay2;
        public string ColorDay2
        {
            get
            {
                return mColorDay2;
            }
            set
            {
                mColorDay2 = value;
                RaisePropertyChangedEvent("ColorDay2");
            }
        }

        private string mColorDay3;
        public string ColorDay3
        {
            get
            {
                return mColorDay3;
            }
            set
            {
                mColorDay3 = value;
                RaisePropertyChangedEvent("ColorDay3");
            }
        }

        private string mColorDay4;
        public string ColorDay4
        {
            get
            {
                return mColorDay4;
            }
            set
            {
                mColorDay4 = value;
                RaisePropertyChangedEvent("ColorDay4");
            }
        }

        private string mColorDay5;
        public string ColorDay5
        {
            get
            {
                return mColorDay5;
            }
            set
            {
                mColorDay5 = value;
                RaisePropertyChangedEvent("ColorDay5");
            }
        }

        private string mColorDay6;
        public string ColorDay6
        {
            get
            {
                return mColorDay6;
            }
            set
            {
                mColorDay6 = value;
                RaisePropertyChangedEvent("ColorDay6");
            }
        }

        private string mColorDay7;
        public string ColorDay7
        {
            get
            {
                return mColorDay7;
            }
            set
            {
                mColorDay7 = value;
                RaisePropertyChangedEvent("ColorDay7");
            }
        }
        #endregion Colors

        //ukaze datum alebo "nie"
        #region DateOrNoX
        private string mDateOrNo1;
        public string DateOrNo1
        {
            get
            {
                return mDateOrNo1;
            }
            set
            {
                mDateOrNo1 = value;
                RaisePropertyChangedEvent("DateOrNo1");
                ValidateRow(1);
                ChangeColor(1);
            }
        }

        private string mDateOrNo2;
        public string DateOrNo2
        {
            get
            {
                return mDateOrNo2;
            }
            set
            {
                mDateOrNo2 = value;
                RaisePropertyChangedEvent("DateOrNo2");
                ValidateRow(2);
                ChangeColor(2);
            }
        }

        private string mDateOrNo3;
        public string DateOrNo3
        {
            get
            {
                return mDateOrNo3;
            }
            set
            {
                mDateOrNo3 = value;
                RaisePropertyChangedEvent("DateOrNo3");
                ValidateRow(3);
                ChangeColor(3);
            }
        }

        private string mDateOrNo4;
        public string DateOrNo4
        {
            get
            {
                return mDateOrNo4;
            }
            set
            {
                mDateOrNo4 = value;
                RaisePropertyChangedEvent("DateOrNo4");
                ValidateRow(4);
                ChangeColor(4);
            }
        }

        private string mDateOrNo5;
        public string DateOrNo5
        {
            get
            {
                return mDateOrNo5;
            }
            set
            {
                mDateOrNo5 = value;
                RaisePropertyChangedEvent("DateOrNo5");
                ValidateRow(5);
                ChangeColor(5);
            }
        }

        private string mDateOrNo6;
        public string DateOrNo6
        {
            get
            {
                return mDateOrNo6;
            }
            set
            {
                mDateOrNo6 = value;
                RaisePropertyChangedEvent("DateOrNo6");
                ValidateRow(6);
                ChangeColor(6);
            }
        }

        private string mDateOrNo7;
        public string DateOrNo7
        {
            get
            {
                return mDateOrNo7;
            }
            set
            {
                mDateOrNo7 = value;
                RaisePropertyChangedEvent("DateOrNo7");
                ValidateRow(7);
                ChangeColor(7);
            }
        }

        //

        private string mWorkHasAlreadyEmployee1;
        public string WorkHasAlreadyEmployee1
        {
            get
            {
                return mWorkHasAlreadyEmployee1;
            }
            set
            {
                mWorkHasAlreadyEmployee1 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee1");
            }
        }

        private string mWorkHasAlreadyEmployee2;
        public string WorkHasAlreadyEmployee2
        {
            get
            {
                return mWorkHasAlreadyEmployee2;
            }
            set
            {
                mWorkHasAlreadyEmployee2 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee2");
            }
        }

        private string mWorkHasAlreadyEmployee3;
        public string WorkHasAlreadyEmployee3
        {
            get
            {
                return mWorkHasAlreadyEmployee3;
            }
            set
            {
                mWorkHasAlreadyEmployee3 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee3");
            }
        }

        private string mWorkHasAlreadyEmployee4;
        public string WorkHasAlreadyEmployee4
        {
            get
            {
                return mWorkHasAlreadyEmployee4;
            }
            set
            {
                mWorkHasAlreadyEmployee4 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee4");
            }
        }

        private string mWorkHasALreadyEmployee5;
        public string WorkHasAlreadyEmployee5
        {
            get
            {
                return mWorkHasALreadyEmployee5;
            }
            set
            {
                mWorkHasALreadyEmployee5 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee5");
            }
        }

        private string mWorkHasAlreadyEmployee6;
        public string WorkHasAlreadyEmployee6
        {
            get
            {
                return mWorkHasAlreadyEmployee6;
            }
            set
            {
                mWorkHasAlreadyEmployee6 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee6");
            }
        }

        private string mWorkHasAlreadyEmployee7;
        public string WorkHasAlreadyEmployee7
        {
            get
            {
                return mWorkHasAlreadyEmployee7;
            }
            set
            {
                mWorkHasAlreadyEmployee7 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyEmployee7");
            }
        }

        //

        private string mWorkHasAlreadyJob1;
        public string WorkHasAlreadyJob1
        {
            get
            {
                return mWorkHasAlreadyJob1;
            }
            set
            {
                mWorkHasAlreadyJob1 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob1");
            }
        }

        private string mWorkHasAlreadyJob2;
        public string WorkHasAlreadyJob2
        {
            get
            {
                return mWorkHasAlreadyJob2;
            }
            set
            {
                mWorkHasAlreadyJob2 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob2");
            }
        }

        private string mWorkHasAlreadyJob3;
        public string WorkHasAlreadyJob3
        {
            get
            {
                return mWorkHasAlreadyJob3;
            }
            set
            {
                mWorkHasAlreadyJob3 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob3");
            }
        }

        private string mWorkHasAlreadyJob4;
        public string WorkHasAlreadyJob4
        {
            get
            {
                return mWorkHasAlreadyJob4;
            }
            set
            {
                mWorkHasAlreadyJob4 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob4");
            }
        }

        private string mWorkHasAlreadyJob5;
        public string WorkHasAlreadyJob5
        {
            get
            {
                return mWorkHasAlreadyJob5;
            }
            set
            {
                mWorkHasAlreadyJob5 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob5");
            }
        }

        private string mWorkHasAlreadyJob6;
        public string WorkHasAlreadyJob6
        {
            get
            {
                return mWorkHasAlreadyJob6;
            }
            set
            {
                mWorkHasAlreadyJob6 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob6");
            }
        }

        private string mWorkHasAlreadyJob7;
        public string WorkHasAlreadyJob7
        {
            get
            {
                return mWorkHasAlreadyJob7;
            }
            set
            {
                mWorkHasAlreadyJob7 = value;
                RaisePropertyChangedEvent("WorkHasAlreadyJob7");
            }
        }
        //

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

        private string mDayPic2;
        public string DayPic2
        {
            get
            {
                return mDayPic2;
            }
            set
            {
                mDayPic2 = value;
                RaisePropertyChangedEvent("DayPic2");
            }
        }

        private string mDayPic3;
        public string DayPic3
        {
            get
            {
                return mDayPic3;
            }
            set
            {
                mDayPic3 = value;
                RaisePropertyChangedEvent("DayPic3");
            }
        }

        private string mDayPic4;
        public string DayPic4
        {
            get
            {
                return mDayPic4;
            }
            set
            {
                mDayPic4 = value;
                RaisePropertyChangedEvent("DayPic4");
            }
        }

        private string mDayPic5;
        public string DayPic5
        {
            get
            {
                return mDayPic5;
            }
            set
            {
                mDayPic5 = value;
                RaisePropertyChangedEvent("DayPic5");
            }
        }

        private string mDayPic6;
        public string DayPic6
        {
            get
            {
                return mDayPic6;
            }
            set
            {
                mDayPic6 = value;
                RaisePropertyChangedEvent("DayPic6");
            }
        }

        private string mDayPic7;
        public string DayPic7
        {
            get
            {
                return mDayPic7;
            }
            set
            {
                mDayPic7 = value;
                RaisePropertyChangedEvent("DayPic7");
            }
        }
        #endregion DateOrNoX

        private EmployeeEntity _empl;
        public EmployeeEntity Empl
        {
            get
            {
                return _empl;
            }
            set
            {
                _empl = value;
                RaisePropertyChangedEvent("Empl");
            }
        }

        #region ComboBoxesValues
        private int _EmployeeId;
        public int EmployeeId
        {
            get { return _EmployeeId; }
            set
            {
                _EmployeeId = value;
                _empl = Employees.FirstOrDefault(e => e.Id.Equals(EmployeeId));
                ValidateAllRows();
                RaisePropertyChangedEvent("EmployeeId");
                SecondDateTime = SecondDateTime;
            }
        }

        private int _Job1Id;
        public int Job1Id
        {
            get { return _Job1Id; }
            set
            {
                _Job1Id = value;
                ValidateRow(1);
                Job2Id = Job1Id;
                Job3Id = Job1Id;
                RaisePropertyChangedEvent("Job1Id");
                ValidateEmployee1();
            }
        }

        private int _Job2Id;
        public int Job2Id
        {
            get { return _Job2Id; }
            set
            {
                _Job2Id = value;
                ValidateRow(2);
                Job4Id = Job2Id;
                RaisePropertyChangedEvent("Job2Id");

                ValidateEmployee2();
            }
        }

        private int _Job3Id;
        public int Job3Id
        {
            get { return _Job3Id; }
            set
            {
                _Job3Id = value;
                ValidateRow(3);
                Job5Id = Job3Id;
                RaisePropertyChangedEvent("Job3Id");
                ValidateEmployee3();
            }
        }

        private int _Job4Id;
        public int Job4Id
        {
            get { return _Job4Id; }
            set
            {
                _Job4Id = value;
                ValidateRow(4);
                RaisePropertyChangedEvent("Job4Id");
                ValidateEmployee4();
            }
        }

        private int _Job5Id;
        public int Job5Id
        {
            get { return _Job5Id; }
            set
            {
                _Job5Id = value;
                ValidateRow(5);
                RaisePropertyChangedEvent("Job5Id");
                ValidateEmployee5();
            }
        }

        private int _Job6Id;
        public int Job6Id
        {
            get { return _Job6Id; }
            set
            {
                _Job6Id = value;
                ValidateRow(6);
                RaisePropertyChangedEvent("Job6Id");
                ValidateEmployee6();
            }
        }

        private int _Job7Id;
        public int Job7Id
        {
            get { return _Job7Id; }
            set
            {
                _Job7Id = value;
                ValidateRow(7);
                RaisePropertyChangedEvent("Job7Id");
                ValidateEmployee7();
            }
        }
        #endregion ComboBoxesValues

        #region TimePickers
        #region Monday
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
                HourFrom2 = HourFrom1;
                HourFrom3 = HourFrom1;
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
                MinuteFrom2 = MinuteFrom1;
                MinuteFrom3 = MinuteFrom1;
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
                HourTo2 = HourTo1;
                HourTo3 = HourTo1;
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
                MinuteTo2 = MinuteTo1;
                MinuteTo3 = MinuteTo1;
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
                ValidateRow(1);
            }
        }
        #endregion Monday

        #region Tuesday
        private string mHourFrom2;
        public string HourFrom2
        {
            get
            {
                return mHourFrom2;
            }
            set
            {
                mHourFrom2 = value;
                IsOkDay2 = ValidateTime(HourFrom2, MinuteFrom2, HourTo2, MinuteTo2);
                HourFrom4 = HourFrom2;
                RaisePropertyChangedEvent("HourFrom2");
            }
        }

        private string mMinuteFrom2;
        public string MinuteFrom2
        {
            get
            {
                return mMinuteFrom2;
            }
            set
            {
                mMinuteFrom2 = value;
                IsOkDay2 = ValidateTime(HourFrom2, MinuteFrom2, HourTo2, MinuteTo2);
                MinuteFrom4 = MinuteFrom2;
                RaisePropertyChangedEvent("MinuteFrom2");
            }
        }

        private string mHourTo2;
        public string HourTo2
        {
            get
            {
                return mHourTo2;
            }
            set
            {
                mHourTo2 = value;
                IsOkDay2 = ValidateTime(HourFrom2, MinuteFrom2, HourTo2, MinuteTo2);
                HourTo4 = HourTo2;
                RaisePropertyChangedEvent("HourTo2");
            }
        }

        private string mMinuteTo2;
        public string MinuteTo2
        {
            get
            {
                return mMinuteTo2;
            }
            set
            {
                mMinuteTo2 = value;
                IsOkDay2 = ValidateTime(HourFrom2, MinuteFrom2, HourTo2, MinuteTo2);
                MinuteTo4 = MinuteTo2;
                RaisePropertyChangedEvent("MinuteTo2");
            }
        }

        private string mIsOkDay2;
        public string IsOkDay2
        {
            get
            {
                return mIsOkDay2;
            }
            set
            {
                mIsOkDay2 = value;
                RaisePropertyChangedEvent("IsOkDay2");
                ValidateRow(2);
            }
        }
        #endregion Tuesday

        #region Wednesday
        private string mHourFrom3;
        public string HourFrom3
        {
            get
            {
                return mHourFrom3;
            }
            set
            {
                mHourFrom3 = value;
                IsOkDay3 = ValidateTime(HourFrom3, MinuteFrom3, HourTo3, MinuteTo3);
                HourFrom5 = HourFrom3;
                RaisePropertyChangedEvent("HourFrom3");
            }
        }

        private string mMinuteFrom3;
        public string MinuteFrom3
        {
            get
            {
                return mMinuteFrom3;
            }
            set
            {
                mMinuteFrom3 = value;
                IsOkDay3 = ValidateTime(HourFrom3, MinuteFrom3, HourTo3, MinuteTo3);
                MinuteFrom5 = MinuteFrom3;
                RaisePropertyChangedEvent("MinuteFrom3");
            }
        }

        private string mHourTo3;
        public string HourTo3
        {
            get
            {
                return mHourTo3;
            }
            set
            {
                mHourTo3 = value;
                IsOkDay3 = ValidateTime(HourFrom3, MinuteFrom3, HourTo3, MinuteTo3);
                HourTo5 = HourTo3;
                RaisePropertyChangedEvent("HourTo3");
            }
        }

        private string mMinuteTo3;
        public string MinuteTo3
        {
            get
            {
                return mMinuteTo3;
            }
            set
            {
                mMinuteTo3 = value;
                IsOkDay3 = ValidateTime(HourFrom3, MinuteFrom3, HourTo3, MinuteTo3);
                MinuteTo5 = MinuteTo3;
                RaisePropertyChangedEvent("MinuteTo3");
            }
        }

        private string mIsOkDay3;
        public string IsOkDay3
        {
            get
            {
                return mIsOkDay3;
            }
            set
            {
                mIsOkDay3 = value;
                RaisePropertyChangedEvent("IsOkDay3");
                ValidateRow(3);
            }
        }
        #endregion Wednesday

        #region Thursday
        private string mHourFrom4;
        public string HourFrom4
        {
            get
            {
                return mHourFrom4;
            }
            set
            {
                mHourFrom4 = value;
                IsOkDay4 = ValidateTime(HourFrom4, MinuteFrom4, HourTo4, MinuteTo4);
                RaisePropertyChangedEvent("HourFrom4");
            }
        }

        private string mMinuteFrom4;
        public string MinuteFrom4
        {
            get
            {
                return mMinuteFrom4;
            }
            set
            {
                mMinuteFrom4 = value;
                IsOkDay4 = ValidateTime(HourFrom4, MinuteFrom4, HourTo4, MinuteTo4);
                RaisePropertyChangedEvent("MinuteFrom4");
            }
        }

        private string mHourTo4;
        public string HourTo4
        {
            get
            {
                return mHourTo4;
            }
            set
            {
                mHourTo4 = value;
                IsOkDay4 = ValidateTime(HourFrom4, MinuteFrom4, HourTo4, MinuteTo4);
                RaisePropertyChangedEvent("HourTo4");
            }
        }

        private string mMinuteTo4;
        public string MinuteTo4
        {
            get
            {
                return mMinuteTo4;
            }
            set
            {
                mMinuteTo4 = value;
                IsOkDay4 = ValidateTime(HourFrom4, MinuteFrom4, HourTo4, MinuteTo4);
                RaisePropertyChangedEvent("MinuteTo4");
            }
        }

        private string mIsOkDay4;
        public string IsOkDay4
        {
            get
            {
                return mIsOkDay4;
            }
            set
            {
                mIsOkDay4 = value;
                RaisePropertyChangedEvent("IsOkDay4");
                ValidateRow(4);
            }
        }
        #endregion Thursday

        #region Friday
        private string mHourFrom5;
        public string HourFrom5
        {
            get
            {
                return mHourFrom5;
            }
            set
            {
                mHourFrom5 = value;
                IsOkDay5 = ValidateTime(HourFrom5, MinuteFrom5, HourTo5, MinuteTo5);
                RaisePropertyChangedEvent("HourFrom5");
            }
        }

        private string mMinuteFrom5;
        public string MinuteFrom5
        {
            get
            {
                return mMinuteFrom5;
            }
            set
            {
                mMinuteFrom5 = value;
                IsOkDay5 = ValidateTime(HourFrom5, MinuteFrom5, HourTo5, MinuteTo5);
                RaisePropertyChangedEvent("MinuteFrom5");
            }
        }

        private string mHourTo5;
        public string HourTo5
        {
            get
            {
                return mHourTo5;
            }
            set
            {
                mHourTo5 = value;
                IsOkDay5 = ValidateTime(HourFrom5, MinuteFrom5, HourTo5, MinuteTo5);
                RaisePropertyChangedEvent("HourTo5");
            }
        }

        private string mMinuteTo5;
        public string MinuteTo5
        {
            get
            {
                return mMinuteTo5;
            }
            set
            {
                mMinuteTo5 = value;
                IsOkDay5 = ValidateTime(HourFrom5, MinuteFrom5, HourTo5, MinuteTo5);
                RaisePropertyChangedEvent("MinuteTo5");
            }
        }

        private string mIsOkDay5;
        public string IsOkDay5
        {
            get
            {
                return mIsOkDay5;
            }
            set
            {
                mIsOkDay5 = value;
                RaisePropertyChangedEvent("IsOkDay5");
                ValidateRow(5);
            }
        }
        #endregion Friday

        #region Saturday
        private string mHourFrom6;
        public string HourFrom6
        {
            get
            {
                return mHourFrom6;
            }
            set
            {
                mHourFrom6 = value;
                IsOkDay6 = ValidateTime(HourFrom6, MinuteFrom6, HourTo6, MinuteTo6);
                RaisePropertyChangedEvent("HourFrom6");
            }
        }

        private string mMinuteFrom6;
        public string MinuteFrom6
        {
            get
            {
                return mMinuteFrom6;
            }
            set
            {
                mMinuteFrom6 = value;
                IsOkDay6 = ValidateTime(HourFrom6, MinuteFrom6, HourTo6, MinuteTo6);
                RaisePropertyChangedEvent("MinuteFrom6");
            }
        }

        private string mHourTo6;
        public string HourTo6
        {
            get
            {
                return mHourTo6;
            }
            set
            {
                mHourTo6 = value;
                IsOkDay6 = ValidateTime(HourFrom6, MinuteFrom6, HourTo6, MinuteTo6);
                RaisePropertyChangedEvent("HourTo6");
            }
        }

        private string mMinuteTo6;
        public string MinuteTo6
        {
            get
            {
                return mMinuteTo6;
            }
            set
            {
                mMinuteTo6 = value;
                IsOkDay6 = ValidateTime(HourFrom6, MinuteFrom6, HourTo6, MinuteTo6);
                RaisePropertyChangedEvent("MinuteTo6");
            }
        }

        private string mIsOkDay6;
        public string IsOkDay6
        {
            get
            {
                return mIsOkDay6;
            }
            set
            {
                mIsOkDay6 = value;
                RaisePropertyChangedEvent("IsOkDay6");
                ValidateRow(6);
            }
        }
        #endregion Saturday

        #region Sunday
        private string mHourFrom7;
        public string HourFrom7
        {
            get
            {
                return mHourFrom7;
            }
            set
            {
                mHourFrom7 = value;
                IsOkDay7 = ValidateTime(HourFrom7, MinuteFrom7, HourTo7, MinuteTo7);
                RaisePropertyChangedEvent("HourFrom7");
            }
        }

        private string mMinuteFrom7;
        public string MinuteFrom7
        {
            get
            {
                return mMinuteFrom7;
            }
            set
            {
                mMinuteFrom7 = value;
                IsOkDay7 = ValidateTime(HourFrom7, MinuteFrom7, HourTo7, MinuteTo7);
                RaisePropertyChangedEvent("MinuteFrom7");
            }
        }

        private string mHourTo7;
        public string HourTo7
        {
            get
            {
                return mHourTo7;
            }
            set
            {
                mHourTo7 = value;
                IsOkDay7 = ValidateTime(HourFrom7, MinuteFrom7, HourTo7, MinuteTo7);
                RaisePropertyChangedEvent("HourTo7");
            }
        }

        private string mMinuteTo7;
        public string MinuteTo7
        {
            get
            {
                return mMinuteTo7;
            }
            set
            {
                mMinuteTo7 = value;
                IsOkDay7 = ValidateTime(HourFrom7, MinuteFrom7, HourTo7, MinuteTo7);
                RaisePropertyChangedEvent("MinuteTo7");
            }
        }

        private string mIsOkDay7;
        public string IsOkDay7
        {
            get
            {
                return mIsOkDay7;
            }
            set
            {
                mIsOkDay7 = value;
                RaisePropertyChangedEvent("IsOkDay7");
                ValidateRow(7);
            }
        }
        #endregion Thursday
        #endregion TimePickers

        public AssignWork(DateTime ChoosenDate, ObservableCollection<EmployeeEntity> Employees, ObservableCollection<JobEntity> Jobs, ObservableCollection<WorkEntity> Works)
        {
            InitializeComponent();
            this.DataContext = this;

            this.Employees = Employees;
            this.Jobs = Jobs;
            this.Works = Works;
            SortAndCreateComboBox();

            HourFrom1 = "5"; MinuteFrom1 = "00"; HourTo1 = "11"; MinuteTo1 = "00";
            HourFrom6 = "5"; MinuteFrom7 = "00"; HourTo6 = "11"; MinuteTo6 = "00";
            HourFrom7 = "5"; MinuteFrom6 = "00"; HourTo7 = "11"; MinuteTo7 = "00";

            Date = ChoosenDate;
        }

        private void SortAndCreateComboBox()
        {
            var List = new ObservableCollection<Employee>();
            foreach (var current in Employees.OrderBy(e => e.LastName))
            {
                List.Add(current);
            }
            People = new CollectionView(List);

            var List2 = new ObservableCollection<Job>();
            foreach (var current in Jobs.OrderBy(e => e.Name))
            {
                List2.Add(current);
            }
            JobTypes1 = new CollectionView(List2);
            JobTypes2 = new CollectionView(List2);
            JobTypes3 = new CollectionView(List2);
            JobTypes4 = new CollectionView(List2);
            JobTypes5 = new CollectionView(List2);
            JobTypes6 = new CollectionView(List2);
            JobTypes7 = new CollectionView(List2);
        }

        private void NullActiveDays()
        {
            IsActiveDay1 = false;
            IsActiveDay2 = false;
            IsActiveDay3 = false;
            IsActiveDay4 = false;
            IsActiveDay5 = false;
            IsActiveDay6 = false;
            IsActiveDay7 = false;
            DateOrNo1 = NO_MSG; DateOrNo2 = NO_MSG;
            DateOrNo3 = NO_MSG; DateOrNo4 = NO_MSG;
            DateOrNo5 = NO_MSG; DateOrNo6 = NO_MSG;
            DateOrNo7 = NO_MSG;
            WorkHasAlreadyEmployee1 = ""; WorkHasAlreadyEmployee2 = "";
            WorkHasAlreadyEmployee3 = ""; WorkHasAlreadyEmployee4 = "";
            WorkHasAlreadyEmployee5 = ""; WorkHasAlreadyEmployee6 = "";
            WorkHasAlreadyEmployee7 = "";
        }

        #region RaiseProperty
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion RaiseProperty

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

        private string ValidateRow(string DayX, int JobId, string IsOkDayX)
        {
            if (DayX == NO_MSG)
            {
                return PIC_DISABLE;
            }
            else
            {
                if (IsOkDayX == TIME_ERROR)
                {
                    return PIC_ERROR;
                }
                else
                {
                    if (this.Empl == null)
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
            }
        }

        private void ValidateAllRows()
        {
            ValidateRow(1);
            ValidateRow(2);
            ValidateRow(3);
            ValidateRow(4);
            ValidateRow(5);
            ValidateRow(6);
            ValidateRow(7);
        }

        private void ValidateRow(int i)
        {
            switch (i)
            {
                case 1:
                    DayPic1 = ValidateRow(DateOrNo1, Job1Id, IsOkDay1);
                    break;
                case 2:
                    DayPic2 = ValidateRow(DateOrNo2, Job2Id, IsOkDay2);
                    break;
                case 3:
                    DayPic3 = ValidateRow(DateOrNo3, Job3Id, IsOkDay3);
                    break;
                case 4:
                    DayPic4 = ValidateRow(DateOrNo4, Job4Id, IsOkDay4);
                    break;
                case 5:
                    DayPic5 = ValidateRow(DateOrNo5, Job5Id, IsOkDay5);
                    break;
                case 6:
                    DayPic6 = ValidateRow(DateOrNo6, Job6Id, IsOkDay6);
                    break;
                case 7:
                    DayPic7 = ValidateRow(DateOrNo7, Job7Id, IsOkDay7);
                    break;
            }
        }

        #region ValidateEmployee
        private void ValidateEmployee1()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job1Id));
            if (job != null)
            {
                if (DateOrNo1 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo1));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob1 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo1);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob1 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee2()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job2Id));
            if (job != null)
            {
                if (DateOrNo2 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo2));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob2 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo2);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob2 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee3()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job3Id));
            if (job != null)
            {
                if (DateOrNo3 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo3));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob3 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo3);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob3 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee4()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job4Id));
            if (job != null)
            {
                if (DateOrNo4 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo4));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob4 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo4);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob4 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee5()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job5Id));
            if (job != null)
            {
                if (DateOrNo5 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo5));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob5 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo5);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob5 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee6()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job6Id));
            if (job != null)
            {
                if (DateOrNo6 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo6));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob6 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo6);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob6 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }

        private void ValidateEmployee7()
        {
            JobEntity job = Jobs.FirstOrDefault(j => j.Id.Equals(Job7Id));
            if (job != null)
            {
                if (DateOrNo7 != NO_MSG)
                {
                    job.filter(Convert.ToDateTime(DateOrNo7));
                    if (job.SetOfWorkingPeopleAsString == "")
                    {
                        WorkHasAlreadyJob7 = "";
                    }
                    else
                    {
                        var _date = Convert.ToDateTime(DateOrNo7);
                        var output = new StringBuilder();
                        foreach (var currentJob in job.WorksEntity.Where(w => w.From.Date.Equals(_date.Date)))
                        {
                            output.Append(BuildString(currentJob));
                        }
                        WorkHasAlreadyJob7 = "Na tejto pozícii už robia: " + output.ToString();
                    }
                }
            }
        }
        #endregion ValidateEmployee

        private void ChangeColor(int i)
        {
            switch (i)
            {
                case 1:
                    if (DateOrNo1 == NO_MSG) ColorDay1 = COLOR_PASSIVE;
                    else ColorDay1 = COLOR_ACTIVE;
                    break;
                case 2:
                    if (DateOrNo2 == NO_MSG) ColorDay2 = COLOR_PASSIVE;
                    else ColorDay2 = COLOR_ACTIVE;
                    break;
                case 3:
                    if (DateOrNo3 == NO_MSG) ColorDay3 = COLOR_PASSIVE;
                    else ColorDay3 = COLOR_ACTIVE;
                    break;
                case 4:
                    if (DateOrNo4 == NO_MSG) ColorDay4 = COLOR_PASSIVE;
                    else ColorDay4 = COLOR_ACTIVE;
                    break;
                case 5:
                    if (DateOrNo5 == NO_MSG) ColorDay5 = COLOR_PASSIVE;
                    else ColorDay5 = COLOR_ACTIVE;
                    break;
                case 6:
                    if (DateOrNo6 == NO_MSG) ColorDay6 = COLOR_PASSIVE;
                    else ColorDay6 = COLOR_ACTIVE;
                    break;
                case 7:
                    if (DateOrNo7 == NO_MSG) ColorDay7 = COLOR_PASSIVE;
                    else ColorDay7 = COLOR_ACTIVE;
                    break;
            }
        }

        private void EndWork(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            int count = 0;
            if (mDayPic1 == PIC_YES && DateOrNo1 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job1Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo1, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom1), Convert.ToInt32(MinuteFrom1), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo1), Convert.ToInt32(MinuteTo1), 0);
                var work = new Work();

                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job1Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic2 == PIC_YES && DateOrNo2 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job2Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo2, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom2), Convert.ToInt32(MinuteFrom2), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo2), Convert.ToInt32(MinuteTo2), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job2Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic3 == PIC_YES && DateOrNo3 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job3Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo3, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom3), Convert.ToInt32(MinuteFrom3), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo3), Convert.ToInt32(MinuteTo3), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job3Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic4 == PIC_YES && DateOrNo4 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job4Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo4, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom4), Convert.ToInt32(MinuteFrom4), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo4), Convert.ToInt32(MinuteTo4), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job4Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic5 == PIC_YES && DateOrNo5 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job2Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo5, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom5), Convert.ToInt32(MinuteFrom5), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo5), Convert.ToInt32(MinuteTo5), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job5Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic6 == PIC_YES && DateOrNo6 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job6Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo6, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom6), Convert.ToInt32(MinuteFrom6), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo6), Convert.ToInt32(MinuteTo6), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job6Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (mDayPic7 == PIC_YES && DateOrNo7 != NO_MSG)
            {
                var JobChoosen = Jobs.FirstOrDefault(element => element.Id.Equals(Job7Id));
                var FirstDateChoosen = DateTime.ParseExact(DateOrNo7, PARSE_DATE, System.Globalization.CultureInfo.InvariantCulture);
                var DateFrom = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourFrom7), Convert.ToInt32(MinuteFrom7), 0);
                var DateTo = new DateTime(FirstDateChoosen.Year, FirstDateChoosen.Month, FirstDateChoosen.Day,
                    Convert.ToInt32(HourTo7), Convert.ToInt32(MinuteTo7), 0);

                var work = new Work();
                work.EmployeeId = Empl.Id;
                work.From = DateFrom;
                work.To = DateTo;
                work.JobId = Job7Id;
                using (var db = new ModelContainer())
                {
                    db.Works.Add(work);
                    db.SaveChanges();
                    count++;
                }
            }

            if (count != 0) MessageBox.Show(Empl.FirstName + " " + Empl.LastName + " :: úspešne vložené " + count + " záznamov");
            EndWork(sender, e);
        }

        private string BuildString(Work currentJob)
        {
            var output = new StringBuilder();
            output.Append(currentJob.From.ToString("HH:mm"));
            output.Append(" - ");
            output.Append(currentJob.To.ToString("HH:mm"));
            output.Append(" ");
            output.Append(currentJob.Employee.LastName);
            output.Append(" ");
            output.Append(currentJob.Employee.FirstName);
            output.Append(" ||| ");
            return output.ToString();
        }

        private string GenerateHasAlreadyWorkString(EmployeeEntity empl, DateTime day)
        {
            //var empl = new EmployeeEntity(Empl, true);
            empl.filter(day);
            if (empl.SetOfWorkAsString == "")
            {
                return "";
            }
            else
            {
                return empl + " už robí: " + empl.SetOfWorkAsString;
            }
        }
    }
}
