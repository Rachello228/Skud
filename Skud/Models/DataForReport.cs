using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class DataForReport : List<ExtRecord>
    {
        public DataForReport(Context context, DateTime from, DateTime to)
        {
            GetData(context.Journal.Where(j => DbFunctions.TruncateTime(j.Date) >= from.Date.Date && DbFunctions.TruncateTime(j.Date) <= to.Date.Date).ToList(), context);
        }
        public DataForReport(Context context)
        {
            GetData(context.Journal.ToList(), context);
        }

        public DataForReport(Context context, long employee)
        {
            GetData(context.Journal.Where(j => j.Employee == employee).ToList(), context);
        }

        public DataForReport(Context context, long employee, DateTime from, DateTime to)
        {
            GetData(context.Journal.Where(j => DbFunctions.TruncateTime(j.Date) >= from.Date.Date && DbFunctions.TruncateTime(j.Date) <= to.Date.Date && j.Employee == employee).ToList(), context);
        }

        public void GetData(List<JournalRecord> records, Context context)
        {
            foreach (JournalRecord rec in records)
            {
                Employee emp = context.Employees.Find(rec.Employee);
                ExtRecord extendRec = new ExtRecord();
                extendRec.Date = rec.Date.ToShortDateString();
                extendRec.FIO = emp.FullName;
                extendRec.In = rec.In.ToString(@"HH\:mm");
                extendRec.Out = rec.Out.HasValue ? rec.Out.Value.ToString(@"HH\:mm") : null;
                if (!rec.Out.HasValue)
                    extendRec.Time = 0;
                else
                    extendRec.Time = (int)rec.Out.Value.TimeOfDay.TotalMinutes - (int)rec.In.TimeOfDay.TotalMinutes + (rec.Out.Value.Date - rec.In.Date).Days * 1440;
                Add(extendRec);
            }
        }
    }
}
