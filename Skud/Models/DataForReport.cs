using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skud
{
    public class DataForReport : List<ExtRecord>
    {
        public DataForReport(JournalContext context, DateTime from, DateTime to)
        {
            GetData(context.JournalRecors.Where(j => j.Date >= from.Date.Date && j.Date <= to.Date.Date).ToList(), context);
        }
        public DataForReport(JournalContext context)
        {
            DateTime first = DateTime.Now.First();
            DateTime last = DateTime.Now.Last(); 
            GetData(context.JournalRecors.Where(j => j.Date >= first.Date && j.Date <= last.Date).ToList(), context);
        }

        public DataForReport(JournalContext context, Employee employee)
        {
            GetData(context.JournalRecors.Where(j => j.Employee == employee).ToList(), context);
        }

        public DataForReport(JournalContext context, Employee employee, DateTime from, DateTime to)
        {
            GetData(context.JournalRecors.Where(j => j.Date >= from.Date.Date && j.Date <= to.Date.Date && j.Employee == employee).ToList(), context);
        }

        public void GetData(List<JournalRecord> records, JournalContext context)
        {
            foreach (JournalRecord rec in records)
            {
                ExtRecord extendRec = new ExtRecord();
                extendRec.Date = rec.Date.ToShortDateString();
                extendRec.Image = rec.Employee.Photo != null ? rec.Employee.Photo : Properties.Resources.specialist_user.ToByteArray(ImageFormat.Bmp);
                extendRec.FIO = rec.Employee.FullName;
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
    static class DataExt
    {
        public static DateTime First(this DateTime current)
        {
            DateTime first = current.AddDays(1 - current.Day);
            return first;
        }

        public static DateTime Last(this DateTime current)
        {
            int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

            DateTime last = current.First().AddDays(daysInMonth - 1);
            return last;
        }
    }
}
