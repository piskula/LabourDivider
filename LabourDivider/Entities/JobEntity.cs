using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourDivider.Entities
{
    public class JobEntity : Job, IComparable<JobEntity>
    {
        public const string Divider = " ||| ";

        public virtual string SetOfWorkingPeopleAsString { get; set; }
        public virtual ICollection<WorkEntity> WorksEntity { get; set; }

        public JobEntity()
        {
            WorksEntity = new HashSet<WorkEntity>();
        }

        public JobEntity(Job job)
        {
            WorksEntity = new HashSet<WorkEntity>();

            if (job != null)
            {
                Id = job.Id;
                Name = job.Name;
                //if (job.Works != null && restore)
                //{
                //    foreach (var current in job.Works)
                //        WorksEntity.Add(new WorkEntity(current));
                //}
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
                    people.Append(currentWork);
                    people.Append(Divider);
                }
            }
            else
            {
                foreach (var currentWork in works)
                {
                    if (currentWork.From.Date.Equals(filterDate.Date))
                    {
                        people.Append(currentWork);
                        people.Append(Divider);
                    }
                }
            }
            SetOfWorkingPeopleAsString = people.ToString();
        }

        public void filterToAdding(DateTime filterDate)
        {
            SortedSet<WorkEntity> works = new SortedSet<WorkEntity>(WorksEntity);
            StringBuilder people = new StringBuilder();
            if (filterDate == null)
            {
                foreach (var currentWork in works)
                {
                    people.Append("tam už robí ");
                    people.Append(currentWork);
                    people.Append(Divider);
                }
            }
            else
            {
                foreach (var currentWork in works)
                {
                    if (currentWork.From.Date.Equals(filterDate.Date))
                    {
                        people.Append("tam už robí ");
                        people.Append(currentWork);
                        people.Append(Divider);
                    }
                }
            }
            SetOfWorkingPeopleAsString = people.ToString();
        }

        public void filter(DateTime from, DateTime to)
        {
            if (from != null && to != null)
            {
                SortedSet<Work> works = new SortedSet<Work>(WorksEntity);
                StringBuilder people = new StringBuilder();
                foreach (var currentWork in works)
                {
                    if (currentWork.From.Date >= from.Date && currentWork.From.Date <= to.Date)
                    {
                        people.Append(currentWork.From.ToString("dd.MM."));
                        people.Append(" ");
                        people.Append(currentWork);
                        people.Append(Divider);
                    }
                }
                SetOfWorkingPeopleAsString = people.ToString();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        int IComparable<JobEntity>.CompareTo(JobEntity job)
        {
            return this.Name.CompareTo(job.Name);
        }
    }
}
