using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class Context : DbContext
    {
        public Context(string conString) : base(conString) { }
        public DbSet<JournalRecord> Journal { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Job> Jobs { get; set; }

    }
}
