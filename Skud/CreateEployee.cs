using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.IO;
using System.Drawing.Imaging;
using System.Data.Entity.Validation;

namespace Skud
{
    public partial class CreateEployee : Form
    {
        Context context;
        bool fileSelect = false;
        public CreateEployee(Context context)
        {
            InitializeComponent();
            this.context = context;
            comboBox1.DataSource = context.Jobs.Local;
            comboBox1.DisplayMember = "JobDescription";
            comboBox1.ValueMember = "JobId";
        }
        public CreateEployee(long id, Context context)
        {
            InitializeComponent();
            this.context = context;
            textBox4.Text = id.ToString();
            textBox4.Enabled = false;
            comboBox1.DataSource = context.Jobs.Local;
            comboBox1.DisplayMember = "JobDescription";
            comboBox1.ValueMember = "JobId";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Employee emp = new Employee();
                emp.CardId = long.Parse(textBox4.Text);
                emp.Job = (int)comboBox1.SelectedValue;
                emp.Name = textBox2.Text;
                emp.Surname = textBox1.Text;
                emp.Patronymic = textBox3.Text;
                if (fileSelect)
                    emp.Photo = ConvertToByteArray(Image.FromFile(openFileDialog1.FileName));
                if ((!fileSelect && MessageBox.Show("Вы не выбрали фото, продолжить?", "СКУД", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) || fileSelect)
                {
                    context.Employees.Add(emp);
                    context.SaveChanges();
                    this.Close();
                }
            }
            catch(DbEntityValidationException)
            {
                MessageBox.Show("Заполните ФИО");
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.ToString());
            }
        }

        public byte[] ConvertToByteArray(Image img)
        {
            byte[] ret = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    ret = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return ret;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog1.FileName);
                fileSelect = true;
            }
        }
    }
}
