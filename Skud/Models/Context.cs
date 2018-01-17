using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class JournalContext : DbContext
    {
        public JournalContext(string connString) : base(connString)
        {
            Database.SetInitializer(new JournalContextInitializer());
        }
        public DbSet<JournalRecord> JournalRecors { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Job> Jobs { get; set; }

    }
    public class JournalContextInitializer : DropCreateDatabaseIfModelChanges<JournalContext>
    {
        protected override void Seed(JournalContext context)
        {
            IList<Job> defaulJobs = new List<Job>();

            defaulJobs.Add(new Job() { JobDescription = "порашист" });
            defaulJobs.Add(new Job() { JobDescription = "дворник" });
            defaulJobs.Add(new Job() { JobDescription = "продавец" });
            defaulJobs.Add(new Job() { JobDescription = "учитель" });

            foreach (Job std in defaulJobs)
                context.Jobs.Add(std);

            base.Seed(context);
        }
    }
}
