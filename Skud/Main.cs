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
using System.Management;

namespace Skud
{
    public partial class Main : Form
    {
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        public SqlConnectionStringBuilder sqlConnection;
        public JournalContext context;
        public Thread dataThread;
        public static bool connected = false;
        public static bool firstLoad = true;
        public Main()
        {
            InitializeComponent();
            dataThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    GetSqlServer();
                    context = new JournalContext(sqlConnection.ConnectionString);
                    context.Employees.Load();
                    context.Journal.Load();
                    context.Jobs.Load();
                    WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
                    ManagementEventWatcher watcher = new ManagementEventWatcher(query);
                    watcher.EventArrived += Watcher_EventArrived;
                    watcher.Start();
                    GetArduino();
                    Invoke((Action)(() => circularProgressBar1.Visible = false));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));
        }

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            GetArduino();
        }

        public void GetSqlServer()
        {
            try
            {
                string SqlServerName = string.Empty;
                bool IsConnected = false;
                sqlConnection = new SqlConnectionStringBuilder();
                sqlConnection.IntegratedSecurity = true;
                DataTable table = SmoApplication.EnumAvailableSqlServers(true);
                if (!config.AppSettings.Settings.AllKeys.Contains("SqlServerName"))
                {
                    if (table.Rows.Count > 1)
                    {
                        while (!IsConnected)
                        {
                            SqlServerName = Interaction.InputBox("Введите имя SQL сервера", "СКУД", "");
                            sqlConnection = new SqlConnectionStringBuilder();
                            sqlConnection.DataSource = SqlServerName;
                            IsConnected = IsServerConnected(sqlConnection.ConnectionString);
                        }
                    }
                    else
                        SqlServerName = table.Rows[0][0].ToString();
                    sqlConnection.DataSource = SqlServerName;
                    sqlConnection.InitialCatalog = "Journal";
                    config.AppSettings.Settings.Add("SqlServerName", SqlServerName);
                    config.Save(ConfigurationSaveMode.Modified);
                }
                else
                {
                    sqlConnection.DataSource = config.AppSettings.Settings["SqlServerName"].Value;
                    sqlConnection.InitialCatalog = "Journal";
                    sqlConnection.IntegratedSecurity = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static bool IsServerConnected(string connectionString)
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

        public static bool IsSerialPortConnected(string port)
        {
            try
            {
                SerialPort sp = new SerialPort(port);
                sp.Open();
                sp.Close();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        //public void GetAllSerialPorts()
        //{
        //    if (SerialPort.GetPortNames().Length > 0)
        //    {
        //        foreach (string port in SerialPort.GetPortNames())
        //        {
        //            ToolStripMenuItem comPort = new ToolStripMenuItem();
        //            comPort.Text = port;
        //            if (config.AppSettings.Settings.AllKeys.Contains("SerialPort") && config.AppSettings.Settings["SerialPort"].Value == port && IsSerialPortConnected(port))
        //            {
        //                comPort.Checked = true;
        //                ReadFromController(port);
        //            }
        //            else
        //                comPort.Checked = false;
        //            comPort.Click += comPort_Click;
        //            Invoke((Action)(() => PortToolStripMenuItem.DropDownItems.Add(comPort)));
        //        }
        //    }
        //    else
        //        MessageBox.Show("Устройство не подлючено", "СКУД", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //}

        private void comPort_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            //    foreach (ToolStripMenuItem item in PortToolStripMenuItem.DropDownItems)
            //    {
            //        item.Checked = false;
            //    }
            //    selectedItem.Checked = true;
            //    config.AppSettings.Settings.Remove("SerialPort");
            //    config.AppSettings.Settings.Add("SerialPort", selectedItem.Text);
            //    config.Save(ConfigurationSaveMode.Modified);
            //    if (selectedPort != null && selectedPort.IsOpen)
            //    {
            //        selectedPort.Dispose();
            //        selectedPort.Close();
            //    }
            //    ReadFromController(selectedItem.Text);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            dataThread.Start();
        }

        public void ReadFromController(string portName)
        {
            try
            {
                selectedPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                selectedPort.Open();
                selectedPort.DataReceived += SerialPortDataReceived;
            }
            catch (IOException)
            {
                MessageBox.Show("Устройство не подлючено", "СКУД", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Invoke((Action)(() =>
            {
                try
                {
                    string CardIdStr = selectedPort.ReadLine();
                    selectedPort.DiscardInBuffer();
                    if (CardIdStr.Length == 11)
                    {
                        long CardId = long.Parse(CardIdStr);
                        Employee employee = context.Employees.Where(emp => emp.CardId == CardId).FirstOrDefault();
                        if (employee == null)
                        {
                            CreateEployee emp = new CreateEployee(CardId, context);
                            emp.ShowDialog();
                        }
                        else
                        {
                            string status = AddJournalRecord(employee);
                            //string job = context.Jobs.Find(employee.JobId).JobDescription;
                            EmployeeInfo info = new EmployeeInfo(employee, status);
                            info.ShowDialog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));
        }

        //private void Test(string id)
        //{
        //    try
        //    {
        //        long CardId = long.Parse(id);
        //        Employee employee = context.Employees.Where(emp => emp.CardId == CardId).FirstOrDefault();
        //        if (employee == null)
        //        {
        //            CreateEployee emp = new CreateEployee(CardId, context);
        //            emp.ShowDialog();
        //        }
        //        else
        //        {
        //            string status = AddJournalRecord(employee.Id);
        //            string job = context.Jobs.Find(employee.JobId).JobDescription;
        //            EmployeeInfo info = new EmployeeInfo(employee, status, job);
        //            info.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        public string AddJournalRecord(Employee emp)
        {
            string status = string.Empty;
            try
            {
                JournalRecord record = new JournalRecord();
                record = context.Journal.Where(j => j.EmployeeId == emp.Id).ToList().LastOrDefault();
                if(record == null)
                {
                    context.Journal.Add(new JournalRecord() { EmployeeId = emp.Id, Date = DateTime.Now.Date, In = DateTime.Now });
                    emp.Status = true;
                    status = "Вход";
                }
                else if (record.Out != null) // сотрудник вышел
                {
                    context.Journal.Add(new JournalRecord() { EmployeeId = emp.Id, Date = DateTime.Now.Date, In = DateTime.Now }); // вход
                    emp.Status = true;
                    status = "Вход";
                }
                else // сотрудник вошел
                {
                    record.Out = DateTime.Now; // выход
                    emp.Status = false;
                    status = "Выход";
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Test("4576756775");
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    for(int i = 1; i <= 1000; i++)
        //    {
        //        context.Journal.Add(new JournalRecord() { Date = DateTime.Now.Date, EmployeeId = 457, In = DateTime.Now, Out = DateTime.Now });
        //    }
        //    context.SaveChanges();
        //}

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Employees emps = new Employees(context);
            emps.Show();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                selectedPort.Dispose();
                selectedPort.Close();
            }
            catch (Exception) { };
        }

        private void статусыСотрудниковToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmployeeState es = new EmployeeState(context);
            es.ShowDialog();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
        //    notifyIcon1.BalloonTipText = "I am a NotifyIcon Balloon";
        //    notifyIcon1.BalloonTipTitle = "Welcome Message";
        //    notifyIcon1.ShowBalloonTip(1000);
        //}

        //private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    PortToolStripMenuItem.DropDownItems.Clear();
        //    GetAllSerialPorts();
        //}
        public void GetArduino()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort");
                ManagementObjectCollection ports = searcher.Get();
                if ((ports.Count == 0 && connected) || (ports.Count == 0 && firstLoad))
                {
                    if (selectedPort != null)
                        selectedPort.Close();
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipText = firstLoad ? "Устройство не подключено" : "Устройство отключено";
                    notifyIcon1.BalloonTipTitle = "СКУД";
                    notifyIcon1.ShowBalloonTip(1000);
                    connected = false;
                    firstLoad = false;
                }
                else
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if ((string)queryObj["PNPDeviceID"] == "USB\\VID_2341&PID_0001\\75638303337351515270" && !connected)
                        {
                            ReadFromController((string)queryObj["DeviceID"]);
                            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                            notifyIcon1.BalloonTipText = "Устройство подключено";
                            notifyIcon1.BalloonTipTitle = "СКУД";
                            notifyIcon1.ShowBalloonTip(1000);
                            connected = true;
                            firstLoad = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
