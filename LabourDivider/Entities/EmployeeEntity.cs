using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourDivider.Entities
{
    public class EmployeeEntity : Employee, IComparable<EmployeeEntity>
    {
        public const string Divider = " ||| ";

        public virtual string SetOfWorkAsString { get; set; }

        //private ICollection<WorkEntity> mWorksEntity;
        public virtual ICollection<WorkEntity> WorksEntity { get; set; }

        public EmployeeEntity()
        {
            this.WorksEntity = new HashSet<WorkEntity>();
        }

        public EmployeeEntity(Employee employee)
        {
            WorksEntity = new HashSet<WorkEntity>();

            if (employee != null)
            {
                Id = employee.Id;
                FirstName = employee.FirstName;
                LastName = employee.LastName;
                //    //if(employee.Works != null)
                //    //{
                //    //    foreach (var current in employee.Works)
                //    //        WorksEntity.Add(new WorkEntity(current));
                //    //}
            }
        }

        public void filter(DateTime filterDate)
        {
            SortedSet<WorkEntity> works = new SortedSet<WorkEntity>(WorksEntity);
            StringBuilder people = new StringBuilder();
            if (filterDate == null)
            {
                foreach (var currentWork in works)
                {
                    people.Append(currentWork.ToJobString());
                    people.Append(Divider);
                }
            }
            else
            {
                foreach (var currentWork in works)
                {
                    if (currentWork.From.Date.Equals(filterDate.Date))
                    {
                        people.Append(currentWork.ToJobString());
                        people.Append(Divider);
                    }
                }
            }
            SetOfWorkAsString = people.ToString();
        }

        public void filter(DateTime from, DateTime to)
        {
            if (from != null && to != null)
            {
                SortedSet<WorkEntity> works = new SortedSet<WorkEntity>(WorksEntity);
                StringBuilder people = new StringBuilder();
                foreach (var currentWork in works)
                {
                    if (currentWork.From.Date >= from.Date && currentWork.From.Date <= to.Date)
                    {
                        people.Append(currentWork.From.ToString("dd.MM."));
                        people.Append(" ");
                        people.Append(currentWork.ToJobString());
                        people.Append(Divider);
                    }
                }
                SetOfWorkAsString = people.ToString();
            }
        }

        public override string ToString()
        {
            return LastName + " " + FirstName;
        }

        int IComparable<EmployeeEntity>.CompareTo(EmployeeEntity empl)
        {
            return this.LastName.CompareTo(empl.LastName);
        }
    }
}
