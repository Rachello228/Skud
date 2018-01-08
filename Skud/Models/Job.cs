using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class Job
    {
        public int JobId { get; set; }
        public string JobDescription { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
