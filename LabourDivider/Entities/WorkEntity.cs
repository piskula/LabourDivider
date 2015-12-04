using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourDivider.Entities
{
    public class WorkEntity : Work, IComparable<WorkEntity>
    {
        private DateTime _From;
        public override System.DateTime From
        {
            get
            {
                return _From;
            }
            set
            {
                _From = value;
                if (To != null)
                {
                    Duration = (To - From);
                }
            }
        }
        private DateTime _To;
        public override System.DateTime To
        {
            get
            {
                return _To;
            }
            set
            {
                _To = value;
                if (From != null)
                {
                    Duration = (To - From);
                }
            }
        }

        public TimeSpan Duration { get; set; }

        public WorkEntity(Work work)
        {
            if(work != null)
            {
                Id = work.Id;
                From = work.From;
                To = work.To;
                EmployeeId = work.EmployeeId;
                JobId = work.JobId;
            }
            
            Employee = new EmployeeEntity(work.Employee);
            Job = new JobEntity(work.Job);
        }

      
        public override string ToString()
        {
            var people = new StringBuilder();
            people.Append(From.ToString("HH:mm"));
            people.Append(" - ");
            people.Append(To.ToString("HH:mm"));
            people.Append(" = ");
            people.Append(Employee.LastName);
            people.Append(" ");
            people.Append(Employee.FirstName);

            return people.ToString();
        }
        public string ToJobString()
        {
            var jobs = new StringBuilder();
            jobs.Append(From.ToString("HH:mm"));
            jobs.Append(" - ");
            jobs.Append(To.ToString("HH:mm"));
            jobs.Append(" = ");
            jobs.Append(Job.Name);
            return jobs.ToString();
        }

        int IComparable<WorkEntity>.CompareTo(WorkEntity work)
        {
            return this.From.CompareTo(work.From);
        }
    }
}
