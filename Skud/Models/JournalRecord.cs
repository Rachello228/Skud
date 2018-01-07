using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class JournalRecord
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public long Employee { get; set; }
        public DateTime In { get; set; }
        public DateTime? Out { get; set; }

    }

    public class ExtRecord
    {
        public string FIO { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public int Time { get; set; }
        public string Date { get; set; }
    }
}
