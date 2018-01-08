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
    public partial class Emploees : Form
    {
        Context context;
        Employee emp;
        bool fileSelect = false;
        bool filedelete = false;
        public Emploees(Context context)
        {
            InitializeComponent();
            this.context = context;
            comboBox2.DataSource = context.Employees.Local;
            comboBox1.DataSource = context.Jobs.Local;
            comboBox1.DisplayMember = "JobDescription";
            comboBox1.ValueMember = "JobId";
            comboBox2.DisplayMember = "FullName";
            comboBox2.ValueMember = "Id";
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
                comboBox1.SelectedValue = emp.JobId;
                if (emp.Photo != null)
                    pictureBox1.Image = emp.GetImage;
                else
                    pictureBox1.Image = Properties.Resources.specialist_user;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                emp.JobId = (int)comboBox1.SelectedValue;
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
            pictureBox1.Image = Skud.Properties.Resources.specialist_user;
            fileSelect = false;
            filedelete = true;
        }
    }
}
