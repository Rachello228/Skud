using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skud
{
    public class Employee
    {
        public int Id { get; set; }
        public long CardId { get; set; }
        public byte[] Photo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Patronymic { get; set; }
        [NotMapped]
        public string FullName { get { return Surname + " " + Name + " " + Patronymic; } }
        [ForeignKey("Job")]
        public int JobId { get; set; }
        public Job Job { get; set; }

        public ICollection<JournalRecord> JournalRecords { get; set; }
        [NotMapped]
        public Image GetImage { get { return byteArrayToImage(Photo); } }
        public Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }
    }
}
