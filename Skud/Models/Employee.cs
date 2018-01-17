using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
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
        public bool Status { get; set; } = false;

        public ICollection<JournalRecord> JournalRecords { get; set; }
        [NotMapped]
        public Image GetImage { get { return Photo.ToImage(); } }

    }
    public static class ImageExtensions
    {
        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
    public static class ByteArrayExtensions
    {
        public static Image ToImage(this byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }
    }
}
