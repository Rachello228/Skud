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
    public partial class Report : Form
    {
        JournalContext context;
        public Report(JournalContext context)
        {
            InitializeComponent();
            this.context = context;
        }

        private void Report_Load(object sender, EventArgs e)
        {
            this.dataForReportBindingSource.DataSource = new DataForReport(context);
            this.comboBox1.DataSource = context.Employees.Local;
            this.comboBox1.ValueMember = "Id";
            this.comboBox1.DisplayMember = "FullName";
            this.reportViewer1.RefreshReport();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.dataForReportBindingSource.DataSource = new DataForReport(context);
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked)
                this.dataForReportBindingSource.DataSource = new DataForReport(context, (int)comboBox1.SelectedValue, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
            else if (checkBox1.Checked)
                this.dataForReportBindingSource.DataSource = new DataForReport(context, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
            else if (checkBox2.Checked)
                this.dataForReportBindingSource.DataSource = new DataForReport(context, (int)comboBox1.SelectedValue);
            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_ReportRefresh(object sender, CancelEventArgs e)
        {
            this.dataForReportBindingSource.DataSource = new DataForReport(context);
            this.reportViewer1.RefreshReport();
        }
    }
}
