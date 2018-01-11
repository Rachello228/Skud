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
    public partial class EmployeeInfo : Form
    {
        Employee emp;
        Timer timer;
        public EmployeeInfo(Employee employee, string status)
        {
            try
            {
                this.emp = employee;
                InitializeComponent();
                label6.Text = emp.Surname;
                label7.Text = emp.Name;
                label8.Text = emp.Patronymic;
                label9.Text = employee.Job.JobDescription;
                label10.Text = status;
                label10.ForeColor = status == "Вход" ? Color.Green : Color.Red;
                if (emp.Photo != null)
                    pictureBox1.Image = employee.GetImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                timer = new Timer() { Interval = 3000 };
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
