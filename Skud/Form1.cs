using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Sql;
using Microsoft.SqlServer.Management.Smo;
using System.Data.Entity;
using System.IO;
using Microsoft.VisualBasic;

namespace Skud
{
    public partial class Form1 : Form
    {
        SerialPort selectedPort;
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        SqlConnectionStringBuilder sqlConnection;
        Context context;
        public Form1()
        {
            InitializeComponent();
            try
            {
                if (!config.AppSettings.Settings.AllKeys.Contains("SqlServerName"))
                {
                    string SqlServerName = string.Empty;
                    bool IsConnected = false;
                    while(!IsConnected)
                    {
                        //DataTable table = SmoApplication.EnumAvailableSqlServers(true);
                        SqlServerName = Interaction.InputBox("Введите имя SQL сервера", "СКУД", "");
                        sqlConnection = new SqlConnectionStringBuilder();
                        sqlConnection.DataSource = SqlServerName;
                        sqlConnection.InitialCatalog = "Journal";
                        sqlConnection.IntegratedSecurity = true;
                        IsConnected = IsServerConnected(sqlConnection.ConnectionString); 
                    }
                    config.AppSettings.Settings.Add("SqlServerName", SqlServerName);
                    config.Save(ConfigurationSaveMode.Modified);
                }
                else
                {
                    sqlConnection = new SqlConnectionStringBuilder();
                    sqlConnection.DataSource = config.AppSettings.Settings["SqlServerName"].Value;
                    sqlConnection.InitialCatalog = "Journal";
                    sqlConnection.IntegratedSecurity = true;                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            context = new Context(sqlConnection.ConnectionString);
            context.Employees.Load();
            context.Journal.Load();
            context.Jobs.Load();
            GetAllSerialPorts();
        }
        private static bool IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public void GetAllSerialPorts()
        {
            foreach (String port in SerialPort.GetPortNames())
            {
                ToolStripMenuItem comPort = new ToolStripMenuItem();
                comPort.Text = port;
                comPort.Checked = config.AppSettings.Settings.AllKeys.Contains("SerialPort") && config.AppSettings.Settings["SerialPort"].Value == port ? true : false;
                comPort.Click += comPort_Click;
                PortToolStripMenuItem.DropDownItems.Add(comPort);
            }
        }

        private void comPort_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
                foreach (ToolStripMenuItem item in PortToolStripMenuItem.DropDownItems)
                {
                    item.Checked = false;
                }
                selectedItem.Checked = true;
                config.AppSettings.Settings.Remove("SerialPort");
                config.AppSettings.Settings.Add("SerialPort", selectedItem.Text);
                config.Save(ConfigurationSaveMode.Modified);
                if (selectedPort != null && selectedPort.IsOpen)
                    selectedPort.Close();
                ReadFromController(selectedItem.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            int a = 12;
            if (config.AppSettings.Settings.AllKeys.Contains("SerialPort") && config.AppSettings.Settings["SerialPort"].Value.Length > 0)
                ReadFromController(config.AppSettings.Settings["SerialPort"].Value);
            else
                MessageBox.Show("Устройство не настроено, выберите com port", "СКУД", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ReadFromController(string portName)
        {
            try
            {
                selectedPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                selectedPort.DataReceived += SerialPortDataReceived;
                selectedPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                long CardId = long.Parse(selectedPort.ReadExisting());
                Employee employee = context.Employees.Find(CardId);
                if (employee == null)
                {
                    CreateEployee emp = new CreateEployee(CardId, context);
                    emp.ShowDialog();
                }
                else
                {
                    string status = AddJournalRecord(employee.CardId);
                    string job = context.Jobs.Find(employee.Job).JobDescription;
                    EmployeeInfo info = new EmployeeInfo(employee, status, job);
                    info.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Test(string id)
        {
            try
            {
                long CardId = long.Parse(id);
                Employee employee = context.Employees.Find(CardId);
                if (employee == null)
                {
                    CreateEployee emp = new CreateEployee(CardId, context);
                    emp.ShowDialog();
                }
                else
                {
                    string status = AddJournalRecord(employee.CardId);
                    string job = context.Jobs.Find(employee.Job).JobDescription;
                    EmployeeInfo info = new EmployeeInfo(employee, status, job);
                    info.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string AddJournalRecord(long CardId)
        {
            string status = string.Empty;
            try
            {
                JournalRecord record = new JournalRecord();
                record = context.Journal.Where(j => j.Employee == CardId).ToList().LastOrDefault();
                if(record == null)
                {
                    context.Journal.Add(new JournalRecord() { Employee = CardId, Date = DateTime.Now.Date, In = DateTime.Now });
                    status = "Вход";
                }
                else if (record.Out != null) // сотрудник вышел
                {
                    context.Journal.Add(new JournalRecord() { Employee = CardId, Date = DateTime.Now.Date, In = DateTime.Now }); // вход
                    status = "Вход";
                }
                else // сотрудник вошел
                {
                    record.Out = DateTime.Now; // выход
                    status = "Выход";
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return status;
        }

        private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report rp = new Report(context);
            rp.Show();
        }

        private void добавитьСотрудникаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateEployee emp = new CreateEployee(context);
            emp.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test("458");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 1; i <= 1000; i++)
            {
                context.Journal.Add(new JournalRecord() { Date = DateTime.Now.Date, Employee = 457, In = DateTime.Now, Out = DateTime.Now });
            }
            context.SaveChanges();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }
}
