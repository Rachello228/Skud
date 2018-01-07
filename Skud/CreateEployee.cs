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

namespace Skud
{
    public partial class CreateEployee : Form
    {
        Context context;
        string path = String.Empty;
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
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Images"))
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Images");
                if (!string.IsNullOrEmpty(path))
                    File.Copy(openFileDialog1.FileName, Directory.GetCurrentDirectory() + @"\Images\" + openFileDialog1.SafeFileName);
                emp.Photo = path;
                context.Employees.Add(emp);
                context.SaveChanges();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.SafeFileName;
                pictureBox1.Load(openFileDialog1.FileName);
            }
        }
    }
}
