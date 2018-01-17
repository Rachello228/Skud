using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skud
{
    public partial class Employees : Form
    {
        JournalContext context;
        Employee emp;
        bool fileSelect = false;
        bool filedelete = false;
        public Employees(JournalContext context)
        {
            InitializeComponent();
            this.context = context;
            comboBox1.DisplayMember = "JobDescription";
            comboBox1.ValueMember = "JobId";
            comboBox2.DisplayMember = "FullName";
            comboBox2.ValueMember = "Id";
            comboBox1.DataSource = context.Jobs.Local;
            comboBox2.DataSource = context.Employees.Local;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                emp = (Employee)comboBox2.SelectedItem;
                textBox4.Text = emp.CardId.ToString();
                textBox1.Text = emp.Surname;
                textBox2.Text = emp.Name;
                textBox3.Text = emp.Patronymic;
                comboBox1.SelectedItem = emp.Job;
                if (emp.Photo != null)
                    pictureBox1.Image = emp.GetImage;
                else
                    pictureBox1.Image = Properties.Resources.specialist_userpng;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                emp.Job = (Job)comboBox1.SelectedItem;
                emp.Name = textBox2.Text;
                emp.Surname = textBox1.Text;
                emp.Patronymic = textBox3.Text;
                emp.CardId = long.Parse(textBox4.Text);
                if (fileSelect)
                    emp.Photo = ConvertToByteArray(pictureBox1.Image);
                if (filedelete)
                    emp.Photo = null;
                context.SaveChanges();
                //comboBox2.DataSource = null;
                //comboBox2.DataSource = context.Employees.Local;
                //comboBox2.DisplayMember = "FullName";
                //comboBox2.ValueMember = "Id";
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("При удалении сотрудника удалятся все его записи в журнале, продолжить?", "СКУД", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                context.Employees.Remove(emp);
                context.SaveChanges();
                //comboBox2.DataSource = null;
                //comboBox2.DataSource = context.Employees.Local;
                //comboBox2.DisplayMember = "FullName";
                //comboBox2.ValueMember = "Id";
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog1.FileName);
                fileSelect = true;
                filedelete = false;
            }
        }

        private void улалитьФотоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Skud.Properties.Resources.specialist_userpng;
            fileSelect = false;
            filedelete = true;
        }
    }
}
