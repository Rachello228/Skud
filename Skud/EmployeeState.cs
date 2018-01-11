using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skud
{
    public partial class EmployeeState : Form
    {
        JournalContext context;
        public EmployeeState(JournalContext context)
        {
            InitializeComponent();
            this.context = context;
            comboBox1.DisplayMember = "FullName";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = context.Employees.Local;
            comboBox1.SelectedIndex = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            context.Journal.Add(new JournalRecord() { Date = DateTime.Now, EmployeeId = 5, In = DateTime.Now, Out = DateTime.Now });
            context.SaveChanges();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = context.Journal.Local;
        }
        
        public void LoadData(Employee empl = null)
        {
            dataGridView1.Rows.Clear();
            if (empl == null)
            {
                foreach (Employee emp in context.Employees.Local)
                {
                    dataGridView1.Rows.Add(new string[] { emp.Surname, emp.Name, emp.Patronymic, emp.Job.JobDescription, emp.Status.ToString() });
                }
            }
            else
                dataGridView1.Rows.Add(new string[] { empl.Surname, empl.Name, empl.Patronymic, empl.Job.JobDescription, empl.Status.ToString() });
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[4].Style.BackColor =  row.Cells[4].Value.ToString() == false.ToString() ? Color.Red : Color.Green;
                row.Cells[4].Value = row.Cells[4].Value.ToString() == false.ToString() ? "Нет на месте" : "На месте";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadData(context.Employees.Find(((sender as ComboBox).SelectedItem as Employee).Id));
            }
            catch(NullReferenceException)
            {
                LoadData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
